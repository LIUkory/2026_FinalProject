using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TabInventoryUI : MonoBehaviour
{
    [Header("TAB 面板設定")]
    public GameObject tabPanel;

    [Header("生成清單用的設定")]
    public Transform contentParent;     // 列表的父物件 (用來排版)
    public GameObject buffUIPrefab;     // 單一個 Buff 顯示的 Prefab 藍圖

    void Start()
    {
        tabPanel.SetActive(false);
    }

    void Update()
    {
        // 按下 TAB 開啟，放開 TAB 關閉 (經典的遊戲資訊板設計)
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            OpenTabMenu();
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            tabPanel.SetActive(false);
        }
    }

    private void OpenTabMenu()
    {
        tabPanel.SetActive(true);

        // 1. 先清空上一次打開時生成的舊格子
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // 2. 去狐狸身上抓取「已經買到的 Buff 清單」
        var myBuffs = PlayerBuffManager.instance.acquiredBuffs;

        // 3. 迴圈生成 UI
        foreach (BuffData buff in myBuffs)
        {
            // 生成一個格子
            GameObject newSlot = Instantiate(buffUIPrefab, contentParent);

            // 替換格子裡的圖片跟文字 (假設你的 Prefab 底下有 Image 跟 TextMeshPro)
            Image icon = newSlot.transform.Find("Icon").GetComponent<Image>();
            TextMeshProUGUI title = newSlot.transform.Find("Title").GetComponent<TextMeshProUGUI>();

            if (icon != null) icon.sprite = buff.buffIcon;
            if (title != null) title.text = buff.buffName;
        }
    }
}
