using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MinionSelectCard : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private MinionData _minionData;

    [SerializeField] private Image _minionImage;
    [SerializeField] private TextMeshProUGUI _coastText;

    private Coroutine _pressCoroutine;
    private bool _isPressed;
    private Minion _spawnedMinion;

    void Start()
    {
        if(_coastText == null)
            _coastText = GetComponentInChildren<TextMeshProUGUI>();
        
        if(_minionImage == null)
            _minionImage = transform.GetChild(0).GetComponent<Image>();

        _minionImage.sprite = _minionData.MinionSprite;
        _coastText.text = _minionData.Cost.ToString();
    }
    
    public void Init(MinionData minionData)
    {
        _minionData = minionData;
    }

    IEnumerator LongPress(PointerEventData eventData)
    {
        float pressTimer = 0f;

        while (pressTimer <= 0.5f)
        {
            if (!_isPressed)
                yield break;
            
            pressTimer += Time.unscaledDeltaTime;

            yield return null;
        }
        
        if (GameManager.Instance.GameState == GameState.Wave)
        {
            EventBus.Publish(new UIAlertEvent("Wave 중에는 생성이 불가능합니다."));
            yield break;
        }

        if (StageManager.Instance.CheckGold(_minionData.Cost))
        {
            _spawnedMinion = SpawnManager.Instance.SpawnMinion(_minionData, (uint)0, 0);
            eventData.pointerDrag = _spawnedMinion.gameObject;

            ExecuteEvents.Execute(_spawnedMinion.gameObject, eventData, ExecuteEvents.beginDragHandler);
            CommonUIManager.Instance.AllHideUI();

        }

        _pressCoroutine = null;
    }

    


#region  Pointer 인터페이스 구현
    public void OnPointerDown(PointerEventData eventData)
    {
        StageUIController.Instance.ShowMinionInfo(_minionData);
        _isPressed = true;
        _pressCoroutine = StartCoroutine(LongPress(eventData));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isPressed = false;
        if (_pressCoroutine != null)
        {
            StopCoroutine(_pressCoroutine);
            _pressCoroutine = null;
        }
    }

#endregion

}
