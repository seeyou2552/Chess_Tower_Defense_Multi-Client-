using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PoolObject : MonoBehaviour
{
    public abstract void ReturnToPool();
}

#region EventBus
public readonly struct PoolReturn {}

#endregion