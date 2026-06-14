using UnityEngine;
using UnityEngine.UI;
public class BOSSHP : MonoBehaviour
{
    public Image hpFillImage;

    // 提供一個公開方法讓 BOSS 呼叫
    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (hpFillImage != null)
        {
            hpFillImage.fillAmount = currentHealth / maxHealth;
        }
    }
}
