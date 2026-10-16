using UnityEngine;
using UnityEngine.UI;

public class EnemyPostureGauge : MonoBehaviour
{
    // 敵の体勢ゲージを管理するスクリプト
    public int maxPosture;      // 体勢ゲージの最大値
    [SerializeField] float postureRecoveryRate; // 体勢ゲージの回復速度
    public float currentPosture;                  // 現在の体勢ゲージ値
    [SerializeField] Image postureFillImage;  // 体勢ゲージImage (FillAmountで制御)
    float displayFill; // 表示上のFillAmount
        [SerializeField] float smoothTime = 0.2f;  // FillAmount補間時間
    float velocity;

    float cost = 20f;  // デバッグ用消費量

    void Start()
    {
        currentPosture = maxPosture;
        displayFill = 1f;
        postureFillImage.fillAmount = displayFill;
    }

    void Update()
    {
        // デバッグ消費
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            ConsumePosture(cost);
        }
        if(Input.GetKeyDown(KeyCode.LeftAlt))
        {
            RecoveryPosture(maxPosture);
        }

        // FillAmountを滑らかに追従させる
        float targetFill = currentPosture / maxPosture;
        displayFill = Mathf.SmoothDamp(displayFill, targetFill, ref velocity, smoothTime);
        postureFillImage.fillAmount = displayFill;
    }

    /// <summary>
    /// 体勢ゲージ消費
    /// </summary>
    public void ConsumePosture(float amount)
    {
        currentPosture -= amount;
        if (currentPosture < 0) currentPosture = 0;
    }

    /// <summary>
    /// 体勢ゲージ回復、体勢がゼロになった時にECから呼び出す
    /// </summary>
    /// <param name="targetPosture">この値まで回復させる </param>
    public void RecoveryPosture(int targetPosture)
    {
        currentPosture = targetPosture;
    }
}
