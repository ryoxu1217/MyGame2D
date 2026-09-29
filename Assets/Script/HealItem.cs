using UnityEngine;

public class HealItem : MonoBehaviour
{
    [SerializeField] private int healAmount = 5;          // 回復量
    [SerializeField] private GameObject healParticle;     // 回復エフェクト

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth ph = other.GetComponent<PlayerHealth>();

        if (ph != null)
        {
            // 回復
            ph.TakeHeal(healAmount);

            // パーティクルが設定されていれば再生
            if (healParticle != null)
            {
                Vector3 pos = transform.position;
                pos.z = -1f; // 手前に出す（必要なければ削除OK）
                Instantiate(healParticle, pos, Quaternion.identity);
            }

            // 自分を消す
            Destroy(gameObject);
        }
    }
}
