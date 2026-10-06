using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;


public class GameManager : Singleton<GameManager>
{
    public ShopData ShopData { get; private set;}
    public SettingData SettingData { get; private set;} 
    public GameState GameState { get; private set; }

    async void Awake()
    {
        base.Awake();

        // 프레임 설정
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        SettingData = SaveSystem.LoadSettingData();
        ApplySettings();
    }


    void Start()
    {
        GameState = GameState.Account;

        NetworkManager.Instance.RegisterPacketHandler(
            PacketType.StageStartResponse,
            HandleStageStartNotify
        );
    }

    public void ChangeGameState(GameState newState)
    {
        GameState = newState;

        switch(GameState)
        {
            case GameState.Wave :
                break;
                
            case GameState.Intermission :
                break;

            case GameState.GameClear :
                GameClear();
                break;

            case GameState.GameOver :
                GameOver();
                break;

            case GameState.GameExit :
                GameExit();
                break;

            case GameState.Lobby :
                SceneFlowManager.Instance.LoadLobbyScene(LobbySetting);
                break;

            case GameState.StageStart :
                GameStart();
                break;

        }
    }

    public void GameStart()
    {
        SceneFlowManager.Instance.LoadStageScene(StageManager.Instance.CurrentStage, StageSetting);
    }

    private void LobbySetting()
    {
        EventBus.Publish(new UIGoldChangedEvent(PlayerManager.Instance.Data.Gold));
        SoundManager.Instance.PlayBGM("LobbyBGM");
    }

    private void StageSetting()
    {
        StageManager.Instance.StartStage();
    }

    public void ApplySettings()
    {
        if (SettingData == null)
            return;

        SoundManager.Instance?.ApplySettings(SettingData);
        AttackRange.SetVisibleForAll(SettingData.IsAtkRangeOn);
    }


    private void GameOver()
    {
        EventBus.Publish(new UIResultEvent(false));
    }

    private void GameClear()
    {
        EventBus.Publish(new UIResultEvent(true));
    }

    private void GameExit()
    {
        SoundManager.Instance.StopBGM();
        Application.Quit();
    }

    public async void SendJoinStage(int stageId)
    {
        JoinRoomRequest request = new JoinRoomRequest
        {
            stageId = stageId
        };

        try
        {
            bool sent = await NetworkManager.Instance.SendAsync(
                PacketType.JoinRoomRequest,
                request
            );

            if (!sent)
            {
                Debug.LogError(
                    "Join stage request failed: not connected."
                );

                return;
            }

        }
        catch (Exception e)
        {
            Debug.LogError(
                $"Join stage request failed: {e.Message}"
            );
        }
    }

    private void HandleStageStartNotify(byte[] payload)
    {
        try
        {
            StageStartNotify notify =
                NetworkManager.BytesToStruct<StageStartNotify>(
                    payload
                );

            if (notify.errCode != ErrorCode.None)
            {
                Debug.LogError(
                    $"Join Failed: {notify.errCode}"
                );

                return;
            }

            NetworkManager.Instance.AddAction(()=>
            {
                ChangeGameState(GameState.StageStart);
                StageManager.Instance.UpdateGold(notify.gold);
                StageManager.Instance.UpdateHp(100);
            });


        }
        catch (Exception e)
        {
            Debug.LogError(
                $"JoinStageResponse Parse Failed: {e.Message}"
            );
        }
    }
}
