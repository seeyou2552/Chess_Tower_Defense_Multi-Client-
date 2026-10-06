using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialData : MonoBehaviour
{
    public List<TutorialStep> InitializeTutorialSteps()
    {
        return new List<TutorialStep>
        {
            new TutorialStep
            {
                Title = "튜토리얼 - 기본 설명",
                Description = "체스 타워 디펜스 게임에 오신 것을 환영합니다!\n이 게임은 체스말을 배치하여 적들을 물리치는 전략 게임입니다.\n\n먼저 체스 말에 대해 설명해드리겠습니다.",
                GuideImagePosition = Vector2.zero,
                HighlightPosition = Vector2.zero,
                HighlightRadius = 0.0f,
                ChessPieceType = MinionType.Pawn,
                ShowChessPiece = false
            },
            new TutorialStep
            {
                Title = "튜토리얼 - 폰 (Pawn)",
                Description = "가장 기본적인 체스 말입니다.",
                GuideImagePosition = new Vector2(350, -100),
                HighlightPosition = Vector2.zero,
                HighlightRadius = 0.0f,
                ChessPieceType = MinionType.Pawn,
                ChessPieceRule = "• 적의 이동속도 비례 추가 데미지\n • 최대 업그레이드 시 필드의 Queen 강화",
                ShowChessPiece = true
            },
            new TutorialStep
            {
                Title = "튜토리얼 - 나이트 (Knight)",
                Description = "Wave 중이 아닐 때 항상 움직일 수 있는 말입니다.",
                GuideImagePosition = new Vector2(350, -100),
                HighlightPosition = Vector2.zero,
                HighlightRadius = 0.0f,
                ChessPieceType = MinionType.Knight,
                ChessPieceRule = "• 전투 중 아닐 때 위치 변경 가능",
                ShowChessPiece = true
            },
            new TutorialStep
            {
                Title = "튜토리얼 - 비숍 (Bishop)",
                Description = "기본 공격이 항상 적을 관통 시키는 특수한 말입니다.",
                GuideImagePosition = new Vector2(350, -100),
                HighlightPosition = Vector2.zero,
                HighlightRadius = 0.0f,
                ChessPieceType = MinionType.Bishop,
                ChessPieceRule = "• 기본 공격이 적을 관통",
                ShowChessPiece = true
            },
            new TutorialStep
            {
                Title = "튜토리얼 - 루크 (Rook)",
                Description = "광범위한 공격이 가능한 말입니다.",
                GuideImagePosition = new Vector2(350, -100),
                HighlightPosition = Vector2.zero,
                HighlightRadius = 0.0f,
                ChessPieceType = MinionType.Rook,
                ChessPieceRule = "• 넓은 범위 공격",
                ShowChessPiece = true
            },
            new TutorialStep
            {
                Title = "튜토리얼 - 퀸 (Queen)",
                Description = "Pawn을 강화할 때 마다 강해지는 특수한 말입니다.",
                GuideImagePosition = new Vector2(350, -100),
                HighlightPosition = Vector2.zero,
                HighlightRadius = 0.0f,
                ChessPieceType = MinionType.Queen,
                ChessPieceRule = "• Pawn 최대 업그레이드 시 공격력 강화",
                ShowChessPiece = true
            },
            new TutorialStep
            {
                Title = "튜토리얼 - 체스 말 배치",
                Description = "이제 게임 화면에 대해 알아봅시다.\n하단의 버튼을 클릭하여\n보유 중인 체스 말을 확인 해 봅시다.",
                GuideImagePosition = Vector2.zero,
                HighlightPosition = new Vector2(0.5f, 0.1f),
                HighlightRadius = 0.2f,
                ChessPieceType = MinionType.Pawn,
                ShowChessPiece = false,
                EventType = TutorialEventType.ShowMinionSelectUI
            },
            new TutorialStep
            {
                Title = "튜토리얼 - 체스 말 배치",
                Description = "생성 할 말을 꾹 누른 다음\n드래그를 통해 말을 배치할 수 있습니다.\n\n배치는오직 흰색 타일에만 배치할 수 있습니다.",
                GuideImagePosition = Vector2.zero,
                HighlightPosition = new Vector2(0.15f, 0.2f),
                HighlightRadius = 0.25f,
                ChessPieceType = MinionType.Pawn,
                ShowChessPiece = false,
                EventType = TutorialEventType.SpawnMinion
            },
            new TutorialStep
            {
                Title = "튜토리얼 - 체스 말 정보",
                Description = "생성된 체스 말을 터치하여\n정보 창을 열 수 있습니다.\n한번 터치해볼까요?",
                GuideImagePosition = Vector2.zero,
                HighlightPosition = Vector2.zero,
                HighlightRadius = 0.0f,
                ChessPieceType = MinionType.Pawn,
                ShowChessPiece = false,
                EventType = TutorialEventType.ShowMinionInfoUI
            },
            new TutorialStep
            {
                Title = "튜토리얼 - 체스 말 정보",
                Description = "정보 창에서는\n공격력, 공격 속도, AC, 스킬 정보를 확인할 수 있습니다.\n\nAC는 Attack Count의 약자로\n일정 횟수가 차게되면 스킬을 발동합니다.",
                GuideImagePosition = new Vector2(-900, -100),
                HighlightPosition = new Vector2(0.85f, 0.7f),
                HighlightRadius = 0.3f,
                ChessPieceType = MinionType.Pawn,
                ShowChessPiece = false
            },
            new TutorialStep
            {
                Title = "튜토리얼 - 체스 말 정보",
                Description = "아래 쪽에는 판매 가격, 업그레이드 정보를 확인할 수 있습니다.\n\n웨이브 시작 전에는 체스 말 구매 가격을 그대로 돌려받을 수 있습니다.",
                GuideImagePosition = new Vector2(-900, -100),
                HighlightPosition = new Vector2(0.85f, 0.3f),
                HighlightRadius = 0.3f,
                ChessPieceType = MinionType.Pawn,
                ShowChessPiece = false
            },
            new TutorialStep
            {
                Title = "튜토리얼 - 체스 말 업그레이드",
                Description = "업그레이드 카드를 터치하여 업그레이드할 수 있습니다.\n\n 업그레이드 정보는 카드를 꾹 눌러 확인할 수 있습니다.",
                GuideImagePosition = new Vector2(-900, -100),
                HighlightPosition = new Vector2(0.85f, 0.3f),
                HighlightRadius = 0.3f,
                ChessPieceType = MinionType.Pawn,
                ShowChessPiece = false
            },
            new TutorialStep
            {
                Title = "튜토리얼 - 적",
                Description = "적은 오로지 검정색 타일로만 이동합니다.\n\n여러 갈래인 길에서는 적이 어디로 이동할지 예측할 수 없습니다. ",
                GuideImagePosition = new Vector2(350, -300),
                HighlightPosition = new Vector2(0.5f, 0.5f),
                HighlightRadius = 0.3f,
                ChessPieceType = MinionType.Pawn,
                ShowChessPiece = false,
            },
            new TutorialStep
            {
                Title = "튜토리얼 - 적",
                Description = "적이 길에 끝에 도달한 경우\n방패에 데미지를 입습니다.\n\n방패의 내구도가 0이하로 떨어질 경우 패배하게됩니다.",
                GuideImagePosition = new Vector2(350, -300),
                HighlightPosition = new Vector2(0.075f, 0.89f),
                HighlightRadius = 0.12f,
                ChessPieceType = MinionType.Pawn,
                ShowChessPiece = false,
            },
            new TutorialStep
            {
                Title = "튜토리얼 - 웨이브 정보",
                Description = "좌측 패널을 터치해 이번 웨이브 정보를 확인할 수 있습니다.\n\n터치해서 확인해볼까요?",
                GuideImagePosition = new Vector2(350, -300),
                HighlightPosition = new Vector2(0.03f, 0.7f),
                HighlightRadius = 0.1f,
                ChessPieceType = MinionType.Pawn,
                ShowChessPiece = false,
                EventType = TutorialEventType.ShowWaveEnemyInfoUI
            },
            new TutorialStep
            {
                Title = "튜토리얼 - 웨이브 정보",
                Description = "웨이브 정보 창에서는 적의 출현 횟수와 처치시 획득 골드의 정보를 확인할 수 있습니다.\n\n좌 우 버튼을 눌러 이번 웨이브의 다른 적의 정보도 확인이 가능합니다.",
                GuideImagePosition = new Vector2(350, -300),
                HighlightPosition = new Vector2(0.5f, 0.5f),
                HighlightRadius = 0.5f,
                ChessPieceType = MinionType.Pawn,
                ShowChessPiece = false
            },
            new TutorialStep
            {
                Title = "튜토리얼 - 웨이브 정보",
                Description = "튜토리얼은 이걸로 끝입니다!\n\n 우측 상단의 X버튼을 눌러서 닫고 위에 있는\n Wave Start 버튼을 눌러\n게임을 즐겨보세요! ",
                GuideImagePosition = new Vector2(350, -300),
                HighlightPosition = new Vector2(0.5f, 0.5f),
                HighlightRadius = 0.5f,
                ChessPieceType = MinionType.Pawn,
                ShowChessPiece = false
            }

            
        };
    }
}

[System.Serializable]
    public class TutorialStep
    {
        public string Title;
        public string Description;
        public Vector2 GuideImagePosition;
        public Vector2 HighlightPosition;
        public float HighlightRadius;
        public MinionType ChessPieceType;
        public string ChessPieceRule;
        public bool ShowChessPiece;
        public TutorialEventType EventType;
    }