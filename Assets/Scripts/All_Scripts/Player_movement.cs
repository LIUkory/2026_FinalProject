using UnityEngine;
using UnityEngine.InputSystem;

public class Player_movement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private HR_Recruiter myRecruiter;
    void Start()
    {
        // 獲取身上的元件
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        myRecruiter = GetComponent<HR_Recruiter>();
        // 確保重力為 0，避免俯視角遊戲角色一直往下掉
        rb.gravityScale = 0f;
    }

    void Update()
    {
        Vector2 moveInput = Vector2.zero;

        // 讀取鍵盤輸入 (使用新的 Input System)
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) moveInput.y = 1;
            if (Keyboard.current.sKey.isPressed) moveInput.y = -1;
            if (Keyboard.current.aKey.isPressed) moveInput.x = -1;
            if (Keyboard.current.dKey.isPressed) moveInput.x = 1;
        }
        if (Input.GetMouseButtonDown(1))
        {
            myRecruiter.TryRecruitInArea();
        }
        // 單位化向量，避免斜向移動變快 (1.414倍)
        Vector2 movement = moveInput.normalized;

        // 1. 物理移動 (Unity 6 的新寫法 linearVelocity)
        rb.linearVelocity = movement * moveSpeed;

        // 2. 動畫控制：判斷是否有按鍵輸入
        bool isMoving = (movement.x != 0 || movement.y != 0);
        if (anim != null)
        {
            anim.SetBool("isWalking", isMoving);
        }

        // 3. 圖片翻轉控制：根據往左走還是往右走，水平翻轉狐狸圖片
        if (spriteRenderer != null)
        {
            if (movement.x > 0)
            {
                spriteRenderer.flipX = false; // 往右走，不翻轉
            }
            else if (movement.x < 0)
            {
                spriteRenderer.flipX = true;  // 往左走，水平翻轉
            }
        }
    }
}
