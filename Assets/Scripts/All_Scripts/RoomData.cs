using System.Collections.Generic;
using UnityEngine;

public class RoomData : MonoBehaviour
{
    public enum RoomType
    {
        Start,
        Normal,
        Boss,
        End
    }

    public enum DoorDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    [Header("Room Type")]
    public RoomType roomType;

    [Header("Doors")]
    public bool hasUp;
    public bool hasDown;
    public bool hasLeft;
    public bool hasRight;

    [Header("Door Points")]
    public Transform doorUp;
    public Transform doorDown;
    public Transform doorLeft;
    public Transform doorRight;

    [Header("Physical Door Blocks")]
    public DoorBlocker doorBlockUp;
    public DoorBlocker doorBlockDown;
    public DoorBlocker doorBlockLeft;
    public DoorBlocker doorBlockRight;

    private HashSet<DoorDirection> connectedDoors =
        new HashSet<DoorDirection>();

    public bool HasDoor(DoorDirection direction)
    {
        switch (direction)
        {
            case DoorDirection.Up:
                return hasUp;

            case DoorDirection.Down:
                return hasDown;

            case DoorDirection.Left:
                return hasLeft;

            case DoorDirection.Right:
                return hasRight;

            default:
                return false;
        }
    }

    public Transform GetDoor(DoorDirection direction)
    {
        switch (direction)
        {
            case DoorDirection.Up:
                return doorUp;

            case DoorDirection.Down:
                return doorDown;

            case DoorDirection.Left:
                return doorLeft;

            case DoorDirection.Right:
                return doorRight;

            default:
                return null;
        }
    }

    public DoorBlocker GetDoorBlocker(DoorDirection direction)
    {
        switch (direction)
        {
            case DoorDirection.Up:
                return doorBlockUp;

            case DoorDirection.Down:
                return doorBlockDown;

            case DoorDirection.Left:
                return doorBlockLeft;

            case DoorDirection.Right:
                return doorBlockRight;

            default:
                return null;
        }
    }

    public void RegisterConnectedDoor(DoorDirection direction)
    {
        connectedDoors.Add(direction);
    }

    public bool IsDoorConnected(DoorDirection direction)
    {
        return connectedDoors.Contains(direction);
    }

    public void CloseAllDoorBlocks()
    {
        foreach (DoorDirection direction in GetAllDirections())
        {
            DoorBlocker door = GetDoorBlocker(direction);

            if (door != null)
            {
                door.CloseDoor();
            }
        }
    }

    public void OpenConnectedDoorBlocks()
    {
        foreach (DoorDirection direction in GetAllDirections())
        {
            DoorBlocker door = GetDoorBlocker(direction);

            if (door == null)
            {
                continue;
            }

            if (IsDoorConnected(direction))
            {
                door.OpenDoor();
            }
            else
            {
                door.CloseDoor();
            }
        }
    }

    public void CloseConnectedDoorBlocks()
    {
        foreach (DoorDirection direction in GetAllDirections())
        {
            if (!IsDoorConnected(direction))
            {
                continue;
            }

            DoorBlocker door = GetDoorBlocker(direction);

            if (door != null)
            {
                door.CloseDoor();
            }
        }
    }

    private List<DoorDirection> GetAllDirections()
    {
        return new List<DoorDirection>
        {
            DoorDirection.Up,
            DoorDirection.Down,
            DoorDirection.Left,
            DoorDirection.Right
        };
    }
}