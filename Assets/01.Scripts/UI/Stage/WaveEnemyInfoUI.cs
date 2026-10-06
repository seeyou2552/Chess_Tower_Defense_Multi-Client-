using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveEnemyInfoUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject _waveInfoPanel;

    [Header("Image")]
    [SerializeField] private Image _enemyImage;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _waveText;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _pieceText;
    [SerializeField] private TextMeshProUGUI _rewardText;


    [Header("Button")]
    [SerializeField] private Button _showWaveInfoBtn;
    [SerializeField] private Button _quitBtn;
    [SerializeField] private Button _leftBtn;
    [SerializeField] private Button _rightBtn;

    private List<EnemyCount> _enemyList= new List<EnemyCount>();
    private int _showingWave = -1;
    private int _enemyIndex = 0;

    void Start()
    {
        if (_showWaveInfoBtn != null)
            _showWaveInfoBtn.onClick.AddListener(OnShowWaveInfoBtn);
        
        if (_quitBtn != null)
            _quitBtn.onClick.AddListener(OnBackBtn);

        if (_leftBtn != null)
            _leftBtn.onClick.AddListener(OnLeft);

        if (_rightBtn != null)
            _rightBtn.onClick.AddListener(OnRight);

    }

    void OnLeft() => OnChangeEnemyListBtn(-1);
    void OnRight() => OnChangeEnemyListBtn(1);

    private void ShowEnemyInfo()
    {
        _waveInfoPanel.SetActive(true);

        EventBus.Publish(new WaveEnemyInfoOpenedEvent());

        if (_showingWave != StageManager.Instance.CurrentWave)
            EnemyListSetting();
        
        if (_enemyList.Count == 0 || _enemyIndex >= _enemyList.Count)
        {
            CommonUIManager.Instance.BackUI();
            return;
        }

        SetEnemyInfo(_enemyList[_enemyIndex]);

    }

    private void SetEnemyInfo(EnemyCount enemy)
    {
        // Image
        if(_enemyImage == null)
            return;

        _enemyImage.sprite = enemy.EnemyData.Sprite;

        // Text
        _waveText.text = $"Wave {StageManager.Instance.CurrentWave + 1}";
        _nameText.text = enemy.EnemyData.name;
        _pieceText.text = enemy.Count.ToString();
        _rewardText.text = enemy.EnemyData.Reward.ToString();
    }



    private void EnemyListSetting()
    {
        _enemyList.Clear();
        _enemyIndex = 0;

        Dictionary<EnemyData, int> enemyDict = StageManager.Instance.GetCurrentWaveEnemyList();

        foreach (var pair in enemyDict.OrderBy(x => x.Key.name))
        {
            _enemyList.Add(new EnemyCount(pair.Key, pair.Value));
        }
        _showingWave = StageManager.Instance.CurrentWave;
    }

    private void OnShowWaveInfoBtn()
    {
        VisibleUI(true);
    }

    private void OnChangeEnemyListBtn(int index)
    {
        _enemyIndex += index;

        _enemyIndex = (_enemyIndex + _enemyList.Count) % _enemyList.Count;

        SetEnemyInfo(_enemyList[_enemyIndex]);
        SoundManager.Instance.PlaySFX("SwapBtnClick");
    }

    private void VisibleUI(bool isVisible)
    {
        if (isVisible)
        {
            CommonUIManager.Instance.AddUIStack(() => VisibleUI(false));
            ShowEnemyInfo();
        }
        else if (!isVisible)
        {
            _waveInfoPanel.SetActive(false);
        }
    }

    private void OnBackBtn()
    {
        CommonUIManager.Instance.BackUI();
        SoundManager.Instance.PlaySFX("BackBtnClick", 1f);
    }
}

[System.Serializable]
public class EnemyCount
{
    public EnemyData EnemyData;
    public int Count;

    public EnemyCount(EnemyData enemyData, int count)
    {
        EnemyData = enemyData;
        Count = count;
    }
}

#region Events

public readonly struct WaveEnemyInfoOpenedEvent {};

#endregion