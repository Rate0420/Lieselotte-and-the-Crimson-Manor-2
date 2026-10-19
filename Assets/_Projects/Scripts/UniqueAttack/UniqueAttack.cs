using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Scriptable Objects/UniqueAttack")]
public class UniqueAttack : UniqueAttackBase
{
    // 各種攻撃スクリプトは同じような感じで作る
    // 攻撃の種類ごとにScriptableObjectを作成し、EnemyControllerで管理するイメージ

    [SerializeField] float[] atkiInterval;            // 各攻撃間のインターバル
    [SerializeField] float[] atkTime;                // 各攻撃の判定時間
    [SerializeField] int[] atkDamage;            // 各攻撃のダメージ量
    [SerializeField] string atkAnimName;         // 攻撃アニメーション名
    [SerializeField] float endLagTime;          // 攻撃後の硬直時間

    [SerializeField] float invincibleTime = 0.5f;      // 攻撃後の無敵時間

    const int UA_condition = 10; // 固有技発動に必要なコンボ数

    public override void Attack(PlayerStatusManager psm)
    {
        if (psm.playerManager.comboManager.UniqueCombo >= UA_condition)
        {
            psm.playerManager.UAFlg = true;
            psm.StartCoroutine(Atk(psm));
            // 発動したのでコンボ数をリセット
            psm.playerManager.comboManager.ResetUniqueCombo();
        }
    }

    IEnumerator Atk(PlayerStatusManager psm)
    {
        // コンボのタイマーを止める
        psm.playerManager.comboManager.comboTimerFlg = false;
        // 攻撃処理
        // まずアニメーションを再生
        psm.playerManager.PlayAnim(atkAnimName);
        psm.nowAtkState = "UniqueAttack";
        psm.playerManager.UAFlg = true;

        // ここからアニメーションが終わるまでの攻撃の判定の処理を入れる
        for (int i = 0; i < atkiInterval.Length; i++)
        {
            //psm.UniqueAttack = atkDamage[i];
            yield return new WaitForSeconds(atkiInterval[i]);
            // 攻撃判定開始
            psm.nowAtkState = "UniqueAttack";
            psm.UA_OnOff(true);

            // SE再生
            int seIndex = Random.Range(0, 3);

            switch (seIndex)
            {
                case 0:
                    SoundManager.Instance.PlaySE("攻撃音高");
                    break;
                case 1:
                    SoundManager.Instance.PlaySE("攻撃音中");
                    break;
                case 2:
                    SoundManager.Instance.PlaySE("攻撃音低");
                    break;
            }

            yield return new WaitForSeconds(atkTime[i]);
            // 攻撃判定終了
            psm.UA_OnOff(false);
        }

        // 現在再生中のアニメーションが完全に終わるまで待機
        yield return new WaitUntil(() => psm.playerManager.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        psm.nowAtkState = "";
        psm.playerManager.UAFlg = false;
        // コンボのタイマーを再開
        psm.playerManager.comboManager.comboTimerFlg = true;
        psm.playerManager.GiveInvincible(invincibleTime);
        psm.playerManager.ChangeColorTrail(Color.white);
    }
}
