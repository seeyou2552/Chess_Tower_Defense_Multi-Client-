using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class PlayerData 
{
    public string PlayerName;
    public string PlayerCode;
    public int Gold;
    public int CurrentStage;
    public int MaxHP = 100;
    public int StartGold = 200;
    public List<int> MinionList; // minion index를 저장
    
}
