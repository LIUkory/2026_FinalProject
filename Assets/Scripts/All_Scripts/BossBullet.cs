using UnityEngine;

public class BossBullet: MonoBehaviour
{
    [Header("子彈設定")]
    public float speed = 5f;        // 飛行速度
    public float lifetime = 5f;     // 存活時間 (秒)，時間到自動銷毀，避免塞爆記憶體
    public float damage = 10f;      // 打到玩家的傷害量

    void Start()
    {
        // 1. 生命週期設定：生成後幾秒自動銷毀
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // 2. 讓子彈朝著「自己的正前方」飛行
        // 因為我們在 Boss 腳本裡已經幫子彈設定好旋轉角度了，所以只要一直往前走就會散開！
        // (在 Unity 2D 中，Vector3.right 代表圖片的右邊，通常是 0 度的方向)
        // (如果你的子彈圖片預設是朝上，請把 Vector3.right 改成 Vector3.up)
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    // 3. 碰撞偵測 (記得子彈的 Collider2D 要勾選 Is Trigger)
    void OnTriggerEnter2D(Collider2D other)
    {
        // 如果打到玩家
        if (other.CompareTag("Player"))
        {
            // 嘗試抓取狐狸身上的血量腳本
            Fox_stats playerStats = other.GetComponent<Fox_stats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damage); // 造成傷害
            }

            // 打中玩家後，子彈銷毀
            Destroy(gameObject);
        }
        // 如果打到牆壁 (假設你們場景的牆壁 Tag 叫 Wall)
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
