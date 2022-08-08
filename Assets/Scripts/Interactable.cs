using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public bool preventsMovement = false;

    public virtual bool OnInteract()
    {
        return preventsMovement;
    }

}
