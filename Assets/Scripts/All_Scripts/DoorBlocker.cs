using UnityEngine;

public class DoorBlocker : MonoBehaviour
{
    private Collider2D doorCollider;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        doorCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OpenDoor()
    {
        if (doorCollider != null)
        {
            doorCollider.enabled = false;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
    }

    public void CloseDoor()
    {
        if (doorCollider != null)
        {
            doorCollider.enabled = true;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }
    }
}