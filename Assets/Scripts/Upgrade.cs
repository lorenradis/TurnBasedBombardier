using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class Upgrade 
{
    public string upgradeName;
    public int upgradeCost;
    public Sprite icon;
    public bool hasPurchased;

    public Action onUpgradeAction;

    public Upgrade()
    {

    }

    public Upgrade(string newName, int newCost, Action upgradeAction)
    {
        upgradeName = newName;
        upgradeCost = newCost;
        onUpgradeAction = upgradeAction;
    }
}
