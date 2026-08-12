using UnityEngine;

public class Homing : MonoBehaviour
{
    [SerializeField] private Transform target;

    private float moveSpeed = 3f;   //　足の速さ
    private float climbSpeed = 2f;  // 段差を登るときの移動幅

    // RigidbodyとSpriteRendererを取得するための変数
    private Rigidbody2D rb; 
    private SpriteRenderer sr;

    private void Start() // 本物と同期させる。本物を動かすリモコンのように使えるらしい。
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate() // 一定間隔で実行
    {
        // 左右判定（ターゲットが自分より右にいるか左にいるかで判定）
        bool targetIsRight = target.position.x > transform.position.x;
        sr.flipX = !targetIsRight; // true → false | false → true

        // 横方向の速度は一定にする
        float moveX = targetIsRight ? moveSpeed : -moveSpeed;

        // 段差チェック
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position + Vector3.down * 0.1f,
            targetIsRight ? Vector2.right : Vector2.left,0.3f,
            LayerMask.GetMask("Ground")
        );

        float moveY = rb.linearVelocity.y;

        // 段差にぶつかったら少し上に押し上げる
        if (hit.collider != null) // (「Raycastが当たったコライダーの記録」に何か入っているなら)
        {
            moveY = climbSpeed;
        }

        // 速度を直接設定
        rb.linearVelocity = new Vector2(moveX, moveY);

        // 回転固定（オブジェクトの向きを消し去る）
        transform.rotation = Quaternion.identity; // Quaternion.identityはX, Y, Zがすべてゼロの回転の値
    }
}
