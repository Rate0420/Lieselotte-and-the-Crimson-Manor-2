using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Enchantment", menuName = "Enchantment")]
public class Enchantment : ScriptableObject
{
    public List<EnchantmentDataBase> EnchantmentData;
}

[Serializable]
public class EnchantmentDataBase
{
    // 付呪の効果名
    public enum EnchantmentName
    {
        HP増加,
        スタミナ回復上昇,
        攻撃力増加,
        致命強化,
        初撃強化,
        フィニッシュ強化,
        戦技強化,
        コンボ時間増加,
        パリィ猶予増加,
        攻撃速度上昇,
        移動速度上昇,
        ガード強化,
        //固有スキル

        //ダメージ
        パリィ時ダメージ,
        ジャスガ時初撃強化,

        //強化
        パリィ強化,
        ジャスガ強化,
        なし
    }

    [Header("効果の名前")]
    public EnchantmentName Name;

    [Header("効力(%)(Lv)")]
    public float[] Efficacy;
}