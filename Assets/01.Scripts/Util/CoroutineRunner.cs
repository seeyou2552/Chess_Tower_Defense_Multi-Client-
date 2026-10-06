using System.Collections;
using UnityEngine;

public class CoroutineRunner : Singleton<CoroutineRunner>
{
    /// <summary>
    /// 코루틴을 시작합니다.
    /// </summary>
    public Coroutine Run(IEnumerator enumerator)
    {
        return StartCoroutine(enumerator);
    }

    /// <summary>
    /// 코루틴을 정지합니다.
    /// </summary>
    public void Stop(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }

    /// <summary>
    /// 이름으로 코루틴을 정지합니다.
    /// </summary>
    public void Stop(string methodName)
    {
        StopCoroutine(methodName);
    }

    /// <summary>
    /// 모든 코루틴을 정지합니다.
    /// </summary>
    public void StopAll()
    {
        StopAllCoroutines();
    }
}
