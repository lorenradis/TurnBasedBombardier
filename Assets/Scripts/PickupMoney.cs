using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupMoney : Pickup
{

    private void Start()
    {
        amount = Random.Range(10, 30);
    }

    public override void OnPickup()
    {
        GameManager.instance.GainMoney(amount);

        base.OnPickup();
    }
}
