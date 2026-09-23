using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHP = 20;     // 最大HP
    public int MaxHP => maxHP;  // maxHPの値を常に示す読み取り用変数
    public int CurrentHP { get; private set; }  // 書き換えれないが、値を使うことはできる

    [SerializeField] private float invincibleTime = 0.1f; // 無敵時間（秒）
    private bool isInvincible = false; // 無敵状態かどうか

    [SerializeField] private float knockbackForce = 3f;     // ノックバック強さ
    private Rigidbody2D rb;


    private Animator anim;

    private void Start()
    {
        CurrentHP = maxHP;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
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
        if (isInvincible) return; // 無敵中ならダメージ無効

        CurrentHP -= damage;
        CurrentHP = Mathf.Clamp(CurrentHP, 0, maxHP);

        Debug.Log("現在のHP: " + CurrentHP);

        if (CurrentHP <= 0)
        {
            GameOver();
        }

        StartCoroutine(InvincibleCoroutine()); // 無敵時間開始
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
        GameManager.Instance.OnPlayerDead();
    }
}
