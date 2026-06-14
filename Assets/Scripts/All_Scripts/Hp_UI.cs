using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Hp_UI : MonoBehaviour
{
    [Header("血量 UI")]
    public Image healthFill;
    public TextMeshProUGUI healthText;

    [Header("護盾 UI (可選)")]
    public Image shieldFill;
    public TextMeshProUGUI shieldText;

    public void UpdateStatsUI(float curHealth, float maxHealth, float curShield, float maxShield)
    {
        // 1. 更新血條 (必須檢查 healthFill 有沒有接上插頭)
        if (maxHealth > 0 && healthFill != null)
        {
            healthFill.fillAmount = curHealth / maxHealth;

            // 文字是可選的，有接上才更新
            if (healthText != null)
                healthText.text = curHealth.ToString("0") + " / " + maxHealth.ToString("0");
        }

        // 2. 更新護盾條 (如果沒接上護盾UI，這裡就會直接跳過，不會報錯！)
        if (maxShield > 0 && shieldFill != null)
        {
            shieldFill.fillAmount = curShield / maxShield;

            if (shieldText != null)
                shieldText.text = curShield.ToString("0") + " / " + maxShield.ToString("0");
        }
    }
}