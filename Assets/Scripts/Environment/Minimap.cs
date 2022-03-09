using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private RectTransform UI;

    [Header("Config")]
    [SerializeField] private float scaleFactor = 0.175f;

    [Header("Prefabs")]
    [SerializeField] private GameObject arrowUI;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private GameObject[] rocks;

    public static Minimap instance;
    private List<MinimapEnemy> mini_enemies = new List<MinimapEnemy>();
    private Transform mini_player;
    private bool isSetup = false;

    public enum EnemyTypes { Default = 0, Rocket = 1, Guardian = 2 }
    private class MinimapEnemy
    {
        public Transform world_transform;
        public Transform map_transform;
        public EnemyTypes type;
        // Only exists on guardian type enemies.
        public RectTransform guardian_arrow;
    }

    /// <summary>
    /// Convert a vector from world space to minimap space.
    /// </summary>
    private static Vector2 ToMapSpace(Vector2 world_position) => world_position * instance.scaleFactor;

    /// <summary>
    /// Add a rock to the minimap.
    /// </summary>
    /// <param name="world_position">World position of the rock.</param>
    /// <param name="rock_index">Rock sprite index.</param>
    public static void AddRock(Vector2 world_position, Quaternion rotation, int rock_index)
    {
        Vector2 pos = ToMapSpace(world_position);
        Transform rock = Instantiate(instance.rocks[rock_index], instance.transform).transform;
        rock.position = pos;
        rock.rotation = rotation;
    }

    public void Setup()
    {
        // Create the minimap player.
        mini_player = Instantiate(player, transform).transform;
        mini_player.position = ToMapSpace(ShipController.PlayerShip.transform.position);
        mini_player.rotation = ShipController.PlayerShip.transform.rotation;

        isSetup = true;
    }

    /// <summary>
    /// Subscribe an enemy to the minimap.
    /// </summary>
    /// <param name="enemy">The transform of the enemy.</param>
    /// <param name="type">The type of the enemy.</param>
    public static void SubscribeEnemy(Transform enemy, EnemyTypes type)
    {
        if (instance == null) return;

        Transform map_enemy = Instantiate(instance.enemies[(int)type], instance.transform).transform;

        instance.mini_enemies.Add(new MinimapEnemy() 
        { 
            world_transform = enemy,
            map_transform = map_enemy,
            type = type,
            // Only assign the guardian arrow if this enemy is a guardian.
            guardian_arrow = type == EnemyTypes.Guardian ? 
                Instantiate(instance.arrowUI, instance.UI).GetComponent<RectTransform>()
                : null
        });
    }

    /// <summary>
    /// Unsubscribe an enemy from the minimap.
    /// </summary>
    /// <param name="enemy">The enemy transform.</param>
    public static void UnsubscribeEnemy(Transform enemy)
    {
        if (instance == null) return;

        for (int i = 0; i < instance.mini_enemies.Count; i++)
        {
            if (instance.mini_enemies[i].world_transform == enemy)
            {
                Destroy(instance.mini_enemies[i].map_transform.gameObject);
                instance.mini_enemies.RemoveAt(i);
                return;
            }
        }
    }

    private void Awake() => instance = this;

    private void Update()
    {
        if (!isSetup) return;

        // Update the camera position.
        cam.transform.position = ToMapSpace(ShipController.PlayerShip.transform.position);

        // Update the player position and rotation.
        mini_player.position = ToMapSpace(ShipController.PlayerShip.transform.position);
        mini_player.rotation = ShipController.PlayerShip.transform.rotation;

        // Update the enemies.
        for (int i = 0; i < mini_enemies.Count; i++)
        {
            mini_enemies[i].map_transform.position = ToMapSpace(mini_enemies[i].world_transform.position);

            if (mini_enemies[i].type != EnemyTypes.Guardian) return;

            // Rotate the guardian arrows towards the guardians:
            Vector3 dir = ShipController.PlayerShip.transform.position - mini_enemies[i].world_transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            mini_enemies[i].guardian_arrow.rotation = Quaternion.AngleAxis(angle + 90.0f, Vector3.forward);
        }
    }
}
