using UnityEngine;

/// <summary>
/// サポートキャラの基底クラス。サポートごとにScriptableObjectを作り、能力はApplyに実装する。
/// ステージ選択前に2枠セットし(SupportLoadout)、戦闘中にPlayerSupportから1回だけ使う。
/// </summary>
public abstract class SupportBase : ScriptableObject
{
    [Header("表示")]
    public string displayName;
    [TextArea] public string description;      // 効果内容(短い説明。一覧の行と、セット中の枠に出す)
    [TextArea(3, 10)] public string longDescription;   // 詳しい説明文(セット中の枠の説明欄に出す)
    public Sprite icon;

    [Header("演出(任意)")]
    public GameObject effectPrefab;     // 使用時にプレイヤーの位置へ出すエフェクト
    public string seName;               // 使用時のSE(空なら鳴らさない)

    /// <summary>
    /// 効果時間(秒)。0なら即時効果で、UIでは使った瞬間にグレーアウトする。
    /// 時間のあるサポートは、この時間をかけてアイコンのグレーが埋まっていく。
    /// </summary>
    public virtual float Duration
    {
        get { return 0f; }
    }

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
