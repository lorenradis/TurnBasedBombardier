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
    }

    public void ConfirmSellItem()
    {
        GameManager.instance.uiManager.HideShopInventory();

        if (GameManager.instance.money >= item.purchasePrice)
        {

            DialogManager.instance.ShowQuestion("Are you sure you want to buy the " + item.itemName + "?", () =>
            {
                SellItem();
                DialogManager.instance.ShowQuestion("You purchased the " + item.itemName + "!", "Yay!", "Cool", () => {
                    GameManager.instance.uiManager.DisplayShopInventory(shopKeeper.itemsForSale, shopKeeper);
                }, () => {
                    GameManager.instance.uiManager.DisplayShopInventory(shopKeeper.itemsForSale, shopKeeper);

                });
                
            }, () =>
            {

            });
        }
        else
        {
            DialogManager.instance.ShowQuestion("Sorry, you can't afford that", "Okay", "Gotcha", () =>
            {
                GameManager.instance.uiManager.DisplayShopInventory(shopKeeper.itemsForSale, shopKeeper);
            }, () => {
                GameManager.instance.uiManager.DisplayShopInventory(shopKeeper.itemsForSale, shopKeeper);
            });
        }

    }

    public void SellItem()
    {
        GameManager.instance.inventory.AddItemToList(Instantiate(item));
        GameManager.instance.GainMoney(item.purchasePrice * -1);
    }
}
