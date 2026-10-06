using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    public PlayerData Data { get; private set; }
    public uint PlayerId { get; private set; }

    void Awake()
    {
        base.Awake();
        Data = SaveSystem.LoadPlayerData();
    }

    public void SetPlayerId(uint playerId)
    {
        PlayerId = playerId;
    }

}
