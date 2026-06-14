using Unity.Cinemachine;
using UnityEngine;

public class Dungeon_cameraFollow : MonoBehaviour
{
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // 2. 抓取自己身上的 CinemachineCamera 組件
            if (TryGetComponent(out CinemachineCamera vcam))
            {
                // 3. 把相機的跟隨目標，強行綁定在玩家身上
                vcam.Follow = player.transform;

                Debug.Log($"【地牢相機】成功偵測到玩家 {player.name}！已自動鎖定視角。");
            }
            else
            {
                Debug.LogError("【地牢相機】出事了！這個腳本必須掛在 Cinemachine 2D Camera 物件上！");
            }
        }
        else
        {
            Debug.LogWarning("【地牢相機】畫面上找不到任何帶有 Player 標籤的物件！");
        }
    }
}
