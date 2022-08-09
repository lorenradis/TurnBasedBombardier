using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class borderSprite : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private SpriteRenderer referenceRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        spriteRenderer.sortingOrder = (int)referenceRenderer.sortingOrder - 1;
    }
}
