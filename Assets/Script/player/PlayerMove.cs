using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerMove : MonoBehaviour
{
    public float speed = 4f;
    public float jumpPower = 6f;
    public float checkDistance = 0.05f;
    public float footOffset = 0.01f;

    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer sr;
    private Animator anim;

    private Vector2 moveInput = Vector2.zero;
    private bool jumpRequested = false;
    private bool isGrounded = false;
    private bool isJumping = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

        // 左右向き
        if (moveInput.x != 0)
            sr.flipX = moveInput.x < 0;

        // 歩きアニメ
        anim.SetBool("isWalk", moveInput.x != 0);

        if (!isGrounded || isJumping)
        {
            anim.SetBool("isFall", true);    
        } else
        {
            anim.SetBool("isFall", false);  
        }

        // 上キーでジャンプ要求
        if (moveInput.y > 0 && isGrounded && !isJumping)
            jumpRequested = true;
    }

    void Update()
    {
        // 地面判定
        float myHeight = col.bounds.extents.y;
        float footy = transform.position.y - myHeight - footOffset;
        Vector2 startRay = new Vector2(transform.position.x, footy);

        isGrounded = Physics2D.Raycast(
            startRay,
            Vector2.down,
            checkDistance,
            LayerMask.GetMask("Ground")
        );

        anim.SetBool("isFall", !isGrounded);

        // 落下し始めたらジャンプ終了
        if (rb.linearVelocity.y <= 0)
            isJumping = false;
    }

    void FixedUpdate()
    {
        // 水平移動
        float vx = moveInput.x * speed;
        rb.linearVelocity = new Vector2(vx, rb.linearVelocity.y);

        // ジャンプ処理
        if (jumpRequested)
        {
            jumpRequested = false;
            isJumping = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); 
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
    }
}
