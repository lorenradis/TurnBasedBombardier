using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class MapBuilder : MonoBehaviour
{
    public static MapBuilder instance = null;

    public Tilemap floorMap;
    public WeightedRandomTile floorRule;
    public RuleTile wallRule;
    public Tilemap wallsMap;

    public AudioClip boomSound;

    public int width = 24;
    public int height = 14;
    public int fillPercent = 48;
    public int smoothing = 1;

    private int enemyCount;
    private int moneyCount;
    private int healthPickupsCount;
    private int maxHealthPickupsCount;
    private int moneyPickupsCount;

    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject playerPrefab;
    public GameObject healthPickupPrefab;
    public GameObject maxHealthPickupPrefab;
    public GameObject moneyPickupPrefab;
    public GameObject enemySpawnerPrefab;
    public GameObject exitPrefab;

    private List<Vector2> floorTiles = new List<Vector2>();
    private List<Transform> walls = new List<Transform>();
    private List<Transform> floors = new List<Transform>();

    public List<LevelSpecificSpawn> allWalls = new List<LevelSpecificSpawn>();

    public bool[,] isRevealed;

    private void Start()
    {
        if(instance == null)
            instance = this;
        BuildBoard();
    }

    public void BuildBoard()
    {
        //increment map size based on current level here

        width += GameManager.instance.level;
        height += (int)(GameManager.instance.level * .5f);

        isRevealed = new bool[width, height];

        GameManager.instance.mapTiles = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                {
                    GameManager.instance.mapTiles[x, y] = 1;
                }
                else
                {
                    int roll = Random.Range(1, 100);
                    if (roll < fillPercent)
                    {
                        GameManager.instance.mapTiles[x, y] = 1;
                    }
                }
            }
        }

        for (int i = 0; i < smoothing; i++)
        {
            SmoothMap();
        }

        floorTiles.Clear();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (GameManager.instance.mapTiles[x, y] == 0)
                {
                    floorTiles.Add(new Vector2(x, y));
                }
            }
        }

        int minWalkableSpace = 35;

        Debug.Log("There were " + floorTiles.Count + " walkable tiles");

        if(floorTiles.Count < minWalkableSpace)
        {
            Debug.Log("I'm gonna try again");
            BuildBoard();
        }

        GenerateMap();
        PlacePlayer();
        PlaceExit();

        int level = GameManager.instance.level;

        enemyCount = (int)Mathf.Log(level, 2f) * 2;
        healthPickupsCount = enemyCount;
        moneyPickupsCount = enemyCount;
        maxHealthPickupsCount = 0;

        if(level % 10 == 0)
        {
            //do whatever happens on a multiple of 10
        }else if(level % 5 == 0)
        {
            maxHealthPickupsCount = 1;
            moneyPickupsCount *= 2;
            PlacePickups();
        }else if((level + 1) % 5 == 0)
        {
            enemyCount *= 2;
            PlaceEnemies();
        }
        else
        {
            PlaceEnemies();
            PlacePickups();
        }

        RevealTiles(GameManager.instance.player.position, GameManager.instance.sightRange);
        GetComponent<MiniMap>().DrawMiniMap();

        GameManager.instance.playerTurn = true;

    }

    private void SmoothMap()
    {
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                int neighbors = 0;
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i == 0 && j == 0)
                        {
                            //if both i and j are 0 then this is not a neighbor, it's this tile and should not be evaluated
                        }
                        else if (x + i < 0 || x + i >= width || y + j < 0 || y + j >= height)
                        {
                            //if x or y is less than 1 or greater than the width or height of the map then it should not be evaluated
                        }
                        else
                        {
                            neighbors += GameManager.instance.mapTiles[x + i, y + j];
                        }
                    }
                }
                if (neighbors > 4)
                {
                    GameManager.instance.mapTiles[x, y] = 1;
                }
                else if (neighbors < 4)
                {
                    GameManager.instance.mapTiles[x, y] = 0;
                }
            }
        }
    }

    private void GenerateMap()
    {
        for (int x = -10; x < width+10; x++)
        {
            for (int y = -10; y < height +10; y++)
            {
                floorMap.SetTile(new Vector3Int(x, y, 0), floorRule);
                if(x < 0 || x >= width || y < 0 || y >= height)
                {
                    wallsMap.SetTile(new Vector3Int(x, y, 0), wallRule);
                }else if (GameManager.instance.mapTiles[x, y] == 0)
                {
                    

                    //floorMap.SetTile(new Vector3Int(x, y, 0), floorRule);
                    
                    //GameObject newFloor = Instantiate(floorPrefab, new Vector2(x, y), Quaternion.identity) as GameObject;
                    //newFloor.transform.SetParent(transform);
                    //floors.Add(newFloor.transform);
                    
                }
                else
                {
                    //wallsMap.SetTile(new Vector3Int(x, y, 0), wallRule);
                    List<GameObject> availableWalls = new List<GameObject>();

                    GameObject toInstantiate = wallPrefab;

                    for (int i = 0; i < allWalls.Count; i++)
                    {
                        if(allWalls[i].AppearsOnThisLevel(GameManager.instance.level))
                        {
                            availableWalls.Add(allWalls[i].objectToSpawn);
                        }
                    }

                    if(availableWalls.Count > 0)
                    {
                        int index = Random.Range(0, availableWalls.Count);
                        toInstantiate = availableWalls[index];
                    }

                    GameObject newWall = Instantiate(toInstantiate, new Vector2(x, y), Quaternion.identity) as GameObject;
                    newWall.transform.SetParent(transform);
                    walls.Add(newWall.transform);
                    
                }
            }
        }
    }

    private void PlacePlayer()
    {
        int index = Random.Range(0, floorTiles.Count);
        GameObject newPlayer = Instantiate(playerPrefab, floorTiles[index], Quaternion.identity) as GameObject;
        GameManager.instance.player = newPlayer.transform;
        floorTiles.RemoveAt(index);
    }

    private void PlaceEnemies()
    {
        int minDist = 7;
        for (int i = 0; i < enemyCount; i++)
        {
            int index = Random.Range(0, floorTiles.Count);
            float dist = Vector2.Distance(GameManager.instance.player.transform.position, floorTiles[index]);
            int maxTries = floorTiles.Count;
            int tries = 0;
            while (dist < minDist)
            {
                tries++;
                if (tries > maxTries)
                {
                    Debug.Log("Got stuck placing enemies");
                    break;
                }
                index = Random.Range(0, floorTiles.Count);
                dist = Vector2.Distance(GameManager.instance.player.transform.position, floorTiles[index]);
            }
            GameObject newEnemy = Instantiate(enemySpawnerPrefab, floorTiles[index], Quaternion.identity) as GameObject;
            floorTiles.RemoveAt(index);
        }
    }

    private void PlacePickups()
    {
        int minDist = 3;

        float maxTime = 1f;
        float elapsedTime = 0f;

        for (int i = 0; i < healthPickupsCount; i++)
        {
            int index = Random.Range(0, floorTiles.Count);
            float dist = Vector2.Distance(GameManager.instance.player.transform.position, floorTiles[index]);
            while (dist < minDist)
            {
                elapsedTime += Time.deltaTime;
                if(elapsedTime > maxTime)
                {
                    Debug.Log("Got stuck placing pickups");
                    break;
                }
                index = Random.Range(0, floorTiles.Count);
                dist = Vector2.Distance(GameManager.instance.player.transform.position, floorTiles[index]);
            }
            GameObject newPickup = Instantiate(healthPickupPrefab, floorTiles[index], Quaternion.identity) as GameObject;
            floorTiles.RemoveAt(index);
        }
        for (int i = 0; i < maxHealthPickupsCount; i++)
        {
            int index = Random.Range(0, floorTiles.Count);
            float dist = Vector2.Distance(GameManager.instance.player.transform.position, floorTiles[index]);
            int maxTries = floorTiles.Count;
            int tries = 0;
            while (dist < minDist)
            {
                tries++;
                if(tries > maxTries)
                {
                    Debug.Log("got stuck placing pickups");
                    break;
                }
                index = Random.Range(0, floorTiles.Count);
                dist = Vector2.Distance(GameManager.instance.player.transform.position, floorTiles[index]);
            }
            GameObject newPickup = Instantiate(maxHealthPickupPrefab, floorTiles[index], Quaternion.identity) as GameObject;
            floorTiles.RemoveAt(index);
        }
        for (int i = 0; i < moneyPickupsCount; i++)
        {
            int index = Random.Range(0, floorTiles.Count);
            float dist = Vector2.Distance(GameManager.instance.player.transform.position, floorTiles[index]);
            int maxTries = floorTiles.Count;
            int tries = 0;
            while (dist < minDist)
            {
                tries++;
                if (tries > maxTries)
                {
                    Debug.Log("Got stuck placing money");
                    break;
                }
                index = Random.Range(0, floorTiles.Count);
                dist = Vector2.Distance(GameManager.instance.player.transform.position, floorTiles[index]);
            }
            GameObject newPickup = Instantiate(moneyPickupPrefab, floorTiles[index], Quaternion.identity) as GameObject;
            floorTiles.RemoveAt(index);
        }
    }

    private void PlaceExit()
    {
        int minDist = 7;

        float maxTime = 1f;
        float elapsedTime = 0f;

        int index = Random.Range(0, floorTiles.Count);
        float dist = Vector2.Distance(GameManager.instance.player.transform.position, floorTiles[index]);
        while (dist < minDist)
        {
            elapsedTime += Time.deltaTime;
            if(elapsedTime > maxTime)
            {
                Debug.Log("Got stuck placing the exit");
                break;
            }
            index = Random.Range(0, floorTiles.Count);
            dist = Vector2.Distance(GameManager.instance.player.transform.position, floorTiles[index]);
        }
        GameObject newExit = Instantiate(exitPrefab, floorTiles[index], Quaternion.identity) as GameObject;
        GameManager.instance.exit = newExit.transform;
        floorTiles.RemoveAt(index);
    }

    public void DestroyWallAtPosition(Vector2 wallPosition)
    {
        int x = (int)wallPosition.x;
        int y = (int)wallPosition.y;
        if (x < 1 || x >= width-1 || y < 1 || y >= height-1)
            return;

        GameManager.instance.mapTiles[x, y] = 0;

        AudioManager.instance.PlaySound(boomSound);

        //wallsMap.SetTile(new Vector3Int(x, y, 0), null);

        //return;

        for (int i = 0; i < walls.Count; i++)
        {
            if(walls[i].position.x == x && walls[i].position.y == y)
            {
                Transform wallToBreak = walls[i];
                walls.RemoveAt(i);
                Destroy(wallToBreak.gameObject);

                break;
            }
        }
    }

    public bool TileIsWalkable(int x, int y)
    {
        return GameManager.instance.mapTiles[x, y] == 0;
    }

    public void RevealTiles(Vector2 startPos, int range)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float dist = Vector2.Distance(startPos, new Vector2(x, y));
                if(dist <= range)
                {
                    isRevealed[x, y] = true;
                }
            }
        }
    }
}