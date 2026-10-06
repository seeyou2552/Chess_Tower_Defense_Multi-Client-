using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UpgradeData/MaxAC")]
public class UpgradeMaxAC : UpgradeData
{
    public override void Upgrade(MinionState minionState, Upgrade upgrade)
    {
        minionState.RuntimeStat.AddMaxAC((int)upgrade.Increase);
    }
}
