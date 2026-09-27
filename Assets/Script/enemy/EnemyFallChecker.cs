using UnityEngine;

public class EnemyFallChecker : MonoBehaviour
{
    [SerializeField] private float deathY = -10f;   // この高さより下に落ちたら死亡
    [SerializeField] private GameObject deadParticlePrefab; // 死エフェクト
    
    void Update()
    {
        // プレイヤーのY座標が deathY より下なら死亡
        if (transform.position.y < deathY)
        {
            OnFallDead();
        }
    }

    private void OnFallDead()
    {
        // DeathParticle
        Vector3 pos = transform.position;
        pos.z = -1f;
        Instantiate(deadParticlePrefab, pos, Quaternion.identity);


        // 削除
        Destroy(this.gameObject);
    }
}
