using System.Collections.Generic;
using UnityEngine;

public class ChunkPopulator : MonoBehaviour
{
    // Inspector'dan atayacaðýmýz olasý altýn desenlerinin listesi
    public List<GameObject> collectiblePatterns;

    // Bu chunk'ýn içine bir desen yerleþtirme þansý (0=asla, 1=her zaman)
    [Range(0, 1)]
    public float spawnChance = 0.7f;

    // Desenleri yerleþtireceðimiz þerit kýlavuzlarýnýn parent'ý
    public Transform lanesParent;

    // Daha önce yerleþtirilmiþ deseni tutmak için referans
    private GameObject spawnedPattern;

    // Bu fonksiyon, obje SetActive(true) yapýldýðýnda her seferinde çalýþýr.
    // Object Pooling için mükemmeldir!
    void OnEnable()
    {
        // Eðer bu chunk daha önce kullanýldýysa, eski deseni temizle.
        if (spawnedPattern != null)
        {
            Destroy(spawnedPattern);
        }

        // Belirlediðimiz þansa göre bir desen spawn edip etmeyeceðimize karar verelim.
        if (Random.value < spawnChance)
        {
            PopulateCollectibles();
        }
    }

    void PopulateCollectibles()
    {
        // 1. Rastgele bir desen seç
        int patternIndex = Random.Range(0, collectiblePatterns.Count);
        GameObject patternToSpawn = collectiblePatterns[patternIndex];

        // 2. Rastgele bir þerit seç (Sol, Orta, Sað)
        int laneIndex = Random.Range(0, lanesParent.childCount);
        Transform spawnLane = lanesParent.GetChild(laneIndex);

        // 3. Seçilen deseni, seçilen þeridin içine spawn et.
        spawnedPattern = Instantiate(patternToSpawn, spawnLane);

        // Lokal pozisyonunu sýfýrlayarak tam þeridin baþlangýcýna yerleþtir.
        spawnedPattern.transform.localPosition = Vector3.zero;
    }
}