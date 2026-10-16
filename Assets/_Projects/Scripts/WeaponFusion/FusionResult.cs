using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FusionResult : MonoBehaviour
{
    public RawImage weaponImage;
    public Text weaponNameText;
    public Text weaponDescriptionText;
    public Text[] weaponEnchantText;
    public FusionUIManager uiManager;
    WeaponInstance resultWeapon;

    private void OnEnable()
    {
        resultWeapon = WeaponInventory.Instance.GetWeapon(uiManager.createdWeaponIndex);
        weaponImage.texture = resultWeapon.Icon.texture;
        weaponNameText.text = resultWeapon.DisplayName;
        weaponDescriptionText.text = resultWeapon.weaponTemplate.description;
        for (int i = 0; i < weaponEnchantText.Length; i++)
        {
            if (i < resultWeapon.enchantmentSlots.Length && resultWeapon.enchantmentSlots[i] != null)
            {
                var slot = resultWeapon.enchantmentSlots[i];
                int value = slot.enchantLevel;
                weaponEnchantText[i].text = $"{slot.enchantName} + {value}";
            }
            else
            {
                weaponEnchantText[i].text = "なし";
            }
        }
    }

    public void Cont_Select(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // もし自オブジェクトが非アクティブなら何もしない
            if (!gameObject.activeInHierarchy) return;
            uiManager.CloseUI();
        }
    }



}
