using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HitEvent : ScriptableObject
{
    public int ID;
    public abstract void OnHit(MinionState minionState, Enemy enemy, OnHitEvent onHit);
}
