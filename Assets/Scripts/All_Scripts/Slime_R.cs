 using UnityEngine;

public class Slime_R : BaseEnemyAI 
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

    [Header("激光位置微調")]
    [Tooltip("雷射起點離史萊姆中心的距離，調大可以讓雷射頭離身體更遠一點")]
    public float laserOffsetDistance = 0.5f; 

    // ─── 🎯 舊開關轉傳大腦 ───
    protected override void ExecuteAttack()
    {   
        FireLaser();
    }

    // ─── 🎯 紅色史萊姆激光攻擊 ───
    public void FireLaser()
    {   
        // 🎯 主角死亡安全防線
        if (player == null || !player.gameObject.activeInHierarchy || waterBallPrefab == null) 
        {
            Debug.Log("主角已死亡或不存在，停止蓄力激光！");
            return; 
        }

        Debug.Log("紅色史萊姆發射激光！");

        // 📐 1. 計算面朝 Fox 的旋轉角度
        Vector2 direction = (player.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // 🎯 核心旋轉：如果畫面歪 90 度就寫 angle - 90f，如果現在角度是對的就維持 angle
        Quaternion laserRotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // 🚀【位置回歸】：從史萊姆本體出發，沿著方向稍微往前推一點點（避開身體中心）
        Vector3 spawnPosition = transform.position + (Vector3)(direction * laserOffsetDistance);

        // 📦 2. 生成激光
        GameObject laserObj = Instantiate(waterBallPrefab, spawnPosition, laserRotation);

        // 讓激光成為史萊姆的子物件，這樣史萊姆跳躍時激光會跟著同步位移
        laserObj.transform.SetParent(this.transform);

        // 3. 🎯【傷害管道對接】
        var laserScript = laserObj.GetComponent("LaserAttack") as MonoBehaviour;
        if (laserScript != null)
        {
            laserScript.SendMessage("SetDamage", this.damageAmount, SendMessageOptions.DontRequireReceiver);
        }
    }
}
