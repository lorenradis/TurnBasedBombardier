using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectTrigger : MonoBehaviour
{
    public GameManager.StatusEffect statusEffect;
    public int duration = 2;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            GameManager.instance.ApplyStatus(statusEffect, duration);
        }
    }

}
