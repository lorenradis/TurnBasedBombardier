using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupMaxHP : Pickup
{

    public override void OnPickup()
    {
        GameManager.instance.IncreaseMaxHP(amount);
        GameManager.instance.GainHP(amount);

        base.OnPickup();
    }
}
