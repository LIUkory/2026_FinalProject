using System.Collections.Generic;
using UnityEngine;

public class Vehicle_projectile : MonoBehaviour
{
    [Header("飛行設定")]
    public float flySpeed = 15f;
    public float lifeTime = 5f;        // 丟出去後多久強制消失
    public int maxBounces = 1;         // 最多反彈幾次

    [Header("傷害設定")]
    // ★ 修改：將變數名稱改為 damage，方便其他腳本統一讀取，並保持 public
    public float damage = 25f;
    public float damageCooldown = 0.5f; // 同一台車對同一個敵人的傷害冷卻

    private Rigidbody2D rb;
    private Vehicle_weapon owner;
    private int currentBounces = 0;
    private Vector2 lastVelocity;

    private Dictionary<Collider2D, float> hitCooldowns = new Dictionary<Collider2D, float>();

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vehicle_weapon ownerWeapon)
    {
        owner = ownerWeapon;

        // ★ 核心修改：只在飛出去的瞬間，將自己標記為子彈
        gameObject.tag = "bullet";

        // 根據朝向往前飛
        rb.linearVelocity = transform.right * flySpeed;

        // 設定壽命定時炸彈，時間到就自動回歸
        Invoke("ReturnToPlayer", lifeTime);
    }

    void Update()
    {
        lastVelocity = rb.linearVelocity;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            currentBounces++;

            if (currentBounces > maxBounces)
            {
                ReturnToPlayer();
            }
            else
            {
                Vector2 surfaceNormal = collision.contacts[0].normal;
                rb.linearVelocity = Vector2.Reflect(lastVelocity, surfaceNormal).normalized * flySpeed;

                float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                Debug.Log("電動車甩尾反彈！");
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (!hitCooldowns.ContainsKey(other) || Time.time >= hitCooldowns[other])
            {
                // ★ 這裡套用 public 的 damage 變數
                IDamageable enemyTarget = other.GetComponentInParent<IDamageable>();
                if (enemyTarget != null)
                {
                    // 把這顆子彈專屬的 damage 數值，傳給敵人的扣血函式！
                    enemyTarget.TakeDamage(damage);
                    if (enemyTarget != null)
                    {
                        // 把這顆子彈專屬的 damage 數值，傳給敵人的扣血函式！
                        enemyTarget.TakeDamage(damage);
                    }
                    hitCooldowns[other] = Time.time + damageCooldown;

                    Debug.Log("電動車輾過老鼠啦！造成了 " + damage + " 點傷害！");
                }
            }
        }
    }
    void ReturnToPlayer()
    {
        if (owner != null)
        {
            owner.CatchVehicle();
        }
        Destroy(gameObject);
    }
}
