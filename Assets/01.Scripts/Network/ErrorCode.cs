using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ErrorCode : int
{
    None = 0,

    // Login
    InvalidId = 1,
    InvalidPassword = 2,

    // Register
    DuplicateId = 3,


    // Minion
	InvalidMinionId = 4,
    InvalidSpawnPosition = 5,
    
    SpawnedMinionPosition = 6,

    InvalidOrderId = 7,

    AlreadyMaxLevel = 9,

    // Gold
    GoldNotEnough = 8,

    // State
    InvalidState = 10,

    InternalServerError,
}
