using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapTransfer : MonoBehaviour
{
    [Header("Tilemap")]
    [SerializeField] private Tilemap _minionTilemap;

    void Start()
    {
        StageManager.Instance.MinionTilemap = _minionTilemap;
    }
}
