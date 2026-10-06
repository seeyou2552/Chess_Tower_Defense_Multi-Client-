using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRange : MonoBehaviour
{
    [Header("참조 Component")]
    private SpriteRenderer _sr;
    private LayerMask _enemyLayer;
    private LayerMask _minionLayer;
    private Renderer[] _rangeRenderers;
    private CircleCollider2D _circle;

    [Header("Color")]
    [SerializeField] private Color _originalColor;
    [SerializeField] private Color _impossibleColor;

    private List<Enemy> _enemies = new List<Enemy>(); // 적을 관리하는 리스트

    void Awake()
    {
        if (_sr == null)
            _sr = GetComponent<SpriteRenderer>();

        if (_circle == null)
            _circle = GetComponent<CircleCollider2D>();

        // 오브젝트의 모든 Renderer 컴포넌트를 수집
        _rangeRenderers = GetComponentsInChildren<Renderer>(includeInactive: true);
    }

    void Start()
    {       
        _enemyLayer = LayerMask.GetMask("Enemy");
        _minionLayer = LayerMask.GetMask("Minion");

        // 시작시 가시성 설정
        SetOriginalColor();
        ApplyVisibility();
        SetImposibleColor();
    }

    private void ApplyVisibility() 
    {
        bool shouldShow = true;

        // 게임 설정에 따라 AttackRange의 가시성을 적용
        if (GameManager.Instance != null && GameManager.Instance.SettingData != null)
            shouldShow = GameManager.Instance.SettingData.IsAtkRangeOn;

        SetVisible(shouldShow);
    }


    public void RangeIncrease(float range)
    {
        transform.localScale += new Vector3(range, range, 0);
    }

    public void SetRange(float newRange)
    {
        transform.localScale = new Vector3(newRange, newRange, 1);
        _enemies.Clear();
        SetOriginalColor();
    }


#region Visual

    /// <summary>
    /// 모든 AttackRange 오브젝트의 가시성을 설정
    /// </summary>
    public static void SetVisibleForAll(bool visible)
    {
        var allRanges = FindObjectsOfType<AttackRange>();
        for (int i = 0; i < allRanges.Length; i++)
        {
            allRanges[i].SetVisible(visible);
        }
    }

    public void SetVisible(bool visible)
    {
        if (_rangeRenderers == null || _rangeRenderers.Length == 0)
            _rangeRenderers = GetComponentsInChildren<Renderer>(includeInactive: true);

        for (int i = 0; i < _rangeRenderers.Length; i++)
        {
            if (_rangeRenderers[i] != null)
                _rangeRenderers[i].enabled = visible;
        }
    }

    public void SetOriginalColor()
    {
        _sr.color = _originalColor;
    }

    public void SetImposibleColor()
    {

        _impossibleColor.a = _originalColor.a;
        _sr.color = _impossibleColor;
        
    }

#endregion
}