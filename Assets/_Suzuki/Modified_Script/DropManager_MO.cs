using UnityEngine;

public class DropManager_MO : MonoBehaviour
{
    [SerializeField] private WeaponLottery weaponLottery;　 // 武器抽選
    [SerializeField] private EnchantmentEfficacy enchantmentEfficacy;

    [SerializeField] private EnchantmentTable enchantTable; // 付呪のテーブル
    [SerializeField] private SlotTable slotTable;           // スロットのテーブル

    [SerializeField] private WeaponDatabase weaponDatabase; // 追加 武器データの集積所
    [SerializeField] private Enchantment enchantmentDB; // 追加 付呪のデータ
    WeaponInstance weaponInstance; // 追加 武器のインスタンススクリプト


    private int rankNo;     // ランク番号
    private int slotCount;  // 選ばれたスロット数の保存


    private void Start()
    {
        // rankNo = ScoreDeta.instance.rankNoDate;


        // 以下武器生成処理
        DropWepon();
        LotterySlot();
        slotCount = LotterySlot();
        Log();
        SpecialEffects();
        WeaponInstanceAdaptation();
    }

    /// <summary>
    /// ドロップする武器の抽選/ドロップするインスタンスの作成
    /// </summary>
    private void DropWepon()
    {
        WeaponType type = weaponLottery.GetRandomWeaponType();
        var weaponTemp = weaponDatabase.GetWeaponTemplateByName(type.ToString());
        weaponInstance = new WeaponInstance(weaponTemp,enchantmentDB);
        Debug.Log("ドロップ：" + weaponInstance.templateName);
    }

    /// <summary>
    /// 武器スロット数の抽選
    /// </summary>
    private int LotterySlot()
    {
        return GetRandomSlotCount(slotTable.SlotsTableData[rankNo]);
    }


    private int GetRandomSlotCount(SlotsTableDataBase table)
    {
        int total = table.Slot1 + table.Slot2 + table.Slot3;
        int r = Random.Range(0, total);

        if (r < table.Slot1)
            return 1;

        if (r < table.Slot1 + table.Slot2)
            return 2;

        return 3;
    }

    /// <summary>
    /// 付呪効果の抽選
    /// </summary>
    private void SpecialEffects()
    {
        enchantmentEfficacy.SetSlot(slotCount);
        enchantmentEfficacy.SetTable(rankNo);
        enchantmentEfficacy.SetEnchantmentEfficacy();
    }

    private void Log()
    {
        Debug.Log("付与するスロット数" + slotCount);
    }

    /// <summary>
    /// 抽選後のデータをインベントリへ追加
    /// </summary>
    private void WeaponInstanceAdaptation()
    {
        EnchantmentSlot[] setSlots = enchantmentEfficacy.GetResults(); // 抽選結果を格納
        if (setSlots != null)
        {
            weaponInstance.SetEnchantments(setSlots); // 格納した結果を武器インスタンスに入っている付呪スロットへ適応
        }

        if (WeaponInventory.Instance == null)
        {
            Debug.LogError("WeaponInventory.Instance が存在しません");
            return;
        }
        WeaponInventory.Instance.AddWeapon(weaponInstance); // ドロップした武器の情報ををweaponInventory内のリストへ追加
    }

}
