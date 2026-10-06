using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [Header("Tutorial Component")]
    [SerializeField] private TutorialSystem _system;
    [SerializeField] private TutorialUI _ui;
    [SerializeField] private TutorialInput _input;

    private bool _isStop = false;
    private Action _unsubscribeAction;

    public void Start()
    {
        StartCoroutine(ShowTutorialConfirm());
    }

    public void NextStep()
    {
        if (_isStop)
        {
            _input.IsRunning = false;
            _ui.gameObject.SetActive(false);
            return;
        }

        if (_system.NextStep())
            ShowStep();

        else CompleteTutorial();
    }

    private void ShowStep()
    {
        var step = _system.GetStep();
        _ui.Render(step);

        _input.IsRunning = true;
        _input.CanTouchToProgress = false;
        StartCoroutine(EnableTouchProgressCoroutine(0.5f));

        if (step.EventType != TutorialEventType.None)
            _isStop = true;
            
        switch (step.EventType)
        {
            case TutorialEventType.ShowMinionSelectUI :
                EventBus.Subscribe<MinionSelectUIOpenedEvent>(MissionComplete);
                _unsubscribeAction = () => EventBus.Unsubscribe<MinionSelectUIOpenedEvent>(MissionComplete);
                break;

            case TutorialEventType.SpawnMinion :
                EventBus.Subscribe<SpwanMinionEvent>(MissionComplete);
                _unsubscribeAction = () => EventBus.Unsubscribe<SpwanMinionEvent>(MissionComplete);
                break;

            case TutorialEventType.ShowMinionInfoUI :
                EventBus.Subscribe<MinionInfoOpenedEvent>(MissionComplete);
                _unsubscribeAction = () => EventBus.Unsubscribe<MinionInfoOpenedEvent>(MissionComplete);
                break;

            case TutorialEventType.ShowWaveEnemyInfoUI :
                EventBus.Subscribe<WaveEnemyInfoOpenedEvent>(MissionComplete);
                _unsubscribeAction = () => EventBus.Unsubscribe<WaveEnemyInfoOpenedEvent>(MissionComplete);
                break;
        }

        
    }

    /// <summary>
    /// SelectUI를 통해 튜토리얼 동의 팝업 표시
    /// </summary>
    private IEnumerator ShowTutorialConfirm()
    {
        yield return YieldCache.GetWaitForSecondsRealtime(2f);

        EventBus.Publish(new UISelectEvent
        (
            StartTutorial,
            "튜토리얼을 진행하시겠습니까?"
        ));
    }
    
    private void StartTutorial()
    {
        CommonUIManager.Instance.BackUI();

        if (_ui == null)
            _ui = GetComponentInChildren<TutorialUI>();

        if (_input == null)
            _input = GetComponent<TutorialInput>();
        
        if (_system == null)
            _system = GetComponent<TutorialSystem>();
        
        if(_ui != null)
        {
            _ui.gameObject.SetActive(true);
            _system.Reset();
            ShowStep();
        }

        if (_input != null)
        {
            _input.OnTap += NextStep;
        }
    }

    public void CompleteTutorial()
    {   
        if(_ui != null)
        {
            _ui.gameObject.SetActive(false);
        }

        if (_input != null)
        {
            _input.OnTap -= NextStep;
            _input.IsRunning = false;
        }
        
    }

    private void MissionComplete<T>(T e)
    {
        _isStop = false;
        _unsubscribeAction?.Invoke();
        NextStep();
    }

    private IEnumerator EnableTouchProgressCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        _input.CanTouchToProgress = true;
    }
}
