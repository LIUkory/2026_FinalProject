using UnityEngine;

public class Chicken_stats : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Shield Settings")]
    public float maxShield = 50f;
    public float currentShield;
    public float shieldRegenRate = 10f; // 每秒恢復多少
    public float regenDelay = 3f;      // 多久沒受傷開始恢復

    private float lastDamageTime;

    void Start()
    {
        currentHealth = maxHealth;
        currentShield = maxShield;
    }

    void Update()
    {
        // 護盾自動恢復邏輯
        if (Time.time - lastDamageTime > regenDelay && currentShield < maxShield)
        {
            currentShield += shieldRegenRate * Time.deltaTime;
            currentShield = Mathf.Min(currentShield, maxShield); // 限制不超過上限
        }
    }

    // 受到傷害的方法 (由敵人呼叫)
    public void TakeDamage(float amount)
    {
        lastDamageTime = Time.time;

        if (currentShield > 0)
        {
            currentShield -= amount;
            if (currentShield < 0) // 護盾破了剩下的扣血
            {
                currentHealth += currentShield; // 因為 currentShield 是負值
                currentShield = 0;
            }
        }
        else
        {
            currentHealth -= amount;
        }

        if (currentHealth <= 0) Die();
    }

    // 補血方法 (由補包呼叫)
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }

    void Die()
    {
        Debug.Log("雞角色死亡");
        // 在這裡寫死亡效果，例如重新開始
    }
}