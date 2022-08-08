using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private int damage = 2;
    private float range = 4;
    private int fuseLength = 5;

    private Animator animator;

    public Sprite smallRangeSprite;
    public Sprite medRangeSprite;
    public Sprite largeRangeSprite;

    [SerializeField]
    private SpriteRenderer dangerAreaRenderer;

    private void OnEnable()
    {
        GameManager.instance.AddBombToList(this);
    }

    private void Start()
    {
        if(GameManager.instance != null)
        {
            damage = GameManager.instance.bombDamage;
            range = GameManager.instance.bombRadius;
            fuseLength = GameManager.instance.bombTime;
        }

        animator = GetComponent<Animator>();

        if(range <= 2f)
        {
            dangerAreaRenderer.sprite = smallRangeSprite;
        }else if(range <= 3f)
        {
            dangerAreaRenderer.sprite = medRangeSprite;
        }else if(range <= 5)
        {
            dangerAreaRenderer.sprite = largeRangeSprite;
        }
    }

    private void OnDisable()
    {
        GameManager.instance.RemoveBombFromList(this);
    }

    public void CountDown()
    {
        if (fuseLength <= 0 || animator == null)
            return;
        fuseLength--;

        animator.SetFloat("timeRemaining", fuseLength);
        if(fuseLength <= 0)
        {
            StartCoroutine(Explode());
        }
    }

    private IEnumerator Explode()
    {

        yield return new WaitForSeconds(.1f);

        for (int i = GameManager.instance.enemies.Count-1; i >= 0; i--)
        {
            float dist = Vector2.Distance(transform.position, GameManager.instance.enemies[i].transform.position);
            if(dist < range)
            {
                GameManager.instance.enemies[i].TakeDamage(damage);
            }
        }

        for (int x = 0; x < MapBuilder.instance.width; x++)
        {
            for (int y = 0; y < MapBuilder.instance.height; y++)
            {
                float dist = Vector2.Distance(transform.position, new Vector2(x, y));
                if(dist < range)
                {
                    MapBuilder.instance.DestroyWallAtPosition(new Vector2(x, y));
                }
            }
        }

        yield return new WaitForSeconds(.4f);

        gameObject.SetActive(false);
     }
}
