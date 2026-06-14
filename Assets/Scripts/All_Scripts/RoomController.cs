using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public enum RoomKind
    {
        Start,
        Normal,
        Boss,
        End
    }

    [Header("Room Settings")]
    public RoomKind roomKind;

    [Header("Doors")]
    public DoorBlocker[] doors;

    [Header("Enemy Settings")]
    public GameObject[] enemyPrefabs;
    public Transform[] enemySpawnPoints;
    public int minEnemies = 1;
    public int maxEnemies = 3;

    [Header("Boss Settings")]
    public GameObject bossPrefab;
    public Transform bossSpawnPoint;

    private bool playerEntered = false;
    private bool roomCleared = false;

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    private RoomData roomData;

    void Start()
    {
        OpenDoors();

        if (roomKind == RoomKind.Start || roomKind == RoomKind.End)
        {
            roomCleared = true;
        }
    }

    void Update()
    {
        if (!playerEntered || roomCleared)
        {
            return;
        }

        RemoveDeadEnemies();

        if (spawnedEnemies.Count == 0)
        {
            roomCleared = true;
            OpenDoors();
            if (HR_manager.instance != null)
            {
                HR_manager.instance.CalculateLevelClearInterest();
            }
            else
            {
                Debug.LogWarning("【房間系統】找不到 HR_manager！");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (playerEntered)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerEntered = true;

        if (roomKind == RoomKind.Start || roomKind == RoomKind.End)
        {
            roomCleared = true;
            OpenDoors();
            return;
        }

        CloseDoors();

        if (roomKind == RoomKind.Normal)
        {
            SpawnEnemies();
        }
        else if (roomKind == RoomKind.Boss)
        {
            SpawnBoss();
        }
    }

    void SpawnEnemies()
    {
        if (enemyPrefabs.Length == 0 || enemySpawnPoints.Length == 0)
        {
            roomCleared = true;
            OpenDoors();
            return;
        }

        int enemyCount = Random.Range(minEnemies, maxEnemies + 1);
        enemyCount = Mathf.Min(enemyCount, enemySpawnPoints.Length);

        List<Transform> availablePoints =
            new List<Transform>(enemySpawnPoints);

        for (int i = 0; i < enemyCount; i++)
        {
            int spawnIndex = Random.Range(0, availablePoints.Count);
            Transform spawnPoint = availablePoints[spawnIndex];
            availablePoints.RemoveAt(spawnIndex);

            GameObject enemyPrefab =
                enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            GameObject enemy =
                Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

            spawnedEnemies.Add(enemy);
        }
    }

    void SpawnBoss()
    {
        if (bossPrefab == null || bossSpawnPoint == null)
        {
            roomCleared = true;
            OpenDoors();
            return;
        }

        GameObject boss =
            Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);

        spawnedEnemies.Add(boss);
    }

    void RemoveDeadEnemies()
    {
        spawnedEnemies.RemoveAll(enemy => enemy == null);
    }

    void CloseDoors()
    {
        if (roomData != null)
        {
            roomData.CloseConnectedDoorBlocks();
            return;
        }

        foreach (DoorBlocker door in doors)
        {
            if (door != null)
            {
                door.CloseDoor();
            }
        }
    }

    void OpenDoors()
    {
        if (roomData != null)
        {
            roomData.OpenConnectedDoorBlocks();
            return;
        }

        foreach (DoorBlocker door in doors)
        {
            if (door != null)
            {
                door.OpenDoor();
            }
        }
    }

    void Awake()
    {
        roomData = GetComponent<RoomData>();
    }
}