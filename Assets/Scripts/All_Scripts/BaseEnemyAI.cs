using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Rigidbody2D))]
public class BaseEnemyAI : MonoBehaviour, IDamageable
{
    [Header("生命值設定")]
    public float maxHealth = 50f;
    public float health; 

    [Header("偵測與移動設定（純聽力半徑）")]
    public float moveSpeed = 3f;         
    public float stopDistance = 1.2f;     
    public float detectionRange = 5f;    
    public float patrolRange = 3f;       
    protected Vector2 patrolStartPos;    
    protected int patrolDirection = 1;   

    [Header("攻擊與特效設定")]
    public float damageAmount = 10f;     
    public float attackRate = 1.5f;      
    protected float nextAttackTime = 0f; 
    public GameObject attackEffectPrefab; 

    [Header("巡邏停頓設定")]
    public float turnDelay = 0.5f;     
    protected float turnTimer = 0f;      
    protected bool isWaiting = false;

    [Header("面試官專用設定 (整合版)")]
    public string employeeName = "基層員工";
    public GameObject jobApplicationPrefab; // 拖入子物件：工作應徵表
    public GameObject stampPrefab;          // 拖入子物件：印章
    public float stampAnimDuration = 0.15f;
    public float destroyDelay = 1.5f;
    public float finalStampScale = 0.25f;
    

    [Header("暈厥設定")]
    public GameObject starPrefab;    // 拖入你的金星 Prefab
    public float recruitWindow = 8f; // 招募倒數時間
    public bool isFainted = false;   // 標記敵人是否已經暈倒
    private bool isRecruited = false; // 防連點鎖
    private GameObject spawnedStar;   // 用來記住頭頂那顆星星，方便蓋章時把它刪掉

    // ─── 動態生成出來的執行個體暫存 ───
    private GameObject spawnedJobApp;
    private GameObject spawnedStamp;

    // ─── ✨ 體型維穩核心變數 ───
    private float originalScaleX;
    private float originalScaleY;

    public Transform player;
    protected Rigidbody2D rb;
    private bool wasChasing = false; 

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        health = maxHealth;
        patrolStartPos = transform.position; 

        // 🎯 ✨ 開機瞬間，自動記住您在 Inspector 幫牠拉好的完美體型大小！
        originalScaleX = Mathf.Abs(transform.localScale.x);
        originalScaleY = transform.localScale.y;

        // 確保剛生成時 (怪物還活著時)，履歷表和印章都是隱藏的
        if (jobApplicationPrefab != null) jobApplicationPrefab.SetActive(false);
        if (stampPrefab != null)
        {
            stampPrefab.SetActive(false);
            stampPrefab.transform.localScale = Vector3.zero;
        }

