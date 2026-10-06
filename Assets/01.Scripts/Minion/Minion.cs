using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Minion : PoolObject, ChessPiece, IPointerDownHandler
{
    [Header("참조 Script")]
    public AttackRange AttackRange;
    public MinionDrag Drag;

    private AnimationClipController _acc;
    private MinionPresenter _presenter;
    public MinionState State => _presenter.State;
    private bool _isFirstWave;

    public uint InstanceId { get; private set; }

    public float CurrentPosX { get; private set; }
    public float CurrentPosY { get; private set; }

    [Header("참조 Component")]
    [SerializeField] private SpriteRenderer _sr;

    [Header("Visual")]
    [SerializeField] private Color _highlightColor;

    // Bool

    public bool IsFirstWave
    {
        get => _presenter?.State.IsFirstWave ?? _isFirstWave;
        set
        {
            _isFirstWave = value;
            if (_presenter != null && _presenter.State != null)
                _presenter.State.IsFirstWave = value;
        }
    }

    public bool IsSiege => _presenter?.State?.IsSiege ?? false;
    public bool IsSelected {get; private set; }

    // Return Action
    public event Action OnReturn;
    public event Action OnTouch;

    public void SetSprite(Sprite sprite)
    {
        if (_sr == null)
            _sr = GetComponent<SpriteRenderer>();

        if (_sr != null)
            _sr.sprite = sprite;
    }

    public void PlayIdleAnimation(AnimationClip clip)
    {
        if (_acc == null)
            _acc = GetComponent<AnimationClipController>();

        if (clip != null && _acc != null)
            _acc.PlayAnimation(clip, null, "Idle");
    }


    void Awake()
    {
        _presenter = new MinionPresenter(this);
    }

    void OnEnable()
    {
        _presenter.OnEnable();
    }

    void OnDisable()
    {
        _presenter.OnDisable();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnTouch?.Invoke();
    }

    public void Init(MinionData minionData) // 미니언 데이터로 초기화
    {
        if (AttackRange == null)
            AttackRange = GetComponentInChildren<AttackRange>();
        if (_sr == null)
            _sr = GetComponent<SpriteRenderer>();
        if (Drag == null)
            Drag = GetComponent<MinionDrag>();
        if (_acc == null)
            _acc = GetComponent<AnimationClipController>();

        _presenter.Init(minionData);
        IsFirstWave = _presenter.State.IsFirstWave;
    }

    public void SelectMinion(bool isSelect)
    {
        if (AttackRange == null)
            AttackRange = GetComponentInChildren<AttackRange>();
        if (_sr == null)
            _sr = GetComponent<SpriteRenderer>();

        IsSelected = isSelect;

        // 선택 시 공격 범위 활성화 및 Highlight 컬러 적용
        if (isSelect)
        {
            _sr.color = _highlightColor;
            AttackRange.SetVisible(true);
        }
        // 선택 해제 시 설정에 따라 공격 범위 비활성화 및 기본 컬러 적용
        else
        {
            _sr.color = Color.white;

            if (!GameManager.Instance.SettingData.IsAtkRangeOn)
                AttackRange.SetVisible(false);
        }
            
    }

    public void SetInstanceId(uint instanceId)
    {
        InstanceId = instanceId; 
    }

    public void SetPosition(float x, float y)
    {
        CurrentPosX = x;
        CurrentPosY = y;

        transform.position = new Vector3 (x, y, 0);
        Drag.SetStartPos(transform.position);
    }

    public void Attack(AttackType atkType, List<ChessPiece> targets, int damage)
    {
        Debug.Log(atkType);
        if (atkType == AttackType.Default)
        {
            DefenseAttack.DefaultAttack(damage, this, targets);
            State.RuntimeStat.AddAC(1);
            State.NotifyStatChanged();
        }

        else if (atkType == AttackType.Skill || atkType == AttackType.Buff)
        {
            Debug.Log("Use Skill");
            DefenseAttack.UseSkill(damage, this, targets);
            State.RuntimeStat.ResetCurrentAC();
            State.NotifyStatChanged();
        }
        
    }

#region Return

    public override void ReturnToPool()
    {
        OnReturn?.Invoke();


        ObjectPoolManager.Instance.Return(gameObject.name, this);
    }

#endregion

}
