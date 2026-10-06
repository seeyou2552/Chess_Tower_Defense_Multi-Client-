using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class KillEvent : ScriptableObject
{
    public int ID;
    public abstract void OnKill(MinionState minionState, Enemy enemy, OnKillEvent onKillEvent);
}
