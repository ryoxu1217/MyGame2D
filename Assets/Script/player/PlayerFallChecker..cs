using UnityEngine;

public class PlayerFallChecker : MonoBehaviour
{
    [SerializeField] private float deathY = -10f;   // この高さより下に落ちたら死亡
    [SerializeField] private GameObject deadParticlePrefab; // 死エフェクト
    private Rigidbody2D rb;
    private PlayerMove pm;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pm = GetComponent<PlayerMove>();
    }
    
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

        // 移動を禁ずる
        pm.playerCanMove = false;
        rb.linearVelocity = Vector2.zero;  // 速度ゼロ
        rb.constraints = RigidbodyConstraints2D.FreezePosition; // 位置固定
        gameObject.SetActive(false);

        Vector3 pos = transform.position;
        pos.z = -1f;
        Instantiate(deadParticlePrefab, pos, Quaternion.identity);
    }
}
