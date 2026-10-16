//using UnityEngine;
//using UnityEngine.UI;

//public class InventoryData : MonoBehaviour
//{
//    [Header("武器のデータを格納している場所")]
//    [SerializeField] weapontest weaponTest;

//    [Header("今装備している武器のアイコンを入れるImage")]
//    [SerializeField] RawImage beforeWeaponImage; 
//    [Header("選んでいる武器アイコンを入れるImage")]
//    [SerializeField] RawImage afterWeaponImage;

//    [Header("片手剣と盾のアイコン")]
//    [SerializeField] Texture2D swordAndShieldImage;
//    [Header("両手剣のアイコン")]
//    [SerializeField] Texture2D greatSwordImage;
//    [Header("レイピアのアイコン")]
//    [SerializeField] Texture2D rapierImage;

//    [Header("今装備している武器の付呪の効果を出力するテキスト")]
//    [SerializeField] Text[] beforeEnchantmentEfficacyText;
//    [Header("選んでいる武器の付呪の効果を出力するテキスト")]
//    [SerializeField] Text[] afterEnchantmentEfficacyText;

//    [Header("武器の説明をするテキスト")]
//    [SerializeField] Text explanationText;

//    static int nowWeapon;  // 今装備している武器の場所を格納
//    int selectWeapon = 0;  // 今選んでいる武器の場所を格納

//    [Header("インベントリ内の武器アイコンを格納するImage")]
//    [SerializeField] RawImage[] inventoryImage;

//    void Start()
//    {
//        weaponTest.Test[0].weaponType = WeaponDataBase.WeaponType.Rapier;
//        explanationText.text = "レイピアの説明";
//        weaponTest.Test[0].EnchantmentEfficacy[0, 0] = (int)EnchantmentDataBase.EnchantmentName.移動速度増加;
//        weaponTest.Test[0].EnchantmentEfficacy[1, 0] = (int)EnchantmentDataBase.EnchantmentName.移動速度増加;
//        weaponTest.Test[0].EnchantmentEfficacy[2, 0] = (int)EnchantmentDataBase.EnchantmentName.なし;

//        SetAfterWeaponPanel();
//        SetAfterEnchantmentText();
//        SetInventory();
//        WhenEquipped();
//    }

//    private void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.Return))
//        {
//            WhenEquipped();
//        }

//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            selectWeapon++;
//            SetAfterWeaponPanel();
//            SetAfterEnchantmentText();
//        }
//    }

//    /// <summary>
//    /// 今装備している武器の情報を入れる
//    /// </summary>
//    void WhenEquipped()
//    {
//        inventoryImage[nowWeapon].color = Color.white;

//        // 選んだ武器を装備する
//        nowWeapon = selectWeapon;
//        // 選んだ武器のアイコンをセット
//        beforeWeaponImage.texture = afterWeaponImage.texture;
//        // 選んだ武器に付いている付呪をセットする
//        for (int i = 0; i < beforeEnchantmentEfficacyText.Length; i++)
//            beforeEnchantmentEfficacyText[i].text = afterEnchantmentEfficacyText[i].text;

//        inventoryImage[nowWeapon].color = Color.red;
//    }

//    /// <summary>
//    /// 選んだ武器に応じてアイコンと説明文をを変化させる
//    /// </summary>
//    void SetAfterWeaponPanel()
//    {
//        switch (weaponTest.Test[selectWeapon].weaponType)
//        {
//            case WeaponDataBase.WeaponType.SwordAndShield:
//                afterWeaponImage.texture = swordAndShieldImage;
//                explanationText.text = "片手剣と盾の説明";
//                break;
//            case WeaponDataBase.WeaponType.GreatSword:
//                afterWeaponImage.texture = greatSwordImage;
//                explanationText.text = "両手剣の説明";
//                break;
//            case WeaponDataBase.WeaponType.Rapier:
//                afterWeaponImage.texture = rapierImage;
//                explanationText.text = "レイピアの説明";
//                break;
//        }
//    }

