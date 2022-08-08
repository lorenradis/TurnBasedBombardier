using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestInteractable : Interactable
{
    [SerializeField]
    private Sprite closedSprite;
    [SerializeField]
    private Sprite openSprite;

    [SerializeField]
    private Item item;

    public override bool OnInteract()
    {
        return base.OnInteract();
    }
}
