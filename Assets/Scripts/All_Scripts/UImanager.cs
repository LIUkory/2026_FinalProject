using TMPro;
using UnityEngine;

public class UImanager : MonoBehaviour
{
    [Header("財報面板綁定")]
    public TextMeshProUGUI totalMoneyText; // 綁定總金幣的文字
    public TextMeshProUGUI interestText;   // 綁定利息的文字

    void Update()
    {
        // 1. 刷新右上角的總金幣 (從瑞士銀行抓取)
        if (FurryBank_manager.instance != null && totalMoneyText != null)
        {
            totalMoneyText.text = FurryBank_manager.instance.currentData.totalMoney.ToString();
        }

        // 2. 刷新右上角的預期利息 (從人資部抓取)
        if (HR_manager.instance != null && interestText != null)
        {
            // 基礎利息 + 打工仔貢獻的動態利息
            interestText.text = "+" + HR_manager.instance.GetCurrentExpectedInterest().ToString();
        }
    }
}
