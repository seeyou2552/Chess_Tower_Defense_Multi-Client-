using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour
{
    [Header("Button")]
    [SerializeField] private UpgradeBtn[] _btns= new UpgradeBtn[3];
    [SerializeField] private Button _allUpgradeBtn;

    public void SetBtn(MinionState minionState)
    {
        for(int i=0; i<3; i++)
        {
            _btns[i].Init(minionState.Data.Upgrades[i], minionState);
        }
    }
}
