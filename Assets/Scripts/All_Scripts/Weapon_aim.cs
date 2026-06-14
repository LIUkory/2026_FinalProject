using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponAim : MonoBehaviour
{
    // ??? 可以把 public bool isEquipped = false; 這行直接刪掉了！不需要它了！

    private SpriteRenderer foxSR;

    void Start()
    {
        foxSR = GetComponentInParent<SpriteRenderer>();
    }

    void Update()
    {
        // ★ 核心改動：如果 Holder 底下沒有子物件（代表手上沒槍），就不執行旋轉！
        if (transform.childCount == 0) return;

        // 基礎防呆
        if (Mouse.current == null || Camera.main == null) return;

        // --- 1. 計算滑鼠世界座標 ---
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        float zDistance = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, zDistance));
        worldMousePos.z = transform.position.z;

        // --- 2. 處理武器旋轉 ---
        Vector2 direction = worldMousePos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // --- 3. 處理狐狸圖片反轉 ---
        if (foxSR != null)
        {
            if (worldMousePos.x > foxSR.transform.position.x)
                foxSR.flipX = false;
            else
                foxSR.flipX = true;
        }

        // --- 4. 處理槍枝上下顛倒 ---
        if (angle > 90 || angle < -90)
            transform.localScale = new Vector3(1, -1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
    }
}