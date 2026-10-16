using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerParry : MonoBehaviour
{
    public PlayerManager playerManager;
    public PlayerStatusManager playerStatusManager;
    public float parryStartTime;    // パリィ判定がボタン押下後のどのくらいのタイミングで出るか
    public float parryGrace;        // パリィの猶予
    public float gapInParryMotion;  // パリィの後隙
    public bool isParrying;
    public GameObject parryCol;
    public int parryState;
    Coroutine currentCoroutine;
    Coroutine currentFlgCoroutine;
    public enum parryDir
    {
        上,
        下,
        左,
        右
    }


    public void ParryAction(parryDir dir)
    {
        // もしcurrentCoroutineが動いていたら停止させる
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            if (currentFlgCoroutine != null) StopCoroutine(currentFlgCoroutine);
            // 止めたParry()は最後の後始末まで走らないので、ここで判定を片付ける
            isParrying = false;
            parryCol.SetActive(false);
        }
        currentCoroutine = StartCoroutine(Parry(dir));
    }

    IEnumerator Parry(parryDir dir)
    {

        playerStatusManager.StaminaRend(playerStatusManager.parryStamina);
        Debug.Log("parry2");
        yield return new WaitForSeconds(parryStartTime);
        // パリィ開始
        parryCol.SetActive(true);
        isParrying = true;
        switch (dir)
        {
            case parryDir.上:
                playerManager.ContinuousPlayAnim(playerManager.aNameParry_U, 0);
                parryState = (int)parryDir.上;
                break;
            case parryDir.下:
                playerManager.ContinuousPlayAnim(playerManager.aNameParry_D, 0);
                parryState = (int)parryDir.下;
                break;
            case parryDir.左:
                playerManager.ContinuousPlayAnim(playerManager.aNameParry_L, 0);
                parryState = (int)parryDir.左;
                break;
            case parryDir.右:
                playerManager.ContinuousPlayAnim(playerManager.aNameParry_R, 0);
                parryState = (int)parryDir.右;
                break;
        }
        currentFlgCoroutine = StartCoroutine(PallyFlgController());
        yield return new WaitForSeconds(parryGrace);
        // パリィ終了
        isParrying = false;
        parryCol.SetActive(false);
        //yield return new WaitForSeconds(gapInParryMotion);
        // 後隙終了
        //playerManager.ParryFlg = false;
    }

    IEnumerator PallyFlgController()
    {
        // 現在再生中のアニメーションが終わり、gapInParryMotion分待ったらfalseにする

        // 再生中アニメーションの情報を取得
        var state = playerManager.anim.GetCurrentAnimatorStateInfo(0);

        // 終了待ち（normalizedTimeが1以上になるまで）
        while (state.normalizedTime < 1.0f)
        {
            state = playerManager.anim.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }
        //yield return new WaitForSeconds(gapInParryMotion);
        playerManager.ParryFlg = false;
    }

    public void Stop()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            if (currentFlgCoroutine != null) StopCoroutine(currentFlgCoroutine);
            // 状態リセット
            playerManager.PlayAnim(playerManager.aNameIdol);
            playerManager.ParryFlg = false;
            parryCol.SetActive(false);
            isParrying = false;
        }
    }
}
