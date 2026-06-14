using UnityEngine;

public class Camera_Following : MonoBehaviour
{
    [Header("目標設定")]
    public Transform target;        // 要跟隨的目標 (把 Fox 拖進來)
    public float smoothSpeed = 5f;  // 鏡頭跟隨的滑順度 (數字越大跟得越緊)

    [Header("地圖邊界限制 (回彈區域)")]
    public Vector2 minPosition;     // 地圖左下角的極限座標
    public Vector2 maxPosition;     // 地圖右上角的極限座標

    // ★ 資工系重點：為什麼用 LateUpdate 而不是 Update？
    // 因為玩家的移動通常寫在 Update。我們必須等玩家移動「完畢」後，
    // 相機再去抓他的新位置，這樣畫面才不會產生肉眼可見的「微小抖動 (Jitter)」。
    void LateUpdate()
    {
        // 1. 如果沒有目標，就不要執行
        if (target == null) return;

        // 2. 取得目標目前的座標，但保持相機原本的 Z 軸 (通常是 -10)
        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

        // 3. ★ 核心魔法：限制座標範圍 (Clamping)
        // 確保相機的 X 和 Y 絕對不會超過我們設定的 min 和 max 邊界！
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minPosition.x, maxPosition.x);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minPosition.y, maxPosition.y);

        // 4. 平滑移動：從相機「現在的位置」平滑滑動到「目標位置」
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
    void Start()
    {
        // 遊戲一開始，相機自動去全地圖尋找身上掛有 "Player" Tag 的物件，把它當作目標！
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
    }
}
