using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UpgradeData/MaxTarget")]
public class UpgradeMaxTarget : UpgradeData
{
    public override void Upgrade(MinionState minionState, Upgrade upgrade)
    {
        minionState.RuntimeStat.AddSkillMaxTargets((int)upgrade.Increase);
        minionState.RuntimeStat.RecalculateCurrentStats();
    }
}
