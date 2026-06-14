using UnityEngine;
using UnityEngine.InputSystem; // 確保你有安裝並啟用 Input System Package

public class ChickenMovement : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 5f; // 在 Inspector 面板記得把這個數字調大（例如 5）

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // 自動抓取角色身上的組件
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 防止角色因為物理碰撞亂轉，並關閉重力（如果你是做俯視角遊戲）
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }
    }

    void Update()
    {
        // 偵測鍵盤 WASD 按鍵
        moveInput = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) moveInput.y = 1;
            if (Keyboard.current.sKey.isPressed) moveInput.y = -1;
            if (Keyboard.current.aKey.isPressed) moveInput.x = -1;
            if (Keyboard.current.dKey.isPressed) moveInput.x = 1;
        }

        // 處理角色左右翻轉
        if (moveInput.x > 0)
        {
            spriteRenderer.flipX = false; // 面向右邊
        }
        else if (moveInput.x < 0)
        {
            spriteRenderer.flipX = true; // 面向左邊
        }
    }

    void FixedUpdate()
    {
        // 套用物理速度讓雞動起來
        if (rb != null)
        {
            // 使用 normalized 確保斜向移動不會變快
            rb.linearVelocity = moveInput.normalized * moveSpeed;
        }
    }
}