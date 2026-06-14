using UnityEngine;
using UnityEngine.UI;
public class SkillUI_manager : MonoBehaviour
{
    [Header("UI 綁定")]
    public Image uiSkillIcon;

    private Basic_skill currentPlayerSkill;

    void Start()
    {
        // 遊戲一開始去抓主角
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // 抓取主角身上的技能
            currentPlayerSkill = player.GetComponent<Basic_skill>();

            // 替換成該技能的專屬圖示
            if (currentPlayerSkill != null && currentPlayerSkill.skillIcon != null)
            {
                uiSkillIcon.sprite = currentPlayerSkill.skillIcon;
            }
        }
    }

    void Update()
    {
        if (currentPlayerSkill == null) return;

        // ★ 這裡的按鍵偵測已經刪掉了！UI 現在只負責畫面的冷卻更新

        float ratio = currentPlayerSkill.GetCooldownRatio();
        uiSkillIcon.fillAmount = 1f - ratio;

        Color iconColor = uiSkillIcon.color;
        iconColor.a = currentPlayerSkill.IsReady ? 1f : 0.4f;
        uiSkillIcon.color = iconColor;
    }
}
