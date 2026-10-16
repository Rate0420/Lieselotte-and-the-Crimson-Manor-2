using UnityEngine;

[CreateAssetMenu(fileName = "EnchantmentData", menuName = "Scriptable Objects/EnchantmentData")]
public class EnchantmentData : ScriptableObject
{
    // UI表示用の名前
    public string displayName;

    // どのステータスに作用するか
    public AffectedStat targetStat;

    // レベル 1～9
    public int maxLevel = 9;

    // レベルごとの効果値
    // 例:Lv1 = 10, Lv2 = 20, Lv3 = 40...
    public int[] efficacyPerLevel;


    /// <summary>
    /// 指定されたレベルの効果値を返す(配列範囲の場合は最後の値か0で返す)
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public int GetValue(int level)
    {
        if (efficacyPerLevel == null || efficacyPerLevel.Length == 0) return 0;
        int idx = Mathf.Clamp(level - 1, 0, efficacyPerLevel.Length - 1);
        return efficacyPerLevel[idx];
    }
}

public enum AffectedStat
{
    HP,
    StaminaRecoveryRate,
    AttackPower,
    FirstAttackBoost,      // boolフラグ
    CriticalDamage,
    AttackSpeed,
    MoveSpeed,
    ComboBufferTime,
    ParryBufferTime,
    GuardStaminaCost
}
