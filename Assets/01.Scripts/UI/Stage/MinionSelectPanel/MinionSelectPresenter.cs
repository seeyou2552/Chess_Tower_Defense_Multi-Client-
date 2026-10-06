using System.Collections.Generic;
using System.Linq;

public class MinionSelectPresenter
{
    private readonly MinionSelcetUI _view;

    
    private bool _isShowPanel = false;

    public MinionSelectPresenter(MinionSelcetUI view)
    {
        _view = view;

        _view.OnToggleBtn += OnTogglePanel;
        _view.OnShow += OnShowSelectUI;

        Init();
    }

    public void Dispose()
    {
        if (_view != null)
        {
            _view.OnToggleBtn -= OnTogglePanel;
            _view.OnShow -= OnShowSelectUI;
        }
    }

    public void Init()
    {
        if (PlayerManager.Instance == null || PlayerManager.Instance.Data?.MinionList == null)
            return;

        _view.PopulateCards(MinionDataManager.Instance.MinionDatas);
    }
    
    public void OnTogglePanel()
    {
        _view.UpDownToggleUI();
    }

    private void OnShowSelectUI()
    {
        CommonUIManager.Instance.AddUIStack(_view.UpDownToggleUI);

        // open 시 Event 발행
        EventBus.Publish(new MinionSelectUIOpenedEvent());
    }
}
