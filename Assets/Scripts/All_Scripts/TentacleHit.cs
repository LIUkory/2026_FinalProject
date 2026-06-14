using UnityEngine;

public class TentacleHit : MonoBehaviour
{
    public float damageToPlayer = 20f;
    private bool hasDealtDamage = false;
    // 因為這個腳本只會掛在尖端的 Hitbox 上，所以只有 Hitbox 撞到玩家才會觸發！
    public void ResetHitbox()
    {
        hasDealtDamage = false;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (hasDealtDamage) return;
            Fox_stats playerStats = other.GetComponent<Fox_stats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damageToPlayer);
                hasDealtDamage = true;
            }
            Debug.Log("【觸手尖端】精準命中！造成 " + damageToPlayer + " 點傷害！");
        }
    }
}
