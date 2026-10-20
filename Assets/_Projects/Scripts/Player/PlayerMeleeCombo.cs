using System.Collections;
using UnityEngine;

public class PlayerMeleeCombo : MonoBehaviour
{
    public PlayerManager playerManager;
    bool attackButtonPressedThisFrame;  // 近接コンボの予約入力のための変数
    bool bufferedCombo;  // 近接コンボの予約入力のための変数
    int comboStep = 0;
    float comboTimer = 0f;
    public float comboResetTime; // これ以内に次を押さないとリセット
    public float[] animTimes;   // 各コンボアニメーションの時間
    public float[] comboColOffset;
    public bool isFirstAttack = true;
    bool seFlg = false;

    bool firstcheck = false;

    Coroutine currentCoroutine;

    public HitTiming[] comboHitData;

    // 仮システム
    float ColResetTime = 0.1f;

    public PlayerStatusManager playerStatusManager;

    public float attackSpeed = 1f;

    void FirstCheck()
    {
        int len = playerManager.comboAnims.Length;

        if (animTimes.Length != len || comboHitData.Length != len)
        {
            Debug.Log(len + " " + animTimes.Length + " " + comboColOffset.Length);
            Debug.LogError("コンボ配列の長さが一致していません");
        }
        firstcheck = true;
    }


    public void MeleeCombo()
    {
        if (!firstcheck) FirstCheck();

        if (comboStep != animTimes.Length - 1) attackButtonPressedThisFrame = true;

        if (attackButtonPressedThisFrame && !playerManager.ComboFlg)
        {
            attackButtonPressedThisFrame = false;
            currentCoroutine = StartCoroutine(ComboAttack());
        }

    }

    IEnumerator ComboAttack()
    {
        if (comboStep >= comboHitData.Length) yield break;

        // 攻撃開始
        // ComboFlgを立てる前に判定する(立てたまま抜けるとComboFlgが戻らなくなる)
        if (playerManager.dummyFlg) yield break;
        playerManager.ComboFlg = true;

        // 弾きの回数に応じたバフをこの攻撃に持たせ、カウントをリセットする(ヴェルミリオン用。いなければ等倍)
        playerStatusManager.attackBuffRate = (playerManager.playerDeflect != null) ? playerManager.playerDeflect.ConsumeBuff() : 1f;

        // 攻撃速度反映
        playerManager.anim.speed = attackSpeed;

        bufferedCombo = false;
        playerManager.ContinuousPlayAnim(playerManager.comboAnims[comboStep], 0.2f);
        playerManager.faceController.PlayCustomExpression("attack", animTimes[comboStep]);

        comboTimer = 0f;
        seFlg = false;

        // ① 攻撃開始時に1回だけ ON
        isFirstAttack = true;
        playerStatusManager.MeleeAtkOnOff(true);

        if (playerStatusManager.nowAtkState != "MeleeComboFinish") playerStatusManager.nowAtkState = "MeleeCombo";

        //// ダメージ判定が有効になる時間
        //float hitStart = comboColOffset[comboStep];

        //// ダメージ判定が終わる時間
        //float hitEnd = comboColOffset[comboStep] + ColResetTime;


        while (comboTimer < animTimes[comboStep])
        {
            comboTimer += Time.deltaTime * attackSpeed;

            //// ダメージ判定のON/OFFは 1回だけ
            //if (comboTimer >= hitStart && comboTimer < hitEnd)
            //{
            //    if (playerStatusManager.GB_Box.activeSelf == false)
            //    {
            //        if(comboStep != 0)
            //        {
            //            isFirstAttack = false;
            //        }
            //        // SE再生
            //        int seIndex = Random.Range(0, 3);

            //        switch(seIndex)
            //        {
            //            case 0:
            //                SoundManager.Instance.PlaySE("攻撃音高");
            //                break;
            //            case 1:
            //                SoundManager.Instance.PlaySE("攻撃音中");
            //                break;
            //            case 2:
            //                SoundManager.Instance.PlaySE("攻撃音低");
            //                break;
            //        }
            //    }

            //    // 当たり判定中にしたい処理（あればここ）
            //    playerStatusManager.GB_AtkOnOff(true);


            //}
            //else
            //{
            //    playerStatusManager.GB_AtkOnOff(false);
            //}

            bool hitActive = false;

            var hitList = comboHitData[comboStep].hitTimes;

            for (int i = 0; i < hitList.Length; i++)
            {
                float start = hitList[i];
                float end = start + ColResetTime;

                if (comboTimer >= start && comboTimer < end)
                {
                    hitActive = true;
                    break;
                }
            }

            if (hitActive)
            {
                if (!playerStatusManager.GB_Box.activeSelf)
                {
                    if (comboStep != 0) isFirstAttack = false;

                    int seIndex = Random.Range(0, 3);

                    switch (seIndex)
                    {
                        case 0: SoundManager.Instance.PlaySE("攻撃音高"); break;
                        case 1: SoundManager.Instance.PlaySE("攻撃音中"); break;
                        case 2: SoundManager.Instance.PlaySE("攻撃音低"); break;
                    }
                }

                playerStatusManager.GB_AtkOnOff(true);
            }
            else
            {
                playerStatusManager.GB_AtkOnOff(false);
            }

            // 次のコンボ入力（予約）
            if (attackButtonPressedThisFrame)
            {
                bufferedCombo = true;
                attackButtonPressedThisFrame = false;
            }

            yield return null;
        }

        // ② 攻撃アニメーション終了時に1回だけ OFF
        playerStatusManager.MeleeAtkOnOff(false);
        playerStatusManager.GB_AtkOnOff(false);   // ヒット判定が残らないようにする

        // 次のコンボへ
        if (bufferedCombo)
        {
            comboStep = (comboStep + 1) % playerManager.comboAnims.Length;

            if (comboStep == playerManager.comboAnims.Length - 1)
                playerStatusManager.nowAtkState = "MeleeComboFinish";

            currentCoroutine = StartCoroutine(ComboAttack());
        }
        else
        {
            comboStep = 0;
            playerManager.ComboFlg = false;
            playerStatusManager.nowAtkState = "";
            playerManager.anim.speed = 1.0f;
        }
    }


    public void Stop()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            // 状態リセット
            playerManager.anim.speed = 1.0f;
            playerManager.PlayAnim(playerManager.aNameIdol);
            comboStep = 0;
            attackButtonPressedThisFrame = false;
            bufferedCombo = false;
            playerManager.ComboFlg = false;
            playerStatusManager.MeleeAtkOnOff(false);
            playerStatusManager.GB_AtkOnOff(false);   // ヒット判定が残らないようにする
            playerStatusManager.nowAtkState = "";
            seFlg = false;
        }
    }

}
