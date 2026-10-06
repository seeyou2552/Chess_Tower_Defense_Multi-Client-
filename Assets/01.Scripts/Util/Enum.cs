using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Account,
    Wave,
    Intermission,
    GameClear,
    GameOver,
    Lobby,
    StageStart,
    GameExit
}

public enum MinionType
{
    Pawn,
    Knight,
    Bishop,
    Rook,
    Queen,
    King
}

public enum AttackDeliveryType
{
    Instant,
    Projectile
}

public enum SearchScope
{
    SingleTarget,
    MultiTarget,
    Area,
    Self,
    AreaMinion
}
public enum SkillType
{
    Impact,
    Projectile,
    AOE,
    Buff,
    Summon
}

public enum ProjectileType
{
    Chase,
    Straight,
    
}

public enum ProjectileHitType
{
    Impact,     // 타겟만
    Pierce,     // 관통
}

public enum ShopItemType
{
    ChessPiece,
    Upgrade,
    Other
}

public enum TutorialEventType
{
    None,
    ShowMinionSelectUI,
    SpawnMinion,
    ShowMinionInfoUI,
    ShowWaveEnemyInfoUI
}

public enum AttackType
{
    Default = 0,
    Skill = 1,
    Buff = 2
}