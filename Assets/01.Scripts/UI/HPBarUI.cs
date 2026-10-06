using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBarUI : MonoBehaviour
{
    private Enemy _enemy;
    [SerializeField] private Image _hpBar;

    private RectTransform _rectTransform;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        if (_enemy != null)
        {
            FollowEnemy();
        }
    }

    public void Init(Enemy enemy)
    {
        _enemy = enemy;
    }

    public void FollowEnemy()
    {
        if (_enemy == null)
            return;

        // Enemy 사망 시 숨김 처리
        if (_enemy.IsDead)
            gameObject.SetActive(false);

        // World Screen으로 쫓아가도록 설정
        Vector3 worldPos = _enemy.transform.position + Vector3.up * 0.7f;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        _rectTransform.position = screenPos;
    }

    public void UpdateHPBar(float currentHP, float maxHP)
    {
        if (_enemy == null) return;

        // HPBar 비활성화 시 활성화
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        float ratio = currentHP / maxHP;
        _hpBar.fillAmount = ratio;
    }
    
}
