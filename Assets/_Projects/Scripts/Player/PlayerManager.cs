using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    // スクリプト説明
    // プレイヤーの基本操作を管理するスクリプト
    // ここに様々なスクリプトをアタッチして、関数を作り、他のスクリプトが呼び出す形にする
    // 各種ステータスはステータスマネージャから拾ってくる。

    public Animator anim;
    string playAName;   // 再生中のアニメーション名

    public MoveManager moveManager;
    public PlayerMeleeCombo playerMeleeCombo;
    public Player_GB_Attack player_GB_Attack;
    public PlayerGuard playerGuard;
    public PlayerStatusManager playerStatusManager;
    public PlayerDamageReceiver PlayerDamageReceiver;
    public FaceController faceController;
    public PlayerParry playerParry;
    public ComboManager comboManager;

    public UniqueAttack uniqueAttack;
    public PlayerGuardCounter playerGuardCounter;   // リーゼロッテ専用(他キャラは未設定でOK)

    [SerializeField] Material trailMat;
    Color changeColor;

    Vector3 moveDir;

    public string aNameIdol = "Idol";
    public string aNameMove = "Move";
    public string aNameGBAttack = "GB_Attack";
    public string aNameGuard = "Guard";
    public string aNameHit = "Hit";
    public string aNameDown = "Down";
    public string aNameGetUp = "GetUp";
    public string aNameParry_U = "Parry_Up";
    public string aNameParry_D = "Parry_Down";
    public string aNameParry_L = "Parry_Left";
    public string aNameParry_R = "Parry_Right";
    public string aNameDamage_U = "UP_Damage";
    public string aNameDamage_D = "Down_Damage";
    public string aNameDamage_L = "Left_Damage";
    public string aNameDamage_R = "Right_Damage";
    public string aNameUAAttack = "UA_Attack";
    public float HitTime = 0.7f;
    public float DownTime = 1f;
    public float DownMotionTime = 1f;
    public float GetUpMotionTime = 0.5f;

    public string[] comboAnims = {
    "Combo1",
    "Combo2",
    "Combo3",
    "Combo4"
    };

    // 基本フラグ　ステップ、コンボ、ガード、その他攻撃
    // 追加フラグ　ダメージ食らい、ダウン、ゲーム終了

    public bool ComboFlg;
    public bool GuardFlg;
    public bool GB_AttackFlg;
    public bool ParryFlg;
    public bool UAFlg;
    public bool GuardCounterFlg;

    public bool SkillFlg;
    public bool UltFlg;

    public bool GetDamageFlg;
    public bool DownFlg;
    public bool GameEndFlg;

    public bool dummyFlg;   // デバッグ用の操作不能になるフラグ

    public bool JustGuardFlg;

    public bool DebufFlg;
    public bool mutekiFlg;

    public Vector3 dir;

    Coroutine hitCoroutine;
    Coroutine trailFadeCoroutine;
    Rigidbody rb;

    private Vector2 moveInput;

    [SerializeField] bool isController; // キーボードorコントローラーのどちらかを判断
    public bool actFlg;    // trueの時idolモーションを実行しない

    //public Transform opponent;  // 相手プレイヤー
    public float rotateSpeed = 10f;  // 回転速度
    [SerializeField] float trailFadeTime = 0.5f; // トレイルのフェード時間

    private void Awake()
    {
        // VSyncCount を Dont Sync に変更
        QualitySettings.vSyncCount = 0;
        // 60fpsを目標に設定
        Application.targetFrameRate = 30;
    }

    private void Start()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        changeColor = trailMat.color;
        //if (this.tag == "Player1") opponent = GameObject.FindWithTag("Player2").transform;
        //if (this.tag == "Player2") opponent = GameObject.FindWithTag("Player1").transform;
    }


    private void FixedUpdate()
    {

        if (ComboFlg || ParryFlg || UAFlg && changeColor.a != 0.5)
        {
            ActivateTrail();
        }
        else if (!GuardFlg && !ComboFlg && !ParryFlg && !UAFlg && changeColor.a != 0)
        {
            DeactivateTrail(trailFadeTime);
        }
        // コントローラー版
        if (GetDamageFlg || DownFlg || dummyFlg) return;

        if (!dummyFlg)
        {
            if (ComboFlg || GuardFlg || GB_AttackFlg || UltFlg || GameEndFlg || ParryFlg || UAFlg || GuardCounterFlg) return;
            dir = new Vector3(moveInput.x, 0, moveInput.y);
        }



        //if (dir != Vector3.zero)
        //{
        //    moveDir = moveManager.Move(dir);
        //    PlayAnim(aNameMove, 0.1f);
        //}
        //else if(!actFlg)
        //{
        //    PlayAnim(aNameIdol, 0.2f);
        //}

        // 移動中 → 入力方向を向く
        if (dir != Vector3.zero)
        {
            moveDir = moveManager.Move(dir);
            Quaternion moveRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, moveRot, rotateSpeed * Time.fixedDeltaTime);
            PlayAnim(aNameMove, 0.1f);
        }
        // 非移動時 → 相手方向を向く＋待機モーション再生
        else if (!actFlg)
        {
            var state = anim.GetCurrentAnimatorStateInfo(0);
            bool isParryAnim =
                state.IsName(aNameParry_U) ||
                state.IsName(aNameParry_D) ||
                state.IsName(aNameParry_L) ||
                state.IsName(aNameParry_R);

            if (!isParryAnim)
            {
                PlayAnim(aNameIdol, 0.2f);
            }
            if (isParryAnim && state.normalizedTime > 1)
            {
                PlayAnim(aNameIdol, 0.2f);
            }

            // ★ パリィが終わった瞬間だけ Idle にする
        }


        // キーボード版
        if (GetDamageFlg || DownFlg) return;
        if (!dummyFlg)
        {

            if (ComboFlg || GuardFlg || GB_AttackFlg || UltFlg || GetDamageFlg || DownFlg || GameEndFlg || dummyFlg || ParryFlg) return;

            // 移動入力があった時の処理(キーボード版)
            // キー入力(上下左右)を取得
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            // x,z方向の入力から、向きベクトルを計算
            dir = new Vector3(h, 0, v);
        }
    }
    // 以下コントローラー関係

    public void MoveAction(InputAction.CallbackContext context)
    {
        // InputActionのMove用入力値を取得
        moveInput = context.ReadValue<Vector2>();

    }

    public void ParryAction(InputAction.CallbackContext context)
    {
        if (CheckAllFlgisOff())
        {
            Vector2 parryDir = context.ReadValue<Vector2>();
            // 入力が小さい場合は無視（スティックの遊び対策）
            if (parryDir.magnitude < 0.7f)
                return;

            ParryFlg = true;
            // 入力方向を判定
            if (Mathf.Abs(parryDir.x) > Mathf.Abs(parryDir.y))
            {
                // 横方向が強い場合
                if (parryDir.x > 0)
                    playerParry.ParryAction(PlayerParry.parryDir.右);
                else
                    playerParry.ParryAction(PlayerParry.parryDir.左);

            }
            else
            {
                // 縦方向が強い場合
                if (parryDir.y > 0)
                    playerParry.ParryAction(PlayerParry.parryDir.上);
                else
                    playerParry.ParryAction(PlayerParry.parryDir.下);

            }
        }
    }

    // 戦技
    public void GBAtc(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (!CheckAllFlgisOff()) return;

        uniqueAttack.Attack(playerStatusManager);
    }


    // 近接攻撃
    public void MeleeAttack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            Debug.Log("ぱふぉーむど");
        }
        if (GuardFlg)
        { 
            Debug.Log("ガード中");
        }
        if(playerGuardCounter.CanCounter)
        {
            Debug.Log("カウンター受付中");
        }
        // ガード中で、被弾直後(カウンター受付時間内)なら、通常攻撃ではなくガードカウンターを出す
        if (context.phase == InputActionPhase.Performed && GuardFlg && playerGuardCounter != null && playerGuardCounter.CanCounter)
        {
            Debug.Log("ガードカウンター発動");
            playerGuardCounter.Counter();
            return;
        }
        if (!GuardFlg && !GB_AttackFlg && !GetDamageFlg && !DownFlg && !GameEndFlg && !dummyFlg && !ParryFlg && !UAFlg && !GuardCounterFlg && context.phase == InputActionPhase.Performed)
            playerMeleeCombo.MeleeCombo();
    }
    // ガード関連
    public void GuardAction(InputAction.CallbackContext context)
    {
        if (context.started && CheckAllFlgisOff())
        {
            playerGuard.Guard();
            PlayerDamageReceiver.justguardTimer = 0;

        }
        if (context.canceled && GuardFlg)
        {
            playerGuard.GuardRelease();
        }
    }

    // アニメーション再生メソッド
    public void PlayAnim(string reqAName, float blendTime = 0.2f)
    {
        // 現在再生しているアニメーションと違うかチェック
        if (playAName != reqAName)
        {
            // ブレンドの時間があるかチェック
            if (blendTime != 0)
            {
                // アニメーションブレンド
                anim.CrossFade(reqAName, blendTime);
            }
            else
            {
                // アニメーション再生
                anim.Play(reqAName);
            }
            // 現在再生中のアニメーションを設定
            playAName = reqAName;

        }
    }

    // アニメーション再生メソッド(同じモーションでも連続で再生できる)
    public void ContinuousPlayAnim(string reqAName, float blendTime = 0.2f)
    {
        // ブレンドの時間があるかチェック
        if (blendTime != 0)
        {
            // アニメーションブレンド
            anim.CrossFade(reqAName, blendTime, -1, 0f);
        }
        else
        {
            // アニメーション再生
            anim.Play(reqAName, -1, 0f);
        }
        // 現在再生中のアニメーションを設定
        playAName = reqAName;

    }

    bool CheckAllFlgisOff()
    {
        return !ComboFlg && !GuardFlg && !GB_AttackFlg &&
               !GetDamageFlg && !DownFlg && !GameEndFlg && !dummyFlg && !ParryFlg && !UAFlg && !GuardCounterFlg;
    }

    public void OnDamage(int damageDir, bool down = false, Transform attacker = null)
    {
        if (mutekiFlg) return;

        if (DownFlg) return;

        // ガード・攻撃・パリィなど、今の行動をすべてキャンセルする
        CancelAllActions();

        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
        }

        if (down)
        {
            StartCoroutine(Down(attacker));
        }
        else
        {
            //  コルーチンを保存する
            hitCoroutine = StartCoroutine(Hit(damageDir));
        }
    }

    /// <summary>
    /// 今の行動(ガード・コンボ・パリィ・ガードカウンターなど)をすべて中断する。
    /// 被弾時のほか、ヴェルミリオンの緊急回避など「全行動キャンセル」が必要な場面でも使う。
    /// (戦技UniqueAttackは別コルーチンで動いているため、まだ含まれていない)
    /// </summary>
    public void CancelAllActions()
    {
        playerGuard.GuardRelease();
        playerMeleeCombo.Stop();
        player_GB_Attack.Stop();
        playerParry.Stop();
        if (playerGuardCounter != null) playerGuardCounter.Stop();
    }

    IEnumerator Hit(int dir)
    {
        GetDamageFlg = true;
        switch (dir)
        {
            case 0:
                ContinuousPlayAnim(aNameDamage_U, 0.1f);
                break;
            case 1:
                ContinuousPlayAnim(aNameDamage_D, 0.1f);
                break;
            case 2:
                ContinuousPlayAnim(aNameDamage_L, 0.1f);
                break;
            case 3:
                ContinuousPlayAnim(aNameDamage_R, 0.1f);
                break;
        }
        faceController.PlayCustomExpression("damaged", HitTime);
        float timer = 0;
        while (timer <= HitTime)
        {
            yield return null;
            timer += Time.deltaTime;
        }
        GetDamageFlg = false;
        PlayAnim(aNameIdol, 0.1f);
        hitCoroutine = null;
    }

    IEnumerator Down(Transform attacker)
    {
        DownFlg = true;

        Vector3 knockbackDir = Vector3.zero;
        if (attacker != null)
        {
            knockbackDir = (transform.position - attacker.position).normalized;
            knockbackDir.y = 0;
        }

        PlayAnim(aNameDown, 0f);
        faceController.PlayCustomExpression("downed", DownMotionTime);
        if (attacker != null && rb != null)
        {
            float knockbackForce = 8f;
            rb.AddForce(knockbackDir * knockbackForce, ForceMode.VelocityChange);
        }
        yield return new WaitForSeconds(0.4f);
        yield return new WaitForSeconds(DownMotionTime - 0.4f);

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        yield return new WaitForSeconds(DownTime);
        PlayAnim(aNameGetUp, 0f);
        yield return new WaitForSeconds(GetUpMotionTime);

        DownFlg = false;
        GetDamageFlg = false;
        PlayAnim(aNameIdol, 0.1f);
    }

    // trailMatのalphaを変える
    public void ActivateTrail()
    {
        if (trailFadeCoroutine != null) { StopCoroutine(trailFadeCoroutine); trailFadeCoroutine = null; }
        changeColor.a = 0.2f;
        trailMat.color = changeColor;
    }
    // trailMatのalphaを0にする
    // フェードする
    public void DeactivateTrail(float fadetime)
    {
        if (trailFadeCoroutine != null) return;   // FixedUpdateから毎回呼ばれるので多重起動しない
        trailFadeCoroutine = StartCoroutine(TrailFadeCoroutine(fadetime));
    }

    IEnumerator TrailFadeCoroutine(float fadetime)
    {
        float alpha = changeColor.a;
        // fadetime秒かけてalphaを0にする
        while (alpha > 0)
        {
            if (ComboFlg || ParryFlg) break;
            alpha -= Time.deltaTime / fadetime * 0.5f;
            if (alpha < 0) alpha = 0;
            changeColor.a = alpha;
            trailMat.color = changeColor;
            yield return null;
        }
        trailFadeCoroutine = null;
    }

    public void ChangeColorTrail(Color color)
    {
        color.a = changeColor.a;
        changeColor = color;
        trailMat.color = color;
    }

    public void GiveInvincible(float a)
    {
        StartCoroutine(InvincibleCoroutine(a));
    }

    IEnumerator InvincibleCoroutine(float time)
    {
        mutekiFlg = true;
        yield return new WaitForSeconds(time);
        mutekiFlg = false;
    }
}