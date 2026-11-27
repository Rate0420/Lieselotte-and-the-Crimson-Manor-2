
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static EnchantmentDataBase;

public class EnchantmentEfficacy : MonoBehaviour
{
    [SerializeField] Enchantment enchantment;
    [SerializeField] EnchantmentTable enchantmentTable;

    // 付いた付呪の名前と効果を格納（float[効果，効力]）
    float[,] enchantmentEfficacy;

    // テーブルの中身を格納（int[名前, 確率]）
    int[,] enchantmentTableContent;

    [SerializeField] int tablesNo;
    [SerializeField] int slot;

    // 重複禁止の付呪
    private readonly HashSet<EnchantmentDataBase.EnchantmentName>
        uniqueEnchantments = new()
    {
        EnchantmentDataBase.EnchantmentName.毒付与
    };

    void Start()
    {
        enchantmentEfficacy = new float[slot, 2];

        if (enchantment != null && enchantment.EnchantmentData != null)
        {
            enchantmentTableContent =
                new int[enchantment.EnchantmentData.Count, 2];
        }
    }

    /// <summary>
    /// テーブルの中身を格納する
    /// </summary>
    public void SetTable(int rankNo)
    {
        if (enchantment == null ||
            enchantment.EnchantmentData == null ||
            enchantmentTable == null ||
            enchantmentTable.EnchantmentTablesData == null)
        {
            Debug.LogError("Enchantment関連のデータが設定されていません");
            return;
        }

        if (tablesNo < 0 ||
            tablesNo >= enchantmentTable.EnchantmentTablesData.Count)
        {
            Debug.LogError("tablesNoが範囲外です");
            return;
        }

        if (enchantmentTableContent == null ||
            enchantmentTableContent.GetLength(0) != enchantment.EnchantmentData.Count)
        {
            enchantmentTableContent =
                new int[enchantment.EnchantmentData.Count, 2];
        }

        int count = 0;

        for (int i = 0; i < enchantment.EnchantmentData.Count; i++)
        {
            enchantmentTableContent[i, 0] =
                (int)enchantment.EnchantmentData[i].Name;

            switch (enchantmentTableContent[i, 0])
            {
                case (int)EnchantmentDataBase.EnchantmentName.HP増加:
                    enchantmentTableContent[i, 1] =
                        enchantmentTable.EnchantmentTablesData[tablesNo].HP;
                    break;

                case (int)EnchantmentDataBase.EnchantmentName.スタミナ回復上昇:
                    enchantmentTableContent[i, 1] =
                        enchantmentTable.EnchantmentTablesData[tablesNo].SK;
                    break;

                case (int)EnchantmentDataBase.EnchantmentName.攻撃力増加:
                    enchantmentTableContent[i, 1] =
                        enchantmentTable.EnchantmentTablesData[tablesNo].AU;
                    break;

                case (int)EnchantmentDataBase.EnchantmentName.初撃強化:
                    enchantmentTableContent[i, 1] =
                        enchantmentTable.EnchantmentTablesData[tablesNo].FirstCombo;
                    break;

                case (int)EnchantmentDataBase.EnchantmentName.フィニッシュ強化:
                    enchantmentTableContent[i, 1] =
                        enchantmentTable.EnchantmentTablesData[tablesNo].FinishCombo;
                    break;

                case (int)EnchantmentDataBase.EnchantmentName.致命強化:
                    enchantmentTableContent[i, 1] =
                        enchantmentTable.EnchantmentTablesData[tablesNo].DeadlyAttack;
                    break;

                case (int)EnchantmentDataBase.EnchantmentName.戦技強化:
                    enchantmentTableContent[i, 1] =
                        enchantmentTable.EnchantmentTablesData[tablesNo].WeaponArt;
                    break;

                case (int)EnchantmentDataBase.EnchantmentName.攻撃速度上昇:
                    enchantmentTableContent[i, 1] =
                        enchantmentTable.EnchantmentTablesData[tablesNo].AttackIntervalUP;
                    break;

                case (int)EnchantmentDataBase.EnchantmentName.コンボ時間増加:
                    enchantmentTableContent[i, 1] =
                        enchantmentTable.EnchantmentTablesData[tablesNo].ComboGracePeriodUP;
                    break;

                case (int)EnchantmentDataBase.EnchantmentName.パリィ猶予増加:
                    enchantmentTableContent[i, 1] =
                        enchantmentTable.EnchantmentTablesData[tablesNo].ParryGraceTimeUP;
                    break;

                case (int)EnchantmentDataBase.EnchantmentName.移動速度上昇:
                    enchantmentTableContent[i, 1] =
                        enchantmentTable.EnchantmentTablesData[tablesNo].AgeUp;
                    break;

                case (int)EnchantmentDataBase.EnchantmentName.ガード強化:
                    enchantmentTableContent[i, 1] =
                        enchantmentTable.EnchantmentTablesData[tablesNo].GuardReinforcement;
                    break;

                case (int)EnchantmentDataBase.EnchantmentName.パリィ時ダメージ:
                    enchantmentTableContent[i, 1] =
                        enchantmentTable.EnchantmentTablesData[tablesNo].ParryAttack;
                    break;

                //case (int)EnchantmentDataBase.EnchantmentName.ジャスガ時初撃強化:
                //    enchantmentTableContent[i, 1] =
                //        enchantmentTable.EnchantmentTablesData[tablesNo].JGFirstCombo;
                //    break;

                case (int)EnchantmentDataBase.EnchantmentName.パリィ強化:
                    enchantmentTableContent[i, 1] =
                        enchantmentTable.EnchantmentTablesData[tablesNo].ParryUp;
                    break;

                case (int)EnchantmentDataBase.EnchantmentName.ジャスガ強化:
                    enchantmentTableContent[i, 1] =
                        enchantmentTable.EnchantmentTablesData[tablesNo].JGUp;
                    break;

                case (int)EnchantmentDataBase.EnchantmentName.逆境:
                    enchantmentTableContent[i, 1] =
                        enchantmentTable.EnchantmentTablesData[tablesNo].AttackRateUP;
                    break;

                case (int)EnchantmentDataBase.EnchantmentName.毒付与:
                    enchantmentTableContent[i, 1] =
                        enchantmentTable.EnchantmentTablesData[tablesNo].poison;
                    break;
                default:
                    Debug.Log(enchantmentTableContent[i, 0] + "は存在しません");
                    break;
            }

            count += enchantmentTableContent[i, 1];
        }

        if (count == 100) return;

        if (count > 100)
            Debug.Log("確率の合計が100を超えています");
        else
            Debug.Log("確率の合計が100を下回っています");
    }

