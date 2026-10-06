using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "TD/MinionData")]
public class MinionData : ScriptableObject
{
    [Header("Visuals")]
    public int MinionIndex;
    public Sprite MinionSprite;
    public AnimationClip MinionAnim;
    public string CharacterName;

    [Header("Stats")]
    public int MaxAC;
    public float AttackRange;
    public int AttackPower;
    public float AttackCooldown;
    public int Cost;

    [Header("Skill")]
    public SkillData Skill;
    public Upgrade[] Upgrades = new Upgrade[3];

    [Header("Default Attack")]
    public AttackDeliveryType DeliveryType;
    public SearchScope SearchScope;
    public int MaxTargets;
    public Sprite AtkSprite;
    public AnimationClip AtkAnimClip;
    public AudioClip AtkSFX;

    [Header("Projectile Setting")]
    public ProjectileType ProjectileType;
    public ProjectileHitType ProjectileHitType;
    public float ProjectileSpeed;
    public float ProjectileDuration;

    

    [Header("Type")]
    public MinionType MinionType;


}
