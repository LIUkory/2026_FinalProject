using UnityEngine;

public class EnemyAI : BaseEnemyAI
{
    protected override void ExecuteAttack()
    {
        if (player == null) return;

        // 1. 動態無差別強扣血防線（利用之前抓好的 playerObj 傳送訊息）
        player.gameObject.SendMessage("TakeDamage", damageAmount, SendMessageOptions.DontRequireReceiver);

        // 2. 老鼠專屬：生成帶有朝向主角角度的撕裂爪子特效
        if (attackEffectPrefab != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            float offsetDistance = 2.8f; 
            Vector3 spawnPos = transform.position + (Vector3)direction * offsetDistance;
            spawnPos.z = 0f; 

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            GameObject effect = Instantiate(attackEffectPrefab, spawnPos, rotation);
            Destroy(effect, 0.5f);
            
            Debug.Log($"<color=gray>【老鼠撕裂】</color> 爪擊了 {player.name}！");
        }
    }
}