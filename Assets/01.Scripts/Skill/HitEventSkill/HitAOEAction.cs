using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "HitEvent/AOE")]
public class HitAOEAction : HitEvent
{
    public float Range = 2f;
    public override void OnHit(MinionState minionState, Enemy enemy, OnHitEvent onHitEvent)
    {
        EffectObject effectObj = SpawnManager.Instance.GetEffect(enemy.transform.position);
        // effectObj.GetComponent<SpriteRenderer>().sprite = onHitEvent.effectSprite;
        
        // Animation 실행
        if (onHitEvent.EffectAnim != null)
        {
            effectObj.PlayEffect(onHitEvent.EffectAnim);
            SoundManager.Instance.PlaySFX(onHitEvent.AudioClip);
        }
        
        // effect 객체 크기를 range만큼으로 설정
        effectObj.transform.localScale = Vector3.one * Range * 2;
        
        // 범위 내 주변 enemy 탐지
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(enemy.transform.position, Range);
        foreach (Collider2D hitCollider in hitColliders)
        {
            Enemy targetEnemy = hitCollider.GetComponent<Enemy>();
            if (targetEnemy != null)
            {
                targetEnemy.TakeDamage(onHitEvent.Value);
            }
        }
    }
    
}
