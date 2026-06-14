using UnityEngine;

public class LightningBolt_skill : Basic_skill
{
    [Header("雷電長矛設定")]
    public GameObject spearPrefab;
    public Transform firePoint;
    public float throwForce = 20f;

    private GameObject currentSpear; // 記錄目前丟出去的長矛

    protected override void Update()
    {
        // ==========================================
        // ★ 核心攔截：第二段技能 (瞬移)
        // ==========================================
        // 如果長矛還插在場上，且玩家又按下了父類別設定的專屬按鍵 (skillKey)
        if (currentSpear != null && Input.GetKeyDown(skillKey))
        {
            TeleportToSpear();
            return; // 執行完瞬移就跳出，絕對不讓底層的 Update 繼續跑
        }

        // 如果沒有長矛在場上，就乖乖讓父類別處理冷卻計算與第一段施放
        base.Update();
    }

    // ==========================================
    // ★ 第一段技能：父類別規定的發動邏輯
    // ==========================================
    protected override void ActivateSkillLogic()
    {
        ThrowSpear();
    }

    private void ThrowSpear()
    {
        // 1. 更精準的滑鼠座標轉換 (解決視角與深度偏差)
        Vector3 screenMousePos = Input.mousePosition;
        // ★ 關鍵修正：告訴 Unity 螢幕點擊的深度，等於相機到地面的距離
        if (Camera.main != null)
        {
            screenMousePos.z = Mathf.Abs(Camera.main.transform.position.z);
        }
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(screenMousePos);
        mouseWorldPos.z = 0f;

        // 2. 計算方向與角度
        Vector2 throwDirection = (mouseWorldPos - firePoint.position).normalized;
        float angle = Mathf.Atan2(throwDirection.y, throwDirection.x) * Mathf.Rad2Deg;

       
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        // (把情況 A 或 B 其中一個的註解拿掉，看哪個符合你的圖片)
        // ==========================================

        // 3. 生成長矛
        currentSpear = Instantiate(spearPrefab, firePoint.position, rotation);
        Rigidbody2D rb = currentSpear.GetComponent<Rigidbody2D>();

        // 4. 給予速度飛出去
        if (rb != null)
        {
            rb.linearVelocity = throwDirection * throwForce;
        }

        Debug.Log("【雷電長矛】第一段：精準投擲！技能進入冷卻");
    }

    private void TeleportToSpear()
    {
        // 1. 把玩家的位置，瞬間移動到長矛的位置
        transform.position = currentSpear.transform.position;

        // 2. 拔出長矛 (刪除物件)
        Destroy(currentSpear);
        currentSpear = null;

        // 💡 這裡可以加上超帥的雷電瞬移粒子特效！
        Debug.Log("【雷電長矛】第二段：飛雷神瞬移！");
    }
}