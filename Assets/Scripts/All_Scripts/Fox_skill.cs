using System.Collections;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Fox_skill : Basic_skill
{
    [Header("翻滾設定")]
    public float rollSpeed = 15f;
    public float rollDuration = 0.3f;
    public bool isInvincible = false;
    private Rigidbody2D rb;
    private Fox_movement moveScript;
    private Animator anim; // ★ 1. 新增 Animator 變數

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveScript = GetComponent<Fox_movement>();
        anim = GetComponent<Animator>(); // ★ 2. 抓取狐狸身上的 Animator
    }

    protected override void ActivateSkillLogic()
    {
        StartCoroutine(RollRoutine());
    }

    private IEnumerator RollRoutine()
    {
        Debug.Log("【狐狸技能】翻滾啟動！");
        isInvincible = true;
        if (anim != null) anim.SetTrigger("Roll");
        if (moveScript != null) moveScript.enabled = false;

        // 1. 先把滑鼠的螢幕座標存起來
        Vector3 screenMousePos = Input.mousePosition;

        // 2. ★ 關鍵破解：強制給予 Z 軸深度 (攝影機跟狐狸的距離)，通常是 10
        screenMousePos.z = Mathf.Abs(Camera.main.transform.position.z);

        // 3. 這時候再去轉換，Unity 就會算出完美的滑鼠世界座標了！
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(screenMousePos);
        mouseWorldPos.z = 0f;

        // 後面維持你的正確邏輯
        Vector2 rollDir = (mouseWorldPos - transform.position).normalized;

        // ==========================================
        // ★ 視覺優化：翻滾瞬間，讓狐狸面朝滑鼠的方向！
        // ==========================================
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            // 如果方向的 X 小於 0 (往左)，就水平翻轉圖片 (flipX = true)
            sr.flipX = rollDir.x < 0;
        }
        // ==========================================

        float elapsed = 0f;
        while (elapsed < rollDuration)
        {
            rb.linearVelocity = rollDir * rollSpeed;
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        if (moveScript != null) moveScript.enabled = true;
        isInvincible = false;
        Debug.Log("【狐狸技能】翻滾結束，恢復控制！");
    }
}
