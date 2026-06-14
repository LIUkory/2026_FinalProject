using UnityEngine;
using System.Collections;
public class BaseFaintedEnemy : MonoBehaviour
{
    [Header("面試官專用設定")]
    public string employeeName = "基層員工";
    public int laborValue = 1;
    public float expirationTime = 10f;

    [Header("視覺演出綁定")]
    public GameObject jobApplicationObj; // 拖入子物件：工作應徵表
    public GameObject stampObj;          // 拖入子物件：印章
    public float stampAnimDuration = 0.15f; // 印章放大的時間 (越短越有打擊感)
    public float destroyDelay = 0.5f;      // 蓋完章後停留多久才消失
    public float finalStampScale = 0.25f;
    private bool isRecruited = false; // ★ 防連點鎖：防止動畫期間重複蓋章

    protected virtual void Start()
    {
        // 時間到自動被清潔隊銷毀
        Destroy(gameObject, expirationTime);

        // 確保剛生成時，履歷表和印章都是隱藏的
        if (jobApplicationObj != null) jobApplicationObj.SetActive(false);
        if (stampObj != null)
        {
            stampObj.SetActive(false);
            // 初始化印章縮放為 0
            stampObj.transform.localScale = Vector3.zero;
        }
    }

    // 狐狸按右鍵時呼叫這裡
    public virtual void BeSignedUp()
    {
        // ★ 核心防禦：如果已經被蓋過章了，就直接彈回，不准再刷業績！
        if (isRecruited) return;
        isRecruited = true;

        // 1. 跨局存檔：真正把業績寫入數據庫
        int currentSlaves = PlayerPrefs.GetInt("TotalSlaves", 0);
        PlayerPrefs.SetInt("TotalSlaves", currentSlaves + laborValue);
        PlayerPrefs.Save();

        // 2. 啟動蓋章動畫的協程
        StartCoroutine(SignAnimationRoutine());
    }

    protected IEnumerator SignAnimationRoutine()
    {
        // --- 階段 1：貼上履歷表 ---
        if (jobApplicationObj != null) jobApplicationObj.SetActive(true);

        // 稍微停頓 0.1 秒，讓玩家眼睛捕捉到履歷表貼上去的瞬間
        yield return new WaitForSeconds(0.1f);

        // --- 階段 2：印章砸下 (Scale 從 0 到 1) ---
        if (stampObj != null)
        {
            stampObj.SetActive(true);
            Vector3 targetScale = Vector3.one * finalStampScale; // 目標大小 (1, 1, 1)
            float elapsed = 0f;

            while (elapsed < stampAnimDuration)
            {
                // Lerp 會根據時間比例，平滑地把大小從 0 放大到 1
                stampObj.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, elapsed / stampAnimDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            stampObj.transform.localScale = targetScale; // 確保最後剛好是完美的 1

            // 💡 這裡超級適合加上一行播放「碰！」蓋章音效的程式碼！
        }

        OnSignSuccess();

        // --- 階段 3：停留欣賞，打包送走 ---
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }

    protected virtual void OnSignSuccess()
    {
        Debug.Log($"【人事部】錄取 {employeeName}！附上應徵表與核准章！");
    }
}
