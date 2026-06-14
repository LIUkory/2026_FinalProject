using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WeaponGalleryController : MonoBehaviour
{
    public static WeaponGalleryController Instance;

    [Header("🚪 獨立介紹畫面面板（請把新創的單獨面板拉進來）")]
    public GameObject detailIntroPanel; 

    [Header("🎨 單獨畫面內部的 UI 組件對接")]
    public TextMeshProUGUI detailNameText;
    public TextMeshProUGUI detailDescText;
    public TextMeshProUGUI detailStatsText;
    public Image detailBigImage;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 🚀 遊戲剛開始時，確保這個單獨介紹畫面是藏起來的
        if (detailIntroPanel != null)
        {
            detailIntroPanel.SetActive(false);
        }
    }

    // 🎯 【接收端】：當玩家點擊任何畫框時觸發
    public void DisplayWeaponDetails(string wName, string wDesc, string wStats, Sprite wIcon)
    {
        // 1. 先把資料完美注入單獨畫面的文字和圖片裡
        if (detailNameText != null) detailNameText.text = wName;
        if (detailDescText != null) detailDescText.text = wDesc;
        if (detailStatsText != null) detailStatsText.text = wStats;
        
        if (detailBigImage != null && wIcon != null)
        {
            detailBigImage.sprite = wIcon;
            detailBigImage.enabled = true;
        }
        
        // 2. 啪一聲！帥氣地把單獨介紹畫面亮開！
        if (detailIntroPanel != null)
        {
            detailIntroPanel.SetActive(true);
        }

        Debug.Log($"🎒 武器庫大腦：已開啟【{wName}】的單獨介紹畫面！");
    }

    // 🔙 【返回功能】：點擊單獨畫面裡的 ❌ 返回鈕時呼叫
    public void CloseDetailIntro()
    {
        if (detailIntroPanel != null)
        {
            detailIntroPanel.SetActive(false); // 關閉單獨畫面，退回畫框列表
        }
    }
}