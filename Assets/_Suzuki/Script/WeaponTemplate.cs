using UnityEngine;

/// <summary>
/// 武器のカテゴリー
/// </summary>
public enum WeaponType
{
    sword_Shield,
    greatsword,
    rapier,
    vermilion1,
    // カテゴリーを増やすならここを追加
}

/// <summary>
/// 武器ステータスの基盤となるクラス
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Objects/WeaponTemplate")]
public class WeaponTemplate : ScriptableObject
{
    [Header("基本情報")]
    public string weaponName;             // 武器の識別名
    public string displayName;            // UIなどに表示させる名前
    [TextArea] public string description; // (※必要であれば)武器の説明
    public WeaponType weaponType;         // 武器種
    public Sprite weaponIcon;             // 武器アイコン
    public GameObject weaponPrefab;       // 武器のプレハブ

    [Header("付呪の影響があるステータスの初期値")]
    public int basePower;                     // 攻撃力
    public int baseHP;                        // 体力の最大値
    public float baseStaminaRecoverySpeed;    // スタミナ回復速度
    public float baseAttackSpeed;             // 攻撃速度
    public float baseMoveSpeed;               // 移動速度
    public float baseFatalAttackDamage;       // 致命攻撃の威力
    public float baseParryReceptionTime;      // パリィ受付時間
    public float baseComboGraceTime;          // コンボ猶予時間
    public float baseGuradStaminaLoss;        // ガード時のスタミナ減少量

    [Header("付呪で変わらないステータス")]
    public int MAX_Stamina;               // スタミナの最大値
    public float weaponArtsDamage;        // 武器固有攻撃(戦技)の威力
    public int parryStartFrame;           // パリィの発生フレーム
    public float parryEndFrame;           // パリィ後の隙が出来るフレーム
    public float parryFailStaminaCost;    // パリィ失敗時のスタミナ減少量
    public MotionSet CharacterMotionSet;  // 各種モーション
    public float hitboxStartTime;         // 攻撃判定の発生時間
    public float hitboxDuration;          // 攻撃判定の長さ
    public UniqueAttack UniqueAttack;     // 武器固有の攻撃（戦技）に関する情報
    public HitTiming[] comboHitData;
}

/// <summary>
/// モーション格納用クラス
/// 必要なモーションはその数分ここへ宣言する必要あり
/// </summary>
[System.Serializable]
public class MotionSet
{
    public RuntimeAnimatorController AnimatorController; // 武器専用のアニメーターコントローラー
    public float[] AttackMotionTimes;          // 各攻撃モーションの再生時間
    public float[] AttackColliderTiming;       // 各攻撃モーションの当たり判定発生時間
    public Vector3 AttackColliderSize;         // 攻撃判定の大きさ
    public Vector3 AttackColliderPosition;     // 攻撃判定の位置
}

[System.Serializable]
public class HitTiming
{
    public float[] hitTimes;
}