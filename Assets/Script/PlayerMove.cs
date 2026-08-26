using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
// キーを押したら、移動する（重力でジャンプ）
public class PlayerMove : MonoBehaviour
{
    //-------------------------------
    public float speed = 4f;
    public float jumpPower = 5f;
    public float checkDistance = 0.1f;
    public float footOffset = 0.01f;
    //--------------------------------
    private Rigidbody2D    rbody;
    private Collider2D     col;
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

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

        // 移動方向にキャラを向ける
        if (moveInput.x != 0)
            sr.flipX = moveInput.x < 0;
    }

    public void OnJump()
    {
        // 地面についていて、ジャンプしていないときだけジャンプのリクエストを受け付ける。
        if (isGrounded && !isJumping)
            jumpRequested = true;
    }

    void Update()
    {
        // 地面判定
        float myHeight = col.bounds.extents.y;
        float footy = transform.position.y - myHeight - footOffset;
        Vector2 startRay = new Vector2(transform.position.x, footy);

        //
        isGrounded = Physics2D.Raycast(
            startRay, 
            Vector2.down, 
            checkDistance, 
            LayerMask.GetMask("Ground")
        );

        // ジャンプ中かどうかを更新（落下し始めたらジャンプ終了）
        if (rbody.linearVelocity.y <= 0)
            isJumping = false;
        }

    void FixedUpdate()
    {
        // 水平方向の速度設定
        float vx = moveInput.x * speed;
        rbody.linearVelocity = new Vector2(vx, rbody.linearVelocity.y);

        // ジャンプ処理
        if (jumpRequested)
        {
            jumpRequested = false;
            isJumping = true;
            rbody.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
    }
}
