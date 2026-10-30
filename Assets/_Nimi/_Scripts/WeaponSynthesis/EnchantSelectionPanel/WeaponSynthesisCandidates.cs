using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>武器ごとに同名付呪を小さい順で組にし、上限を超えた分を切り捨てて候補を作る。</summary>
public sealed class WeaponSynthesisCandidates : IReadOnlyList<EnchantmentSlot>
{
    // 合算した付呪1候補の上限。超過分を別の候補として表示しない。
    private const int MaxMergedLevel = 10;
    private readonly List<EnchantmentSlot> _candidates = new();

    public int Count => _candidates.Count;

    // 取り出した候補を変更しても、保持している候補の内容には影響させない。
    public EnchantmentSlot this[int index]
    {
        get
        {
            if (index < 0 || index >= Count) throw new ArgumentOutOfRangeException(nameof(index));
            var slot = _candidates[index];
            return new EnchantmentSlot(slot.enchantName, slot.enchantLevel);
        }
    }

    /// <summary>同じ種類の付呪を武器ごとに昇順で並べ、小さいもの同士を1対1で合算する。</summary>
    public static WeaponSynthesisCandidates Create(WeaponInstance first, WeaponInstance second)
    {
        var result = new WeaponSynthesisCandidates();
        var firstSlots = GetSlots(first);
        var secondSlots = GetSlots(second);
        // 付呪の種類は、武器A、武器Bの順に初めて登場した並びを維持する。
        var processed = new HashSet<EnchantmentDataBase.EnchantmentName>();
        foreach (var slot in firstSlots)
            if (processed.Add(slot.enchantName)) result.AddCandidates(slot.enchantName, firstSlots, secondSlots);
        foreach (var slot in secondSlots)
            if (processed.Add(slot.enchantName)) result.AddCandidates(slot.enchantName, firstSlots, secondSlots);
        return result;
    }

    private void AddCandidates(EnchantmentDataBase.EnchantmentName name, List<EnchantmentSlot> first, List<EnchantmentSlot> second)
    {
        var firstLevels = GetSortedLevels(first, name);
        var secondLevels = GetSortedLevels(second, name);
        int pairedCount = Math.Min(firstLevels.Count, secondLevels.Count);
        for (int i = 0; i < pairedCount; i++)
        {
            // 例：Aの1・3・5とBの1・3・7なら、2・6・10。12の超過分2は表示しない。
            // 加算はlongで行い、高レベルのデバッグ武器でも整数の桁あふれを防ぐ。
            long total = (long)firstLevels[i] + secondLevels[i];
            _candidates.Add(new EnchantmentSlot(name, (int)Math.Min(MaxMergedLevel, total)));
        }
        // 相手武器に組となる付呪がない枠は合算せず、元のレベルをそのまま候補に残す。
        for (int i = pairedCount; i < firstLevels.Count; i++)
            _candidates.Add(new EnchantmentSlot(name, firstLevels[i]));
        for (int i = pairedCount; i < secondLevels.Count; i++)
            _candidates.Add(new EnchantmentSlot(name, secondLevels[i]));
    }

    private static List<int> GetSortedLevels(List<EnchantmentSlot> slots, EnchantmentDataBase.EnchantmentName name)
    {
        // レベルだけをコピーして並べ替え、素材武器の付呪の並びや内容は変更しない。
        var levels = new List<int>();
        foreach (var slot in slots)
            if (slot.enchantName == name) levels.Add(Math.Max(1, slot.enchantLevel));
        levels.Sort();
        return levels;
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

    public IEnumerator<EnchantmentSlot> GetEnumerator()
    {
        for (int i = 0; i < Count; i++) yield return this[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