        TryFindPlayer();

    }

    protected virtual void FixedUpdate()
    {
        if (player == null)
        {
            if (TryFindPlayer() == false)
            {
                Patrol(); 
                HandleSpriteFlip(); 
                return; 
            }
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        bool canHear = distanceToPlayer <= detectionRange;

        if (canHear)
        {
            wasChasing = true;
            isWaiting = false; 
            ChaseAndAttackLogic(distanceToPlayer);
        }
        else
        {
            if (wasChasing)
            {
                rb.linearVelocity = Vector2.zero;
                wasChasing = false;
                isWaiting = true; 
                turnTimer = turnDelay;
            }
            Patrol();
        }

        // 全自動絕對速度翻轉
        HandleSpriteFlip();
    }

    private bool TryFindPlayer()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            return true;
        }
        return false;
    }

    protected virtual void Patrol()
    {
        if (isWaiting)
        {
            rb.linearVelocity = Vector2.zero;
            turnTimer -= Time.fixedDeltaTime;
            if (turnTimer <= 0) isWaiting = false;
            return;
        }

        float distanceFromStart = transform.position.x - patrolStartPos.x;

        if ((distanceFromStart >= patrolRange && patrolDirection == 1) || 
            (distanceFromStart <= -patrolRange && patrolDirection == -1))
        {
            patrolDirection *= -1; 
            isWaiting = true;
            turnTimer = turnDelay;
            rb.position = new Vector2(rb.position.x + (patrolDirection * 0.05f), rb.position.y);
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            rb.linearVelocity = new Vector2(patrolDirection * (moveSpeed * 0.5f), 0f);
        }
    }

    // ─── 🚀 解解鎖 Y 軸移動力的通用追擊邏輯 ───
    protected void ChaseAndAttackLogic(float distance)
    {
        Vector2 offset = (Vector2)player.position - rb.position;
        Vector2 direction = offset.normalized;

        if (distance > stopDistance)
        {
            
            rb.linearVelocity = direction * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            if (Time.time >= nextAttackTime)
            {
                ExecuteAttack(); 
                nextAttackTime = Time.time + attackRate;
            }
        }
    }

    // ─── 🎯 全自動「記憶體型」翻轉器 ───
    private void HandleSpriteFlip()
    {
        if (Mathf.Abs(rb.linearVelocity.x) > 0.01f)
        {
            if (rb.linearVelocity.x > 0f)
            {
                // 【往右走】：保留原始寬度，只加負號翻轉
                transform.localScale = new Vector3(-originalScaleX, originalScaleY, 1f);
            }
            else if (rb.linearVelocity.x < 0f)
            {
                // 【往左走】：保留原始寬度，正號朝向
                transform.localScale = new Vector3(originalScaleX, originalScaleY, 1f);
            }
        }
    }

    protected virtual void ExecuteAttack() {}

    public void TakeDamage(float damage)
    {
        if (isFainted) return;
        health -= damage;
        Debug.Log($"{gameObject.name} 受到了 {damage} 點傷害！剩餘：{health}");
        if (health <= 0) Die();
    }

    protected virtual void Die()
    {
        // 1. 防止重複觸發死亡與行動
        this.enabled = false;
        isFainted = true;

        // 2. 停止物理移動，並把碰撞體改成 Trigger (讓子彈可以直接穿過去)
        if (rb != null) { rb.linearVelocity = Vector2.zero; }
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) { col.isTrigger = true; }

        // 3. 改變標籤，讓玩家的招募雷達只抓得到它
        gameObject.tag = "Fainted";
        gameObject.layer = 6;
        // 4. 體型縮小成 0.8 倍   
        transform.localScale = new Vector3(originalScaleX * 0.8f, originalScaleY * 0.8f, 1f);

        // 5. 在頭頂生成會動的金星，並記存在 spawnedStar 中！
        if (starPrefab != null)
        {
            Vector3 starPos = transform.position + new Vector3(0, 0.8f, 0);
            spawnedStar = Instantiate(starPrefab, starPos, Quaternion.identity, transform);
        }

        Debug.Log($"{gameObject.name} 已暈厥！頭冒金星，等待招募中！");

        // 6. 啟動定時炸彈：倒數 recruitWindow (8秒) 後徹底銷毀
        Destroy(gameObject, recruitWindow);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("bullet"))
        {
            float finalDamage = 10f; 
            MonoBehaviour bulletComponent = other.GetComponent<MonoBehaviour>();
            if (bulletComponent != null)
            {
                var field = bulletComponent.GetType().GetField("damage");
                if (field != null) finalDamage = (float)field.GetValue(bulletComponent);
            }
        }
    }
    public void BeSignedUp()
    {
        if (isRecruited) return;
        isRecruited = true;

        // 1. 呼叫 HR 系統，把這隻怪登記進去
        if (HR_manager.instance != null)
        {
            HR_manager.instance.RecruitEnemy(employeeName);
        }

        // 2. 蓋章時，把頭頂轉圈圈的金星刪掉
        if (spawnedStar != null) Destroy(spawnedStar);

        // 3. 啟動蓋章動畫的協程
        StartCoroutine(SignAnimationRoutine());
    }

    protected IEnumerator SignAnimationRoutine()
    {
        // 1. 生成應徵表並強制歸零旋轉與相對座標，防變形！
        if (jobApplicationPrefab != null)
        {
            spawnedJobApp = Instantiate(jobApplicationPrefab, transform.position, Quaternion.identity, transform);
            spawnedJobApp.transform.localRotation = Quaternion.identity;
            spawnedJobApp.transform.localPosition = Vector3.zero;
            spawnedJobApp.SetActive(true);
        }

        yield return new WaitForSeconds(0.1f);

        // 2. 生成印章並執行 Lerp 縮放動畫
        if (stampPrefab != null)
        {
            spawnedStamp = Instantiate(stampPrefab, transform.position, Quaternion.identity, transform);
            spawnedStamp.transform.localRotation = Quaternion.identity;
            spawnedStamp.transform.localPosition = Vector3.zero;

            // 🟢 【關鍵修正】：生出來立刻強行點亮！這樣 Unity 才會願意幫它跑放大動畫！
            spawnedStamp.SetActive(true);

            // 隨後再把它縮小成 0，當作動畫起點
            spawnedStamp.transform.localScale = Vector3.zero;

            Vector3 targetScale = Vector3.one * finalStampScale;
            float elapsed = 0f;

            // 現在物件是亮著的，這個 while 迴圈就能正常每幀把縮放從 0 漸漸加到 targetScale 了！
            while (elapsed < stampAnimDuration)
            {
                if (spawnedStamp == null) yield break;
                spawnedStamp.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, elapsed / stampAnimDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            if (spawnedStamp != null) spawnedStamp.transform.localScale = targetScale;
        }

        Debug.Log($"【人事部】錄取 {employeeName}！附上應徵表與核准章！");

        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }

    // ... 下面接續你原本的 Die() 等其他邏輯 ...

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}