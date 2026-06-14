using System.Collections;
using UnityEngine;

public class TentacleEnemy : MonoBehaviour, IDamageable
{
    [Header("數值設定")]
    public float maxHealth = 50f;
    private float currentHealth;
    public float damageToPlayer = 20f;

    [Header("攻擊節奏 (與動畫完美同步)")]
    public float spawnDelay = 0.67f;    // 出生動畫 (0:40) 大約 0.67 秒
    public float aimTime = 1.0f;        // 盯著玩家的發呆時間
    public float attackWindup = 0.5f;   // ★ 蓄力時間：完美對齊 0:30 砸下的瞬間
    public float hitboxActiveTime = 0.2f;// 傷害判定存活時間 (極短，確保打擊精準)
    public float recoveryTime = 1.0f;   // 收招與休息時間
    public float attackRange = 3.5f;
    [Header("碰撞綁定")]
    public Collider2D damageHitbox;

    private Animator anim;
    private Transform player;
    private bool isAttacking = false;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        if (damageHitbox != null) damageHitbox.enabled = false;

        StartCoroutine(AttackCycle());
    }

    IEnumerator AttackCycle()
    {
        // --- 階段 0：剛出生，等待破土動畫播完 ---
        // 0:40 大約是 0.67 秒，這段時間觸手不瞄準、不攻擊
        yield return new WaitForSeconds(spawnDelay);

        while (currentHealth > 0 && player != null)
        {
            isAttacking = false;

            // --- 階段 1：基礎瞄準與冷卻 ---
            // 剛拔起來時，至少會盯著玩家看 aimTime 秒
            float timer = 0;
            while (timer < aimTime)
            {
                AimAtPlayer();
                timer += Time.deltaTime;
                yield return null;
            }

            // --- 階段 2：等待玩家進入「攻擊範圍」 ---
            // ★ 如果玩家躲得遠遠的，牠就一直轉頭盯著看，直到玩家白目走進範圍內！
            while (Vector2.Distance(transform.position, player.position) > attackRange)
            {
                AimAtPlayer();
                yield return null; // 每一幀都在檢查
            }

            // --- 階段 3：玩家進入範圍，發動攻擊！ ---
            if (anim != null) anim.SetTrigger("Attack");
            
            TentacleHit hitboxScript = damageHitbox.GetComponent<TentacleHit>();
            if (hitboxScript != null)
            {
                hitboxScript.ResetHitbox();
            }
            yield return new WaitForSeconds(attackWindup);
            // --- 階段 4：狠狠砸下 (打開判定) ---
            isAttacking = true;
            if (damageHitbox != null) damageHitbox.enabled = true;

            yield return new WaitForSeconds(hitboxActiveTime);

            if (damageHitbox != null) damageHitbox.enabled = false;

            // --- 階段 5：收招動畫與休息 ---
            yield return new WaitForSeconds(recoveryTime);
        }
    }

void AimAtPlayer()
    {
        // 砸下跟蓄力的瞬間鎖死方向
        if (player == null || isAttacking) return; 
        
        // 1. 強制讓觸手保持絕對直立！不准再亂轉！
        transform.rotation = Quaternion.Euler(0, 0, 0   );

        // 2. 判斷玩家在觸手的左邊還是右邊
        if (player.position.x < transform.position.x)
        {
            // 玩家在左邊 -> 觸手鏡像翻轉朝左
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            // 玩家在右邊 -> 觸手保持原本朝向 (假設原圖砸下的方向是右邊)
            // ⚠️ 如果你的原圖砸下是朝左，請把這裡的 1 和上面的 -1 互換！
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log("【觸手】受到攻擊！剩餘血量：" + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("【觸手】被斬斷了！");
        Destroy(gameObject);
    }

    //void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (isAttacking && other.CompareTag("Player"))
    //    {
    //        // ★ 記得呼叫你們專案裡狐狸的扣血寫法
    //        Debug.Log("【觸手】啪！打中玩家，造成了 " + damageToPlayer + " 點傷害！");
    //    }
    //}
}