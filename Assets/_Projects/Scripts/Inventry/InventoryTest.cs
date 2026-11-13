using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class InventoryTest : MonoBehaviour
{
    [Header("参照するもの")]
    [SerializeField] private WeaponInventory inventory;  // 武器関連
    [SerializeField] private ItemSlotUI slotPrefab;  // ボタン型のUI
    [SerializeField] private Transform gridRoot;

    [Header("武器詳細UI")]
    [SerializeField] private RawImage selectedWeaponIcon;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Text[] enchantmentTexts;

    private List<ItemSlotUI> slotList = new List<ItemSlotUI>();
    private int selectIndex = 0;
    private int equippedIndex = 0;

    // 初期化処理
    private void Awake()
    {
        // 持っている武器のデータを格納する
        if (inventory == null)
            inventory = WeaponInventory.Instance;

        inventory.OnInventoryChanged += RefreshSlots;
        
        // スロット生成
        RefreshSlots();
        
        // 選択中のUIを更新する
        UpdateSelectionUI();
    }

    // このオブジェクトが非アクティブになった時
    private void OnDisable()
    {
        inventory.OnInventoryChanged -= RefreshSlots;
    }

    private void Update()
    {
        HandleInput();
    }

    // 入力処理
    private void HandleInput()
    {
        if (slotList.Count == 0) return;

        // 左右移動（キーボード / コントローラー）
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectIndex = Mathf.Clamp(selectIndex + 1, 0, slotList.Count - 1);
            UpdateSelectionUI();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selectIndex = Mathf.Clamp(selectIndex - 1, 0, slotList.Count - 1);
            UpdateSelectionUI();
        }

        // 装備決定
        if (Input.GetKeyDown(KeyCode.Return))
        {
            equippedIndex = selectIndex;
            UpdateSelectionUI();
        }
    }

    // スロット生成
    private void RefreshSlots()
    {
        // 既存削除
        foreach (Transform child in gridRoot)
            Destroy(child.gameObject);

        slotList.Clear();

        // 再生成
        foreach (var weapon in inventory.GetAllWeapons())
        {
            //// インベントリにボタンを追加する
            //var slot = Instantiate(slotPrefab, gridRoot);
            //// 
            //slot.WeaponBind(weapon);
            //slotList.Add(slot);

            //// ボタンのアイコンをセットする
            //slot.SetIcon(weapon);
        }
        selectIndex = Mathf.Clamp(selectIndex, 0, slotList.Count - 1);

    }

    // 選択中の UI 更新
    private void UpdateSelectionUI()
    {
        // 武器データがなかったらここで処理終わり
        if (slotList.Count == 0) return;

        // スロットのハイライト更新
        for (int i = 0; i < slotList.Count; i++)
        {
            slotList[i].SetHighlight(i == selectIndex, i == equippedIndex);
        }

        // 選択中武器の詳細表示
        var weapon = inventory.GetWeapon(selectIndex);

        selectedWeaponIcon.texture = weapon.Icon.texture;
        descriptionText.text = weapon.weaponTemplate.description;

        for (int i = 0; i < enchantmentTexts.Length; i++)
        {
            if (i < weapon.enchantmentSlots.Length && weapon.enchantmentSlots[i] != null)
            {
                var slot = weapon.enchantmentSlots[i];
                int value = slot.enchantLevel;
                enchantmentTexts[i].text = $"{slot.enchantName} + {value}";
            }
            else
            {
                enchantmentTexts[i].text = "なし";
            }
        }
    }
}