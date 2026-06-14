using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ClamBoss : MonoBehaviour, IDamageable // 假設你有沿用之前的受傷介面
{
    [Header("基本設定")]
    public float maxHealth = 500f;
    public float currentHealth;
    public float mit = 0.8f;
    public Transform player;
    [Header("偵測與啟動")]
    public float detectionRadius = 10f; // 偵測玩家的範圍
    private bool isActivated = false;   // 是否已經開戰

    [Header("視覺與動畫 (跳躍錯覺用)")]
    public Transform bossSprite;        // ★ 請把蚌殼精的「圖片子物件」拖進來
    public float jumpHeight = 2f;       // 跳躍高度
    public float jumpDuration = 1f;     // 滯空時間

    [Header("攻擊設定")]
    public GameObject bubblePrefab; // 把做好的泡泡 Prefab 拖進這裡
    public GameObject bulletPrefab;     // 螺旋彈幕的子彈
    public GameObject shockwavePrefab;  // 震波的子彈 (可以用不同外觀的子彈當震波)
    public Transform firePoint;         // 發射點 (通常放在中心)

    [Header("BOSS UI 設定 (動態生成模式)")]
    public GameObject bossHpUIPrefab; // ★ 這裡放你在 Project 裡的血條 Prefab
    private BOSSHP spawnedHpBarScript;

    [Header("二階段變身設定")]
    public GameObject bossCamera;       // 拖入剛剛做好的 vcam_boss 物件
    public Sprite phaseTwoSprite;       // 拖入二階段蚌殼打開的精美圖片
    private bool isPhaseTwo = false;    // 是否已經進入第二階段
    private bool isTransitioning = false; // 是否正在播變身動畫

    [Header("二階段技能設定")]
    public GameObject tentaclePrefab;     // 觸手怪物 Prefab (自帶 50 血和打人邏輯)
    public GameObject waterWarningPrefab; // 水柱預警光圈 
    public GameObject waterPillarPrefab; // 水柱預警光圈 
    public GameObject waterEruptionPrefab; // 真正造成傷害的水柱特效/判定區
    // 狀態控制
    private bool isAttacking = false;
    private float spiralAngleOffset = 0f; // 紀錄螺旋彈幕當前的旋轉角度
    private bool isDead = false; 
    private bool damagable = true;
    void Start()
    {
        currentHealth = maxHealth;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        if (player == null || currentHealth <= 0) return;
        // ==========================================
        // ★ 開發者作弊模式：按下 P 鍵，血量直降 260 ★
        // ==========================================

        if (Input.GetKeyDown(KeyCode.P))
        {
            currentHealth = 260f;
            Debug.Log("【開發者測試】按下 P 鍵，BOSS 血量已降至 260！準備觸發二階段！");

            // ★ 非常重要：手動更新血條 UI，不然畫面上血條看起來還是滿的
            if (spawnedHpBarScript != null)
            {
                spawnedHpBarScript.UpdateHealthBar(currentHealth, maxHealth);
            }

            // 如果你之前沒有用動態 UI，而是用寫死的 hpFillImage，請用這段：
            // if (hpFillImage != null) hpFillImage.fillAmount = currentHealth / maxHealth;
        }
        // ==========================================

        // 1. 偵測玩家是否進入範圍 (啟動 BOSS)
        if (!isActivated)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance <= detectionRadius)
            {
                isActivated = true;
                Debug.Log("蚌殼精甦醒，進入戰鬥！");
                if (BGM_manager.instance != null)
                {
                    BGM_manager.instance.SwitchToBossMusic();
                }
                Canvas mainCanvas = Object.FindAnyObjectByType<Canvas>();
                if (mainCanvas != null && bossHpUIPrefab != null)
                {
                    GameObject hpUIObject = Instantiate(bossHpUIPrefab, mainCanvas.transform);
                    hpUIObject.SetActive(true);

                    // 2. ★ 核心修改：直接抓取血條身上的 BossHPBar 腳本！
                    spawnedHpBarScript = hpUIObject.GetComponent<BOSSHP>();
                }
            }
            return; // 還沒啟動前，什麼都不做
        }

        // 2. 判斷血量狀態 (> 50% 為第一階段)
        if (currentHealth > maxHealth * 0.5f)
        {
            // 如果沒有在攻擊，就隨機挑一個技能放
            if (!isAttacking)
            {
                StartCoroutine(PhaseOneAttackRoutine());
            }
        }
        else
        {
            // ★ 核心修改：血量低於 50%，且還沒進入二階段、也沒在變身時，觸發變身！
            if (!isPhaseTwo && !isTransitioning)
            {
                StartCoroutine(StartBossTransformRoutine());
            }
            else if (isPhaseTwo && !isTransitioning && !isAttacking)
            {
                StartCoroutine(PhaseTwoAttackRoutine());
            }
        }

    }

    // 負責隨機選擇一階段的攻擊
    IEnumerator PhaseOneAttackRoutine()
    {
        isAttacking = true;

        // 隨機決定要放彈幕(0)還是跳躍震波(1)
        int attackChoice = Random.Range(0, 3);

        if (attackChoice == 0)
        {
            yield return StartCoroutine(SpiralBulletAttack());
        }
        else if (attackChoice == 1)
        {
            yield return StartCoroutine(JumpAndShockwaveAttack());
        }
        else
        {
            yield return StartCoroutine(BubbleAttack());
        }

            // 攻擊結束後，休息 2 秒再放下一招
            yield return new WaitForSeconds(2f);
        isAttacking = false;
    }

    // --- 技能一：螺旋彈幕 ---
    IEnumerator SpiralBulletAttack()
    {
        Debug.Log("蚌殼精使用：螺旋彈幕！");
        int waves = 20; // 總共射幾波
        float fireRate = 0.15f; // 每波間隔時間

        for (int i = 0; i < waves; i++)
        {
            // 四個基礎方向：上、下、左、右 (0, 90, 180, 270)
            float[] baseAngles = { 0f, 90f, 180f, 270f };

            foreach (float angle in baseAngles)
            {
                // 基礎角度加上偏移量，形成螺旋效果
                float finalAngle = angle + spiralAngleOffset;
                Quaternion rotation = Quaternion.Euler(0, 0, finalAngle);

                Instantiate(bulletPrefab, firePoint.position, rotation);
            }

            // 每次發射後，角度增加 15 度 (可調整螺旋密集度)
            spiralAngleOffset += 15f;
            yield return new WaitForSeconds(fireRate);
        }
    }

    // --- 技能二：跳躍與環形震波 ---
    // --- 技能二：跳躍與環形震波 (完整版) ---
    IEnumerator JumpAndShockwaveAttack()
    {
        Debug.Log("蚌殼精使用：跳躍砸地！");

        Vector3 originalPos = bossSprite.localPosition;
        Vector3 peakPos = originalPos + new Vector3(0, jumpHeight, 0);

        // 1. 蓄力準備 (停頓一下)
        yield return new WaitForSeconds(0.5f);

        // 2. 往上跳 (視覺效果)
        float elapsed = 0f;
        while (elapsed < jumpDuration / 2f)
        {
            bossSprite.localPosition = Vector3.Lerp(originalPos, peakPos, elapsed / (jumpDuration / 2f));
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 3. 滯空停頓
        yield return new WaitForSeconds(0.2f);

        // 4. 砸落地面 (速度極快，營造重量感)
        elapsed = 0f;
        while (elapsed < 0.1f)
        {
            bossSprite.localPosition = Vector3.Lerp(peakPos, originalPos, elapsed / 0.1f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        bossSprite.localPosition = originalPos; // 確保完美貼齊地面

        // 5. 碰！砸到地面的瞬間，觸發同心圓震波！
        yield return StartCoroutine(SpawnConcentricShockwaves());
    }

    // --- 新的同心圓震波邏輯 (由砸地觸發) ---
    IEnumerator SpawnConcentricShockwaves()
    {
        int totalRings = 3;             // 總共震幾圈
        float radiusStep = 10.0f;        // 每一圈的距離差多少
        float delayBetweenRings = 0.75f; // 每圈之間的延遲時間 (秒)
        int baseProjectiles = 5;        // 第一圈有幾顆震波

        for (int ring = 1; ring <= totalRings; ring++)
        {
            // 每一圈的半徑越來越大
            float currentRadius = ring * radiusStep;

            // 越外圈範圍越大，需要的震波數量也要變多，才不會有空隙
            int currentProjectiles = baseProjectiles + (ring * 2);
            float angleStep = 360f / currentProjectiles;

            for (int i = 0; i < currentProjectiles; i++)
            {
                // 計算這顆震波應該在哪個角度
                float angle = i * angleStep;

                // 數學魔法：利用 Sin 和 Cos 算出圓上的座標點
                float x = Mathf.Cos(angle * Mathf.Deg2Rad) * currentRadius;
                float y = Mathf.Sin(angle * Mathf.Deg2Rad) * currentRadius;
                Vector3 spawnPos = firePoint.position + new Vector3(x, y, 0);

                // 強制角度永遠為 0 度 (Quaternion.identity)，圖片不旋轉
                Instantiate(shockwavePrefab, spawnPos, Quaternion.identity);
            }

            // 生完一圈後，暫停 0.3 秒，再繼續往外炸下一圈
            yield return new WaitForSeconds(delayBetweenRings);
        }
    }
    IEnumerator BubbleAttack()
    {
        Debug.Log("蚌殼精使用：深海泡泡！");

        // 連續發射 3 顆泡泡
        for (int i = 0; i < 3; i++)
        {
            // 隨機產生 0 到 360 度的角度
            float randomAngle = Random.Range(0f, 360f);
            Quaternion rotation = Quaternion.Euler(0, 0, randomAngle);

            // 生成泡泡
            Instantiate(bubblePrefab, firePoint.position, rotation);
        }

        // 發射完畢，稍微停頓一下
        yield return new WaitForSeconds(1f);
    }
    // --- 實作受傷邏輯 ---
    public void TakeDamage(float damage)
    {
        if (isDead) return;
        if (!damagable) return;
        Debug.Log("【系統通知】BOSS 的 TakeDamage 被呼叫了！收到的傷害值 = " + damage);
        if (currentHealth > maxHealth / 2)
        {
            currentHealth -= damage;
        }
        else
        {
            currentHealth -= damage * mit;
        }
        // 這裡可以加上閃紅光特效
        // ★ 修改：直接叫血條腳本去更新比例！
        if (spawnedHpBarScript != null)
        {
            spawnedHpBarScript.UpdateHealthBar(currentHealth, maxHealth);
        }
        if (currentHealth <= 0)
        {
            StartCoroutine(BossDeathRoutine());
        }
        Debug.Log("【系統通知】BOSS 扣血成功！目前剩餘血量 = " + currentHealth);
    }
  
    
    // --- 視覺化除錯 (在 Scene 視圖看到範圍) ---
    void OnDrawGizmosSelected()
    {
        // 畫出啟動偵測範圍 (黃色圓圈)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
    IEnumerator StartBossTransformRoutine()
    {
        isTransitioning = true;
        isAttacking = true; // 鎖定 AI 狀態，防止 Update 重複觸發

        // ★ 核心修正：千萬不能用 StopAllCoroutines()，否則會把自己掐死！
        // 我們讓 isAttacking = true 就可以有效阻止 PhaseOneAttackRoutine 繼續跑了。

        Debug.Log("【劇情動畫】BOSS 血量低於 50%，準備變身！");
        damagable = false;
        // 1. 彈幕清場
        GameObject[] activeBullets = GameObject.FindGameObjectsWithTag("BossBullet");
        foreach (GameObject bullet in activeBullets)
        {
            Destroy(bullet);
        }

        // 2. 定格玩家
        Fox_movement playerMove = player.GetComponent<Fox_movement>();
        // 注意：如果你朋友改過武器腳本名稱，這裡要確認一下
        Vehicle_weapon playerWeapon = player.GetComponentInChildren<Vehicle_weapon>();
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();

        if (playerMove != null) playerMove.enabled = false;
        if (playerWeapon != null) playerWeapon.enabled = false;
        if (playerRb != null) playerRb.linearVelocity = Vector2.zero;
        // 3. 切換鏡頭：看 BOSS
        if (bossCamera != null) bossCamera.SetActive(true);

        // 等待鏡頭滑過去
        yield return new WaitForSeconds(1.5f);

        // 4. 進行變身：更換 Sprite 圖片
        // ★ 安全修正：加上防呆檢測，看看是不是這裡找不到組件
        SpriteRenderer sr = null;
        if (bossSprite != null)
        {
            sr = bossSprite.GetComponent<SpriteRenderer>();
        }
        else
        {
            // 如果 bossSprite 沒拉，就嘗試抓自己身上的
            sr = GetComponent<SpriteRenderer>();
        }

        if (sr != null && phaseTwoSprite != null)
        {
            sr.sprite = phaseTwoSprite;
            Debug.Log("【劇情動畫】變身成功！蚌殼已打開！");
           
        }
        else
        {
            Debug.LogError("【錯誤】變身失敗！找不到 SpriteRenderer 或沒拉 PhaseTwoSprite 圖片！");
        }

        // 爆氣特寫停留 2 秒
        yield return new WaitForSeconds(2.0f);

        // 5. 結束動畫：關閉 BOSS 相機，鏡頭自動回歸玩家
        if (bossCamera != null) bossCamera.SetActive(false);

        // 等待鏡頭滑回玩家身邊
        yield return new WaitForSeconds(1.5f);

        // 6. 恢復控制
        if (playerMove != null) playerMove.enabled = true;
        if (playerWeapon != null) playerWeapon.enabled = true;

        // 7. 正式開戰
        isPhaseTwo = true;
        isTransitioning = false;
        isAttacking = false;
        damagable = true;
        Debug.Log("【二階段開戰】進入第二階段戰鬥！");
    }
    // --- 二階段大腦：處理 30% 雙重施法 ---
    IEnumerator PhaseTwoAttackRoutine()
    {
        isAttacking = true;

        // 隨機決定是否觸發雙重施法 (0~99 抽數字，小於 30 就是中獎)
        bool isDoubleCast = Random.Range(0, 100) < 30;
        int tentacleCount = FindObjectsByType<TentacleEnemy>(FindObjectsSortMode.None).Length;
        bool isTentacleFull = (tentacleCount >= 3);
        // 抽出第一個技能 (0:螺旋, 1:泡泡, 2:觸手, 3:水柱)
        int skill1 = Random.Range(0, 4);
        while (skill1 == 2 && isTentacleFull)
        {
            skill1 = Random.Range(0, 4);
        }
        StartCoroutine(ExecutePhaseTwoSkill(skill1));

        if (isDoubleCast)
        {
            // 抽出第二個技能，如果跟第一個一樣就重抽，確保兩招不同
            int skill2 = Random.Range(0, 4);
            while (skill2 == skill1 || (skill2 == 2 && isTentacleFull))
            {
                skill2 = Random.Range(0, 4);
            }

            Debug.Log("【二階段發威】觸發雙重施法！技能 " + skill1 + " 與 " + skill2 + " 同時施放！");
            StartCoroutine(ExecutePhaseTwoSkill(skill2));
        }

        // 攻擊後搖，休息 2.5 秒再放下一輪 (可調整難度)
        yield return new WaitForSeconds(2.5f);
        isAttacking = false;
    }

    // --- 負責執行具體技能的派發中心 ---
    IEnumerator ExecutePhaseTwoSkill(int skillID)
    {
        switch (skillID)
        {
            case 0: yield return StartCoroutine(SpiralBulletAttack()); break; // 沿用舊技能
            case 1: yield return StartCoroutine(BubbleAttack()); break;       // 沿用舊技能
            case 2: yield return StartCoroutine(SummonTentacle()); break;     // 新技能：觸手
            case 3: yield return StartCoroutine(WaterPillarStrike()); break;  // 新技能：水柱
        }
    }

    // --- 新技能：召喚觸手 ---
    IEnumerator SummonTentacle()
    {
        Debug.Log("【BOSS 技能】召喚深海觸手！");

        // 在玩家周圍 3~5 單位內隨機挑一個點生成觸手
        Vector2 randomOffset = Random.insideUnitCircle.normalized * Random.Range(3f, 5f);
        Vector3 spawnPos = player.position + new Vector3(randomOffset.x, randomOffset.y, 0);

        if (tentaclePrefab != null)
        {
            Instantiate(tentaclePrefab, spawnPos, Quaternion.identity);
        }

        yield return null;
    }

    // --- 新技能：天降水柱 (預警閃爍 + 打擊) ---
    IEnumerator WaterPillarStrike()
    {
        Debug.Log("【BOSS 技能】天降水柱！鎖定玩家位置！");
        Vector3 targetPos = player.position; // 鎖定玩家當下位置

        // =========================================================
        // ★ 階段 1：生成預警法陣 (Warning, image_5.png) 並閃爍 2 次
        // =========================================================
        GameObject warning = null;
        if (waterWarningPrefab != null)
        {
            warning = Instantiate(waterWarningPrefab, targetPos, Quaternion.identity);
            SpriteRenderer warningSr = warning.GetComponent<SpriteRenderer>();

            // 閃爍 2 次 (顯 0.2s -> 隱 0.2s)
            for (int i = 0; i < 2; i++)
            {
                if (warningSr != null) warningSr.enabled = true;
                yield return new WaitForSeconds(0.2f);
                if (warningSr != null) warningSr.enabled = false;
                yield return new WaitForSeconds(0.2f);
            }
        }

        // =========================================================
        // ★ 階段 2：生成柱體本體 (Body, image_6.png)，兩者同閃 2 次
        // =========================================================
        GameObject pillarBody = null;
        if (waterPillarPrefab != null)
        {
            pillarBody = Instantiate(waterPillarPrefab, targetPos, Quaternion.identity);
            SpriteRenderer pillarBodySr = pillarBody.GetComponent<SpriteRenderer>();
            SpriteRenderer warningSr = null;
            if (warning != null) warningSr = warning.GetComponent<SpriteRenderer>();

            // 確保兩者初始都是顯示狀態，然後同閃 2 次
            for (int i = 0; i < 2; i++)
            {
                // 同時顯示 (顯 0.2s)
                if (warningSr != null) warningSr.enabled = true;
                if (pillarBodySr != null) pillarBodySr.enabled = true;
                yield return new WaitForSeconds(0.2f);

                // 同時隱藏 (隱 0.2s)
                if (warningSr != null) warningSr.enabled = false;
                if (pillarBodySr != null) pillarBodySr.enabled = false;
                yield return new WaitForSeconds(0.2f);
            }
        }

        // =========================================================
        // ★ 階段 3：清理前兩個狀態，砸下完整水花 (Eruption, image_7.png)
        // =========================================================

        // 1. 清理舊物件
        if (warning != null) Destroy(warning);
        if (pillarBody != null) Destroy(pillarBody);

        // 2. 在同一個目標位置生成包含最終傷害判定的完整噴發水花
        if (waterEruptionPrefab != null)
        {
            // 💡 完整噴發 Prefab 身上必須有 Capsule Collider 2D (Is Trigger) 
            // 💡 並掛上 WaterPillar.cs 傷害腳本 (它會自我銷毀)
            Instantiate(waterEruptionPrefab, targetPos, Quaternion.identity);
        }
    }
    IEnumerator BossDeathRoutine()
    {
        isDead = true;
        isAttacking = true; // 鎖定 AI 狀態
        currentHealth = 0;  // 確保數值歸零

        Debug.Log("【BOSS 陣亡】蚌殼精被打敗！啟動謝幕電影鏡頭...");

        // 1. 立即清除血條 UI
        if (spawnedHpBarScript != null)
        {
            Destroy(spawnedHpBarScript.gameObject);
        }

        // 2. 立即將全場的彈幕與觸手蒸發（免得打擾玩家看 BOSS 謝幕）
        GameObject[] activeBullets = GameObject.FindGameObjectsWithTag("BossBullet");
        foreach (GameObject bullet in activeBullets) Destroy(bullet);

        TentacleEnemy[] tentacles = FindObjectsByType<TentacleEnemy>(FindObjectsSortMode.None);
        foreach (TentacleEnemy tentacle in tentacles) Destroy(tentacle.gameObject);

        // 3. 強制踩下煞車定格玩家，防止狐狸在 BOSS 播放死亡動畫時繼續開車亂飆
        Fox_movement playerMove = player.GetComponent<Fox_movement>();
        Vehicle_weapon playerWeapon = player.GetComponentInChildren<Vehicle_weapon>();
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();

        if (playerMove != null) playerMove.enabled = false;
        if (playerWeapon != null) playerWeapon.enabled = false;
        if (playerRb != null) playerRb.linearVelocity = Vector2.zero;

        // 4. 切換虛擬鏡頭：特寫 BOSS
        if (bossCamera != null) bossCamera.SetActive(true);

        // 等待鏡頭平滑滑過去特寫 BOSS (1.5秒)
        yield return new WaitForSeconds(1.5f);

        // 5. 讓 BOSS 本體閃爍四下
        // 安全抓取 SpriteRenderer
        SpriteRenderer sr = (bossSprite != null) ? bossSprite.GetComponent<SpriteRenderer>() : GetComponent<SpriteRenderer>();

        if (sr != null)
        {
            for (int i = 0; i < 4; i++)
            {
                sr.enabled = false;           // 隱形
                yield return new WaitForSeconds(0.15f); // 閃爍頻率
                sr.enabled = true;            // 顯現
                yield return new WaitForSeconds(0.15f);

                // 💡 這裡如果隊友有做出「受傷/爆炸音效」，也非常適合在每次閃爍時播一聲「嗶！」或「碰！」
            }
        }

        // 閃爍完畢後定格 0.2 秒營造消失前的瞬間
        yield return new WaitForSeconds(0.2f);

        // 6. 結束動畫：關閉 BOSS 相機，鏡頭平滑滑回玩家身邊
        if (bossCamera != null) bossCamera.SetActive(false);

        // 等待鏡頭完全滑回玩家
        yield return new WaitForSeconds(1.5f);

        // 7. 恢復玩家控制權（如果遊戲還要繼續的話）
        if (playerMove != null) playerMove.enabled = true;
        if (playerWeapon != null) playerWeapon.enabled = true;

        // 8. 驚天動地大銷毀，BOSS 徹底離開人世
        Destroy(gameObject);

        Debug.Log("【BOSS 陣亡】戰場清理完畢，鏡頭已交還玩家。");
    }
}