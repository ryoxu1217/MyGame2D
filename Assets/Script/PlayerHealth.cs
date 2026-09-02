using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHP = 5;
    public int CurrentHP { get; private set; }

    void Start()
    {
        CurrentHP = maxHP;
    }

    // ダメージ処理
    public void TakeDamage(int amount)
    {
        CurrentHP -= amount;

        // 下限チェック
        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            Die();
        }
    }

    // 回復処理
    public void Heal(int amount)
    {
        CurrentHP += amount;

        // 上限チェック
        if (CurrentHP > maxHP)
            CurrentHP = maxHP;
    }

    private void Die()
    {
        // 死亡処理（テスト）
        Debug.Log("Player Dead");
    }
}
