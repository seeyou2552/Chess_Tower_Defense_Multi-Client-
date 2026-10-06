using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveUI : MonoBehaviour
{
    [Header("Wave Panel")]
    [SerializeField] private TextMeshProUGUI _waveValueText;

    [Header("Start Button")]
    [SerializeField] private Button _startBtn;

    private RectTransform _startBtnRt;
    private Vector2 _originPos;

    void Start()
    {
        if(_startBtn == null)
            return;

        _startBtn.onClick.AddListener(WaveButtonEvent);

        _startBtnRt = _startBtn.GetComponent<RectTransform>();
        _originPos = _startBtnRt.anchoredPosition;

    }

    void OnEnable()
    {
        EventBus.Subscribe<WaveStartEvent>(OnWaveStart);
        EventBus.Subscribe<WaveEndEvent>(OnWaveEnd);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<WaveStartEvent>(OnWaveStart);
        EventBus.Unsubscribe<WaveEndEvent>(OnWaveEnd);
    }

    private void WaveButtonEvent()
    {
        StageManager.Instance.SendStartWave();
    }

    private void OnWaveStart(WaveStartEvent e)
    {
        float canvasHeight = ((RectTransform)_startBtnRt.parent).rect.height;

        _startBtnRt.DOAnchorPosY(canvasHeight + 20f, 1f).SetUpdate(true);
    }

    private void OnWaveEnd(WaveEndEvent e)
    {
        _startBtnRt.DOAnchorPos(_originPos, 1f).SetUpdate(true);

        _waveValueText.text = (StageManager.Instance.CurrentWave + 1).ToString();
    }
}
