using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "HitEvent/Execution")]
public class executionAction : HitEvent
{
    public override void OnHit(MinionState minionState, Enemy enemy, OnHitEvent onHitEvent)
    {
        enemy.ExecuteOnLowHP(onHitEvent.Value);
    }
    
}
