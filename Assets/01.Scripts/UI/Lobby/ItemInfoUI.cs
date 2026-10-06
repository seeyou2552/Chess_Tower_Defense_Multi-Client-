using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoUI : MonoBehaviour
{
    [Header("Minion Info")]
    [SerializeField] private GameObject _minionInfoPanel;
    [SerializeField] private TextMeshProUGUI _powerText;
    [SerializeField] private TextMeshProUGUI _atkCooldownText;
    [SerializeField] private TextMeshProUGUI _acText;
    [SerializeField] private TextMeshProUGUI _skillNameText;
    [SerializeField] private TextMeshProUGUI _skillDescriptionText;

    [Header("Item Info")]
    [SerializeField] private GameObject _itemInfoPanel;
    [SerializeField] private TextMeshProUGUI _itemDescriptionText;
    

    [Header("Default Info")]
    [SerializeField] private TextMeshProUGUI _itemNameText;
    [SerializeField] private TextMeshProUGUI _itemPriceText;
    [SerializeField] private TextMeshProUGUI _buyCountText;
    [SerializeField] private Image _itemIcon;

    void OnEnable()
    {
        EventBus.Subscribe<BuyItemEvent>(OnBuyItem);
    }

    public void ItemSelect(ItemData data)
    {
        VisibleUI(true);
        if (data is MinionItemData minionItem)
        {
            ShowMinionInfo(minionItem);
        }
        else
        {
            ShowItemInfo(data);
        }
    }


    private void ShowMinionInfo(MinionItemData minionItem)
    {
        _minionInfoPanel.SetActive(true);
        _itemInfoPanel.SetActive(false);

        _powerText.text = minionItem.MinionData.AttackPower.ToString();
        _atkCooldownText.text = minionItem.MinionData.AttackCooldown.ToString("F1") + "s";
        _acText.text = minionItem.MinionData.MaxAC.ToString();
        _skillNameText.text = minionItem.MinionData.Skill.SkillName;

        var hitValue = (minionItem.MinionData.Skill.HitEvents != null 
            && minionItem.MinionData.Skill.HitEvents.Count > 0)
            ? (int)minionItem.MinionData.Skill.HitEvents[0].Value : 0;

        var hitDuration = (minionItem.MinionData.Skill.HitEvents != null 
            && minionItem.MinionData.Skill.HitEvents.Count > 0)
            ? (int)minionItem.MinionData.Skill.HitEvents[0].Duration : 0;

        var killValue = (minionItem.MinionData.Skill.KillEvents != null 
            && minionItem.MinionData.Skill.KillEvents.Count > 0)
            ? (int)minionItem.MinionData.Skill.KillEvents[0].Value : 0;

        var killDuration = (minionItem.MinionData.Skill.KillEvents != null 
            && minionItem.MinionData.Skill.KillEvents.Count > 0)
            ? (int)minionItem.MinionData.Skill.KillEvents[0].Duration : 0;

        _skillDescriptionText.text = minionItem.MinionData.Skill.GetDescription(
            minionItem.MinionData.AttackPower,
            minionItem.MinionData.Skill.MaxTargets,
            hitValue,
            hitDuration,
            killValue,
            killDuration);

        ShowDefaultItemInfo(minionItem);
    }

    private void ShowItemInfo(ItemData itemData)
    {
        _minionInfoPanel.SetActive(false);
        _itemInfoPanel.SetActive(true);

        ShowDefaultItemInfo(itemData);
    }

    private void ShowDefaultItemInfo(ItemData itemData)
    {
        _itemIcon.sprite = itemData.Icon;
        _itemNameText.text = itemData.ItemName;
        _itemDescriptionText.text = itemData.Description;
        _itemPriceText.text = itemData.Price.ToString();
        _buyCountText.text = "구매 횟수 (" + 
            GameManager.Instance.ShopData.GetPurchasedCount(itemData) + "/" + itemData.MaxPurchaseCount + ")";
    }

    public void VisibleUI(bool visible)
    {
        gameObject.SetActive(visible);
    }

#region Event Handlers

    private void OnBuyItem(BuyItemEvent e)
    {
        // 구매 횟수 업데이트
         _buyCountText.text = "구매 횟수 (" + 
            GameManager.Instance.ShopData.GetPurchasedCount(e.ItemData) + "/" + e.ItemData.MaxPurchaseCount + ")";
    }

#endregion

}

