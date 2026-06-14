using System.Collections.Generic;
using UnityEngine;

public class Vehicle_weapon : MonoBehaviour,IWeapon
{
    public bool isEquipped = false; // ★ 新增：裝備開
    [Header("騎乘設定 (Riding)")]
    public float speedBoost = 5f;           // 按住左鍵時增加的速度
    public float ridingDamage = 15f;        // 騎車撞到敵人的傷害
    public float ridingHitRadius = 1.2f;    // 騎車撞擊的判定半徑
    public float ridingDamageCooldown = 0.5f; // 對同一隻老鼠的衝撞傷害冷卻

    [Header("投擲設定 (Throwing)")]
    public GameObject vehicleProjectilePrefab; // 飛出去的電動車 Prefab
    public Transform firePoint;                // 發射點 (槍口)

    [Header("系統綁定 (自動抓取)")]
    public Fox_movement playerMovement;        // 玩家的移動腳本
    public WeaponAim weaponAimScript;         // 武器的瞄準腳本

    // --- 內部狀態變數 ---
    private bool isRiding = false;
    private bool isWeaponMissing = false;      // 武器是否飛出去了

    private Transform playerTransform;
    private Dictionary<Collider2D, float> ridingHitCooldowns = new Dictionary<Collider2D, float>();
    public void Equip()
    {
        isEquipped = true;

        // 撿起來的時候才去抓玩家身上的腳本，保證不會抓錯！
        playerTransform = transform.root;
        playerMovement = playerTransform.GetComponentInChildren<Fox_movement>();
        weaponAimScript = playerTransform.GetComponentInChildren<WeaponAim>();

        Debug.Log("電動車已裝備！");
    }
    public void Unequip()
    {
        isEquipped = false;

    }
    void Start()
    {
        // 自動往上層 (狐狸本體) 尋找移動腳本與 Transform
        playerTransform = transform.root;
        if (playerMovement == null) playerMovement = playerTransform.GetComponent<Fox_movement>();

        // 自動抓取掛在同一層 (WeaponHolder) 的瞄準腳本
        if (weaponAimScript == null) weaponAimScript = GetComponent<WeaponAim>();
    }

    void Update()
    {
        if (!isEquipped) return;

        if (isWeaponMissing) return;

        // 1. 按下左鍵
        if (Input.GetMouseButtonDown(0)) StartRiding();

        // 2. 按住左鍵
        if (Input.GetMouseButton(0) && isRiding) CheckRidingCollision();

        // 3. 放開左鍵
        if (Input.GetMouseButtonUp(0) && isRiding) StopRidingAndThrow();
    }

    private void StartRiding()
    {
        isRiding = true;

        // ★ 核心防衝突：強制關閉 Weapon_aim！
        // 這樣騎車時，車頭就不會被滑鼠強迫轉向，也不會發生上下顛倒的翻車 Bug
        if (weaponAimScript != null) weaponAimScript.enabled = false;

        // 將武器角度歸零，讓車子乖乖朝前
        transform.localRotation = Quaternion.identity;

        // 幫玩家加速
        if (playerMovement != null)
        {
            playerMovement.moveSpeed += speedBoost;
        }

        Debug.Log("騎上電動車！速度提升！瞄準系統已鎖定！");
    }

    private void CheckRidingCollision()
    {
        // 畫一個隱形的圓圈，偵測玩家周圍有沒有敵人
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(playerTransform.position, ridingHitRadius);

        // ★ 新增這段除錯代碼：看看這台車到底有沒有「摸到」任何東西
        if (hitEnemies.Length > 0)
        {
            foreach (Collider2D col in hitEnemies)
            {
                Debug.Log("【騎車雷達】有摸到物件：" + col.name + "，它的 Tag 是：" + col.tag);
            }
        }

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                if (!ridingHitCooldowns.ContainsKey(enemy) || Time.time >= ridingHitCooldowns[enemy])
                {
                    IDamageable damageable = enemy.GetComponentInParent<IDamageable>();

                    if (damageable != null)
                    {
                        damageable.TakeDamage(ridingDamage);
                        ridingHitCooldowns[enemy] = Time.time + ridingDamageCooldown;
                        Debug.Log("騎車輾過敵人！造成了 " + ridingDamage + " 點傷害！");
                    }
                }
            }
        }
    }

    private void StopRidingAndThrow()
    {
        isRiding = false;

        // 扣除加速，把玩家速度恢復原狀
        if (playerMovement != null)
        {
            playerMovement.moveSpeed -= speedBoost;
        }

        // ★ 核心防衝突：丟出去的瞬間，重新啟動 Weapon_aim！
        // 這樣 FirePoint 就會瞬間對準滑鼠，確保車子是往你游標的方向飛出去
        if (weaponAimScript != null) weaponAimScript.enabled = true;

        // 執行丟擲動作
        ThrowVehicle();
    }

    private void ThrowVehicle()
    {
        isWeaponMissing = true;

        // ★ 改進：不管 SpriteRenderer 掛在自己身上還是子物件，都把它藏起來
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        GameObject thrownVehicle = Instantiate(vehicleProjectilePrefab, firePoint.position, firePoint.rotation);
        thrownVehicle.GetComponent<Vehicle_projectile>().Initialize(this);
    }

    public void CatchVehicle()
    {
        isWeaponMissing = false;

        // ★ 改進：車子回來時重新顯示
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null) sr.enabled = true;

        Debug.Log("電動車飛回來了！");
    }

    // 在 Unity 編輯器裡畫出衝撞判定的紅色圓圈，方便你調整大小
    private void OnDrawGizmosSelected()
    {
        if (playerTransform == null) playerTransform = transform.root;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerTransform.position, ridingHitRadius);
    }
}
