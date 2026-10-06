using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

public class EnemyManager : Singleton<EnemyManager>
{
    private Dictionary<uint, Enemy> _enemyList = new();

    void Start()
    {
        NetworkManager.Instance.RegisterPacketHandler(
            PacketType.ArrivalEnemyNotify,
            ArrivalEnemyNotify
        );
    }

    public void RegisterEnemy(uint instanceId, Enemy enemy)
    {
        if (_enemyList.ContainsKey(instanceId))
        {
            Debug.Log("이미 등록되어 있습니다.");
            return;
        }

        _enemyList[instanceId] = enemy;
    }

    public Enemy GetEnemy(uint instanceId)
    {
        if (!_enemyList.ContainsKey(instanceId))
        {
            Debug.Log("등록되지 않은 id 입니다.");
            return null;
        }

        Enemy enemy = _enemyList[instanceId];

        return enemy;
    }

    public void RemoveEnemy(uint instanceId)
    {
        if (!_enemyList.ContainsKey(instanceId))
        {
            Debug.Log("등록되지 않은 id 입니다.");
        }

        _enemyList.Remove(instanceId);
    }


    private void ArrivalEnemyNotify(byte[] payload)
    {
        try
        {
            ArrivalEnemyNotify notify =
                NetworkManager.BytesToStruct<ArrivalEnemyNotify>(
                    payload
                );

            NetworkManager.Instance.AddAction(()=>
            {
                Enemy enemy = GetEnemy(notify.instanceId);
                if (!enemy)
                    return;

                RemoveEnemy(notify.instanceId);
                enemy.ReturnToPool();
                StageManager.Instance.UpdateHp(notify.wallHp);
            });

            
        }
        catch (Exception e)
        {
            Debug.LogError(
                $"Arrival Enemy Parse Failed: {e.Message}"
            );
        }
    }
}
