using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static EnchantmentDataBase;

public static class EnchantmentDraftGenerator
{
    public static List<EnchantmentSlot> GenerateDraftCandidates(
        WeaponInstance weaponA,
        WeaponInstance weaponB
    )
    {
        List<EnchantmentSlot> results = new();

        if (weaponA == null || weaponB == null)
        {
            Debug.LogError("WeaponInstance が null");
            return results;
        }

        // A・B それぞれを「付呪名 → レベル配列」に変換
        var dictA = BuildEnchantDict(weaponA.enchantmentSlots);
        var dictB = BuildEnchantDict(weaponB.enchantmentSlots);

        // A または B に存在する全付呪タイプ
        var allKeys = new HashSet<EnchantmentDataBase.EnchantmentName>();
        allKeys.UnionWith(dictA.Keys);
        allKeys.UnionWith(dictB.Keys);

        foreach (var key in allKeys)
        {
            dictA.TryGetValue(key, out var listA);
            dictB.TryGetValue(key, out var listB);

            listA ??= new List<int>();
            listB ??= new List<int>();

            // 弱い順にソート
            listA.Sort();
            listB.Sort();

            int pairCount = Mathf.Min(listA.Count, listB.Count);

            // ペアリング合成
            for (int i = 0; i < pairCount; i++)
            {
                int mergedLevel = Mathf.Min(listA[i] + listB[i], 10);
                results.Add(new EnchantmentSlot(key, mergedLevel));
            }

            // 余った分は単体候補
            for (int i = pairCount; i < listA.Count; i++)
            {
                results.Add(new EnchantmentSlot(key, listA[i]));
            }

            for (int i = pairCount; i < listB.Count; i++)
            {
                results.Add(new EnchantmentSlot(key, listB[i]));
            }
        }

        return results;
    }

    private static Dictionary<EnchantmentDataBase.EnchantmentName, List<int>>
        BuildEnchantDict(EnchantmentSlot[] slots)
    {
        var dict = new Dictionary<EnchantmentDataBase.EnchantmentName, List<int>>();

        if (slots == null) return dict;

        foreach (var slot in slots)
        {
            if (slot == null) continue;

            if (!dict.TryGetValue(slot.enchantName, out var list))
            {
                list = new List<int>();
                dict.Add(slot.enchantName, list);
            }

            list.Add(slot.enchantLevel); // ★ UI基準の 1～10
        }

        return dict;
    }
}
