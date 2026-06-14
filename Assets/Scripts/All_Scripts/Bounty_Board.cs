using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    private bool isPlayerNear = false;

    // 已經把 public BountyShopUI shopUI; 刪除了，因為我們現在要全自動尋找！

    void Update()
    {
        // 玩家在附近，且按下 E 鍵，且遊戲沒有暫停 (Time.timeScale > 0 代表時間正常流動，防呆避免重複狂按)
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E) && Time.timeScale > 0f)
        {
            OpenBountyShop();
        }
    }

    private void OpenBountyShop()
    {
        // ★ 核心魔法：全地圖搜索器！自動尋找隱藏的商店 UI
        BountyShopUI shopUI = Object.FindFirstObjectByType<BountyShopUI>(FindObjectsInactive.Include);

        if (shopUI != null)
        {
            // 1. 暫停時間 / 停止玩家行動
            Time.timeScale = 0f;
            Debug.Log("【懸賞板】找到商店了！打開肉鴿商店，玩家已凍結！");

            // 2. 啟動物件，並呼叫生成技能的函式
            shopUI.gameObject.SetActive(true);
            shopUI.OpenAndRollBuffs();
        }
        else
        {
            Debug.LogError("【懸賞板】出事了！場景裡面找不到 BountyShopUI！");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = true;
            Debug.Log("【懸賞板】按 E 開啟懸賞板");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }
}
