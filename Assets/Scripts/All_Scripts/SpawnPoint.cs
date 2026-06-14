using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    void Start()
    {
        // 1. 在場景載入的瞬間，全地圖搜索帶著 "Player" 標籤的偷渡客
        GameObject incomingPlayer = GameObject.FindGameObjectWithTag("Player");

        if (incomingPlayer != null)
        {
            // 2. 把玩家瞬間移動到這個接機點的位置！
            incomingPlayer.transform.position = transform.position;
            Debug.Log($"【地牢系統】接機成功！已將 {incomingPlayer.name} 傳送至起點房間。");

            // (保險起見) 如果玩家在傳送過程中不小心被關閉了，幫他打開
            if (incomingPlayer.TryGetComponent(out Fox_movement move)) move.enabled = true;
        }
        else
        {
            // ★ 防呆警報：如果印出這行紅字，代表你大廳的 DontDestroyOnLoad 沒生效，玩家死在傳送門裡了！
            Debug.LogError("【地牢大當機】找不到帶有 Player 標籤的玩家！接機失敗！");
        }
    }
}
