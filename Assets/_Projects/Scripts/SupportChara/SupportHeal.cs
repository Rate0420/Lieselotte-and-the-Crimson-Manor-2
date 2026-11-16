using UnityEngine;

/// <summary>
/// HP回復(冥衣)
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Objects/Support/Heal")]
public class SupportHeal : SupportBase
{
    [Range(0f, 1f)]
    [SerializeField] float healRate = 0.4f;     // 最大HPに対する回復割合

    protected override void Apply(PlayerManager pm)
    {
        var psm = pm.playerStatusManager;
        psm.Heal(Mathf.CeilToInt(psm.MaxHp * healRate));
    }
}
