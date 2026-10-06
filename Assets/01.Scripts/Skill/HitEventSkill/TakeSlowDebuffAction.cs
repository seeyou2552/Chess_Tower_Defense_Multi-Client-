using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "HitEvent/TakeSlowDebuff")]
public class TakeSlowDebuffAction : HitEvent
{
    public override void OnHit(MinionState minionState, Enemy enemy, OnHitEvent onHitEvent)
    {
        EffectObject effect;

        if (onHitEvent.ParticleObj != null)
            effect = SpawnManager.Instance.GetEffect(enemy.transform.position, onHitEvent.ParticleObj);

        else
        {
            effect = SpawnManager.Instance.GetEffect(enemy.transform.position);
            effect.LoopPlayEffect(onHitEvent.EffectAnim, onHitEvent.Duration);
        }

        enemy.TakeSlow(onHitEvent.Value, onHitEvent.Duration, effect);    
    }
}
