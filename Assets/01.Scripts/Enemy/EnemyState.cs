using System;

public class EnemyState
{
    public EnemyData Data { get; private set; }
    public DebuffController DebuffController { get; private set; }

    public float CurrentHP { get; private set; }
    public bool IsDead { get; private set; }

    public event Action OnDeath;

    public void Setup(EnemyData data)
    {
        DebuffController = DebuffController ?? new DebuffController();

        Data = data;
        CurrentHP = data.MaxHP;
        IsDead = false;
    }

    public bool TakeDamage(float damage)
    {
        if (IsDead)
            return false;

        CurrentHP -= damage;

        if (CurrentHP <= 0f)
        {
            CurrentHP = 0f;
            Die();
            return true;
        }

        return false;
    }

    public float SpeedBonusDamage(float moveSpeed, float damage)
    {
        float bonusPercent = moveSpeed * 10f;
        return damage * (1f + bonusPercent / 100f);
    }

    public void ExecuteOnLowHP(float executePercent)
    {
        if (IsDead)
            return;

        if (CurrentHP <= Data.MaxHP * (executePercent / 100f))
            Die();
    }

    public void ResetState()
    {
        Data = null;
        IsDead = true;
    }

    private void Die()
    {
        // 이미 Die 처리가 되어있다면 return
        if (IsDead)
            return;

        IsDead = true;
        OnDeath?.Invoke();
    }
}
