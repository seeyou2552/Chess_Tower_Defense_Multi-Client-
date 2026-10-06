using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeBtn : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _upgradeLevelText;
    [SerializeField] private TextMeshProUGUI _upgradeCostText;

    [Header("Image")]
    [SerializeField] private Image _upgradeImage;

    private Upgrade _upgrade;
    private MinionState _minionState;

    private Coroutine _longPressCoroutine;
    private const float LongPressDration = 0.7f;
    private bool _isPressed = false;
    private bool _hasTriggeredLongPress = false;

    private void UpgradeButtonEvent()
    {
        TryUpgrade();
    }

    public void TryUpgrade()
    {
        if(_minionState.UpgradeController.MaxLevelCheck(_upgrade))
        {
            EventBus.Publish(new UIAlertEvent("업그레이드가 이미 최대 레벨입니다."));
            return;
        }
        
        if(StageManager.Instance.CheckGold(_upgrade.Cost) )
        {
            // 업그레이드 작업 Send
            MinionManager.Instance.SendUpgradeMinion(_minionState.InstanceId, _upgrade.UpgradeData.ID);

            
            // _minionState.UpgradeController.UpgradeLevelUp(_upgrade.UpgradeData.ID);
 
            // _minionState.SellGoldUpdate(_upgrade.Cost / 2); // 업그레이드 후 Sell Gold 수정
        }
    }

    public void Init(Upgrade upgrade, MinionState minionState)
    {
        // 데이터 참조
        _minionState = minionState;
        _upgrade = upgrade;

        // Text 설정
        _upgradeCostText.text = _upgrade.Cost.ToString();

        if(_minionState.UpgradeController.MaxLevelCheck(_upgrade))
            _upgradeLevelText.text = "Max";
        else 
            _upgradeLevelText.text = "LV. " + _minionState.UpgradeController.GetUpgradeLevel(_upgrade.UpgradeData.ID);

        if (_upgradeImage == null)
            _upgradeImage ??= GetComponent<Image>() ?? gameObject.AddComponent<Image>();
            
        _upgradeImage.sprite = _upgrade.UpgradeData.UpgradeIcon;
    }

    IEnumerator LongPressCoroutine()
    {
        float tempTime = 0f;
        while (tempTime < LongPressDration)
        {
            tempTime += Time.deltaTime;
            if (!_isPressed)
            {
                yield break; // 버튼에서 손을 떼면 코루틴 종료
            }
            yield return null;
        }

        _hasTriggeredLongPress = true;
        EventBus.Publish(new UIShowUpgradeInfoEvent(_upgrade));
    }

#region Pointer 인터페이스 구현

    public void OnPointerUp(PointerEventData eventData)
    {
        _isPressed = false;

        // 코루틴 중지
        if (_longPressCoroutine != null)
        {
            StopCoroutine(_longPressCoroutine);
            _longPressCoroutine = null;
        }

        // 롱프레스가 이미 발생했다면 숨기기
        if (_hasTriggeredLongPress)
        {
            EventBus.Publish(new UIHideUpgradeInfoEvent());
            _hasTriggeredLongPress = false;
        }
        // 롱프레스가 발생하지 않았다면 업그레이드 실행
        else
            UpgradeButtonEvent();

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 기존 코루틴이 있으면 중지
        if (_longPressCoroutine != null)
        {
            StopCoroutine(_longPressCoroutine);
        }

        _isPressed = true;
        _hasTriggeredLongPress = false;

        // 새로운 롱프레스 코루틴 시작
        _longPressCoroutine = StartCoroutine(LongPressCoroutine());
    }
#endregion

}
