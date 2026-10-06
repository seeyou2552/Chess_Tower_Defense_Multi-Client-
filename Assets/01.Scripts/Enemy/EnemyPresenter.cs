using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPresenter
{
    private readonly Enemy _view;
    private readonly HPBarUI _hpBar;

    public EnemyState State { get; private set; }

    public EnemyPresenter(Enemy view)
    {
        _view = view;

        _hpBar = SpawnManager.Instance.GetHPBar();
        _hpBar.Init(_view);

        // _view.OnTakeDamage += 
    }

    public void Init(EnemyData enemyData)
    {
        EnsureReferences();

        if (State == null)
            State = new EnemyState();

        State.Setup(enemyData);
        State.OnDeath += OnEnemyDeath;

        _view.OnReturn += ResetForPool;

        _view.SetSprite(enemyData.Sprite);
        _view.ResetAnimation();

        _view.Movement.Init(enemyData.MoveSpeed);
        _view.Movement.OnEndMove += OnEndMove;

        State.DebuffController.OnStunChanged += OnStunChanged;
        State.DebuffController.OnDamageTick += TakeDamage;
        State.DebuffController.OnSlowChanged += _view.Movement.SpeedChanged;

        UpdateHPBar();
    }

    public void Update(float deltaTime)
    {
        if (State.IsDead)
            return;

        if (_view.Movement.IsStun)
            return;

        _view.Movement.MoveUpdate(deltaTime);
    }

    // 스킬 공격시 데미지 로직
    public void TakeDamage(float damage, MinionState minionState, List<OnHitEvent> hitEvents, List<OnKillEvent> killEvents)
    {
        if (minionState != null)
        {
            if (MinionTypeAbility.CanSpeedBonusDamage(minionState.Data.MinionType))
                damage = State.SpeedBonusDamage(_view.Movement.CurrentMoveSpeed, damage);

            State.TakeDamage(damage);

            if (hitEvents != null)
            {
                foreach (var e in hitEvents)
                    e.HitEvent.OnHit(minionState, _view, e);
            }

            if (State.IsDead && killEvents != null)
            {
                foreach (var e in killEvents)
                    e.KillEvent.OnKill(minionState, _view, e);
            }
        }
        
        UpdateHPBar();
    }

    // 일반 공격시 데미지 로직
    public void TakeDamage(float damage)
    {
        State.TakeDamage(damage);
        
        UpdateHPBar();
    }
    
    public void ExecuteOnLowHP(float executePercent)
    {
        State.ExecuteOnLowHP(executePercent);
        if (State.IsDead)
            UpdateHPBar();
    }

    public void ResetForPool()
    {
        State.OnDeath -= OnEnemyDeath;
        _view.OnReturn -= ResetForPool;
        _view.Movement.OnEndMove -= OnEndMove;

        CleanupDebuffs();

        State?.ResetState();
        _view.ResetAnimation();
    }

    private void OnEnemyDeath()
    {
        CleanupDebuffs();
        EventBus.Publish(new EnemyDeathEvent(State));
        _view.PlayDeathAnimation();
        _view.Movement.Stop();
    }

    private void EnsureReferences()
    {
        if (_view.Movement == null)
            _view.Movement = _view.GetComponent<EnemyMovement>();
    }

    private void UpdateHPBar()
    {
        if (!_hpBar)
            return;

        if (State == null)
        {
            Debug.LogError("[EnemyPresenter] State is null.");
            return;
        }

        if (State.Data == null)
        {
            Debug.LogError("[EnemyPresenter] State.Data is null.");
            return;
        }

        _hpBar.UpdateHPBar(State.CurrentHP, State.Data.MaxHP);
    }

    private void OnEndMove()
    {
        _view.ReturnToPool();
    }

#region  Debuffs

    private void CleanupDebuffs()
    {
        if (State.DebuffController != null)
        {
            State.DebuffController.OnStunChanged -= OnStunChanged;
            State.DebuffController.OnDamageTick -= TakeDamage;
            State.DebuffController.OnSlowChanged -= _view.Movement.SpeedChanged;

            State.DebuffController?.ClearDebuffs();
        }
    }

    public void TakeStun(float duration, EffectObject effectObj = null)
    {
        State.DebuffController?.ApplyStun(duration);
        if (effectObj != null)
            _view.StartCoroutine(PlayDebuffEffect(effectObj, duration));
    }

    public void TakeSlow(float value, float duration, EffectObject effectObj = null)
    {
        State.DebuffController?.ApplySlow(value, duration);
        if (effectObj != null)
            _view.StartCoroutine(PlayDebuffEffect(effectObj, duration));
    }

    public void TakeDamageDebuff(float damage, float duration, EffectObject effectObj = null)
    {
        if (!_view.gameObject.activeInHierarchy)
            return;

        State.DebuffController?.ApplyDamageDebuff(damage, duration);
        if (effectObj != null)
            _view.StartCoroutine(PlayDebuffEffect(effectObj, duration));
    }

    private IEnumerator PlayDebuffEffect(EffectObject effectObj, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            effectObj.transform.position = _view.transform.position + Vector3.up * 0.5f;

            if (State.IsDead || !_view.gameObject.activeSelf)
            {
                effectObj.ReturnToPool();
                yield break;
            }

            yield return null;
        }

        effectObj.ReturnToPool();
    }

    private void OnStunChanged(bool isStun)
    {
        _view.Movement.SetStun(isStun);
    }

#endregion

}


#region  EventBus

public readonly struct EnemyDeathEvent
{
    public readonly EnemyState EnemyState;

    public EnemyDeathEvent(EnemyState enemyState)
    {
        EnemyState = enemyState;
    }
};

#endregion