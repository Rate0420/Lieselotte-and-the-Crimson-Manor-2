using System.Collections;
using UnityEngine;

public class Player_GB_Attack : MonoBehaviour
{
    public PlayerManager playerManager;
    public PlayerStatusManager playerStatusManager;
    public float GB_AttackTime = 0.5f;  // ステータスマネージャで設定すること
    public float GBColOffset;
    // 仮システム
    float ColResetTime = 0.1f;

    Coroutine currentCoroutine;

    bool seFlg = false;

    public void GBAttack()
    {
        currentCoroutine = StartCoroutine(GB_Attack());
    }

    IEnumerator GB_Attack()
    {
        seFlg = false;
        // その他攻撃フラグをオンにする
        playerManager.GB_AttackFlg = true;
        // ステータスマネージャに現在の攻撃情報を保存する
        playerStatusManager.nowAtkState = "GB_Attack";
        playerStatusManager.GB_AtkOnOff(true);
        playerManager.ContinuousPlayAnim(playerManager.aNameGBAttack, 0.01f);
        float timer = 0f;
        while (timer < GB_AttackTime)
        {
            timer += Time.deltaTime;
            if (timer < ColResetTime + GBColOffset && timer > GBColOffset)
            {
                seFlg = true;
                playerStatusManager.GB_AtkOnOff(true);
            }
            else
            {
                playerStatusManager.GB_AtkOnOff(false);
            }
            yield return null;
        }

        // 終了
        // ステータスマネージャでその他攻撃フラグをオフにする
        playerStatusManager.nowAtkState = "";
        playerManager.GB_AttackFlg = false;
        playerStatusManager.GB_AtkOnOff(false);

    }

    public void Stop()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            playerManager.GB_AttackFlg = false;
            playerStatusManager.GB_AtkOnOff(false);
            playerStatusManager.nowAtkState = "";
            seFlg = false;

            Debug.Log("stop");
        }
    }
}
