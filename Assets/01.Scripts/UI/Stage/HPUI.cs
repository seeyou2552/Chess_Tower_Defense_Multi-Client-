using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private Image _hpImage;
    [SerializeField] private Sprite[] _hpSprites;

    [Header("HP Animation 효과")]
    [SerializeField] private float _shakeDuration = 0.2f;
    [SerializeField] private float _shakeStrength = 10f;

    private Coroutine _shakeCoroutine;
    private int _lastIndex = -1;

    void OnEnable()
    {
        EventBus.Subscribe<UIHPChangedEvent>(OnHPChangedEvent);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<UIHPChangedEvent>(OnHPChangedEvent);
    }

    private void UpdateHPImage(int currentHP)
    {
        float ratio = currentHP / (float)PlayerManager.Instance.Data.MaxHP;

        int index = GetSpriteIndex(ratio);

        if (index == _lastIndex) return;

        _hpImage.sprite = _hpSprites[index];
        _lastIndex = index;
    }

    private int GetSpriteIndex(float ratio)
    {
        int count = _hpSprites.Length;

        // HP가 0 이하일 때만 마지막 인덱스
        if (ratio <= 0f)
            return count - 1;

        // 나머지는 마지막을 제외하고 분배
        int usableCount = count - 1;

        int index = Mathf.FloorToInt((1 - ratio) * usableCount);

        return Mathf.Clamp(index, 0, usableCount - 1);
    }

    private void PlayShake()
    {
        if (_shakeCoroutine != null)
            StopCoroutine(_shakeCoroutine);

        _shakeCoroutine = StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        Vector3 originalPos = _hpImage.rectTransform.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < _shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * _shakeStrength;
            float y = Random.Range(-1f, 1f) * _shakeStrength;

            _hpImage.rectTransform.anchoredPosition = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        _hpImage.rectTransform.anchoredPosition = originalPos;
    }

    public void VisibleUI(bool visible)
    {
        gameObject.SetActive(visible);
    }

#region Event Handlers

    private void OnHPChangedEvent(UIHPChangedEvent hpEvent)
    {
        _hpText.text = hpEvent.CurrentHP.ToString();
        UpdateHPImage(hpEvent.CurrentHP);
        PlayShake();
    }

#endregion
}

#region EventBus

public readonly struct UIHPChangedEvent
{
    public readonly int CurrentHP;
    public readonly int MaxHP;

    public UIHPChangedEvent(int currentHP, int maxHP)
    {
        CurrentHP = currentHP;
        MaxHP = maxHP;
    }
}

#endregion