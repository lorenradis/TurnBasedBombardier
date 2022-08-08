using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogInteractable : Interactable
{
    public string[] dialogs;

    public override bool OnInteract()
    {
        for (int i = 0; i < dialogs.Length; i++)
        { 
            DialogManager.instance.ShowDialog(dialogs[i]);
        }
        return base.OnInteract();
    }
}
