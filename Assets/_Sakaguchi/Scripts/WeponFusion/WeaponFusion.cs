using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class WeaponFusion
{
    public static List<EnchantmentSlot> GenerateCandidates(
        WeaponInstance a,
        WeaponInstance b)
    {
        var result = new List<EnchantmentSlot>();

        // null除外
        var aSlots = a.enchantmentSlots.Where(s => s != null).ToList();
        var bSlots = b.enchantmentSlots.Where(s => s != null).ToList();

        // 付呪名ごとに処理
        var allNames = aSlots
            .Select(s => s.enchantName)
            .Union(bSlots.Select(s => s.enchantName));

        foreach (var name in allNames)
        {
            var listA = aSlots
                .Where(s => s.enchantName == name)
                .OrderBy(s => s.enchantLevel)
                .ToList();

            var listB = bSlots
                .Where(s => s.enchantName == name)
                .OrderBy(s => s.enchantLevel)
                .ToList();

            // ペア数は少ない方
            int pairCount = Mathf.Min(listA.Count, listB.Count);

            // 弱いもの同士を合成
            for (int i = 0; i < pairCount; i++)
            {
                int level =
                    Mathf.Min(listA[i].enchantLevel + listB[i].enchantLevel, 10);

                result.Add(new EnchantmentSlot(name, level));
            }

            // 余った A 側を単独候補に
            for (int i = pairCount; i < listA.Count; i++)
            {
                result.Add(new EnchantmentSlot(
                    name,
                    listA[i].enchantLevel));
            }

            // 余った B 側を単独候補に
            for (int i = pairCount; i < listB.Count; i++)
            {
                result.Add(new EnchantmentSlot(
                    name,
                    listB[i].enchantLevel));
            }
        }

        return result;
    }

    /// <summary>
    /// 選択された付呪3つを使って新武器を生成
    /// </summary>
    public static WeaponInstance Fuse(
        WeaponInstance baseWeapon,
        WeaponInstance materialWeapon,
        List<EnchantmentSlot> selected,
        Enchantment enchantmentDB)
    {
        // ★ 最初に選択した武器のテンプレートを引き継ぐ
        var result = new WeaponInstance(
            baseWeapon.weaponTemplate,
            enchantmentDB);

        result.SetEnchantments(selected.ToArray());

        return result;
    }

    public static WeaponInstance CreateResultWeapon(
    WeaponInstance baseWeapon,
    List<EnchantmentSlot> selected,
    Enchantment enchantmentDB)
    {
        var result = new WeaponInstance(baseWeapon.weaponTemplate, enchantmentDB);

        result.SetEnchantments(selected.ToArray());

        return result;
    }

}
