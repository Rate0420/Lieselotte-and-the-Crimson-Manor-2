using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DropManager : MonoBehaviour
{
    [SerializeField] private WeaponLottery weaponLottery;　 // 武器抽選
    [SerializeField] private EnchantmentEfficacy enchantmentEfficacy;
    [SerializeField] private EnchantmentText enchantmentText;

    [SerializeField] private EnchantmentTable enchantTable; // 付呪のテーブル
    [SerializeField] private SlotTable slotTable;           // スロットのテーブル

    [SerializeField] private Image targetImage;     // 変更元の武器画像
    [SerializeField] private Sprite[] weaponImages; // 変更後の武器画像

    [SerializeField] private TextMeshProUGUI weaponNameText; // 武器名テキスト
    [SerializeField] private string[] weaponName;            // 武器の名前

    private int rankNo;     // ランク番号
    private int slotCount;  // 選ばれたスロット数の保存

    [SerializeField] private WeaponDatabase weaponDatabase; // 武器データの集積所
    [SerializeField] private Enchantment enchantmentDB;     // 付呪のデータ
    WeaponInstance weaponInstance;                          // 武器のインスタンススクリプト

    private void Start()
    {
        rankNo = ScoreData.instance.rankNoDate;
        DropWepon();
        LotterySlot();
        slotCount = LotterySlot();
        SpecialEffects();
        WeaponInstanceAdaptation();
    }

    /// <summary>
    /// ドロップする武器の抽選
    /// </summary>
    private void DropWepon()
    {
        WeaponType type = weaponLottery.GetRandomWeaponType();
        ImageChange(type);
        var weaponTemp = weaponDatabase.GetWeaponTemplateByName(type.ToString());
        weaponInstance = new WeaponInstance(weaponTemp, enchantmentDB);
    }

    /// <summary>
    /// 武器スロットの抽選
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
        enchantmentText.TextChange_EN();
    }

    /// <summary>
    /// 武器画像の変更
    /// </summary>
    private void ImageChange(WeaponType type)
    {
        targetImage.sprite = weaponImages[(int)type];
        weaponNameText.text = weaponName[(int)type];
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
