using UnityEngine;

public class SwordAndShield : MonoBehaviour
{
    [SerializeField] Enchantment enchantment;

    int atk;
    int DeadlyATK;
    int WeaponArtATK;
    int enchantmentName;
    float efficacy;
    float[,] enchantmentEfficacy = new float[3, 2];

    void Start()
    {
        GetEnchantmentEfficacy();
    }

    /// <summary>
    /// •t‚¢‚½•tŽô‚ðŽæ“¾
    /// </summary>
    void GetEnchantmentEfficacy()
    {       
        // •tŽô‚ðŠi”[
        for (int i = 0; i < 3; i++)
        {
            // Œø‰Ê‚ðŠi”[‚·‚éi‰¼’u‚«j
            enchantmentName = (int)enchantment.EnchantmentData[0].Name;
            // Œø—Í‚ðŠi”[ (‰¼’u‚«)
            efficacy = enchantment.EnchantmentData[0].Efficacy[0];
            // ƒGƒ“ƒ`ƒƒƒ“ƒg“à—e‚ðŠi”[‚·‚é
            enchantmentEfficacy[i, 0] = enchantmentName;
            enchantmentEfficacy[i, 1] = efficacy;
        }
    }
}
