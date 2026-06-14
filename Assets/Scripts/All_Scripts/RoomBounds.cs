using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RoomBounds : MonoBehaviour
{
    private BoxCollider2D boxCollider;

    public Bounds Bounds
    {
        get
        {
            if (boxCollider == null)
            {
                boxCollider = GetComponent<BoxCollider2D>();
            }

            return boxCollider.bounds;
        }
    }

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Reset()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.isTrigger = true;
    }

    void OnDrawGizmos()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();

        if (collider == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
    }
}