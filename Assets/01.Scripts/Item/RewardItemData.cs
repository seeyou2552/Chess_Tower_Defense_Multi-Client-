using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TD/Shop/RewardData", fileName = "RewardGold")]
public class RewardItemData : ItemData
{
    public int RewardAmount;

    public override bool TryBuy()
    {
        // AdMobManager.Instance.ShowAd(GoldReward);
        return false; // 주석 전환
    }

    private void GoldReward()
    {
        
        Debug.Log($"골드 {RewardAmount} 획득");
    }
}
