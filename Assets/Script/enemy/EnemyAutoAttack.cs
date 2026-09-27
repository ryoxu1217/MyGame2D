using UnityEngine;

public class EnemyAutoAttack : MonoBehaviour
{
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float attackHeight = 1.0f;
    [SerializeField] private float attackForce = 8f;
    [SerializeField] private float cooldown = 1.5f;
    [SerializeField] private float ParticleDelay;
    [SerializeField] private GameObject attackParticlePrefab;

    private bool isCooldown = false;
    private float cooldownTimer = 0f;

    private Animator anim;
    private SpriteRenderer sr;
    private EnemyStatus es;

    private int playerLayer;

    void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        es = GetComponent<EnemyStatus>();

        playerLayer = LayerMask.GetMask("Player");
    }

    void Update()
    {
        // クールダウン処理
        if (isCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                cooldownTimer = 0f;
                isCooldown = false;
            }
        }

        AutoAttackCheck();
    }

    private void AutoAttackCheck()
    {
        if (isCooldown) return;

        float dir = sr.flipX ? -1f : 1f;

        Vector2 center = new Vector2(
            transform.position.x + dir * (attackRange * 0.8f),
            transform.position.y + attackHeight * 0.05f
        );

        Vector2 size = new Vector2(attackRange, attackHeight);

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f, playerLayer);

        if (hits.Length > 0)
        {
            PerformAttack(dir, hits);
        }
    }

    private void PerformAttack(float dir, Collider2D[] hits)
    {
        isCooldown = true;
        cooldownTimer = cooldown;

        anim.SetTrigger("AttackTrigger");

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Rigidbody2D prb))
            {
                Vector2 force = new Vector2(dir * attackForce, attackForce * 0.2f);
                prb.AddForce(force, ForceMode2D.Impulse);
            }

            if (hit.TryGetComponent(out PlayerHealth player))
            {
                player.TakeDamage(es.AttackDamage);
            }
        }
         StartCoroutine(DelayedAttackParticle(dir, 0.1f));
    }

    private System.Collections.IEnumerator DelayedAttackParticle(float dir, float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnattackParticle(dir);
    }

    private void SpawnattackParticle(float dir)
    {
        if (attackParticlePrefab == null) return;

        Vector3 pos = new Vector3(
            transform.position.x + dir * 0.6f,
            transform.position.y + 0.3f,
            -1f
        );

        Instantiate(attackParticlePrefab, pos, Quaternion.identity);
    }

    private void OnDrawGizmosSelected()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();

        float dir = sr.flipX ? -1f : 1f;

        Vector2 center = new Vector2(
            transform.position.x + dir * (attackRange * 0.8f),
            transform.position.y + attackHeight * 0.05f
        );

        Vector2 size = new Vector2(attackRange, attackHeight);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, size);
    }
}
