using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SkillStrategyFactory
{
    private static readonly ISkillStrategy _impact = new ImpactSkillStrategy();
    private static readonly ISkillStrategy _projectile = new ProjectileSkillStrategy();
    private static readonly ISkillStrategy _aoe = new AOESkillStrategy();
    private static readonly ISkillStrategy _buff = new BuffSkillStrategy();

    public static ISkillStrategy GetSkillStrategy(SkillType type)
        => type switch
        {
            SkillType.Impact => _impact,
            SkillType.Projectile => _projectile,
            SkillType.AOE => _aoe,
            SkillType.Buff => _buff,
            _ => _impact
        };
}
