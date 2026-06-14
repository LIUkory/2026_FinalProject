using System.Collections.Generic;
using UnityEngine;

public class PlayerBuffManager : MonoBehaviour
{
    public static PlayerBuffManager instance;
    private Fox_stats foxStats;
    private Fox_movement foxMovement;
    [Header("已擁有的增益")]
    public List<BuffData> acquiredBuffs = new List<BuffData>(); // TAB 鍵要顯示的清單就看這裡

    [Header("通膨設定")]
    public float inflationRate = 0.5f; // 每買一個，後面的技能就變貴 50%

    // --- 假設這裡有狐狸的能力值腳本引用 ---
    // public Fox_stats foxStats; 

    void Awake()
    {
        if (instance == null) instance = this;
        foxStats = GetComponent<Fox_stats>();
        foxMovement = GetComponent<Fox_movement>();
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

    // 實際把能力加到狐狸身上的邏輯
    private void ApplyBuffEffects(BuffData buff)
    {
        // 檢查有沒有抓到血量腳本
        if (foxStats != null)
        {
            if (buff.addMaxHealth > 0)
            {
                foxStats.maxHealth += buff.addMaxHealth;
                foxStats.currentHealth += buff.addMaxHealth;
            }
            if (buff.addMaxShield > 0)
            {
                foxStats.maxShield += buff.addMaxShield;
                foxStats.currentShield += buff.addMaxShield;
            }
            foxStats.Heal(0); // 更新 UI

            // ★ 把成功的訊息移到這裡，並且印出「真正的」血量上限！
            Debug.Log($"【肉鴿系統】加血成功！狐狸目前的 MaxHealth 變成：{foxStats.maxHealth}");
        }
        else
        {
            // ★ 防呆警報器！如果沒抓到腳本，印出超大紅字！
            Debug.LogError("【肉鴿系統大當機】抓不到 Fox_stats！加血失敗！請檢查 PlayerBuffManager 是不是掛錯物件了！");
        }

        // --- 跑速同理 ---
        if (foxMovement != null)
        {
            if (buff.addMoveSpeed > 0)
            {
                foxMovement.moveSpeed += buff.addMoveSpeed;
                Debug.Log($"【肉鴿系統】加速成功！目前的跑速變成：{foxMovement.moveSpeed}");
            }
        }
        else if (buff.addMoveSpeed > 0)
        {
            Debug.LogError("【肉鴿系統大當機】抓不到 Fox_movement！加速失敗！");
        }
    }
}
