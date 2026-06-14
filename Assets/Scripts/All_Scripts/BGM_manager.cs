using UnityEngine;



[RequireComponent(typeof(AudioSource))]
public class BGM_manager : MonoBehaviour
{
    // ★ 單例模式：讓全場景的人都能直接叫他，不用慢慢找
    public static BGM_manager instance;

    [Header("音樂錄音帶")]
    public AudioClip normalBGM; // 拖入一般房間音樂
    public AudioClip bossBGM;   // 拖入 BOSS 戰音樂

    private AudioSource audioSource;

    void Awake()
    {
        // 單例模式的安全設定
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 抓取自己身上的喇叭組件
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        // 遊戲一開始，預設播放一般房間音樂
        if (normalBGM != null)
        {
            audioSource.clip = normalBGM;
            audioSource.Play();
        }
    }

    // ★ 開放給 BOSS 呼叫的切換音樂功能
    public void SwitchToBossMusic()
    {
        // 檢查如果現在已經在播 BOSS 音樂了，就不要重複切換（防呆）
        if (audioSource.clip == bossBGM) return;

        audioSource.Stop();       // 先停止目前的音樂
        audioSource.clip = bossBGM; // 換上 BOSS 的錄音帶
        audioSource.Play();       // 重新按下播放鍵
    }
}