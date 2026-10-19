using System.Collections;
using UnityEngine;

/// <summary>
/// リーゼロッテ専用: ガードカウンター
/// ガードで敵の攻撃を受けた直後(受付時間内)に、ガード中に攻撃ボタンを押すと発動する。
/// ダメージ判定はUniqueAttackと同じくMeleeBoxを使い、nowAtkStateを"GuardCounter"にする。
/// (敵側の被弾処理はEnemyController.OnTriggerEnterの "GuardCounter" で行う)
/// </summary>
public class PlayerGuardCounter : MonoBehaviour
{
    public PlayerManager playerManager;
    public PlayerStatusManager playerStatusManager;

    [Header("受付")]
    [SerializeField] float counterWindow = 0.6f;    // ガード成功後、カウンターを受け付ける時間(秒)

    [Header("攻撃モーション")]
    [SerializeField] string animName = "GuardCounter";   // Animatorのステート名(武器ごとのAnimatorControllerに必要)
    [SerializeField] float totalTime = 0.9f;        // 技全体の時間(秒)
    [SerializeField] float hitStart = 0.25f;        // 判定が出るまでの時間(技開始から)
    [SerializeField] float hitTime = 0.15f;         // 判定の持続時間
    [SerializeField] string hitSeName = "攻撃音高";

    [Header("威力・コスト")]
    [SerializeField] float damageRate = 2.5f;       // MeleeAtkに掛ける倍率
    [SerializeField] float postureDamage = 15f;     // 敵の体勢ゲージを削る量
    [SerializeField] float staminaCost = 30f;       // 消費スタミナ(多め)

    float windowTimer;
    Coroutine currentCoroutine;

    /// <summary>
    /// 敵に与えるダメージ(コンボによるMeleeAtkの上昇も乗る)
    /// </summary>
    public int Damage
    {
        get { return Mathf.FloorToInt(playerStatusManager.MeleeAtk * damageRate); }
    }

    public float PostureDamage
    {
        get { return postureDamage; }
    }

    /// <summary>
    /// 今カウンターを出せるか(受付時間内・ガード中・スタミナが足りている)
    /// </summary>
    public bool CanCounter
    {
        get
        {
            return windowTimer > 0f
                && playerManager.GuardFlg
                && !playerManager.dummyFlg
                && !playerManager.GameEndFlg
                && playerStatusManager.CurrentVitality >= staminaCost;
        }
    }

    void Update()
    {
        // ガードをやめたら受付は終わり
        if (!playerManager.GuardFlg)
        {
            windowTimer = 0f;
            return;
        }
        if (windowTimer > 0f) windowTimer -= Time.deltaTime;
    }

    /// <summary>
    /// ガードで敵の攻撃を受けたときにPlayerDamageReceiverから呼ぶ
    /// </summary>
    public void OnGuardSuccess()
    {
        windowTimer = counterWindow;
    }

    /// <summary>
    /// ガードカウンターを出す(PlayerManager.MeleeAttackから呼ぶ)
    /// </summary>
    public void Counter()
    {
        if (!CanCounter) return;
        windowTimer = 0f;
        currentCoroutine = StartCoroutine(CounterAttack());
    }

    IEnumerator CounterAttack()
    {
        // ガードを解いて技に入る
        playerManager.playerGuard.GuardRelease();
        playerManager.GuardCounterFlg = true;
        playerStatusManager.StaminaRend(staminaCost);
        playerStatusManager.nowAtkState = "GuardCounter";

        playerManager.ContinuousPlayAnim(animName, 0.05f);
        playerManager.faceController.PlayCustomExpression("attack", totalTime);

        float timer = 0f;
        bool hitOn = false;
        while (timer < totalTime)
        {
            timer += Time.deltaTime;
            bool active = timer >= hitStart && timer < hitStart + hitTime;
            if (active && !hitOn)
            {
                hitOn = true;
                playerStatusManager.MeleeAtkOnOff(true);
                SoundManager.Instance.PlaySE(hitSeName);
            }
            else if (!active && hitOn)
            {
                hitOn = false;
                playerStatusManager.MeleeAtkOnOff(false);
            }
            yield return null;
        }

        Finish();
        currentCoroutine = null;
    }

    /// <summary>
    /// 被弾などで中断されたとき(PlayerManager.CancelAllActionsから呼ぶ)
    /// </summary>
    public void Stop()
    {
        windowTimer = 0f;
        if (currentCoroutine == null) return;
        StopCoroutine(currentCoroutine);
        currentCoroutine = null;
        Finish();
    }

    void Finish()
    {
        playerStatusManager.MeleeAtkOnOff(false);
        if (playerStatusManager.nowAtkState == "GuardCounter") playerStatusManager.nowAtkState = "";
        playerManager.GuardCounterFlg = false;
    }
}
