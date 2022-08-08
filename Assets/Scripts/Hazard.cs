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
        Debug.Log("Adding myself to the player turn end method");
        GameManager.onPlayerTurnEndCallback += CheckForPlayerContact;
    }

    private void CheckForPlayerContact()
    {
        float dist = Vector2.Distance(transform.position, GameManager.instance.player.position);

        Debug.Log("I'm checking how close the player is.  looks like they're " + dist + " away.");

        if (dist <= distance)
        {
            GameManager.instance.TakeDamage(damage);
            GameManager.instance.player.GetComponent<PlayerControls>().Knockback(transform);
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
