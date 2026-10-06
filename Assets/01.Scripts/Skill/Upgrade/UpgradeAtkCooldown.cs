using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UpgradeData/AtkCooldown")]
public class UpgradeAtkCooldown : UpgradeData
{
    public override void Upgrade(MinionState minionState, Upgrade upgrade)
    {
        minionState.RuntimeStat.AddBaseAtkCooldown(upgrade.Increase);
        minionState.RuntimeStat.RecalculateCurrentStats();
    }
}
