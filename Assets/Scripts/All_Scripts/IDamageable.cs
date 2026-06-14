public interface IDamageable
{
    // 只要繼承這個介面的東西，都必須實作「受傷」這個動作
    void TakeDamage(float damage);
}