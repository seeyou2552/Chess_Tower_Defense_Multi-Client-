using System.Runtime.InteropServices;

public enum PacketType : ushort
{
    None = 0,

    LoginRequest = 1,
    LoginResponse = 2,

    RegisterRequest = 3,
    RegisterResponse = 4,

    JoinRoomRequest = 5,
    JoinRoomResponse = 6,

    StageStartResponse = 7,

    LoadSceneRequest = 8,
    LoadedScene = 9,

    // Minion
    SpawnMinionRequest = 10,
    SpawnMinionResponse = 11,
    SpawnMinionNotify = 12,

    RelocateMinionRequest = 13,
    RelocateMinionResponse = 14,
    RelocateMinionNotify = 15,

    SellMinionRequest = 27,
    SellMinionResponse = 28,

    UpgradeMinionRequest = 29,
    UpgradeMinionResponse = 30,
    UpgradeMinionNotify = 31,

    ReturnMinionNotify = 32,
    UpdateMinionNotify = 33,

    AttackNotify = 35,
    UseBuffSkillNotify = 37,

    // Player
    UseGoldRequest = 16,
    UseGoldResponse = 17,
    UpdateGold = 25,

    // Wave
    StartWaveRequest = 18,
    StartWaveNotify = 19,
    EndWaveNotify = 23,

    // Enemy
    SpawnEnemyNotify = 20,
    ArrivalEnemyNotify = 21,
    DamageToEnemyNotify = 22,
    DeadEnemyNotify = 34,

    //Stage
    EndStageNotify = 36,

    PingRequest,
    PingResponse
}


[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct LoginRequest
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
    public byte[] loginId;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
    public byte[] password;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct LoginResponse
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
    public byte[] uuid;
    public uint playerId;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128, ArraySubType = UnmanagedType.I1)]
    public byte[] accessToken;
    public ErrorCode errCode;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct RegisterRequest
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
    public byte[] loginId;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
    public byte[] password;
}


[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct RegisterResponse
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
    public byte[] uuid;

    public ErrorCode errCode;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct JoinRoomRequest
{
    public int stageId;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct JoinRoomResponse
{
    public ErrorCode errCode;
}


[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct StageStartNotify
{
    public int gold;
    public int orderId;
    public ErrorCode errCode;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct LoadSceneRequest
{

};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct LoadedSceneNotify
{
    public ErrorCode errCode;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct StartWaveRequest
{

};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct StartWaveNotify
{

};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct EndWaveNotify
{

};

#region Minion
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SpawnMinionRequest
{
    public uint minionId;
    public int x;
    public int y;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SpawnMinionResponse
{
    public int currentGold;
    public ErrorCode errCode;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SpawnMinionNotify
{
    public uint minionId;
    public uint instanceId;
    public int orderId;
    public int x;
    public int y;
};

// Relocate
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct RelocateMinionRequest
{
    public uint instanceId;
    public int x;
    public int y;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct RelocateMinionResponse
{
    public uint instanceId;
    public ErrorCode errCode;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct RelocateMinionNotify
{
    public uint instanceId;
    public int x;
    public int y;
};

// Sell
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SellMinionRequest
{
    public uint instanceId;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SellMinionResponse
{
    public int currentGold;
    public ErrorCode errCode;
};

// Upgrade
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct UpgradeMinionRequest
{
    public uint instanceId;
    public int upgradeId;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct UpgradeMinionResponse
{
    public int currentGold;
    public ErrorCode errCode;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct UpgradeMinionNotify
{
    public uint instanceId;
    public int upgradeId;
    public int upgradeLevel;
};

// Return
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ReturnMinionNotify
{
    public uint instanceId;
};

// Attack
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct AttackNotify
{
    public int atkCount;
};

#endregion

#region  Enemy

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SpawnEnemyNotify
{
    public uint enemyId;
    public uint instanceId;
    public int pathIndex;
};

public struct ArrivalEnemyNotify
{
    public uint instanceId;
    public int wallHp;
};



#endregion

// Gold
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct UpdateGold
{
    public int currentGold;
};


// Stage
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct EndStageNotify
{
    public uint success;
};