using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProjectileStrategyFactory
{
    // ProjectileHit
    private static readonly IProjectileHitStrategy _impact = new ImpactHitStrategy();
    private static readonly IProjectileHitStrategy _pierce = new PierceHitStrategy();

    // ProjectileMovement
    private static readonly IProjectileMovementStrategy _chase = new ChaseMovementStrategy();
    private static readonly IProjectileMovementStrategy _straight = new StraightMovementStrategy();

    public static IProjectileHitStrategy GetHitBehavior(ProjectileHitType type) => type switch
    {
        ProjectileHitType.Impact => _impact,
        ProjectileHitType.Pierce => _pierce,
        _ => new ImpactHitStrategy()
    };

    public static IProjectileMovementStrategy GetMovementBehavior(ProjectileType type) => type switch
    {
        ProjectileType.Chase => _chase,
        ProjectileType.Straight => _straight,
        _ => new StraightMovementStrategy()
    };
}
