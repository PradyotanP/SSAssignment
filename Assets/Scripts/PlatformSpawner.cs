using UnityEngine;
using System.Collections.Generic;

public class PlatformSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] platformPrefabs;     // Array of possible platform prefabs to spawn
    public GameObject[] obstaclePrefabs;     // Array of possible obstacle prefabs to spawn on platforms

    [Header("Player Reference")]
    public Transform player;                 // Reference to the player to determine when to spawn new platforms

    [Header("Spawn Settings")]
    public float spawnDistance = 30f;        // Distance ahead of the player to spawn new platforms
    public float platformLength = 10f;       // Length of each platform
    public int initialPlatforms = 5;         // Number of platforms to spawn at game start
    public int maxPlatforms = 10;            // Maximum number of platforms to keep active

    private float lastZ = 0f;                // Z-position to place the next platform
    private Queue<GameObject> spawnedPlatforms = new Queue<GameObject>();  // Queue to manage spawned platforms

    void Start()
    {
        // Spawn the first platform without obstacles
        SpawnPlatform(cleanStart: true);

        // Spawn the remaining initial platforms with obstacles
        for (int i = 1; i < initialPlatforms; i++)
        {
            SpawnPlatform();
        }
    }

    void Update()
    {
        // Check if it's time to spawn the next platform
        if (player.position.z + spawnDistance > lastZ)
        {
            SpawnPlatform();
        }
    }

    /// <summary>
    /// Spawns a platform and optionally spawns obstacles on it.
    /// </summary>
    /// <param name="cleanStart">If true, spawn a platform without obstacles (for safe starting zone).</param>
    void SpawnPlatform(bool cleanStart = false)
    {
        // Select a random platform prefab
        GameObject prefab = platformPrefabs[Random.Range(0, platformPrefabs.Length)];

        // Calculate spawn position
        Vector3 spawnPos = new Vector3(0, 0, lastZ);

        // Instantiate and queue the platform
        GameObject platform = Instantiate(prefab, spawnPos, Quaternion.identity);
        spawnedPlatforms.Enqueue(platform);

        // Optionally spawn obstacles (not for the first platform)
        if (!cleanStart)
        {
            SpawnObstacles(platform.transform.position);
        }

        // Move lastZ forward for the next platform
        lastZ += platformLength;

        // Destroy oldest platform if exceeding max allowed
        if (spawnedPlatforms.Count > maxPlatforms)
        {
            Destroy(spawnedPlatforms.Dequeue());
        }
    }

    /// <summary>
    /// Spawns a random number of obstacles on the given platform position.
    /// </summary>
    /// <param name="platformPos">The base position of the platform.</param>
    void SpawnObstacles(Vector3 platformPos)
    {
        int count = Random.Range(0, 3);  // Random number of obstacles (0 to 2)

        for (int i = 0; i < count; i++)
        {
            // Random position on the platform
            Vector3 spawnPos = platformPos + new Vector3(
                Random.Range(-2f, 2f),      // Horizontal range (X)
                1f,                         // Height above the platform (Y)
                Random.Range(1f, platformLength - 1f)  // Depth on the platform (Z)
            );

            // Spawn a random obstacle
            Instantiate(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)], spawnPos, Quaternion.identity);
        }
    }
}
