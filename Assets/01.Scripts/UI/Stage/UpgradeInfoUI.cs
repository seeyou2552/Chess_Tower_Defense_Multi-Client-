using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeInfoUI : MonoBehaviour
{
    [Header("Main Panel")]
    [SerializeField] private GameObject _upgradeInfoPanel;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _upgradeNameText;
    [SerializeField] private TextMeshProUGUI _upgradeDescText;

    [Header("Image")]
    [SerializeField] private Image _upgradeImage;

    void OnEnable()
    {
        EventBus.Subscribe<UIShowUpgradeInfoEvent>(OnUpgradeInfo);
        EventBus.Subscribe<UIHideUpgradeInfoEvent>(OnHideUpgradeInfoUI);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<UIShowUpgradeInfoEvent>(OnUpgradeInfo);
        EventBus.Unsubscribe<UIHideUpgradeInfoEvent>(OnHideUpgradeInfoUI);
    }

#region Event Handlers

    private void OnUpgradeInfo(UIShowUpgradeInfoEvent e)
    {
        // 업그레이드 정보 설정 및 출력
        _upgradeInfoPanel.SetActive(true);
        _upgradeNameText.text = e.Upgrade.UpgradeData.UpgradeName;
        _upgradeDescText.text = e.Upgrade.UpgradeData.GetDescription(e.Upgrade.Increase);

        _upgradeImage.sprite = e.Upgrade.UpgradeData.UpgradeIcon;
    }

    private void OnHideUpgradeInfoUI(UIHideUpgradeInfoEvent e)
    {
        _upgradeInfoPanel.SetActive(false);
    }

#endregion

}

#region EventBus

public readonly struct UIShowUpgradeInfoEvent
{
    public readonly Upgrade Upgrade;

    public UIShowUpgradeInfoEvent(Upgrade upgrade)
    {
        Upgrade = upgrade;
    }
}

public readonly struct UIHideUpgradeInfoEvent {}

#endregion