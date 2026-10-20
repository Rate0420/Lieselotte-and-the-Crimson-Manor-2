using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ヴェルミリオン専用: 緊急回避
/// 今の行動(被弾モーション・戦技も含む)をすべてキャンセルして、敵と逆方向へワープする。
/// 被弾中でも使えるが、クールタイムは長い。ダウン中は使えない(ダウン中はもともとダメージを受けないため)。
/// </summary>
public class PlayerEmergencyDodge : MonoBehaviour
{
    public PlayerManager playerManager;
    public PlayerStatusManager playerStatusManager;
    [SerializeField] Rigidbody rb;                  // 未設定ならPlayerManagerのRigidbodyを使う

    [Header("性能")]
    [SerializeField] float cooldown = 8f;           // クールタイム(秒)
    [SerializeField] float dodgeDistance = 5f;      // 敵と逆方向へ移動する距離
    [SerializeField] float invincibleTime = 0.6f;   // ワープ後の無敵時間(秒)
    [SerializeField] float lockTime = 0.25f;        // ワープ後に操作できない時間(秒)
    [SerializeField] float staminaCost = 0f;        // 消費スタミナ(0なら消費なし)

    [Header("壁の判定")]
    [SerializeField] LayerMask blockMask = ~0;      // 壁・ステージのレイヤー(進路をふさぐもの)
    [SerializeField] float wallMargin = 0.6f;       // 壁との間に空ける距離

    [Header("演出(任意)")]
    [SerializeField] string animName = "";          // 空ならモーションは再生しない
    [SerializeField] GameObject dodgeEffect;        // ワープ前後に出すエフェクト
    [SerializeField] string seName = "";            // 空ならSEは鳴らさない
    [SerializeField] Image cooldownImage;           // クールタイムの表示(fillAmountで制御)

    float cooldownTimer;

    /// <summary>
    /// クールタイムの進み具合(0 = 使った直後、1 = 使える)
    /// </summary>
    public float CooldownRatio
    {
        get { return cooldown > 0f ? 1f - Mathf.Clamp01(cooldownTimer / cooldown) : 1f; }
    }

    public bool CanDodge
    {
        get
        {
            return cooldownTimer <= 0f
                && !playerManager.DodgeFlg
                && !playerManager.DownFlg
                && !playerManager.GameEndFlg
                && !playerManager.dummyFlg
                && playerStatusManager.CurrentVitality >= staminaCost
                && !playerManager.isLiselotte;
        }
    }

    void Awake()
    {
        if (rb == null) rb = playerManager.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
        if (cooldownImage != null) cooldownImage.fillAmount = CooldownRatio;
    }

    /// <summary>
    /// 緊急回避を出す(PlayerManager.DodgeActionから呼ぶ)
    /// </summary>
    public void Dodge()
    {
        if (!CanDodge) return;
        StartCoroutine(DodgeRoutine());
    }

    IEnumerator DodgeRoutine()
    {
        cooldownTimer = cooldown;

        // 今の行動をすべてキャンセル(被弾モーションも解除する)
        playerManager.CancelAllActions();
        playerManager.CancelHitReaction();
        playerManager.DodgeFlg = true;
        if (staminaCost > 0f) playerStatusManager.StaminaRend(staminaCost);

        // 敵と逆方向へワープ(壁があれば手前で止まる)
        Transform pt = playerManager.transform;
        Vector3 from = pt.position;
        Vector3 away = GetAwayDirection(from);
        float dist = GetFreeDistance(from, away);
        Vector3 to = from + away * dist;

        if (dodgeEffect != null) Instantiate(dodgeEffect, from, Quaternion.identity);

        pt.position = to;
        if (rb != null)
        {
            rb.position = to;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        // ワープしたら敵の方を向く
        pt.rotation = Quaternion.LookRotation(-away);

        if (dodgeEffect != null) Instantiate(dodgeEffect, to, Quaternion.identity);
        if (!string.IsNullOrEmpty(animName)) playerManager.ContinuousPlayAnim(animName, 0.05f);
        if (!string.IsNullOrEmpty(seName)) SoundManager.Instance.PlaySE(seName);

        playerManager.GiveInvincible(invincibleTime);

        yield return new WaitForSeconds(lockTime);
        playerManager.DodgeFlg = false;
    }

    // 一番近い敵から見て逆方向(水平)を返す。敵がいなければ後ろ向き
    Vector3 GetAwayDirection(Vector3 from)
    {
        EnemyController nearest = null;
        float best = float.MaxValue;
        foreach (var e in FindObjectsByType<EnemyController>(FindObjectsSortMode.None))
        {
            float d = (e.transform.position - from).sqrMagnitude;
            if (d < best)
            {
                best = d;
                nearest = e;
            }
        }

        Vector3 away = (nearest != null) ? from - nearest.transform.position : -playerManager.transform.forward;
        away.y = 0f;
        if (away.sqrMagnitude < 0.0001f) away = -playerManager.transform.forward;
        away.y = 0f;
        return away.normalized;
    }

    // 壁の手前までの距離(自分自身のコライダーは無視する)
    float GetFreeDistance(Vector3 from, Vector3 dir)
    {
        float dist = dodgeDistance;
        Vector3 origin = from + Vector3.up * 0.5f;
        RaycastHit[] hits = Physics.RaycastAll(origin, dir, dodgeDistance + wallMargin, blockMask, QueryTriggerInteraction.Ignore);
        foreach (var hit in hits)
        {
            if (hit.collider.transform.IsChildOf(playerManager.transform)) continue;
            dist = Mathf.Min(dist, Mathf.Max(0f, hit.distance - wallMargin));
        }
        return dist;
    }
}
