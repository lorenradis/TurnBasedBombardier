using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemSlot : ItemSlot
{
    private ShopKeeper shopKeeper;

    public override void SetItem(Item newItem, ShopKeeper newShopkeeper)
    {
        item = newItem;
        shopKeeper = newShopkeeper;
        image.sprite = item.itemIcon;
    }

    public void ConfirmItemPurchase()
    {
        GameManager.instance.uiManager.HideShopInventory();

        if (GameManager.instance.money >= item.purchasePrice)
        {

            DialogManager.instance.ShowQuestion("Oh one o' them " + item.itemName + "s, huh?  That'll run ya about " + item.purchasePrice + " gold, sound OK?", () =>
            {
                PurchaseItem();
                DialogManager.instance.ShowDialog("You purchased the " + item.itemName + "!");
                DialogManager.instance.ShowQuestion("Would you like to buy something else?",() => {
                    GameManager.instance.uiManager.DisplayShopInventory(shopKeeper.itemsForSale, shopKeeper);
                }, () => {
                    shopKeeper.AnythingElse();
                });
                
            }, () =>
            {

            });
        }
        else
        {
            DialogManager.instance.ShowQuestion("Sorry, you can't afford that", "Okay", "Gotcha", () =>
            {
                shopKeeper.AnythingElse();
            }, () => {
                shopKeeper.AnythingElse();
            });
        }

    }

    public void PurchaseItem()
    {
        GameManager.instance.inventory.AddItemToList(Instantiate(item));
        GameManager.instance.GainMoney(item.purchasePrice * -1);
    }
}
