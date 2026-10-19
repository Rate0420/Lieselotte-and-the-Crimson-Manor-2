using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/EnemyAttack")]
public class EnemyAttack : Enemy_AttackBase
{
    [SerializeField] PlayerParry.parryDir[] dir;
    [SerializeField] float[] atkiInterval;
    [SerializeField] float[] atkTime;
    [SerializeField] int[] atkDamage;
    [SerializeField] string atkAnimName;
    [SerializeField] float endLagTime;

    public override void Attack(EnemyController ec)
    {
        ec.StartCoroutine(Atk(ec));
    }

    IEnumerator Atk(EnemyController ec)
    {
        ec.cancelAttack = false;
        ec.PlayAnim(atkAnimName);

        // 各攻撃ヒットごとに処理
        for (int i = 0; i < atkiInterval.Length; i++)
        {
            bool interrupted = false;

            // ▼ インターバル待機
            ec.currentAttackDir = (int)dir[i];
            float warn = Mathf.Min(ec.warningTime, atkiInterval[i]);

            // 予告前待機
            yield return ec.StartCoroutine(WaitInterruptible(ec, atkiInterval[i] - warn, r => interrupted = r));
            if (interrupted) { EndImmediately(ec); yield break; }

            // ★ここで通知
            ec.ShowAttackWarning();

            // 予告時間待機
            yield return ec.StartCoroutine(WaitInterruptible(ec, warn, r => interrupted = r));
            if (interrupted) { EndImmediately(ec); yield break; }
            if (interrupted) { EndImmediately(ec); yield break; }

            // ▼ 攻撃判定 ON

            ec.currentAttackDamage = atkDamage[i];
            ec.EnableAttackCollider(true);
            ec.AtkEnd();
            // ▼ 攻撃時間
            yield return ec.StartCoroutine(WaitInterruptible(ec, atkTime[i], r => interrupted = r));
            ec.EnableAttackCollider(false);
            ec.HideAttackWarning();


            if (interrupted) { EndImmediately(ec); yield break; }
        }

        // ▼ 攻撃後硬直
        bool interruptedEnd = false;
        yield return ec.StartCoroutine(WaitInterruptible(ec, endLagTime, r => interruptedEnd = r));
        if (interruptedEnd) { EndImmediately(ec); yield break; }

        // ▼ アニメーション終了まで待つ
        yield return new WaitUntil(() =>
            ec.cancelAttack ||
            ec.enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f
        );

        ec.isAttackFinished = true;
    }

    // 中断された時のまとめ処理
    void EndImmediately(EnemyController ec)
    {
        ec.EnableAttackCollider(false);
        ec.HideAttackWarning();   // 予告表示が残らないようにする
        ec.isAttackFinished = true;
    }

    // ================================
    //   中断可能な Wait（修正版）
    // ================================
    IEnumerator WaitInterruptible(EnemyController ec, float time, System.Action<bool> resultSetter)
    {
        float t = 0f;

        while (t < time)
        {
            if (ec.cancelAttack)
            {
                resultSetter(true); // 中断フラグ
                yield break;
            }

            t += Time.deltaTime;
            yield return null;
        }

        resultSetter(false); // 中断されずに終了
    }
}
