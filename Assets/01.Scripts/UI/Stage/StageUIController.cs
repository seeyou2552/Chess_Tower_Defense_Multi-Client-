using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.Runtime.ExceptionServices;

public class StageUIController : MonoBehaviour
{
    public static StageUIController Instance { get; private set; } 

    [Header("Select UI")]
    [SerializeField] private MinionSelcetUI _minionSelectUI;
    private MinionSelectPresenter _minionSelectPresenter;


    [Header("Info UI")]
    [SerializeField] private MinionInfoUI _minionInfoUI;
    private MinionInfoPresenter _minionInfoPresenter;

    [Header("Wave UI")]

    [Header("Button")]
    [SerializeField] private Button _settingBtn;

    void Awake()
    {
        Instance = this;

        if (_minionSelectUI == null)
        {
            _minionSelectUI = FindObjectOfType<MinionSelcetUI>();
        }

        if (_minionSelectUI != null)
            _minionSelectPresenter = new MinionSelectPresenter(_minionSelectUI);


        _minionInfoPresenter = new MinionInfoPresenter(_minionInfoUI);
    }

    void OnEnable() 
    {
        if (_settingBtn != null)
            _settingBtn.onClick.AddListener(OnSettingBtn);
    }

    void OnDisable()
    {


        if (_settingBtn != null)
            _settingBtn.onClick.RemoveAllListeners();
    }

    void OnDestroy()
    {
        _minionInfoPresenter.Dispose();
        _minionSelectPresenter.Dispose();

        Instance = null;
    }

#region Minion Select UI
    public void ToggleMinionSelectUI() => _minionSelectPresenter.OnTogglePanel();

#endregion

#region Minion Info UI
    public void UpdateMinionInfo(MinionState minionState) => _minionInfoPresenter.UpdateMinionInfo(minionState);
    public void ShowMinionInfo(MinionState minionState, Minion minion) => _minionInfoPresenter.ShowMinionInfo(minionState, minion);
    public void ShowMinionInfo(MinionData minionData) => _minionInfoPresenter.ShowMinionPreview(minionData);

#endregion

    

    // Setting UI
    private void OnSettingBtn()
    {
        // 세팅 버튼 클릭 시 Setting UI 호출
        EventBus.Publish(new UISettingEvent());
    }
}
