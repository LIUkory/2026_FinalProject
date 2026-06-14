using UnityEngine;
using TMPro; // 🎯 引入 TextMeshPro 命名空間

public class CardProjectile : MonoBehaviour
{
    public float flySpeed = 12f;
    public float lifeTime = 2f;
    
    [Header("UI 對接")]
    [Tooltip("請把子彈物件地底下的 TextMeshPro - Text 拖進來")]
    public TextMeshPro textComponent; 

    [HideInInspector]
    public float damage; 

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // 撲克牌直線飛射
        transform.Translate(Vector2.right * flySpeed * Time.deltaTime);
    }

    // 🎯 由武器腳本呼叫：初始化這張牌的威力和外觀文字
    public void SetupCard(float dmg, string visualText)
    {
        this.damage = dmg;

        // 🎨 把文字（例如 "A" 或 "7"）即時寫入畫面的字體元件中！
        if (textComponent != null)
        {
            textComponent.text = visualText;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.gameObject.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
            Debug.Log($"🎯 撲克牌傷害：{damage} 點！");
            Destroy(gameObject); 
        }
    }
}