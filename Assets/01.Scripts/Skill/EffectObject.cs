using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectObject : PoolObject
{
    [SerializeField] private AnimationClipController _acc;

    private bool _isReturned = false;
    void Start()
    {
        if (_acc == null)
            _acc = gameObject.GetComponent<AnimationClipController>();
    }

    void OnEnable()
    {
        _isReturned = false;
    }

    void OnDisable()
    {
        _isReturned = true;
    }

    public void PlayEffect(AnimationClip animClip)
    {
        if (_acc == null)
            return;

        _acc.PlayAnimation(animClip, ReturnToPool);
    }

    public void LoopPlayEffect(AnimationClip animClip, float duration)
    {
        if (_acc == null)
            return;
        
        _acc.LoopPlayAnimation(animClip, ReturnToPool, duration);
    }

    public override void ReturnToPool()
    {
        if (_isReturned)
            return;
            
        _isReturned = true;
        ObjectPoolManager.Instance.ReturnEffect(gameObject.name, this);
        transform.localScale = Vector3.one;
    }
}
