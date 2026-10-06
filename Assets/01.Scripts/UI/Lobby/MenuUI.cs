using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private Button _stageBtn;
    [SerializeField] private Button _settingBtn;
    [SerializeField] private Button _exitBtn;

    void Start()
    {
        _stageBtn.onClick.AddListener(OnStageBtn);
        _settingBtn.onClick.AddListener(OnSettingBtn);
        _exitBtn.onClick.AddListener(OnExitBtn);
    }

    private void OnStageBtn()
    {
        EventBus.Publish(new UIStageEvent());
    }

    private void OnSettingBtn()
    {
        EventBus.Publish(new UISettingEvent());
    }

    private void OnExitBtn()
    {
        EventBus.Publish(new UISelectEvent(OnGameExitBtn, "정말로 종료하시겠습니까?"));
    }

    private void OnGameExitBtn()
    {
        GameManager.Instance.ChangeGameState(GameState.GameExit);
    }
}
