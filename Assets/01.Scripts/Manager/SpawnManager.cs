using System;
using System.Collections;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
    [Header("Minion")]
    [SerializeField] private GameObject _minionPrefab;
    [SerializeField] private GameObject _minionPool;

    [Header("Enemy")]
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyPool;

    [Header("Enemy HPBar")]
    [SerializeField] private HPBarUI _hpBarPrefab;
    [SerializeField] private GameObject _hpBarPool;

    [Header("Skill")]
    [SerializeField] private SkillEffect _skillPrefab;
    [SerializeField] private GameObject _skillPool;

    [Header("Effect")]
    [SerializeField] private EffectObject _effectPrefab;
    [SerializeField] private GameObject _effectPool;

    void Start()
    {
        if(_enemyPrefab == null)
            _enemyPrefab = Resources.Load<GameObject>("Enemy/DefaultEnemy");
        if(_minionPrefab == null)
            _minionPrefab = Resources.Load<GameObject>("Minion/DefaultMinion");
        if(_skillPrefab == null)
            _skillPrefab = Resources.Load<SkillEffect>("Skill/DefaultSkillObject");
        if(_effectPrefab == null)
            _effectPrefab = Resources.Load<EffectObject>("Skill/DefaultAfterObject");
        if(_hpBarPrefab == null)
            _hpBarPrefab = Resources.Load<HPBarUI>("UI/HPBar");

        NetworkManager.Instance.RegisterPacketHandler(
            PacketType.SpawnMinionNotify,
            HandleSpawnMinionNotify
        );

        NetworkManager.Instance.RegisterPacketHandler(
            PacketType.SpawnEnemyNotify,
            HandleSpawnEnemyNotify
        );
    }


    public Minion SpawnMinion(MinionData minionData, uint instanceId, int ownerId)
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        
        GameObject createMinion = ObjectPoolManager.Instance.Get(_minionPrefab.name, _minionPrefab, pos, _minionPool);

        Minion minion = createMinion.GetComponent<Minion>();
        minion.Init(minionData);

        minion.State.SetInstanceId(instanceId);
        minion.State.SetOwnerId(ownerId);

        return minion;
    }


    public Enemy SpawnEnemy(EnemyData enemyData, uint instanceId, int pathIndex)
    {

        GameObject enemyObj = ObjectPoolManager.Instance.Get(
            _enemyPrefab.name,
            _enemyPrefab,
            PathManager.Instance.GetPoint(pathIndex, 0).position,
            _enemyPool
            );
        
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        enemy.Movement.SetTarget(pathIndex);
        enemy.Init(enemyData);

        enemy.SetInstanceId(instanceId);

        return enemy;
    }

    public HPBarUI GetHPBar()
    {
        HPBarUI hPBar = Instantiate(_hpBarPrefab, _hpBarPool.transform);

        return hPBar;
    }


    public SkillEffect GetSkillObject(Vector3 pos)
    {
        SkillEffect skillObj = ObjectPoolManager.Instance.GetSkillEffect(_skillPrefab, _skillPool);
        skillObj.transform.position = pos;

        return skillObj;
    }


    public EffectObject GetEffect(Vector3 pos, EffectObject effectObjectPrefab = null)
    {
        EffectObject effectObj;
        
        if (effectObjectPrefab != null)
            effectObj = ObjectPoolManager.Instance.GetEffect(effectObjectPrefab.name, effectObjectPrefab, _effectPool);
        
        else
            effectObj = ObjectPoolManager.Instance.GetEffect(_effectPrefab.name, _effectPrefab, _effectPool);
        
        effectObj.transform.position = pos;
        
        return effectObj;
    }

    private void HandleSpawnMinionNotify(byte[] payload)
    {
        try
        {
            SpawnMinionNotify notify =
                NetworkManager.BytesToStruct<SpawnMinionNotify>(
                    payload
                );

            NetworkManager.Instance.AddAction(()=>
            {
                MinionData data = MinionDataManager.Instance.GetData((int)notify.minionId);
                if (data == null)
                    return;

                Minion minion = SpawnMinion(data, notify.instanceId, notify.orderId);
                if (minion == null)
                    return;

                minion.SetPosition((float)notify.x / 2.0f, (float)notify.y / 2.0f);

                MinionManager.Instance.RegisterMinion(notify.instanceId, minion);
            });

        }
        catch (Exception e)
        {
            Debug.LogError(
                $"Start Wave Parse Failed: {e.Message}"
            );
        }
    }

    private void HandleSpawnEnemyNotify(byte[] payload)
    {
        try
        {
            SpawnEnemyNotify notify =
                NetworkManager.BytesToStruct<SpawnEnemyNotify>(
                    payload
                );

            NetworkManager.Instance.AddAction(()=>
            {
                EnemyData data = EnemyDataManager.Instance.GetData((int)notify.enemyId);
                if (data == null)
                    return;

                Enemy enemy = SpawnEnemy(data, notify.instanceId, notify.pathIndex);
                if (enemy == null)
                    return;

                EnemyManager.Instance.RegisterEnemy(notify.instanceId, enemy);
            });

        }
        catch (Exception e)
        {
            Debug.LogError(
                $"Start Wave Parse Failed: {e.Message}"
            );
        }
    }
    
}