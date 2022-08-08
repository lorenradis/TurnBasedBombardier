using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<LevelSpecificSpawn> enemySpawns = new List<LevelSpecificSpawn>();

    private void Start()
    {
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        List<GameObject> availableEnemies = new List<GameObject>();

        for (int i = 0; i < enemySpawns.Count; i++)
        {
            if(enemySpawns[i].AppearsOnThisLevel(GameManager.instance.level))
            {
                availableEnemies.Add(enemySpawns[i].objectToSpawn);
            }
        }

        if(availableEnemies.Count >0)
        {
            int index = Random.Range(0, availableEnemies.Count);
            GameObject newEnemy = Instantiate(availableEnemies[index], transform.position, Quaternion.identity) as GameObject;
        }

        gameObject.SetActive(false);
    }
}

[System.Serializable]
public class LevelSpecificSpawn
{
    public int startLevel;
    public int endLevel;
    public GameObject objectToSpawn;

    public bool AppearsOnThisLevel(int level)
    {
        return level >= startLevel && level <= endLevel;
    }
}