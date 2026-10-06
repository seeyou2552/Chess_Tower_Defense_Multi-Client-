using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDataManager : Singleton<EnemyDataManager>
{
    public List<EnemyData> EnemyDatas;

    private readonly Dictionary<int, EnemyData> _enemyIndexDict = new();

    protected override void Awake()
    {
        base.Awake();
        EnemyDatas.ForEach(data => 
        {
            _enemyIndexDict.Add(data.Id, data);
        });
    }

    public EnemyData GetData(int minionId)
    {
        if (!_enemyIndexDict.ContainsKey(minionId))
        {
            Debug.Log("해당 minionId가 존재하지 않습니다.");
            return null;
        }

        return _enemyIndexDict[minionId];
    }
}
