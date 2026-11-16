using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 戦闘中のサポートの使用を管理する。2枠それぞれ、1回の戦闘につき1回だけ使える。
/// 使えるサポートはSupportLoadout(ステージ選択前にセットしたもの)から読む。
/// アイコンの上にグレーの画像(slotOverlays)を重ね、使った後に効果時間をかけて埋めていく。
/// </summary>
public class PlayerSupport : MonoBehaviour
{
    public PlayerManager playerManager;

    [Header("表示(任意)")]
    [SerializeField] Image[] slotIcons;                 // 枠ごとのアイコン
    [SerializeField] Image[] slotOverlays;              // 枠ごとのグレーの画像(アイコンの上に重ねる)。使った後、効果時間をかけて円形に埋まる

#if UNITY_EDITOR
    [Header("テスト用(エディタのみ): ステージ選択を通さずに起動したとき、空の枠にこれを使う")]
    [SerializeField] SupportBase[] debugSlots = new SupportBase[SupportLoadout.SlotCount];
#endif

    public SupportBase[] slots = new SupportBase[SupportLoadout.SlotCount];
    bool[] used = new bool[SupportLoadout.SlotCount];

    // 効果時間中のグレー表示用
    float[] durations = new float[SupportLoadout.SlotCount];    // 効果時間
    float[] elapsed = new float[SupportLoadout.SlotCount];      // 使ってからの経過時間
    bool[] running = new bool[SupportLoadout.SlotCount];        // 効果時間中か

    void Start()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = SupportLoadout.Instance.Get(i);
#if UNITY_EDITOR
            if (slots[i] == null && debugSlots != null && i < debugSlots.Length) slots[i] = debugSlots[i];
#endif
            SetupUI(i);
        }
    }

    void Update()
    {
        // 効果時間に合わせてグレーを埋めていく(終わると全部グレーになる)
        for (int i = 0; i < slots.Length; i++)
        {
            if (!running[i]) continue;

            elapsed[i] += Time.deltaTime;
            float ratio = Mathf.Clamp01(elapsed[i] / durations[i]);
            SetOverlay(i, ratio);
            if (ratio >= 1f) running[i] = false;
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

        SupportBase support = slots[index];
        support.Activate(playerManager);

        // 効果時間のあるサポートは、時間をかけてグレーが埋まっていく。
        // 回復や次の攻撃へのバフなど、即時効果のものは使った瞬間に全部グレーにする。
        float duration = support.Duration;
        if (duration > 0f)
        {
            durations[index] = duration;
            elapsed[index] = 0f;
            running[index] = true;
            SetOverlay(index, 0f);
        }
        else
        {
            SetOverlay(index, 1f);
        }
    }

    void SetupUI(int index)
    {
        SupportBase s = slots[index];

        if (slotIcons != null && index < slotIcons.Length && slotIcons[index] != null)
        {
            slotIcons[index].enabled = (s != null);
            if (s != null)
            {
                slotIcons[index].sprite = s.icon;
                slotIcons[index].color = Color.white;
            }
        }

        if (slotOverlays != null && index < slotOverlays.Length && slotOverlays[index] != null)
        {
            Image overlay = slotOverlays[index];
            // 円形(Radial 360)のFilled画像として使う
            overlay.type = Image.Type.Filled;
            overlay.fillMethod = Image.FillMethod.Radial360;
            overlay.fillAmount = 0f;
            overlay.enabled = (s != null);
        }
    }

    void SetOverlay(int index, float fill)
    {
        if (slotOverlays == null || index >= slotOverlays.Length || slotOverlays[index] == null) return;
        slotOverlays[index].fillAmount = Mathf.Clamp01(fill);
    }
}
