using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class MinionManager : Singleton<MinionManager>
{
    private Dictionary<uint, Minion> _minionList = new();

    void Start()
    {
        NetworkManager.Instance.RegisterPacketHandler(
            PacketType.AttackNotify,
            HandleAttakcNotify
        );

        NetworkManager.Instance.RegisterPacketHandler(
            PacketType.UpgradeMinionNotify,
            UpgradeMinionNotify
        );

        NetworkManager.Instance.RegisterPacketHandler(
            PacketType.ReturnMinionNotify,
            ReturnMinionNotify
        );

        NetworkManager.Instance.RegisterPacketHandler(
            PacketType.RelocateMinionNotify,
            RelocateMinionNotify
        );
    }

    public void RegisterMinion(uint instanceId, Minion minion)
    {
        if (_minionList.ContainsKey(instanceId))
        {
            Debug.Log("이미 등록되어 있습니다.");
            return;
        }

        _minionList[instanceId] = minion;
    }

    public Minion GetMinion(uint instanceId)
    {
        if (!_minionList.ContainsKey(instanceId))
        {
            Debug.Log("등록되지 않은 id 입니다.");
            return null;
        }

        Minion minion = _minionList[instanceId];

        return minion;
    }

    public void RemoveMinion(uint instanceId)
    {
        if (!_minionList.ContainsKey(instanceId))
        {
            Debug.Log("등록되지 않은 id 입니다.");
        }

        _minionList.Remove(instanceId);
    }

    public async Task SendSpawnMinion(uint minionId, int x, int y)
    {
        SpawnMinionRequest request = new SpawnMinionRequest
        {
            minionId = minionId,
            x = x,
            y = y
        };

        try
        {
            Task<byte[]> responseTask =
                NetworkManager.Instance.WaitForResponseAsync(
                    PacketType.SpawnMinionResponse
                );

            bool sent = await NetworkManager.Instance.SendAsync(
                PacketType.SpawnMinionRequest,
                request
            );

            if (!sent)
            {
                Debug.LogError(
                    "Spawn Minion request failed: not connected."
                );

                NetworkManager.Instance.CancelResponseWait(
                    PacketType.SpawnMinionResponse
                );

                return;
            }

            byte[] response = await responseTask;
            HandleSpawnMinionResponse(response);

        }
        catch (Exception e)
        {
            Debug.LogError(
                $"Start Wave request failed: {e.Message}"
            );
        }
    }

    private void HandleSpawnMinionResponse(byte[] payload)
    {
        try
        {
            SpawnMinionResponse response =
                NetworkManager.BytesToStruct<SpawnMinionResponse>(
                    payload
                );

            if (response.errCode == ErrorCode.InvalidId)
            {
                Debug.Log("잘못된 id");
            }

            else if (response.errCode == ErrorCode.GoldNotEnough)
            {
                EventBus.Publish(new UIAlertEvent("골드가 부족합니다."));
            }

            else if (response.errCode == ErrorCode.InvalidSpawnPosition)
            {
                EventBus.Publish(new UIAlertEvent("잘못된 위치입니다."));
            }

            else if (response.errCode == ErrorCode.None)
            {
                StageManager.Instance.UpdateGold(response.currentGold);
            }

            
        }
        catch (Exception e)
        {
            Debug.LogError(
                $"Start Wave Parse Failed: {e.Message}"
            );
        }
    }

#region Attack

    private void HandleAttakcNotify(byte[] payload)
    {
        try
        {
            int offset = 0;

            // 공격 이벤트 개수
            int atkCount = BitConverter.ToInt32(payload, offset);
            offset += sizeof(int);

            if (atkCount < 0)
            {
                throw new Exception($"Invalid attack count: {atkCount}");
            }

            List<AttackEvent> attackEvents = new(atkCount);

            // 2. 공격 이벤트 순회
            for (int i = 0; i < atkCount; i++)
            {
                AttackEvent atkEvent = new();
                atkEvent.TargetInstanceIds = new List<uint>();

                // AttackType
                atkEvent.AtkType =
                    (AttackType)payload[offset];
                offset += sizeof(byte);

                // bool
                atkEvent.IsProjectile =
                    BitConverter.ToBoolean(payload, offset);
                offset += sizeof(bool);

                // Minion Instance ID
                atkEvent.MinionInstanceId =
                    BitConverter.ToUInt32(payload, offset);
                offset += sizeof(uint);

                // Damage
                atkEvent.damage =
                    BitConverter.ToInt32(payload, offset);
                offset += sizeof(int);

                // Target Count
                int targetCount =
                    BitConverter.ToInt32(payload, offset);
                offset += sizeof(int);

                if (targetCount < 0)
                {
                    throw new Exception(
                        $"Invalid target count: {targetCount}"
                    );
                }

                // Target Instance IDs
                for (int j = 0; j < targetCount; j++)
                {
                    uint targetInstanceId =
                        BitConverter.ToUInt32(payload, offset);

                    offset += sizeof(uint);

                    atkEvent.TargetInstanceIds.Add(targetInstanceId);
                }

                attackEvents.Add(atkEvent);
            }

            // 파싱한 공격 이벤트 처리
            NetworkManager.Instance.AddAction(()=>
            {
                foreach (AttackEvent atkEvent in attackEvents)
                {
                    Minion minion = GetMinion(atkEvent.MinionInstanceId);
                    if (minion == null)
                        continue;

                    List<ChessPiece> targetList = new();

                    // 버프의 경우 Minion을 Tartget
                    if (atkEvent.AtkType == AttackType.Buff)
                    {
                        foreach (uint minionId in atkEvent.TargetInstanceIds)
                        {
                            Minion target = GetMinion(minionId);
                            if (target == null)
                                continue;

                            targetList.Add(target);
                        }
                    }

                    // 그 외의 경우 Enemy를 Target
                    else
                    {
                        foreach (uint enemyId in atkEvent.TargetInstanceIds)
                        {
                            Enemy enemy = EnemyManager.Instance.GetEnemy(enemyId);
                            if (enemy == null)
                                continue;

                            targetList.Add(enemy);
                        }
                    }

                    // Projectile은 바로 데미지
                    if (atkEvent.IsProjectile)
                    {
                        foreach(Enemy enemy in targetList)
                        {
                            enemy.TakeDamage(atkEvent.damage);
                        }
                    }

                    else
                    {
                        minion.Attack(atkEvent.AtkType, targetList, atkEvent.damage);
                    }
                }
            });
            
        }
        catch (Exception e)
        {
            Debug.LogError(
                $"Attack Notify Parse Failed: {e.Message}"
            );
        }
    }

#endregion

#region Relocate

    public async Task SendRelocateMinion(uint minionId, int x, int y)
    {
        RelocateMinionRequest request = new RelocateMinionRequest
        {
            instanceId = minionId,
            x = x,
            y = y
        };

        try
        {
            Task<byte[]> responseTask =
                NetworkManager.Instance.WaitForResponseAsync(
                    PacketType.RelocateMinionResponse
                );

            bool sent = await NetworkManager.Instance.SendAsync(
                PacketType.RelocateMinionRequest,
                request
            );

            if (!sent)
            {
                Debug.LogError(
                    "Relocate Minion request failed: not connected."
                );

                NetworkManager.Instance.CancelResponseWait(
                    PacketType.RelocateMinionResponse
                );

                return;
            }

            byte[] response = await responseTask;
            HandleRelocateMinionResponse(response);

        }
        catch (Exception e)
        {
            Debug.LogError(
                $"Start Wave request failed: {e.Message}"
            );
        }
    }

    private void HandleRelocateMinionResponse(byte[] payload)
    {
        try
        {
            RelocateMinionResponse response =
                NetworkManager.BytesToStruct<RelocateMinionResponse>(
                    payload
                );

            if (response.errCode == ErrorCode.InvalidId)
            {
                Debug.Log("잘못된 id");
            }

            
        }
        catch (Exception e)
        {
            Debug.LogError(
                $"Relocate Minion Parse Failed: {e.Message}"
            );
        }
    }

    private void RelocateMinionNotify(byte[] payload)
    {
        try
        {
            RelocateMinionNotify notify =
                NetworkManager.BytesToStruct<RelocateMinionNotify>(
                    payload
                );


            NetworkManager.Instance.AddAction(()=>
            {
                Minion minion = GetMinion(notify.instanceId);
                if (!minion)
                    return;

                minion.SetPosition((float)notify.x / 2.0f, (float)notify.y / 2.0f);
            });
            

        }
        catch (Exception e)
        {
            Debug.LogError(
                $"Relocate Notify Parse Failed: {e.Message}"
            );
        }
    }

#endregion

#region Upgrade

    public async Task SendUpgradeMinion(uint minionId, int upgradeId)
    {
        UpgradeMinionRequest request = new UpgradeMinionRequest
        {
            instanceId = minionId,
            upgradeId = upgradeId
        };

        try
        {
            Task<byte[]> responseTask =
                NetworkManager.Instance.WaitForResponseAsync(
                    PacketType.UpgradeMinionResponse
                );

            bool sent = await NetworkManager.Instance.SendAsync(
                PacketType.UpgradeMinionRequest,
                request
            );

            if (!sent)
            {
                Debug.LogError(
                    "Spawn Minion request failed: not connected."
                );

                NetworkManager.Instance.CancelResponseWait(
                    PacketType.UpgradeMinionResponse
                );

                return;
            }

            byte[] response = await responseTask;
            HandleUpgradeMinionResponse(response);

        }
        catch (Exception e)
        {
            Debug.LogError(
                $"Start Wave request failed: {e.Message}"
            );
        }
    }

    private void HandleUpgradeMinionResponse(byte[] payload)
    {
        try
        {
            UpgradeMinionResponse response =
                NetworkManager.BytesToStruct<UpgradeMinionResponse>(
                    payload
                );

            if (response.errCode == ErrorCode.InvalidId)
            {
                Debug.Log("잘못된 id");
            }

            else if (response.errCode == ErrorCode.GoldNotEnough)
            {
                EventBus.Publish(new UIAlertEvent("골드가 부족합니다."));
            }

            else if (response.errCode == ErrorCode.None)
            {
                StageManager.Instance.UpdateGold(response.currentGold);
            }

            
        }
        catch (Exception e)
        {
            Debug.LogError(
                $"Upgarde Minion Parse Failed: {e.Message}"
            );
        }
    }

    private void UpgradeMinionNotify(byte[] payload)
    {
        try
        {
            UpgradeMinionNotify notify =
                NetworkManager.BytesToStruct<UpgradeMinionNotify>(
                    payload
                );


            NetworkManager.Instance.AddAction(()=>
            {
                Minion minion = GetMinion(notify.instanceId);
                if (!minion)
                    return;

                minion.State.UpgradeController.UpgradeLevelUp(notify.upgradeId);
            });
            

        }
        catch (Exception e)
        {
            Debug.LogError(
                $"Upgrade Notify Parse Failed: {e.Message}"
            );
        }
    }

#endregion

#region Return

    private void ReturnMinionNotify(byte[] payload)
    {
        try
        {
            ReturnMinionNotify notify =
                NetworkManager.BytesToStruct<ReturnMinionNotify>(
                    payload
                );

            NetworkManager.Instance.AddAction(()=>
            {
                Minion minion = GetMinion(notify.instanceId);
                if (!minion)
                    return;

                minion.ReturnToPool();
                RemoveMinion(notify.instanceId);
            });
            

        }
        catch (Exception e)
        {
            Debug.LogError(
                $"Remove Minion Notify Parse Failed: {e.Message}"
            );
        }
    }

#endregion

#region Sell

    public async Task SendSellMinion(uint minionId)
    {
        SellMinionRequest request = new SellMinionRequest
        {
            instanceId = minionId,
        };

        try
        {
            Task<byte[]> responseTask =
                NetworkManager.Instance.WaitForResponseAsync(
                    PacketType.SellMinionResponse
                );

            bool sent = await NetworkManager.Instance.SendAsync(
                PacketType.SellMinionRequest,
                request
            );

            if (!sent)
            {
                Debug.LogError(
                    "Sell Minion request failed: not connected."
                );

                NetworkManager.Instance.CancelResponseWait(
                    PacketType.SellMinionResponse
                );

                return;
            }

            byte[] response = await responseTask;
            SellMinionResponse(response);

        }
        catch (Exception e)
        {
            Debug.LogError(
                $"Start Wave request failed: {e.Message}"
            );
        }
    }

    private void SellMinionResponse(byte[] payload)
    {
        try
        {
            SellMinionResponse response =
                NetworkManager.BytesToStruct<SellMinionResponse>(
                    payload
                );

            if (response.errCode == ErrorCode.InvalidId)
            {
                Debug.Log("잘못된 id");
            }

            else if (response.errCode == ErrorCode.None)
            {
                StageManager.Instance.UpdateGold(response.currentGold);
            }

            
        }
        catch (Exception e)
        {
            Debug.LogError(
                $"Start Wave Parse Failed: {e.Message}"
            );
        }
    }

#endregion

}

public struct AttackEvent
{
    public AttackType AtkType;
    public bool IsProjectile;

    public uint MinionInstanceId;
    public List<uint> TargetInstanceIds;
    public int damage;
}