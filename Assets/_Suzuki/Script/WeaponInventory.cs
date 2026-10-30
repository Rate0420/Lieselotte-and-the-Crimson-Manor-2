using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    [SerializeField]
    private List<WeaponInstance> weapons = new List<WeaponInstance>();

    

    // 初期装備の武器データ関連
    public WeaponTemplate sword_ShieldTemplate;
    public WeaponTemplate greatswordTemplate;
    public WeaponTemplate rapierTemplate;

    // 付呪データベース（初期付呪用）
    [SerializeField]  public Enchantment defaultEnchantmentDB;

    public event Action OnInventoryChanged;
    public event Action OnEquipmentChanged;

    public static WeaponInventory Instance { get; private set; }

    public WeaponInstance testinstance;

    public int equipmentIndex = 0;

    protected virtual string SavePath => Application.persistentDataPath + "/inventory.json";

    /// <summary>
    /// シーンを跨いでもインベントリの中身を扱えるようにする
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (weapons.Count == 0 && sword_ShieldTemplate != null)
        {
            ////初期装備の武器を複数生成してリストに追加する（テスト用）
            //for (int i = 0; i < 25; i++)
            //{
            //    var instance = new WeaponInstance(
            //        sword_ShieldTemplate,
            //        defaultEnchantmentDB
            //    );

            //    // ランダム付呪を付与
            //    var slots = CreateRandomEnchantments();
            //    instance.SetEnchantments(slots);

            //    AddWeapon(instance);
            //}
        }

        LoadInventory();
        if (weapons.Count == 0)
        {
            var instance = new WeaponInstance(
                sword_ShieldTemplate,
                defaultEnchantmentDB
            );
            AddWeapon(instance);
        }
        SaveInventory();

    }

    public void startAddWeapon()
    {
        var instance = new WeaponInstance(
        sword_ShieldTemplate,
        defaultEnchantmentDB
        );
        AddWeapon(instance);
    }

    /// <summary>
    /// リストへ武器の情報を追加する
    /// </summary>
    /// <param name="instance"></param>
    public bool AddWeapon(WeaponInstance instance)
    {
        if (instance == null) return false;

        weapons.Add(instance);
        SaveInventory();
        OnInventoryChanged?.Invoke();
        Debug.Log(instance.weaponTemplate.displayName + "を追加しました");
        return true;
    }

    /// <summary>
    /// リストの武器の情報を削除する
    /// </summary>
    /// <param name="weaponIndex"></param>
    public bool RemoveWeapon(int weaponIndex)
    {
        if (weaponIndex < 0 || weaponIndex >= weapons.Count)
            return false;

        weapons.RemoveAt(weaponIndex);
        if (equipmentIndex > weaponIndex) equipmentIndex--;
        else if (equipmentIndex == weaponIndex) equipmentIndex = Mathf.Min(weaponIndex, Mathf.Max(0, weapons.Count - 1));
        OnInventoryChanged?.Invoke();
        return true;
    }

    /// <summary>所持している武器だけを装備し、保存成功後に装備変更を通知する。同じ武器の再装備では解除しない。</summary>
    public bool TryEquipWeapon(WeaponInstance weapon)
    {
        int index = weapon != null ? weapons.IndexOf(weapon) : -1;
        if (index < 0) return false;
        if (index == equipmentIndex) return true;
        // 保存できなかった場合は元の装備へ戻せるよう、変更前の位置を保持する。
        int previous = equipmentIndex;
        equipmentIndex = index;
        try
        {
            SaveInventory();
        }
        catch (Exception exception) when (exception is System.IO.IOException || exception is UnauthorizedAccessException)
        {
            equipmentIndex = previous;
            Debug.LogError($"装備状態の保存に失敗しました: {exception.Message}", this);
            return false;
        }
        OnEquipmentChanged?.Invoke();
        return true;
    }

    /// <summary>合成素材2本を完成品1本へ入れ替え、保存後に一度だけUIへ通知する。</summary>
    public bool TryReplaceWeapons(WeaponInstance first, WeaponInstance second, WeaponInstance result)
    {
        if (first == null || second == null || first == second || result == null ||
            result.weaponTemplate == null || weapons.Contains(result)) return false;
        int firstIndex = weapons.IndexOf(first);
        int secondIndex = weapons.IndexOf(second);
        if (firstIndex < 0 || secondIndex < 0) return false;

        // 合成の保存に失敗した場合に、消費した素材と元の装備状態を復元するための控え。
        var previousWeapons = new List<WeaponInstance>(weapons);
        int previousEquipmentIndex = equipmentIndex;
        var equipped = GetWeapon(equipmentIndex);
        weapons.RemoveAt(Mathf.Max(firstIndex, secondIndex));
        weapons.RemoveAt(Mathf.Min(firstIndex, secondIndex));
        weapons.Add(result);
        // 素材を装備していた場合は完成品、それ以外なら同じ武器を装備し続ける。
        equipmentIndex = equipped == first || equipped == second
            ? weapons.Count - 1 : Mathf.Max(0, weapons.IndexOf(equipped));
        try
        {
            SaveInventory();
        }
        catch (Exception exception) when (exception is System.IO.IOException ||
                                          exception is UnauthorizedAccessException)
        {
            weapons.Clear();
            weapons.AddRange(previousWeapons);
            equipmentIndex = previousEquipmentIndex;
            Debug.LogError($"武器合成の保存に失敗しました: {exception.Message}", this);
            return false;
        }
        OnInventoryChanged?.Invoke();
        return true;
    }


    /// <summary>
    /// indexから参照して中にある武器の情報を取得する
    /// </summary>
    /// <param name="weaponIndex"></param>
    /// <returns></returns>
    public WeaponInstance GetWeapon(int weaponIndex)
    {
        if (weaponIndex < 0 || weaponIndex >= weapons.Count) return null;
        return weapons[weaponIndex];
    }


    /// <summary>
    /// リストそのものを取得する関数
    /// UI側が一覧表示に使う
    /// </summary>
    /// <returns></returns>
    public IReadOnlyList<WeaponInstance> GetAllWeapons()
    {
        return weapons;
    }

    // テスト用：ランダム付呪を生成する関数群
    EnchantmentSlot CreateRandomEnchantment()
    {
        // Enum からランダムに1つ選ぶ
        var values = Enum.GetValues(typeof(EnchantmentDataBase.EnchantmentName));
        var name = (EnchantmentDataBase.EnchantmentName)
            values.GetValue(UnityEngine.Random.Range(0, values.Length));

        // Lv1〜Lv10 → enchantLevel 1〜10
        int level = UnityEngine.Random.Range(1, 11);

        return new EnchantmentSlot(name, level);
    }

    // ランダムな付呪枠を1〜3個生成する関数
    EnchantmentSlot[] CreateRandomEnchantments()
    {
        int slotCount = UnityEngine.Random.Range(1, 4); // 1〜3枠

        EnchantmentSlot[] slots = new EnchantmentSlot[slotCount];

        for (int i = 0; i < slotCount; i++)
        {
            slots[i] = CreateRandomEnchantment();
        }

        return slots;
    }

    /// <summary>
    /// セーブ
    /// </summary>
    public void SaveInventory()
    {
        InventorySaveData save = new();
        // 所持一覧と一緒に装備位置を保存し、再起動後も同じ武器を装備できるようにする。
        save.equipmentIndex = Mathf.Clamp(equipmentIndex, 0, Mathf.Max(0, weapons.Count - 1));

        foreach (var weapon in weapons)
        {
            WeaponSaveData w = new();
            w.weaponName = weapon.weaponTemplate.weaponName;

            foreach (var e in weapon.enchantmentSlots)
            {
                w.enchantments.Add(new EnchantmentSaveData
                {
                    enchantName = e.enchantName,
                    enchantLevel = e.enchantLevel
                });
            }

            save.weapons.Add(w);
            Debug.Log("セーブ");
        }

        string json = JsonUtility.ToJson(save, true);
        System.IO.File.WriteAllText(SavePath, json);
    }

    /// <summary>
    /// ロード
    /// </summary>
    public void LoadInventory()
    {
        if (!System.IO.File.Exists(SavePath)) return;

        string json = System.IO.File.ReadAllText(SavePath);
        InventorySaveData save = JsonUtility.FromJson<InventorySaveData>(json);

        weapons.Clear();

        foreach(var w in save.weapons)
        {
            WeaponTemplate template = GetTemplateByID(w.weaponName);
            if(template == null) continue;

            var instance = new WeaponInstance(template, defaultEnchantmentDB);

            var slots = new List<EnchantmentSlot>();
            foreach(var e in w.enchantments)
            {
                slots.Add(new EnchantmentSlot(e.enchantName, e.enchantLevel));
            }

            instance.SetEnchantments(slots.ToArray());
            weapons.Add(instance);
        }
        // 古いセーブは既定値の0を使い、保存位置が所持数の範囲外なら有効な位置に収める。
        equipmentIndex = Mathf.Clamp(save.equipmentIndex, 0, Mathf.Max(0, weapons.Count - 1));
        Debug.Log("ロード");
    }

    WeaponTemplate GetTemplateByID(string id)
    {
        if (sword_ShieldTemplate.weaponName == id) return sword_ShieldTemplate;
        if (greatswordTemplate.weaponName == id) return greatswordTemplate;
        if (rapierTemplate.weaponName == id) return rapierTemplate;
        return null;
    }

    /// <summary>
    /// 武器の削除
    /// </summary>
    public void ClearAllWeapons()
    {
        weapons.Clear();
        equipmentIndex = 0;
        SaveInventory();
        OnInventoryChanged?.Invoke();
    }
}
