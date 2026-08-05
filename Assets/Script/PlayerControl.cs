using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerControl : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float jumpPower = 8f;

    [Header("Ground Check Settings")]
    public float checkDistance = 0.1f;
    public float footOffset = 0.01f;

    private Rigidbody2D rbody;
    private Collider2D col;
    private SpriteRenderer sr;

    private Vector2 moveInput = Vector2.zero;
    private bool jumpRequested = false;
    private bool isGrounded = false;
    private bool isJumping = false;

    void Awake()
    {
        rbody = GetComponent<Rigidbody2D>();
        rbody.constraints = RigidbodyConstraints2D.FreezeRotation;

        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    // 新 Input System：移動入力
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // 新 Input System：ジャンプ入力
    public void OnJump()
    {
        if (isGrounded && !isJumping)
            jumpRequested = true;
    }

    void Update()
    {
        UpdateGroundCheck();
        UpdateJumpState();
        UpdateSpriteDirection();
    }

    // 地面判定（左右2本のRay）
    void UpdateGroundCheck()
    {
        float myHeight = col.bounds.extents.y;
        float footY = transform.position.y - myHeight - footOffset;

        Vector2 leftFoot = new Vector2(col.bounds.min.x, footY);
        Vector2 rightFoot = new Vector2(col.bounds.max.x, footY);

        bool leftHit = Physics2D.Raycast(leftFoot, Vector2.down, checkDistance);
        bool rightHit = Physics2D.Raycast(rightFoot, Vector2.down, checkDistance);

        isGrounded = leftHit || rightHit;
    }

    // ジャンプ中判定（地面に着いたら解除）
    void UpdateJumpState()
    {
        if (isGrounded)
            isJumping = false;
    }

    // キャラの向きを速度で反転
    void UpdateSpriteDirection()
    {
        if (rbody.linearVelocity.x != 0)
            sr.flipX = rbody.linearVelocity.x < 0;
    }

    void FixedUpdate()
    {
        ApplyHorizontalMovement();
        ApplyJump();
    }

    // AddForce を使った自然な横移動
    void ApplyHorizontalMovement()
    {
        float targetSpeed = moveInput.x * speed;
        float speedDiff = targetSpeed - rbody.linearVelocity.x;

        rbody.AddForce(new Vector2(speedDiff, 0), ForceMode2D.Force);
    }

    // ジャンプ処理
    void ApplyJump()
    {
        if (jumpRequested)
        {
            jumpRequested = false;
            isJumping = true;

            rbody.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
    }

    // SceneビューでRayを可視化
    void OnDrawGizmos()
    {
        if (col == null) return;

        float myHeight = col.bounds.extents.y;
        float footY = transform.position.y - myHeight - footOffset;

        Vector2 leftFoot = new Vector2(col.bounds.min.x, footY);
        Vector2 rightFoot = new Vector2(col.bounds.max.x, footY);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(leftFoot, leftFoot + Vector2.down * checkDistance);
        Gizmos.DrawLine(rightFoot, rightFoot + Vector2.down * checkDistance);
    }
}
