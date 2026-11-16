using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnchantmentTable", menuName = "EnchantmentTable")]
public class EnchantmentTable : ScriptableObject
{
    public List<EnchantmentTablesDataBase> EnchantmentTablesData;
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

}