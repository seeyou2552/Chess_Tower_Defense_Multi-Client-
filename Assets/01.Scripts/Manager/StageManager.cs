using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

// using Firebase.Analytics;
using UnityEngine;
using UnityEngine.Tilemaps;


public class StageManager : Singleton<StageManager>
{
    [Header("Stage Data")]
    public List<StageData> Stages = new List<StageData>();
    public int CurrentStage { get; private set; }
    public int CurrentWave = 0;
    public int CurrentHP = 0;
    public int StageGold { get; private set; }

    [Header("Tilemap")]
    public Tilemap MinionTilemap;

    private Coroutine _spawnCoroutine;


    void OnEnable()
    {
        EventBus.Subscribe<WaveStartRequestEvent>(WaveStart);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<WaveStartRequestEvent>(WaveStart);
    }

    void Start()
    {
        NetworkManager.Instance.RegisterPacketHandler(
            PacketType.StartWaveNotify,
            HandleStartWaveNotify
        );

        NetworkManager.Instance.RegisterPacketHandler(
            PacketType.EndWaveNotify,
            HandleEndWaveNotify
        );

        NetworkManager.Instance.RegisterPacketHandler(
            PacketType.UpdateGold,
            HandleUpdateGold
        );

        NetworkManager.Instance.RegisterPacketHandler(
            PacketType.EndStageNotify,
            EndStageNotify
        );
    }

    public void StartStage()
    {
        GameManager.Instance.ChangeGameState(GameState.Intermission);
        CurrentWave = 0;
        
        EventBus.Publish(new UIGoldChangedEvent(StageGold));
        EventBus.Publish(new UIHPChangedEvent(CurrentHP, PlayerManager.Instance.Data.MaxHP));
        
        SoundManager.Instance.PlayBGM("StageBGM");
    }

    private void StageClear()
    {
        GameManager.Instance.ChangeGameState(GameState.GameClear);
    }

    public void StageOver()
    {
        if(_spawnCoroutine != null)
            StopCoroutine(_spawnCoroutine);


        GameManager.Instance.ChangeGameState(GameState.GameOver);
    }

    private void StartWave()
    {
        EventBus.Publish(new WaveStartEvent());
    }

    private void EndWave()
    {
        EventBus.Publish(new WaveEndEvent());
        GameManager.Instance.ChangeGameState(GameState.Intermission);
    }

    public bool CheckGold(int value)
    {
        if(StageGold - value < 0)
        {
            EventBus.Publish(new UIAlertEvent("골드가 부족합니다."));
            return false;
        }
 
        return true;
    }

    public void UpdateGold(int currentGold)
    {
        StageGold = currentGold;
        EventBus.Publish(new UIGoldChangedEvent(StageGold));
    }

    public void UpdateHp(int currentHp)
    {
        CurrentHP = currentHp;
        EventBus.Publish(new UIHPChangedEvent(CurrentHP, 100));
    }

    public Dictionary<EnemyData, int> GetCurrentWaveEnemyList() 
    {
        Dictionary<EnemyData, int> enemyDict = new Dictionary<EnemyData, int>();

        foreach(var enemyList in Stages[CurrentStage-1].Wave[CurrentWave].EnemyList)
        {
            if (!enemyDict.ContainsKey(enemyList.EnemyData))
                enemyDict[enemyList.EnemyData] = 0;
            
            enemyDict[enemyList.EnemyData] += enemyList.Count;
        }

        return enemyDict;
    }

    public async Task SendStartWave()
    {
        StartWaveRequest request = new StartWaveRequest
        {

        };

        try
        {

            bool sent = await NetworkManager.Instance.SendAsync(
                PacketType.StartWaveRequest,
                request
            );

            if (!sent)
            {
                Debug.LogError(
                    "Start Wave request failed: not connected."
                );

                return;
            }

        }
        catch (Exception e)
        {
            Debug.LogError(
                $"Start Wave request failed: {e.Message}"
            );
        }
    }

    private void HandleStartWaveNotify(byte[] payload)
    {
        try
        {
            StartWaveNotify notify =
                NetworkManager.BytesToStruct<StartWaveNotify>(
                    payload
                );

            NetworkManager.Instance.AddAction(()=>
            {
                StartWave();
                EventBus.Publish(new WaveStartRequestEvent());
            });

        }
        catch (Exception e)
        {
            Debug.LogError(
                $"Start Wave Parse Failed: {e.Message}"
            );
        }
    }

    private void HandleEndWaveNotify(byte[] payload)
    {
        try
        {
            EndWaveNotify notify =
                NetworkManager.BytesToStruct<EndWaveNotify>(
                    payload
                );

            NetworkManager.Instance.AddAction(()=>
            {
                EndWave();
            });

        }
        catch (Exception e)
        {
            Debug.LogError(
                $"End Wave Parse Failed: {e.Message}"
            );
        }
    }

    private void HandleUpdateGold(byte[] payload)
    {
        try
        {
            UpdateGold notify =
                NetworkManager.BytesToStruct<UpdateGold>(
                    payload
                );

            NetworkManager.Instance.AddAction(()=>
            {
                UpdateGold(notify.currentGold);
            });

        }
        catch (Exception e)
        {
            Debug.LogError(
                $"Update Gold Parse Failed: {e.Message}"
            );
        }
    }

    private void EndStageNotify(byte[] payload)
    {
        try
        {
            EndStageNotify notify =
                NetworkManager.BytesToStruct<EndStageNotify>(
                    payload
                );

            NetworkManager.Instance.AddAction(()=>
            {
                if (notify.success == 1)
                    StageClear();

                else
                    StageOver();
            });

        }
        catch (Exception e)
        {
            Debug.LogError(
                $"End Stage Parse Failed: {e.Message}"
            );
        }
    }

    public void SetStageNumber(int index)
    {
        CurrentStage = index;
    }


#region Event Handlers

    private void WaveStart(WaveStartRequestEvent e)
    {
        if(GameManager.Instance.GameState != GameState.Intermission)
            return;

        GameManager.Instance.ChangeGameState(GameState.Wave);
        EventBus.Publish(new WaveStartEvent());
    }

#endregion

}

#region EventBus

 /// <summary>
 /// 웨이브 시작 요청
 /// </summary>
public readonly struct WaveStartRequestEvent {}

/// <summary>
///  웨이브 시작 시 발생할 이벤트
/// </summary>
public readonly struct WaveStartEvent {}

/// <summary>
///  웨이브 끝날 시 발생할 이벤트
/// </summary>
public readonly struct WaveEndEvent {}

#endregion