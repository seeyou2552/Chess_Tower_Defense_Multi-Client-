using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TD/TipData")]
public class TipDatabase : ScriptableObject
{
    [TextArea]
    public List<string> Tips;
}