using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject _selectPanel;

    [Header("Button")]
    [SerializeField] private Button _agreeBtn;
    [SerializeField] private Button _cancelBtn;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _selectText;

    void Awake()
    {
        if (_cancelBtn != null)
            _cancelBtn.onClick.AddListener(OnCancelBtn);
    }

    void OnEnable()
    {
        EventBus.Subscribe<UISelectEvent>(OnSelectBtn);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<UISelectEvent>(OnSelectBtn);
    }

    private void OnSelectBtn(UISelectEvent e)
    {
        VisibleUI(true);

        // Listeners 초기화 후 Yes 버튼 클릭 시 실행할 Action 설정
        _agreeBtn.onClick.RemoveAllListeners();

        _agreeBtn.onClick.AddListener(() =>
        {
            e.SelectAction?.Invoke();
            SoundManager.Instance.PlaySFX("SelectBtnClick");
        });

        _selectText.text = e.Text;
    }

    public void VisibleUI(bool isVisible)
    {
        _selectPanel.SetActive(isVisible);

        if (isVisible)
        {
            CommonUIManager.Instance.AddUIStack(() => VisibleUI(false));
            SoundManager.Instance.PlaySFX("ShowSelectPanel");
        }
            
    }

    private void OnCancelBtn()
    {
        CommonUIManager.Instance.BackUI();
        SoundManager.Instance.PlaySFX("SelectBtnClick");
    }
}

#region EventBus

public readonly struct UISelectEvent
{
    public readonly Action SelectAction;
    public readonly String Text;

    public UISelectEvent(Action selectAction, string text)
    {
        SelectAction = selectAction;
        Text = text;
    }
}

#endregion