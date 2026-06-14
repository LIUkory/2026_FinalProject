using UnityEngine;

public class InteractUI_manager : MonoBehaviour
{
    public static InteractUI_manager instance;

    [Header("提示 UI 綁定")]
    public GameObject promptObj; // 把狐狸頭上的 [E] 拖進來

    [Header("浮動動畫設定")]
    public float floatSpeed = 5f;    // 浮動的速度
    public float floatHeight = 0.2f; // 浮動的高度

    private Vector3 startLocalPos;

    void Awake()
    {
        if (instance == null) instance = this;

        if (promptObj != null)
        {
            // 紀錄 [E] 最初在狐狸頭上的位置
            startLocalPos = promptObj.transform.localPosition;
            promptObj.SetActive(false); // 遊戲一開始先隱藏
        }
    }

    void Update()
    {
        // ★ 動畫魔法：只要 [E] 是顯示狀態，就利用 Mathf.Sin 讓它上下平滑浮動！
        if (promptObj != null && promptObj.activeSelf)
        {
            float newY = startLocalPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
            promptObj.transform.localPosition = new Vector3(startLocalPos.x, newY, startLocalPos.z);
        }
    }

    // 開放給武器呼叫的方法
    public void ShowPrompt()
    {
        Debug.Log("【UI管理員】收到顯示指令！");
        if (promptObj != null) promptObj.SetActive(true);
    }

    public void HidePrompt()
    {
        if (promptObj != null) promptObj.SetActive(false);
    }
}
