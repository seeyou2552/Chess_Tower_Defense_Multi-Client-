using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeController
{
    private MinionState _state;
    private Dictionary<int, int> _upgradeLevels = new(); // id - Level
    private Dictionary<int, Upgrade> _upgrades = new();

    public UpgradeController(MinionState state)
    {
        _state = state;

        _upgradeLevels = new();
    }
    public void Init()
    {
        CleanupUpgrade();
        
        foreach(var upgrade in _state.Data.Upgrades)
        {
            _upgradeLevels.Add(upgrade.UpgradeData.ID, 0);
            _upgrades.Add(upgrade.UpgradeData.ID, upgrade);
        }
    }

    public void UpgradeLevelUp(int upgradeId)
    {
        if (!_upgradeLevels.ContainsKey(upgradeId))
            return;

        _upgradeLevels[upgradeId] += 1;

        Upgrade upgrade = _upgrades[upgradeId];

        upgrade.UpgradeData.Upgrade(_state, upgrade);

        MinionTypeAbility.UpgradeAvility(_state); // Pawn 최대 업그레이드인지 확인
        StageUIController.Instance.UpdateMinionInfo(_state);
    }

    public bool AllMaxLevelCheck()
    {
        foreach(var upgrade in _state.Data.Upgrades)
        {
            if (_upgradeLevels[upgrade.UpgradeData.ID] < upgrade.MaxLevel)
                return false;
        }

        return true;
    }

    public bool MaxLevelCheck(Upgrade upgrade)
    {
        return _upgradeLevels[upgrade.UpgradeData.ID] >= upgrade.MaxLevel;
    }

    public int GetUpgradeLevel(int upgradeId)
    {
        if (!_upgradeLevels.ContainsKey(upgradeId))
            return 0;

        return _upgradeLevels[upgradeId];
    }

    public void CleanupUpgrade()
    {
        _upgradeLevels.Clear();
        _upgrades.Clear();
    }
}
