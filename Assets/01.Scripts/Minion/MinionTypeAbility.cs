using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionTypeAbility
{
    private static HashSet<MinionState> _queenMinions = new();
    private const int QueenIncreasePower = 10;

    public static bool CanBypassSiege(MinionType type)
    {
        return type == MinionType.Knight;
    }

    public static bool CanSpeedBonusDamage(MinionType type)
    {
        return type == MinionType.Pawn;
    }

    public static void UpgradeAvility(MinionState minionState)
    {
        if (minionState.Data.MinionType == MinionType.Pawn && minionState.UpgradeController.AllMaxLevelCheck())
        {
            foreach (var queen in _queenMinions)
            {
                if (queen == null)
                    continue;

                queen.RuntimeStat.AddBasePower(QueenIncreasePower);
                queen.RuntimeStat.RecalculateCurrentStats();
            }
        }
    }

    public static void SubscribeAbility(MinionState minionState)
    {
        if (minionState.Data.MinionType == MinionType.Queen)
        {
            _queenMinions.Add(minionState);
        }

    }

    public static void RemoveSubscribeAbility(MinionState minionState)
    {
        if (minionState.Data.MinionType == MinionType.Queen)
        {
            _queenMinions.Remove(minionState);
        }
        
    }
}
