using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI; // 🌟 為了使用 Slider 必須加這行
using System.Collections;
[System.Serializable]
public class ShopItem
{
    public string itemName;      
    public string description;   
    public int price;            
    public Sprite itemIcon;      
    public GameObject itemPrefab; 
}

public class ShopManager : MonoBehaviour
{
    public static List<ShopItem> boughtItems = new List<ShopItem>();

    [Header("UI 綁定區")]
    public GameObject infoPanel; 
    public TMP_Text nameText;    
    public TMP_Text descText;    
    public TMP_Text priceText;   
    public GameObject confirmPanel; 
    public TMP_Text moneyText; // 🌟 新增這行：用來綁定你的金幣文字框
    [Header("殺價系統")]
    public Slider bargainSlider;        // 綁定拉桿
    public TMP_Text bargainPercentText; // 綁定顯示趴數的文字
    public Button bargainButton;        // 綁定砍價按鈕
    public TMP_Text dialogueText;       // 綁定老闆講話的對話框 (猶豫就會敗北那個)

    private int failCount = 0;
    private bool hasBargained = false;
    public static float currentDiscount = 1.0f; // 1.0 就是沒打折，0.8 就是 8 折

    [Header("商品與貨架設定")]
    public ShopItem[] allItemsDatabase; 
    public ShopSlot[] allSlots;         

    void Start()
    {
        infoPanel.SetActive(false); 
        if (confirmPanel != null) confirmPanel.SetActive(false); 

        // 每次進商店，重置殺價狀態
        failCount = 0;
        hasBargained = false;
        currentDiscount = 1.0f;
        UpdateBargainText(); // 更新一次文字
        dialogueText.text = "猶豫就會敗北！"; // 初始台詞

        RollRandomItems(); 
        UpdateMoneyDisplay(); // 🌟 新增這行：商店一開門就更新金幣顯示！
    }
    public void UpdateMoneyDisplay()
    {
        if (moneyText != null && FurryBank_manager.instance != null)
        {
            moneyText.text = ": " + FurryBank_manager.instance.currentData.totalMoney;
        }
    }
    // 🌟 隨著玩家拉動拉桿，更新文字顯示
    public void UpdateBargainText()
    {
        int percent = (int)bargainSlider.value * 10;
        bargainPercentText.text = "要求打折: " + percent + "%";
    }

    // 🌟 按下殺價按鈕後執行的功能
   // 🌟 按下殺價按鈕後執行的功能
    // 🌟 尋找這個功能，並把內部全部替換成這樣：
    public void AttemptBargain()
    {
        if (hasBargained) return;

        int percent = (int)bargainSlider.value * 10;
        
        // 1. 基礎機率直接砍半 (方法二)
        int successChance = (100 - percent) / 2; 

        // 2. 老闆不耐煩機制：每失敗一次，下次成功率直接扣 10%
        successChance -= (failCount * 10);

        // 防呆：機率扣到變負數的話，當作 0%
        if (successChance < 0) 
        {
            successChance = 0; 
        }

        // 3. 奇蹟保底：只要玩家拉到 100% 想要白嫖，永遠強制給他 1% 的夢幻成功率！
        if (percent == 100)
        {
            successChance = 1; 
        }

        int roll = Random.Range(1, 101); 

        if (roll <= successChance)
        {
            hasBargained = true;
            currentDiscount = 1.0f - (percent / 100f);
            
            if (percent == 100)
            {
                dialogueText.text = "天啊...我居然被你感動了！？全部免費拿去吧！";
            }
            else
            {
                dialogueText.text = "算你狠...就當交個朋友，全部商品打 " + (100 - percent)/10 + " 折！";
            }
            
            bargainButton.interactable = false;
            bargainSlider.interactable = false;
        }
        else
        {
            failCount++;
            if (failCount == 1) dialogueText.text = "我也需要養雞糊口的";
            else if (failCount == 2) dialogueText.text = "這樣有點太過份了.";
            else if (failCount == 3) dialogueText.text = "你是不是乞丐?!";
            else if (failCount == 4) dialogueText.text = "這樣砍價是不是沒被砍過?"; 
            else if (failCount >= 5)
            {
                dialogueText.text = "...聽不懂道理，我也懂億點拳法";
                bargainButton.interactable = false;
                bargainSlider.interactable = false;
                
                BusinessChicken.isHostile = true;

                BusinessChicken.bossLoot.Clear();
                foreach (ShopItem item in allItemsDatabase)
                {
                    if (item.itemPrefab != null) BusinessChicken.bossLoot.Add(item.itemPrefab);
                }

                boughtItems.Clear();
                StartCoroutine(KickOutRoutine()); 
            }
        }
    }

    private IEnumerator KickOutRoutine()
    {
        yield return new WaitForSecondsRealtime(1.5f); 
        GoBackToDungeon(); 
    }

    // 顯示商品資訊 (先不動它)
    public void ShowItemInfo(ShopItem item)
    {
        infoPanel.SetActive(true);
        nameText.text = item.itemName;
        descText.text = item.description; 

        // 🌟 核心改動：即時計算這個商品打折後的價格
        int finalPrice = Mathf.RoundToInt(item.price * currentDiscount);

        // 如果目前有成功打折 (折扣小於 1.0)
        if (currentDiscount < 1.0f)
        {
            // 使用 TextMeshPro 的彩色語法：把原價畫上刪除線(<s>)並變紅，新價格變成綠色！
            priceText.text = "價格: <color=#FF5555><s>" + item.price + "</s></color> -> <color=#55FF55>" + finalPrice ;
        }
        else
        {
            // 沒打折就顯示原本的顏色
            priceText.text = "價格: " + item.price ;
        }
    }

    public void HideItemInfo()
    {
        infoPanel.SetActive(false);
    }
    public void RollRandomItems()
    {
        foreach (ShopSlot slot in allSlots)
        {
            int randomIndex = Random.Range(0, allItemsDatabase.Length);
            slot.SetupSlot(allItemsDatabase[randomIndex]);
        }
    }
    public void AskToLeave() { confirmPanel.SetActive(true); }
    public void CancelLeave() { confirmPanel.SetActive(false); }

   // 🚪 離開商店時的判斷：是和平離開還是進入戰鬥？
    public void GoBackToDungeon()
    {
        Time.timeScale = 1f; 

        BusinessChicken chicken = FindFirstObjectByType<BusinessChicken>();
        if (chicken != null)
        {
            if (BusinessChicken.isHostile)
            {
                // 🌟 呼叫新的召喚術！
                chicken.SummonBossAndVanish();
            }
            else
            {
                chicken.DropItemsAndLeave();
            }
        }

        SceneManager.UnloadSceneAsync("ShopScene"); 
    }
}