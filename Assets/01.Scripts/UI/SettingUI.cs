using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject _settingPanel;

    [Header("Sound")]
    [SerializeField] private Button _bgmToggleBtn;
    [SerializeField] private Button _sfxToggleBtn;

    [Header("Attack Range")]
    [SerializeField] private Button _attackRangeToggleBtn;

    [Header("Button")]
    [SerializeField] private Button _toLobbyCheckBtn;
    [SerializeField] private Button _quitBtn;

    
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _versionText;

    void Awake()
    {
        // 초기 버튼 Listener 설정
        _bgmToggleBtn.onClick.AddListener(OnToggleBgmBtn);
        _sfxToggleBtn.onClick.AddListener(OnToggleSfxBtn);
        if (_attackRangeToggleBtn != null)
            _attackRangeToggleBtn.onClick.AddListener(OnToggleAttackRangeBtn);
        
        if (_toLobbyCheckBtn != null)
            _toLobbyCheckBtn.onClick.AddListener(OnLobbyCheckPanelBtn);

        if (_quitBtn != null)
            _quitBtn.onClick.AddListener(OnQuitBtn);
    }

    void OnEnable()
    {
        EventBus.Subscribe<UISettingEvent>(OnSettingEvent);
        EventBus.Subscribe<UIVersionEvent>(OnShowVersionEvent);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<UISettingEvent>(OnSettingEvent);
        EventBus.Unsubscribe<UIVersionEvent>(OnShowVersionEvent);
    }

    void Start()
    {
        if (GameManager.Instance == null || GameManager.Instance.SettingData == null)
            return;
        
        InitializeFromSettings();
    }

    public void InitializeFromSettings()
    {
        if (GameManager.Instance == null)
            return;

        var settings = GameManager.Instance.SettingData;
        if (settings == null)
            return;

        // 저장된 기존 설정에 따른 활성화 표시
        CheckImageActive(_bgmToggleBtn, settings.IsBGMOn);
        CheckImageActive(_sfxToggleBtn, settings.IsSFXOn);
        CheckImageActive(_attackRangeToggleBtn, settings.IsAtkRangeOn);

        // 저장된 기존 설정에 맞게 활성화 또는 비활성화
        SoundManager.Instance?.ApplySettings(settings);
        AttackRange.SetVisibleForAll(settings.IsAtkRangeOn);
    }

    private void OnToggleBgmBtn()
    {
        if (GameManager.Instance?.SettingData == null)
            return;

        var sound = SoundManager.Instance;
        if (sound == null)
            return;

        bool enabled = !sound.IsBgmEnabled();
        sound.SetBgmEnabled(enabled);
        GameManager.Instance.SettingData.IsBGMOn = enabled;

        CheckImageActive(_bgmToggleBtn, sound.IsBgmEnabled());
        SaveSystem.Save(GameManager.Instance.SettingData);
    }

    private void OnToggleSfxBtn()
    {
        if (GameManager.Instance?.SettingData == null)
            return;

        var sound = SoundManager.Instance;
        if (sound == null)
            return;

        bool enabled = !sound.IsSfxEnabled();
        sound.SetSfxEnabled(enabled);
        GameManager.Instance.SettingData.IsSFXOn = enabled;
        
        CheckImageActive(_sfxToggleBtn, sound.IsSfxEnabled());
        SaveSystem.Save(GameManager.Instance.SettingData);
    }

    private void OnToggleAttackRangeBtn()
    {
        if (GameManager.Instance?.SettingData == null)
            return;

        bool visible = !GameManager.Instance.SettingData.IsAtkRangeOn;
        GameManager.Instance.SettingData.IsAtkRangeOn = visible;
        AttackRange.SetVisibleForAll(visible);

        CheckImageActive(_attackRangeToggleBtn, GameManager.Instance.SettingData.IsAtkRangeOn);
        SaveSystem.Save(GameManager.Instance.SettingData);
    }

    private void OnToLobbyBtn()
    {
        StageManager.Instance.StageOver();
        OnQuitBtn();
    }

    private void OnLobbyCheckPanelBtn()
    {
        EventBus.Publish(new UISelectEvent(OnToLobbyBtn, "로비로 나가시겠습니까?"));
    }

    private void OnQuitBtn()
    {
        CommonUIManager.Instance.BackUI();
        SoundManager.Instance.PlaySFX("BackBtnClick", 1f);
    }

    private void CheckImageActive(Button button, bool isActive)
    {
        if (button == null || button.transform.childCount == 0)
            return;

        GameObject checkImage = button.transform.GetChild(0).gameObject;
        checkImage.SetActive(isActive);
    }

    public void VisibleUI(bool isVisible)
    {
        _settingPanel.SetActive(isVisible);

        if (GameManager.Instance.GameState == GameState.Lobby)
            _toLobbyCheckBtn.gameObject.SetActive(false);
        else
            _toLobbyCheckBtn.gameObject.SetActive(true);

        if (isVisible)
        {
            // UI 오픈 시 현재 저장된 설정값으로 갱신
            RefreshUISettings();
            CommonUIManager.Instance.AddUIStack(() => VisibleUI(false));
            SoundManager.Instance.PlaySFX("ShowSettingUI");
        }
    }

    private void RefreshUISettings()
    {
        if (GameManager.Instance == null || GameManager.Instance.SettingData == null)
            return;

        CheckImageActive(_bgmToggleBtn, GameManager.Instance.SettingData.IsBGMOn);
        CheckImageActive(_sfxToggleBtn, GameManager.Instance.SettingData.IsSFXOn);
        CheckImageActive(_attackRangeToggleBtn, GameManager.Instance.SettingData.IsAtkRangeOn);
    }

#region Event Handlers

    private void OnSettingEvent(UISettingEvent e)
    {
        VisibleUI(true);
    }

    private void OnShowVersionEvent(UIVersionEvent e)
    {
        if (_versionText == null)
            return;

        _versionText.text = e.VersionText;
    }

#endregion

}

#region EventBus

/// <summary>
/// 설정 UI 표시 이벤트
/// </summary>
public readonly struct UISettingEvent {}


/// <summary>
/// 버전 정보 표시 이벤트
/// </summary>
public readonly struct UIVersionEvent
{
    public readonly string VersionText;

    public UIVersionEvent(string versionText)
    {
        VersionText = versionText;
    }
}

#endregion
