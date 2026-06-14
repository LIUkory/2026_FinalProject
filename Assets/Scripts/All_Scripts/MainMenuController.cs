using UnityEngine;
using UnityEngine.SceneManagement; // 負責切換場景

public class MainMenuController : MonoBehaviour
{
    // 這個函式給「開始遊戲」按鈕用
    public void StartGame()
    {
        // 括號內填入你遊戲關卡場景的名稱，例如 "GameScene" 或 "0"
        SceneManager.LoadScene("Lobby");
    }

    // 這個函式可以給「離開遊戲」按鈕用
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("已退出遊戲");
    }
}