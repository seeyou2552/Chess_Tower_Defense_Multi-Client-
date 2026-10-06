using System;
using System.Collections;
using System.Collections.Generic;


public class DefenseAttack
{
    public static void DefaultAttack(float damage, Minion minion, List<ChessPiece> targets)
    {
        SkillEffect skillObj;
        switch (minion.State.RuntimeStat.DeliveryType)
        {
            case AttackDeliveryType.Instant:
                foreach (var target in targets)
                {
                    if (target is Enemy enemy)
                    {
                        skillObj = SpawnManager.Instance.GetSkillObject(minion.transform.position);
                        skillObj.transform.position = target.transform.position;
                        skillObj.DefaultAttack(target as Enemy, minion);

                        enemy.TakeDamage(damage);
                    }
                    
                }
                break;

            case AttackDeliveryType.Projectile:
                foreach (var target in targets)
                {
                    if (target is Enemy enemy)
                    {
                        skillObj = SpawnManager.Instance.GetSkillObject(minion.transform.position);
                        skillObj.transform.position = minion.transform.position;
                        skillObj.DefaultAttack(target as Enemy, minion, true); // isProjectile true로 설정
                    }
                }
                break;
        }

        SoundManager.Instance.PlaySFX(minion.State.Data.AtkSFX);
    }

    public static void UseSkill(float damage, Minion minion, List<ChessPiece> targets)
    {
        ISkillStrategy skillStrategy = SkillStrategyFactory.GetSkillStrategy(minion.State.Data.Skill.SkillType);
        skillStrategy?.Execute(minion, targets, damage);

        SoundManager.Instance.PlaySFX(minion.State.Data.Skill.SkillSFX);
    }
    
}
