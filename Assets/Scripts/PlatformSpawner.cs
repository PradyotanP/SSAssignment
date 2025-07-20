using UnityEngine;
using System.Collections.Generic;

public class PlatformSpawner : MonoBehaviour
{
    public GameObject[] platformPrefabs;
    public GameObject[] obstaclePrefabs;
    public Transform player;
    public float spawnDistance = 30f;
    public float platformLength = 10f;
    public int initialPlatforms = 5;
    public int maxPlatforms = 10;

    private float lastZ = 0f;
    private Queue<GameObject> spawnedPlatforms = new Queue<GameObject>();

    void Start()
    {
        SpawnPlatform(cleanStart: true);

        for (int i = 1; i < initialPlatforms; i++)
        {
            SpawnPlatform();
        }
    }

    void Update()
    {
        if (player.position.z + spawnDistance > lastZ)
        {
            SpawnPlatform();
        }
    }

    void SpawnPlatform(bool cleanStart = false)
    {
        GameObject prefab = platformPrefabs[Random.Range(0, platformPrefabs.Length)];

        Vector3 spawnPos = new Vector3(0, 0, lastZ);
        GameObject platform = Instantiate(prefab, spawnPos, Quaternion.identity);
        spawnedPlatforms.Enqueue(platform);

        if (!cleanStart)
        {
            SpawnObstacles(platform.transform.position);
        }

        lastZ += platformLength;

        if (spawnedPlatforms.Count > maxPlatforms)
        {
            Destroy(spawnedPlatforms.Dequeue());
        }
    }

    void SpawnObstacles(Vector3 platformPos)
    {
        int count = Random.Range(0, 3);
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = platformPos + new Vector3(Random.Range(-2f, 2f), 1f, Random.Range(1f, platformLength - 1f));
            Instantiate(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)], spawnPos, Quaternion.identity);
        }
    }
}
