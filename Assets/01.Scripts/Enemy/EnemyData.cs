using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TD/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Stats")]
    public float MaxHP;
    public int AttackDamage;
    public float MoveSpeed;
    public int Reward;

    [Header("Visuals")]
    public int Id;
    public string Name;
    public Sprite Sprite;
    

}
