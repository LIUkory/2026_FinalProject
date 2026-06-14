using UnityEngine;
using UnityEngine.UI;

// 🎯 確保這個物件身上一定要有 UI Button 組件
[RequireComponent(typeof(Button))]
public class WeaponSlot_Gatling : MonoBehaviour
{
    // 🧠 這裡直接整合你填寫的資料欄位
    [System.Serializable]
    public class WeaponData
    {
        public string weaponName;       // 武器名稱
        [TextArea(3, 5)]
        public string description;      // 武器說明
        public string stats;            // 武器數值
        public Sprite weaponIcon;       // 武器圖片
    }

    [Header("📋 填入這把武器的圖鑑資料")]
    public WeaponData weaponData;

    [Header("🎨 畫框自己的圖片組件連連看")]
    public Image myIconImage; 

    void Start()
    {
        // 🚀 遊戲開始時，全自動把精美武器圖片換上畫框
        if (myIconImage != null && weaponData != null && weaponData.weaponIcon != null)
        {
            myIconImage.sprite = weaponData.weaponIcon;
        }

        // 🚀 綁定點擊事件
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(OnClicked);
        }
    }

    private void OnClicked()
    {
        // 🎯 【連線核心】：當點擊按鈕時，直接去呼叫掛在 Panel 上的大腦控制器！
        if (WeaponGalleryController.Instance != null)
        {
            WeaponGalleryController.Instance.DisplayWeaponDetails(weaponData.weaponName, weaponData.description, weaponData.stats, weaponData.weaponIcon);
        }
        else
        {
            Debug.LogError("❌ 找不到 WeaponGalleryController 大腦！請確保 Panel 身上有掛腳本！");
        }
    }
}