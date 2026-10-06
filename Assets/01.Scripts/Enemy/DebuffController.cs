using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy의 디버프를 관리하는 컨트롤러
/// 스턴, 슬로우 등의 디버프 상태를 계산하고 이벤트를 발행
/// </summary>
public class DebuffController
{
    public bool IsStun { get; private set; }
    public float SlowPercent { get; private set; }

    public event Action<bool> OnStunChanged;
    public event Action<float> OnSlowChanged;
    public event Action<float> OnDamageTick;

    private Dictionary<int, float> _slowDebuffs = new Dictionary<int, float>();
    private Dictionary<int, Coroutine> _slowCoroutines = new Dictionary<int, Coroutine>();
    private int _slowDebuffCounter = 0;

    private Coroutine _stunCoroutine;
    private Coroutine _stunIgnoreCoroutine;
    private float _stunIgnoreDuration = 1f;

    private Dictionary<int, Coroutine> _damageDebuffCoroutines = new Dictionary<int, Coroutine>();
    private int _damageDebuffCounter = 0;

    public DebuffController()
    {
        IsStun = false;
        SlowPercent = 0f;
    }

    public void ClearDebuffs()
    {
        if (_stunCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(_stunCoroutine);
            _stunCoroutine = null;
        }

        if (_stunIgnoreCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(_stunIgnoreCoroutine);
            _stunIgnoreCoroutine = null;
        }

        foreach (var coroutine in _slowCoroutines.Values)
        {
            if (coroutine != null)
                CoroutineRunner.Instance.StopCoroutine(coroutine);
        }

        _slowCoroutines.Clear();
        _slowDebuffs.Clear();

        foreach (var coroutine in _damageDebuffCoroutines.Values)
        {
            if (coroutine != null)
                CoroutineRunner.Instance.StopCoroutine(coroutine);
        }

        _damageDebuffCoroutines.Clear();

        SetStun(false);
        SetSlowPercent(0f);
    }

#region Slow

    public void ApplySlow(float percent, float duration)
    {
        int slowId = _slowDebuffCounter++;
        Coroutine coroutine = CoroutineRunner.Instance.StartCoroutine(SlowCoroutine(slowId, percent, duration));
        _slowCoroutines[slowId] = coroutine;
    }

    private IEnumerator SlowCoroutine(int slowId, float percent, float duration)
    {
        _slowDebuffs[slowId] = percent;
        RecalculateSlowPercent();

        yield return YieldCache.GetWaitForSeconds(duration);

        // Slow 끝날 때 속도 재계산
        _slowDebuffs.Remove(slowId);
        RecalculateSlowPercent();

        if (_slowCoroutines.ContainsKey(slowId))
            _slowCoroutines.Remove(slowId);
    }

    private void RecalculateSlowPercent()
    {
        float multiplier = 1f;
        foreach (var value in _slowDebuffs.Values)
            multiplier *= (1f - value / 100f);

        SetSlowPercent((1f - multiplier) * 100f);
    }

    private void SetSlowPercent(float percent)
    {
        SlowPercent = Mathf.Clamp(percent, 0f, 100f);
        OnSlowChanged?.Invoke(SlowPercent);
    }

#endregion

#region Stun

    public void ApplyStun(float duration)
    {
        if (_stunIgnoreCoroutine != null || _stunCoroutine != null)
            return;

        _stunCoroutine = CoroutineRunner.Instance.StartCoroutine(StunCoroutine(duration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        SetStun(true);

        yield return YieldCache.GetWaitForSeconds(duration);

        SetStun(false);
        _stunCoroutine = null;

        _stunIgnoreCoroutine = CoroutineRunner.Instance.StartCoroutine(StunIgnoreCoroutine(_stunIgnoreDuration));
    }

    private IEnumerator StunIgnoreCoroutine(float cooldownDuration)
    {
        yield return YieldCache.GetWaitForSeconds(cooldownDuration);
        _stunIgnoreCoroutine = null;
    }

    private void SetStun(bool value)
    {
        if (IsStun == value)
            return;

        IsStun = value;
        OnStunChanged?.Invoke(IsStun);
    }

#endregion

#region Damage

    public void ApplyDamageDebuff(float damage, float duration)
    {
        Debug.Log("Damage Debuff");
        int damageId = _damageDebuffCounter++;
        Coroutine coroutine = CoroutineRunner.Instance.StartCoroutine(DamageDebuffCoroutine(damageId, damage, duration));
        _damageDebuffCoroutines[damageId] = coroutine;
    }

    private IEnumerator DamageDebuffCoroutine(int damageId, float damage, float duration)
    {
        float damageTimer = 0f;

        while (damageTimer <= duration)
        {
            yield return YieldCache.GetWaitForSeconds(1f);
            damageTimer += 1f;
            OnDamageTick?.Invoke(damage);
        }

        if (_damageDebuffCoroutines.ContainsKey(damageId))
            _damageDebuffCoroutines.Remove(damageId);
    }

#endregion
}
