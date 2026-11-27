using UnityEngine;
using System.Collections;
using Unity.Jobs;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class EnemyController : MonoBehaviour
{
    // 敵の総合的な管理を行うスクリプト 
    // ひとまずアニメーションと攻撃を管理する

    [SerializeField] private TutorialManager tutorialManager;

    public Animator enemyAnimator;    // アニメーター格納用変数
    [SerializeField] string aNameIdle;          // 待機アニメーション名
    [SerializeField] string aNameMove;          // 移動アニメーション名
    [SerializeField] string aNameHit;           // 被ダメージアニメーション名(体幹が0になった時に使う)
    [SerializeField] string aNameDown;          // ダウンアニメーション名
    [SerializeField] BoxCollider attackCollider;   // 攻撃判定用コライダー
    [SerializeField] float attackRange;        // 攻撃範囲
    [SerializeField] float moveSpeed;          // 移動速度
    [SerializeField] EnemyAttack[] enemyAttacks; // 各攻撃スクリプト格納用配列
    [SerializeField] float rotateSpeed = 10f;    // 回転速度
    public HpBar hpBar;                // 敵の体力バー
    [SerializeField] int hpMax;          // 敵の最大体力
    [SerializeField] EnemyPostureGauge enemyPostureGauge; // 敵の体勢ゲージ
    [SerializeField] ComboManager comboManager; // コンボマネージャー

    public float warningTime = 0.5f;

    // ダメージを喰らった時に再生するエフェクト
    [SerializeField] GameObject damageEffect;

    public int currentAttackDir;                // 現在の攻撃方向(上下左右)
    public int currentAttackDamage;              // 現在の攻撃力
    GameObject player;
    public bool isAttackFinished = false;
    bool isdowned = false;
    bool isdownHit = false;
    public bool cancelAttack = false;

    float downedDuration = 5f; // ダウン状態の持続時間
    float elapsed = 0f;

    float DownAtkWait = 1f;

    [Header("毒設定")]
    [SerializeField] private float poisonInterval = 1f;
    [SerializeField] private float poisonDuration = 5f;

    private Coroutine poisonCoroutine;

    private bool isPoisoned = false;

    Coroutine normalLoop;
    Coroutine tutorialLoop;

    PlayerStatusManager psm;

    bool isDead = false;

    public bool tutorialMode = false;
    // チュートリアル用のenum
    public enum TutorialState
    {
        Move = 0,
        Attack1 = 1,
    }
    public TutorialState tutorialState = TutorialState.Move;

    public enum State
    {
        Stay = 0,
        Move = 1,
        Attack1 = 2,
        Attack2 = 3,
        Attack3 = 4
    }

    void Start()
    {
        hpBar.maxHp = hpMax;
        hpBar.nowHp = hpMax;
        player = GameObject.FindWithTag("Player");
        psm = player.GetComponent<PlayerStatusManager>();

        if (tutorialMode)
            tutorialLoop = StartCoroutine(TutorialEnemyActionLoop());
        else
            normalLoop = StartCoroutine(EnemyActionLoop());

    }

    private void Update()
    {

        Vector3 lookDir = player.transform.position - transform.position;
        lookDir.y = 0;
        if (lookDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.fixedDeltaTime);
        }
    }

    // 行動ループ
    IEnumerator EnemyActionLoop()
    {
        // 念のため二重起動防止
        if (isdowned) yield break;

        while (!isdowned)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            if (distance < attackRange)
            {
                int choice = Random.Range(0, 10);

                if (choice == 0)
                    yield return DoIdle(1f);
                else
                {
                    int attackIndex = Random.Range(0, enemyAttacks.Length);
                    yield return DoAttack(attackIndex);
                }
            }
            else
            {
                yield return DoMove();
            }

            yield return null;
        }

        // ダウン状態へ（ループ停止）
    }

    IEnumerator TutorialEnemyActionLoop()
    {
        // 念のため二重起動防止
        if (isdowned) yield break;

        while (!isdowned)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            if (distance < attackRange)
            {
                int choice = Random.Range(0, 10);

                if (choice == 0)
                    yield return DoIdle(1f);
                else
                {
                    if (tutorialState == TutorialState.Attack1)
                    {
                        int attackIndex = Random.Range(0, enemyAttacks.Length);
                        yield return DoAttack(attackIndex);
                    }
                }
            }
            else
            {
                yield return DoMove();
            }

            yield return null;
        }

    }

    public void EndTutorial()
    {
        if (isDead) return;   // 撃破済みの敵の行動ループを再開しない
        if (tutorialLoop != null)
            StopCoroutine(tutorialLoop);

        if (normalLoop == null)
            normalLoop = StartCoroutine(EnemyActionLoop());
    }

    /// <summary>
    /// プレイヤーにパリィされたとき
    /// </summary>
    public void GetParry()
    {
        // パリィ強化の効果を反映
        float basePostureDamage = 20f;
        float postureDamage = basePostureDamage * psm.parryPostureBoost;

        enemyPostureGauge.ConsumePosture(postureDamage);

        // パリィ強化のログ
        if (psm.parryPostureBoost > 1f)
        {
            Debug.Log(
                $"<color=cyan>[パリィ強化発動]</color> " +
                $"倍率：{psm.parryPostureBoost:F2}倍 / " +
                $"体幹ダメージ：{postureDamage:F0}"
            );
        }
        // パリィ成功時の追加HPダメージ
        if (psm.parryDamage > 0)
        {
            int damage = Mathf.FloorToInt(psm.parryDamage);
            hpBar.ApplyDamage(damage);

            Debug.Log($"パリィ追加ダメージ：{damage}");
        }

        // パリィ練習
        if (tutorialManager != null)
            if (tutorialManager.tutorialNum == 3)
                tutorialManager.TutorialCount();

        // パリィSE再生
        int seIndex = Random.Range(0, 3);

        switch (seIndex)
        {
            case 0:
                SoundManager.Instance.PlaySE("パリィ音高");
                break;
            case 1:
                SoundManager.Instance.PlaySE("パリィ音中");
                break;
            case 2:
                SoundManager.Instance.PlaySE("パリィ音低");
                break;
        }

        // パリィ成功したらスタミナ回復
        psm.StaminaRend(-psm.parryStamina);

        //psmのパリィのカウントを増やす
        psm.parryCount++;

        if (enemyPostureGauge.currentPosture <= 0)
        {
            isdowned = true;
            StartCoroutine(DownedState());
        }
    }

    /// <summary>
    /// ダウン状態(一定時間行動不能、攻撃を喰らったら指定のアニメーションをプレイヤーにさせhpBar.AppryDamageで多めの値で減らす、
    ///  一定時間後にダウン解除、EnemyActionLoopに戻る)
    /// </summary>
    /// <returns></returns>
    IEnumerator DownedState()
    {
        CancelCurrentAttack();
        elapsed = 0f;
        // ダウンSE再生
        SoundManager.Instance.PlaySE("体勢崩し");

        while (elapsed < downedDuration)
        {
            if (!isdownHit) elapsed += Time.deltaTime;
            yield return null;
        }

        // 撃破済みなら復帰しない(ダウン中に倒された場合に行動ループが再開するのを防ぐ)
        if (isDead) yield break;

        // ダウン解除
        isdowned = false;
        enemyPostureGauge.RecoveryPosture(enemyPostureGauge.maxPosture);

        // 古い行動ループが動いていたら止める
        if (normalLoop != null)
        {
            StopCoroutine(normalLoop);
        }

        // 起き上がりアニメーション再生
        // (idleモーションを再生し、行動ループに入る前に待機時間を設ける)
        PlayAnim(aNameIdle, 1f);
        yield return new WaitForSeconds(1f);

        // 新しい行動ループを開始
        normalLoop = StartCoroutine(EnemyActionLoop());
    }


    // ◆ 待機
    IEnumerator DoIdle(float duration)
    {
        PlayAnim(aNameIdle);
        yield return new WaitForSeconds(duration);
    }

    // ◆ 移動
    IEnumerator DoMove()
    {
        // 移動アニメーション再生(aNameMoveがすでに再生されていた場合は無視される)
        if (enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName(aNameMove) == false) PlayAnim(aNameMove, 0.1f);

        // プレイヤーの方へ1秒間歩く（必要なら調整）
        float moveTime = 1f;
        float t = 0f;

        while (t < moveTime)
        {
            t += Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position,
                                                     player.transform.position,
                                                     moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    // ◆ 攻撃
    IEnumerator DoAttack(int index)
    {
        isAttackFinished = false; // 攻撃開始時にリセット
        enemyAttacks[index].Attack(this);

        // 攻撃が完了するまで待つ
        yield return new WaitUntil(() => isAttackFinished);

        // ここに来たら攻撃完全終了
    }

    // アニメーション再生メソッド
    public void PlayAnim(string name, float blend = 0f)
    {
        // ブレンドの時間があるかチェック
        if (blend != 0)
        {
            // アニメーションブレンド
            enemyAnimator.CrossFade(name, blend, 0, 0f);
        }
        else
        {
            // アニメーション再生
            enemyAnimator.Play(name, 0, 0f);
        }
    }

    public void EnableAttackCollider(bool onOff)
    {
        attackCollider.enabled = onOff;
        attackCollider.GetComponent<MeshRenderer>().enabled = onOff;
    }

    public void CancelCurrentAttack()
    {
        cancelAttack = true;
        EnableAttackCollider(false);

        // 強制的にヒットモーションへ
        PlayAnim(aNameHit, 0.1f);
        // ヒットモーションが終わったらダウンモーションを再生
        StartCoroutine(WaitForHitToDown());

        isAttackFinished = true; // ループ側の待機を解除
    }


    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;
        if (this.gameObject.tag == "EnemyAtk") return;

        if (other.CompareTag("PlayerAtk"))
        {
            PlayerStatusManager psm =
                other.GetComponentInParent<PlayerStatusManager>();

            if (psm == null) return;

            // ダウン状態で攻撃を受けた場合
            if (isdowned && !isdownHit && psm.nowAtkState != "UniqueAttack")
            {
                isdownHit = true;
                PlayerManager pm = other.GetComponentInParent<PlayerManager>();

                StartCoroutine(hitDownAttack(pm));
                return;
            }

            // 実際にダメージを与えたかどうか
            bool didDamage = false;

            switch (psm.nowAtkState)
            {
                case "MeleeCombo":

                    int damage = psm.MeleeAtk;

                    if (psm.playerMeleeCombo.isFirstAttack) damage = Mathf.FloorToInt(damage * psm.firstAtkDmgRate * psm.attackBuffRate);
                    else if (psm.playerMeleeCombo.isLastAttack)
                    {
                        // 最後の攻撃の時の処理を書く
                    }
                    else damage = Mathf.FloorToInt(damage * psm.attackBuffRate);

                    damage = psm.ApplyAdversity(damage);
                    hpBar.ApplyDamage(damage);
                    didDamage = true;
                    if (tutorialManager != null)
                        if (tutorialManager.tutorialNum == 0)
                            tutorialManager.TutorialCount();
                    break;

                case "MeleeComboFinish":
                    {
                        damage = Mathf.FloorToInt(psm.MeleeAtk * psm.finishAtkDmgRate * psm.attackBuffRate);

                        damage = psm.ApplyAdversity(damage);
                        hpBar.ApplyDamage(damage);
                        if (tutorialManager != null)
                            if (tutorialManager.tutorialNum == 0)
                                tutorialManager.TutorialCount();
                        didDamage = true;
                        break;
                    }

                case "UniqueAttack":
                    {
                        damage = psm.ApplyAdversity(psm.UniqueAttack);
                        hpBar.ApplyDamage(damage);


                        break;
                    }
                case "GuardCounter":
                    damage = psm.ApplyAdversity(Mathf.FloorToInt(psm.playerManager.playerGuardCounter.Damage * psm.attackBuffRate));
                    hpBar.ApplyDamage(damage);
                    DamagePosture(Mathf.FloorToInt(psm.playerManager.playerGuardCounter.PostureDamage * psm.playerManager.GetBuffRate(PlayerBuffs.BuffType.PostureDamage)));
                    didDamage = true;
                    break;
            }

            // 有効な攻撃が命中したときだけコンボ加算
            if (didDamage)
            {
                comboManager.ComboContinues();

                // 有効な攻撃が命中したときだけコンボ加算
                if (didDamage)
                {
                    // 毒の受け渡し確認
                    Debug.Log(
                        $"【毒チェック】hasPoisonEnchantment={psm.hasPoisonEnchantment}, " +
                        $"PoisonAttack={psm.PoisonAttack}"
                    );

                    if (psm.hasPoisonEnchantment && psm.PoisonAttack > 0)
                    {
                        Debug.Log($"【毒付与発動】毒ダメージ={psm.PoisonAttack}");
                        ApplyPoison(psm.PoisonAttack);
                    }
                    else
                    {
                        Debug.Log("【毒付与されていない】");
                    }

                    // 以下のダメージエフェクトとSEの処理はそのまま
                }

                // ダメージエフェクト
                Instantiate(
                    damageEffect,
                    transform.position + new Vector3(0, 1, 0),
                    Quaternion.identity
                );

                // ダメージSE
                int seIndex = Random.Range(0, 3);

                switch (seIndex)
                {
                    case 0:
                        SoundManager.Instance.PlaySE("命中音血高");
                        break;
                    case 1:
                        SoundManager.Instance.PlaySE("命中音血中");
                        break;
                    case 2:
                        SoundManager.Instance.PlaySE("命中音血低");
                        break;
                }
            }

            // HPが0以下になったら行動ループを停止
            if (hpBar.nowHp <= 0)
            {
                if (normalLoop != null)
                    StopCoroutine(normalLoop);

                if (tutorialLoop != null)
                {
                    StopCoroutine(tutorialLoop);
                    tutorialManager.tutorialNum++;
                }

                PlayAnim(aNameDown, 0.1f);
            }
        }
    }

    IEnumerator WaitForHitToDown()
    {
        // CrossFade直後は「現在のステート」がまだ遷移元のままなので、Hitに入るまで待つ(入らなくても1秒で諦める)
        float t = 0f;
        while (t < 1f && !IsHitPlaying())
        {
            t += Time.deltaTime;
            yield return null;
        }

        // ヒットアニメーションが終わるまで待つ
        while (IsHitPlaying())
        {
            yield return null;
        }

        // 撃破済みなら死亡モーション(Die)を上書きしない
        if (isDead) yield break;

        // ダウンアニメーション再生
        PlayAnim(aNameDown, 0.1f);
    }

    // ヒットアニメーションの再生中か(Hitへの遷移中も含む)
    bool IsHitPlaying()
    {
        var cur = enemyAnimator.GetCurrentAnimatorStateInfo(0);
        if (cur.IsName(aNameHit)) return cur.normalizedTime < 1f;
        return enemyAnimator.IsInTransition(0) && enemyAnimator.GetNextAnimatorStateInfo(0).IsName(aNameHit);
    }

    IEnumerator hitDownAttack(PlayerManager pm)
    {
        pm.dummyFlg = true;
        pm.PlayAnim(pm.aNameGBAttack);
        pm.comboManager.comboTimerFlg = false;
        yield return new WaitForSeconds(pm.playerStatusManager.GB_AttackTime);

        //hpBar.ApplyDamage(hpMax / 4);
        // 致命攻撃のダメージを与える
        hpBar.ApplyDamage(Mathf.FloorToInt(pm.playerStatusManager.MeleeAtk * pm.playerStatusManager.fatalAtkRate));
        comboManager.ComboContinues();
        Instantiate(damageEffect, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        // ダメージSEの再生
        int seIndex = Random.Range(0, 3);

        switch (seIndex)
        {
            case 0:
                SoundManager.Instance.PlaySE("命中音血高");
                break;
            case 1:
                SoundManager.Instance.PlaySE("命中音血中");
                break;
            case 2:
                SoundManager.Instance.PlaySE("命中音血低");
                break;
        }

        yield return new WaitForSeconds(DownAtkWait);

        // ダウン解除させる
        isdowned = false;
        isdownHit = false;
        elapsed = downedDuration; // 強制的に終了
        pm.ComboFlg = false;
        pm.dummyFlg = false;
        pm.comboManager.comboTimerFlg = true;

        // もしHPが0以下になっていたら行動ループを止める
        if (hpBar.nowHp <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 撃破時の処理(行動ループと実行中の攻撃を止めて死亡モーションへ)
    /// </summary>
    void Die()
    {
        if (isDead) return;
        isDead = true;
        // EnemyAttack.Atkは別コルーチンで動いているので、行動ループを止めるだけでは攻撃が続いてしまう
        cancelAttack = true;
        EnableAttackCollider(false);
        HideAttackWarning();
        isAttackFinished = true;
        if (normalLoop != null) StopCoroutine(normalLoop);
        if (tutorialLoop != null) StopCoroutine(tutorialLoop);
        PlayAnim(aNameDown, 0.1f);

    }

    public void AtkEnd()
    {
        psm.playerManager.PlayerDamageReceiver.isDamagedThisAttack = false;
    }

    public void ShowAttackWarning()
    {
        Debug.Log("saw");
        if (ParryCanvas.Instance != null)
        {
            ParryCanvas.Instance.ShowDirection(currentAttackDir);
        }
    }

    public void HideAttackWarning()
    {
        if (ParryCanvas.Instance != null)
        {
            ParryCanvas.Instance.ShowDirection(4);
        }
    }


    public void ApplyPoison(float damage)
    {
        if (damage <= 0 || hpBar.nowHp <= 0)
        {
            Debug.Log($"【毒付与失敗】毒ダメージ={damage}, 敵HP={hpBar.nowHp}");
            return;
        }

        Debug.Log($"【毒状態開始】毒ダメージ={damage} / 継続時間={poisonDuration}秒");

        // 毒状態中に再度攻撃されたら、毒の時間をリセット
        if (poisonCoroutine != null)
        {
            StopCoroutine(poisonCoroutine);
            Debug.Log("【毒時間リセット】");
        }

        poisonCoroutine = StartCoroutine(PoisonEffect(damage));
    }

    /// <summary>
    /// 体勢ゲージを削る(0になったらダウンさせる)。パリィ以外(ガードカウンターなど)の体勢削りで使う
    /// </summary>
    public void DamagePosture(float amount)
    {
        if (isdowned || isDead) return;
        enemyPostureGauge.ConsumePosture(amount);
        if (enemyPostureGauge.currentPosture <= 0)
        {
            isdowned = true;
            StartCoroutine(DownedState());
        }
    }



    private IEnumerator PoisonEffect(float damage)
    {
        Debug.Log("【毒コルーチン開始】");

        float elapsed = 0f;

        while (elapsed < poisonDuration && hpBar.nowHp > 0)
        {
            yield return new WaitForSeconds(poisonInterval);

            if (hpBar.nowHp <= 0)
                break;

            int poisonDamage = Mathf.CeilToInt(damage);

            Debug.Log($"【毒ダメージ直前】敵HP={hpBar.nowHp}, 毒ダメージ={poisonDamage}");

            hpBar.ApplyDamage(poisonDamage);

            Debug.Log($"【毒ダメージ直後】敵HP={hpBar.nowHp}");

            elapsed += poisonInterval;
        }

        Debug.Log("【毒コルーチン終了】");

        poisonCoroutine = null;
    }

}
