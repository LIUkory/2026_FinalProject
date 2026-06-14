using UnityEngine;

public class WaterPillar : MonoBehaviour
{
    public float damageToPlayer = 30f;
    public float lifetime = 1.0f; // 水柱存活時間 (1秒後自動消失)
    private bool hasHit = false;  // 防止重複扣血的鎖

    void Start()
    {
        // 生成的瞬間，就開始倒數計時準備銷毀
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 如果撞到玩家，且這根水柱還沒造成過傷害
        if (other.CompareTag("Player") && !hasHit)
        {
            Fox_stats fox = other.GetComponent<Fox_stats>();
            if (fox != null)
            {
                fox.TakeDamage(damageToPlayer);
                hasHit = true; // 上鎖！這根水柱不會再扣血了
                Debug.Log("【天降水柱】轟！命中玩家！造成 " + damageToPlayer + " 點傷害！");
            }
        }
    }
}
