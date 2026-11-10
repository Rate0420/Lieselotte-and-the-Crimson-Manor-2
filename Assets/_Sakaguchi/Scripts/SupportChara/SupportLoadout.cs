using UnityEngine;

/// <summary>
/// ステージ選択前にセットしたサポート(2枠)を、ステージのSceneへ持ち越すための入れ物。
/// 選択画面からはTrySetでセットし、戦闘中はPlayerSupportがGetで読む。
/// (アプリを終了すると空に戻る。セーブへの保存は含まない)
/// </summary>
public static class SupportLoadout
{
    public const int SlotCount = 2;

    static SupportBase[] slots = new SupportBase[SlotCount];

    // エディタで「ドメインのリロード」を切っていても、起動時に空へ戻す
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetOnLaunch()
    {
        slots = new SupportBase[SlotCount];
    }

    public static SupportBase Get(int index)
    {
        return (index >= 0 && index < SlotCount) ? slots[index] : null;
    }

    /// <summary>
    /// 枠にサポートをセットする(nullで外す)。同じサポートを2枠に入れることはできず、その場合はfalse
    /// </summary>
    public static bool TrySet(int index, SupportBase support)
    {
        if (index < 0 || index >= SlotCount) return false;
        if (support != null)
        {
            for (int i = 0; i < SlotCount; i++)
            {
                if (i != index && slots[i] == support) return false;
            }
        }
        slots[index] = support;
        return true;
    }

    public static void Clear()
    {
        slots = new SupportBase[SlotCount];
    }
}
