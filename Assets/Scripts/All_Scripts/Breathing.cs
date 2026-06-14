using UnityEngine;

public class BreathingEffect : MonoBehaviour
{
    [Header("呼吸設定")]
    public float speed = 3f;        // 呼吸的速度 (越快喘得越急)
    public float amount = 0.05f;    // 形變的幅度 (數字越大，吸氣膨脹越明顯)

    private Vector3 originalScale;

    void Start()
    {
        // 記住角色原本的大小
        originalScale = transform.localScale;
    }

    void Update()
    {
        // 利用 Time.time 配合 Sin 函數，產生一個平滑循環的數值
        float wave = Mathf.Sin(Time.time * speed) * amount;

        // ★ 擠壓與拉伸 (Squash and Stretch) 的動畫法則：
        // 當 Y 軸 (身高) 被拉長時，X 軸 (寬度) 就稍微縮扁一點點，看起來會更有彈性肉感！
        transform.localScale = new Vector3(
            originalScale.x - (wave * 0.3f), // X 軸微微反向形變
            originalScale.y + wave,          // Y 軸上下浮動
            originalScale.z
        );
    }
}