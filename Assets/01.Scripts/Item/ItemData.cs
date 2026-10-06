using System;
using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    [Header("Common")]
    public string ItemName;
    public int ItemIndex;
    [TextArea(2, 4)]
    public string Description;
    public Sprite Icon;
    public int Price;
    public ShopItemType ItemType = ShopItemType.Other;
    public int MaxPurchaseCount = 1;
    public bool DateReset = false;

    public abstract bool TryBuy();
}
