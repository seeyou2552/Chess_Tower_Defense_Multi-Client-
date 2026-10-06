using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectileMovementStrategy
{
    void UpdateMovement(SkillEffect projectile);
}

// 타겟 추적 이동 (Chase)
public class ChaseMovementStrategy : IProjectileMovementStrategy
{
    public void UpdateMovement(SkillEffect projectile)
    {
        if (projectile.Target == null || projectile.Target.IsDead)
        {
            projectile.ReturnToPool();
            return;
        }

        Vector2 dir = (projectile.Target.transform.position - projectile.transform.position).normalized;
        projectile.transform.position += (Vector3)(dir * projectile.Speed * Time.deltaTime);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}

// 고정 방향 직진 이동 (Straight)
public class StraightMovementStrategy : IProjectileMovementStrategy
{
    public void UpdateMovement(SkillEffect projectile)
    {
        if (projectile.Direction == Vector2.zero)
        {
            projectile.ReturnToPool();
            return;
        }

        float angle = Mathf.Atan2(projectile.Direction.y, projectile.Direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0, 0, angle);

        projectile.transform.position += (Vector3)projectile.Direction * projectile.Speed * Time.deltaTime;
    }
}
