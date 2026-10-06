using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UpgradeData/Range")]
public class UpgradeRange : UpgradeData
{
    public override void Upgrade(MinionState minionState, Upgrade upgrade)
    {
        minionState.RuntimeStat.IncreaseAtkRange(upgrade.Increase);
    }
}
