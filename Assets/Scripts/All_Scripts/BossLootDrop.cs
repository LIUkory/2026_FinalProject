using UnityEngine;

public class BossLootDrop : MonoBehaviour
{
    private EnemyAI myAI;         // 抓取隊友寫的 AI 系統
    private float maxHealth;      // 記憶這隻 Boss 最多有幾滴血
    private bool isDead = false;  // 記憶 Boss 死掉了沒
[Header("噴寶設定")]
    public float scatterRadius = 8f; // 寶物噴發的範圍，數字越大散越開！
    void Start()
    {
        myAI = GetComponent<EnemyAI>(); // 取得這隻雞身上的 AI 元件
        
        if (myAI != null)
        {
            maxHealth = myAI.health; // 紀錄一開局的最大血量
        }

        // 🌟 關鍵 1：出生時，呼叫血條總管「亮起來！」
        if (BossUIController.Instance != null)
        {
            BossUIController.Instance.ActivateBossUI("地牢商雞：暴走模式");
        }
    }

    void Update()
    {
        // 如果沒有 AI，或者已經死掉了，就什麼都不做
        if (myAI == null || isDead) return;

        // 🌟 關鍵 2：戰鬥中，無時無刻把當前的血量傳給血條總管！
        if (BossUIController.Instance != null)
        {
            BossUIController.Instance.UpdateHealth(myAI.health, maxHealth);
        }

        // 🌟 關鍵 3：當血量歸零 (死掉)
        if (myAI.health <= 0)
        {
            isDead = true; // 標記為死亡，避免重複執行
            
            // 呼叫血條總管「把血條關掉！」
            if (BossUIController.Instance != null)
            {
                BossUIController.Instance.DeactivateBossUI();
            }

            SpawnLoot(); // 執行噴寶物
        }
    }

    // 噴寶物的動作 (這段是你原本就有的邏輯)
    void SpawnLoot()
    {
        for (int i = 0; i < BusinessChicken.bossLoot.Count; i++)
        {
            GameObject dropItem = Instantiate(BusinessChicken.bossLoot[i]);
            float randomX = Random.Range(-2f, 2f);
            float randomY = Random.Range(-2f, 2f);
            dropItem.transform.position = transform.position + new Vector3(randomX, randomY, 0);
        }
        Destroy(gameObject); // 寶物噴完，Boss 屍體消失
    }
}