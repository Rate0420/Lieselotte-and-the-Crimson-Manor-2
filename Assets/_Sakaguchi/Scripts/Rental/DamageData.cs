using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Damage", menuName = "Damage")]
public class DamageData : ScriptableObject
{
    public List<Damage> Damage;
}

[Serializable]
public class Damage
{
    [Header("ƒLƒƒƒ‰ƒNƒ^[‚Ì–¼‘O")] public string Name;

    [Header("‹ß‹——£UŒ‚‚ÌUŒ‚—Í")] public int MeleeAtk;
    [Header("‰“‹——£UŒ‚‚ÌUŒ‚—Í")] public int RangeAtk;
    [Header("ƒK[ƒuƒŒUŒ‚‚ÌUŒ‚—Í")] public int BreakAtk;

    [Header("ƒXƒLƒ‹1‚ÌUŒ‚—Í")] public int S1Atk;
    [Header("ƒXƒLƒ‹2‚ÌUŒ‚—Í")] public int S2Atk;
    [Header("ƒXƒLƒ‹3‚ÌUŒ‚—Í")] public int S3Atk;

    [Header("•KE‹Z1‚ÌUŒ‚—Í")] public int U1Atk;
    [Header("•KE‹Z2‚ÌUŒ‚—Í")] public int U2Atk;
    [Header("•KE‹Z3‚ÌUŒ‚—Í")] public int U3Atk;

    [Header("‹ßÚUŒ‚‚ğ“–‚Ä‚½‚ÌƒQ[ƒW‘‰Á—Ê")] public float MeleeAtkHitUltIncrease;
    [Header("‰“‹——£UŒ‚‚ğ“–‚Ä‚½‚ÌƒQ[ƒW‘‰Á—Ê")] public float RangeAtkHitUltIncrease;
    [Header("ƒK[ƒuƒŒUŒ‚‚ğ“–‚Ä‚½‚ÌƒQ[ƒW‘‰Á—Ê")] public float BreakAtkHitUltIncrease;

    [Header("•KE‹Z1‚ğ”í’e‚µ‚½‚ÌƒQ[ƒW‘‰Á—Ê")] public float HitByU1Increase;
    [Header("•KE‹Z2‚ğ”í’e‚µ‚½‚ÌƒQ[ƒW‘‰Á—Ê")] public float HitByU2Increase;
    [Header("•KE‹Z3‚ğ”í’e‚µ‚½‚ÌƒQ[ƒW‘‰Á—Ê")] public float HitByU3Increase;
    [Header("ƒXƒLƒ‹1‚ğ”í’e‚µ‚½‚ÌƒQ[ƒW‘‰Á—Ê")] public float HitByS1Increase;
    [Header("ƒXƒLƒ‹2‚ğ”í’e‚µ‚½‚ÌƒQ[ƒW‘‰Á—Ê")] public float HitByS2Increase;
    [Header("ƒXƒLƒ‹3‚ğ”í’e‚µ‚½‚ÌƒQ[ƒW‘‰Á—Ê")] public float HitByS3Increase;
    [Header("‹ßÚUŒ‚‚ğ”í’e‚µ‚½‚ÌƒQ[ƒW‘‰Á—Ê")] public float HitByMeleeAtkIncrease;
    [Header("‰“‹——£UŒ‚‚ğ”í’e‚µ‚½‚ÌƒQ[ƒW‘‰Á—Ê")] public float HitByRangeAtkIncrease;
    [Header("ƒK[ƒLƒƒƒ“UŒ‚‚ğ”í’e‚µ‚½‚ÌƒQ[ƒW‘‰Á—Ê")] public float HitByBreakAtkIncrease;

    [Header("•KE‹Z1‚ÌƒQ[ƒWÁ”ï—Ê")] public int U1Consumption;
    [Header("•KE‹Z2‚ÌƒQ[ƒWÁ”ï—Ê")] public int U2Consumption;
    [Header("•KE‹Z3‚ÌƒQ[ƒWÁ”ï—Ê")] public int U3Consumption;

    [Header("ƒWƒƒƒXƒK‚µ‚½‚Ì•KE‹ZƒQ[ƒW‚Ì‘‰Á—Ê")] public int GuardUltIncrease;
    [Header("ƒWƒƒƒXƒK‚µ‚½‚ÌƒXƒ^ƒ~ƒiƒQ[ƒW‚Ì‘‰Á—Ê")] public int GuardVitalityIncrease;
}