using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float checkDistance = 0.05f;
    [SerializeField] private float footOffset = 0.01f;

    public bool isCanMove = true;
    public bool isCanWalk = true;

    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer sr;
    private Animator anim;
    private PlayerStatus ps;

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
        ps = GetComponent<PlayerStatus>();
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
        if (isCanWalk)
        {
            // 水平移動
            float targetVx = moveInput.x * ps.speed;

            rb.linearVelocity = Vector2.Lerp(
                rb.linearVelocity,
                new Vector2(targetVx, rb.linearVelocity.y),
                0.2f
            );
        }    

        // ジャンプ処理
        if (jumpRequested)
        {
            jumpRequested = false;
            isJumping = true;
            Vector2 vel = rb.linearVelocity;
            vel.y = 0;
            rb.linearVelocity = vel;
            rb.AddForce(Vector2.up * ps.jumpPower, ForceMode2D.Impulse);
        }
    }
}
