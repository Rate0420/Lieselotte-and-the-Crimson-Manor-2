using UnityEngine;

/// <summary>
/// 使った時点の残りHP(最大HPに対する割合)が少ないほど、一定時間の攻撃力上昇が大きい(ブラックウッド)
/// 倍率 = 1 + maxBonus x (1 - HP割合)。効果は使った時点の値で固定される。
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Objects/Support/LowHpAttack")]
public class SupportLowHpAttack : SupportBase
{
    [SerializeField] float maxBonus = 1f;       // HPがほぼ0のときの上昇量(1 = +100%)
    [SerializeField] float duration = 20f;      // 効果時間(秒)

    protected override void Apply(PlayerManager pm)
    {
        if (pm.playerBuffs == null) return;
        var psm = pm.playerStatusManager;
        float hpRatio = psm.MaxHp > 0 ? Mathf.Clamp01((float)psm.CurrentHp / psm.MaxHp) : 1f;
        float multiplier = 1f + maxBonus * (1f - hpRatio);
        Debug.Log("残りHP割合" + hpRatio + "で攻撃力倍率" + multiplier);
        pm.playerBuffs.AddTimedBuff(PlayerBuffs.BuffType.Attack, multiplier, duration);
    }
}
