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

    private GameObject player;
    private List<GameObject> spawnPositions = new List<GameObject>();
    private List<GameObject> enemies = new List<GameObject>();
    private bool alarmed = false;
    private bool enemiesEnabled = true;

    private LayerMask playerMask;
    private LayerMask obstacleMask;

    Vector2 RandomPos() => Random.insideUnitCircle * Random.Range(0, spawnRadius) + (Vector2)transform.position;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        playerMask = LayerMask.GetMask("Player");
        obstacleMask = LayerMask.GetMask("Obstacle");

        int itterations = 0;
        // Generating Spawn Points
        for (int i = 0; i < enemyCount; i++) {
            Vector2 pos = RandomPos();

            checkAllPositions:
            foreach (GameObject spawnPoint in spawnPositions) {
                itterations++;
                // Max itteration check
                if(itterations > enemyCount * 10) {
                    Debug.LogWarning("Max itterations hit");
                    goto addObject;
                }

                // Distance check
                if (Vector2.Distance(pos, spawnPoint.transform.position) < enemyDistance) {
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

        spawnPositions.Reverse();

        // Spawning the enemise on athe spawnPoints
        foreach (GameObject spawnPoint in spawnPositions) {
            if(Random.Range(0f, 1f) < enemyRatio)
                enemies.Insert(0, Instantiate(rocketEnemy, spawnPoint.transform.position, Quaternion.identity));
            else 
                enemies.Insert(0, Instantiate(basicEnemy, spawnPoint.transform.position, Quaternion.identity));

            enemies[0].GetComponent<EnemyBase>().spawnObject = spawnPoint;
            enemies[0].transform.rotation = new Quaternion(0,0, Random.Range(0, Mathf.PI * 2), 1);
        }
    }

    private void Update()
    {
        if (Vector2.Distance(transform.position, player.transform.position) < 50) {
            // Check if player is in line of sight
            RaycastHit2D hit = Physics2D.Raycast(transform.position, player.transform.position - transform.position, 35, (playerMask | obstacleMask));
            if (hit && hit.collider.CompareTag("Player")) {
                Debug.DrawRay(transform.position, player.transform.position - transform.position, Color.green);
                if (!alarmed)
                    StartCoroutine(alarming());
            } else {
                
                Debug.DrawRay(transform.position, player.transform.position - transform.position, Color.red);
            }

            if (!enemiesEnabled)
                EnableEnemies();
        }
        else if (enemiesEnabled)
            DisableEnemies();
    }

    void EnableEnemies() {
        foreach (GameObject enemy in enemies) enemy.SetActive(true);
        enemiesEnabled = true;
    }

    void DisableEnemies() {
        foreach (GameObject enemy in enemies) enemy.SetActive(false);
        enemiesEnabled = false;
    }

    IEnumerator alarming() {
        alarmed = true;
        foreach (GameObject enemy in enemies) enemy.GetComponent<EnemyBase>().state = EnemyBase.FightState.Relocating;
        yield return new WaitForSeconds(5f);
        alarmed = false;
    }
}
