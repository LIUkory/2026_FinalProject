using UnityEngine;

public class BossBubble : MonoBehaviour
{
    [Header("泡泡設定")]
    public float speed = 2f;           // 泡泡的飛行速度 (因為是緩慢的，設小一點)
    public float lifetime = 3f;        // 飛多遠/多久後自動爆炸
    public GameObject vortexPrefab;    // ★ 爆炸後要生成的漩渦 Prefab

    private bool hasExploded = false;  // 防止重複爆炸的保險開關

    void Start()
    {
        // 倒數計時，時間到就自動呼叫 Explode 爆炸
        Invoke("Explode", lifetime);
    }

    void Update()
    {
        // 泡泡緩慢往前飛
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasExploded) return;

        // 碰到玩家 或 碰到牆壁 時，立刻爆炸
        if (other.CompareTag("Player") || other.CompareTag("Wall"))
        {
            Explode();
        }
    }

    void Explode()
    {
        hasExploded = true;

        // 1. 生成漩渦 (在泡泡目前的位置，角度歸零)
        if (vortexPrefab != null)
        {
            Instantiate(vortexPrefab, transform.position, Quaternion.identity);
        }

        // 2. 泡泡功成身退，銷毀自己
        Destroy(gameObject);
    }
}
