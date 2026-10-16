using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "weapontest", menuName = "Test/weapontest")]
public class weapontest : ScriptableObject
{
    public List<Weapons> Test;
}

[Serializable]
public class Weapons
{
    //public WeaponDataBase.WeaponType weaponType;
    public int[,] EnchantmentEfficacy = new int[3, 2];
    public int[] EnchantmentLevel;
    public string weaponExplanation;
}