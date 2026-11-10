using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 戦闘中のサポートの使用を管理する。2枠それぞれ、1回の戦闘につき1回だけ使える。
/// 使えるサポートはSupportLoadout(ステージ選択前にセットしたもの)から読む。
/// </summary>
public class PlayerSupport : MonoBehaviour
{
    public PlayerManager playerManager;

    [Header("表示(任意)")]
    [SerializeField] Image[] slotIcons;                 // 枠ごとのアイコン
    [SerializeField] Color usedColor = new Color(0.4f, 0.4f, 0.4f, 1f);   // 使用済みの色

#if UNITY_EDITOR
    [Header("テスト用(エディタのみ): ステージ選択を通さずに起動したとき、空の枠にこれを使う")]
    [SerializeField] SupportBase[] debugSlots = new SupportBase[SupportLoadout.SlotCount];
#endif

    SupportBase[] slots = new SupportBase[SupportLoadout.SlotCount];
    bool[] used = new bool[SupportLoadout.SlotCount];

    void Start()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = SupportLoadout.Get(i);
#if UNITY_EDITOR
            if (slots[i] == null && debugSlots != null && i < debugSlots.Length) slots[i] = debugSlots[i];
#endif
            RefreshUI(i);
        }
    }

    /// <summary>
    /// その枠のサポートを使える状態か
    /// </summary>
    public bool CanUse(int index)
    {
        if (index < 0 || index >= slots.Length) return false;
        if (slots[index] == null || used[index]) return false;
        if (playerManager.GameEndFlg || playerManager.dummyFlg) return false;
        if (playerManager.playerStatusManager.CurrentHp <= 0) return false;
        return true;
    }

    /// <summary>
    /// サポートを使う(PlayerManager.SupportAction1/2から呼ぶ)
    /// </summary>
    public void Use(int index)
    {
        if (!CanUse(index)) return;
        used[index] = true;
        slots[index].Activate(playerManager);
        RefreshUI(index);
    }

    void RefreshUI(int index)
    {
        if (slotIcons == null || index >= slotIcons.Length || slotIcons[index] == null) return;
        var s = slots[index];
        slotIcons[index].enabled = (s != null);
        if (s == null) return;
        slotIcons[index].sprite = s.icon;
        slotIcons[index].color = used[index] ? usedColor : Color.white;
    }
}
