using UnityEngine;

public class Weapon_pickup : MonoBehaviour
{
    private bool isPlayerInRange = false;
    
    // ★ 變數 weaponPrefab 已經不需要了，可以刪除！
     
   
    private void Update()
    {
        // 只有當玩家在範圍內，且按下 E 鍵時才拾取
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // 尋找場景中的玩家（確保你的 Fox 有掛 WeaponManager 腳本）
            Weapon_manager wm = GameObject.FindGameObjectWithTag("Player").GetComponent<Weapon_manager>();

            if (wm != null)
            {
                // ★ 關鍵修改 1：直接把「這把武器的實體物件本身」傳過去！
                // 如果你這個腳本是掛在一個叫 "Trigger" 的子物件上，而武器本體是父物件，就用 transform.parent.gameObject
                // 如果這個腳本是直接掛在武器最上層本體上，就改成傳入 gameObject 即可！
                wm.SwapWeapon(transform.parent.gameObject);

                // ★ 關鍵修改 2：把兩個 Destroy 刪掉！因為實體已經被移到玩家手上了！

                Debug.Log("武器已撿起！");
                if (InteractUI_manager.instance != null)
                {
                    InteractUI_manager.instance.HidePrompt();
                }
                // ★ 關鍵修改 3：重設判定，避免拿在手上後，按 E 還會重複觸發
                isPlayerInRange = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("按 E 撿起武器"); // 之後可以在這裡寫 UI 提示
            if (InteractUI_manager.instance != null)
            {
                InteractUI_manager.instance.ShowPrompt();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (InteractUI_manager.instance != null)
            {
                InteractUI_manager.instance.HidePrompt();
            }
        }
    }
}
