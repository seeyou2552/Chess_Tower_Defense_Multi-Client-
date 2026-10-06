using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class UpgradeData : ScriptableObject
{
    [Header("ID")]
    public int ID;

    [Header("Info")]
    [TextArea(2, 4)]
    public string UpgradeName;
    public Sprite UpgradeIcon;

    [Header("Descript")]
    [TextArea(2, 4)]
    public string UpgradeDescription;

    public abstract void Upgrade(MinionState minionState, Upgrade upgrade);
    public string GetDescription(float increase)
    {
        return string.Format(
        UpgradeDescription,
        Colorize(increase, "#a5ff30")         // 0
        );
    }

    private string Colorize(float value, string color)
    {
        return $"<color={color}>{value}</color>";
    }

}

[System.Serializable]
public class Upgrade 
{
    public UpgradeData UpgradeData;
    public int MaxLevel;
    public float Increase;
    public int Cost;
}