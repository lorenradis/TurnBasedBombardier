using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSorter : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public bool isMobile = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = (int)(transform.position.y * -10);
    }

    private void LateUpdate()
    {
        if(isMobile)
            spriteRenderer.sortingOrder = (int)(transform.position.y * -10);
    }
}
