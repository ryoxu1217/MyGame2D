using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHP = 20;     // 最大HP
    public int MaxHP => maxHP;  // maxHPの値を常に示す読み取り用変数
    public int CurrentHP { get; private set; }  // 書き換えれないが、値を使うことはできる

    [SerializeField] private float invincibleTime = 0.1f; // 無敵時間（秒）
    private bool isInvincible = false; // 無敵状態かどうか

    [SerializeField] private float knockbackForce = 1f;     // ノックバック強さ

    [SerializeField] private GameObject deadParticlePrefab; // 死エフェクト

    private Rigidbody2D rb;
    private PlayerMove pm;
    private Animator anim;

    private bool isDead = false;

    private void Start()
    {
        CurrentHP = maxHP;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        pm = GetComponent<PlayerMove>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        int enemyLayer = LayerMask.NameToLayer("Enemy");

        if (collision.gameObject.layer == enemyLayer)
        {
            TakeDamage(1);

            // ノックバック発生
            Vector2 direction = (transform.position - collision.transform.position).normalized;
            rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        }
    }


    public void TakeDamage(int damage)
    {
        
        if (isInvincible || isDead) return; // 無敵中 or 死亡中 ならダメージ無効

        CurrentHP -= damage;
        CurrentHP = Mathf.Clamp(CurrentHP, 0, maxHP);

        Debug.Log("現在のHP: " + CurrentHP);

        if (CurrentHP <= 0)
        {
            GameOver();
        } else {
            StartCoroutine(InvincibleCoroutine()); // 無敵時間開始
        }

    }

    private IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;
        anim.SetBool("isDamage", true); // ← アニメーション開始

        yield return new WaitForSeconds(invincibleTime);

        isInvincible = false;
        anim.SetBool("isDamage", false); // ← アニメーション終了
    }

    private void GameOver()
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
