using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MinionInfoUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject _minionInfoPanel;

    [Header("Default UI")]
    [SerializeField] private TextMeshProUGUI _minionNameText;
    [SerializeField] private TextMeshProUGUI _powerValueText;
    [SerializeField] private TextMeshProUGUI _atkCooldownValueText;
    [SerializeField] private TextMeshProUGUI _currentACText;
    [SerializeField] private TextMeshProUGUI _skillNameText;
    [SerializeField] private TextMeshProUGUI _skillDesText;
    

    [Header("소환된 Minion UI")]
    [SerializeField] private GameObject _sellPanel;
    [SerializeField] private Button _sellBtn;
    [SerializeField] private TextMeshProUGUI _sellGoldText;
    [SerializeField] private UpgradePanel _upgradePanel;

    // Action 이벤트
    public event Action OnSellBtn;
    public event Action OnHide; 

    void Start()
    {
        if(_sellBtn == null)
            return;

        _sellBtn.onClick.AddListener(() => OnSellBtn?.Invoke());
    }


    public void SetMinionInfo(MinionState minionState) // 생성된 미니언
    {
        DefaultSetInfo(
            minionState.RuntimeStat.CurrentPower.ToString(),
            minionState.RuntimeStat.CurrentAttackCooldown.ToString("F1") + "s",
            minionState.RuntimeStat.CurrentAC.ToString(),
            minionState.RuntimeStat.CurrentMaxAC.ToString(),
            minionState.Data
        );
    }

    public void SetMinionInfo(MinionData minionData) // 생성되지 않은 미니언
    { 
        DefaultSetInfo(
            minionData.AttackPower.ToString(),
            minionData.AttackCooldown.ToString(),
            "0",
            minionData.MaxAC.ToString(),
            minionData    
        );

        _sellPanel.gameObject.SetActive(false);
        _upgradePanel.gameObject.SetActive(false);

        VisibleUI(true);
    }

    public void SetSellPanel(bool active, string gold = null)
    {
        _sellPanel.gameObject.SetActive(active);

        if (active)
            _sellGoldText.text = gold;
    }

    public void SetUpgradePanel(bool active, MinionState minionState = null)
    {
        _upgradePanel.gameObject.SetActive(active);

        if (active)
            _upgradePanel.SetBtn(minionState);
    }

    /// <summary>
    /// Info UI에 출력되는 공통 정보 입력
    /// </summary>
    private void DefaultSetInfo(string powerValue, string atkCoolValue, string currentAC, string maxAC, MinionData minionData)
    {
        // 상단에 출력되는 기본 정보
        _minionNameText.text = minionData.CharacterName;
        _powerValueText.text = powerValue;
        _atkCooldownValueText.text = atkCoolValue;
        _currentACText.text = currentAC + " / " + maxAC;
        _skillNameText.text = minionData.Skill.SkillName;

        // Minion이 보유한 Hit 및 Kill Event의 값 수집
        var hitValue = (minionData.Skill.HitEvents != null 
            && minionData.Skill.HitEvents.Count > 0)
            ? (int)minionData.Skill.HitEvents[0].Value : 0;

        var hitDuration = (minionData.Skill.HitEvents != null 
            && minionData.Skill.HitEvents.Count > 0)
            ? (int)minionData.Skill.HitEvents[0].Duration : 0;

        var killValue = (minionData.Skill.KillEvents != null 
            && minionData.Skill.KillEvents.Count > 0)
            ? (int)minionData.Skill.KillEvents[0].Value : 0;

        var killDuration = (minionData.Skill.KillEvents != null 
            && minionData.Skill.KillEvents.Count > 0)
            ? (int)minionData.Skill.KillEvents[0].Duration : 0;

        _skillDesText.text = minionData.Skill.GetDescription(
            minionData.AttackPower,
            minionData.Skill.MaxTargets,
            hitValue,
            hitDuration,
            killValue,
            killDuration
            );
    }

    public void VisibleUI(bool isVisible)
    {
        _minionInfoPanel.SetActive(isVisible);
        
        // Hide시 액션 실행
        if (!isVisible)
            OnHide?.Invoke();
    }



}

#region EventBus

/// <summary>
/// Minion 데이터가 있는 Info를 Open했을 때 이벤트 Publish
/// </summary>
public readonly struct MinionInfoOpenedEvent {};

#endregion