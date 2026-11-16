using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 武器の基本情報をこの中に詰め込む。
/// Inspectorで武器のデータを登録し、JSONのデータを参照する時にまとめてこちらから行えるようにする。
/// 実行する際は名前で検索する必要がある。
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Objects/WeaponDatabase")]
public class WeaponDatabase:ScriptableObject
{
    [Header("データとして登録する武器テンプレート※現状武器種1つにつき1個のみセット可能")]
    public List<WeaponTemplate> weaponTemplates = new List<WeaponTemplate>();

    /// <summary>
    /// 引数の文字と一致する武器データを取得する関数
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public WeaponTemplate GetWeaponTemplateByName(string name)
    {
        Debug.Log($"検索する武器の素データ名: {name}");
        foreach (var t in weaponTemplates)
        {
            if (t == null)
            {
                Debug.Log("武器の素データがListに入っていません");
                continue;
            }

            // 武器種をチェックして対応する変数を返す
            // 1つの武器種に複数の武器が存在するならこっちに変える↓
            // if (t.weaponName == name)
            if (t.weaponType.ToString() == name)
            {
                return t;
            }
        }
        Debug.LogWarning("該当する武器データが見つかりませんでした");
        return null;
    }
}
