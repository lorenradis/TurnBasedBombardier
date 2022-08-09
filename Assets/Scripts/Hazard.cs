using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    public bool isDestructible;
    public bool destroyOnTouch;
    public int damage = 1;
    public float distance = .5f;

    public bool causesKnockback;
    public bool preventMovement;

    private void OnEnable()
    {
        Debug.Log("Adding myself to the player turn end method");
        GameManager.onPlayerTurnEndCallback += CheckForPlayerContact;
    }

    public void CheckForPlayerContact()
    {
        float dist = Vector2.Distance(transform.position, GameManager.instance.player.position);

        Debug.Log("The player is " + dist + " away from " + name);

        if (dist <= distance)
        {
            OnPlayerContact();
        }
    }

    public void OnPlayerContact()
    {
        GameManager.instance.TakeDamage(damage);
        GameManager.instance.player.GetComponent<PlayerControls>().Knockback(transform);
        if (destroyOnTouch)
        {
            Destroy(gameObject);
        }
    }

    private void OnDisable()
    {
        GameManager.onPlayerTurnEndCallback -= CheckForPlayerContact;
    }
}
