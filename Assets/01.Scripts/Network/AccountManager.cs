using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccountManager : Singleton<AccountManager>
{
    [Header("Account Text")]
    [SerializeField] private TMP_InputField _id;
    [SerializeField] private TMP_InputField _password;

    [Header("Account Button")]
    [SerializeField] private Button _loginBtn;
    [SerializeField] private Button _registerBtn;

    private const int IdSize = 32;
    private const int PasswordSize = 32;

    private void Start()
    {
        _loginBtn.onClick.AddListener(Login);
        _registerBtn.onClick.AddListener(Register);
    }

    public async void Login()
    {
        if (string.IsNullOrEmpty(_id.text))
        {
            Debug.LogError("Login failed: ID is empty.");
            return;
        }

        if (string.IsNullOrEmpty(_password.text))
        {
            Debug.LogError("Login failed: Password is empty.");
            return;
        }

        byte[] idBytes = Encoding.UTF8.GetBytes(_id.text);
        byte[] passwordBytes = Encoding.UTF8.GetBytes(_password.text);

        if (idBytes.Length > IdSize)
        {
            Debug.LogError(
                $"Login failed: ID is too long. Max bytes: {IdSize}"
            );
            return;
        }

        if (passwordBytes.Length > PasswordSize)
        {
            Debug.LogError(
                $"Login failed: Password is too long. Max bytes: {PasswordSize}"
            );
            return;
        }

        LoginRequest request = new LoginRequest
        {
            loginId = new byte[IdSize],
            password = new byte[PasswordSize]
        };

        Buffer.BlockCopy(
            idBytes,
            0,
            request.loginId,
            0,
            idBytes.Length
        );

        Buffer.BlockCopy(
            passwordBytes,
            0,
            request.password,
            0,
            passwordBytes.Length
        );

        try
        {
            Task<byte[]> responseTask =
                NetworkManager.Instance.WaitForResponseAsync(
                    PacketType.LoginResponse
                );

            bool sent =
                await NetworkManager.Instance.SendAsync(
                    PacketType.LoginRequest,
                    request
                );

            if (!sent)
            {
                Debug.LogError(
                    "Login Request Failed: not connected."
                );

                NetworkManager.Instance.CancelResponseWait(
                    PacketType.LoginResponse
                );

                return;
            }

            byte[] response = await responseTask;
            HandleLoginResponse(response);
        }
        catch (TimeoutException e)
        {
            Debug.LogError($"Login Response Timeout: {e.Message}");
        }
        catch (InvalidOperationException e)
        {
            Debug.LogError($"Login Response Failed: {e.Message}");
        }
        catch (Exception e)
        {
            Debug.LogError(
                $"Login Request Failed: {e.Message}"
            );
        }
    }

    public async void Register()
    {
        if (string.IsNullOrEmpty(_id.text))
        {
            Debug.LogError("Register failed: ID is empty.");
            return;
        }

        if (string.IsNullOrEmpty(_password.text))
        {
            Debug.LogError("Register failed: Password is empty.");
            return;
        }

        byte[] idBytes = Encoding.UTF8.GetBytes(_id.text);
        byte[] passwordBytes = Encoding.UTF8.GetBytes(_password.text);

        if (idBytes.Length > IdSize)
        {
            Debug.LogError(
                $"Register failed: ID is too long. Max bytes: {IdSize}"
            );
            return;
        }

        if (passwordBytes.Length > PasswordSize)
        {
            Debug.LogError(
                $"Register failed: Password is too long. Max bytes: {PasswordSize}"
            );
            return;
        }

        RegisterRequest request = new RegisterRequest
        {
            loginId = new byte[IdSize],
            password = new byte[PasswordSize]
        };

        Buffer.BlockCopy(
            idBytes,
            0,
            request.loginId,
            0,
            idBytes.Length
        );

        Buffer.BlockCopy(
            passwordBytes,
            0,
            request.password,
            0,
            passwordBytes.Length
        );

        try
        {
            Task<byte[]> responseTask =
                NetworkManager.Instance.WaitForResponseAsync(
                    PacketType.RegisterResponse
                );

            bool sent =
                await NetworkManager.Instance.SendAsync(
                    PacketType.RegisterRequest,
                    request
                );

            if (!sent)
            {
                Debug.LogError(
                    "Register Request Failed: not connected."
                );

                NetworkManager.Instance.CancelResponseWait(
                    PacketType.RegisterResponse
                );

                return;
            }

            byte[] response = await responseTask;
            HandleRegisterResponse(response);
        }
        catch (TimeoutException e)
        {
            Debug.LogError($"Register Response Timeout: {e.Message}");
        }
        catch (InvalidOperationException e)
        {
            Debug.LogError($"Register Response Failed: {e.Message}");
        }
        catch (Exception e)
        {
            Debug.LogError(
                $"Register Request Failed: {e.Message}"
            );
        }
    }

    public void HandleLoginResponse(byte[] payload)
    {
        try
        {
            LoginResponse response =
                NetworkManager.BytesToStruct<LoginResponse>(
                    payload
                );

            if (response.errCode != ErrorCode.None)
            {
                Debug.LogError(
                    $"Login Failed: {response.errCode}"
                );

                return;
            }

            string uuid = GetString(
                response.uuid
            );

            string accessToken = GetString(
                response.accessToken
            );

            Debug.Log(
                $"Login Success. " +
                $"UUID: {uuid}, " +
                $"PlayerId: {response.playerId}"
            );

            Debug.Log(
                $"AccessToken: {accessToken}"
            );

            // 로그인 성공 후 처리
            NetworkManager.Instance.AddAction(() =>
            {
                PlayerManager.Instance.SetPlayerId(response.playerId);

                GameManager.Instance.ChangeGameState(GameState.Lobby);
            });
        }
        catch (Exception e)
        {
            Debug.LogError(
                $"LoginResponse Parse Failed: {e.Message}"
            );
        }
    }

    public void HandleRegisterResponse(byte[] payload)
    {
        try
        {
            RegisterResponse response =
                NetworkManager.BytesToStruct<RegisterResponse>(
                    payload
                );

            if (response.errCode != ErrorCode.None)
            {
                Debug.LogError(
                    $"Register Failed: {response.errCode}"
                );

                return;
            }

            string uuid = GetString(
                response.uuid
            );

            Debug.Log(
                $"Register Success. UUID: {uuid}"
            );
        }
        catch (Exception e)
        {
            Debug.LogError(
                $"RegisterResponse Parse Failed: {e.Message}"
            );
        }
    }

    private string GetString(byte[] bytes)
    {
        if (bytes == null)
            return string.Empty;

        int length = Array.IndexOf(
            bytes,
            (byte)0
        );

        if (length < 0)
            length = bytes.Length;

        return Encoding.UTF8.GetString(
            bytes,
            0,
            length
        );
    }
}