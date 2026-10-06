using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionRuntimeStat
{
    private MinionData _minionData;
    public event Action<float> OnRangeChanged;

    // Skill Stat
    public int CurrentAC {get; private set;}
    public int CurrentMaxAC {get; private set;}
    public int SkillMaxTarget {get; private set;}
    

    // Power Stat
    public float BasePower {get; private set;}     // 기본 공격력

    public float CurrentPower {get; private set;}   // 최종 적용 값 캐싱

    public float BuffPower      // 버프에 의해 변경된 공격력
    {
        get
        {
            float totalPowerBonus = 0f;

            foreach (var buff in PowerBuffs)
            {
                totalPowerBonus += buff;
            }

            return BasePower * (1 + totalPowerBonus / 100f);
        }
    }

    public List<float> PowerBuffs = new();

    // Atk Cooldown Stat
    public float BaseAttackCooldown {get; private set;}   // 기본 공격 쿨타임 (강화 시 이 값이 변함)

    public float CurrentAttackCooldown {get; private set;}      // 최종 적용 값 캐싱

    public float BuffAttackCooldown         // 버프에 의해 변경된 공격 쿨타임
    {
        get
        {
            float totalAtkSpeedBonus = 0f;

            foreach (var buff in AtkSpeedBuffs)
            {
                totalAtkSpeedBonus += buff;
            }

            return Mathf.Max(0.1f, BaseAttackCooldown / (1 + totalAtkSpeedBonus / 100f));
        }
    }

    public List<float> AtkSpeedBuffs = new();

    // Default Attack
    public AttackDeliveryType DeliveryType {get; private set;}
    public SearchScope SearchScope {get; private set;}
    public Sprite AtkSprite {get; private set;}
    public AnimationClip AtkAnimClip {get; private set;}
    public AudioClip AtkSFX {get; private set;}
    public int BaseAtkMaxTarget {get; private set;}        // 기본 최대 공격 대상 수
    public int CurrentAtkMaxTarget {get; private set;}     // 최종 적용 값 캐싱

    public int BuffAtkMaxTarget         // 버프에 의해 변경된 최대 공격 대상 수
    {
        get
        {
            return BaseAtkMaxTarget + AtkMaxTargetBuff;
        }
    }

    public int AtkMaxTargetBuff;

    // Projectile Setting
    public ProjectileType ProjectileType {get; private set;}
    public ProjectileHitType ProjectileHitType {get; private set;}
    public float ProjectileSpeed {get; private set;}
    public float ProjectileDuration {get; private set;}


    public void Init(MinionData minionData)
    {
        _minionData = minionData;

        // power
        BasePower = _minionData.AttackPower;
        CurrentPower = BasePower;

        // attack cooldown
        BaseAttackCooldown = _minionData.AttackCooldown;
        CurrentAttackCooldown = BaseAttackCooldown;

        // Default Attack
        BaseAtkMaxTarget = _minionData.MaxTargets;
        CurrentAtkMaxTarget = BaseAtkMaxTarget;
        DeliveryType = _minionData.DeliveryType;
        SearchScope = _minionData.SearchScope;

        // AC
        CurrentMaxAC = _minionData.MaxAC;
        CurrentAC = 0;

        // Skill
        SkillMaxTarget = _minionData.Skill.MaxTargets;

        // Atk Visuals
        AtkSprite = _minionData.AtkSprite;
        AtkAnimClip = _minionData.AtkAnimClip;
        AtkSFX = _minionData.AtkSFX;

        // Projectile Setting
        ProjectileType = _minionData.ProjectileType;
        ProjectileHitType = _minionData.ProjectileHitType;
        ProjectileSpeed = _minionData.ProjectileSpeed;
        ProjectileDuration = _minionData.ProjectileDuration;

    }

    public void ResetDefaultAttack()
    {
        DeliveryType = _minionData.DeliveryType;
        SearchScope = _minionData.SearchScope;
        AtkSprite = _minionData.AtkSprite;
        AtkAnimClip = _minionData.AtkAnimClip;
        AtkSFX = _minionData.AtkSFX;

        // Projectile Setting
        ProjectileType = _minionData.ProjectileType;
        ProjectileHitType = _minionData.ProjectileHitType;
        ProjectileSpeed = _minionData.ProjectileSpeed;
        ProjectileDuration = _minionData.ProjectileDuration;
    }

    public void ChangeDefaultAttack(ChangeDefaultAttack option)
    {
        DeliveryType = option.DeliveryType;
        SearchScope = option.SearchScope;

        if (option.AtkSprite != null)
            AtkSprite = option.AtkSprite;

        if (option.AtkAnimClip != null)
            AtkAnimClip = option.AtkAnimClip;

        if (option.AtkSFX != null)
            AtkSFX = option.AtkSFX;

        ProjectileType = option.ProjectileType;
        ProjectileHitType = option.ProjectileHitType;
        ProjectileSpeed = option.ProjectileSpeed;
        ProjectileDuration = option.ProjectileDuration;

    }

    // 버프 갱신 후 해당 버프 타입에 맞는 current 값 재계산
    public void ChangedBuffValue(Buff buff)
    {
        switch (buff.Data)
        {
            case MaxAtkTargetBuff _:
                CurrentAtkMaxTarget = BuffAtkMaxTarget;
                break;

            case AtkSpeedBuff _:
                CurrentAttackCooldown = BuffAttackCooldown;
                break;

            case PowerUpBuff _:
                CurrentPower = BuffPower;
                break;
        }
    }

    // 업그레이드 후 current 값 재계산
    public void RecalculateCurrentStats()
    {
        CurrentPower = BuffPower;
        CurrentAttackCooldown = BuffAttackCooldown;
        CurrentAtkMaxTarget = BuffAtkMaxTarget;
    }

    public void ResetRuntimeStat()
    {
        CurrentAC = 0;
        AtkMaxTargetBuff = 0;
        PowerBuffs.Clear();
        AtkSpeedBuffs.Clear();
    }

    public void AddBasePower(int value)
    {
        BasePower += value;
    }

    public void AddBaseAtkCooldown(float value)
    {
        BaseAttackCooldown -= value;
    }

    public void AddAC(int value)
    {
        CurrentAC += value;
    }

    public void AddMaxAC(int value)
    {
        CurrentMaxAC -= value;
    }

    public void AddSkillMaxTargets(int value)
    {
        SkillMaxTarget += value;
    }

    public void IncreaseAtkRange(float value)
    {
        OnRangeChanged?.Invoke(value);
    }

    public void ResetCurrentAC()
    {
        CurrentAC = 0;
    }

    public void ClearBuffs()
    {
        PowerBuffs.Clear();
        AtkSpeedBuffs.Clear();
        AtkMaxTargetBuff = 0;
    }

}