using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectUI : MonoBehaviour
{
    [Header("Image")]
    [SerializeField] private Image _nowMapImage;
    [SerializeField] private Image _prevMapImage;
    [SerializeField] private Image _nextMapImage;

    [SerializeField] private Image _stageLevelImage;
    

    [Header("Button")]
    [SerializeField] private Button _leftBtn;
    [SerializeField] private Button _rightBtn;
    [SerializeField] private Button _startBtn;

    private bool swapping = false;
    private int selectStage = 1;


    private void Start()
    {
        _leftBtn.onClick.AddListener(OnLeftBtn);
        _rightBtn.onClick.AddListener(OnRightBtn);
        _startBtn.onClick.AddListener(OnStartBtn);

        _nowMapImage.sprite = StageManager.Instance.Stages[0].MapThumbnail;
        // _nextMapImage.sprite = StageManager.Instance.Stages[1].MapThumbnail;
        // _prevMapImage.sprite = StageManager.Instance.Stages[StageManager.Instance.Stages.Count - 1].MapThumbnail;
    }

    public void Swap(bool isLeft)
    {
        Sequence seq = DOTween.Sequence();

        Vector3 nowPos = _nowMapImage.rectTransform.localPosition;
        Vector3 nextPos = _nextMapImage.rectTransform.localPosition;
        Vector3 prevPos = _prevMapImage.rectTransform.localPosition;

        if (isLeft)
        {
            seq.Join(_prevMapImage.rectTransform.DOLocalMove(nowPos, 0.3f));
            seq.Join(_nowMapImage.rectTransform.DOLocalMove(nextPos, 0.3f));
        }
        else
        {
            seq.Join(_nextMapImage.rectTransform.DOLocalMove(nowPos, 0.3f));
            seq.Join(_nowMapImage.rectTransform.DOLocalMove(prevPos, 0.3f));
        }

        seq.OnComplete(() =>
        {
            if (isLeft)
            {
                _nowMapImage.sprite = _prevMapImage.sprite;
                selectStage--;
            }
            else
            {
                _nowMapImage.sprite = _nextMapImage.sprite;
                selectStage++;
            }            

            int stageCount = StageManager.Instance.Stages.Count;
            int currentIndex = selectStage - 1; // 1-based → 0-based


            _nextMapImage.sprite = StageManager.Instance.Stages[(currentIndex + 1) % stageCount].MapThumbnail;
            _prevMapImage.sprite = StageManager.Instance.Stages[(currentIndex - 1 + stageCount) % stageCount].MapThumbnail;

            if (selectStage < 1)
                selectStage = stageCount;
            else if (selectStage > stageCount)
                selectStage = 1;

            _nowMapImage.rectTransform.localPosition = nowPos;
            _prevMapImage.rectTransform.localPosition = prevPos;
            _nextMapImage.rectTransform.localPosition = nextPos;

            _stageLevelImage.sprite = StageManager.Instance.Stages[selectStage-1].StageLevelIcon;
            swapping = false;
        });

    }
    
    private void OnLeftBtn()
    {
        if(swapping)
            return;

        swapping = true;
        Swap(true);
        SoundManager.Instance.PlaySFX("SwapBtnClick");
    }

    private void OnRightBtn()
    {
        if(swapping)
            return;
            
        swapping = true;
        Swap(false);
        SoundManager.Instance.PlaySFX("SwapBtnClick");
    }

    private void OnStartBtn()
    {
        if (GameManager.Instance.GameState != GameState.Lobby)
            return;
            
        StageManager.Instance.SetStageNumber(selectStage);
        GameManager.Instance.SendJoinStage(selectStage);
    }

}
