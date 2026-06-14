using UnityEngine;

public class LaserAttack : MonoBehaviour
{
    [Header("時間設定")]
    public float chargeDuration = 0.5f; 
    public float activeDuration = 0.3f; 

    [Header("雷射視覺粗細設定")]
    public float thickScaleY = 2f;   
    public float thinScaleY = 0.2f;   

    [Header("長方形傷害範圍設定")]
    [Tooltip("雷射實際傷害長方形的：(長度, 寬度)")]
    public Vector2 laserBoxSize = new Vector2(24f, 2.0f); 

    private float damageAmount;         
    private float timer = 0f;
    private bool hasDealtDamage = false;

    public void SetDamage(float amount)
    {
        damageAmount = amount;
    }

    void Start()
    {
        // 出生瞬間 Y 軸壓扁
        transform.localScale = new Vector3(transform.localScale.x, thinScaleY, 1f);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer < chargeDuration)
        {
            transform.localScale = new Vector3(transform.localScale.x, thinScaleY, 1f);
        }
        else if (timer < chargeDuration + activeDuration)
        {
            transform.localScale = new Vector3(transform.localScale.x, thickScaleY, 1f);

            // 🎯【長方形精準打擊】
            if (!hasDealtDamage)
            {
                DealLaserBoxDamage();
                hasDealtDamage = true;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void DealLaserBoxDamage()
    {
        Debug.Log("💥 💥 激光爆發！長方形綠框完美置中判定！");

        // 📐【自動置中核心】：長方形的中心點，永遠等於「起點往前推【長度的一半】」
        // 因為你的雷射是朝向 transform.right 噴出去，所以我們用長度 (laserBoxSize.x) 除以 2
        float halfLength = laserBoxSize.x * 0.5f;
        Vector2 perfectCenter = (Vector2)transform.position + ((Vector2)transform.right * halfLength);

        // 🚀 執行旋轉長方形偵測
        Collider2D[] hits = Physics2D.OverlapBoxAll(perfectCenter, laserBoxSize, transform.eulerAngles.z);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                hit.gameObject.SendMessage("TakeDamage", damageAmount, SendMessageOptions.DontRequireReceiver);
                Debug.Log($"🎯 置中雷射切過 Fox！造成了 {damageAmount} 點傷害！");
            }
        }
    }

    // 📐 在 Scene 視窗畫出長方形框框
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        
        // 📐【自動置中核心】：讓 Gizmos 畫出來的綠框也自動往前挪【長度的一半】
        float halfLength = laserBoxSize.x * 0.5f;
        Vector2 localCenter = Vector2.right * halfLength;

        // 套用旋轉矩陣
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        
        // 畫出完美包覆雷射前半段的置中方框
        Gizmos.DrawWireCube(localCenter, laserBoxSize);
    }
}