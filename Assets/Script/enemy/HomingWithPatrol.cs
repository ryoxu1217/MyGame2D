using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class HomingWithPatrol : MonoBehaviour
{
    public enum State { Patrol, Pause, Homing }

    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private string targetTag = "Player";

    [Header("Patrol")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float waypointReachThreshold = 0.2f;   //　ウェイポイント到達判定距離
    [SerializeField] private bool loopWaypoints = true;     // ウェイポイントを全て行き終わってからループするか

    [Header("Homing")]
    [SerializeField] private float climbSpeed = 4f;     // さかのぼり速度
    [SerializeField] private float detectionRange = 5f;     // ターゲット補足範囲
    [SerializeField] private float loseRange = 7f;  // ターゲットを見失う範囲（detectionRangeより必ず大きく）
    [SerializeField] private float flipDeadzone = 0.15f;
    [SerializeField] private float flipSmooth = 0.08f; // 0.05〜0.2が自然。

    [Header("Pause Before Homing")]
    [SerializeField] private float pauseDuration = 0.6f;    // フューズ時間

    [Header("Physics")]
    [SerializeField] private LayerMask groundLayer = 1 << 3;    // 地面と判定するレイヤー

    private Rigidbody2D rb;     // 物理演算
    private SpriteRenderer sr;      // スプライト描画
    private Animator anim;      // アニメーション
    private EnemyHealth eh;
    private EnemyStatus es;
    private State state = State.Patrol;     // ステート
    private int currentWaypoint = 0;    // 巡回位置
    private Vector2 randomDir = Vector2.right;
    private float randomMoveTimer = 0f;
    private float randomMoveInterval = 1.0f;
    private bool lastTargetIsRight = true;  // ターゲットが右方向にいるか
    private float flipValue = 1f; // 右=1, 左=0 の連続値
    private float pauseTimer = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        anim.SetBool("isWalk", true);
        eh = GetComponent<EnemyHealth>();
        es = GetComponent<EnemyStatus>();
    }

    void Start()
    {
        if (target == null && !string.IsNullOrEmpty(targetTag))
        {
            GameObject found = GameObject.FindWithTag(targetTag);
            if (found != null) target = found.transform;
        }

        if (waypoints == null || waypoints.Length == 0)
        {
            randomDir = Random.value > 0.5f ? Vector2.right : Vector2.left;
            randomMoveTimer = Random.Range(0f, randomMoveInterval);
        }
    }

    void FixedUpdate()
    {
        //プレイヤーとの距離を測る
        float distanceToTarget = target != null ? Vector2.Distance(transform.position, target.position) : Mathf.Infinity;

        // Patrol → Pause
        if (state == State.Patrol && distanceToTarget <= detectionRange)
        {
            StartPauseBeforeHoming();
        }
        // Homing → Patrol
        else if (state == State.Homing && distanceToTarget > loseRange)
        {
            state = State.Patrol;
        }

        // switch文でステートごとの処理を変える
        switch (state)
        {
            case State.Patrol: DoPatrol(); break;
            case State.Pause: UpdatePause(distanceToTarget); break;
            case State.Homing: DoHoming(); break;
        }

    }

    private void StartPauseBeforeHoming()
    {
        state = State.Pause;
        pauseTimer = pauseDuration;

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (target != null)
        {
            float dx = target.position.x - transform.position.x;
            if (Mathf.Abs(dx) > flipDeadzone) lastTargetIsRight = dx > 0f;
            sr.flipX = !lastTargetIsRight;
        }
    }

    private void UpdatePause(float distanceToTarget)
    {
        if (distanceToTarget > loseRange)
        {
            state = State.Patrol;
            return;
        }

        pauseTimer -= Time.fixedDeltaTime;
        if (pauseTimer <= 0f)
        {
            state = State.Homing;

            rb.linearVelocity = new Vector2(
                lastTargetIsRight ? es.homingSpeed : -es.homingSpeed,
                rb.linearVelocity.y
            );
        }
    }

    private void DoPatrol()
    {
        float vx = 0f;

        if (waypoints != null && waypoints.Length > 0)  // ウェイポイントが存在するなら
        {
            Transform wp = waypoints[currentWaypoint];
            Vector2 dir = wp.position - transform.position;
            float dist = dir.magnitude;

            if (dist <= waypointReachThreshold)
            {
                currentWaypoint++;
                if (currentWaypoint >= waypoints.Length)
                    currentWaypoint = loopWaypoints ? 0 : waypoints.Length - 1;
            }
            else
            {
                vx = dir.normalized.x * es.patrolSpeed;
            }
        }
        else    //ウェイポイントがなければ
        {
            randomMoveTimer -= Time.fixedDeltaTime;
            if (randomMoveTimer <= 0f)
            {
                randomDir = Random.value > 0.5f ? Vector2.right : Vector2.left;
                randomMoveTimer = randomMoveInterval;
            }
            vx = randomDir.x * es.patrolSpeed;
        }

        if (Mathf.Abs(vx) > 0.01f) lastTargetIsRight = vx > 0f;
        sr.flipX = !lastTargetIsRight;

        // 移動を上書きせず、滑らかに動かす（目標地点に向かって移動）
        rb.linearVelocity = Vector2.Lerp(
            rb.linearVelocity,
            new Vector2(vx, rb.linearVelocity.y),
            0.2f
        );
    }

    private void DoHoming()
    {
        // ターゲットがいないならリターン
        if (target == null) return;
        float moveY = 0;
        float moveX = 0;

        bool grounded = rb.IsTouchingLayers(groundLayer);
        
        if (grounded || !eh.isInvincible)
        {
            float dx = target.position.x - transform.position.x;
            float targetVal = dx > 0f ? 1f : 0f;

            // デッドゾーン（小刻み反転防止）
            if (Mathf.Abs(dx) > flipDeadzone)
            {
                flipValue = Mathf.Lerp(flipValue, targetVal, flipSmooth);
            }

            lastTargetIsRight = flipValue > 0.5f;

            sr.flipX = !lastTargetIsRight;

            moveX = lastTargetIsRight ? es.homingSpeed : -es.homingSpeed;
            Vector2 dir = lastTargetIsRight ? Vector2.right : Vector2.left;

            // 段差判定
            float forwardDist = 0.3f;      // 前方の障害物検出距離
            float stepMaxHeight = 0.4f;    // 登れる最大段差高さ

            // 前方に障害物があるか
            Vector2 footOrigin = (Vector2)transform.position + Vector2.down * 0.1f;
            RaycastHit2D forwardHit = Physics2D.Raycast(
                footOrigin,
                dir,
                forwardDist,
                groundLayer
            );

            moveY = rb.linearVelocity.y;

            if (forwardHit.collider != null)
            {
                // 障害物の上に足場があるか確認
                Vector2 topCheckOrigin = forwardHit.point + Vector2.up * stepMaxHeight;
                RaycastHit2D topHit = Physics2D.Raycast(
                    topCheckOrigin,
                    Vector2.down,
                    stepMaxHeight + 0.1f,
                    groundLayer
                );

                if (topHit.collider != null)
                {
                    // 足場の高さ差が登れる範囲か
                    float topY = topHit.point.y;
                    float myFootY = transform.position.y - 0.1f; // 足元の高さ

                    float heightDiff = topY - myFootY;

                    if (heightDiff > 0f && heightDiff <= stepMaxHeight)
                    {
                        // 段差と判断して登る
                        moveY = climbSpeed;
                    }
                }
            }
        }

        rb.linearVelocity = Vector2.Lerp(
        rb.linearVelocity,
        new Vector2(moveX, moveY),
        0.2f
        );
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, loseRange);

        if (waypoints != null && waypoints.Length > 0)
        {
            Gizmos.color = Color.green;
            foreach (var wp in waypoints)
            {
                if (wp != null) Gizmos.DrawSphere(wp.position, 0.05f);
            }
        }
    }
}
