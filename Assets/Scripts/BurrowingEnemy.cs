using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurrowingEnemy : EnemyMovement {

    public enum EnemyState {BURIED, RELOCATE, EMERGE, COOLDOWN, BURROW}
    public EnemyState enemyState;

    public int moveRange = 3;
/*
    The actions I can take are - choose a position near the player, erupt, idle, burrow, repeat
    The states in which I can be are: below, relocate, emerge, cooldown, burrow
*/

    private int turnsInState = 0;

    private void ChangeState(EnemyState newState)
    {
        if(enemyState != newState)
        {
            enemyState = newState;
            turnsInState = 0;
        }
    }

    public override void ChooseAction()
    {
        timeWaited -= GameManager.actThreshhold;
        turnsInState++;
        switch(enemyState)
        {
            case EnemyState.BURIED:
                animator.SetTrigger("buried");
                ChangeState(EnemyState.RELOCATE);
                break;
            case EnemyState.RELOCATE:
                Relocate();
                ChangeState(EnemyState.EMERGE);
                break;
            case EnemyState.EMERGE:
                if (GameManager.instance.mapTiles[(int)transform.position.x, (int)transform.position.y] == 1)
                {
                    Relocate();
                }
                else
                {
                    animator.SetTrigger("emerge");
                    ChangeState(EnemyState.COOLDOWN);
                }
                    break;
            case EnemyState.COOLDOWN:
                animator.SetTrigger("idle");
                //just, like, sit there, this is the player's chance to hit
                ChangeState(EnemyState.BURROW);
                break;
            case EnemyState.BURROW:
                animator.SetTrigger("burrow");
                //play the burrow animation
                ChangeState(EnemyState.BURIED);
                break;
            default:
                break;

        }
    }

    private void Relocate()
    {
        Vector2 movementVector = GameManager.instance.player.position - transform.position;
        movementVector = Vector2.ClampMagnitude(movementVector, moveRange);

        Vector2 newPosition = (Vector2)transform.position + movementVector;

        if (newPosition.x < 1)
        {
            newPosition.x = 1;
        }
        if (newPosition.x > MapBuilder.instance.width - 1)
        {
            newPosition.x = MapBuilder.instance.width - 1;
        }
        if (newPosition.y < 1)
        {
            newPosition.y = 1;
        }
        if (newPosition.y > MapBuilder.instance.height - 1)
        {
            newPosition.y = MapBuilder.instance.height - 1;
        }

        transform.position = new Vector3((int)newPosition.x, (int)newPosition.y);
    }
}