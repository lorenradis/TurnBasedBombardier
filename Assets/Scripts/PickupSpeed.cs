using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpeed : Pickup
{
    public override void OnPickup()
    {
        GameManager.instance.GainSpeed(amount);
        base.OnPickup();
    }
}
