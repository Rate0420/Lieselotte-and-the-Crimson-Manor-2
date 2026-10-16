using UnityEngine;

public class EnchantmentEfficacy : MonoBehaviour
{
    [SerializeField] Enchantment enchantment;
    [SerializeField] EnchantmentTable enchantmentTable;

    // 付いた付呪の名前と効果を格納（float[効果，効力]）
    float[,] enchantmentEfficacy;
    // テーブルの中身を格納　（int[名前, 確率]）
    int[,] enchantmentTableContent;
    [SerializeField] int tablesNo;      // テーブルの番号
    [SerializeField] int slot;

    void Start()
    {
        // 付呪関連の初期化処理
        enchantmentEfficacy = new float[slot, 2];
        enchantmentTableContent = new int[enchantment.EnchantmentData.Count, 2];
    }

    /// <summary>
    /// テーブルの中身を格納する
    /// </summary>
    public void SetTable(int rankNo)
    {
        // enchantmentTableContentの中身がnull化してないかチェックし、ないなら再度初期化
        if (enchantmentTableContent == null || enchantmentTableContent.GetLength(0) != enchantment.EnchantmentData.Count)
        {
            enchantmentTableContent = new int[enchantment.EnchantmentData.Count, 2];
        }
        Debug.Log($"enchantment = {enchantment}");
        Debug.Log($"EnchantmentData = {enchantment?.EnchantmentData}");
        Debug.Log($"EnchantmentData.Count = {enchantment?.EnchantmentData?.Count}");
        Debug.Log($"enchantmentTableContent.Length = {enchantmentTableContent?.Length}");
        // 確率の合計が100を超えていないかの確認用
        int count = 0;

        for (int i = 0; i < enchantment.EnchantmentData.Count; i++)
        {
            enchantmentTableContent[i, 0] = (int)enchantment.EnchantmentData[i].Name;

            // 付呪の名前を参照して確率を格納する
            switch (enchantmentTableContent[i, 0])
            {
                case (int)EnchantmentDataBase.EnchantmentName.HP増加:
                    enchantmentTableContent[i, 1] = enchantmentTable.EnchantmentTablesData[tablesNo].HP;
                    break;
                case (int)EnchantmentDataBase.EnchantmentName.スタミナ回復上昇:
                    enchantmentTableContent[i, 1] = enchantmentTable.EnchantmentTablesData[tablesNo].SK;
                    break;
                case (int)EnchantmentDataBase.EnchantmentName.攻撃力増加:
                    enchantmentTableContent[i, 1] = enchantmentTable.EnchantmentTablesData[tablesNo].AU;
                    break;
                case (int)EnchantmentDataBase.EnchantmentName.初撃強化:
                    enchantmentTableContent[i, 1] = enchantmentTable.EnchantmentTablesData[tablesNo].FirstCombo;
                    break;
                case (int)EnchantmentDataBase.EnchantmentName.致命強化:
                    enchantmentTableContent[i, 1] = enchantmentTable.EnchantmentTablesData[tablesNo].DeadlyAttack;
                    break;
                case (int)EnchantmentDataBase.EnchantmentName.戦技強化:
                    enchantmentTableContent[i, 1] = enchantmentTable.EnchantmentTablesData[tablesNo].WeaponArt;
                    break;
                case (int)EnchantmentDataBase.EnchantmentName.攻撃速度上昇:
                    enchantmentTableContent[i, 1] = enchantmentTable.EnchantmentTablesData[tablesNo].AttackIntervalUP;
                    break;
                case (int)EnchantmentDataBase.EnchantmentName.コンボ時間増加:
                    enchantmentTableContent[i, 1] = enchantmentTable.EnchantmentTablesData[tablesNo].ComboGracePeriodUP;
                    break;
                case (int)EnchantmentDataBase.EnchantmentName.パリィ猶予増加:
                    enchantmentTableContent[i, 1] = enchantmentTable.EnchantmentTablesData[tablesNo].ParryGraceTimeUP;
                    break;
                case (int)EnchantmentDataBase.EnchantmentName.移動速度上昇:
                    enchantmentTableContent[i, 1] = enchantmentTable.EnchantmentTablesData[tablesNo].AgeUp;
                    break;
                case (int)EnchantmentDataBase.EnchantmentName.ガード強化:
                    enchantmentTableContent[i, 1] = enchantmentTable.EnchantmentTablesData[tablesNo].GuardReinforcement;
                    break;
                default:
                    Debug.Log(enchantmentTableContent[i, 0] + "は存在しません");
                    break;
            }
            // 確率の合計を取得
            count += enchantmentTableContent[i, 1];
        }
        // 確率の合計が100％になっているかの確認
        if (count == 100) return;
        if (100 < count) Debug.Log("確率の合計が100を超えています");
        else Debug.Log("確率の合計が100を下回っています");
    }

    /// <summary>
    /// 付呪の効果を格納する
    /// </summary>
    public void SetEnchantmentEfficacy()
    {
        // 付呪のスロット分だけ付与する
        for (int i = 0; i < slot; i++)
        {
            int rand = Random.Range(0, 100);
            // ランダムに決まった数字を、効果が付く確率分引いていき
            // 0になったタイミングの効果を付与する
            for (int j = 0; j < enchantmentTableContent.Length / 2; j++)
            {
                rand -= enchantmentTableContent[j, 1];
                if (rand < 0)
                {
                    // 抽選された付呪を格納する
                    enchantmentEfficacy[i, 0] = enchantmentTableContent[j, 0];
                    Debug.Log(enchantmentEfficacy[i, 0]);

                    // 付呪の効力を格納する
                    for (int k = 0; k < enchantment.EnchantmentData.Count; k++)
                    {
                        // チェック.
                        if (enchantmentEfficacy[i, 0] == (int)enchantment.EnchantmentData[k].Name)
                            enchantmentEfficacy[i, 1] = enchantment.EnchantmentData[k].Efficacy[0];
                    }
                    Debug.Log(((EnchantmentDataBase.EnchantmentName)enchantmentEfficacy[i, 0]).ToString());
                    break;
                }
            }
        }
    }

    public EnchantmentSlot[] GetResults()
    {
        EnchantmentSlot[] results = new EnchantmentSlot[slot];

        for (int i = 0; i < slot; i++)
        {
            var nameEnum = (EnchantmentDataBase.EnchantmentName)enchantmentEfficacy[i, 0];
            var enchantmentLevel = 0;

            results[i] = new EnchantmentSlot(nameEnum, enchantmentLevel);
        }

        return results;
    }

    public void SetSlot(int slotCount)
    {
        slot = slotCount; // ここでスロット数を反映！

        // 受け取ったスロット数に合わせて配列を作り直す
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