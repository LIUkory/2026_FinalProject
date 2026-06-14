using UnityEngine;

public class BossVertex : MonoBehaviour
{
    [Header("漩渦設定")]
    public float lifetime = 3f;        // 漩渦存在的時間
    public float pullForce = 4f;       // ★ 牽引力道！(你的狐狸跑速是 8，設為 4 代表狐狸拼命跑還是能以 4 的速度逃離)

    [Header("視覺效果")]
    public float rotateSpeed = -200f;  // 旋轉速度 (負數代表順時針)
    public float shrinkSpeed = 0.3f;   // 每秒縮小的速度

    void Start()
    {
        // 時間到自動銷毀
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // 1. 一直旋轉
        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);

        // 2. 慢慢縮小 (確保不會縮到變成負數反轉)
        if (transform.localScale.x > 0)
        {
            transform.localScale -= Vector3.one * shrinkSpeed * Time.deltaTime;
        }
    }

    // ★ 關鍵：只要玩家還在漩渦的觸發範圍內，就會不斷被吸過去
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 算出從「玩家」指向「漩渦中心」的方向
            Vector3 direction = (transform.position - other.transform.position).normalized;

            // 將玩家往中心點拖拽
            other.transform.position += direction * pullForce * Time.deltaTime;
        }
    }
}
