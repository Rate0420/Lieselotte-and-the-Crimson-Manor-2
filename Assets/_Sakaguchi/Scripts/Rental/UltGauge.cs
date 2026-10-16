using UnityEngine;
using UnityEngine.UI;

public class UltGauge : MonoBehaviour
{
    [SerializeField] Slider ultGauge;
    [SerializeField] int maxUltGauge;
    [SerializeField] int cost;
    public float nowUltGauge;

    void Start()
    {
        // 初期化処理
        nowUltGauge = 0;
        nowUltGauge = Ult(0, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            nowUltGauge = Ult(30, true);
            if (maxUltGauge < nowUltGauge) nowUltGauge = maxUltGauge;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            nowUltGauge = Ult(cost, false);
        }
    }

    /// <summary>
    /// 必殺技ゲージを変更する処理
    /// </summary>
    /// <param name="Amount">変動する値</param>
    /// <param name="fluctuation">trueなら増やすfalseなら減らす</param>
    /// <returns></returns>
    public float Ult(float Amount, bool fluctuation)
    {
        // 必殺技ゲージを増加させる処理
        if (fluctuation)
        {
            nowUltGauge += Amount;
            if (maxUltGauge < nowUltGauge)
                nowUltGauge = maxUltGauge;
        }

        // 必殺技ゲージを減少させる時
        if (!fluctuation)
        {
            // 必殺技ゲージがコストより小さければ処理終わり
            if (nowUltGauge < Amount)
            {
                Debug.Log("終わり");
                ultGauge.value = nowUltGauge / maxUltGauge;
                return nowUltGauge;
            }

            // 必殺技ゲージをコスト分消費する
            else nowUltGauge -= Amount;
            Debug.Log("減らしたよ");
            // 必殺技の処理
        }
        // ゲージに反映させる
        ultGauge.value = nowUltGauge / maxUltGauge;
        return nowUltGauge;
    }
}