using UnityEngine;
using System;
using System.Collections;
using System.Threading;
using UnityEngine.Pool;

public class AnimationClipController : MonoBehaviour
{
    [Header("Default Option")]
    [SerializeField] private AnimationClip _defaultClip;
    [SerializeField] private string _targetStateName = "Active";

    private Animator _animator;
    private AnimatorOverrideController _overrideController;

    void OnEnable()
    {
        RestoreDefault();
    }

    /// <summary>
    /// AnimationClipChange 초기화
    /// </summary>
    public AnimationClipController(Animator animator, AnimationClip defaultClip, string targetStateName = "Active")
    {
        _animator = animator;
        _defaultClip = defaultClip;
        _targetStateName = targetStateName;

        // AnimatorOverrideController 초기화
        _overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        _animator.runtimeAnimatorController = _overrideController;
    }

    /// <summary>
    /// AnimationClip을 변경하고 재생
    /// </summary>
    public void PlayAnimation(AnimationClip clip, Action onComplete = null, string playStateName = "ATK")
    {
        if (clip == null)
            return;

        if(_animator == null)
        {
            _animator = GetComponent<Animator>();
            _overrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
            _animator.runtimeAnimatorController = _overrideController;
        }

        if(_animator == null)
        {
            _animator = GetComponent<Animator>();
            _overrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
            _animator.runtimeAnimatorController = _overrideController;
        }

        _overrideController[_targetStateName] = clip;
        _animator.Play(playStateName);

        if (onComplete != null)
        {
            StartCoroutine(WaitForAnimationComplete(onComplete));
        }
    }

    public void LoopPlayAnimation(AnimationClip clip, Action onComplete, float duration = 0, string playStateName = "ATK")
    {
        if (clip == null)
            return;

        if(_animator == null)
        {
            _animator = GetComponent<Animator>();
            _overrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
            _animator.runtimeAnimatorController = _overrideController;
        }

        if(_animator == null)
        {
            _animator = GetComponent<Animator>();
            _overrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
            _animator.runtimeAnimatorController = _overrideController;
        }

        _overrideController[_targetStateName] = clip;
        _animator.Play(playStateName);

        if (duration > 0)
            StartCoroutine(CoroutineReturn(duration, onComplete));
    }

    /// <summary>
    /// 애니메이션 완료 대기
    /// </summary>
    private IEnumerator WaitForAnimationComplete(Action onComplete)
    {
        yield return null;
        while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        onComplete?.Invoke();
    }

    /// <summary>
    /// 기본 AnimationClip으로 복원
    /// </summary>
    public void RestoreDefault(string playStateName = "Idle", int layer = 0, float normalizedTime = 0f)
    {
        if (_animator == null)
            return;
        _overrideController[_targetStateName] = _defaultClip;
        // animator.Rebind();
        // animator.Update(0f);
        _animator.Play(playStateName, layer, normalizedTime);
    }

    /// <summary>
    /// 현재 설정된 AnimationClip 반환
    /// </summary>
    public AnimationClip GetCurrentClip()
    {
        return _overrideController[_targetStateName];
    }

    /// <summary>
    /// 기본 AnimationClip 반환
    /// </summary>
    public AnimationClip GetDefaultClip()
    {
        return _defaultClip;
    }

    IEnumerator CoroutineReturn(float duration, Action onComplete)
    {
        yield return YieldCache.GetWaitForSeconds(duration);
        onComplete?.Invoke();

        yield return null;
    }
}
