using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private List<GameObject> spawnPoints;

    [SerializeField] private int spawnCountEach = 5;
    [SerializeField] private float spawnRadiusEach = 10f;

    private List<EnemyStateSystem> enemies = new();

    private void Awake()
    {
        SpawnEnemies();
    }

    private void Update()
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null)
                enemy.UpdateMe();
        }
    }

    /// <summary>
    /// Spawm the enemies.
    /// </summary>
    private void SpawnEnemies()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            for (int i = 0; i < spawnCountEach; i++)
            {
                Vector2 rng = Random.insideUnitCircle * spawnRadiusEach;
                Vector3 dir = new Vector3(rng.x, 0, rng.y);

                Vector3 spawnPosition =
                    spawnPoint.transform.position + dir;

                GameObject go = Instantiate(enemyPrefab, spawnPosition,
                    Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)));

                var stateSystem = go.GetComponent<EnemyStateSystem>();

                enemies.Add(stateSystem);
            }
        }
    }
}
