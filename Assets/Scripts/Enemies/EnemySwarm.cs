using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwarm : MonoBehaviour
{
    [Header("References")]
    public GameObject basicEnemy;
    public GameObject rocketEnemy;
    public GameObject eliteEnemy;

    [Header("Spawning")]
    public float spawnRadius;
    public float enemyDistance;
    public int enemyCount;
    [Tooltip("0 = only basic enemies, 1 = only rocket enemies")]
    [Range(0f, 1f)] public float enemyRatio;

    private List<GameObject> spawnPositions = new List<GameObject>();

    Vector2 RandomPos() => Random.insideUnitCircle * Random.Range(0, spawnRadius) + (Vector2)transform.position;
    
    // Start is called before the first frame update
    void Start()
    {
        int itterations = 0;
        // Generating Spawn Points
        for (int i = 0; i < enemyCount; i++)
        {
            Vector2 pos = RandomPos();

            checkAllPositions:
            foreach (GameObject spawnPoint in spawnPositions)
            {
                itterations++;
                // Max itteration check
                if(itterations > enemyCount * 10)
                {
                    Debug.LogWarning("Max itterations hit");
                    goto addObject;
                }

                // Distance check
                if (Vector2.Distance(pos, spawnPoint.transform.position) < enemyDistance)
                {
                    pos = RandomPos();
                    goto checkAllPositions;
                }
            }

            addObject:
            itterations = 0;
            spawnPositions.Add(new GameObject("Spawn Position " + i));
            spawnPositions[i].transform.parent = transform;
            spawnPositions[i].transform.position = pos;
        }

        foreach (GameObject spawnPoint in spawnPositions)
        {
            if(Random.Range(0f, 1f) < enemyRatio)
                Instantiate(rocketEnemy, spawnPoint.transform.position, Quaternion.identity).GetComponent<EnemyBase>().spawnObject = spawnPoint;
            else
                Instantiate( basicEnemy, spawnPoint.transform.position, Quaternion.identity).GetComponent<EnemyBase>().spawnObject = spawnPoint;

        }
    }
}
