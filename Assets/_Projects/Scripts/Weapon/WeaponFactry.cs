using UnityEngine;

public class WeaponFactry : MonoBehaviour
{
    [SerializeField] private WeaponDatabase weaponDatabase;
    [SerializeField] private WeaponInventory weaponInventory;

    void Start()
    {
        //// 仮実装
        //var template = weaponDatabase.GetWeaponTemplateByName("greatsword");
        //if (template != null)
        //{
        //    var instance = new WeaponInstance(template, );
        //    SetEnchant(instance);

        //    weaponInventory.AddWeapon(instance);
        //    Debug.Log("武器の追加に成功しました。武器識別名:" + instance.templateName);
        //}
        //else
        //{
        //    Debug.LogWarning("参照元の名前が確認できませんでした。WeaponDatabase内のListを確認してください");
        //}
    }

    void SetEnchant(WeaponInstance instance)
    {
        //instance.enchantmentSlots[0] = new EnchantmentSlot(weaponDatabase.GetEnchantmentByName("攻撃力増加"), 1);
        //instance.enchantmentSlots[1] = new EnchantmentSlot(weaponDatabase.GetEnchantmentByName("HP増加"), 1);
        //instance.enchantmentSlots[2] = new EnchantmentSlot(weaponDatabase.GetEnchantmentByName("スタミナ回復速度"),1);
        //instance.EnhanceStats();

        //for (int i = 0; i < instance.enchantmentSlots.Length; i++)
        //{
        //    Debug.Log("付呪の追加に成功しました。" + i + "番目：" + instance.enchantmentSlots[i].GetDataName());
        //}
    }
}
