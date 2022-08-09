using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeSlot : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI upgradeNameText;
    public TextMeshProUGUI upgradeCostText;
    public Upgrade upgrade;

    public void SetUpgrade(Upgrade _upgrade)
    {
        upgrade = _upgrade;
        iconImage.sprite = upgrade.icon;
        upgradeNameText.text = upgrade.upgradeName;
        upgradeCostText.text = "" + upgrade.upgradeCost;
    }

    public void BuyThisUpgrade()
    {
        Debug.Log("You clicked on the " + upgrade.upgradeName + " button");
        UpgradeManager.instance.BuyUpgrade(upgrade);
    }
}
