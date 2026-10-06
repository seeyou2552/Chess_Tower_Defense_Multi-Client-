using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BuffData/AtkMaxTargetBuff")]
public class MaxAtkTargetBuff : BuffData
{
    public override void ApplyBuff(Minion minion, Buff buff)
    {
        minion.State.BuffController.ApplyBuff(buff);
    }
}
