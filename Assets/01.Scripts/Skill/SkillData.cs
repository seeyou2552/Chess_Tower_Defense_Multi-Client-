using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TD/SkillData")]
public class SkillData : ScriptableObject
{
    [Header("ID")]
    public int ID;
    
    [Header("Visuals")]
    public Sprite SkillSprite;
    public AnimationClip SkillAnimClip;
    public AudioClip SkillSFX;
    public string SkillName;
    [TextArea(2, 4)]
    public string Description;
    
    [Header("Skill Type")]
    public SkillType SkillType;
    public SearchScope SearchScope;
    public int MaxTargets;

    [Header("Projectile Setting")]
    public ProjectileType ProjectileType;
    public ProjectileHitType ProjectileHitType;
    public float ProjectileSpeed;
    public float ProjectileDuration;

    [Header("Buff Setting")]
    public List<Buff> BuffList;

    [Header("After Effect")]
    public AnimationClip AfterAnimClip;
    public AudioClip AfterSFX;
    
    
    [Header("Stats")]
    public int SkillDamage;
    public float SkillScale = 1f;
    

    [Header("Skill Events")]
    public List<OnHitEvent> HitEvents;
    public List<OnKillEvent> KillEvents;

    

    public string GetDescription(int power, int maxTarget, int hitValue, float hitDuration, float killValue, float killDuration)
    {
        return string.Format(
        Description,
        Colorize(power, "#ff5555"),         // 0
        Colorize(SkillDamage, "#00a2ff"),   // 1
        Colorize(maxTarget, "#a200ff"),     // 2
        Colorize(hitValue, "#fbff00"),      // 3
        Colorize(hitDuration, "#00ff22"),   // 4        
        Colorize(killValue, "#d0ff00c0"),     // 5
        Colorize(killDuration, "#00ff22b6")   // 6
        );
    }

    private string Colorize(float value, string color)
    {
        return $"<color={color}>{value}</color>";
    }

    

}

[System.Serializable]
public class OnHitEvent
{
    public HitEvent HitEvent;
    public float Duration;
    public float Value;
    public EffectObject ParticleObj;
    public Sprite EffectSprite;
    public AnimationClip EffectAnim;
    public AudioClip AudioClip;
}

[System.Serializable]
public class OnKillEvent
{
    public KillEvent KillEvent;
    public float Duration;
    public float Value;
    public EffectObject ParticleObj;
    public Sprite EffectSprite;
    public AnimationClip EffectAnim;
    public AudioClip AudioClip;
}
