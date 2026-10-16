using UnityEngine;
using UnityEngine.UI;
using System;

public class ItemSlotUI : MonoBehaviour
{
    [Header("UIで必要なもの")]
    [SerializeField] private Image icon;
    [SerializeField] private Text nameText;
    [SerializeField] private Text enchantText;
    [SerializeField] private Button button;

    public WeaponInstance weapon;

    [SerializeField] RawImage buttonIcon;
    [SerializeField] WeaponTemplate greatsword;
    [SerializeField] WeaponTemplate sword_Shield;
    [SerializeField] WeaponTemplate rapier;

    // インベントリのアイコンをセットする
    public void SetIcon(WeaponInstance weapon)
    {
        WeaponType weaponType = weapon.WeaponType;

        switch (weaponType)
        {
            case WeaponType.greatsword:
                buttonIcon.texture = greatsword.weaponIcon.texture;
                break;
            case WeaponType.sword_Shield:
                buttonIcon.texture = sword_Shield.weaponIcon.texture;
                break;
            case WeaponType.rapier:
                buttonIcon.texture = rapier.weaponIcon.texture;
                break;
        }
    }

    // ★ 追加：スロットが選択されたことを通知するイベント
    public Action<ItemSlotUI> OnSelected;

    public WeaponInstance Weapon => weapon;

    public void WeaponBind(WeaponInstance weapon)
    {
        icon.sprite = weapon.Icon;
        nameText.text = weapon.DisplayName;
        enchantText.text = weapon.EnchantmentCount.ToString();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        Debug.Log("選択された武器:" + weapon.DisplayName);

        // ★ 追加：選択されたことを通知
        OnSelected?.Invoke(this);
    }

    // ★ ハイライト用（選択中・装備中）
    public void SetHighlight(bool isSelected, bool isEquipped)
    {
        var color = Color.white;

        if (isEquipped)
            color = Color.red;
        else if (isSelected)
            color = Color.yellow;

        GetComponent<RawImage>().color = color;
    }
}