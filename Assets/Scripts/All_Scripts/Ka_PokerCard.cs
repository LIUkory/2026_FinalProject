using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro; // 🎯 引入文字命名空間

public class Ka_PokerCard : MonoBehaviour, IWeapon
{
    public bool isEquipped = false;

    [Header("物件與發射設定")]
    public GameObject cardPrefab;   
    public Transform firePoint;     
    public float attackRate = 0.5f; // CD 時間（建議設 0.5 秒，翻牌效果比較明顯）

    [Header("撲克牌傷害機制")]
    public float baseDamageMultiplier = 3f;

    [Header("手持外觀設定（新增加）")]
    [Tooltip("撲克牌背面的圖片")]
    public Sprite cardBackSprite;   
    [Tooltip("撲克牌正面的圖片（白底）")]
    public Sprite cardFrontSprite;  
    [Tooltip("本體身上的 TextMeshPro 元件")]
    public TextMeshPro myTextComponent; 

    private float nextAttackTime = 0f;
    private bool isCountingDown = false; // 是否正在冷卻中
    private int[] pokerDeck = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
    
    private SpriteRenderer mySpriteRenderer;
    private int nextDrawnNumber = 1;     // 預先抽好的下一張牌

    void Awake()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // 初始狀態下（如果在地上），確保維持背面
        ShowCardBack();
    }

    public void Equip()
    {
        isEquipped = true;
        this.gameObject.SetActive(true);
        
        // 🚀 2. 【拿起時】：立刻翻到正面，並抽好第一張預備牌亮給玩家看
        RollNextCard();
        ShowCardFront();
        Debug.Log("🃏 抽牌手感就緒！");
    }

    public void Unequip()
    {
        isEquipped = false;
        // 🚀 1. 【在地上時】：切斷父子關係（用你原本的地上邏輯），並翻回背面、藏字
        transform.SetParent(null);
        this.gameObject.SetActive(true);
        ShowCardBack();
        
        // 恢復地上碰撞（如果你有加地上的撿起 Collider）
        Collider2D cd = GetComponent<Collider2D>();
        if (cd != null) cd.enabled = true;

        Debug.Log("🃏 撲克牌回歸地面（背面）。");
    }

    void Update()
    {
        if (!isEquipped) return;

        // 🎯 點擊左鍵攻擊
        if (Mouse.current.leftButton.wasPressedThisFrame && Time.time >= nextAttackTime)
        {
            ThrowCard();
            nextAttackTime = Time.time + attackRate;
            
            // 🚀 3. 【射出後】：立刻進入冷卻協程，變回背面
            StartCoroutine(CooldownRoutine());
        }
    }

    void ThrowCard()
    {
        if (cardPrefab == null || firePoint == null) return;

        // 計算當前這張牌的傷害與文字
        float finalDamage = nextDrawnNumber * baseDamageMultiplier;
        string cardText = GetCardVisualText(nextDrawnNumber);
        Quaternion finalRot = firePoint.rotation;
        if (transform.lossyScale.x < 0) 
        {
            finalRot *= Quaternion.Euler(0, 0, 180); // 👈 這才是「四元數流」的正確加負號反轉法！
        }

        GameObject flyingCard = Instantiate(cardPrefab, firePoint.position, finalRot);
        // 生成飛出去的子彈
        // 把當前傷害與字灌進飛出去的子彈
        CardProjectile cardScript = flyingCard.GetComponent<CardProjectile>();
        if (cardScript != null)
        {
            cardScript.SetupCard(finalDamage, cardText);
        }
    }

    // ⏳ 掌控冷卻外觀的時空協程
    private IEnumerator CooldownRoutine()
    {
        isCountingDown = true;

        // 🚀【射出後 CD 中】：手上的本體瞬間翻到背面，字體消失！
        ShowCardBack();

        // 等待冷卻時間結束
        yield return new WaitForSeconds(attackRate);

        // 🚀 4. 【CD 結束】：全自動隨機抽下一張新牌，並再次翻回正面亮出數字！
        RollNextCard();
        ShowCardFront();

        isCountingDown = false;
    }

    // 🧠 隨機抽下一張牌
    private void RollNextCard()
    {
        int randomIndex = Random.Range(0, pokerDeck.Length);
        nextDrawnNumber = pokerDeck[randomIndex];
    }

    // 🛠️ 顯示正面狀態
    private void ShowCardFront()
    {
        if (mySpriteRenderer != null && cardFrontSprite != null)
            mySpriteRenderer.sprite = cardFrontSprite;

        if (myTextComponent != null)
        {
            myTextComponent.text = GetCardVisualText(nextDrawnNumber);
            myTextComponent.enabled = true; // 亮出字體
        }
    }

    // 🛠️ 顯示背面狀態
    private void ShowCardBack()
    {
        if (mySpriteRenderer != null && cardBackSprite != null)
            mySpriteRenderer.sprite = cardBackSprite;

        if (myTextComponent != null)
            myTextComponent.enabled = false; // 隱藏字體，背面不能穿幫！
    }

    private string GetCardVisualText(int number)
    {
        switch (number)
        {
            case 1: return "A";
            case 11: return "J";
            case 12: return "Q";
            case 13: return "K";
            default: return number.ToString();
        }
    }
}