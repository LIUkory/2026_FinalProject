using UnityEngine;
using System.Collections.Generic;
public class PlayerSkillManagement : MonoBehaviour
{
    public static PlayerSkillManagement instance;

    [Header("已擁有的增益")]
    public List<BuffData> acquiredBuffs = new List<BuffData>(); // TAB 鍵要顯示的清單就看這裡

    [Header("通膨設定")]
    public float inflationRate = 0.5f; // 每買一個，後面的技能就變貴 50%

    // --- 假設這裡有狐狸的能力值腳本引用 ---
    private Fox_stats Fox_Stats;
    private Player_movement Fox_movement; // 假設你的移動腳本叫這個名字

    void Awake()
    {
        if (instance == null) instance = this;
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            Fox_Stats = playerObj.GetComponent<Fox_stats>();
            Fox_movement = playerObj.GetComponent<Fox_movement>();
        }
    }

    // 取得當前通膨倍率 (買 0 個 = 1倍，買 1 個 = 1.5倍，買 2 個 = 2倍...)
    public float GetCurrentInflationMultiplier()
    {
        return 1f + (acquiredBuffs.Count * inflationRate);
    }

    // 購買並獲得 Buff 的核心方法
    public void AddBuff(BuffData newBuff)
    {
        // 1. 加進背包清單 (讓 TAB 介面可以讀取)
        acquiredBuffs.Add(newBuff);

        Debug.Log($"【肉鴿系統】獲得增益：{newBuff.buffName}！目前已擁有 {acquiredBuffs.Count} 個增益。");

        // 2. 套用能力值 (把 Buff 裡面的數值加到玩家身上)
        ApplyBuffEffects(newBuff);
    }

    // ==========================================
    // ★ 核心魔法：把買到的 Buff 數值真實加上去！
    // ==========================================
    private void ApplyBuffEffects(BuffData buff)
    {
        if (Fox_Stats != null)
        {
            // 1. 套用最大血量
            if (buff.addMaxHealth > 0)
            {
                Fox_Stats.maxHealth += buff.addMaxHealth;
                // 順便把現在的血量也補滿，不然血條看起來會空一塊
                Fox_Stats.currentHealth += buff.addMaxHealth;
            }

            // 2. 套用最大護盾
            if (buff.addMaxShield > 0)
            {
                Fox_Stats.maxShield += buff.addMaxShield;
                Fox_Stats.currentShield += buff.addMaxShield;
            }

            // ⚠️ 關鍵：因為我們改了血量跟護盾的上限，必須強迫 UI 重新整理！
            // (我們利用了你寫的 Heal 函式，傳入 0 滴血，藉此觸發裡面的 UpdateUI() 邏輯)
            Fox_Stats.Heal(0);
        }

        if (Fox_movement != null)
        {
            // 3. 套用移動速度
            if (buff.addMoveSpeed > 0)
            {
                // 假設你的 Player_movement 裡面有個控制速度的公開變數叫 moveSpeed
                Fox_movement.moveSpeed += buff.addMoveSpeed;
            }
        }

        Debug.Log($"【肉鴿系統】已成功套用：{buff.buffName} 的能力加成！");
    }
}
