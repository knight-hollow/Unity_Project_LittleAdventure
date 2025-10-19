using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Controls the enemy spawning system across multiple waves.
/// Manages when enemies appear, gates open, and when the player wins.
/// </summary>
public class Spawner : MonoBehaviour
{
    // --- Singleton Instance ---
    public static Spawner Instance;

    // --- Enemy Prefabs ---
    public GameObject enemy1;     // Melee enemy type
    public GameObject enemy2;     // Ranged enemy type

    // --- Gate References ---
    public Gate gate1;
    public Gate gate2;
    public Gate gate3;

    // --- Spawn Point Lists (for different waves) ---
    public List<Transform> spawnPosList1 = new List<Transform>();
    public List<Transform> spawnPosList2 = new List<Transform>();
    public List<Transform> spawnPosList3 = new List<Transform>();
    public List<Transform> spawnPosList4 = new List<Transform>();
    public List<Transform> spawnPosList5 = new List<Transform>();
    public List<Transform> spawnPosList6 = new List<Transform>();

    // --- Game Progress ---
    public int remainEnemies = 18;     // Total number of enemies currently alive
    public bool isSpawning1 = true;    // Whether wave 2 has been spawned
    public bool isSpawning2 = true;    // Whether wave 3 has been spawned

    private void Awake()
    {
        // Initialize Singleton instance
        Instance = this;

        // Load enemy prefabs from the Resources folder
        enemy1 = Resources.Load<GameObject>("Prefab/Enemy1");
        enemy2 = Resources.Load<GameObject>("Prefab/Enemy2");
    }

    void Start()
    {
        // Spawn the initial wave (Wave 1)
        SpawnEnemy(spawnPosList1, enemy1);
        SpawnEnemy(spawnPosList4, enemy2);
    }

    void Update()
    {
        // --- Wave 2 Trigger ---
        // When the first 4 enemies are defeated (remainEnemies drops to 14)
        if (remainEnemies == 14 && isSpawning1)
        {
            // Open gate 1 to release the next area
            if (gate1 != null)
                gate1.OpenGate();

            // Spawn the next set of enemies
            SpawnEnemy(spawnPosList2, enemy1);
            SpawnEnemy(spawnPosList5, enemy2);

            isSpawning1 = false;
        }

        // --- Wave 3 Trigger ---
        // When remainEnemies drops to 9
        if (remainEnemies == 9 && isSpawning2)
        {
            // Open both gates to release the final area
            if (gate2 != null)
                gate2.OpenGate();
            if (gate3 != null)
                gate3.OpenGate();

            // Spawn the final set of enemies
            SpawnEnemy(spawnPosList3, enemy1);
            SpawnEnemy(spawnPosList6, enemy2);

            isSpawning2 = false;
        }

        // --- Victory Condition ---
        if (remainEnemies == 0 && !GameManage.Instance.isWin)
        {
            Debug.Log("Victory");
            GameManage.Instance.Win();
        }
    }

    /// <summary>
    /// Spawns enemies of a given type at each position in the list.
    /// </summary>
    /// <param name="spawnList">List of spawn point transforms.</param>
    /// <param name="enemytype">Enemy prefab to spawn.</param>
    public void SpawnEnemy(List<Transform> spawnList, GameObject enemytype)
    {
        foreach (var pos in spawnList)
        {
            Instantiate(enemytype, pos);
        }
    }
}
