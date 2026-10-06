using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UpgradeData/Power")]
public class UpgradePower : UpgradeData
{
    public override void Upgrade(MinionState minionState, Upgrade upgrade)
    {
        minionState.RuntimeStat.AddBasePower((int)upgrade.Increase);
        minionState.RuntimeStat.RecalculateCurrentStats();
    }
}
