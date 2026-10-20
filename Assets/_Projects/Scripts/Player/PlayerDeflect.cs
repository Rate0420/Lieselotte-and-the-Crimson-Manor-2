using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ヴェルミリオン専用: 弾き(攻撃の直前のガード = ジャストガード)の管理
/// ・弾きに成功すると敵の体勢を削り、弾いた回数をカウントする
/// ・カウントに応じて「次の攻撃」のダメージが上がる(攻撃を出す or 被弾でカウントはリセット)
/// 弾き判定そのものはPlayerDamageReceiverのジャストガード(enableJustGuard)が行い、成功時にOnDeflectを呼ぶ。
/// </summary>
public class PlayerDeflect : MonoBehaviour
{
    public PlayerManager playerManager;
    public PlayerStatusManager playerStatusManager;

    [Header("弾きの性能")]
    [SerializeField] float postureDamage = 8f;      // 1回弾くごとに削る敵の体勢量(パリィより少なめ)
    [SerializeField] float buffPerDeflect = 0.2f;   // 1回弾くごとに次の攻撃のダメージが増える割合(0.2 = +20%)
    [SerializeField] int maxCount = 5;              // カウントの上限

    [Header("表示(任意)")]
    [SerializeField] Text countText;                // 弾き回数の表示

    int count;

    public int Count
    {
        get { return count; }
    }

    void Start()
    {
        UpdateUI();
    }

    /// <summary>
    /// 弾きに成功したとき(PlayerDamageReceiverから呼ぶ)
    /// </summary>
    public void OnDeflect(EnemyController enemy)
    {
        if (count < maxCount) count++;
        UpdateUI();

        // 敵の体勢を削る(0になったらダウン)
        if (enemy != null) enemy.DamagePosture(postureDamage);

        // スコア用の「パリィ回数」にも数える
        playerStatusManager.parryCount++;
    }

    /// <summary>
    /// 攻撃を出すときに呼ぶ。今のカウント分のダメージ倍率を返し、カウントをリセットする。
    /// (カウントが0なら1.0を返す)
    /// </summary>
    public float ConsumeBuff()
    {
        float rate = 1f + count * buffPerDeflect;
        ResetCount();
        return rate;
    }

    /// <summary>
    /// カウントをリセットする(被弾時など)
    /// </summary>
    public void ResetCount()
    {
        if (count == 0) return;
        count = 0;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (countText != null) countText.text = count > 0 ? "弾き x" + count : "";
    }
}
