using UnityEngine;

public class DeleteSaveDada : MonoBehaviour
{
    // ハイスコアと武器データの削除
    public void Delete()
    {
        HigeScoreDelete();
        WeaponInventory.Instance.ClearAllWeapons();
        // 初期武器データの追加
        WeaponInventory.Instance.startAddWeapon();  // 初期武器IDを指定して追加
    }

    /// <summary>
    /// データの削除
    /// </summary>
    private void HigeScoreDelete()
    {
        HigeScoreLoad.instans._isStage5Flg = false;
        PlayerPrefs.DeleteAll() ;
        PlayerPrefs.Save();
    }
}
