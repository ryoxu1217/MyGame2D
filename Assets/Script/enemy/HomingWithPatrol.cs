using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))] // Rigidbody2DとSpriteRendererが必ず存在することを保証
public class HomingWithPatrol : MonoBehaviour
{
    public enum State { Patrol, Homing } // enum = 始めに指定した型のみを入れることが可能。今回はPatrolとHoming。

    [Header("Target")]
    [SerializeField] private Transform target;  // 追跡のターゲット
    [SerializeField] private string targetTag = "Player";   // フォールバック

    [Header("Patrol")]
    [SerializeField] private Transform[] waypoints;     // 空ならランダム徘徊
    [SerializeField] private float patrolSpeed = 2f;    // パトロール時のスピード
    [SerializeField] private float waypointReachThreshold = 0.2f;   // ウェイポイントに到達したと判断する距離
    [SerializeField] private bool loopWaypoints = true;     // 全てのウェイポイントに到達したときにループするか

    [Header("Homing")]
    [SerializeField] private float homingSpeed = 3f;    // ホーミング時のスピード
    [SerializeField] private float climbSpeed = 2f;     //段差を上がるスピード
    [SerializeField] private float detectionRange = 5f;   // 追跡開始距離
    [SerializeField] private float loseRange = 7f;        // 追跡解除距離（detectionRangeより大きめに）
    [SerializeField] private float flipDeadzone = 0.15f;  // 向き切替無効距離
    
    [Header("Pause Before Homing")]
    [SerializeField] private float pauseDuration = 0.6f; // ホーミング開始前に立ち止まる時間

    [Header("Physics")]
    [SerializeField] private LayerMask groundLayer = 1 << 3; // Groundレイヤーをセット

    // 内部
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private State state = State.Patrol;    // 現在の状態
    private int currentWaypoint = 0;
    private Vector2 randomDir = Vector2.right;  // ウェイポイントが無いときのランダム移動方向
    private float randomMoveTimer = 0f;     // ランダム方向を切り替えるタイマー。
    private float randomMoveInterval = 1.0f;
    private bool lastTargetIsRight = true;  // 最後に向いていた方向（右向きかどうか）。
    

    // Pause 用
    private bool isPausing = false;
    private float pauseTimer = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        if (rb != null) rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Start()
    {
        // target が未割当ならタグで探す
        if (target == null && !string.IsNullOrEmpty(targetTag))
        {
            GameObject found = GameObject.FindWithTag(targetTag);
            if (found != null) target = found.transform;
        }

        // waypoints が空ならランダム方向を初期化
        if (waypoints == null || waypoints.Length == 0)
        {
            randomDir = Random.value > 0.5f ? Vector2.right : Vector2.left;
            randomMoveTimer = Random.Range(0f, randomMoveInterval);
        }
    }

    void FixedUpdate()
    {
        float distanceToTarget = target != null ? Vector2.Distance(transform.position, target.position) : Mathf.Infinity;

        // 状態遷移と Pause トリガー
        if (state == State.Patrol)
        {
            if (!isPausing && distanceToTarget <= detectionRange)
            {
                // Patrol -> ホーミングに移る前に一時停止する
                StartPauseBeforeHoming();
            }
        }
        else if (state == State.Homing)
        {
            if (distanceToTarget > loseRange)
            {
                state = State.Patrol;
            }
        }

        // Pause 中の処理（優先）
        if (isPausing)
        {
            UpdatePause(distanceToTarget);
            return; // Pause 中は他の行動をしない
        }

        // 通常の状態ごとの挙動
        if (state == State.Patrol)
            DoPatrol();
        else
            DoHoming();
    }

    // Pause 開始
    private void StartPauseBeforeHoming()
    {
        isPausing = true;
        pauseTimer = pauseDuration;

        // 横方向の速度を止める（縦は保持）
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        // プレイヤーの方向を向く（瞬時に向ける）
        if (target != null)
        {
            float dx = target.position.x - transform.position.x;
            if (Mathf.Abs(dx) > flipDeadzone) lastTargetIsRight = dx > 0f;
            sr.flipX = !lastTargetIsRight;
        }
    }

    // Pause 更新
    private void UpdatePause(float distanceToTarget)
    {
        // ターゲットが遠くなったらキャンセルして Patrol に戻る
        if (distanceToTarget > loseRange)
        {
            isPausing = false;
            state = State.Patrol;
            return;
        }

        pauseTimer -= Time.fixedDeltaTime;
        if (pauseTimer <= 0f)
        {
            isPausing = false;
            state = State.Homing;
            // ホーミング開始時に縦速度はそのまま、横速度はホーミング速度にする
            float moveX = lastTargetIsRight ? homingSpeed : -homingSpeed;
            rb.linearVelocity = new Vector2(moveX, rb.linearVelocity.y);
        }
    }

    // Patrol
    private void DoPatrol()
    {
        float vx = 0f;
        if (waypoints != null && waypoints.Length > 0)
        {
            Transform wp = waypoints[currentWaypoint];
            Vector2 dir = (wp.position - transform.position);
            float dist = dir.magnitude;
            if (dist <= waypointReachThreshold)
            {
                currentWaypoint++;
                if (currentWaypoint >= waypoints.Length)
                {
                    if (loopWaypoints) currentWaypoint = 0;
                    else currentWaypoint = waypoints.Length - 1;
                }
            }
            else
            {
                Vector2 move = dir.normalized * patrolSpeed;
                vx = move.x;
            }
        }
        else
        {
            randomMoveTimer -= Time.fixedDeltaTime;
            if (randomMoveTimer <= 0f)
            {
                randomDir = Random.value > 0.5f ? Vector2.right : Vector2.left;
                randomMoveTimer = randomMoveInterval;
            }
            vx = randomDir.x * patrolSpeed;
        }

        if (Mathf.Abs(vx) > 0.01f) lastTargetIsRight = vx > 0f;
        sr.flipX = !lastTargetIsRight;

        float vy = rb.linearVelocity.y;
        rb.linearVelocity = new Vector2(vx, vy);
    }

    // Homing
    private void DoHoming()
    {
        if (target == null) return;

        float dx = target.position.x - transform.position.x;
        float dy = target.position.y - transform.position.y;

        if (Mathf.Abs(dx) > flipDeadzone)
            lastTargetIsRight = dx > 0f;

        sr.flipX = !lastTargetIsRight;

        float moveX = lastTargetIsRight ? homingSpeed : -homingSpeed;

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position + Vector3.down * 0.1f,
            lastTargetIsRight ? Vector2.right : Vector2.left,
            0.3f,
            groundLayer
        );

        float moveY = rb.linearVelocity.y;
        if (hit.collider != null) moveY = climbSpeed;

        rb.linearVelocity = new Vector2(moveX, moveY);
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
