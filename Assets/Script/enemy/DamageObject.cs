using UnityEngine;

public class DamageObject : MonoBehaviour
{
    public int damage = 1;
    public LayerMask targetLayer; // ← ここで「ダメージを与えるレイヤー」を指定

    private void OnTriggerEnter2D(Collider2D other)
    {
        // other のレイヤーが targetLayer に含まれているか判定
        if ((targetLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            PlayerHealth hp = other.GetComponent<PlayerHealth>();
            if (hp != null)
            {
            }
        }
    }
}
