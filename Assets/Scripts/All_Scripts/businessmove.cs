using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;

public class BusinessChicken : MonoBehaviour
{
    private bool isFoxNearby = false;
    public static bool hasVisitedShop = false;
    public static bool isHostile = false; 
    public static List<GameObject> bossLoot = new List<GameObject>(); 

    [Header("戰鬥召喚設定")]
    public GameObject bossChickenPrefab; // 🌟 用來放你剛剛做好的 Boss 雞

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetMemoryForNewGame()
    {
        hasVisitedShop = false;
        isHostile = false;
        bossLoot.Clear();
        if (ShopManager.boughtItems != null) ShopManager.boughtItems.Clear();
    }

    void Update()
    {
        if (isFoxNearby == true && Input.GetKeyDown(KeyCode.E) && !hasVisitedShop)
        {
            hasVisitedShop = true;
            SceneManager.LoadScene("ShopScene", LoadSceneMode.Additive);
            Time.timeScale = 0f; 
        }
    }

    // 🌟 終極召喚術：原地生出 Boss，然後 NPC 消失！
    public void SummonBossAndVanish()
    {
        if (bossChickenPrefab != null)
        {
            Instantiate(bossChickenPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("店長！你忘記把 BossChicken 放進格子裡了！");
        }
        
        Destroy(gameObject); 
    }

    // 🕊️ 和平離開技能
    public void DropItemsAndLeave()
    {
        if (ShopManager.boughtItems != null && ShopManager.boughtItems.Count > 0)
        {
            for (int i = 0; i < ShopManager.boughtItems.Count; i++)
            {
                ShopItem item = ShopManager.boughtItems[i];
                if (item.itemPrefab != null)
                {
                    GameObject dropItem = Instantiate(item.itemPrefab);
                    float xOffset = (i + 1) * 3.5f; 
                    float yOffset = Random.Range(-1.6f, 1.6f); 
                    dropItem.transform.position = this.transform.position + new Vector3(xOffset, yOffset, 0);
                }
            }
            ShopManager.boughtItems.Clear();
        }
        if (hasVisitedShop == true) Destroy(gameObject); 
    }

    private void OnTriggerEnter2D(Collider2D other) { if (other.CompareTag("Player")) isFoxNearby = true; }
    private void OnTriggerExit2D(Collider2D other) { if (other.CompareTag("Player")) isFoxNearby = false; }
}