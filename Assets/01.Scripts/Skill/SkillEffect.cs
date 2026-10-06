using System;
using System.Collections;
using UnityEngine;

public class SkillEffect : PoolObject
{
    // 인터페이스 변수
    private IProjectileMovementStrategy _movementBehavior;
    private IProjectileHitStrategy _hitBehavior;

    // 프로퍼티
    public Enemy Target { get; private set; }
    public Vector2 Direction { get; set; }
    public float Speed { get; private set; }
    public float Damage { get; private set; }
    public Minion OwnerMinion { get; private set; }
    public SkillData SkillData { get; private set; }

    [Header("Projectile Setting")]
    private bool _isProjectile = false;

    // Components
    private CircleCollider2D _cr;
    private SpriteRenderer _sr;
    public AnimationClipController _acc;

    private Coroutine _returnCoroutine;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _cr = GetComponent<CircleCollider2D>();
        _acc = GetComponent<AnimationClipController>();
    }

    void OnEnable()
    {
        EventBus.Subscribe<PoolReturn>(OnReturnSkillEffect);
    }
    void OnDisable()
    {
        EventBus.Unsubscribe<PoolReturn>(OnReturnSkillEffect);
    }

    void Update()
    {
        if (!_isProjectile) return;

        _movementBehavior?.UpdateMovement(this);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        
        // if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        // {
        //     if (collider.TryGetComponent<Enemy>(out var enemy))
        //     {
                
        //         _hitBehavior?.ProcessHit(this, enemy);
        //     }
        // }
    }

    
    public void DefaultAttack(Enemy target, Minion minion, bool isProjectile = false)
    {
        Target = target;
        OwnerMinion = minion;
        _isProjectile = isProjectile;

        if (isProjectile)
        {
            SetupBehaviors(minion.State.RuntimeStat.ProjectileType, minion.State.RuntimeStat.ProjectileHitType);
            if (minion.State.RuntimeStat.ProjectileType == ProjectileType.Straight)
                InitStraightDirection(target, minion.State.RuntimeStat.ProjectileDuration);
        }

        ApplyDefaultSettings(
            minion.State.RuntimeStat.CurrentPower,
            minion.State.RuntimeStat.ProjectileSpeed,
            minion.State.RuntimeStat.AtkSprite,
            minion.State.RuntimeStat.AtkAnimClip
        );
    }

    
    public void SkillInit(Enemy target, Minion minion, SkillData data, float damage, bool isProjectile = false)
    {
        Debug.Log("Skill Active");
        SkillData = data;
        Target = target;
        OwnerMinion = minion;
        _isProjectile = isProjectile;

        transform.localScale = Vector3.one * Mathf.Max(SkillData.SkillScale, 1f);

        if (isProjectile)
        {
            SetupBehaviors(SkillData.ProjectileType, SkillData.ProjectileHitType);
            if (SkillData.ProjectileType == ProjectileType.Straight)
                InitStraightDirection(target, SkillData.ProjectileDuration);
        }

        ApplyDefaultSettings(damage, SkillData.ProjectileSpeed, SkillData.SkillSprite, SkillData.SkillAnimClip);
    }

    private void SetupBehaviors(ProjectileType moveType, ProjectileHitType hitType)
    {
        _cr.enabled = true;
        _movementBehavior = ProjectileStrategyFactory.GetMovementBehavior(moveType);
        _hitBehavior = ProjectileStrategyFactory.GetHitBehavior(hitType);
    }

    private void InitStraightDirection(Enemy target, float duration)
    {
        Direction = (target.transform.position - transform.position).normalized;
        Target = null; // 직선형은 타겟 추적을 끊음
        _returnCoroutine = StartCoroutine(ReturnCoroutine(duration));
    }

    private void ApplyDefaultSettings(float damage, float speed, Sprite sprite, AnimationClip animClip)
    {
        Damage = damage;
        Speed = speed;

        if (sprite != null) _sr.sprite = sprite;

        if (animClip != null)
        {
            if (SkillData != null && SkillData.SkillType == SkillType.Projectile)
                _acc.PlayAnimation(animClip);
            else if (!_isProjectile)
                _acc.PlayAnimation(animClip, ReturnToPool);
            else
                _acc.LoopPlayAnimation(animClip, ReturnToPool);
        }
    }

    // 데미지 처리를 위한 공통 메서드
    public void ApplyDamageTo(Enemy enemy)
    {
        if (SkillData != null)
            enemy.TakeDamage(Damage, OwnerMinion.State, SkillData.HitEvents, SkillData.KillEvents);
        else
            enemy.TakeDamage(Damage);
    }

#region Return
    public override void ReturnToPool()
    {
        if (_returnCoroutine != null)
        {
            StopCoroutine(_returnCoroutine);
            _returnCoroutine = null;
        }

        // 초기화 코드
        Target = null;
        _isProjectile = false;
        Speed = 0f;
        Damage = 0;
        Direction = Vector2.zero;
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;

        _acc.RestoreDefault();
        _sr.sprite = null;
        _cr.enabled = false;
        SkillData = null;
        _movementBehavior = null;
        _hitBehavior = null;

        ObjectPoolManager.Instance.ReturnSkillEffect(this);
    }

    private IEnumerator ReturnCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        ReturnToPool();
    }

#endregion

    private void OnReturnSkillEffect(PoolReturn e) => ReturnToPool();
}