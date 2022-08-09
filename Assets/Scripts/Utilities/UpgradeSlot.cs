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

    public void SetUpgrade(Upgrade upgrade)
    {
        iconImage.sprite = upgrade.icon;
        upgradeNameText.text = upgrade.upgradeName;
        upgradeCostText.text = "" + upgrade.upgradeCost;
    }

    public void BuyThisUpgrade()
    {

    }
}
