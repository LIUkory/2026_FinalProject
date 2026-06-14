using UnityEngine;

public abstract class Basic_skill : MonoBehaviour
{
    [Header("技能基本資料")]
    public string skillName = "未命名技能";
    public Sprite skillIcon;       // 專屬技能圖示
    public float cooldownTime = 5f;

    // ★ 關鍵新增：讓每個技能可以自己決定要綁定哪個按鍵！預設給它 R 鍵
    public KeyCode skillKey = KeyCode.R;

    protected float currentCooldown = 0f;
    public bool IsReady => currentCooldown <= 0f;

    protected virtual void Update()
    {
        // 1. 自動計算冷卻時間
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
        }

        // 2. ★ 移到這裡：技能自己監聽專屬的按鍵！
        if (Input.GetKeyDown(skillKey))
        {
            TryActivateSkill();
        }
    }

    public float GetCooldownRatio()
    {
        if (cooldownTime == 0) return 0;
        return currentCooldown / cooldownTime;
    }

    public bool TryActivateSkill()
    {
        if (IsReady)
        {
            ActivateSkillLogic();
            currentCooldown = cooldownTime;
            return true;
        }
        return false;
    }

    protected abstract void ActivateSkillLogic();
}
