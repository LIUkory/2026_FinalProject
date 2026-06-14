using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class BG_fire : MonoBehaviour,IWeapon
{
    public bool isEquipped = false;
    public GameObject bulletPrefab;
    public Transform firePoint;
    [Header("武器數值設定")]
    // ★ 開放給 Inspector 調整的冷卻時間 (單位：秒)
    public float fireCooldown = 0.5f;

    // 紀錄「下一次可以合法開火」的時間點
    private float nextFireTime = 0f;
    public void Equip()
    {
        isEquipped = true;
        Debug.Log("手槍已裝備，準備開火！");
    }
    public void Unequip()
    {
        isEquipped = false;
        Debug.Log("手槍已卸下，安全鎖關閉。");
    }
    void Update()
    {
        if (!isEquipped) return;
        // 點擊滑鼠左鍵開火
        if (Mouse.current.leftButton.wasPressedThisFrame && Time.time >= nextFireTime)
        {
            Shoot();

            // 開火後，立刻把「下一次合法開火時間」往後推延
            nextFireTime = Time.time + fireCooldown;
        }
    }

    void Shoot()
    {
        // 在 FirePoint 的位置生成子彈，並套用跟槍一樣的旋轉角度
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}
