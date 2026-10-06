using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "KillEvent/GoldOnKill")]
public class GoldOnKill : KillEvent
{
    public override void OnKill(MinionState minionState, Enemy enemy, OnKillEvent onKillEvent)
    {
        // StageManager.Instance.AddGold((int)onKillEvent.Value);
    }
}
