using System;
using System.Collections.Generic;

[Serializable]
public class InventorySaveData
{
    public List<WeaponSaveData> weapons = new();
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