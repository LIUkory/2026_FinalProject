using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Ka_SimpleBat : MonoBehaviour, IWeapon
{
    public bool isEquipped = false;

    [Header("發射與特效設定")]
    public GameObject batSwingEffectPrefab; 
    public Transform firePoint;            
    public float attackRate = 0.5f;         
    public float batDamage = 15f;          

    [Header("基礎揮動時間")]
    [Tooltip("揮棒動作總共要花多少秒完成")]
    public float swingDuration = 0.15f;   

    private float nextAttackTime = 0f;
    private bool isSwinging = false;       
    private Collider2D myBatCollider;      

    void Awake()
    {
        myBatCollider = GetComponent<Collider2D>();
    }

    void Start()
    {
        // 如果一開始是放在地上的掉落物，開啟碰撞；如果是裝備狀態就交由 Equip 控制
        if (isEquipped)
        {
            if (myBatCollider != null) myBatCollider.enabled = false;
        }
        else
        {
            if (myBatCollider != null) myBatCollider.enabled = true;
        }
    }

    public void Equip()
    {
        isEquipped = true;
        this.gameObject.SetActive(true);
        
        // 🚀 拿在手上時，平時關閉傷害碰撞，等揮動時才開啟
        if (myBatCollider != null) myBatCollider.enabled = false; 
        
        Debug.Log("🪵 木棒已裝備到手上！");
    }

    // 🎯 核心改動：放下武器，讓它老老實實留在原地（地上）
    public void Unequip()
    {
        isEquipped = false;
        isSwinging = false;

        // 1. 🔗 斷開與 Fox 的物理跟隨，讓木棒重回世界大地的懷抱
        transform.SetParent(null);

        // 2. 🟢 確保物件在世界上是「亮著」的，別讓它隱藏消失
        this.gameObject.SetActive(true);

        // 3. 🎨 確保圖片渲染是開啟的，玩家在地上才看得到這根魔性木棒
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = true;

        // 4. ⚡ 讓木棒留在地上的時候，碰撞器（Collider）是開啟的，這樣 Fox 走過去才能再次偵測並撿起
        if (myBatCollider != null) 
        {
            myBatCollider.enabled = true;
        }

        // 5. 📐 可選：讓落地的木棒角度回正（比如平躺在地上，看起來更像掉落物）
        transform.localRotation = Quaternion.identity;

        Debug.Log("🪵 木棒已丟棄，留在了地上！");
    }

    void Update()
    {
        // 🚀 如果木棒已經掉在地上（沒被裝備），就不要執行按左鍵揮動的邏輯！
        if (!isEquipped || isSwinging) return;

        // 🎯 只要按左鍵，且冷卻時間到了就揮動
        if (Mouse.current.leftButton.wasPressedThisFrame && Time.time >= nextAttackTime)
        {
            StartCoroutine(SimpleBatSwingRoutine());
            nextAttackTime = Time.time + attackRate;
        }
    }

    private IEnumerator SimpleBatSwingRoutine()
    {
        isSwinging = true;

        // 1. 🚀 產生揮擊特效
        ShootMeleeEffect();

        // 2. ⚡ 揮擊瞬間，立刻開啟木棒自己的碰撞判定！
        if (myBatCollider != null) myBatCollider.enabled = true;

        // 3. 📐 記憶揮擊前的「初始角度」
        float startAngle = transform.localRotation.eulerAngles.z;
        
        // 4. 🧠 判斷區間並決定目標角度
        float targetAngleOffset = 0f;

        if (startAngle >= 0f && startAngle <= 180f)
        {
            targetAngleOffset = -120f;  
        }
        else
        {
            targetAngleOffset = 120f; 
        }

        float endAngle = startAngle + targetAngleOffset;

        // 5. 🚀 執行平滑旋轉動畫
        float elapsedTime = 0f;
        while (elapsedTime < swingDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / swingDuration;

            float currentAngle = Mathf.LerpAngle(startAngle, endAngle, t);
            transform.localRotation = Quaternion.Euler(0f, 0f, currentAngle);

            yield return null;
        }

        // 6. 🛑 揮完之後瞬間歸位，並且「立刻關閉」碰撞器！
        transform.localRotation = Quaternion.Euler(0f, 0f, startAngle);
        
        // 🛡️ 安全防線：揮完收招後，因為還在手上，所以要關閉碰撞判定，避免走路誤觸
        if (isEquipped && myBatCollider != null) myBatCollider.enabled = false;

        isSwinging = false;
    }

    void ShootMeleeEffect()
    {
        if (batSwingEffectPrefab == null || firePoint == null) return;

        GameObject effectObj = Instantiate(batSwingEffectPrefab, firePoint.position, firePoint.rotation);
        Destroy(effectObj, swingDuration);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ❌ 只有在「拿在手上揮舞」的狀態下，木棒才能砸碎子彈和打怪！
        if (!isEquipped) return;

        // 🔥【功能 A：木棒肉體砸碎子彈】
        if (other.CompareTag("EnemyProjectile") || 
            other.name.Contains("Arrow") || 
            other.name.Contains("WaterBall") || 
            other.name.Contains("bullet"))
        {
            Debug.Log($"🪵【純物理格擋】木棒當場砸爛了子彈：{other.name}！");
            Destroy(other.gameObject); 
        }

        // 💥【功能 B：木棒肉體棒打怪物】
        if (other.CompareTag("Enemy"))
        {
            other.gameObject.SendMessage("TakeDamage", batDamage, SendMessageOptions.DontRequireReceiver);
            Debug.Log($"🪵【純物理重擊】木棒本尊狠狠痛扁了 {other.name}！造成 {batDamage} 點傷害！");
        }
    }
}