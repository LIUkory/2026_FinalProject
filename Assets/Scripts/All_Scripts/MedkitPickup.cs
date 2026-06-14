using UnityEngine;

public class MedkitPickup : MonoBehaviour
{
    [Header("補品設定")]
    [Tooltip("在 Inspector 自由修改這個補品能回多少血")]
    public float healAmount = 20f;

    // 當有物體走進這個補包的觸發範圍時
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. 身分驗證：只允許身上有 "Player" 標籤的人撿起 (老鼠直接被無視)
        if (other.CompareTag("Player"))
        {
            // 2. 獲取玩家的屬性腳本
            Fox_stats playerStats = other.GetComponent<Fox_stats>();

            if (playerStats != null)
            {
                // 3. 【業界防呆設計】如果玩家已經滿血，就不讓他吃，把補包留著！
                // 這樣才不會浪費資源。如果滿血也能吃，這行判斷拿掉即可。
                if (playerStats.currentHealth < playerStats.maxHealth)
                {
                    // 呼叫玩家腳本裡早就寫好的補血功能
                    playerStats.Heal(healAmount);

                    Debug.Log($"玩家撿到了補包！回復 {healAmount} 點生命值。");

                    // 4. 銷毀補包自己 (從場景上消失)
                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log("玩家已經滿血了，補包留著等一下吃！");
                }
            }
        }
    }
}
