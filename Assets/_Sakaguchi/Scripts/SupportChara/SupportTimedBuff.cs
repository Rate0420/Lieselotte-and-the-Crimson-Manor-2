using UnityEngine;

/// <summary>
/// 一定時間のバフ。buffTypeを変えるだけで次の能力になる。
/// ・小夜: PostureDamage(弾き/パリィでの体勢削り量。持っている方のキャラに効く)
/// ・加流: AttackSpeed(攻撃速度)
/// ・澪凜: Attack(攻撃力)
/// ・陽向: ComboTime(コンボ受付時間)
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Objects/Support/TimedBuff")]
public class SupportTimedBuff : SupportBase
{
    [SerializeField] PlayerBuffs.BuffType buffType = PlayerBuffs.BuffType.Attack;
    [SerializeField] float multiplier = 1.3f;   // 倍率(1.3 = +30%)
    [SerializeField] float duration = 20f;      // 効果時間(秒)

    protected override void Apply(PlayerManager pm)
    {
        if (pm.playerBuffs == null) return;
        pm.playerBuffs.AddTimedBuff(buffType, multiplier, duration);
    }
}
