using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHP : Pickup
{

    private void Start()
    {
        amount = 1;
    }

    public override void OnPickup()
    {
        GameManager.instance.GainHP(amount);
        gameObject.SetActive(false);
        base.OnPickup();
    }
}
