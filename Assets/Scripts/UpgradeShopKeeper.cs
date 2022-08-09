using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeShopKeeper : Interactable
{

    public override bool OnInteract()
    {
        UpgradeManager.instance.StartUpgradeShop();
        return base.OnInteract();
    }

}