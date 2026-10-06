using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuffData : ScriptableObject
{
    public int ID;
    public abstract void ApplyBuff(Minion minion, Buff buff);
}


[System.Serializable]
public class Buff
{
    public BuffData Data;
    public float Increase;
    public float Duration;
    public ChangeDefaultAttack DefaultAttackData; /// 기본 공격의 성능을 변경하는 버프에 추가적으로 적용할 수 있는 데이터
}
