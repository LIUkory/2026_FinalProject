using UnityEngine;
using System.IO;

[System.Serializable]
public class GameData
{
    public int totalMoney = 0; // 永久保留的總金幣
}

public class FurryBank_manager : MonoBehaviour
{
    public static FurryBank_manager instance;
    private string savePath;

    public GameData currentData;

    void Awake()
    {
        // 確保整個遊戲只有一個銀行，而且切換場景不會被摧毀
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        savePath = Application.persistentDataPath + "/bank_save.json";
        LoadMoney();
    }

    // 存錢進硬碟
    public void SaveMoney()
    {
        string json = JsonUtility.ToJson(currentData);
        File.WriteAllText(savePath, json);
        Debug.Log($"【銀行】存檔成功！總資產：{currentData.totalMoney} 金幣");
    }

    // 從硬碟領錢
    public void LoadMoney()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            currentData = JsonUtility.FromJson<GameData>(json);
        }
        else
        {
            currentData = new GameData();
        }
    }

    // 呼叫這個來加錢
    public void AddMoney(int amount)
    {
        currentData.totalMoney += amount;
        SaveMoney(); // 每次加錢立刻存檔，這樣狐狸下一秒死掉錢也在！
    }
    public bool HasEnoughMoney(int amount)
    {
        return currentData.totalMoney >= amount;
    }

    // 花錢（會自動存檔）
    public bool SpendMoney(int amount)
    {
        if (HasEnoughMoney(amount))
        {
            currentData.totalMoney -= amount;
            SaveMoney(); // 花完錢立刻存檔，防止玩家買完拔電源作弊
            Debug.Log($"【銀行】支出 {amount} 元，剩餘：{currentData.totalMoney}");
            return true; // 購買成功
        }
        else
        {
            Debug.Log("【銀行】餘額不足，交易失敗！");
            return false; // 購買失敗
        }
    }
}
