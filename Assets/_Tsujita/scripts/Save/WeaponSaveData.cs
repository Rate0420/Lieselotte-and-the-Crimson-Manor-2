using System;
using System.Collections.Generic;

[Serializable]
public class InventorySaveData
{
    public List<WeaponSaveData> weapons = new();
    // 所持一覧のどの武器を装備しているかを保存する。項目のない旧セーブでは0になる。
    public int equipmentIndex;
}

[Serializable]
public class WeaponSaveData
{
    public string weaponName;
    public List<EnchantmentSaveData> enchantments = new();
}

[Serializable]
public class EnchantmentSaveData
{
    public EnchantmentDataBase.EnchantmentName enchantName;
    public int enchantLevel;
}
