using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

public class InventoryWeponSelectView : MonoBehaviour
{
    [Header("外部参照")]
    private WeaponInventory _inventory;

    [Header("表示設定")]
    [SerializeField] private Transform _printed;
    [SerializeField] private InventoryWeponDataView _dataPrefab;

    [Header("付呪選択パネル")]
    [SerializeField] private EnchantSelectionPanel _enchantSelectionPanel;

    [Header("プレビュー")]
    [SerializeField] private WeaponsPreviewPlate _previewPlate;

    [Header("武器表示リスト")]
    [SerializeField] private List<InventoryWeponDataView> _dataList = new();

    [Header("デバッグ設定")]
    [SerializeField] private bool _randomSynthesisOnStart = false;

    private readonly List<WeaponInstance> _selectedWeapons = new();
    private const int MaxSelectCount = 2;

    private void Start()
    {
        _inventory = WeaponInventory.Instance;
        InitializeWeaponList();

        if (_randomSynthesisOnStart)
        {
            RandomSynthesisDebug();
        }
    }

    private void InitializeWeaponList()
    {
        if (_inventory == null)
        {
            Debug.LogError("WeaponInventory が設定されていません。", this);
            return;
        }

        if (_printed == null || _dataPrefab == null)
        {
            Debug.LogError("生成先または武器表示プレハブが設定されていません。", this);
            return;
        }

        var weapons = _inventory.GetAllWeapons();

        foreach (var dataView in _dataList)
        {
            if (dataView != null)
            {
                dataView.gameObject.SetActive(false);
                dataView.ResetSelection();
            }
        }

        for (int i = 0; i < weapons.Count; i++)
        {
            var weapon = weapons[i];
            var dataView = GetOrCreateDataView(i);

            if (dataView == null) continue;

            var enchantments = CreateEnchantmentTexts(weapon.enchantmentSlots);
            dataView.SetUpData(weapon.Icon, enchantments);

            int weaponIndex = i;
            dataView.OnSelectClicked += () => SelectWeapon(weaponIndex);

            dataView.gameObject.SetActive(true);
        }
    }

    private InventoryWeponDataView GetOrCreateDataView(int index)
    {
        if (index < _dataList.Count && _dataList[index] != null)
        {
            return _dataList[index];
        }

        var dataView = Instantiate(_dataPrefab, _printed);

        while (_dataList.Count <= index)
        {
            _dataList.Add(null);
        }

        _dataList[index] = dataView;
        return dataView;
    }

    private List<string> CreateEnchantmentTexts(IEnumerable<EnchantmentSlot> enchantmentSlots)
    {
        var texts = new List<string>();

        foreach (var slot in enchantmentSlots)
        {
            if (slot.enchantName.ToString() == "なし")
            {
                texts.Add("なし");
            }
            else
            {
                texts.Add($"{slot.enchantName} + {slot.enchantLevel}");
            }
        }

        while (texts.Count < 3)
        {
            texts.Add("なし");
        }

        return texts;
    }

    private void SelectWeapon(int index)
    {
        var weapons = _inventory.GetAllWeapons();

        if (index < 0 || index >= weapons.Count) return;

        var weapon = weapons[index];

        // すでに選択していれば選択解除
        if (_selectedWeapons.Contains(weapon))
        {
            _selectedWeapons.Remove(weapon);
            RefreshSelection();
            UpdatePreview();
            UpdateEnchantPanel();
            return;
        }

        if (_selectedWeapons.Count >= MaxSelectCount)
        {
            Debug.Log($"選択できる武器は{MaxSelectCount}個までです。");
            return;
        }

        _selectedWeapons.Add(weapon);
        RefreshSelection();
        UpdatePreview();
        UpdateEnchantPanel();
    }

    /// <summary>
    /// 選択した武器の付呪候補を付呪選択パネルへ渡す
    /// </summary>
    private void UpdateEnchantPanel()
    {
        if (_enchantSelectionPanel == null)
        {
            Debug.LogError("EnchantSelectionPanel が設定されていません。", this);
            return;
        }

        var enchantTexts = new List<string>();

        foreach (var weapon in _selectedWeapons)
        {
            enchantTexts.AddRange(CreateEnchantmentTexts(weapon.enchantmentSlots));
        }

        _enchantSelectionPanel.SetUp(enchantTexts);
    }

