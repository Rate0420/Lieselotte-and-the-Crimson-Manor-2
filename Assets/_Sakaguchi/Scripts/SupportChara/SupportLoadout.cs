using System;
using UnityEngine;

/// <summary>
/// メニューで選んだサポート(2枠)を保持し、戦闘シーンへ引き継ぐ常駐オブジェクト(DontDestroyOnLoad)。
/// WeaponInventoryと同じように、SupportLoadout.Instanceからアクセスする。
/// シーンに置かなくても、最初にInstanceへアクセスしたときに自動で作られる。
/// ・メニュー(SupportEquipmentController)が Toggle / TrySet で枠を設定する
/// ・戦闘シーンのPlayerSupportが、開始時に Get で取り出す
/// (アプリを終了すると空に戻る。セーブへの保存は含まない)
/// </summary>
public class SupportLoadout : MonoBehaviour
{
    public const int SlotCount = 2;

    public enum ToggleResult
    {
        Equipped,       // 空き枠にセットした
        Unequipped,     // セット済みだったので外した
        SlotsFull       // 枠が埋まっていてセットできなかった
    }

    static SupportLoadout instance;
    static bool applicationQuitting;

    /// <summary>
    /// 枠の中身が変わったときの通知(選択画面の表示更新用)
    /// </summary>
    public event Action OnLoadoutChanged;

    readonly SupportBase[] slots = new SupportBase[SlotCount];

    public static SupportLoadout Instance
    {
        get
        {
            if (instance == null && !applicationQuitting)
            {
                var go = new GameObject("SupportLoadout");
                instance = go.AddComponent<SupportLoadout>();
            }
            return instance;
        }
    }

    // エディタで「ドメインのリロード」を切っていても、起動時に状態を戻す
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        instance = null;
        applicationQuitting = false;
    }

    void Awake()
    {
        // シーンに置いた場合に、2つ目以降を作らない
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }

    void OnApplicationQuit()
    {
        applicationQuitting = true;
    }

    public SupportBase Get(int index)
    {
        return (index >= 0 && index < SlotCount) ? slots[index] : null;
    }

    /// <summary>
    /// そのサポートがセットされている枠の番号(セットされていなければ-1)
    /// </summary>
    public int IndexOf(SupportBase support)
    {
        if (support == null) return -1;
        for (int i = 0; i < SlotCount; i++)
        {
            if (slots[i] == support) return i;
        }
        return -1;
    }

    /// <summary>
    /// セット済みなら外し、未セットなら空き枠(番号の若い順)にセットする
    /// </summary>
    public ToggleResult Toggle(SupportBase support)
    {
        int slot = IndexOf(support);
        if (slot >= 0)
        {
            slots[slot] = null;
            Notify();
            return ToggleResult.Unequipped;
        }

        for (int i = 0; i < SlotCount; i++)
        {
            if (slots[i] != null) continue;
            slots[i] = support;
            Notify();
            return ToggleResult.Equipped;
        }
        return ToggleResult.SlotsFull;
    }

    /// <summary>
    /// 枠を指定してセットする(nullで外す)。同じサポートを2枠に入れることはできず、その場合はfalse
    /// </summary>
    public bool TrySet(int index, SupportBase support)
    {
        if (index < 0 || index >= SlotCount) return false;
        int other = IndexOf(support);
        if (other >= 0 && other != index) return false;
        if (slots[index] == support) return true;
        slots[index] = support;
        Notify();
        return true;
    }

    /// <summary>
    /// 枠(0 = X, 1 = Y)にサポートを入れる。
    /// ・すでにその枠に入っているサポートなら外す
    /// ・もう一方の枠に入っているサポートなら、2つの枠を入れ替える(同じサポートが2枠に入ることはない)
    /// ・それ以外は、その枠の中身を置き換える
    /// </summary>
    public void Assign(int index, SupportBase support)
    {
        if (index < 0 || index >= SlotCount || support == null) return;

        if (slots[index] == support)
        {
            slots[index] = null;
            Notify();
            return;
        }

        int other = IndexOf(support);
        if (other >= 0) slots[other] = slots[index];
        slots[index] = support;
        Notify();
    }

    public void Clear()
    {
        for (int i = 0; i < SlotCount; i++) slots[i] = null;
        Notify();
    }

    void Notify()
    {
        if (OnLoadoutChanged != null) OnLoadoutChanged();
    }
}
