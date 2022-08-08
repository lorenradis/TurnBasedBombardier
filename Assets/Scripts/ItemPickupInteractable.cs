using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickupInteractable : Interactable
{
    public Item item;

    public override bool OnInteract()
    {
        if(GameManager.instance.inventory.AddItemToList(item))
        {
            Destroy(gameObject);
        }
        return base.OnInteract();
    }
}
