using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Scriptable Objects/Weapon")]
public class Weapon : ScriptableObject
{
    public List<WeaponDataBase> WeaponData;
}

[Serializable]
public class WeaponDataBase
{
    public enum WeaponType
    {
        SwordAndShield,
        GreatSword,
        Rapier
    }

    [Header("–¼‘O")]
    public WeaponType name;

    [Header("ƒpƒŠƒB—P—\")]
    public float ParryGrace;

    [Header("ƒpƒŠƒB”­¶ƒtƒŒ[ƒ€")]
    public float OccurrenceFrame;

    [Header("UŒ‚—Í")]
    public int ATK;

    [Header("’v–½UŒ‚—Í")]
    public int DeadlyATK;

    [Header("í‹Z‚ÌUŒ‚—Í")]
    public int WeaponArtATK;
    
    [Header("UŒ‚ŠÔŠu")]
    public float AttackInterval;
    
    [Header("ƒRƒ“ƒ{—P—\ŠÔ")]
    public float ComboGracePeriod;
}