using UnityEngine;
using System.Collections; // 🎯 引入協程（Coroutine）必備

public class Mouse_arrower : BaseEnemyAI
{
    [Header("遠程攻擊設定")]
    public GameObject arrow;

    [Header("弓箭手圖片切換設定")]
    public Sprite attackSprite;       // 🎯 請在 Inspector 放「拉弓/射擊時」的圖片
    public float spriteChangeDuration = 0.2f; // 圖片切換持續時間（秒）

    private SpriteRenderer spriteRenderer;
    private Sprite normalSprite;      // 用來自動記憶老鼠平時的圖片

    // ─── ✨ 體型維穩變數（從大腦偷偷拷貝過來，確保翻轉時不縮水） ───
    private float myOriginalScaleX;
    private float myOriginalScaleY;

    protected override void Start()
    {
        // 1. 先跑大腦基底類別的開機設定
        base.Start();

        // 2. 🎯 自動抓取老鼠身上的 SpriteRenderer，並記住牠原本的日常長相
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            normalSprite = spriteRenderer.sprite;
        }

        // 🎯 記憶體型大小，防止攻擊翻轉時體型突然變大或變小
        myOriginalScaleX = Mathf.Abs(transform.localScale.x);
        myOriginalScaleY = transform.localScale.y;
    }

    protected override void ExecuteAttack()
    {   
        if (player == null || arrow == null) return;

        // 🚀【核心修正：強制面對主角防線】
        // 幾何邏輯：如果主角的 X 座標大於老鼠，代表主角在右邊！
        if (player.position.x > transform.position.x)
        {
            // 主角在右邊 ➔ 根據大腦原本的習慣，往右走是負的 originalScaleX
            transform.localScale = new Vector3(-myOriginalScaleX, myOriginalScaleY, 1f);
        }
        else
        {
            // 主角在左邊 ➔ 往左走是正的 originalScaleX
            transform.localScale = new Vector3(myOriginalScaleX, myOriginalScaleY, 1f);
        }

        Debug.Log("🎯 弓箭手老鼠已完成強制面朝主角，開始蓄力射擊！");

        // 🚀【圖片切換防線】：啟動時間切換協程
        if (spriteRenderer != null && attackSprite != null)
        {
            StartCoroutine(ChangeAttackSpriteRoutine());
        }

        // 1. 生成箭矢實體
        GameObject projectile = Instantiate(arrow, transform.position, Quaternion.identity);

        // 2. 🎯【傷害管道對接】：精準將傷害傳給箭矢的 Arrow 腳本！
        Arrow arrowScript = projectile.GetComponent<Arrow>();
        if (arrowScript != null)
        {
            arrowScript.damageAmount = this.damageAmount;
            Debug.Log($"【管道疏通】已成功將老鼠的攻擊力 {this.damageAmount} 灌進箭矢中！");
        }

        // 3. 【全敵人穿透核心】
        Collider2D projectileCollider = projectile.GetComponent<Collider2D>();

        if (projectileCollider != null)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (GameObject enemy in enemies)
            {
                Collider2D enemyCollider = enemy.GetComponent<Collider2D>();       
                
                if (enemyCollider != null)
                {
                    Physics2D.IgnoreCollision(projectileCollider, enemyCollider);
                }
            }
            Debug.Log($"【物理防線】箭矢已成功穿透場上所有 Enemy 友軍！");
        }
    }

    // ⏳ 圖片閃爍切換的局部小計時器（協程）
    private IEnumerator ChangeAttackSpriteRoutine()
    {
        // 換成主人準備好的帥氣拉弓圖
        spriteRenderer.sprite = attackSprite;

        // 等待設定好的時間（例如 0.2 秒）
        yield return new WaitForSeconds(spriteChangeDuration);

        // 射擊結束，全自動換回原本正常的走路/巡邏圖
        spriteRenderer.sprite = normalSprite;
    }
}