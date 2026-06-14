using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class BuffUISlot
{
    public GameObject slotObj;           // 整個選項的外框
    public Image iconImage;              // 技能圖片
    public TextMeshProUGUI titleText;    // 技能標題
    public TextMeshProUGUI descText;     // 技能描述
    public TextMeshProUGUI priceText;    // 價錢文字
    public Button buyButton;             // 購買按鈕

    [HideInInspector] public BuffData currentAssignedBuff; // 記錄目前這個格子抽到什麼技能
}

public class BountyShopUI : MonoBehaviour
{
    [Header("商店面板")]
    public GameObject shopPanel;

    [Header("技能池 (把你所有做好的 BuffData 拖進來)")]
    public List<BuffData> allAvailableBuffs;

    [Header("畫面上的三個選項")]
    public BuffUISlot[] buffSlots = new BuffUISlot[3];

    void Start()
    {
        shopPanel.SetActive(false); // 遊戲開始時隱藏商店
    }

    // 由 BountyBoard (懸賞板) 呼叫這個來開門
    public void OpenAndRollBuffs()
    {
        shopPanel.SetActive(true);
        RefreshShop(); // 刷新三個隨機選項
    }

    // 關閉商店
    public void CloseShop()
    {
        shopPanel.SetActive(false);
        Time.timeScale = 1f; // 恢復時間流動 (解除凍結)
    }

    // 刷新商店內容 (隨機抽 3 個 + 根據通膨計算價錢)
    public void RefreshShop()
    {
        // 簡單的洗牌演算法
        List<BuffData> shuffledBuffs = new List<BuffData>(allAvailableBuffs);
        for (int i = 0; i < shuffledBuffs.Count; i++)
        {
            BuffData temp = shuffledBuffs[i];
            int randomIndex = Random.Range(i, shuffledBuffs.Count);
            shuffledBuffs[i] = shuffledBuffs[randomIndex];
            shuffledBuffs[randomIndex] = temp;
        }

        // 取得目前的通膨倍率 (從你身上的 PlayerBuffManager 抓)
        float inflation = PlayerBuffManager.instance.GetCurrentInflationMultiplier();

        for (int i = 0; i < 3; i++)
        {
            if (i < shuffledBuffs.Count)
            {
                BuffData selectedBuff = shuffledBuffs[i];
                buffSlots[i].currentAssignedBuff = selectedBuff;

                // 更新 UI 顯示
                buffSlots[i].iconImage.sprite = selectedBuff.buffIcon;
                buffSlots[i].titleText.text = selectedBuff.buffName;
                buffSlots[i].descText.text = selectedBuff.description;

                // 💰 計算通膨後的實際價格！(基礎價錢 * 通膨倍率)
                int currentPrice = Mathf.RoundToInt(selectedBuff.basePrice * inflation);
                buffSlots[i].priceText.text = "$" + currentPrice.ToString();
            }
        }
    }

    // 當玩家按下購買按鈕時觸發 (由 Unity Button OnClick 綁定 0, 1, 2)
    public void OnBuyButtonClicked(int slotIndex)
    {
        BuffData buffToBuy = buffSlots[slotIndex].currentAssignedBuff;

        // 再次計算當前價格防呆
        float inflation = PlayerBuffManager.instance.GetCurrentInflationMultiplier();
        int currentPrice = Mathf.RoundToInt(buffToBuy.basePrice * inflation);

        // 呼叫你的 FurryBank_manager 扣錢！
        if (FurryBank_manager.instance.SpendMoney(currentPrice))
        {
            // 扣錢成功，把 Buff 塞給狐狸
            PlayerBuffManager.instance.AddBuff(buffToBuy);

            // 買完後，馬上刷新商店的價錢 (因為通膨又變貴了！)
            RefreshShop();
        }
        else
        {
            Debug.Log("【商店】錢不夠啦，再去多抓一點勞工！");
        }
    }
}