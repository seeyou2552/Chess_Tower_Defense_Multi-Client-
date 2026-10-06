using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TD/Shop/MinionItemData", fileName = "NewMinionItem")]
public class MinionItemData : ItemData
{
    public MinionData MinionData;

    public override bool TryBuy()
    {
        
        return true;
    }
}
