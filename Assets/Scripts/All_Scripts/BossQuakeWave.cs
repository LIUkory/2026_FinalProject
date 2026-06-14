using UnityEngine;

public class BossQuakeWave : MonoBehaviour
{
    [Header("震波設定")]
    public float lifetime = 1f;        // 存活時間
    public float damage = 20f;

    [Header("視覺效果")]
    public float expandSpeed = 0.1f;   // 原地擴張的速度
    public float maxScale = 0.5f;        // ★ 新增：震波的最大尺寸限制 (例如放大到原本的 2 倍就停住)

    void Start()
    {
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        // 時間到自動銷毀
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // ★ 修改這裡：只有當目前的尺寸小於「最大尺寸限制」時，才繼續放大
        if (transform.localScale.x < maxScale)
        {
            // 繼續放大
            transform.localScale += Vector3.one * expandSpeed * Time.deltaTime;

            // 防呆機制：如果一不小心放大超過了，強制作為 maxScale，確保大小完美精準
            if (transform.localScale.x > maxScale)
            {
                transform.localScale = Vector3.one * maxScale;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Fox_stats playerStats = other.GetComponent<Fox_stats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
            }
        }
    }
}
