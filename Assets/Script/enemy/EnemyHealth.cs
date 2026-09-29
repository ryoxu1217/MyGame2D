using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHP = 20;
    public int MaxHP => maxHP;
    public int CurrentHP { get; private set; }
    [SerializeField] private float invincibleTime = 0.5f;
    public bool isInvincible { get; private set; } = false;
    [SerializeField] private GameObject deadParticlePrefab;

    private bool isDead;
    private PlayerStatus ps;
    private EnemyStatus es;
    private Animator anim;

    private void Start()
    {
        CurrentHP = maxHP;
        anim = GetComponent<Animator>();
        es = GetComponent<EnemyStatus>();

        // プレイヤーを Layer で探す（安全版）
        int playerLayer = LayerMask.GetMask("Player");
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 100f, playerLayer);

        if (hits.Length > 0)
        {
            ps = hits[0].GetComponent<PlayerStatus>();
        }
        else
        {
            Debug.LogWarning("EnemyHealth: Player が見つからず ps が null のままです");
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible || ps == null || isDead) return;

        CurrentHP -= damage;
        CurrentHP = Mathf.Clamp(CurrentHP, 0, maxHP);

        Debug.Log($"{gameObject} の現在のHP: {CurrentHP}");

        if (CurrentHP <= 0)
        {
            EnemyDead();
        }
        else
        {
            StartCoroutine(InvincibleCoroutine());
        }
    }

    private IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;
        anim.SetBool("isDamage", true);

        yield return new WaitForSeconds(invincibleTime);

        isInvincible = false;
        anim.SetBool("isDamage", false);
    }

    private void EnemyDead()
    {
        if (isDead) return;
        isDead = true;
        if (ps != null)
        {
            ps.TakeEXP(es.haveEXP);
        }
        else
        {
            Debug.LogWarning("EnemyDead: ps が null のため EXP を渡せませんでした");
        }

        Vector3 pos = transform.position;
        pos.z = -1f;
        Instantiate(deadParticlePrefab, pos, Quaternion.identity);

        Destroy(gameObject);
    }
}
