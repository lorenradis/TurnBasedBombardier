using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopKeeper : Interactable
{
    public Item selectedItem;

    public List<Item> itemsForSale = new List<Item>();

    public override bool OnInteract()
    {
        StartShopDialog();
        return base.OnInteract();
    }

    private void StartShopDialog()
    {
        DialogManager.instance.ShowQuestion("Hi, would you like to do some shopping?", () =>
        {
            BuyOrSell();
        }, () => {
        });
    }

    private void BuyOrSell()
    {
        DialogManager.instance.ShowQuestion("Will you be buying or selling?", "Buy", "Sell", () =>
        {
            ShowShop();
        }, () => {
            ShowPlayerInventory();
        });
    }

    private void ShowShop()
    {
        GameManager.instance.uiManager.DisplayShopInventory(itemsForSale, this);
    }

    private void ShowPlayerInventory()
    {
        GameManager.instance.uiManager.DisplaySellableInventory();
    }
}
