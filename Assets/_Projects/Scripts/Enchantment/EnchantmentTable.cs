using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnchantmentTable", menuName = "EnchantmentTable")]
public class EnchantmentTable : ScriptableObject
{
    public List<EnchantmentTablesDataBase> EnchantmentTablesData;
    // 取得済みの重複なしスキル
    private List<string> acquiredUniqueSkills = new List<string>();
}

[Serializable]
public class EnchantmentTablesDataBase
{
    [Header("移動速度上昇の確率")]
    public int AgeUp;

    [Header("ガード強化の確率")]
    public int GuardReinforcement;

    [Header("致命強化の確率")]
    public int DeadlyAttack;

    [Header("初撃強化の確率")]
    public int FirstCombo;

    [Header("最終段強化の確率")]
    public int FinishCombo;

    [Header("HP増加の確率")]
    public int HP;

    [Header("スタミナ回復上昇の確率")]
    public int SK;

    [Header("戦技強化の確率")]
    public int WeaponArt;

    [Header("攻撃力増加の確率")]
    public int AU;

    [Header("コンボ時間増加の確率")]
    public int ComboGracePeriodUP;

    [Header("パリィ猶予時間増加の確率")]
    public int ParryGraceTimeUP;

    [Header("攻撃速度上昇の確率")]
    public int AttackIntervalUP;

    [Header("残HPによって攻撃力上昇")]
    public int AttackRateUP;
    //重複なしスキル
    [Header("毒付与")]
    public int poison;
    //固有スキル
    [Header("パリィ時相手に反撃")]
    public int ParryAttack;
    [Header("ジャスガすると次の初撃強化")]
    public int JGFirstCombo;
    [Header("パリィ強化")]
    public int ParryUp;
    [Header("ジャスガ強化")]
    public int JGUp;
}