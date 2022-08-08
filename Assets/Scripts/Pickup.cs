using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public int amount;

    private void OnEnable()
    {
        GameManager.instance.AddPickupToList(this);
    }

    private void OnDisable()
    {
        GameManager.instance.RemovePickupFromList(this);
    }

    public virtual void OnPickup()
    {
        gameObject.SetActive(false);
    }
}
