using UnityEngine;

/// <summary>
/// サポートキャラの基底クラス。サポートごとにScriptableObjectを作り、能力はApplyに実装する。
/// ステージ選択前に2枠セットし(SupportLoadout)、戦闘中にPlayerSupportから1回だけ使う。
/// </summary>
public abstract class SupportBase : ScriptableObject
{
    [Header("表示")]
    public string displayName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("演出(任意)")]
    public GameObject effectPrefab;     // 使用時にプレイヤーの位置へ出すエフェクト
    public string seName;               // 使用時のSE(空なら鳴らさない)

    /// <summary>
    /// サポートを使う(PlayerSupportから呼ぶ)
    /// </summary>
    public void Activate(PlayerManager pm)
    {
        if (effectPrefab != null) Instantiate(effectPrefab, pm.transform.position, Quaternion.identity, pm.transform);
        if (!string.IsNullOrEmpty(seName)) SoundManager.Instance.PlaySE(seName);
        Apply(pm);
    }

    /// <summary>
    /// サポートごとの能力
    /// </summary>
    protected abstract void Apply(PlayerManager pm);
}
