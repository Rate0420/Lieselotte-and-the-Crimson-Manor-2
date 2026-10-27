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
    public WeaponTemplate vermilion_TestTemplate;

    // 付呪データベース（初期付呪用）
    [SerializeField]  public Enchantment defaultEnchantmentDB;

    public event Action OnInventoryChanged;

    public static WeaponInventory Instance { get; private set; }

    public WeaponInstance testinstance;

    public int equipmentIndex = 0;

    public bool vermilionTestMode = false;    

    string SavePath => Application.persistentDataPath + "/inventory.json";

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
            if (vermilionTestMode)
            {
                var instance = new WeaponInstance(
                    vermilion_TestTemplate,
                    defaultEnchantmentDB
                );
                AddWeapon(instance);
                weapons[0] = instance;
            }
            else
            {
                var instance = new WeaponInstance(
                sword_ShieldTemplate,
                defaultEnchantmentDB
            );
                AddWeapon(instance);
            }
            SaveInventory();
        }
        else if (vermilionTestMode)
        {
            var instance = new WeaponInstance(
    vermilion_TestTemplate,
    defaultEnchantmentDB
);
            weapons[0] = instance;
        }
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
        Debug.Log("ロード");
    }

    WeaponTemplate GetTemplateByID(string id)
    {
        if (sword_ShieldTemplate.weaponName == id) return sword_ShieldTemplate;
        if (greatswordTemplate.weaponName == id) return greatswordTemplate;
        if (rapierTemplate.weaponName == id) return rapierTemplate;
        Debug.Log(id);
        Debug.Log(vermilion_TestTemplate.weaponName);
        if (vermilion_TestTemplate.weaponName == id)
        {
            Debug.Log("vermilion_TestTemplateを取得");
            Debug.Log(id);
            Debug.Log(vermilion_TestTemplate.weaponName);
            return vermilion_TestTemplate;
        } 
        return null;
    }

    /// <summary>
    /// 武器の削除
    /// </summary>
    public void ClearAllWeapons()
    {
        weapons.Clear();
        SaveInventory();
        OnInventoryChanged?.Invoke();
    }

    private void Update()
    {
        // テスト用：Mキーでvermilion_TestTemplateの武器を追加
        if (Input.GetKeyDown(KeyCode.M))
        {
            var instance = new WeaponInstance(
                vermilion_TestTemplate,
                defaultEnchantmentDB
            );
            AddWeapon(instance);
        }
    }
}
