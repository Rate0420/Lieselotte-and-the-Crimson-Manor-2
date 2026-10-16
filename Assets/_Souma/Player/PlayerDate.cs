using UnityEngine;

public class PlayerDate : MonoBehaviour
{
    [SerializeField] StaminaGauge staminaGauge;
    [SerializeField][Header("最大HP")] int HP;
    [SerializeField][Header("スタミナの最大値")] float STAMINA;
    [SerializeField][Header("スタミナの回復速度")] float STAMINARECOVERY;
    [SerializeField][Header("移動速度")] float AGE;
    [SerializeField][Header("コンボ猶予時間")] float COMBOGRASETIME;
    [SerializeField][Header("ガードのスタミナ減少")] float GUARDSTANIMADECREASED;
    [SerializeField][Header("パリィ猶予")] float PARRYGRASETIME;
    [SerializeField][Header("パリィ発生フレーム")] float PARRYOCCURRENCESPEED;
    [Header("通常攻撃力")] int ATK;
    [Header("被ダメージ")] int damage;
    [Header("致命攻撃力")] int deadlyATK;
    [Header("攻撃速度")] float attackSpeed;

    int nowHp;
    float nowStamina;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 付呪の処理
        Enchantment();
        // ステータスの初期化
        SetStatus();
    }

    // Update is called once per frame
    void Update()
    {
        // スタミナ関連の処理
        // staminaGauge.StaminaRecovery(nowStamina, STAMINARECOVERY);
    }

    /// <summary>
    /// ステータスの初期化
    /// </summary>
    void SetStatus()
    {
        nowHp = HP;
        nowStamina = STAMINA;
        /*
         * atk = 
         * deadlyATK = 
         * attackSpeed = 
         */
    }

    /// <summary>
    /// 付呪関連の処理
    /// </summary>
    void Enchantment()
    {
        // スクリプトを呼び出す
    }

    /// <summary>
    /// 攻撃の処理
    /// </summary>
    void Attack()
    {

    }

    /// <summary>
    /// 被ダメの処理
    /// </summary>
    void Damage(int damage)
    {

    }

    /// <summary>
    /// ガード関連の処理
    /// </summary>
    void Guard()
    {

    }
}