using UnityEngine;

public class HR_Recruiter : MonoBehaviour
{
    [Header("招募雷達設定")]
    public float recruitRadius = 2.5f;   // 蓋章的有效距離
    public LayerMask faintedEnemyLayer;  // 專門偵測「倒地怪物」的圖層

    [Header("選用：招募特效")]
    public ParticleSystem areaScanVFX;   // (選用) 掃描時可以播個雷達光波特效

    // 🔥 萬用公開接口：誰都可以呼叫這個方法！
    public void TryRecruitInArea()
    {
        // 1. 測試有沒有成功收到右鍵訊號
        Debug.Log("【HR雷達】啟動！狐狸按下了右鍵！");

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, recruitRadius, faintedEnemyLayer);

        // 2. 測試雷達的圖層設定有沒有抓到東西
        Debug.Log($"【HR雷達】掃描半徑內，共找到 {hitColliders.Length} 個符合圖層的碰撞體！");

        foreach (Collider2D co in hitColliders)
        {
            // 3. 測試掃到的東西，名字跟 Tag 是什麼
            Debug.Log($"【HR雷達】正在檢查：{co.gameObject.name}，它的 Tag 是：{co.tag}");

            if (co.CompareTag("Fainted"))
            {
                BaseEnemyAI faintedCandidate = co.GetComponent<BaseEnemyAI>();

                if (faintedCandidate != null)
                {
                    Debug.Log("【HR雷達】完美鎖定目標！開始執行蓋章動畫！");
                    faintedCandidate.BeSignedUp();
                    break;
                }
                else
                {
                    Debug.LogWarning("【HR雷達】警告！抓到了帶有 fainted 標籤的物件，但它身上卻沒有繼承 BaseFaintedEnemy 的腳本！");
                }
            }
        }
    }

    // 畫出輔助線，方便你在 Unity 編輯器裡調整雷達大小
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f); // 橘色半透明
        Gizmos.DrawWireSphere(transform.position, recruitRadius);
    }
}
