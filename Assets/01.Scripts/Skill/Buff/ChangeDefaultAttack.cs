using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ChangeDefaultAttackData")]
public class ChangeDefaultAttack : ScriptableObject
{
    [Header("ID")]
    public int ID;

    [Header("Default Attack")]
    public AttackDeliveryType DeliveryType;
    public SearchScope SearchScope;
    public Sprite AtkSprite;
    public AnimationClip AtkAnimClip;
    public AudioClip AtkSFX;


    [Header("Projectile Setting")]
    public ProjectileType ProjectileType;
    public ProjectileHitType ProjectileHitType;
    public float ProjectileSpeed;
    public float ProjectileDuration;
}
