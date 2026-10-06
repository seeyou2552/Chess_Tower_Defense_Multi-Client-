using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillStrategy
{
    void Execute(Minion minion, List<ChessPiece> targets, float damage);
}

public class ImpactSkillStrategy : ISkillStrategy
{
    public void Execute(Minion minion, List<ChessPiece> targets, float damage)
    {
        foreach (var target in targets)
        {
            if (target is Enemy enemy)
            {
                SkillEffect skillObj = SpawnManager.Instance.GetSkillObject(enemy.transform.position);
                skillObj.SkillInit(enemy, minion, minion.State.Data.Skill, damage, false);
                skillObj.ApplyDamageTo(enemy);
            }
        }
    }
}

public class ProjectileSkillStrategy : ISkillStrategy
{
    public void Execute(Minion minion, List<ChessPiece> targets, float damage)
    {
        foreach (var target in targets)
        {
            Debug.Log(target);
            if (target is Enemy enemy)
            {
                SkillEffect skillObj = SpawnManager.Instance.GetSkillObject(minion.transform.position);
                skillObj.SkillInit(enemy, minion, minion.State.Data.Skill, damage, true);
            }
        }
    }
}

public class AOESkillStrategy : ISkillStrategy
{
    public void Execute(Minion minion, List<ChessPiece> targets, float damage)
    {
        Debug.Log("AOE");
        SkillEffect skillObj = SpawnManager.Instance.GetSkillObject(minion.transform.position);

        foreach (var target in targets)
        {
            if (target is Enemy enemy)
            {
                skillObj.SkillInit(enemy, minion, minion.State.Data.Skill, damage, false);
                enemy.TakeDamage(damage);
            }
        }

        skillObj.transform.localScale = minion.AttackRange.gameObject.transform.localScale;
    }
}

public class BuffSkillStrategy : ISkillStrategy
{
    public void Execute(Minion minion, List<ChessPiece> targets, float damage)
    {
        foreach (var target in targets)
        {
            if (target is Minion targetMinion)
            {
                foreach(var buff in minion.State.Data.Skill.BuffList)
                    buff.Data.ApplyBuff(targetMinion, buff);
            }
        }
    }
}