    private void RefreshSelection()
    {
        for (int i = 0; i < _dataList.Count; i++)
        {
            var dataView = _dataList[i];

            if (dataView == null) continue;

            var weapons = _inventory.GetAllWeapons();

            if (i >= weapons.Count)
            {
                dataView.ResetSelection();
                continue;
            }

            int selectionIndex = _selectedWeapons.IndexOf(weapons[i]);

            if (selectionIndex >= 0)
            {
                dataView.Selection(selectionIndex + 1);
            }
            else
            {
                dataView.ResetSelection();
            }
        }
    }

    /// <summary>
    /// 選択した武器をプレビュー01・02へ反映する
    /// </summary>
    private void UpdatePreview()
    {
        if (_previewPlate == null)
        {
            Debug.LogError("WeaponsPreviewPlate が設定されていません。", this);
            return;
        }

        _previewPlate.ClearPreview();

        for (int i = 0; i < _selectedWeapons.Count && i < 2; i++)
        {
            var weapon = _selectedWeapons[i];
            var enchantments = CreateEnchantmentTexts(weapon.enchantmentSlots);

            _previewPlate.SetWeaponPreview(i, weapon.Icon, enchantments);
        }

        // 合成結果の武器は、1番目に選択した武器
        if (_selectedWeapons.Count > 0)
        {
            _previewPlate.SetResultIcon(_selectedWeapons[0].Icon);
        }
    }


    /// <summary>
    /// デバッグ用：武器2つと付呪効果3つをランダムに決めて合成プレビューまで反映する
    /// </summary>
    [ContextMenu("Random Synthesis Debug")]
    public void RandomSynthesisDebug()
    {
        if (_inventory == null)
        {
            Debug.LogError("WeaponInventory が設定されていません。", this);
            return;
        }

        var weapons = _inventory.GetAllWeapons();

        if (weapons == null || weapons.Count < 2)
        {
            Debug.LogWarning("デバッグ合成には武器が2つ以上必要です。", this);
            return;
        }

        // 武器選択をリセット
        _selectedWeapons.Clear();
        RefreshSelection();

        // 重複しない武器を2つ選ぶ
        var weaponIndexes = Enumerable.Range(0, weapons.Count).ToList();

        for (int i = 0; i < weaponIndexes.Count; i++)
        {
            int swapIndex = Random.Range(i, weaponIndexes.Count);
            (weaponIndexes[i], weaponIndexes[swapIndex]) =
                (weaponIndexes[swapIndex], weaponIndexes[i]);
        }

        SelectWeapon(weaponIndexes[0]);
        SelectWeapon(weaponIndexes[1]);

        // 付呪候補を集める
        var allEnchantments = new List<string>();

        foreach (var weapon in _selectedWeapons)
        {
            allEnchantments.AddRange(CreateEnchantmentTexts(weapon.enchantmentSlots));
        }

        if (allEnchantments.Count == 0)
        {
            Debug.LogWarning("選択された武器に付呪候補がありません。", this);
            return;
        }

        // 重複しない付呪効果を最大3つ選ぶ
        var enchantIndexes = Enumerable.Range(0, allEnchantments.Count).ToList();

        for (int i = 0; i < enchantIndexes.Count; i++)
        {
            int swapIndex = Random.Range(i, enchantIndexes.Count);
            (enchantIndexes[i], enchantIndexes[swapIndex]) =
                (enchantIndexes[swapIndex], enchantIndexes[i]);
        }

        int selectCount = Mathf.Min(3, allEnchantments.Count);
        var selectedEnchantments = new List<string>();

        for (int i = 0; i < selectCount; i++)
        {
            selectedEnchantments.Add(allEnchantments[enchantIndexes[i]]);
        }

        // 重複しない付呪効果を最大3つ選ぶ
        var selectedEnchantIndexes = enchantIndexes.Take(selectCount).ToList();

        if (_enchantSelectionPanel != null)
        {
            _enchantSelectionPanel.SetUp(allEnchantments);
            _enchantSelectionPanel.SetSelectedEnchantmentsForDebug(selectedEnchantIndexes);
        }

        if (_previewPlate != null)
        {
            var selectedEnchantmentTexts = new List<string>();

            foreach (int index in selectedEnchantIndexes)
            {
                selectedEnchantmentTexts.Add(allEnchantments[index]);
            }

            _previewPlate.SetResultEnchantments(selectedEnchantmentTexts);
        }

        Debug.Log(
            $"デバッグ合成: 武器[{weaponIndexes[0]}], 武器[{weaponIndexes[1]}] / " +
            $"効果: {string.Join(", ", selectedEnchantments)}"
        );
    }
}