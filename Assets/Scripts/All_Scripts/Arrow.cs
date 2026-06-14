using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Arrow : MonoBehaviour
{
    [Header("飛行速度設定")]
    public float flySpeed = 8f;

    // 🎯【開放公開變數】：這樣不管是誰來抓這個欄位，都能精準給值
    [Header("傷害數值")]
    public float damageAmount = 5f; // 💡 給個預設值 5f，就算大腦管道卡住，它也絕對不會是 0 傷害！

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.gravityScale = 0f; // 重力歸零
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        // 🚀【全敵人幽靈化攔截】：確保老鼠射出來的箭一出生就不會卡到自己
        Collider2D myCollider = GetComponent<Collider2D>();
        if (myCollider != null)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                Collider2D enemyCollider = enemy.GetComponent<Collider2D>();
                if (enemyCollider != null)
                {
                    Physics2D.IgnoreCollision(myCollider, enemyCollider);
                }
            }
        }

        // 1. 全自動尋找狐狸主角
        GameObject playerObj = GameObject.FindWithTag("Player");
        
        if (playerObj != null && rb != null)
        {
            // 2. 📐 計算前進方向
            Vector2 direction = (playerObj.transform.position - transform.position).normalized;
            
            // 🚀 3. 物理開火！
            rb.linearVelocity = direction * flySpeed;

            // 4. 📐 角度校正
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        // ⏰ 3 秒後自動銷毀
        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 🚀【只對 Player 標籤起反應】
        if (other.CompareTag("Player"))
        {
            // 🎯【修正關鍵】：這裡的 Log 修正了字串插值符號 $，才能在 Console 誠實印出數字！
            other.gameObject.SendMessage("TakeDamage", damageAmount, SendMessageOptions.DontRequireReceiver);
            Debug.Log($"💥 箭精準穿透命中 Fox！造成了 {damageAmount} 點高額傷害！");
            
            // 砸中主角後，自爆消失
            Destroy(gameObject);
        }
    }
}