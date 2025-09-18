using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
    public List<GameObject> chunkPrefabs;
    public Transform player;
    public int poolSize = 7;
    public int numberOfStartingChunks = 3;
    public float spawnTriggerDistance = 75f;

    private List<GameObject> chunkPool = new List<GameObject>();
    private Vector3 nextSpawnPoint;

    void Start()
    {
        CreatePool();
        for (int i = 0; i < numberOfStartingChunks; i++)
        {
            if (i == 0)
                SpawnChunk(Vector3.zero);
            else
                SpawnChunk(nextSpawnPoint);
        }
    }

    void Update()
    {
        if (player.position.z > nextSpawnPoint.z - spawnTriggerDistance)
        {
            SpawnChunk(nextSpawnPoint);
            DeactivateOldestChunk();
        }
    }

    void CreatePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject chunk = Instantiate(GetRandomChunkPrefab(), transform.position, Quaternion.identity);
            chunk.SetActive(false);
            chunkPool.Add(chunk);
        }
    }

    GameObject GetChunkFromPool()
    {
        // 1. Kullanýlabilir (aktif olmayan) chunk'larý bulmak için geçici bir liste oluþtur.
        List<GameObject> availableChunks = new List<GameObject>();
        foreach (var chunk in chunkPool)
        {
            if (!chunk.activeInHierarchy)
            {
                availableChunks.Add(chunk);
            }
        }

        // 2. Eðer en az bir tane kullanýlabilir chunk varsa...
        if (availableChunks.Count > 0)
        {
            // 3. O kullanýlabilir olanlarýn içinden rastgele bir tane seç.
            int randomIndex = Random.Range(0, availableChunks.Count);
            return availableChunks[randomIndex];
        }

        // Hiç boþ chunk yoksa (çok nadir bir durum) null döndür.
        return null;
    }

    public void SpawnChunk(Vector3 spawnPosition)
    {
        GameObject newChunk = GetChunkFromPool();
        if (newChunk != null)
        {
            newChunk.transform.position = spawnPosition;
            newChunk.transform.rotation = Quaternion.identity;
            newChunk.SetActive(true);
            nextSpawnPoint = newChunk.transform.Find("CikisNoktasi").position;
        }
    }

    public void DeactivateOldestChunk()
    {
        GameObject oldestChunk = null;
        float oldestZ = float.MaxValue;
        foreach (var chunk in chunkPool)
        {
            if (chunk.activeInHierarchy && chunk.transform.position.z < oldestZ)
            {
                oldestZ = chunk.transform.position.z;
                oldestChunk = chunk;
            }
        }
        if (oldestChunk != null)
        {
            oldestChunk.SetActive(false);
        }
    }

    GameObject GetRandomChunkPrefab()
    {
        int randomIndex = Random.Range(0, chunkPrefabs.Count);
        return chunkPrefabs[randomIndex];
    }
}