using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : PoolObject, ChessPiece
{
    [Header("Script")]
    public EnemyMovement Movement;

    private EnemyPresenter _presenter;
    private Animator _anim;
    private SpriteRenderer _sr;
    public event Action OnTakeDamage;
    public event Action OnReturn;

    public uint InstanceId { get; private set; }

    public bool IsDead => _presenter?.State.IsDead ?? true;

    void Awake()
    {
        _presenter = new EnemyPresenter(this);

        if (Movement == null)
            Movement = GetComponent<EnemyMovement>();

        if(_anim == null)
            _anim = GetComponent<Animator>();

        if (_sr == null)
            _sr = GetComponent<SpriteRenderer>();

        _anim.enabled = false;
    }

    public void Init(EnemyData enemyData)
    {
        if (_sr == null)
            _sr = GetComponent<SpriteRenderer>();

        _presenter?.Init(enemyData);
    }
    
    public void TakeDamage(
        float damage, 
        MinionState minionState = null, 
        List<OnHitEvent> hitEvents = null, 
        List<OnKillEvent> killEvents = null)
    {
        // 기본 공격 처리
        if (minionState == null)
            _presenter.TakeDamage(damage);

        // 스킬 공격 처리
        else 
            _presenter.TakeDamage(damage, minionState, hitEvents, killEvents);
    }
    
    public void ExecuteOnLowHP(float executePercent)
    {
        _presenter?.ExecuteOnLowHP(executePercent);
    }

    public void SetSprite(Sprite sprite)
    {
        _sr.sprite = sprite;
    }

    public void ResetAnimation()
    {
        if (_anim != null)
            _anim.enabled = false;
    }

    public void PlayDeathAnimation()
    {
        if (_anim == null)
            return;

        _anim.enabled = true;
        _anim.Play("Destroy");
    }

    public void SetInstanceId(uint instanceId) { InstanceId = instanceId; }

#region Debuff
    public void TakeStun(float duration, EffectObject effectObj = null)
    {
        _presenter?.TakeStun(duration, effectObj);
    }

    public void TakeSlow(float value, float duration, EffectObject effectObj = null)
    {
        _presenter?.TakeSlow(value, duration, effectObj);
    }

    public void TakeDamageDebuff(float damage, float duration, EffectObject effectObj = null)
    {
        _presenter?.TakeDamageDebuff(damage, duration, effectObj);
    }

#endregion

#region Return

    public override void ReturnToPool()
    {
        if (!gameObject.activeSelf)
            return;

        OnReturn?.Invoke();
        Movement.ClearMovement();
        ObjectPoolManager.Instance.Return(gameObject.name, this);
    }

    void Update()
    {
        _presenter?.Update(Time.deltaTime);
    }

    // DestroyEnemy 애니메이션에서 자체 호출
    public void OnDestroyAnimEnd()
    {
        ReturnToPool();
    }

#endregion
}
