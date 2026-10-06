using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MinionState
{
    public MinionData Data { get; private set; }
    public MinionRuntimeStat RuntimeStat { get; private set; }
    public BuffController BuffController { get; private set; }
    public UpgradeController UpgradeController { get; private set; }

    public int SellGold { get; private set; }
    public bool IsFirstWave { get; set; }
    public bool IsSelected { get; set; }
    public bool IsSiege { get; set; }

    public uint InstanceId { get; private set; }
    public int OwnerId { get; private set; }

    // 상태 변화를 알려줄 이벤트들
    public event Action OnStatChanged;

    // 풀에서 꺼내어 재사용할 때 호출할 함수
    public void Setup(MinionData data)
    {
        Data = data;

        RuntimeStat = RuntimeStat ?? new MinionRuntimeStat();

        RuntimeStat.ResetRuntimeStat(); // 기존 데이터 청소
        RuntimeStat.Init(data);         // 새 데이터로 덮어쓰기

        BuffController = BuffController ?? new BuffController(this);

        UpgradeController = UpgradeController ?? new UpgradeController(this);
        UpgradeController.Init();

        SellGold = data.Cost;
        IsFirstWave = true;
        IsSelected = false;
        IsSiege = false;
    }

    public void SetInstanceId(uint instanceId)
    {
        InstanceId = instanceId;
    }

    public void SetOwnerId(int ownerId)
    {
        OwnerId = ownerId;
    }

    public void WaveEndReset()
    {
        RuntimeStat.ResetCurrentAC();
        OnStatChanged?.Invoke();
    }

    public void SellGoldUpdate(int goldValue)
    {
        SellGold += goldValue;
        OnStatChanged?.Invoke();
    }

    public void NotifyStatChanged()
    {
        OnStatChanged?.Invoke();
    }
}
