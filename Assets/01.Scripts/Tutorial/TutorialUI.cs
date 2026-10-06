using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [Header("Circle Hole Material")]
    [SerializeField] private Image _circleHoleImage;

    [Header("Tutorial Content")]
    [SerializeField] private TextMeshProUGUI _tutorialText;
    [SerializeField] private Image _guideImage;

    [Header("Chess Piece Display")]
    [SerializeField] private Image _chessPieceImage;
    [SerializeField] private TextMeshProUGUI _chessPieceNameText;
    [SerializeField] private TextMeshProUGUI _chessPieceRuleText;
    [SerializeField] private CanvasGroup _chessPiecePanel;

    [Header("Tutorial Settings")]
    [SerializeField] private float _transitionDuration = 0.5f;
    [SerializeField] private bool _showTouchHint = true;

    [Header("Chess Piece Sprite")]
    [SerializeField] private Sprite _pawnSprite;
    [SerializeField] private Sprite _knightSprite;
    [SerializeField] private Sprite _rookSprite;
    [SerializeField] private Sprite _bishopSprite;
    [SerializeField] private Sprite _queenSprite;

    public void Render(TutorialStep step)
    {
        gameObject.SetActive(true);

        _tutorialText.text = step.Description;
        _guideImage.rectTransform.anchoredPosition = step.GuideImagePosition;

        // CircleHole 업데이트
        UpdateCircleHole(step.HighlightPosition, step.HighlightRadius);

        // 체스 말 정보 표시
        if(step.ShowChessPiece)
        {
            ShowChessPieceInfo(step.ChessPieceType, step.ChessPieceRule);
        }
        else
        {
            _chessPiecePanel.alpha = 0f;
        }

    }

    private void UpdateCircleHole(Vector2 position, float radius)
    {
        if(_circleHoleImage != null && _circleHoleImage.material != null)
        {
            _circleHoleImage.material.SetVector("_Center", position);
            _circleHoleImage.material.SetFloat("_Radius", radius);
            
            // Softness는 고정값으로 설정 (부드러운 경계)
            _circleHoleImage.material.SetFloat("_Softness", 0.03f);
        }
    }

    private void ShowChessPieceInfo(MinionType pieceType, string rule)
    {
        if(_chessPiecePanel != null)
            StartCoroutine(FadeInChessPieceCoroutine());
        
        if(_chessPieceNameText != null)
            _chessPieceNameText.text = GetChessPieceName(pieceType);
        
        if(_chessPieceRuleText != null)
            _chessPieceRuleText.text = rule;
    
        if(_chessPieceImage != null)
            _chessPieceImage.sprite = GetChessPieceSprite(pieceType);
        
    }

    private IEnumerator FadeInChessPieceCoroutine()
    {
        float elapsed = 0f;
        while(elapsed < _transitionDuration && _chessPiecePanel != null)
        {
            elapsed += Time.deltaTime;
            _chessPiecePanel.alpha = Mathf.Lerp(0f, 1f, elapsed / _transitionDuration);
            yield return null;
        }
        
        if(_chessPiecePanel != null)
            _chessPiecePanel.alpha = 1f;
    }

    private string GetChessPieceName(MinionType type)
    {
        switch(type)
        {
            case MinionType.Pawn: return "폰 (Pawn)";
            case MinionType.Knight: return "나이트 (Knight)";
            case MinionType.Bishop: return "비숍 (Bishop)";
            case MinionType.Rook: return "룩 (Rook)";
            case MinionType.Queen: return "퀸 (Queen)";
            default: return "";
        }
    }

    private Sprite GetChessPieceSprite(MinionType type)
    {
        switch(type)
        {
            case MinionType.Pawn: return _pawnSprite;
            case MinionType.Knight: return _knightSprite;
            case MinionType.Bishop: return _bishopSprite;
            case MinionType.Rook: return _rookSprite;
            case MinionType.Queen: return _queenSprite;
            default: return null;
        }
    }

    

}

