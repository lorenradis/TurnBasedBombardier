using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class Item : ScriptableObject
{
    public string itemName;
    public string description;
    public int quantity;
    public int maxStackSize;
    public int sellPrice;
    public int purchasePrice;
    public Sprite itemIcon;
}
