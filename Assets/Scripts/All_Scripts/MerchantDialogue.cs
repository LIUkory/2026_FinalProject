using UnityEngine;
using TMPro; // 🌟 這是控制 TextMeshPro 必備的魔法咒語！

public class MerchantDialogue : MonoBehaviour
{
    public TMP_Text textDisplay; // 用來綁定畫面上的文字框
    public string[] lines; // 存放你自製台詞的清單 (陣列)
    
    private int currentLine = 0; // 記住現在講到第幾句

    void Start()
    {
        // 遊戲一開始，先顯示第一句話 (如果清單裡有東西的話)
        if (lines.Length > 0)
        {
            textDisplay.text = lines[0];
        }
    }

    // 這是一個準備給隱形按鈕觸發的功能
    public void NextLine()
    {
        // 每按一次，句子編號就加 1
        currentLine = currentLine + 1;

        // 如果編號超過了台詞的總數量 (代表講完了)
        if (currentLine >= lines.Length)
        {
            currentLine = 0; // 歸零，從第一句重新開始循環！
        }

        // 把畫面上的文字換成新的這一句
        textDisplay.text = lines[currentLine];
    }
}