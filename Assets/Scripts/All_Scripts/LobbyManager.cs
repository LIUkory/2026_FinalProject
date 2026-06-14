using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [Header("攝影機設定")]
    public GameObject vcamGlobal; // 放入全局攝影機
    public GameObject vcamFollow; // 放入跟隨攝影機

    [Header("系統設定")]
    public Transform cameraTarget; // ★ 把剛剛建的 CameraTarget 拖進來！

    private Player_movement selectedCharacter; // 暫時選中的角色
    private int lobbyState = 0; // 0=全景選角, 1=檢視模式(放大), 2=確認遊玩

    void Start()
    {
        // 1. 一開始顯示全景
        vcamGlobal.SetActive(true);
        vcamFollow.SetActive(false);

        // 2. 搜出大廳裡【所有】會動的角色，強迫牠們全部罰站 (關閉移動)
        Player_movement[] allCharacters = FindObjectsByType<Player_movement>(FindObjectsSortMode.None);
        foreach (var character in allCharacters)
        {
            character.enabled = false;
            character.gameObject.tag = "Untagged"; // 清除所有主角標籤
        }

        Debug.Log("【大廳階段 0】請用滑鼠左鍵點擊角色進行預覽！");
    }

    void Update()
    {
        if (lobbyState == 0 && Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 🟢 改用 RaycastAll：射出一道穿透雷射，抓出滑鼠底下的【所有】碰撞體！
            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);

            // 走訪這道雷射穿透的所有物件
            foreach (RaycastHit2D hit in hits)
            {
            if (hit.collider != null)
            {
                Player_movement clickedCharacter = hit.collider.GetComponent<Player_movement>();

                    // 只要這群被穿透的物件裡，有一個帶有角色腳本，就代表點對了！
                if (clickedCharacter != null)
                {
                        selectedCharacter = clickedCharacter;

                    cameraTarget.SetParent(selectedCharacter.transform);
                    cameraTarget.localPosition = Vector3.zero;

                    vcamGlobal.SetActive(false);
                    vcamFollow.SetActive(true);

                    lobbyState = 1;
                        Debug.Log($"【大廳階段 1】已檢視 {selectedCharacter.name}。按 [Enter] 確認選擇，按 [ESC] 取消。");

                        break; // 🎯 成功抓到主角了，立刻跳出迴圈，不用再檢查後面的路人物件！
                }
            }
        }
        }
        else if (lobbyState == 1)
        {
            // 按下 ESC：取消選擇，退回全景
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                vcamFollow.SetActive(false);
                vcamGlobal.SetActive(true);

                selectedCharacter = null;
                cameraTarget.SetParent(null); // 解除攝影機綁定

                lobbyState = 0;
                Debug.Log("【大廳階段 0】已取消選擇，退回全景畫面。");
            }
            // 按下 Enter：確認選擇，開始遊戲！
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                selectedCharacter.enabled = true; // 解鎖移動能力！
                selectedCharacter.gameObject.tag = "Player"; // 貼上主角標籤，讓怪物跟武器能認出牠
                //
                Player_movement[] allCharacters = FindObjectsByType<Player_movement>(FindObjectsSortMode.None);
                foreach (var character in allCharacters)
                {
                    if (character == selectedCharacter)
                    {
                        // 發放護照！讓這隻主角不會被傳送門摧毀
                        DontDestroyOnLoad(character.gameObject);
                    }
                    else
                    {
                        // 落選者無情抹除，確保世界上只有一個主角
                        Destroy(character.gameObject);
                    }
                }
                //
                lobbyState = 2;
                Debug.Log($"【大廳階段 2】選擇鎖定！現在開始操控 {selectedCharacter.name}！");
            }
        }
    }
}
