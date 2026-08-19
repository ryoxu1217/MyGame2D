using UnityEngine;

public class Homing : MonoBehaviour
{
    [SerializeField] private Transform target;

    private float moveSpeed = 3f;   // 足の速さ
    private float climbSpeed = 5f;  // 段差を登るときの移動幅

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    // フリップ安定化用
    [SerializeField] private float flipDeadzone = 0.15f;  // X差がこれ以下なら向きを変えない
    private bool lastTargetIsRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        if (rb != null)
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void FixedUpdate()
    {
        // dx: ターゲットと自分のX差
        float dx = target.position.x - transform.position.x;
        float dy = target.position.y - transform.position.y;

        // 基本の向き（差が十分大きければ即決）
        bool targetIsRightCandidate = dx > 0f;

        // dx, dy, targetIsRightCandidate は既に計算済みとする
        if (Mathf.Abs(dx) > flipDeadzone)
        {
            lastTargetIsRight = targetIsRightCandidate;
        }


        // スプライト反転
        sr.flipX = !lastTargetIsRight;

        // 横速度
        float moveX = lastTargetIsRight ? moveSpeed : -moveSpeed;

        // 段差チェック（元のまま）
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position + Vector3.down * 0.1f,
            lastTargetIsRight ? Vector2.right : Vector2.left,
            0.3f,
            LayerMask.GetMask("Ground")
        );

        float moveY = rb.linearVelocity.y;
        if (hit.collider != null)
        {
            moveY = climbSpeed;
        }

        rb.linearVelocity = new Vector2(moveX, moveY);
    }
}
