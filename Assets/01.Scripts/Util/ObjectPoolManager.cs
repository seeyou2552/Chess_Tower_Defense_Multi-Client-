using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    private readonly Dictionary<string, Queue<PoolObject>> _pool = new();
    private readonly Queue<SkillEffect> _skillPool = new();
    private readonly Dictionary<string, Queue<EffectObject>> _effectPool = new();
    private readonly HashSet<PoolObject> _activeObjectPool = new();

    public GameObject Get(string key, GameObject prefab, Vector3 pos, GameObject parent = null)
    {
        if (!_pool.ContainsKey(key))
        {
            _pool[key] = new Queue<PoolObject>();
        }
    
        PoolObject obj;

        if (_pool[key].Count > 0)
        {
            obj = _pool[key].Dequeue();
            
            if (obj.TryGetComponent<PoolObject>(out PoolObject poolObj))
            {
                EnterActivePool(poolObj);
            }

        }
        else
        {
            if (parent != null)
            {
                obj = Instantiate(prefab, pos, Quaternion.identity, parent.transform).GetComponent<PoolObject>(); 
            }
                
            else
            {
                obj = Instantiate(prefab, pos, Quaternion.identity).GetComponent<PoolObject>()  ;
                DontDestroyOnLoad(obj);
            }

            obj.name = prefab.name;
        }

        obj.transform.position = pos;
        obj.gameObject.SetActive(true);
        EnterActivePool(obj);
        
        return obj.gameObject;
    }

    public void Return(string key, PoolObject obj)
    {
        obj.gameObject.SetActive(false);
        _pool[key].Enqueue(obj);
        _activeObjectPool.Remove(obj);
    }

#region SkillPool

    public SkillEffect GetSkillEffect(SkillEffect prefab, GameObject parent)
    {
        SkillEffect skillObj;
        if (_skillPool.Count > 0)
        {
            skillObj = _skillPool.Dequeue();
        }
        else
        {
            // 부족하면 새로 생성
            skillObj = Instantiate(prefab, parent.transform);
        }

        skillObj.gameObject.SetActive(true);
        EnterActivePool(skillObj);

        return skillObj;
    }

    public void ReturnSkillEffect(SkillEffect skillObj)
    {
        skillObj.gameObject.SetActive(false);
        _skillPool.Enqueue(skillObj);
        _activeObjectPool.Remove(skillObj);
    }

#endregion

#region EffectPool

    public EffectObject GetEffect(string key, EffectObject prefab, GameObject parent)
    {
        if (!_effectPool.ContainsKey(key))
        {
            _effectPool[key] = new Queue<EffectObject>();
        }
    
        EffectObject effectObj;

        if (_effectPool[key].Count > 0)
        {
            effectObj = _effectPool[key].Dequeue();
            
            EnterActivePool(effectObj);

        }
        else
        {

            effectObj = Instantiate(prefab, parent.transform);
            effectObj.name = prefab.name;
        }

        effectObj.gameObject.SetActive(true);
        EnterActivePool(effectObj);

        return effectObj;
    }

    public void ReturnEffect(string key, EffectObject effectObj)
    {
        effectObj.gameObject.SetActive(false);
        _effectPool[key].Enqueue(effectObj);
        _activeObjectPool.Remove(effectObj);
    }

#endregion


#region ActivePool

    private void EnterActivePool(PoolObject obj)
    {
        _activeObjectPool.Add(obj);
    }

    public void ReturnAllActiveObject()
    {
        foreach (var obj in _activeObjectPool.ToArray())
        {
            if (obj == null)
                continue;

            obj.ReturnToPool();
        }

        _activeObjectPool.Clear();
    }

#endregion

}