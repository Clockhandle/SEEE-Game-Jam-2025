// BombSpawner.cs

using UnityEngine;

public class BombSpawner : MonoBehaviour
{
    public GameObject bombPrefab;   
    public float spawnInterval = 1.5f; 
    public float minX, maxX;          
    public float spawnY;                
    
    void Start()
    {
        InvokeRepeating(nameof(SpawnBomb), 0f, spawnInterval);
    }

    void SpawnBomb()
    {
        float x = Random.Range(minX, maxX);
        Vector3 pos = new Vector3(x, spawnY, 0f);
        Instantiate(bombPrefab, pos, Quaternion.identity);
    }
}
