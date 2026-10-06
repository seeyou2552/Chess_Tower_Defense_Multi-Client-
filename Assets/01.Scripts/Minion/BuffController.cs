using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffController
{
    private readonly MinionState _state;
    private readonly MinionRuntimeStat _runtimeStat;
    private Dictionary<int, Coroutine> _buffRemoveCoroutines = new(); // 버프별 제거 코루틴 관리
    private Dictionary<int, ChangeDefaultAttack> _changeDefaultAttackOptions = new(); // 기본 공격 변경 옵션들

    public BuffController(MinionState state)
    {
        _state = state;
        _runtimeStat = state?.RuntimeStat;
    }

    public void ApplyBuff(Buff buff)
    {
        // BuffData의 타입에 따라 적절한 메서드 호출

        switch (buff.Data)
        {
            case MaxAtkTargetBuff _:
                ApplyAtkMaxTargetBuff(buff);
                break;

            case AtkSpeedBuff _:
                ApplyAtkSpeedBuff(buff);
                break;

            case PowerUpBuff _:
                ApplyPowerUpBuff(buff);
                break;
        }

    }

    private IEnumerator RemoveBuffAfterDuration(int buffId, Buff buff)
    {
        yield return YieldCache.GetWaitForSeconds(buff.Duration);

        switch (buff.Data)
        {
            case MaxAtkTargetBuff _:
                RemoveAtkMaxTargetBuff((int)buff.Increase, buff.DefaultAttackData, buffId, buff);
                break;

            case AtkSpeedBuff _:
                RemoveAttackSpeedBuff(buff.Increase, buff.DefaultAttackData, buffId, buff);
                break;

            case PowerUpBuff _:
                RemovePowerUpBuff(buff.Increase, buff.DefaultAttackData, buffId, buff);
                break;
        }

    }

    public void ClearBuffList()
    {
        foreach (var coroutine in _buffRemoveCoroutines.Values)
        {
            CoroutineRunner.Instance.StopCoroutine(coroutine);
        }
        _buffRemoveCoroutines.Clear();

        _runtimeStat?.ClearBuffs();
        _changeDefaultAttackOptions.Clear();
        _runtimeStat?.ResetDefaultAttack();
        _state?.NotifyStatChanged();
    }

#region PowerUpBuff

    private void ApplyPowerUpBuff(Buff buff)
    {
        int buffId = buff.GetHashCode();

        // 기존 같은 버프의 코루틴이 있으면 중지 (시간 초기화)
        if (_buffRemoveCoroutines.ContainsKey(buffId))
        {
            CoroutineRunner.Instance.StopCoroutine(_buffRemoveCoroutines[buffId]);
            _buffRemoveCoroutines.Remove(buffId);
        }

        AddPowerUpBuff(buff.Increase, buff.DefaultAttackData, buffId, buff);

        // 제거 코루틴 시작
        Coroutine removeCoroutine = CoroutineRunner.Instance.StartCoroutine(RemoveBuffAfterDuration(buffId, buff));
        _buffRemoveCoroutines[buffId] = removeCoroutine;
    }

    private void AddPowerUpBuff(float increase, ChangeDefaultAttack option = null, int buffId = 0, Buff buff = null)
    {
        _runtimeStat.PowerBuffs.Add(increase);
        if (option != null)
        {
            _changeDefaultAttackOptions[buffId] = option;
            _runtimeStat.ChangeDefaultAttack(option);
        }
        if (buff != null)
        {
            _runtimeStat.ChangedBuffValue(buff);
        }
        _state?.NotifyStatChanged();
    }

    private void RemovePowerUpBuff(float increase, ChangeDefaultAttack option = null, int buffId = 0, Buff buff = null)
    {
        _runtimeStat.PowerBuffs.Remove(increase);
        if (option != null)
        {
            _changeDefaultAttackOptions.Remove(buffId);
            
            if (_changeDefaultAttackOptions.Count > 0)
            {
                var lastOption = _changeDefaultAttackOptions.Values.Last();
                _runtimeStat.ChangeDefaultAttack(lastOption);
            }
            else
            {
                _runtimeStat.ResetDefaultAttack();
            }
        }
        if (buff != null)
        {
            _runtimeStat.ChangedBuffValue(buff);
        }
        _state?.NotifyStatChanged();
    }

#endregion

#region AtkSpeedBuff

    private void ApplyAtkSpeedBuff(Buff buff)
    {
        int buffId = buff.GetHashCode();

        // 기존 같은 버프의 코루틴이 있으면 중지
        if (_buffRemoveCoroutines.ContainsKey(buffId))
        {
            CoroutineRunner.Instance.StopCoroutine(_buffRemoveCoroutines[buffId]);
            _buffRemoveCoroutines.Remove(buffId);
        }

        AddAttackSpeedBuff(buff.Increase, buff.DefaultAttackData, buffId, buff);

        // 제거 코루틴 시작
        Coroutine removeCoroutine = CoroutineRunner.Instance.StartCoroutine(RemoveBuffAfterDuration(buffId, buff));
        _buffRemoveCoroutines[buffId] = removeCoroutine;
    }

    private void AddAttackSpeedBuff(float increase, ChangeDefaultAttack option = null, int buffId = 0, Buff buff = null)
    {
        _runtimeStat.AtkSpeedBuffs.Add(increase);
        if (option != null)
        {
            _changeDefaultAttackOptions[buffId] = option;
            _runtimeStat.ChangeDefaultAttack(option);
        }
        if (buff != null)
        {
            _runtimeStat.ChangedBuffValue(buff);
        }
        _state?.NotifyStatChanged();
    }

    private void RemoveAttackSpeedBuff(float increase, ChangeDefaultAttack option = null, int buffId = 0, Buff buff = null)
    {
        _runtimeStat.AtkSpeedBuffs.Remove(increase);
        if (option != null)
        {
            _changeDefaultAttackOptions.Remove(buffId);
            
            if (_changeDefaultAttackOptions.Count > 0)
            {
                var lastOption = _changeDefaultAttackOptions.Values.Last();
                _runtimeStat.ChangeDefaultAttack(lastOption);
            }
            else
            {
                _runtimeStat.ResetDefaultAttack();
            }
        }
        if (buff != null)
        {
            _runtimeStat.ChangedBuffValue(buff);
        }
        _state?.NotifyStatChanged();
    }

#endregion


#region MaxAtkBuff

    private void ApplyAtkMaxTargetBuff(Buff buff)
    {
        int buffId = buff.GetHashCode();

        // 기존 같은 버프의 코루틴이 있으면 중지 (시간 초기화)
        if (_buffRemoveCoroutines.ContainsKey(buffId))
        {
            CoroutineRunner.Instance.StopCoroutine(_buffRemoveCoroutines[buffId]);
            _buffRemoveCoroutines.Remove(buffId);
        }

        AddAtkMaxTargetBuff((int)buff.Increase, buff.DefaultAttackData, buffId, buff);

        // 제거 코루틴 시작
        Coroutine removeCoroutine = CoroutineRunner.Instance.StartCoroutine(RemoveBuffAfterDuration(buffId, buff));
        _buffRemoveCoroutines[buffId] = removeCoroutine;
    }

    private void AddAtkMaxTargetBuff(int increase, ChangeDefaultAttack option = null, int buffId = 0, Buff buff = null)
    {
        _runtimeStat.AtkMaxTargetBuff = increase;
        
        if (option != null)
        {
            _changeDefaultAttackOptions[buffId] = option;
            _runtimeStat.ChangeDefaultAttack(option);
        }
        if (buff != null)
        {
            _runtimeStat.ChangedBuffValue(buff);
        }
        _state?.NotifyStatChanged();
        
    }

    private void RemoveAtkMaxTargetBuff(int decrease, ChangeDefaultAttack option = null, int buffId = 0, Buff buff = null)
    {
        _runtimeStat.AtkMaxTargetBuff = 0;
        if (option != null)
        {
            _changeDefaultAttackOptions.Remove(buffId);
            
            if (_changeDefaultAttackOptions.Count > 0)
            {
                var lastOption = _changeDefaultAttackOptions.Values.Last();
                _runtimeStat.ChangeDefaultAttack(lastOption);
            }
            else
            {
                _runtimeStat.ResetDefaultAttack();
            }
        }
        if (buff != null)
        {
            _runtimeStat.ChangedBuffValue(buff);
        }
        _state?.NotifyStatChanged();
    }

#endregion
}