//    /// <summary>
//    /// 選んだ武器の付呪の効果をテキストに出力する
//    /// </summary>
//    void SetAfterEnchantmentText()
//    {
//        for (int i = 0; i < weaponTest.Test[selectWeapon].EnchantmentEfficacy.Length / 2; i++)
//        {
//            switch (weaponTest.Test[selectWeapon].EnchantmentEfficacy[i, 0])
//            {
//                case (int)EnchantmentDataBase.EnchantmentName.HP増加:
//                    afterEnchantmentEfficacyText[i].text = "HP増加 + " + weaponTest.Test[0].EnchantmentLevel[i];
//                    break;

//                case (int)EnchantmentDataBase.EnchantmentName.スタミナ回復速度:
//                    afterEnchantmentEfficacyText[i].text = "スタミナ回復速度 + " + weaponTest.Test[0].EnchantmentLevel[i];
//                    break;

//                case (int)EnchantmentDataBase.EnchantmentName.攻撃力増加:
//                    afterEnchantmentEfficacyText[i].text = "攻撃力増加 + " + weaponTest.Test[0].EnchantmentLevel[i];
//                    break;

//                case (int)EnchantmentDataBase.EnchantmentName.コンボ１撃目のみ強化:
//                    afterEnchantmentEfficacyText[i].text = "コンボ１撃目のみ強化 + " + weaponTest.Test[0].EnchantmentLevel[i];
//                    break;

//                case (int)EnchantmentDataBase.EnchantmentName.致命の一撃を強化:
//                    afterEnchantmentEfficacyText[i].text = "致命の一撃を強化 + " + weaponTest.Test[0].EnchantmentLevel[i];
//                    break;

//                case (int)EnchantmentDataBase.EnchantmentName.戦技を強化:
//                    afterEnchantmentEfficacyText[i].text = "戦技を強化 + " + weaponTest.Test[0].EnchantmentLevel[i];
//                    break;

//                case (int)EnchantmentDataBase.EnchantmentName.攻撃速度増加:
//                    afterEnchantmentEfficacyText[i].text = "攻撃速度増加 + " + weaponTest.Test[0].EnchantmentLevel[i];
//                    break;

//                case (int)EnchantmentDataBase.EnchantmentName.コンボ時間増加:
//                    afterEnchantmentEfficacyText[i].text = "コンボ時間増加 + " + weaponTest.Test[0].EnchantmentLevel[i];
//                    break;

//                case (int)EnchantmentDataBase.EnchantmentName.パリィ猶予増加:
//                    afterEnchantmentEfficacyText[i].text = "パリィ猶予増加 + " + weaponTest.Test[0].EnchantmentLevel[i];
//                    break;

//                case (int)EnchantmentDataBase.EnchantmentName.移動速度増加:
//                    afterEnchantmentEfficacyText[i].text = "移動速度増加 + " + weaponTest.Test[0].EnchantmentLevel[i];
//                    break;

//                case (int)EnchantmentDataBase.EnchantmentName.ガード強化:
//                    afterEnchantmentEfficacyText[i].text = "ガード強化 + " + weaponTest.Test[0].EnchantmentLevel[i];
//                    break;

//                default:
//                    afterEnchantmentEfficacyText[i].text = "なし";
//                    break;
//            }
//        }
//    }

//    /// <summary>
//    /// インベントリ内のアイコンを表示させる処理
//    /// </summary>
//    void SetInventory()
//    {
//        for (int i = 0; i < inventoryImage.Length; i++)
//        {
//            if (i < weaponTest.Test.Count)
//            {
//                // 必要分だけ出現させる
//                inventoryImage[i].gameObject.SetActive(true);
//                // 武器種に応じてアイコンを変える
//                switch (weaponTest.Test[i].weaponType)
//                {
//                    case WeaponDataBase.WeaponType.SwordAndShield:
//                        inventoryImage[i].texture = swordAndShieldImage;
//                        break;
//                    case WeaponDataBase.WeaponType.GreatSword:
//                        inventoryImage[i].texture = greatSwordImage;
//                        break;
//                    case WeaponDataBase.WeaponType.Rapier:
//                        inventoryImage[i].texture = rapierImage;
//                        break;
//                }
//            }
//            // 必要ない分を非表示にする
//            else inventoryImage[i].gameObject.SetActive(false);
//        }
//    }
//}
