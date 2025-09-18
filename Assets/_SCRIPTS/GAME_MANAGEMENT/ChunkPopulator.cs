using System.Collections.Generic;
using UnityEngine;

public class ChunkPopulator : MonoBehaviour
{
    // Inspector'dan atayaca��m�z olas� alt�n desenlerinin listesi
    public List<GameObject> collectiblePatterns;

    // Bu chunk'�n i�ine bir desen yerle�tirme �ans� (0=asla, 1=her zaman)
    [Range(0, 1)]
    public float spawnChance = 0.7f;

    // Desenleri yerle�tirece�imiz �erit k�lavuzlar�n�n parent'�
    public Transform lanesParent;

    // Daha �nce yerle�tirilmi� deseni tutmak i�in referans
    private GameObject spawnedPattern;

    // Bu fonksiyon, obje SetActive(true) yap�ld���nda her seferinde �al���r.
    // Object Pooling i�in m�kemmeldir!
    void OnEnable()
    {
        // E�er bu chunk daha �nce kullan�ld�ysa, eski deseni temizle.
        if (spawnedPattern != null)
        {
            Destroy(spawnedPattern);
        }

        // Belirledi�imiz �ansa g�re bir desen spawn edip etmeyece�imize karar verelim.
        if (Random.value < spawnChance)
        {
            PopulateCollectibles();
        }
    }

    void PopulateCollectibles()
    {
        // 1. Rastgele bir desen se�
        int patternIndex = Random.Range(0, collectiblePatterns.Count);
        GameObject patternToSpawn = collectiblePatterns[patternIndex];

        // 2. Rastgele bir �erit se� (Sol, Orta, Sa�)
        int laneIndex = Random.Range(0, lanesParent.childCount);
        Transform spawnLane = lanesParent.GetChild(laneIndex);

        // 3. Se�ilen deseni, se�ilen �eridin i�ine spawn et.
        spawnedPattern = Instantiate(patternToSpawn, spawnLane);

        // Lokal pozisyonunu s�f�rlayarak tam �eridin ba�lang�c�na yerle�tir.
        spawnedPattern.transform.localPosition = Vector3.zero;
    }
}