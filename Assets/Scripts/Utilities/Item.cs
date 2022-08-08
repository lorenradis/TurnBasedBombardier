using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class Item : ScriptableObject
{
    public string itemName;
    public int quantity;
    public int isStackable;
    public int maxStackSize;
    public int sellPrice;
    public int purchasePrice;
    public Sprite itemIcon;
}
