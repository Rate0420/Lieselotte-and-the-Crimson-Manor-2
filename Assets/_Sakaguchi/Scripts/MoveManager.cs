using UnityEngine;

public class MoveManager : MonoBehaviour
{
    // 連結必要なスクリプト　ステータスマネージャ

    public Rigidbody rb;
    public float moveSpeed;     // ステータスマネージャ側で設定すること
    public Camera cam;

    public Vector3 Move(Vector3 dir)
    {
        // カメラの方向を取得する
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;
        camForward.y = 0; camRight.y = 0; // Yは計算しない
        camForward.Normalize(); camRight.Normalize();    // 正規化
        // カメラ基準の移動ベクトルを計算
        Vector3 moveDir = camForward * dir.z + camRight * dir.x;
        // 向きベクトルに速度を掛ける
        Vector3 vel = moveDir.normalized * moveSpeed;
        //// シフト―
        //if (Input.GetKey(KeyCode.LeftShift))
        //{
        //    vel = moveDir.normalized * moveSpeed * 2f;
        //}
        // 計算した速度(x,z)を設定
        vel = new Vector3(vel.x, rb.linearVelocity.y, vel.z);
        // 合成した速度を計算

        // 移動方向があるかチェック
        if (dir != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(moveDir);
            // rigidbodyのMoveRotationを使って回転
            // Slerpを使って徐々に回転
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, rot, 0.1f));
        }

        //計算した速度を設定
        rb.linearVelocity = vel;

        return moveDir;
    }

    //public Transform lockOnTarget; // ロックオン対象（あれば）

    //public Vector3 Move(Vector3 dir)
    //{
    //    if (dir.sqrMagnitude < 0.01f)
    //    {
    //        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
    //        return Vector3.zero;
    //    }

    //    // カメラ方向を取得（Y軸のみ考慮）
    //    Vector3 camForward = Camera.main.transform.forward;
    //    camForward.y = 0;
    //    camForward.Normalize();
    //    Vector3 camRight = Camera.main.transform.right;
    //    camRight.y = 0;
    //    camRight.Normalize();

    //    Vector3 moveDir = (camForward * dir.z + camRight * dir.x).normalized;
    //    Vector3 velocity = moveDir * moveSpeed;
    //    velocity.y = rb.linearVelocity.y;

    //    // 方向転換（ロックオン中はターゲット方向、それ以外は移動方向）
    //    if (lockOnTarget != null)
    //    {
    //        Debug.Log("いる");
    //        Vector3 toTarget = lockOnTarget.position - transform.position;
    //        toTarget.y = 0;
    //        if (toTarget.sqrMagnitude > 0.001f)
    //        {
    //            Debug.Log("1");
    //            Quaternion targetRot = Quaternion.LookRotation(toTarget);
    //            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, 0.05f));
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("2");
    //        Quaternion targetRot = Quaternion.LookRotation(moveDir);
    //        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, 0.05f));
    //    }

    //    rb.linearVelocity = velocity;
    //    return moveDir;
    //}
}
