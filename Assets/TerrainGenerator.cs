using System.Collections.Generic;
using UnityEngine;
using VoronoiLib;
using VoronoiLib.Structures;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float terrainRadius = 50.0f;
    [SerializeField] private int rockDensity = 10;
    [Tooltip("Minimum distance around a rock")]
    [SerializeField] private float rockClearance = 18.0f;
    [Tooltip("How many points the voronoi diagram has")]
    [SerializeField] private int voronoiPoints = 100;
    [Tooltip("Minimum distance between the voronoi points")]
    [SerializeField] private float minDist = 8.0f;
    [Tooltip("Minimum distance around the portal")]
    [SerializeField] private float portalClearance = 100.0f;

    [Header("Appearence")]
    [SerializeField] private float gradientFrequency = 2.0f;
    [SerializeField] private Gradient terrainGradient;

    [Header("Prefabs")]
    [SerializeField] private GameObject[] rocks;

    [HideInInspector] public Vector2 seed;
    private LinkedList<VEdge> diagram;

    private void Start()
    {
        seed = new Vector2(Random.Range(0.0f, 256.0f), Random.Range(0.0f, 256.0f));

        // Generate the points for the voronoi diagram.
        List<FortuneSite> points = GenerateVPoints(voronoiPoints, terrainRadius, minDist);

        // Get the portal position.
        Vector2 portal = Random.onUnitSphere * terrainRadius / 4.0f;

        // Create voronoi diagram.
        diagram = FortunesAlgorithm.Run(points, -terrainRadius, -terrainRadius, terrainRadius + 1, terrainRadius + 1);

        // Create the terrain:
        InstantiateGrid(diagram, portal, portalClearance, terrainRadius, 18.0f, rockDensity);
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            // Draw the voronoi edges.
            LinkedListNode<VEdge> edge = diagram.First;
            for (int i = 0; i < diagram.Count; i++)
            {
                Gizmos.DrawLine((Vector2)edge.Value.Start, (Vector2)edge.Value.End);
                if (edge.Next != null)
                    edge = edge.Next;
                else break;
            }
        }
    }

    /// <summary>
    /// Generate the voronoi points for the map.
    /// </summary>
    /// <param name="quantity">The amount of points to add.</param>
    /// <param name="radius">The radius of the map.</param>
    /// <param name="min_dist">The minimum distance for the points.</param>
    /// <returns>The voronoi points.</returns>
    private List<FortuneSite> GenerateVPoints(int quantity, float radius, float min_dist)
    {
        List<FortuneSite> points = new List<FortuneSite>();

        // Generate all the points randomly.
        for (int i = 0; i < quantity; i++)
        {
            Vector2 point = Random.insideUnitCircle * radius;

            // Check if the new point isn't too close to any existing points.
            bool tooClose = false;
            for (int j = 0; j < points.Count; j++)
            {
                tooClose = Vector2.Distance(point, (Vector2)points[j]) < min_dist;
                if (tooClose) break;
            }
            if (tooClose) continue;

            points.Add(new FortuneSite(point.x, point.y));
        }

        return points;
    }

    /// <summary>
    /// Instantiate the terrain using a grid.
    /// </summary>
    /// <param name="voronoi">The voronoi diagram.</param>
    /// <param name="portal">The position of the portal.</param>
    /// <param name="portal_dist">The minimum distance from the portal.</param>
    /// <param name="radius">The radius of the terrain.</param>
    /// <param name="min_dist">The minimum distance from the voronoi edges.</param>
    /// <param name="density">The density of the rocks.</param>
    private void InstantiateGrid(LinkedList<VEdge> voronoi, Vector2 portal, float portal_dist, float radius, float min_dist, int density)
    {
        for (int y = Mathf.FloorToInt(-radius); y < radius; y += density)
        {
            for (int x = Mathf.FloorToInt(-radius); x < radius; x += density)
            {
                Vector2 point = new Vector2(x, y);

                // Check if the point is within the terrain radius.
                if (Vector2.Distance(point, Vector2.zero) > radius) continue;

                // Check if the point isn't too close to the portal.
                if (Vector2.Distance(point, portal) < portal_dist) continue;

                // Check if the new point isn't too close to any edges.
                bool tooClose = false;
                LinkedListNode<VEdge> edge = voronoi.First;
                while (edge != null)
                {
                    // Create some points along the edge.
                    Vector2 midpoint = new Vector2((float)(edge.Value.Start.X + edge.Value.End.X) / 2.0f, (float)(edge.Value.Start.Y + edge.Value.End.Y) / 2.0f);
                    Vector2 startmidpoint = new Vector2((float)(edge.Value.Start.X + midpoint.x) / 2.0f, (float)(edge.Value.Start.Y + midpoint.y) / 2.0f);
                    Vector2 midendpoint = new Vector2((float)(midpoint.x + edge.Value.End.X) / 2.0f, (float)(midpoint.y + edge.Value.End.Y) / 2.0f);

                    // Check if any of these points are too close.
                    tooClose = Vector2.Distance(point, midpoint) < min_dist || 
                               Vector2.Distance(point, (Vector2)edge.Value.Start) < min_dist || 
                               Vector2.Distance(point, (Vector2)edge.Value.End) < min_dist ||
                               Vector2.Distance(point, startmidpoint) < min_dist ||
                               Vector2.Distance(point, midendpoint) < min_dist;
                    if (tooClose) break;
                    edge = edge.Next;
                }
                if (tooClose) continue;

                // Create the random rock.
                Transform rock = Instantiate(rocks[Random.Range(0, rocks.Length)], transform).transform;
                rock.position = point;
                rock.rotation = Quaternion.Euler(0, 0, Random.Range(0.0f, 360.0f));
                rock.gameObject.name = "Rock";

                rock.GetComponent<SpriteRenderer>().color = TerrainColor(point);
            }
        }
    }

    /// <summary>
    /// Sample the terrain color.
    /// </summary>
    /// <param name="point">The sample point.</param>
    /// <returns>The color of the terrain at the sample point.</returns>
    public Color TerrainColor(Vector2 point)
    {
        return terrainGradient.Evaluate(Mathf.PerlinNoise(point.x / terrainRadius * gradientFrequency + seed.x, point.y / terrainRadius * gradientFrequency + seed.y));
    }

    #region Deprecated
    /// <summary>
    /// Instantiate a cluster of rocks.
    /// </summary>
    /// <param name="pos">The center of the cluster.</param>
    /// <param name="radius">The radius of the cluster.</param>
    /// <param name="min_dist">The minimum distance between rocks.</param>
    /// <param name="instances">The number of rocks.</param>
    private void InstantiateRocks(Vector2 pos, float radius, float min_dist, int instances)
    {
        List<Vector2> points = new List<Vector2>();

        for (int i = 0; i < instances; i++)
        {
            Vector2 point = UnityEngine.Random.insideUnitCircle * radius;

            // Check if the new point isn't too close to any existing points.
            bool tooClose = false;
            for (int j = 0; j < points.Count; j++)
            {
                tooClose = Vector2.Distance(point, points[j]) < min_dist;
                if (tooClose) break;
            }
            if (tooClose) continue;

            points.Add(point);

            // Create the random rock.
            Transform rock = Instantiate(rocks[UnityEngine.Random.Range(0, rocks.Length)], transform).transform;
            rock.position = pos + point;
            rock.rotation = Quaternion.FromToRotation(pos + point, pos);
            rock.gameObject.name = "Rock";
        }
    }

    private void InstantiateTerrain(LinkedList<VEdge> voronoi, float radius, float min_dist, int instances)
    {
        for (int i = 0; i < instances; i++)
        {
            Vector2 point = UnityEngine.Random.insideUnitCircle * radius;

            // Check if the new point isn't too close to any edges.
            bool tooClose = false;
            LinkedListNode<VEdge> edge = voronoi.First;
            while (edge != null)
            {
                Vector2 midpoint = new Vector2((float)(edge.Value.Start.X + edge.Value.End.X) / 2.0f, (float)(edge.Value.Start.Y + edge.Value.End.Y) / 2.0f);
                tooClose = Vector2.Distance(point, midpoint) < min_dist || Vector2.Distance(point, (Vector2)edge.Value.Start) < min_dist || Vector2.Distance(point, (Vector2)edge.Value.End) < min_dist;
                if (tooClose) break;
                edge = edge.Next;
            }
            if (tooClose) continue;

            // Create the random rock.
            Transform rock = Instantiate(rocks[UnityEngine.Random.Range(0, rocks.Length)], transform).transform;
            rock.position = point;
            rock.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0.0f, 360.0f));
            rock.gameObject.name = "Rock";
        }
    }
    #endregion
}
