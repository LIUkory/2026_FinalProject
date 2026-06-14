using UnityEngine;

public class Weapon_manager : MonoBehaviour
{
    public Transform weaponHolder;

    // 🟢 改動 1：參數不要傳 Prefab，直接傳入「場景上那把實體的武器」
    public void SwapWeapon(GameObject groundWeapon)
    {
        // --- 1. 處理手上的舊武器：把它「丟在地上」 ---
        foreach (Transform child in weaponHolder)
        {
            child.SetParent(null);
            child.position = transform.position + new Vector3(0.5f, -0.5f, 0f);

            // ★ 新增這兩行：強制重置角度與縮放比例 ★
            child.rotation = Quaternion.identity;   // 將旋轉角度歸零 (0, 0, 0)，讓它正正地躺著
            child.localScale = Vector3.one;         // 將縮放比例重置為 (1, 1, 1)，避免因為狐狸面朝左邊而被水平反轉

            Collider2D col = child.GetComponent<Collider2D>();
            if (col != null) col.enabled = true;

            child.gameObject.tag = "WeaponItem";

            // 🟢 改動 2：呼叫 Unequip() 關閉舊武器的安全鎖，解決掉在地上的「幽靈開火」
            IWeapon oldWeaponScript = child.GetComponent<IWeapon>();
            if (oldWeaponScript != null)
            {
                oldWeaponScript.Unequip();
            }
        }

        // --- 2. 直接拿起地上的武器 ---
        // 🟢 改動 3：刪除原本的 Instantiate！直接把 groundWeapon 抓過來設定為子物件
        groundWeapon.transform.SetParent(weaponHolder);

        // ★ 核心魔法：只問它「是不是武器」，是的話就叫它「裝備」！
        IWeapon weaponScript = groundWeapon.GetComponentInChildren<IWeapon>();
        if (weaponScript != null)
        {
            weaponScript.Equip(); // 呼叫裝備！
        }
        else
        {
            Debug.LogError("這把武器身上沒有 IWeapon 腳本！請檢查！");
        }

        // 重設位置與旋轉
        groundWeapon.transform.localPosition = Vector3.zero;
        groundWeapon.transform.localRotation = Quaternion.identity;
        groundWeapon.transform.localScale = Vector3.one;

        // --- 3. 關閉手上的新武器碰撞體 ---
        Collider2D newCol = groundWeapon.GetComponent<Collider2D>();
        if (newCol != null) newCol.enabled = false;

        groundWeapon.tag = "Untagged";
    }
}
