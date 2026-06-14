using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Rooms")]
    public RoomData startRoom;
    public RoomData bossRoom;
    public List<RoomData> normalRooms;
    public List<RoomData> endRooms;

    [Header("NPC")]
    public GameObject merchantPrefab;

    [Header("Settings")]
    public int normalRoomCount = 6;
    public int maxAttempts = 100;

    private List<RoomData> spawnedRooms = new List<RoomData>();
    private List<OpenDoor> openDoors = new List<OpenDoor>();

    private class OpenDoor
    {
        public RoomData room;
        public RoomData.DoorDirection direction;

        public OpenDoor(RoomData room, RoomData.DoorDirection direction)
        {
            this.room = room;
            this.direction = direction;
        }
    }

    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        spawnedRooms.Clear();
        openDoors.Clear();

        RoomData start = Instantiate(
            startRoom,
            Vector3.zero,
            Quaternion.identity
        );

        spawnedRooms.Add(start);
        MovePlayerToStartRoom(start);
        SpawnMerchantInStartRoom(start);
        AddOpenDoors(start, null);

        GenerateNormalRooms();

        bool bossSpawned = TrySpawnBossRoom();

        if (!bossSpawned)
        {
            Debug.LogWarning("Boss 房生成失敗，找不到合適的開口。");
        }

        CloseRemainingDoors();
        ApplyDoorStatesToAllRooms();
    }
    void ApplyDoorStatesToAllRooms()
    {
        foreach (RoomData room in spawnedRooms)
        {
            room.CloseAllDoorBlocks();
            room.OpenConnectedDoorBlocks();
        }
    }
    void MovePlayerToStartRoom(RoomData start)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("找不到 Tag 為 Player 的玩家物件");
            return;
        }

        PlayerSpawnPoint spawnPoint =
            start.GetComponentInChildren<PlayerSpawnPoint>();

        if (spawnPoint == null)
        {
            Debug.LogWarning("Start 房裡找不到 PlayerSpawnPoint");
            return;
        }

        player.transform.position = spawnPoint.transform.position;
    }

    void SpawnMerchantInStartRoom(RoomData start)
    {
        if (merchantPrefab == null)
        {
            Debug.LogWarning("DungeonGenerator 沒有設定 Merchant Prefab");
            return;
        }

        MerchantSpawnPoint spawnPoint =
            start.GetComponentInChildren<MerchantSpawnPoint>();

        if (spawnPoint == null)
        {
            Debug.LogWarning("Start 房裡找不到 MerchantSpawnPoint");
            return;
        }

        Instantiate(
            merchantPrefab,
            spawnPoint.transform.position,
            Quaternion.identity
        );
    }
    void GenerateNormalRooms()
    {
        for (int i = 0; i < normalRoomCount; i++)
        {
            bool success = TrySpawnNormalRoom();

            if (!success)
            {
                Debug.LogWarning("普通房生成失敗，提前停止。");
                break;
            }
        }
    }

    bool TrySpawnNormalRoom()
    {
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            if (openDoors.Count == 0)
            {
                return false;
            }

            int openDoorIndex = Random.Range(0, openDoors.Count);
            OpenDoor openDoor = openDoors[openDoorIndex];

            RoomData.DoorDirection requiredEntrance =
                GetOppositeDirection(openDoor.direction);

            RoomData roomPrefab =
                GetRandomRoomWithDoor(normalRooms, requiredEntrance);

            if (roomPrefab == null)
            {
                openDoors.RemoveAt(openDoorIndex);
                continue;
            }

            RoomData newRoom = TrySpawnRoomConnected(
                openDoor.room,
                openDoor.direction,
                roomPrefab,
                requiredEntrance
            );

            if (newRoom == null)
            {
                continue;
            }

            openDoor.room.RegisterConnectedDoor(openDoor.direction);
            newRoom.RegisterConnectedDoor(requiredEntrance);
            openDoors.RemoveAt(openDoorIndex);

            spawnedRooms.Add(newRoom);

            AddOpenDoors(newRoom, requiredEntrance);

            return true;
        }

        return false;
    }

    bool TrySpawnBossRoom()
    {
        List<OpenDoor> compatibleDoors = new List<OpenDoor>();

        foreach (OpenDoor openDoor in openDoors)
        {
            RoomData.DoorDirection requiredEntrance =
                GetOppositeDirection(openDoor.direction);

            if (bossRoom.HasDoor(requiredEntrance))
            {
                compatibleDoors.Add(openDoor);
            }
        }

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            if (compatibleDoors.Count == 0)
            {
                break;
            }

            int index = Random.Range(0, compatibleDoors.Count);
            OpenDoor openDoor = compatibleDoors[index];

            RoomData.DoorDirection requiredEntrance =
                GetOppositeDirection(openDoor.direction);

            RoomData boss = TrySpawnRoomConnected(
                openDoor.room,
                openDoor.direction,
                bossRoom,
                requiredEntrance
            );

            if (boss == null)
            {
                compatibleDoors.RemoveAt(index);
                continue;
            }

            openDoor.room.RegisterConnectedDoor(openDoor.direction);
            boss.RegisterConnectedDoor(requiredEntrance);

            openDoors.Remove(openDoor);

            spawnedRooms.Add(boss);

            AddOpenDoors(boss, requiredEntrance);

            return true;
        }

        return TrySpawnConnectorThenBoss();
    }
    bool TrySpawnConnectorThenBoss()
    {
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            if (openDoors.Count == 0)
            {
                return false;
            }

            int openDoorIndex = Random.Range(0, openDoors.Count);
            OpenDoor openDoor = openDoors[openDoorIndex];

            RoomData.DoorDirection connectorEntrance =
                GetOppositeDirection(openDoor.direction);

            RoomData connectorPrefab =
                GetConnectorRoomForBoss(connectorEntrance);

            if (connectorPrefab == null)
            {
                continue;
            }

            RoomData connector = TrySpawnRoomConnected(
                openDoor.room,
                openDoor.direction,
                connectorPrefab,
                connectorEntrance
            );

            if (connector == null)
            {
                continue;
            }

            spawnedRooms.Add(connector);

            RoomData.DoorDirection? connectorExit =
                GetExitDirectionThatCanConnectToBoss(
                    connector,
                    connectorEntrance
                );

            if (!connectorExit.HasValue)
            {
                spawnedRooms.Remove(connector);
                Destroy(connector.gameObject);
                continue;
            }

            RoomData.DoorDirection bossEntrance =
                GetOppositeDirection(connectorExit.Value);

            RoomData boss = TrySpawnRoomConnected(
                connector,
                connectorExit.Value,
                bossRoom,
                bossEntrance
            );

            if (boss == null)
            {
                spawnedRooms.Remove(connector);
                Destroy(connector.gameObject);
                continue;
            }

            openDoor.room.RegisterConnectedDoor(openDoor.direction);
            connector.RegisterConnectedDoor(connectorEntrance);
            connector.RegisterConnectedDoor(connectorExit.Value);
            boss.RegisterConnectedDoor(bossEntrance);

            openDoors.RemoveAt(openDoorIndex);

            spawnedRooms.Add(boss);

            AddOpenDoors(connector, connectorEntrance);
            AddOpenDoors(boss, bossEntrance);

            return true;
        }

        return false;
    }

    void CloseRemainingDoors()
    {
        int safety = 0;

        while (openDoors.Count > 0 && safety < maxAttempts * 5)
        {
            safety++;

            OpenDoor openDoor = openDoors[0];

            RoomData.DoorDirection requiredEntrance =
                GetOppositeDirection(openDoor.direction);

            RoomData endRoomPrefab =
                GetRandomRoomWithDoor(endRooms, requiredEntrance);

            if (endRoomPrefab == null)
            {
                Debug.LogWarning("沒有可以堵住 " + openDoor.direction + " 的 End Room");
                openDoors.RemoveAt(0);
                continue;
            }

            RoomData endRoom = TrySpawnRoomConnected(
                openDoor.room,
                openDoor.direction,
                endRoomPrefab,
                requiredEntrance
            );

            openDoors.RemoveAt(0);

            if (endRoom != null)
            {
                openDoor.room.RegisterConnectedDoor(openDoor.direction);
                endRoom.RegisterConnectedDoor(requiredEntrance);
                spawnedRooms.Add(endRoom);
            }
        }
    }

    RoomData TrySpawnRoomConnected(
        RoomData currentRoom,
        RoomData.DoorDirection currentExit,
        RoomData nextRoomPrefab,
        RoomData.DoorDirection nextEntrance
    )
    {
        RoomData newRoom =
            Instantiate(nextRoomPrefab, Vector3.zero, Quaternion.identity);

        Transform currentDoor =
            currentRoom.GetDoor(currentExit);

        Transform newRoomDoor =
            newRoom.GetDoor(nextEntrance);

        if (currentDoor == null)
        {
            Debug.LogError(currentRoom.name + " 缺少出口：" + currentExit);
            Destroy(newRoom.gameObject);
            return null;
        }

        if (newRoomDoor == null)
        {
            Debug.LogError(newRoom.name + " 缺少入口：" + nextEntrance);
            Destroy(newRoom.gameObject);
            return null;
        }

        Vector3 targetDoorPosition = currentDoor.position;
        Vector3 newDoorLocalPosition = newRoomDoor.localPosition;

        newRoom.transform.position =
            targetDoorPosition - newDoorLocalPosition;

        Physics2D.SyncTransforms();

        if (IsOverlappingExistingRooms(newRoom))
        {
            Debug.LogWarning(
                newRoom.name + " 生成後和其他房間重疊，所以刪除重試"
            );

            Destroy(newRoom.gameObject);
            return null;
        }

        return newRoom;
    }

    void AddOpenDoors(
        RoomData room,
        RoomData.DoorDirection? entranceDirection
    )
    {
        foreach (RoomData.DoorDirection direction in GetAllDirections())
        {
            if (!room.HasDoor(direction))
            {
                continue;
            }

            if (entranceDirection.HasValue &&
                direction == entranceDirection.Value)
            {
                continue;
            }

            openDoors.Add(new OpenDoor(room, direction));
        }
    }

    List<RoomData.DoorDirection> GetAllDirections()
    {
        return new List<RoomData.DoorDirection>
        {
            RoomData.DoorDirection.Up,
            RoomData.DoorDirection.Down,
            RoomData.DoorDirection.Left,
            RoomData.DoorDirection.Right
        };
    }
    RoomData GetConnectorRoomForBoss(
        RoomData.DoorDirection requiredEntrance
    )
    {
        List<RoomData> candidates = new List<RoomData>();

        foreach (RoomData room in normalRooms)
        {
            if (!room.HasDoor(requiredEntrance))
            {
                continue;
            }

            RoomData.DoorDirection? bossExit =
                GetExitDirectionThatCanConnectToBoss(
                    room,
                    requiredEntrance
                );

            if (bossExit.HasValue)
            {
                candidates.Add(room);
            }
        }

        if (candidates.Count == 0)
        {
            return null;
        }

        return candidates[Random.Range(0, candidates.Count)];
    }


    RoomData.DoorDirection? GetExitDirectionThatCanConnectToBoss(
        RoomData room,
        RoomData.DoorDirection entranceDirection
    )
    {
        foreach (RoomData.DoorDirection direction in GetAllDirections())
        {
            if (direction == entranceDirection)
            {
                continue;
            }

            if (!room.HasDoor(direction))
            {
                continue;
            }

            RoomData.DoorDirection bossEntrance =
                GetOppositeDirection(direction);

            if (bossRoom.HasDoor(bossEntrance))
            {
                return direction;
            }
        }

        return null;
    }
    
    
    
    
    
    RoomData GetRandomRoomWithDoor(
        List<RoomData> roomList,
        RoomData.DoorDirection requiredDoor
    )
    {
        List<RoomData> candidates = new List<RoomData>();

        foreach (RoomData room in roomList)
        {
            if (room.HasDoor(requiredDoor))
            {
                candidates.Add(room);
            }
        }

        if (candidates.Count == 0)
        {
            return null;
        }

        return candidates[Random.Range(0, candidates.Count)];
    }

    bool IsOverlappingExistingRooms(RoomData newRoom)
    {
        Bounds newBounds = GetRoomBounds(newRoom);

        foreach (RoomData existingRoom in spawnedRooms)
        {
            Bounds existingBounds = GetRoomBounds(existingRoom);

            if (BoundsOverlap2D(newBounds, existingBounds, 0.05f))
            {
                Debug.LogWarning(
                    newRoom.name + " 和 " + existingRoom.name + " 重疊"
                );

                return true;
            }
        }

        return false;
    }
    bool BoundsOverlap2D(Bounds a, Bounds b, float tolerance)
    {
        float overlapX =
            Mathf.Min(a.max.x, b.max.x) - Mathf.Max(a.min.x, b.min.x);

        float overlapY =
            Mathf.Min(a.max.y, b.max.y) - Mathf.Max(a.min.y, b.min.y);

        return overlapX > tolerance && overlapY > tolerance;
    }

    Bounds GetRoomBounds(RoomData room)
    {
        RoomBounds roomBounds =
            room.GetComponentInChildren<RoomBounds>();

        if (roomBounds == null)
        {
            Debug.LogError(room.name + " 缺少 RoomBounds");
            return new Bounds(room.transform.position, Vector3.one);
        }

        return roomBounds.Bounds;
    }

    RoomData.DoorDirection GetOppositeDirection(
        RoomData.DoorDirection direction
    )
    {
        switch (direction)
        {
            case RoomData.DoorDirection.Up:
                return RoomData.DoorDirection.Down;

            case RoomData.DoorDirection.Down:
                return RoomData.DoorDirection.Up;

            case RoomData.DoorDirection.Left:
                return RoomData.DoorDirection.Right;

            case RoomData.DoorDirection.Right:
                return RoomData.DoorDirection.Left;

            default:
                return RoomData.DoorDirection.Up;
        }
    }    
}