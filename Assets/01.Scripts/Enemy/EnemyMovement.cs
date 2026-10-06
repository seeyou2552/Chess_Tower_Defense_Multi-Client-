using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Enemy _enemy;
    private Transform _target;
    private int _routeIndex; // 시작 - 도착 지점까지의 루트 인덱스
    private int _waypointIndex; // 시작 - 도착 지점까지의 루트에서 현재 위치한 웨이포인트 인덱스

    public float BaseMoveSpeed { get; private set; }
    public float CurrentMoveSpeed { get; private set; }

    public event Action OnEndMove; // 이동이 끝났을 때 Action
    public bool IsStun;

    void Awake()
    {
        if (_enemy == null)
            _enemy = GetComponent<Enemy>();
    }

    public void Init(float baseSpeed)
    {
        BaseMoveSpeed = baseSpeed;
        CurrentMoveSpeed = BaseMoveSpeed;
    }

    /// <summary>
    /// 현재 스피드 변경 (Slow 적용)
    /// </summary>
 
    public void SpeedChanged(float slowPercent)
    {
        CurrentMoveSpeed = BaseMoveSpeed * (1f - Mathf.Clamp01(slowPercent / 100f));
    }

    /// <summary>
    /// 현재 스피드를 BaseSpeed로 적용
    /// </summary>
    public void ApplyBaseSpeed()
    {
        CurrentMoveSpeed = BaseMoveSpeed;
    }

    public void SetStun(bool isStun)
    {
        IsStun = isStun;
    }

    void OnEnable()
    {
        _waypointIndex = 0;
        IsStun = false;
    }

    public void MoveUpdate(float deltaTime)
    {
        if (IsStun)
            return;

        if (_target == null)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            _target.position,
            CurrentMoveSpeed * deltaTime
        );

        if ((transform.position - _target.position).sqrMagnitude < 0.0001f)
        {
            // 웨이 포인트 도착 시 Index 값 상승
            _waypointIndex++;
            _target = PathManager.Instance.GetPoint(_routeIndex, _waypointIndex);

            // 더 이상 나아갈 포인트가 없다면 생존-도착 메서드 실행
            if (_target == null)
            {
                ArrivalEndPoint();
            }
        }
    }

    public void Stop()
    {
        _target = null;
    }

    private void ArrivalEndPoint()
    {
        OnEndMove?.Invoke();
    }

    /// <summary>
    /// 시작 포인트 및 이동 루트 설정
    /// </summary>
    public void SetTarget(int routeIndex)
    {
        _routeIndex = routeIndex;
        _waypointIndex = 0;
        _target = PathManager.Instance.GetPoint(routeIndex, 0);
    }

    public void ClearMovement()
    {
        _waypointIndex = 0;
        _target = null;
    }
}
