using System.Collections.Generic;
using UnityEngine;

public class HR_manager : MonoBehaviour
{
    public static HR_manager instance;

    // 紀錄：<怪物名稱, 抓到的數量>
    private Dictionary<string, int> workingEnemies = new Dictionary<string, int>();

    void Awake()
    {
        if (instance == null) instance = this;
    }

    // 當敵人倒地被徵收時，呼叫這個函式
    public void RecruitEnemy(string enemyType)
    {
        if (workingEnemies.ContainsKey(enemyType))
        {
            workingEnemies[enemyType]++; // 已經有這個品種，數量 +1
        }
        else
        {
            workingEnemies.Add(enemyType, 1); // 新抓到的品種，數量設為 1
        }
        Debug.Log($"【人資部】成功徵收 {enemyType}！目前該部門有 {workingEnemies[enemyType]} 人。");
    }

    // ==========================================
    // ★ 核心魔法：關卡通關時的利息計算
    // ==========================================
    public void CalculateLevelClearInterest()
    {
        float baseInterest = 10f; // 每關的基礎利息
        float totalInterest = baseInterest;

        // 走訪每一個抓到的品種
        foreach (KeyValuePair<string, int> employee in workingEnemies)
        {
            string type = employee.Key;
            int count = employee.Value;

            float enemyValue = GetEnemyBaseValue(type); // 取得這種怪物的產值

            // ★ 邊際效益遞減公式：
            // 第 1 隻 100% 產能，第 2 隻 85% 產能，第 3 隻 72% 產能...
            // 公式：產值 * (0.85 ^ (隻數 - 1))
            for (int i = 1; i <= count; i++)
            {
                float diminishingMultiplier = Mathf.Pow(0.85f, i - 1);
                totalInterest += enemyValue * diminishingMultiplier;
            }
        }

        int finalEarnings = Mathf.RoundToInt(totalInterest); // 四捨五入成整數
        Debug.Log($"【結算】本關總利息為：{finalEarnings} 幣！");

        // 把賺到的錢直接匯進剛剛寫好的瑞士銀行！
        if (FurryBank_manager.instance != null)
        {
            FurryBank_manager.instance.AddMoney(finalEarnings);
        }

        // 結算完清空名單，為下一關做準備
        //workingEnemies.Clear();
    }

    // 這裡可以自由設定每種怪物的「基本產能」
    private float GetEnemyBaseValue(string enemyType)
    {
        switch (enemyType)
        {
            case "Rat": return 10f;     
            case "Slime": return 5f; 
            case "Shirmp": return 15f;
            case "Crab": return 20f;
            default: return 5f;
        }
    }
    public int GetCurrentExpectedInterest()
    {
        float baseInterest = 0f; // 通關基礎給的利息 (要跟之前結算寫的數字一樣)
        float totalInterest = baseInterest;

        foreach (var employee in workingEnemies)
        {
            float enemyValue = GetEnemyBaseValue(employee.Key);
            int count = employee.Value;

            for (int i = 1; i <= count; i++)
            {
                float diminishingMultiplier = Mathf.Pow(0.85f, i - 1);
                totalInterest += enemyValue * diminishingMultiplier;
            }
        }
        return Mathf.RoundToInt(totalInterest);
    }
}
