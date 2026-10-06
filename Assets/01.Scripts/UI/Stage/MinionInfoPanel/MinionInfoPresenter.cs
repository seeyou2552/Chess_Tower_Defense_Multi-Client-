using UnityEngine;

public class MinionInfoPresenter
{
    private readonly MinionInfoUI _view;
    private Minion _selectedMinion;
    private MinionState _selectedState;

    public MinionInfoPresenter(MinionInfoUI view)
    {
        _view = view;

        _view.OnSellBtn += HandleSellButton;
        _view.OnHide += OnHideInfo;
    }

    public void Dispose()
    {
        if (_view != null)
        {
            _view.OnSellBtn -= HandleSellButton;
            _view.OnHide -= OnHideInfo;
        }
    }

    public void HandleSellButton()
    {
        if (_selectedMinion == null)
            return;

        if (GameManager.Instance.GameState == GameState.Wave)
        {
            EventBus.Publish(new UIAlertEvent("웨이브 중에는 판매할 수 없습니다."));
            return;
        }

        // StageManager.Instance.AddGold(_selectedState.SellGold);
        MinionManager.Instance.SendSellMinion(_selectedMinion.State.InstanceId);
        _view.VisibleUI(false);
        _selectedMinion = null;
        _selectedState = null;

    }

    public void UpdateMinionInfo(MinionState minionState)
    {
        _view.SetMinionInfo(minionState);
        _view.SetSellPanel(true, minionState.SellGold.ToString());
        _view.SetUpgradePanel(true, minionState);
    }

    public void ShowMinionInfo(MinionState minionState, Minion minion)
    {
        if (_selectedMinion != minion && _selectedMinion != null)
        {
            _selectedMinion.SelectMinion(false);
        }

        _selectedMinion = minion;
        _selectedState = minionState;
        _selectedMinion.SelectMinion(true);

        _view.SetMinionInfo(minionState);
        _view.SetSellPanel(true, minionState.SellGold.ToString());
        _view.SetUpgradePanel(true, minionState);
        _view.VisibleUI(true);

        CommonUIManager.Instance.AddUIStack(() => _view.VisibleUI(false));

        EventBus.Publish(new MinionInfoOpenedEvent());
    }

    public void ShowMinionPreview(MinionData minionData)
    {
        if (_selectedMinion != null)
        {
            _selectedMinion.SelectMinion(false);
        }

        _selectedMinion = null;
        _selectedState = null;
        _view.SetMinionInfo(minionData);
        _view.SetSellPanel(false);
        _view.SetUpgradePanel(false);
        _view.VisibleUI(true);

        CommonUIManager.Instance.AddUIStack(() => _view.VisibleUI(false));

        EventBus.Publish(new MinionInfoOpenedEvent());
    }

    // Info가 Hide 될때 실행되는 메서드
    private void OnHideInfo()
    {
       if (_selectedMinion != null)
        {
            _selectedMinion.SelectMinion(false);
            _selectedMinion = null;
        }
    }
}
