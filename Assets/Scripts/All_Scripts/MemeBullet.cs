using UnityEngine;

public class MemeBullet : MonoBehaviour
{
    [Header("子彈設定")]
    public float speed = 15f;
    public float damage = 10f;
    public float lifeTime = 2f;

    private Vector3 flyDirection; 

    void Start()
    {
        flyDirection = transform.right;
        transform.rotation = Quaternion.identity; // 確保 6 永遠是 6，不變 9

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += flyDirection * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // 🌟 終極無視清單：直接穿過玩家、地上武器、大 Boss 的子彈、小怪的投射物！
        if (hitInfo.CompareTag("Player") || 
            hitInfo.CompareTag("Weapons") || 
            hitInfo.CompareTag("BossBullet") || 
            hitInfo.CompareTag("EnemyProjectile")) 
        {
            return; // 直接飄過去，當作沒看到，不銷毀自己
        }

        // 檢查是不是撞到敵人本體
        IDamageable enemy = hitInfo.GetComponent<IDamageable>();
        if (enemy != null) 
        {
            enemy.TakeDamage(damage);
        }

        // 只有撞到「敵人本體」或是「牆壁磚塊」時，數字子彈才會消失
        Destroy(gameObject);
    }
}