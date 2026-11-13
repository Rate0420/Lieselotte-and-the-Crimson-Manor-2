using UnityEngine;
using TMPro;

public class EnchantmentText : MonoBehaviour
{
    [SerializeField] private EnchantmentEfficacy enchantmentEfficacy;
    [SerializeField] private TextMeshProUGUI[] enchantTexts;

    /// <summary>
    /// 付呪効果テキスト変更
    /// </summary>
    public void TextChange_EN()
    {
        //float[,] data = enchantmentEfficacy.GetEnchantmentResult();

        //for (int i = 0; i < data.GetLength(0); i++)
        //{
        //    int id = (int)data[i, 0];
        //    int value = (int)data[i, 1];

        //    string name = enchantmentEfficacy.GetEnchantmentName(id);
        //    enchantTexts[i].text = $"{name}";


        //}

        // 抽選結果を取得
        EnchantmentSlot[] data = enchantmentEfficacy.GetResults();

        for (int i = 0; i < data.Length; i++)
        {
            string name = data[i].enchantName.ToString();
            int level = data[i].enchantLevel;

            enchantTexts[i].text = $"{name} Lv.{level}";
        }
    }
    //public void TextChange_EN(EnchantmentSlot[] data)
    //{
    //    for (int i = 0; i < data.Length; i++)
    //    {
    //        enchantTexts[i].text =
    //            $"{data[i].enchantName} Lv.{data[i].enchantLevel}";
    //    }
    //}
}
