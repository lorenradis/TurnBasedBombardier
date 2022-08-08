using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Item item;
    public Image image;

    public virtual void SetItem(Item newItem)
    {
        item = newItem;
        image.sprite = newItem.itemIcon;
    }

    public virtual void SetItem(Item newItem, ShopKeeper newShopKeeper)
    {

    }
}
