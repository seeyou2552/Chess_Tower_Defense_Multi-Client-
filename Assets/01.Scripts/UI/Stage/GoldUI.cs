using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _goldText;
    [SerializeField] private CanvasGroup _canvasGroup;

    void OnEnable()
    {
        EventBus.Subscribe<UIGoldChangedEvent>(OnGoldChangedEvent);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<UIGoldChangedEvent>(OnGoldChangedEvent);
    }

    private void OnGoldChangedEvent(UIGoldChangedEvent goldEvent)
    {
        _goldText.text = goldEvent.Gold.ToString();
    }
}

#region EventBus

public readonly struct UIGoldChangedEvent
{
    public readonly int Gold;

    public UIGoldChangedEvent(int gold)
    {
        Gold = gold;
    }
}

#endregion