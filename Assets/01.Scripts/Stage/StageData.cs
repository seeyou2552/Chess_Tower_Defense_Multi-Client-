using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TD/StageData")]
public class StageData : ScriptableObject
{
    public List<Wave> Wave;
    public Sprite MapThumbnail;
    public Sprite StageLevelIcon;
    public int ClearGold;

}

[Serializable]
public class Wave
{
    public List<WaveEnemy> EnemyList;

}

[Serializable]
public class WaveEnemy
{
    public EnemyData EnemyData;
    public int Count;
    public float SpawnDelay;
}
