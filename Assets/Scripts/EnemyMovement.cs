using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    protected Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb2d;

    protected Vector2 facingVector;

    public float attackDistance = 1.25f;
    public int sightDistance = 7;
    public int attack = 1;
    public int hp = 2;
    [SerializeField]
    private int speed = 10;
    protected int timeWaited = 0;

    protected Transform player;

    private void Start()
    {
        player = GameManager.instance.player;   
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = animator.gameObject.GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        GameManager.instance.AddEnemyToList(this);
    }

    private void OnDisable()
    {
        GameManager.instance.RemoveEnemyFromList(this);
    }

    private void Update()
    {
        spriteRenderer.flipX = facingVector.x < 0;
        
    }

    public void TickSpeed()
    {
        timeWaited += speed;
    }

    public bool IsReady()
    {
        return timeWaited >= GameManager.actThreshhold;
    }

    public virtual void ChooseAction()
    {
        timeWaited -= GameManager.actThreshhold;

        float dist = Vector2.Distance(transform.position, GameManager.instance.player.position);
        if (dist < attackDistance)
        {
            AttackPlayer();
        }
        else if (dist < sightDistance)
        {
            MoveTowardsPlayer();
        }
        else
        {
            MoveRandom();
        }
    }

    protected void MoveRandom()
    {
        int x = Random.Range(-1, 2);
        int y = 0;
        if(x == 0)
        {
            y = Random.Range(-1, 2);
        }
        facingVector = new Vector2(x, y);
        AttemptMove(new Vector2(transform.position.x + x, transform.position.y + y));
    }

    protected void MoveTowardsPlayer()
    {
        Transform player = GameManager.instance.player;
        float xDif = transform.position.x - player.position.x;
        float yDif = transform.position.y - player.position.y;

        List<Vector2> directions = new List<Vector2>();

        if(Mathf.Abs(xDif) > Mathf.Abs(yDif))
        {
            if(xDif < 0)
            {
                directions.Add(Vector2.right);
                if(yDif < 0)
                {
                    directions.Add(Vector2.up);
                    directions.Add(Vector2.left);
                    directions.Add(Vector2.down);
                }
                else
                {
                    directions.Add(Vector2.down);
                    directions.Add(Vector2.left);
                    directions.Add(Vector2.up);
                }
            }
            else
            {
                directions.Add(Vector2.left);
                if(yDif < 0)
                {
                    directions.Add(Vector2.up);
                    directions.Add(Vector2.right);
                    directions.Add(Vector2.down);
                }
                else
                {
                    directions.Add(Vector2.down);
                    directions.Add(Vector2.right);
                    directions.Add(Vector2.up);
                }
            }
        }
        else
        {
            if(yDif < 0)
            {
                directions.Add(Vector2.up);
                if(xDif < 0)
                {
                    directions.Add(Vector2.right);
                    directions.Add(Vector2.down);
                    directions.Add(Vector2.left);
                }
                else
                {
                    directions.Add(Vector2.left);
                    directions.Add(Vector2.down);
                    directions.Add(Vector2.right);
                }
            }
            else
            {
                directions.Add(Vector2.down);
                if (xDif < 0)
                {
                    directions.Add(Vector2.right);
                    directions.Add(Vector2.up);
                    directions.Add(Vector2.left);
                }
                else
                {
                    directions.Add(Vector2.left);
                    directions.Add(Vector2.up);
                    directions.Add(Vector2.right);
                }
            }
        }

        Vector2 movementVector = Vector2.zero;

        for (int i = 0; i < directions.Count; i++)
        {
            if(MapBuilder.instance.TileIsWalkable((int)(transform.position.x + directions[i].x), (int)(transform.position.y + directions[i].y)))
            {
                movementVector = directions[i];
                break;
            }
        }

        if(movementVector != Vector2.zero)
        {
            facingVector = new Vector2(movementVector.x, movementVector.y);
            AttemptMove(new Vector2(transform.position.x + movementVector.x, transform.position.y + movementVector.y));
        }
    }

    protected virtual void AttackPlayer()
    {
        animator.SetTrigger("attack");
        int damage = attack;
        GameManager.instance.TakeDamage(damage);
        GameManager.instance.player.GetComponent<PlayerControls>().Knockback(transform);
    }

    private void AttemptMove(Vector2 newPosition)
    {
        Vector2Int targetPosition = new Vector2Int((int)newPosition.x, (int)newPosition.y);
        Vector2Int playerPosition = new Vector2Int((int)GameManager.instance.player.position.x, (int)GameManager.instance.player.position.y);
        if(GameManager.instance.mapTiles[targetPosition.x, targetPosition.y] == 0 && targetPosition != playerPosition)
        {
            StartCoroutine(SmoothMovement(newPosition));
        }
    }

    private IEnumerator SmoothMovement(Vector2 endPosition)
    {
        Vector2 startPosition = transform.position;
        float elapsedTime = 0f;
        float duration = GameManager.turnTime * .5f;

        animator.SetTrigger("move");

        yield return new WaitForSeconds(4f / 60f);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            t = t * t * (3f - 2f * t);
            Vector2 newPosition = Vector2.Lerp(startPosition, endPosition, t);
            transform.position = newPosition;
            yield return null;
        }

        transform.position = endPosition;

    }

    public void TakeDamage(int amount)
    {
        hp -= amount;
        if(hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if(GetComponent<LootDrop>())
        {
            GetComponent<LootDrop>().DropLoot();
        }
        GameManager.instance.IncrementKills();
        gameObject.SetActive(false);
    }
}
