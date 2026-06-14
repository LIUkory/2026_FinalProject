using UnityEngine;

// 加上這行魔法，你就能在 Unity 資料夾按右鍵直接創造新技能！
[CreateAssetMenu(fileName = "New Buff", menuName = "Roguelike System/New Buff Data")]
public class BuffData : ScriptableObject
{
    [Header("增益基本資訊")]
    public string buffName = "神秘增益";         // 標題
    [TextArea(2, 4)]
    public string description = "這裡輸入技能描述..."; // 描述 (TextArea 讓輸入框變大比較好打字)
    public Sprite buffIcon;                    // 圖片

    [Header("經濟設定")]
    public int basePrice = 100;                // 基礎價錢

    [Header("能力數值 (根據你的遊戲擴充)")]
    public float addMaxHealth = 0f;            // 加最大生命
    public float addMoveSpeed = 0f;            // 加移動速度
    public float addDamage = 0f;               // 加攻擊力
    public float addMaxShield = 0f;
    // (未來有新能力，就直接來這裡加變數)
}