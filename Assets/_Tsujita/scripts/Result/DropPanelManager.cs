//using Microsoft.Unity.VisualStudio.Editor;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.UIElements;

//public class DropPanelManager : MonoBehaviour
//{
//    [SerializeField] private RawImage weaponImage;        // 表示する画像
//    [SerializeField] private Sprite swordShield;       // 剣盾
//    [SerializeField] private Sprite greatSword;        // 大剣
//    [SerializeField] private Sprite rapier;            // レイピア
//    void Start()
//    {
        
//    }

//    void Update()
//    {
        
//    }

//    public void WeaponImageChenge(WeaponType type)
//    {
//        switch (type)
//        {
//            case WeaponType.sword_shield:
//                weaponImage.sprite = swordShield;
//                break;

//            case WeaponType.greatSword:
//                weaponImage.sprite = greatSword;
//                break;

//            case WeaponType.rapier:
//                weaponImage.sprite = rapier;
//                break;

//            default:
//                Debug.LogWarning("未設定の武器タイプが指定されました: " + type);
//                break;
//        }
//    }
//}