using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootDrop : MonoBehaviour
{
    [SerializeField]
    private int lootChance = 50;
    private int commonChance = 60;
    private int rareChance = 30;
    private int epicChance = 10;

    public List<GameObject> commonDrops = new List<GameObject>();
    public List<GameObject> rareDrops = new List<GameObject>();
    public List<GameObject> epicDrops = new List<GameObject>();

    public void DropLoot()
    {
        int roll = Random.Range(1, 100);


        if (roll < lootChance)
        {
            List<GameObject> avaialbleLoots = new List<GameObject>();

            roll = Random.Range(1, 100);
            if (roll <= epicChance)
            {
                avaialbleLoots = epicDrops;
            }else if (roll < rareChance)
            {
                avaialbleLoots = rareDrops;
            }
            else if (roll < commonChance)
            {
                avaialbleLoots = commonDrops;
            }


            if (avaialbleLoots.Count > 0)
            {
                GameObject lootToSpawn = avaialbleLoots[Random.Range(0, avaialbleLoots.Count)];
                GameObject newLoot = Instantiate(lootToSpawn, new Vector3Int((int)transform.position.x, (int)transform.position.y, 0), Quaternion.identity) as GameObject;
            }
        }
    }
}
