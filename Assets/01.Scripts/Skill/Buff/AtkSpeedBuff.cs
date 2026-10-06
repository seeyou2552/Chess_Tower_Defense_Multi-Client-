using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BuffData/AtkSpeedBuff")]
public class AtkSpeedBuff : BuffData
{
    public override void ApplyBuff(Minion minion, Buff buff)
    {
        minion.State.BuffController.ApplyBuff(buff);
    }
}
