using UnityEngine;

public class PlayerFallChecker : MonoBehaviour
{
    [SerializeField] private float deathY = -10f;   // この高さより下に落ちたら死亡
    [SerializeField] private GameObject deadParticlePrefab; // 死エフェクト
    private bool isDead = false;

    void Update()
    {
        // すでに死んでいたら何もしない
        if (isDead) return;

        // プレイヤーのY座標が deathY より下なら死亡
        if (transform.position.y < deathY)
        {
            isDead = true;
            OnFallDead();
        }
    }

    private void OnFallDead()
    {
        // 死んだ
        isDead = true;
        gameObject.SetActive(false);

        Vector3 pos = transform.position;
        pos.z = -1f;
        Instantiate(deadParticlePrefab, pos, Quaternion.identity);
    }
}
