using UnityEngine;
using UnityEngine.InputSystem;

public class KickAttack : MonoBehaviour
{
    [Header("Kick Settings")]
    public float kickRange = 1.2f;
    public float kickHeight = 0.8f;
    public float kickForce = 10f;
    public float cooldown = 1.0f;

    [Header("Effects")]
    public GameObject kickEffectPrefab;   // ← パーティクルプレハブ

    private bool canKick = true;
    private SpriteRenderer sr;
    private Animator anim;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    public void OnKick(InputValue value)
    {
        if (!canKick) return;
        PerformKick();
    }

    private void PerformKick()
    {
        canKick = false;

        float dir = sr.flipX ? -1f : 1f;

        // animation再生
        anim.SetTrigger("AttackTriggr"); 
        
        // 判定位置
        Vector2 center = new Vector2(
            transform.position.x + dir * (kickRange * 0.5f),
            transform.position.y + kickHeight * 0.5f
        );

        Vector2 size = new Vector2(kickRange, kickHeight);

        // 敵判定
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f, LayerMask.GetMask("Enemy"));

        foreach (var hit in hits)
        {
            Rigidbody2D er = hit.GetComponent<Rigidbody2D>();

            if (er != null)
            {
                Vector2 force = new Vector2(dir * kickForce, kickForce * 0.7f);
                er.AddForce(force, ForceMode2D.Impulse);

                    // EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
                    // if (enemy != null)
                    // {
                    //     enemy.TakeDamage(1);
                    // }
            }
        }

        // パーティクル発生
        SpawnKickEffect(dir);

        // クールダウン
        StartCoroutine(KickCooldown());
    }

    private void SpawnKickEffect(float dir)
    {
        if (kickEffectPrefab == null) return;

        Vector3 pos = new Vector3(
            transform.position.x + dir * 0.6f,
            transform.position.y + 0.3f,
            -1f
        );

        Instantiate(kickEffectPrefab, pos, Quaternion.identity);
    }

    private System.Collections.IEnumerator KickCooldown()
    {
        yield return new WaitForSeconds(cooldown);
        canKick = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();

        float dir = sr.flipX ? -1f : 1f;

        Vector2 center = new Vector2(
            transform.position.x + dir * (kickRange * 0.5f),
            transform.position.y + kickHeight * 0.5f
        );

        Vector2 size = new Vector2(kickRange, kickHeight);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, size);
    }
}
