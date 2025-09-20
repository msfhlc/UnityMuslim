using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Bu sat�r� listenin sonuna ekliyoruz, daha sonra kullanaca��z.

public class LevelSpawner : MonoBehaviour
{
    [Header("Chunk Prefabs")]
    public List<GameObject> commonChunkPrefabs;
    public List<GameObject> specialChunkPrefabs;

    [Header("Generation Settings")]
    public int chunksBetweenSpecial = 5;

    public Transform player;
    public int poolSizePerChunkType = 3; // Havuzda her bir chunk T�R�NDEN ka� tane olaca��n� belirler.
    public int numberOfStartingChunks = 3;
    public float spawnTriggerDistance = 75f;

    private List<GameObject> chunkPool = new List<GameObject>();
    private Vector3 nextSpawnPoint;
    private int commonChunksSpawned = 0;

    void Start()
    {
        CreatePool();
        for (int i = 0; i < numberOfStartingChunks; i++)
        {
            SpawnChunk(i == 0 ? Vector3.zero : nextSpawnPoint, false);
        }
    }

    void Update()
    {
        // Player referans� atanmam��sa bir �ey yapma (hata vermesini engeller)
        if (player == null) return;

        if (player.position.z > nextSpawnPoint.z - spawnTriggerDistance)
        {
            SpawnChunk(nextSpawnPoint, true);
            DeactivateOldestChunk();
        }
    }

    // BU FONKS�YONUN ���N� DOLDURUYORUZ
    void CreatePool()
    {
        // Havuzu doldururken, HER B�R s�radan chunk prefab'�ndan belirli say�da olu�tur.
        foreach (var prefab in commonChunkPrefabs)
        {
            for (int i = 0; i < poolSizePerChunkType; i++)
            {
                GameObject chunk = Instantiate(prefab, transform.position, Quaternion.identity);
                chunk.SetActive(false);
                chunkPool.Add(chunk);
            }
        }
        // Ayn� �ekilde, HER B�R �zel chunk'tan da olu�tur.
        foreach (var prefab in specialChunkPrefabs)
        {
            for (int i = 0; i < poolSizePerChunkType; i++)
            {
                GameObject chunk = Instantiate(prefab, transform.position, Quaternion.identity);
                chunk.SetActive(false);
                chunkPool.Add(chunk);
            }
        }
    }

    public void SpawnChunk(Vector3 spawnPosition, bool canBeSpecial)
    {
        GameObject prefabToUse = GetRandomChunkPrefab(canBeSpecial);
        GameObject newChunk = GetChunkFromPool(prefabToUse);

        if (newChunk != null)
        {
            newChunk.transform.position = spawnPosition;
            newChunk.transform.rotation = Quaternion.identity;
            newChunk.SetActive(true);
            nextSpawnPoint = newChunk.transform.Find("CikisNoktasi").position;
        }
        else
        {
            Debug.LogWarning("Havuzda " + prefabToUse.name + " tipinde uygun bir chunk bulunamad�!");
        }
    }

    // BU FONKS�YONUN ���N� DOLDURUYORUZ
    GameObject GetChunkFromPool(GameObject prefab)
    {
        // Havuzun i�inde, istedi�imiz prefab ile ayn� isimde olan VE aktif olmayan ilk chunk'� bul.
        foreach (var chunk in chunkPool)
        {
            if (!chunk.activeInHierarchy && chunk.name == prefab.name + "(Clone)")
            {
                return chunk;
            }
        }

        // E�er havuzda hi� uygun chunk kalmam��sa, hata vermemesi i�in anl�k olarak yeni bir tane olu�tur ve havuza ekle.
        // Bu, sistemi daha sa�lam yapar.
        GameObject newChunk = Instantiate(prefab, transform.position, Quaternion.identity);
        chunkPool.Add(newChunk);
        return newChunk;
    }

    GameObject GetRandomChunkPrefab(bool canBeSpecial)
    {
        if (canBeSpecial && commonChunksSpawned >= chunksBetweenSpecial && specialChunkPrefabs.Count > 0)
        {
            commonChunksSpawned = 0;
            return specialChunkPrefabs[Random.Range(0, specialChunkPrefabs.Count)];
        }
        else
        {
            if (commonChunkPrefabs.Count > 0)
            {
                commonChunksSpawned++;
                return commonChunkPrefabs[Random.Range(0, commonChunkPrefabs.Count)];
            }
            return null; // Hi� s�radan chunk yoksa hata verme.
        }
    }

    public void DeactivateOldestChunk()
    {
        // Linq kullanarak en eski (en d���k Z pozisyonuna sahip) aktif chunk'� buluyoruz.
        GameObject oldestChunk = activeChunks.OrderBy(c => c.transform.position.z).FirstOrDefault();

        if (oldestChunk != null)
        {
            oldestChunk.SetActive(false);
        }
    }

    // YARDIMCI B�R FONKS�YON: Sadece aktif olan chunk'lar� d�nd�r�r.
    public List<GameObject> activeChunks
    {
        get
        {
            List<GameObject> aChunks = new List<GameObject>();
            foreach (var chunk in chunkPool)
            {
                if (chunk.activeInHierarchy)
                {
                    aChunks.Add(chunk);
                }
            }
            return aChunks;
        }
    }
}