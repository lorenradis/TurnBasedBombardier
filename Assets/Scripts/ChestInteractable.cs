using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestInteractable : Interactable
{

    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Sprite closedSprite;
    [SerializeField]
    private Sprite openSprite;

    [SerializeField]
    private Item item;

    private bool isOpen;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override bool OnInteract()
    {
        OpenChest();
        return base.OnInteract();
    }

    private void OpenChest()
    {
        spriteRenderer.sprite = openSprite;
        if (isOpen)
        {
            DialogManager.instance.ShowDialog("It's empty...");
        }
        else
        {
            DialogManager.instance.ShowDialog("You opened the chest...");
            DialogManager.instance.ShowDialog("It contained a " + item.itemName + "!");
            if (GameManager.instance.inventory.AddItemToList(Instantiate(item)))
            {
                DialogManager.instance.ShowDialog("You pocket the treasure.  Nice.");
                isOpen = true;
            }
            else
            {
                DialogManager.instance.ShowDialog("But you don't have enough room for it in your bag...");
            }
        }
        spriteRenderer.sprite = isOpen ? openSprite : closedSprite;
    }
}
