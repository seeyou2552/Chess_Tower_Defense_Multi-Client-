using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectileHitStrategy
{
    void ProcessHit(SkillEffect projectile, Enemy enemy);
}

// 단일 타격 후 소멸 (Impact)
public class ImpactHitStrategy : IProjectileHitStrategy
{
    public void ProcessHit(SkillEffect projectile, Enemy enemy)
    {
        // 유도탄인데 원래 조준한 타겟이 아니면 통과
        if (projectile.Target != null && projectile.Target != enemy) 
            return;

        projectile.ApplyDamageTo(enemy);
        projectile.ReturnToPool(); // 충돌 후 풀로 반환
    }
}

// 관통 타격 (Pierce)
public class PierceHitStrategy : IProjectileHitStrategy
{
    public void ProcessHit(SkillEffect projectile, Enemy enemy)
    {
        projectile.ApplyDamageTo(enemy);
        // 관통은 소멸하지 않고 통과함 (지속시간 만료로 소멸)
    }
}
