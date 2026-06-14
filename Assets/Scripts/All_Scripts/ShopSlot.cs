using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public ShopItem myItem; 

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(BuyItem);
    }
    
    public void SetupSlot(ShopItem newItem)
    {
        myItem = newItem;
        GetComponent<Image>().sprite = newItem.itemIcon; 
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (myItem != null)
        {
            ShopManager manager = FindFirstObjectByType<ShopManager>();
            if (manager != null)
            {
                manager.ShowItemInfo(myItem);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ShopManager manager = FindFirstObjectByType<ShopManager>();
        if (manager != null)
        {
            manager.HideItemInfo();
        }
    }

    // 💰 點擊按鈕時會執行的「購買功能」
    public void BuyItem()
    {
        if (myItem != null)
        {
            int finalPrice = Mathf.RoundToInt(myItem.price * ShopManager.currentDiscount);
            ShopManager manager = FindFirstObjectByType<ShopManager>();

            // 🌟 核心升級：向你的 FurryBank_manager 請款！
            if (FurryBank_manager.instance.SpendMoney(finalPrice))
            {
                // 🟢 錢夠，扣款成功！
                Debug.Log("狐狸買了：" + myItem.itemName + "！ 實際花費：" + finalPrice);

                // 讓商雞的對話框稱讚你
                if (manager != null && manager.dialogueText != null)
                {
                    manager.dialogueText.text = "好眼光！這東西歸你了！";
                }
                if (manager != null) manager.UpdateMoneyDisplay();
                // 1. 把商品裝進跨場景購物車裡
                ShopManager.boughtItems.Add(myItem);

                // 2. 讓這個商品按鈕在畫面上「隱藏消失」
                gameObject.SetActive(false);

                // 3. 隱藏商品資訊看板
                if (manager != null) manager.HideItemInfo();
            }
            else
            {
                // 🔴 錢不夠，拒絕交易！
                Debug.Log("銀行餘額不足，購買失敗！");

                // 讓商雞的對話框無情嘲諷
                if (manager != null && manager.dialogueText != null)
                {
                    manager.dialogueText.text = "臭乞丐，沒錢還敢來逛街啊！";
                }
                
                // ⚠️ 注意：因為錢不夠，所以下面「加進購物車」和「按鈕消失」的程式都不會執行！
                // 商品會乖乖留在架上等你存夠錢來買。
            }
        }
    }
}