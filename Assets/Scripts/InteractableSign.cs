using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableSign : Interactable
{
    public string message;
    public override bool OnInteract()
    {
        Debug.Log(message);

        return base.OnInteract();
    }
}
