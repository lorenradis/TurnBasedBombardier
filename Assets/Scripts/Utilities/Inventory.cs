using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{

    public List<Item> items = new List<Item>();

    public bool isActive = false;

    public ItemSlot[] itemSlots;

    public int maxItems = 3;

    public bool AddItemToList(Item item)
    {
        if (items.Count < maxItems)
        {
            items.Add(item);
            return true;
        }
        if (item.maxStackSize > 1)
        {

            for (int i = 0; i < items.Count; i++)
            {
                if (item.itemName == items[i].itemName)
                {
                    if (items[i].quantity < item.maxStackSize - item.quantity)
                    {
                        items[i].quantity += item.quantity;
                    }else
                    {
                        int dif = item.maxStackSize - items[i].quantity;
                        item.quantity -= dif;
                        items[i].quantity += dif;
                    }
                    if(item.quantity < 1)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void RemoveItemFromList(Item item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
        }
    }

}




/*
some pickups are straight cash.
some pickups are special treasures for which player has limited space, and which must be sold to gain money.
treasures could have odd shapes/sizes and packing the bag could be a microgame

need to create more bad guy types - some are just faster or stronger but there should be other differences too.

status effects for the player - poison, take damage every [x] moves, stun - can't move for [x] turns, slow - moves take speed * 2, blind - lights dim or off

powerups - max health up, speed up, bomb power up, bomb range up, invisibility, boomerang, bigger light, compass (shows exit, enemies)
    shield - negate damage for [x] turns

throwable bombs - when bomb placed, can pick it up and toss it within throwDistance - pickup and toss takes one turn, is cancellable

for the minimap - a two dimensional array of bools isRevealed to determine whether or not an area is displayed - any map tile within player sight range is marked isRevealed = true


*/