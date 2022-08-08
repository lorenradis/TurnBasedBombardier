using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    public bool isDestructible;
    public bool destroyOnTouch;
    public int damage = 1;
    public float distance = .5f;

    private void OnEnable()
    {
        GameManager.onPlayerTurnEndCallback += CheckForPlayerContact;
    }

    private void CheckForPlayerContact()
    {
        float dist = Vector2.Distance(transform.position, GameManager.instance.player.position);
        if(dist <= distance)
        {
            GameManager.instance.TakeDamage(damage);
            if (destroyOnTouch)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnDisable()
    {
        GameManager.onPlayerTurnEndCallback -= CheckForPlayerContact;
    }
}
