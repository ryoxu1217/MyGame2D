using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerKickAttack : MonoBehaviour
{
    [SerializeReference] private float kickRange = 1.2f;
    [SerializeReference] private float kickHeight = 1.2f;
    public float kickForce = 10f;
    public float cooldown = 1.0f;
    public bool isCooldown{get; private set;} = false;
    public float cooldownTimer{get; private set;} = 0f;


    public GameObject kickParticlePrefab;
    private PlayerRollingAttack pra;
    private SpriteRenderer sr;
    private Animator anim;
    private PlayerStatus ps;

    private int enemyLayer;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        pra = GetComponent<PlayerRollingAttack>();
        ps = GetComponent<PlayerStatus>();
        enemyLayer = LayerMask.GetMask("Enemy");
    }

    void Update()
    {
        // クールダウン中ならタイマーを減らす
        if (isCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                cooldownTimer = 0f;
                isCooldown = false;
            }
        }
    }

    public void OnKick(InputValue value)
    {
        if (!isCooldown && !pra.isRolling)    // 蹴れる状態なら
            PerformKick();
    }

    private void PerformKick()
    {
        isCooldown = true;
        cooldownTimer = cooldown;    // cooltime発動

        float dir = sr.flipX ? -1f : 1f;    // 向きから攻撃方法を確認

        // アニメ再生
        anim.SetTrigger("KickTriggr");    // アタックアニメーションを再生

        // 判定位置
        Vector2 center = new Vector2(
            transform.position.x + dir * (kickRange * 0.8f),
            transform.position.y + kickHeight * 0.05f
        );

        Vector2 size = new Vector2(kickRange, kickHeight);
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f, enemyLayer);

        foreach (var hit in hits)
        {
            // Rigidbody2Dを取得
            if (hit.TryGetComponent(out Rigidbody2D er))
            {
                // ノックバック
                Vector2 force = new Vector2(dir * kickForce, kickForce * 0.2f);
                er.AddForce(force, ForceMode2D.Impulse);
            }

            // TryGetComponentというのはちょっと処理の軽いGetComponentみたいなヤツ
            if (hit.TryGetComponent(out EnemyHealth enemy))
            {
                enemy.TakeDamage(ps.AttackDamage);
            }
        }

        StartCoroutine(DelayedKickParticle(dir, 0.1f));
    }

    
    private System.Collections.IEnumerator DelayedKickParticle(float dir, float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnKickParticle(dir);
    }
    private void SpawnKickParticle(float dir)
    {
        if (kickParticlePrefab == null) return;

        Vector3 pos = new Vector3(
            transform.position.x + dir * 0.6f,
            transform.position.y + 0.3f,
            -1f
        );

        Instantiate(kickParticlePrefab, pos, Quaternion.identity);
    }

    private void OnDrawGizmosSelected()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();

        float dir = sr.flipX ? -1f : 1f;

        Vector2 center = new Vector2(
            transform.position.x + dir * (kickRange * 0.8f),
            transform.position.y + kickHeight * 0.05f
        );

        Vector2 size = new Vector2(kickRange, kickHeight);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, size);
    }
}
