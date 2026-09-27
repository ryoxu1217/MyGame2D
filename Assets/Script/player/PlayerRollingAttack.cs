using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class PlayerRollingAttack : MonoBehaviour
{
    // ローリング設定
    [SerializeReference] private float damageInterval = 0.2f;
    [SerializeReference] private float rollRange = 1.5f;
    [SerializeReference] private float rollHeight = 1.5f;
    [SerializeReference] private float knockbackForce = 5f;

    // 時間
    public float rollDuration = 3f;
    public float cooldown = 10f;

    // ローリング
    public bool isRolling{get; private set;} = false;
    public bool isCooldown{get; private set;} = false;
    private float rollTimer = 0f;
    // タイマー
    private float damageTimer = 0f;
    public float cooldownTimer{get; private set;} = 0f;
    // 参照
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Animator anim;
    private PlayerHealth ph;
    private PlayerStatus ps;
    private PlayerMove pm;
    private int enemyLayer;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ph = GetComponent<PlayerHealth>();
        pm = GetComponent<PlayerMove>();
        ps = GetComponent<PlayerStatus>();
        enemyLayer = LayerMask.GetMask("Enemy");
    }
    public void OnRoll(InputValue value)
    {
        if (isRolling || isCooldown) return;
        StartRolling();
    }

    private void StartRolling()
    {
        isRolling = true;
        pm.isCanWalk = false;
        rollTimer = rollDuration;
        anim.SetBool("isRolling", true);
    }

    private void StopRolling()
    {
        pm.isCanWalk = true;
        isRolling = false;
        anim.SetBool("isRolling", false);

        isCooldown = true;
        cooldownTimer = cooldown;
    }

    void Update()
    {
        float dir = sr.flipX ? -1f : 1f;

        if (isRolling)
        {
            if (ph.isInvincible)
            {
                StopRolling();
                return;
            }
            
            rollTimer -= Time.deltaTime;

            float targetX = dir * (ps.speed * 2 - ps.speed / 3);
    ;
            rb.linearVelocity = Vector2.Lerp(
                rb.linearVelocity,
                new Vector2(targetX, rb.linearVelocity.y),
                0.2f
            );

            damageTimer -= Time.deltaTime;
            if (damageTimer <= 0f)
            {
                damageTimer = damageInterval;
                ApplyRollingDamage(dir);
            }

            if (rollTimer <= 0f)
            {
                StopRolling();
            }
        }

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

    private void ApplyRollingDamage(float dir)
    {
        Vector2 center = new Vector2(
            transform.position.x + dir * (rollRange * 0.5f),
            transform.position.y
        );

        Vector2 size = new Vector2(rollRange, rollHeight);

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f, enemyLayer);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out EnemyHealth enemy))
            {
                enemy.TakeDamage(ps.AttackDamage);
            }

            if (hit.TryGetComponent(out Rigidbody2D er))
            {
                Vector2 force = new Vector2(dir * knockbackForce, knockbackForce * 0.2f);
                er.AddForce(force, ForceMode2D.Impulse);
            }
        }
    }
}
