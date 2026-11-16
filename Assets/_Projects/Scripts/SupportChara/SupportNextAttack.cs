using UnityEngine;

/// <summary>
/// 使った後の最初の攻撃の攻撃力が大きく上がる(澪音)
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Objects/Support/NextAttack")]
public class SupportNextAttack : SupportBase
{
    [SerializeField] float multiplier = 3f;     // 最初の攻撃にかかる倍率

    protected override void Apply(PlayerManager pm)
    {
        if (pm.playerBuffs == null) return;
        pm.playerBuffs.AddNextAttackBuff(multiplier);
    }
}
