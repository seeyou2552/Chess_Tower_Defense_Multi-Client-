using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] private Image _loadingPanel;

    [Header("Tip")]
    [SerializeField] private TextMeshProUGUI _tipText;
    [SerializeField] private TipDatabase _tipData;

    private const float DOMoveTime = 0.5f;

    void Start()
    {
        if (_loadingPanel == null)
            _loadingPanel = gameObject.GetComponent<Image>();

        // 초기 transform 저장
        RectTransform rt = _loadingPanel.rectTransform;
        RectTransform canvasRt = rt.root as RectTransform;

        float targetY = canvasRt.rect.height + rt.rect.height;

        Vector3 pos = rt.localPosition;
        pos.y = targetY;
        rt.localPosition = pos;
    }

    public IEnumerator ShowLoadingUI()  
    {
        // Tip 출력
        if (_tipData != null && _tipData.Tips.Count > 0 && _tipText != null)
        {
            _tipText.text = _tipData.Tips[Random.Range(0, _tipData.Tips.Count)];
        }

        // UI 활성화 및 애니메이션
        VisibleUI(true);
        yield return _loadingPanel.rectTransform.DOLocalMove(Vector3.zero, DOMoveTime).WaitForCompletion();
    }

    public IEnumerator HideLoadingUI()
    {   
        // Position 계산
        RectTransform rt = _loadingPanel.rectTransform;
        RectTransform canvasRt = rt.root as RectTransform;

        float targetY = canvasRt.rect.height + rt.rect.height;

        // 애니메이션 실행 후 비활성화
        yield return rt.DOAnchorPosY(targetY, DOMoveTime)
            .SetEase(Ease.InCubic)
            .SetUpdate(true)
            .WaitForCompletion();

        VisibleUI(false);
    }

    public void VisibleUI(bool isVisible)
    {
        _loadingPanel.gameObject.SetActive(isVisible);
    }

}
