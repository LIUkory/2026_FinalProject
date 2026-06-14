using UnityEngine;
using UnityEngine.UI; // 控制 UI 圖片需要的工具
using TMPro;          // 控制文字需要的工具

public class BossUIController : MonoBehaviour
{
    // 這是一個「單例 (Singleton)」，讓全遊戲的怪物都能瞬間找到這個血條總管
    public static BossUIController Instance;

    [Header("把 UI 元件拉進這裡")]
    public GameObject rootObject; // 放總開關 (BossHealthUI_Root)
    public Image healthFill;      // 放紅色的血 (BossHealthFill)
    public TMP_Text nameText;     // 放 Boss 名字的文字 (沒有就不放)

    void Awake()
    {
        Instance = this;             // 遊戲一開始，把自己註冊為總管
        rootObject.SetActive(false); // 第一時間先把整個血條隱藏起來！
    }

    // 🌟 技能一：開啟血條 (Boss 變身時會呼叫這個)
    public void ActivateBossUI(string bossName)
    {
        rootObject.SetActive(true); // 把 UI 總開關打開
        
        if(nameText != null) 
        {
            nameText.text = bossName; // 幫血條上面的文字改名
        }
        
        healthFill.fillAmount = 1f; // 讓紅血條變成 100% 滿的
    }

    // 🌟 技能二：更新血量 (Boss 被砍的時候會呼叫這個)
    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        // 算出百分比 (例如 50 / 100 = 0.5)，UI 的 Fill Amount 就會變成 0.5 (剩一半)
        healthFill.fillAmount = currentHealth / maxHealth;
    }

    // 🌟 技能三：關閉血條 (Boss 死掉時會呼叫這個)
    public void DeactivateBossUI()
    {
        rootObject.SetActive(false); // 再次把 UI 總開關關掉
    }
}