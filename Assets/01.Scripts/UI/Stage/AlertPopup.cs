using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AlertPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _alertText;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _fadeDuration = 1f;

    private Coroutine _coroutine;

    void Start()
    {
        _canvasGroup.alpha = 0f;
    }

    void OnEnable()
    {
        EventBus.Subscribe<UIAlertEvent>(OnAlertPopupEvent);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<UIAlertEvent>(OnAlertPopupEvent);
    }

    public void OnAlertPopupEvent(UIAlertEvent alertEvent)
    {
        _alertText.text = alertEvent.Message;
        _canvasGroup.alpha = 1f;
        if(_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }

        _coroutine = StartCoroutine(FadeOut());
    }

    public void HideAlertPopup()
    {
        if(_coroutine != null)
        {
            _canvasGroup.alpha = 0f;
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    IEnumerator FadeOut()
    {
        float time = 0f;

        while (time < _fadeDuration)
        {
            time += Time.deltaTime;
            _canvasGroup.alpha = 1f - (time / _fadeDuration);
            yield return null;
        }

        _canvasGroup.alpha = 0f;
        _coroutine = null;
    }
}

#region EventBus

public readonly struct UIAlertEvent
{
    public readonly string Message;

    public UIAlertEvent(string message)
    {
        Message = message;
    }
}

#endregion