using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MinionSelcetUI : MonoBehaviour
{
    [Header("Prefab & Parent")]
    [SerializeField] private GameObject _minionCardPrefab;
    [SerializeField] private Transform _cardParent;

    [Header("Toggle Button")]
    [SerializeField] private Button _toggleBtn;
    private Image _toggleBtnImage;

    [Header("Button Sprite")]
    [SerializeField] private Sprite _downSprite;
    [SerializeField] private Sprite _upSprite;

    [Header("Panel")]
    [SerializeField] private RectTransform _rect;
    [SerializeField] private ScrollRect _scrollRect;

    [Header("Animation Settings")]
    public float DownDuration = 0.5f;
    public float UpDuration = 0.5f;

    private Vector2 _startPos;
    private Vector2 _downPos;
    private float _rectHeight; // height 크기

    
    private bool _isPanelDown;

    // Action 이벤트
    public event Action OnToggleBtn;
    public event Action OnShow; 

    void Start()
    {
        if(_rect == null)
            _rect = GetComponent<RectTransform>();

        if(_scrollRect == null)
            _scrollRect = transform.Find("Scroll View").GetComponent<ScrollRect>();
    
        if (_toggleBtn != null)
        {
            _toggleBtn.onClick.AddListener(() => OnToggleBtn?.Invoke());
            _toggleBtnImage = _toggleBtn.GetComponent<Image>();
        }

        _rectHeight = _rect.rect.height;

        _startPos = _rect.anchoredPosition;
        _downPos = _startPos + Vector2.down * _rectHeight;

        if(_minionCardPrefab == null)
            _minionCardPrefab = Resources.Load<GameObject>("UI/MinionSelectCard");

        UpDownToggleUI();
    }

    public void PopulateCards(List<MinionData> minionList)
    {
        if (_cardParent != null)
        {
            for (int i = _cardParent.childCount - 1; i >= 0; i--)
            {
                Destroy(_cardParent.GetChild(i).gameObject);
            }
        }

        foreach (var minionIndex in minionList)
            CreateSelectCard(minionIndex);
    }

    public void CreateSelectCard(MinionData minionData)
    {
        GameObject obj = Instantiate(_minionCardPrefab, _cardParent);
        
        MinionSelectCard card = obj.GetComponent<MinionSelectCard>();

        card.Init(minionData);
    }

    public void UpDownToggleUI()
    {
        if (_isPanelDown)
        {
            UpPanel();
        }
        else
        {
            DownPanel();
        }
    }

    public void DownPanel()
    {
        if(_isPanelDown)
            return;

        if (_toggleBtnImage != null)
        {
            _toggleBtnImage.sprite = _upSprite;
        }

        _rect.DOAnchorPos(_downPos, DownDuration)
        .SetEase(Ease.InOutQuad)
        .SetUpdate(true);

        _isPanelDown = true;
    }

    public void UpPanel()
    {
        if(!_isPanelDown)
            return;

        if (_toggleBtnImage != null)
        {
            _toggleBtnImage.sprite = _downSprite;
        }

        _rect.DOAnchorPos(_startPos, UpDuration)
        .SetEase(Ease.OutQuad)
        .SetUpdate(true);

        _isPanelDown = false;

        OnShow?.Invoke();
    }

}

#region  Events

/// <summary>
/// Minion Select Panel이 Open 되었을 때 이벤트 Publish
/// </summary>
public readonly struct MinionSelectUIOpenedEvent {};

#endregion
