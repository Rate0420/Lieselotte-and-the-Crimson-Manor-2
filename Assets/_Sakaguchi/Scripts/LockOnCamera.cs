using UnityEngine;

public class LockOnCamera : MonoBehaviour
{
    public Transform player;     // プレイヤー
    public Transform target;     // 敵（ロックオン対象）
    public float distance = 6f;  // プレイヤーからカメラまでの距離
    public float height = 2f;    // カメラの高さオフセット
    public Vector3 offset;      // カメラの位置オフセット
    public float smoothSpeed = 5f; // 補間スピード
    public Vector3 PlayerOffset;  // プレイヤー位置(カメラのターゲット位置)のオフセット
    public Vector3 EnemyOffset;  // 敵位置(カメラのターゲット位置)のオフセット

    private void Awake()
    {
        target = GameObject.FindWithTag("Enemy").transform;
        player = GameObject.FindWithTag("Player").transform;
    }

    void LateUpdate()
    {
        if (player == null || target == null) return;

        // プレイヤーから敵への水平ベクトル
        Vector3 dirToTarget = (target.position - player.position);
        dirToTarget.y = 0f; // 垂直方向を無視
        dirToTarget.Normalize();

        // プレイヤーの位置を基準に、dirToTargetの逆方向にdistance分離れる
        Vector3 desiredPosition = (player.position + PlayerOffset) - dirToTarget * distance;
        // desiredPosition.y += height;
        desiredPosition += offset;

        // スムーズに補間
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);

        // プレイヤーと敵の中間点を注視
        Vector3 midPoint = ((player.position + PlayerOffset) + (target.position + EnemyOffset)) * 0.5f;
        transform.LookAt(midPoint);
    }
}