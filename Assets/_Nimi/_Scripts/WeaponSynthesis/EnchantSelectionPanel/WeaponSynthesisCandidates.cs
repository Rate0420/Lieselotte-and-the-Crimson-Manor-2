using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>武器間の付呪を表示順で組にし、合算分だけを最大+9の候補に分ける。</summary>
public sealed class WeaponSynthesisCandidates : IReadOnlyList<EnchantmentSlot>
{
    private const int MaxMergedLevel = 9;
    private readonly List<CandidateGroup> _groups = new();

    // 大量の候補を実体として並べず、「+9の個数」と「最後のレベル」でまとめて保持する。
    private struct CandidateGroup
    {
        public EnchantmentDataBase.EnchantmentName Name;
        public int FullCount;
        public int LastLevel;
        public int Count => FullCount + (LastLevel > 0 ? 1 : 0);
    }

    public int Count { get; private set; }

    // 画面に必要な候補だけを取り出す。高レベルでも候補数に比例する大量生成を避ける。
    public EnchantmentSlot this[int index]
    {
        get
        {
            if (index < 0 || index >= Count) throw new ArgumentOutOfRangeException(nameof(index));
            foreach (var group in _groups)
            {
                if (index < group.Count)
                    return new EnchantmentSlot(group.Name, index < group.FullCount ? MaxMergedLevel : group.LastLevel);
                index -= group.Count;
            }
            throw new ArgumentOutOfRangeException(nameof(index));
        }
    }

    /// <summary>武器Aの表示順で武器Bの同名付呪と1対1に組み合わせる。同じ武器内の付呪は合算しない。</summary>
    public static WeaponSynthesisCandidates Create(WeaponInstance first, WeaponInstance second)
    {
        var result = new WeaponSynthesisCandidates();
        var firstSlots = GetSlots(first);
        var secondSlots = GetSlots(second);
        // 武器Bの同じ枠を複数回合算しないため、組み合わせ済みの枠を記録する。
        var used = new bool[secondSlots.Count];
        foreach (var slot in firstSlots)
        {
            int match = -1;
            for (int i = 0; i < secondSlots.Count; i++)
                if (!used[i] && secondSlots[i].enchantName == slot.enchantName)
                {
                    match = i;
                    break;
                }
            // 相手武器に同名付呪がなければ、+11などの元のレベルを分割せず残す。
            if (match < 0)
                result.Add(new CandidateGroup { Name = slot.enchantName, LastLevel = Math.Max(1, slot.enchantLevel) });
            else
            {
                used[match] = true;
                // 高レベルのデバッグ武器も、整数の桁あふれや大量生成を起こさず扱う。
                long total = (long)Math.Max(1, slot.enchantLevel) + Math.Max(1, secondSlots[match].enchantLevel);
                result.Add(new CandidateGroup
                {
                    Name = slot.enchantName,
                    // 例：7+7=14なら+9と+5、合計20なら+9・+9・+2に分ける。
                    FullCount = (int)(total / MaxMergedLevel),
                    LastLevel = (int)(total % MaxMergedLevel)
                });
            }
        }
        for (int i = 0; i < secondSlots.Count; i++)
            if (!used[i])
                result.Add(new CandidateGroup { Name = secondSlots[i].enchantName, LastLevel = Math.Max(1, secondSlots[i].enchantLevel) });
        return result;
    }

    /// <summary>元の表示順を維持して最大3枠を取り出し、空枠と「なし」は候補から除外する。</summary>
    private static List<EnchantmentSlot> GetSlots(WeaponInstance weapon)
    {
        var result = new List<EnchantmentSlot>();
        if (weapon?.enchantmentSlots == null) return result;
        for (int i = 0; i < Math.Min(3, weapon.enchantmentSlots.Length); i++)
        {
            var slot = weapon.enchantmentSlots[i];
            if (slot != null && slot.enchantName != EnchantmentDataBase.EnchantmentName.なし) result.Add(slot);
        }
        return result;
    }

    private void Add(CandidateGroup group)
    {
        _groups.Add(group);
        Count = checked(Count + group.Count);
    }

    public IEnumerator<EnchantmentSlot> GetEnumerator()
    {
        for (int i = 0; i < Count; i++) yield return this[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
