using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public Color playerColor;
    public Color enemyColor;
    public Color exitColor;
    public Color floorColor;
    public GameObject dotPrefab;
    public Color wallColor;

    public int startX = 0;
    public int startY = 0;

    public float scaleFactor = .125f;

    private bool hasGenerated;

    public SpriteRenderer[,] miniMapObjects;

    private void Start()
    {
        startX = (int)(Camera.main.orthographicSize * Camera.main.aspect * -1f) + startX;
        startY = (int)(Camera.main.orthographicSize * -1f) + startY;
    }

    public void GenerateMiniMap()
    {
        hasGenerated = true;

        miniMapObjects = new SpriteRenderer[MapBuilder.instance.width, MapBuilder.instance.height];

        for (int x = -1; x <= MapBuilder.instance.width; x++)
        {
            for (int y = -1; y <= MapBuilder.instance.height; y++)
            {
                Vector2 newPosition = new Vector2(Camera.main.transform.position.x + startX + x * scaleFactor, Camera.main.transform.position.y + startY + y * scaleFactor);
                GameObject newDot = Instantiate(dotPrefab, newPosition, Quaternion.identity) as GameObject;
                newDot.transform.SetParent(Camera.main.transform);
                if (x >= 0 && x < MapBuilder.instance.width && y >= 0 && y < MapBuilder.instance.height)
                {
                    miniMapObjects[x, y] = newDot.GetComponent<SpriteRenderer>();
                }
            }
        }
    }

    public void DrawMiniMap()
    {
        if (!hasGenerated)
        {
            GenerateMiniMap();
        }

        for (int x = 0; x < MapBuilder.instance.width; x++)
        {
            for (int y = 0; y < MapBuilder.instance.height; y++)
            {
                if (GameManager.instance.mapTiles[x, y] == 0 && MapBuilder.instance.isRevealed[x, y])
                {
                    miniMapObjects[x, y].color = floorColor;
                }
                else
                {
                    miniMapObjects[x, y].color = wallColor;
                }
            }
        }

        if (GameManager.instance.hasCompass)
        {
            for (int i = 0; i < GameManager.instance.enemies.Count; i++)
            {
                int x = (int)GameManager.instance.enemies[i].transform.position.x;
                int y = (int)GameManager.instance.enemies[i].transform.position.y;
                if (x > 0 && x < MapBuilder.instance.width && y > 0 && y < MapBuilder.instance.height)
                {
                    miniMapObjects[(int)GameManager.instance.enemies[i].transform.position.x, (int)GameManager.instance.enemies[i].transform.position.y].color = enemyColor;
                }
            }

            miniMapObjects[(int)GameManager.instance.exit.transform.position.x, (int)GameManager.instance.exit.transform.position.y].color = exitColor;
        }

        miniMapObjects[GameManager.instance.player.GetComponent<PlayerControls>().gridPosition.x, GameManager.instance.player.GetComponent<PlayerControls>().gridPosition.y].color = playerColor;
    }
}
