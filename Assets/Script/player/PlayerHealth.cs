using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHP = 20;     // 最大HP
    public int MaxHP => maxHP;  // maxHPの値を常に示す読み取り用変数
    public int CurrentHP { get; private set; }  // 書き換えれないが、値を使うことはできる

    [SerializeField] private float invincibleTime = 0.5f; // 無敵時間（秒）
    public bool isInvincible { get; private set; } = false; // 無敵状態かどうか

    [SerializeField] private GameObject deadParticlePrefab; // 死エフェクト

    private Animator anim;

    private bool isDead = false;

    private void Start()
    {
        CurrentHP = maxHP;
        anim = GetComponent<Animator>();
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

    public void TakeHeal(int heal)
    {
        if (isDead) return; // 死亡中ならダメージ無効

        CurrentHP += heal;
        CurrentHP = Mathf.Clamp(CurrentHP, 0, maxHP);

        Debug.Log("現在のHP: " + CurrentHP);
    }

    private IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;
        anim.SetBool("isDamage", true); // アニメーション開始

        yield return new WaitForSeconds(invincibleTime);

        isInvincible = false;
        anim.SetBool("isDamage", false); // アニメーション終了
    }

    private void GameOver()
    {
        // 死んだ
        isDead = true;

        gameObject.SetActive(false);

        Vector3 pos = transform.position;
        pos.z = -1f;
        Instantiate(deadParticlePrefab, pos, Quaternion.identity);
    }
}
