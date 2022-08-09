using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int moveSpeed;
    public int damage;
    public Vector2 movementVector;
    private int timeWaited = 0;

    void Start()
    {
        GameManager.onPlayerTurnEndCallback += MoveInDirection;
    }

    private void OnDisable()
    {
        GameManager.onPlayerTurnEndCallback -= MoveInDirection;
    }

    private void IncrementSpeed()
    {
        timeWaited += moveSpeed;
    }

    private void MoveInDirection()
    {
        timeWaited -= GameManager.actThreshhold;
        Vector2 moveTarget = (Vector2)transform.position + movementVector;
        StartCoroutine(SmoothMovement(moveTarget));
    }

    private IEnumerator SmoothMovement(Vector2 endPosition)
    {
        Vector2 startPosition = transform.position;
        float elapsedTime = 0f;
        float duration = GameManager.turnTime;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            t = t * t * (3f - 2f * t);
            Vector2 newPosition = Vector2.Lerp(startPosition, endPosition, t);
            transform.position = newPosition;
            yield return null;
        }
        if(GetComponent<Hazard>())
        {
            GetComponent<Hazard>().CheckForPlayerContact();
        }
    }
}
