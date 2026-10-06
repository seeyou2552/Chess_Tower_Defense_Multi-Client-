// using Firebase.Analytics;
using UnityEngine;

public class MinionPresenter
{
    private readonly Minion _view;

    public MinionState State { get; private set; }

    public MinionPresenter(Minion view)
    {
        _view = view;
    }

    public void Init(MinionData minionData)
    {
        if (State == null)
            State = new MinionState();

        State.Setup(minionData);
        State.OnStatChanged += OnStatChanged;

        State.RuntimeStat.OnRangeChanged += OnRangeChanged; 

        SetDrag();

        _view.OnTouch += OnTouch; 

        _view.SetSprite(minionData.MinionSprite);
        _view.AttackRange.SetRange(State.Data.AttackRange);

        State.IsFirstWave = true;
        _view.PlayIdleAnimation(minionData.MinionAnim);

        _view.OnReturn += ResetForPool;
        MinionTypeAbility.SubscribeAbility(State);
    }

    public void OnEnable()
    {
        EventBus.Subscribe<WaveStartEvent>(OnWaveStart);
        EventBus.Subscribe<WaveEndEvent>(OnWaveEnd);
    }

    public void OnDisable()
    {
        MinionTypeAbility.RemoveSubscribeAbility(State);
        EventBus.Unsubscribe<WaveStartEvent>(OnWaveStart);
        EventBus.Unsubscribe<WaveEndEvent>(OnWaveEnd);
    }

    public void ApplyBuff(Buff buff)
    {
        State.BuffController.ApplyBuff(buff);
    }

    private void OnWaveStart(WaveStartEvent e)
    {
        //  해당 Minion생성 후에 첫 웨이브 시작 시
        if (State.IsFirstWave)
        {
            // 웨이브 시작 시 Cost에 절반 만큼 감소 
            State.SellGoldUpdate(- State.Data.Cost / 2);
            State.IsFirstWave = false;

        }
        State.IsSiege = true;
    }

    public void OnWaveEnd(WaveEndEvent e)
    {
        State.WaveEndReset();
        State.BuffController.ClearBuffList();
        OnStatChanged();
    }

    private void Cleanup()
    {
        if (State == null)
            return;

        State.OnStatChanged -= OnStatChanged;

        _view.OnTouch -= OnTouch; 

        CleanupDrag();
    }

    public void ResetForPool()
    {
        State?.RuntimeStat.ResetRuntimeStat();
        State?.BuffController.ClearBuffList();

        _view.SelectMinion(false);
        _view.AttackRange.SetOriginalColor();

        Cleanup();
    }

    private void OnStatChanged()
    {
        // 선택된 상태에서 스텟 변경시 변경사항 UI업데이트
        if (_view.IsSelected)
            StageUIController.Instance.ShowMinionInfo(State, _view);
    }

    private void OnTouch()
    {
        StageUIController.Instance.ShowMinionInfo(State, _view);
    }

    private void OnRangeChanged(float value)
    {
        _view.AttackRange.RangeIncrease(value);
    }

#region Drag

    private void SetDrag()
    {
        _view.Drag.OnDragStart += HandleDragStart;
        _view.Drag.OnDragEnd += HandleDragEnd;

        _view.Drag.OnPlacementFailedAndReturn += ReturnCellMinion;
        _view.Drag.OnPlacementSuccess += HandlePlacementSuccess;
    }

    private void CleanupDrag()
    {
        _view.Drag.OnDragStart -= HandleDragStart;
        _view.Drag.OnDragEnd -= HandleDragEnd;

        _view.Drag.OnPlacementFailedAndReturn -= ReturnCellMinion;
        _view.Drag.OnPlacementSuccess -= HandlePlacementSuccess;
    }

    private void HandleDragStart()
    {
        CommonUIManager.Instance.AllHideUI();
        SoundManager.Instance.PlaySFX("MinionDragStart");
        _view.SelectMinion(true);
    }

    private void HandleDragEnd()
    {
        SoundManager.Instance.PlaySFX("DropMinion");
        _view.SelectMinion(false);
    }

    private void HandlePlacementSuccess()
    {
        EventBus.Publish(new SpwanMinionEvent());
    }

    private void ReturnCellMinion()
    {
        // 미니언 판매 처리
        // StageManager.Instance.AddGold(State.Data.Cost);
        _view.ReturnToPool();

        EventBus.Publish(new UIAlertEvent("체스 말이 설치되지 않아 자동으로 판매되었습니다."));
    }

#endregion
}
