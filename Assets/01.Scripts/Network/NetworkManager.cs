using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class NetworkManager : Singleton<NetworkManager>
{
    [Header("Server")]
    [SerializeField] private string _serverHost = "3.37.127.47";
    [SerializeField] private int _serverPort = 7777;

    private TcpClient _client;
    private NetworkStream _stream;
    private CancellationTokenSource _cancellationTokenSource;

    private readonly ConcurrentQueue<Action> _actionQueue = new();
    private readonly ConcurrentDictionary<PacketType, Action<byte[]>>
        _packetHandlers = new();
    private readonly ConcurrentDictionary<PacketType, TaskCompletionSource<byte[]>>
        _pendingResponses = new();
    private readonly SemaphoreSlim _sendLock = new(1, 1);

    private bool _isConnected;

    private const int BufferSize = 4096;
    private const int HeaderSize = 4;
    private const int MaxPacketSize = ushort.MaxValue;

    public bool IsConnected => _isConnected;

    private void Start()
    {
        _ = ConnectAsync();
    }

    private void Update()
    {
        while (_actionQueue.TryDequeue(out Action action))
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }

    public void AddAction(Action action)
    {
        if (action != null)
            _actionQueue.Enqueue(action);
    }

    public void RegisterPacketHandler(
        PacketType packetType,
        Action<byte[]> handler
    )
    {
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        _packetHandlers[packetType] = handler;
    }

    public async Task<bool> ConnectAsync()
    {
        if (_isConnected)
            return true;

        try
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _client = new TcpClient();

            await _client.ConnectAsync(_serverHost, _serverPort);

            _stream = _client.GetStream();
            _isConnected = true;

            Debug.Log($"Connected to {_serverHost}:{_serverPort}");

            _ = ReceiveLoopAsync(_cancellationTokenSource.Token);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Connect Failed: {e.Message}");
            Disconnect();
            return false;
        }
    }

    private async Task ReceiveLoopAsync(CancellationToken cancellationToken)
    {
        byte[] tempBuffer = new byte[BufferSize];
        byte[] receiveBuffer = new byte[BufferSize];
        int bufferedSize = 0;

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                int bytesRead = await _stream.ReadAsync(
                    tempBuffer,
                    0,
                    tempBuffer.Length,
                    cancellationToken
                );

                if (bytesRead == 0)
                    break;

                if (bufferedSize + bytesRead > receiveBuffer.Length)
                {
                    int newSize = receiveBuffer.Length;

                    while (newSize < bufferedSize + bytesRead)
                        newSize *= 2;

                    Array.Resize(ref receiveBuffer, newSize);
                }

                Buffer.BlockCopy(
                    tempBuffer,
                    0,
                    receiveBuffer,
                    bufferedSize,
                    bytesRead
                );

                bufferedSize += bytesRead;
                ProcessPackets(receiveBuffer, ref bufferedSize);
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (IOException e)
        {
            Debug.LogWarning($"Receive Failed: {e.Message}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Receive Exception: {e}");
        }
        finally
        {
            FailPendingResponses(new IOException("Network connection was closed."));

            AddAction(() =>
            {
                _isConnected = false;
            });
        }
    }

    private void ProcessPackets(byte[] buffer, ref int bufferedSize)
    {
        int offset = 0;

        while (bufferedSize - offset >= HeaderSize)
        {
            ushort packetSize = BitConverter.ToUInt16(buffer, offset);
            ushort packetType = BitConverter.ToUInt16(buffer, offset + 2);

            if (packetSize < HeaderSize || packetSize > MaxPacketSize)
            {
                Debug.LogError($"Invalid Packet Size: {packetSize}");
                Disconnect();
                return;
            }

            if (bufferedSize - offset < packetSize)
                break;

            int payloadSize = packetSize - HeaderSize;
            byte[] payload = new byte[payloadSize];

            Buffer.BlockCopy(
                buffer,
                offset + HeaderSize,
                payload,
                0,
                payloadSize
            );

            HandlePacket(packetType, payload);
            offset += packetSize;
        }

        if (offset > 0)
        {
            Buffer.BlockCopy(
                buffer,
                offset,
                buffer,
                0,
                bufferedSize - offset
            );

            bufferedSize -= offset;
        }
    }

    private void HandlePacket(ushort packetType, byte[] payload)
    {
        PacketType type = (PacketType)packetType;

        if (_pendingResponses.TryRemove(
            type,
            out TaskCompletionSource<byte[]> response
        ))
        {
            response.TrySetResult(payload);
        }

        AddAction(() =>
        {
            Debug.Log(
                $"Received Packet: Type={packetType}, " +
                $"Payload={payload.Length} bytes"
            );

            if (_packetHandlers.TryGetValue(type, out Action<byte[]> handler))
            {
                handler(payload);
            }
        });
    }

    public async Task<byte[]> WaitForResponseAsync(
        PacketType packetType,
        int timeoutMilliseconds = 10000
    )
    {
        if (timeoutMilliseconds <= 0)
            throw new ArgumentOutOfRangeException(nameof(timeoutMilliseconds));

        if (!_isConnected)
            throw new InvalidOperationException("Network is not connected.");

        TaskCompletionSource<byte[]> response =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        if (!_pendingResponses.TryAdd(packetType, response))
        {
            throw new InvalidOperationException(
                $"Already waiting for response: {packetType}"
            );
        }

        try
        {
            Task completedTask = await Task.WhenAny(
                response.Task,
                Task.Delay(timeoutMilliseconds)
            );

            if (completedTask != response.Task)
            {
                _pendingResponses.TryRemove(packetType, out _);
                throw new TimeoutException(
                    $"Response timed out: {packetType}"
                );
            }

            return await response.Task;
        }
        catch
        {
            _pendingResponses.TryRemove(packetType, out _);
            throw;
        }
    }

    public void CancelResponseWait(PacketType packetType)
    {
        if (_pendingResponses.TryRemove(
            packetType,
            out TaskCompletionSource<byte[]> response
        ))
        {
            response.TrySetCanceled();
        }
    }

    private void FailPendingResponses(Exception exception)
    {
        foreach (KeyValuePair<PacketType, TaskCompletionSource<byte[]>> pending
            in _pendingResponses)
        {
            if (_pendingResponses.TryRemove(
                pending.Key,
                out TaskCompletionSource<byte[]> response
            ))
            {
                response.TrySetException(exception);
            }
        }
    }

    public async Task<bool> SendAsync(byte[] packet)
    {
        if (!_isConnected ||
            _stream == null ||
            packet == null ||
            packet.Length == 0)
        {
            return false;
        }

        await _sendLock.WaitAsync();

        try
        {
            await _stream.WriteAsync(packet, 0, packet.Length);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Send Failed: {e.Message}");
            return false;
        }
        finally
        {
            _sendLock.Release();
        }
    }

    public async Task<bool> SendAsync<T>(PacketType packetType, T structure)
    {
        byte[] payload;

        try
        {
            payload = StructToBytes(structure);
        }
        catch (Exception e)
        {
            Debug.LogError($"Packet Serialize Failed: {e.Message}");
            return false;
        }

        int packetSize = HeaderSize + payload.Length;

        if (packetSize > MaxPacketSize)
        {
            Debug.LogError($"Packet is too large: {packetSize}");
            return false;
        }

        byte[] packet = new byte[packetSize];

        Buffer.BlockCopy(
            BitConverter.GetBytes((ushort)packetSize),
            0,
            packet,
            0,
            sizeof(ushort)
        );

        Buffer.BlockCopy(
            BitConverter.GetBytes((ushort)packetType),
            0,
            packet,
            sizeof(ushort),
            sizeof(ushort)
        );

        Buffer.BlockCopy(payload, 0, packet, HeaderSize, payload.Length);
        return await SendAsync(packet);
    }

    public static byte[] StructToBytes<T>(T structure)
    {
        int size = Marshal.SizeOf(structure);
        IntPtr pointer = Marshal.AllocHGlobal(size);

        try
        {
            Marshal.StructureToPtr(structure, pointer, false);

            byte[] bytes = new byte[size];
            Marshal.Copy(pointer, bytes, 0, size);
            return bytes;
        }
        finally
        {
            Marshal.DestroyStructure(pointer, typeof(T));
            Marshal.FreeHGlobal(pointer);
        }
    }

    public static T BytesToStruct<T>(byte[] bytes)
    {
        int size = Marshal.SizeOf<T>();

        if (bytes == null || bytes.Length < size)
        {
            throw new ArgumentException(
                $"Invalid struct data. Expected={size}, " +
                $"Actual={bytes?.Length ?? 0}"
            );
        }

        IntPtr pointer = Marshal.AllocHGlobal(size);

        try
        {
            Marshal.Copy(bytes, 0, pointer, size);
            return Marshal.PtrToStructure<T>(pointer);
        }
        finally
        {
            Marshal.FreeHGlobal(pointer);
        }
    }

    public void Disconnect()
    {
        _isConnected = false;
        FailPendingResponses(new IOException("Network connection was closed."));

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;

        _stream?.Close();
        _stream = null;

        _client?.Close();
        _client = null;

        Debug.Log("Disconnected");
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Disconnect();
            _sendLock.Dispose();
        }
    }
}
