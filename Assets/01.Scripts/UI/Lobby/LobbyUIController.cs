using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIController : MonoBehaviour
{
    [SerializeField] private Image _chessImage;

    private bool _isPanelVisible = false;
  
    void OnEnable()
    {
        EventBus.Subscribe<UIStageEvent>(OnStageEvent);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<UIStageEvent>(OnStageEvent);
    }

    private void SlideChessImage()
    {
        SoundManager.Instance.PlaySFX("Drawer");
        RectTransform rt = _chessImage.rectTransform;

        Vector2 target;
        _isPanelVisible = !_isPanelVisible;
        if (_isPanelVisible)
            target = new Vector2(rt.anchoredPosition.x, 450f);

        else
            target = new Vector2(rt.anchoredPosition.x, 0f);

        rt.DOAnchorPos(target, 1f).SetUpdate(true);
    }

#region Event Handlers

    private void OnStageEvent(UIStageEvent e)
    {
        SlideChessImage();
    }

#endregion

}

#region EventBus

/// <summary>
/// 로비에서 스테이지 진입 시 UI 이벤트
/// </summary>
public readonly struct UIStageEvent {}

#endregion