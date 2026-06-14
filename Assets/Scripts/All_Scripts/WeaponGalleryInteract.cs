using UnityEngine;

public class WeaponGalleryInteract : MonoBehaviour
{
    [Header("🎯 要開啟的武器庫總畫布 (Canvas)")]
    public GameObject weaponGalleryCanvas;

    private bool isPlayerInRange = false;

    void Start()
    {
        weaponGalleryCanvas.SetActive(false);
    }
    void Update()
    {
        // 如果 FOX 在範圍內，而且玩家按下了鍵盤的 'E' 鍵
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            OpenWeaponGallery();
        }
    }

    void OpenWeaponGallery()
    {
        // 1. 打開武器庫 UI
        weaponGalleryCanvas.SetActive(true);
        
        // 2. 🌟 終極時間停止魔法：讓整個遊戲的物理、動畫、移動瞬間凍結！
        Time.timeScale = 0f; 
    }

    // 當有物件踩進感應區
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("FOX 進入武器庫感應區！可以按 E 鍵。");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("FOX 進入武器庫感應區！可以按 E 鍵。");
        }
    }

    public void close()
    {
        weaponGalleryCanvas.SetActive(false);
        Time.timeScale = 1f;
    }
}