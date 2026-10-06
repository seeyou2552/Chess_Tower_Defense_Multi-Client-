using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TD/Shop/HPItemData", fileName = "HPItem")]
public class IncreaseHPItemData : ItemData
{   
    public int HpIncreaseAmount;

    public override bool TryBuy()
    {
        
        return true;
    }
}
