using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float smoothSpeed = 3.5f;   // 追従の速さ（数値は後で調整）
    [SerializeField] private Vector3 offset = new Vector3(2, 1, -10);

    void FixedUpdate()
    {
        if (player == null) return;

        Vector3 targetPos = player.position + offset;

        float t = smoothSpeed * Time.deltaTime;
        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            t
        );
    }
}
