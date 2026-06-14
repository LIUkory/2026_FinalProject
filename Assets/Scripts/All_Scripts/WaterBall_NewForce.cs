using UnityEngine;



[RequireComponent(typeof(Rigidbody2D))]

public class WaterBall_NewForce : MonoBehaviour

{

    [Header("飛行速度設定")]

    public float flySpeed = 8f;

    [HideInInspector] // 在 Inspector 隱藏它，防止主人不小心點到手動填值

    public float damageAmount;

    private Rigidbody2D rb;



    void Start()

    {

        rb = GetComponent<Rigidbody2D>();



        if (rb != null)

        {

            rb.gravityScale = 0f; // 重力歸零



            // 🎯【全軸向解鎖】：強制解除 X、Y 軸的所有鎖定，只鎖定旋轉

            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        }



        // 1. 全自動尋找狐狸主角

        GameObject playerObj = GameObject.FindWithTag("Player");

       

        if (playerObj != null && rb != null)

        {

            // 2. 📐 計算前進方向（全方位向量）

            Vector2 direction = (playerObj.transform.position - transform.position).normalized;

           

            // 🚀 3. 物理開火！讓它往玩家的方向直線動過去

            rb.linearVelocity = direction * flySpeed;



            // 4. 📐 旋轉圖片角度面朝玩家

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

            // 使用主人指定的強扣血防線！

            other.gameObject.SendMessage("TakeDamage", damageAmount, SendMessageOptions.DontRequireReceiver);

            Debug.Log($"水泡精準穿透命中 Fox！造成 {damageAmount} 點傷害！");
            // 砸中主角後，水泡自爆消失

            Destroy(gameObject);

        }
        // 💡 註：因為除了 Player 之外我們什麼都不做，所以撞到史萊姆或牆壁時，
        // 水泡會像幽靈一樣直接穿過去，直到 3 秒時間到自然蒸發！
    }
} 

