using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEnemy : EnemyMovement
{
    public GameObject projectilePrefab;



    public override void ChooseAction()
    {
        timeWaited -= GameManager.actThreshhold;


        float dist = Vector2.Distance(transform.position, GameManager.instance.player.position);

        if (dist < sightDistance)
        {
            if((int)transform.position.x == (int)player.position.x || (int)transform.position.y == (int)player.position.y)
            {
                facingVector = (player.position - transform.position).normalized;
                FireProjectile();
            }
            else
            {
                if (dist < attackDistance)
                {
                    AttackPlayer();
                }
                else 
                {
                    MoveTowardsPlayer();
                }

            }
        }
        else
        {
            MoveRandom();
        }


    }

    private void FireProjectile()
    {
        //subtracting time again so that firing projectiles takes longer than moving or melee attacks (prevents firing a projectile every turn)
        timeWaited -= GameManager.actThreshhold;
        GameObject newProjectile = Instantiate(projectilePrefab, (Vector2)transform.position + facingVector.normalized, Quaternion.identity) as GameObject;
        newProjectile.GetComponent<Projectile>().movementVector = facingVector.normalized;
    }
}