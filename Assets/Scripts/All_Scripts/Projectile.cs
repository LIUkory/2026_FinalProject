using UnityEngine;

public class Projectile : MonoBehaviour
{
   public float speed = 10f;
    public float lifeTime = 2f; // 子彈生存時間，避免射出去後無窮無盡吃資源
    private Rigidbody2D rb;
    public float damage;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // 讓子彈朝著生成的右方（武器指向的方向）飛出去
        rb.linearVelocity = transform.right * speed;
        
        // 時間到自動刪除
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // 1. 如果撞到的物件，身上帶有 "Enemy" 這個標籤
        if (hitInfo.CompareTag("Enemy"))
        {
            // 嘗試抓取敵人身上的 IDamageable 介面 (建議用 GetComponentInParent 比較保險)
            IDamageable enemyTarget = hitInfo.GetComponentInParent<IDamageable>();

            // 如果這個敵人真的有受傷機制
            if (enemyTarget != null)
            {
                // 把這顆子彈專屬的 damage 數值，傳給敵人的扣血函式！
                enemyTarget.TakeDamage(damage);
            }

            // (未來可以在這裡呼叫敵人的扣血函數，例如：)
            // hitInfo.GetComponent<EnemyHealth>().TakeDamage(10);

            // 銷毀子彈自己
            Destroy(gameObject);
        }

        // 2. 順便處理撞到牆壁的狀況（記得你之前 Console 出現過的 Wall 標籤嗎？）
        else if (hitInfo.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
