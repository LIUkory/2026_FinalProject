using UnityEngine;

public class Slime_B : BaseEnemyAI 
{   
    [Header("遠程攻擊設定")]
    public GameObject waterBallPrefab;
    [Header("連環跳設定")]
    [Tooltip("每次跳躍在空中往上衝的時間（秒）")]
    public float jumpDuration = 0.2f;  
    [Tooltip("跳躍的瞬間向上附加速度，覺得不明顯可以改大（例如 8 或 10）")]
    public float jumpForce = 5f;       

    private float airTimer = 0f;
    private bool isGoingUp = true; // 用來標記目前是在跳上去還是掉下來

    protected override void FixedUpdate()
    {
        // ─── 🚀 移動：完全交給老鼠大腦 ───
        // 讓大腦原本的物理追擊正常跑，全方位追擊，X 與 Y 軸速度基礎動能不卡死
        base.FixedUpdate(); 

        if (rb == null) return;

        // ─── 📐 跳躍：採用指定的 rb.linearVelocity 疊加寫法 ───
        airTimer += Time.fixedDeltaTime;

        if (airTimer < jumpDuration)
        {
            if (isGoingUp)
            {
                // ✨ 執行公式：前半段往上跳！
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y + jumpForce);
            }
            else
            {
                // ✨ 執行公式反向：後半段掉下來！
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y - jumpForce);
            }
        }
        else
        {
            // ✨【完全不停頓】：時間一到，立刻切換狀態，並把時間重置為 0 進入下一次循環！
            airTimer = 0f;
            isGoingUp = !isGoingUp; // 往上變往下，往下變往上，無限循環連環跳！
        }
        
    }


    // ─── 🎯 遠程攻擊：進入大腦的 Stop Distance 後自動觸發 ───
    protected override void ExecuteAttack()
    {   
        if (player == null || waterBallPrefab == null) return;

        Debug.Log("史萊姆發射水泡，開始處理動態傷害與全敵人穿透！");

        // 1. 生成水泡實體
        GameObject projectile = Instantiate(waterBallPrefab, transform.position, Quaternion.identity);

        // 2. 🎯【傷害管道對接】：精準將史萊姆的傷害傳給水泡
        WaterBall_NewForce waterBallScript = projectile.GetComponent<WaterBall_NewForce>();
        if (waterBallScript != null)
        {
            waterBallScript.damageAmount = this.damageAmount;
        }

        // 3. 🎯【全敵人穿透核心】：找出水泡自己的碰撞器
        Collider2D projectileCollider = projectile.GetComponent<Collider2D>();

       

        if (projectileCollider != null)

        {

            // 🔍 透過「全場景標籤搜尋」，把所有目前在場上、Tag 是 Enemy 的怪物通通找出來

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");



            foreach (GameObject enemy in enemies)

            {

                // 抓取每一隻怪物的碰撞器

                Collider2D enemyCollider = enemy.GetComponent<Collider2D>();       

                // 🚀 只要怪物有碰撞器，就告訴 Unity 物理引擎：「水泡跟這隻怪物互相穿透！」

                if (enemyCollider != null)

                {

                    Physics2D.IgnoreCollision(projectileCollider, enemyCollider);

                }

            }

            Debug.Log($"【物理防線】水泡已成功無視場上所有 Tag 為 Enemy 的怪物碰撞！");

        }

    }

}