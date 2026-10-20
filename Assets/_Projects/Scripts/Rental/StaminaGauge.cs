using UnityEngine;
using UnityEngine.UI;

public class StaminaGauge : MonoBehaviour
{
    [SerializeField] Image staminaFillImage;  // スタミナゲージImage (FillAmountで制御)
    public float recoverySpeed = 10f; // 回復速度     (武器データから読む) 
    [SerializeField] int maxStamina = 100;
    public float currentStamina = 100f;
    [SerializeField] float cost = 20f;  // デバッグ用消費量
    [SerializeField] PlayerManager playerManager;

    [SerializeField] float smoothTime = 0.2f;  // FillAmount補間時間
    float displayFill; // 表示上のFillAmount
    float velocity;

    void Start()
    {
        currentStamina = maxStamina;
        displayFill = 1f;
        staminaFillImage.fillAmount = displayFill;
    }

    void Update()
    {
        // 回復
        // ニュートラル状態(ガード中・被弾中・ダウン中ではない)のときだけ回復する
        if (playerManager.IsNeutral)
        {
            currentStamina += Time.deltaTime * recoverySpeed;
            if (currentStamina > maxStamina)
                currentStamina = maxStamina;
        }

#if UNITY_EDITOR
        // デバッグ消費
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ConsumeStamina(cost);
        }
#endif

        // FillAmountを滑らかに追従させる
        float targetFill = currentStamina / maxStamina;
        displayFill = Mathf.SmoothDamp(displayFill, targetFill, ref velocity, smoothTime);
        staminaFillImage.fillAmount = displayFill;
    }

    /// <summary>
    /// スタミナ消費
    /// </summary>
    public void ConsumeStamina(float amount)
    {
        currentStamina -= amount;
        if (currentStamina < 0) currentStamina = 0;
    }
}
