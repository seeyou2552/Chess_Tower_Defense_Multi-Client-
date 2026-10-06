using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "KillEvent/PowerUpKill")]
public class PowerUpKillAction : KillEvent
{
    public override void OnKill(MinionState minionState, Enemy enemy, OnKillEvent onKillEvent)
    {
        minionState.RuntimeStat.AddBasePower((int)onKillEvent.Value);
        minionState.RuntimeStat.RecalculateCurrentStats();
    }
}
