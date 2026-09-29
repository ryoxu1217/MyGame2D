using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerMove : MonoBehaviour
{
    // 足元の地面判定に使う距離とオフセット
    [SerializeField] private float checkDistance = 0.2f;
    [SerializeField] private float footOffset = 0.01f;

    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer sr;
    private Animator anim;
    private PlayerStatus ps;
    private Vector2 moveInput = Vector2.zero;   // 入力ベクトル（左右＋上下）
    private bool jumpRequested = false;     // ジャンプ要求フラグ
    private bool isGrounded = false;    // 地面にいるかどうか
    private bool isJumping = false; // ジャンプ中かどうか
    public bool isCanWalk = true;
    // 段差登り用パラメータ
    [SerializeField] private float forwardDist = 0.8f;     // 前方の障害物検出距離
    [SerializeField] private float stepMaxHeight = 0.4f;   // 登れる最大段差高さ
    [SerializeField] private float climbSpeed = 6f;        // 段差を登るときの上方向速度

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        ps = GetComponent<PlayerStatus>();
    }

    // 入力処理（Input System）
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

        // 左右向きの反転
        if (moveInput.x != 0)
            sr.flipX = moveInput.x < 0;

        // アニメ
        anim.SetBool("isWalk", moveInput.x != 0);
        if (!isGrounded || isJumping)
            anim.SetBool("isFall", true);
        else
            anim.SetBool("isFall", false);

        // 上方向入力でジャンプ要求
        if (moveInput.y > 0 && isGrounded && !isJumping)
            jumpRequested = true;
    }

    void Update()
    {
        // 地面判定
        float myHeight = col.bounds.extents.y;
        float footY = transform.position.y - myHeight - footOffset;
        Vector2 startRay = new Vector2(transform.position.x, footY);

        isGrounded = Physics2D.Raycast(
            startRay,
            Vector2.down,
            checkDistance,
            LayerMask.GetMask("Ground","Enemy")
        );

        anim.SetBool("isFall", !isGrounded);

        // 上方向速度が 0 以下になったらジャンプ終了
        if (rb.linearVelocity.y <= 0)
            isJumping = false;
    }

    void FixedUpdate()
    {
        float moveX = moveInput.x * ps.speed;   // 入力に応じた水平速度
        float moveY = rb.linearVelocity.y;  // 現在の縦方向速度

        // 段差登り判定（左右入力があり、地面にいるとき）
        if (moveInput.x != 0 && isGrounded)
        {
            // 左右方向
            Vector2 dir = moveInput.x > 0 ? Vector2.right : Vector2.left;

            // 足元から前方に Raycast → 障害物検出
            Vector2 footOrigin = (Vector2)transform.position + Vector2.down * 0.1f;
            RaycastHit2D forwardHit = Physics2D.Raycast(
                footOrigin,
                dir,
                forwardDist,
                LayerMask.GetMask("Ground")
            );

            if (forwardHit.collider != null)
            {
                // 障害物の上に足場があるか確認
                Vector2 topCheckOrigin = forwardHit.point + Vector2.up * stepMaxHeight;
                RaycastHit2D topHit = Physics2D.Raycast(
                    topCheckOrigin,
                    Vector2.down,
                    stepMaxHeight + 0.1f,
                    LayerMask.GetMask("Ground")
                );

                if (topHit.collider != null)
                {
                    // 足場の高さ差を計算
                    float topY = topHit.point.y;
                    float myFootY = transform.position.y - 0.1f;
                    float heightDiff = topY - myFootY;

                    // 登れる段差なら上方向速度を climbSpeed にする
                    if (heightDiff > 0f && heightDiff <= stepMaxHeight)
                    {
                        moveY = climbSpeed;
                    }
                }
            }
        }

        if (isCanWalk)
        {
            // 現在の速度と目標速度を補間して滑らかに移動
            rb.linearVelocity = Vector2.Lerp(
                rb.linearVelocity,
                new Vector2(moveX, moveY),
                0.2f
            );
        }

        // ジャンプ処理
        if (jumpRequested)
        {
            jumpRequested = false;
            isJumping = true;

            // 縦速度をリセットしてからジャンプ力を加える
            Vector2 vel = rb.linearVelocity;
            vel.y = 0;
            rb.linearVelocity = vel;

            rb.AddForce(Vector2.up * ps.jumpPower, ForceMode2D.Impulse);
        }
    }
}
