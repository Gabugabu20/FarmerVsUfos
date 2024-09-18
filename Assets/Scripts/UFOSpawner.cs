using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject ufo;

    [SerializeField]
    private float spawnInterval = 5f;

    [SerializeField]
    private int maxUFOs = 10;

    [SerializeField]
    private Terrain terrain;

    [SerializeField]
    private float spawnHeightAboveTerrain = 10f;

    private int currentUFOCount = 0;

    void Start()
    {
        StartCoroutine(SpawnUFOs());
    }

    private IEnumerator SpawnUFOs()
    {
        while (true)
        {
            if (currentUFOCount < maxUFOs)
            {
                SpawnUFO();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnUFO()
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();
        GameObject newUfo = Instantiate(ufo, spawnPosition, Quaternion.identity);
        currentUFOCount++;
        EnemyHealth enemyHealth = newUfo.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.OnDestroyed += UFODestroyed;
        }
    }

    private void UFODestroyed()
    {
        currentUFOCount--;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainSize = terrainData.size;

        float randomX = Random.Range(0, terrainSize.x);
        float randomZ = Random.Range(0, terrainSize.z);

        float terrainHeight = terrain.SampleHeight(new Vector3(randomX, 0, randomZ));

        Vector3 spawnPosition = new Vector3(randomX + terrain.transform.position.x, terrainHeight + spawnHeightAboveTerrain, randomZ + terrain.transform.position.z);

        return spawnPosition;
    }
}
