using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private RectTransform UI;

    [Header("Config")]
    [SerializeField] private float scaleFactor = 0.175f;
    [SerializeField] private float cameraRadius = 50.0f;

    [Header("Prefabs")]
    [SerializeField] private GameObject arrowUI;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject portal;
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

    /// <summary>
    /// Setup the Minimap.
    /// </summary>
    public static void Setup(Vector2 portal_pos)
    {
        // Create the minimap player.
        instance.mini_player = Instantiate(instance.player, instance.transform).transform;
        instance.mini_player.position = ToMapSpace(ShipController.PlayerShip.transform.position);
        instance.mini_player.rotation = ShipController.PlayerShip.transform.rotation;

        // Create the portal.
        Instantiate(instance.portal, ToMapSpace(portal_pos), Quaternion.identity);

        instance.isSetup = true;
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
            MinimapEnemy enemy = mini_enemies[i];

            enemy.map_transform.position = ToMapSpace(enemy.world_transform.position);

            if (enemy.type != EnemyTypes.Guardian) continue;

            // Turn the arrow on and off based on if its on the camera.
            float dist = Vector3.Distance(ShipController.PlayerShip.transform.position, enemy.world_transform.position);

            if (dist >= cameraRadius && enemy.guardian_arrow.gameObject.activeSelf == false)
                enemy.guardian_arrow.gameObject.SetActive(true);
            else if (dist < cameraRadius && enemy.guardian_arrow.gameObject.activeSelf == true)
                enemy.guardian_arrow.gameObject.SetActive(false);

            // Rotate the guardian arrows towards the guardians:
            Vector3 dir = ShipController.PlayerShip.transform.position - enemy.world_transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            enemy.guardian_arrow.rotation = Quaternion.AngleAxis(angle + 90.0f, Vector3.forward);
        }
    }
}
