using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Yield 객체들을 캐싱하여 성능 최적화
/// WaitForSeconds 등의 객체를 매번 생성하지 않고 재사용
/// </summary>
public class YieldCache
{
    private static readonly Dictionary<float, WaitForSeconds> _waitForSecondsDictionary = new();
    private static readonly Dictionary<float, WaitForSecondsRealtime> _waitForSecondsRealtimeDictionary = new();
    private static readonly WaitForFixedUpdate _waitForFixedUpdate = new();
    private static readonly WaitForEndOfFrame _waitForEndOfFrame = new();

    /// <summary>
    /// 캐시된 WaitForSeconds 객체 반환
    /// 없으면 생성 후 캐싱
    /// </summary>
    public static WaitForSeconds GetWaitForSeconds(float seconds)
    {
        if (!_waitForSecondsDictionary.ContainsKey(seconds))
        {
            _waitForSecondsDictionary[seconds] = new WaitForSeconds(seconds);
        }
        return _waitForSecondsDictionary[seconds];
    }

    /// <summary>
    /// WaitForFixedUpdate 객체 반환
    /// </summary>
    public static WaitForFixedUpdate GetWaitForFixedUpdate()
    {
        return _waitForFixedUpdate;
    }

    /// <summary>
    /// WaitForEndOfFrame 객체 반환
    /// </summary>
    public static WaitForEndOfFrame GetWaitForEndOfFrame()
    {
        return _waitForEndOfFrame;
    }

    /// <summary>
    /// 캐시된 WaitForSecondsRealtime 객체 반환
    /// 없으면 생성 후 캐싱
    /// </summary>
    public static WaitForSecondsRealtime GetWaitForSecondsRealtime(float seconds)
    {
        if (!_waitForSecondsRealtimeDictionary.ContainsKey(seconds))
        {
            _waitForSecondsRealtimeDictionary[seconds] = new WaitForSecondsRealtime(seconds);
        }
        return _waitForSecondsRealtimeDictionary[seconds];
    }
}
