using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public int CurrentEnemies;
    public int MaxEnemies;
    public List<EnemyData> EnemiesData = new List<EnemyData>();
    void Start()
    {
        for (int i = 0; i < MaxEnemies; ++i)
        {
            SpawnNewEnemy();
        }
    }

    void Update()
    {

    }

    void SpawnNewEnemy()
    {
        if (CurrentEnemies < MaxEnemies && EnemiesData.Count > 0)
        {
            // Instantiate the prefab
            EnemyData randomEnemyData = EnemiesData[Random.Range(0, EnemiesData.Count)];
            Vector3 randomPositionInsideArea = GetComponent<EditableArea>().GetRandomPosition(true);
            GameObject obj = Instantiate(randomEnemyData.Prefab, randomPositionInsideArea, Quaternion.identity);
            obj.GetComponent<EnemyScript>().OwnerSpawner = this;

            CurrentEnemies++;
        }
    }

    public void KillEnemy(GameObject enemy)
    {
        if (enemy == null)
            return;
        Destroy(enemy);
        CurrentEnemies--;
        SpawnNewEnemy();
    }
}
