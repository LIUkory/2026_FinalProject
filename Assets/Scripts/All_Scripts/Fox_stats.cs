using UnityEngine;
using UnityEngine.SceneManagement;

public class Fox_stats : MonoBehaviour
{
    [Header("UI 綁定")]
    public Hp_UI healthUI; // ★ 在這裡宣告 UI 管理器

    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Shield Settings")]
    public float maxShield = 50f;
    public float currentShield;
    public float shieldRegenRate = 10f; // 每秒回復量
    public float regenDelay = 3f;       // 受傷後延遲幾秒開始回盾

    private float lastDamageTime;
    private bool isDead = false;
    public GameObject deathEffectPrefab;
    void Start()
    {
        currentHealth = maxHealth;
        currentShield = maxShield;

        // ★ 遊戲開始時，先更新一次 UI 讓雙條全滿
        UpdateUI();
    }

    void Update()
    {
        // 護盾自動回復邏輯
        if (Time.time - lastDamageTime > regenDelay && currentShield < maxShield)
        {
            currentShield += shieldRegenRate * Time.deltaTime;
            currentShield = Mathf.Min(currentShield, maxShield); // 確保不超過上限

            // ★ 回盾的過程中，每一幀都要更新 UI
            UpdateUI();
        }
    }

    // 受到傷害函式
    public void TakeDamage(float amount)
    {
        // ★ 核心除錯：加上這行守門員！如果已經死了，直接踢回，不准再往下執行！
        if (isDead) return;
        Fox_skill skillScript = GetComponent<Fox_skill>();
        lastDamageTime = Time.time;
        if (skillScript != null && skillScript.isInvincible)
        {
            Debug.Log("【翻滾無敵】閃避成功，不扣護盾也不扣血！");
            return; // 直接踢回，不再往下執行扣血邏輯
        }
        // ★ 核心修改：只有在「最大護盾大於 0」且「目前護盾大於 0」時，才計算護盾傷害
        if (maxShield > 0 && currentShield > 0)
        {
            currentShield -= amount;
            if (currentShield < 0)
            {
                currentHealth += currentShield; // 計算溢出傷害
                currentShield = 0;
            }
        }
        else // 如果沒有護盾，或是護盾已經破了，直接扣血
        {
            currentHealth -= amount;
        }

        UpdateUI();

        if (currentHealth <= 0) Die();
    }

    // 補血函式 (給補包呼叫)
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        // ★ 補血後更新 UI
        UpdateUI();
    }

    // 為了讓程式碼更簡潔，我們寫一個專門更新 UI 的小函式
    private void UpdateUI()
    {
        if (healthUI != null)
        {
            healthUI.UpdateStatsUI(currentHealth, maxHealth, currentShield, maxShield);
        }
    }

    void Die()
    {
        // 1. 宣佈死亡：打上死亡證明，避免重複觸發
         isDead = true;
        Debug.Log("玩家死亡！");

        // 2. 處理死亡特效 (生成並設定 1.5 秒後自動毀滅！)
        if (deathEffectPrefab != null)
        {
            // 把生成的煙霧存進變數 effect 裡
            GameObject effect = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

            // ★ 強制刪除：告訴 Unity 1.5 秒後把這團煙霧刪掉，絕不讓它輪迴播放！
            // (1.5f 的數字你可以根據你的動畫長度自由微調)
            Destroy(effect, 3.0f);
        }

        // 3. 處理狐狸本體 (拔掉標籤、關閉物理與渲染)
        gameObject.tag = "Untagged"; // ★ 拔掉標籤！老鼠的 AI 瞬間失去目標

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        // ★ 如果狐狸身上有 Rigidbody2D，讓它關閉模擬，避免死掉後還被推來推去
        if (GetComponent<Rigidbody2D>() != null)
        {
            GetComponent<Rigidbody2D>().simulated = false;
        }

        Transform weaponHolder = transform.Find("WeaponHolder");

        if (weaponHolder != null)
        {
            // 把 WeaponHolder 設為 false，裡面的槍就會消失，也不能開火了！
            weaponHolder.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("找不到 WeaponHolder，請確認名字有沒有拼錯！");
        }
        // 這裡可以加上延遲重新載入場景的語法 (準備下一階段)
        Invoke("RestartGame", 3f); 
    }

    void RestartGame()
    {
        // 重新載入目前所在的場景 (一切歸零重來)
        SceneManager.LoadScene("main menu");
    }
}