    /// <summary>
    /// 付呪の効果を格納する
    /// </summary>
    public void SetEnchantmentEfficacy()
    {
        if (enchantmentTableContent == null)
        {
            Debug.LogError("先にSetTableを実行してください");
            return;
        }

        // 抽選済みの重複禁止スキル
        HashSet<EnchantmentDataBase.EnchantmentName>
            acquiredUniqueSkills = new();

        // スロットを初期化
        enchantmentEfficacy = new float[slot, 2];

        for (int i = 0; i < slot; i++)
        {
            int totalProbability = 0;

            // 現在のスロットで抽選可能な確率を合計
            for (int j = 0; j < enchantmentTableContent.GetLength(0); j++)
            {
                var enchantName =
                    (EnchantmentDataBase.EnchantmentName)
                    enchantmentTableContent[j, 0];

                if (uniqueEnchantments.Contains(enchantName) &&
                    acquiredUniqueSkills.Contains(enchantName))
                {
                    continue;
                }

                totalProbability += enchantmentTableContent[j, 1];
            }

            if (totalProbability <= 0)
            {
                Debug.LogWarning("抽選可能な付呪がありません");
                break;
            }

            // 抽選
            int rand = Random.Range(0, totalProbability);

            for (int j = 0; j < enchantmentTableContent.GetLength(0); j++)
            {
                var enchantName =
                    (EnchantmentDataBase.EnchantmentName)
                    enchantmentTableContent[j, 0];

                int probability = enchantmentTableContent[j, 1];

                // 既に取得した重複禁止スキルは除外
                if (uniqueEnchantments.Contains(enchantName) &&
                    acquiredUniqueSkills.Contains(enchantName))
                {
                    continue;
                }

                rand -= probability;

                if (rand < 0)
                {
                    // 付呪名を格納
                    enchantmentEfficacy[i, 0] = (int)enchantName;

                    // 付呪の効力を格納
                    for (int k = 0; k < enchantment.EnchantmentData.Count; k++)
                    {
                        if (enchantmentEfficacy[i, 0] ==
                            (int)enchantment.EnchantmentData[k].Name)
                        {
                            enchantmentEfficacy[i, 1] =
                                enchantment.EnchantmentData[k].Efficacy[0];

                            break;
                        }
                    }

                    // 重複禁止スキルなら取得済みに登録
                    if (uniqueEnchantments.Contains(enchantName))
                    {
                        acquiredUniqueSkills.Add(enchantName);
                    }

                    Debug.Log(enchantName.ToString());

                    break;
                }
            }
        }
    }

    /// <summary>
    /// 抽選結果を取得する
    /// </summary>
    public EnchantmentSlot[] GetResults()
    {
        EnchantmentSlot[] results = new EnchantmentSlot[slot];

        for (int i = 0; i < slot; i++)
        {
            var nameEnum =
                (EnchantmentDataBase.EnchantmentName)
                enchantmentEfficacy[i, 0];

            int enchantmentLevel = 1;

            results[i] = new EnchantmentSlot(nameEnum, enchantmentLevel);
        }

        return results;
    }

    /// <summary>
    /// スロット数を設定する
    /// </summary>
    public void SetSlot(int slotCount)
    {
        slot = slotCount;
        enchantmentEfficacy = new float[slot, 2];
    }

    public float[,] GetEnchantmentResult()
    {
        return enchantmentEfficacy;
    }

    public string GetEnchantmentName(int id)
    {
        return ((EnchantmentDataBase.EnchantmentName)id).ToString();
    }
}
