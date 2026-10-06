using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    [Header("Result Object")]
    [SerializeField] private GameObject _resultPanel;
    [SerializeField] private Button _multiBtn;
    [SerializeField] private Button _lobbyBtn;
    [SerializeField] private GameObject _victoryObj;
    [SerializeField] private GameObject _defeatObj;


    [Header("Sprite")]
    [SerializeField] private Sprite _nextBtnSprite;
    [SerializeField] private Sprite _retryBtnSprite;

    private Image _multiBtnImage;

    void Start()
    {
        if(_lobbyBtn == null)
            return;

        _lobbyBtn.onClick.AddListener(OnLobbyBtn);
    }

    void OnEnable()
    {
        EventBus.Subscribe<UIResultEvent>(OnResultEvent);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<UIResultEvent>(OnResultEvent);
    }

    public void ShowResult(bool success)
    {
        // 스테이지 클리어 여부에 따라 다른 결과 표시
        if(success)
        {
            StageClear();
        }
        else
        {
            StageDefeat();
        }

        VisibleUI(true);
    }

    private void StageClear()
    {
        _victoryObj.SetActive(true);

        _multiBtn.onClick.RemoveAllListeners();
        _multiBtn.onClick.AddListener(OnNextStageBtn);
        
        if (_multiBtnImage == null)
            _multiBtnImage = _multiBtn.GetComponent<Image>();
        
        _multiBtnImage.sprite = _nextBtnSprite;
    }

    private void StageDefeat()
    {
        _defeatObj.SetActive(true);

        _multiBtn.onClick.RemoveAllListeners();
        _multiBtn.onClick.AddListener(OnRetryBtn);
        
        if (_multiBtnImage == null)
            _multiBtnImage = _multiBtn.GetComponent<Image>();
        
        _multiBtnImage.sprite = _retryBtnSprite;
    }

    public void VisibleUI(bool isVisible)
    {
        _resultPanel.SetActive(isVisible);

        if (isVisible)
            CommonUIManager.Instance.AllHideUI();
            
    }


    #region Button Evnet
    private void OnLobbyBtn()
    {
        VisibleUI(false);
        GameManager.Instance.ChangeGameState(GameState.Lobby);
    }

    private void OnNextStageBtn()
    {
        VisibleUI(false);
        SceneFlowManager.Instance.LoadStageScene(StageManager.Instance.CurrentStage+1, StageManager.Instance.StartStage);
    }

    private void OnRetryBtn()
    {
        VisibleUI(false);
        SceneFlowManager.Instance.ReloadCurrentScene(StageManager.Instance.StartStage);
    }
    #endregion

    #region Event Handlers

    public void OnResultEvent(UIResultEvent resultEvent)
    {
        ShowResult(resultEvent.IsClear);
    }

    #endregion 
}

#region EventBus

public readonly struct UIResultEvent
{
    public readonly bool IsClear;
    public UIResultEvent(bool clear)
    {
        IsClear = clear;
    }
}

#endregion