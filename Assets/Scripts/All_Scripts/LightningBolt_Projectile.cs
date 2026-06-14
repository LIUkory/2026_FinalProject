using UnityEngine;

public class LightningBolt_Projectile : MonoBehaviour
{
    [Header("長矛設定")]
    public float damage = 50f;
    public float lifeTime = 4f; // 4秒後不管有沒有瞬移，都會自動消失

    private Rigidbody2D rb;
    private bool isStuck = false; // 判斷是否已經插在東西上了

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // 丟出去的瞬間開始倒數，時間到自動摧毀，避免佔用記憶體
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 如果已經插在東西上了，就不准再觸發任何碰撞（避免穿透好幾隻怪）
        if (isStuck) return;

        // ★ 防呆：千萬要忽略玩家自己，不然長矛一生成就會插在主角臉上！
        if (collision.CompareTag("Player")) return;

        // 如果碰到敵人、BOSS 或是牆壁
        // (請確定你的牆壁有設定 Tag，例如 "Wall" 或 "Environment")
        if (collision.CompareTag("Enemy") || collision.CompareTag("Boss") || collision.CompareTag("Wall"))
        {
            StickIntoTarget(collision.transform);

            // ==========================================
            // ★ 處理傷害邏輯 (請依據你的專案修改這裡)
            // ==========================================

            // 如果你的敵人/BOSS 有掛 IDamageable 介面：
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }

            // 如果你是直接呼叫敵人的特定腳本 (例如 Enemy_stats)：
            // Enemy_stats enemy = collision.GetComponent<Enemy_stats>();
            // if (enemy != null) enemy.TakeDamage(damage);

            Debug.Log($"【雷電長矛】擊中 {collision.name}！長矛已固定。");
        }
    }

    // 負責讓長矛「插」進目標的函式
    private void StickIntoTarget(Transform target)
    {
        isStuck = true;

        // 1. 踩煞車：讓長矛瞬間停下來
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false; // ★ 關閉物理模擬，讓它變成一個純視覺裝飾品，才不會卡住敵人的移動
        }

        // 2. 核心魔法：把長矛變成目標的「子物件」！
        // 這樣如果目標是會走動的敵人，長矛就會牢牢插在牠身上跟著牠走！
        transform.SetParent(target);
    }
}
