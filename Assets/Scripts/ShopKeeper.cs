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

    public void BuyOrSell()
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

    public void AnythingElse()
    {
        DialogManager.instance.ShowQuestion("Anything else I can do for ya?", "Yes", "No", () =>
        {
            BuyOrSell();
        }, () => {
        });
    }

    public void SuccessfulPlayerSale()
    {
        DialogManager.instance.ShowQuestion("Pleasure doing business with you!  Anything else you'd like to buy?", () => {
            ShowShop();
        }, () => {
            AnythingElse();
        });
            

    }

    public void SuccessfulPlayerPurchase()
    {
        DialogManager.instance.ShowQuestion("You drive a hard bargain!  Anything else you'd like to sell?", () => {
            ShowPlayerInventory();
        }, () => {
            AnythingElse();
        });
    }

    private void ShowPlayerInventory()
    {
        DialogManager.instance.ShowQuestion("What would you like to sell?", "Confirm", "Cancel", () => { GameManager.instance.uiManager.ConfirmSell(); }, () => { GameManager.instance.uiManager.CancelSell(); });
        GameManager.instance.uiManager.DisplaySellableInventory(this);
    }
}
