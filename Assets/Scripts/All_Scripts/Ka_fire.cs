using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Ka_fire : MonoBehaviour, IWeapon
{
    public bool isEquipped = false;
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("🎯 三向散射設定")]
    private float[] spreadAngles = { 30f, 0f, -30f };

    [Header("⚡ 加特林連發與 CD 設定")]
    [Tooltip("每發子彈之間的射速間隔（秒）")]
    public float fireRate = 0.1f;       
    [Tooltip("過熱後的 CD 冷卻時間（秒）")]
    public float cooldownDuration = 2.0f; 
    public float conshoot_time = 10f;
    private float nextFireTime = 0f;    // 控管射速間隔的計時器
    private int bulletCount = 0;        // 目前累計射擊次數
    private bool isOverheated = false;  // 是否處於過熱 CD 狀態

    private SpriteRenderer mySpriteRenderer;

    void Awake()
    {
        // 抓取加特林本體身上的圖片渲染器，用來改顏色
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Equip()
    {
        isEquipped = true;
        Debug.Log("加特林已裝備，準備火線壓制！");
    }

    public void Unequip()
    {
        isEquipped = false;
        // 如果卸下武器，確保把狀態和顏色還原
        ResetWeaponState();
        Debug.Log("加特林已卸下。");
    }

    void Update()
    {
        if (!isEquipped) return;
        
        // 🛑 如果目前過熱中，直接罷工，不允許開火
        if (isOverheated) return;

        // 🚀【連發優化】：將 wasPressedThisFrame 改為 isPressed
        // 這樣主人只要「按住滑鼠左鍵不放」，加特林就會自動突突突狂射！
        if (Mouse.current.leftButton.isPressed && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate; // 計算下一發能開火的時間
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        Debug.Log("💥 觸發三向擴散射擊！");

        // 利用迴圈同時生成三顆不同角度的子彈
        foreach (float angleOffset in spreadAngles)
        {
            Quaternion bulletRotation = firePoint.rotation * Quaternion.Euler(0f, 0f, angleOffset);
            Instantiate(bulletPrefab, firePoint.position, bulletRotation);
        }

        // 📈 每射擊一次（噴出三顆子彈），計數器 +1
        bulletCount++;
        Debug.Log($"🔥 加特林熱度：{bulletCount} / 10");

        // 🚨 射滿 10 發，觸發過熱變紅機制！
        if (bulletCount >= conshoot_time)
        {
            StartCoroutine(OverheatCooldownRoutine());
        }
    }

    // ⏳ 控管過熱變紅與 CD 的時空協程
    private IEnumerator OverheatCooldownRoutine()
    {
        isOverheated = true;
        Debug.LogWarning("⚠️ 加特林槍管過熱！強制進入冷卻狀態！");

        // 🎨【視覺變紅】：強行把加特林的 Sprite 渲染顏色改成紅色
        if (mySpriteRenderer != null)
        {
            mySpriteRenderer.color = Color.red; 
        }

        // ⏳ 罰站等待冷卻時間（預設 2 秒）
        yield return new WaitForSeconds(cooldownDuration);

        // 🔄 冷卻結束，還原武器狀態
        ResetWeaponState();
        Debug.Log("✅ 槍管冷卻完畢，可以再度開火！");
    }

    // 🛠️ 還原加特林狀態的專用方法
    private void ResetWeaponState()
    {
        isOverheated = false;
        bulletCount = 0;

        // 🎨【顏色還原】：把顏色洗回純白色（即圖片原本的外觀顏色）
        if (mySpriteRenderer != null)
        {
            mySpriteRenderer.color = Color.white; 
        }
    }
}