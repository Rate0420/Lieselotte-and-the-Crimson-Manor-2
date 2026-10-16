using System.Collections.Generic;
using UnityEngine;

public class FusionUIManager : MonoBehaviour
{
    public FusionUIState state;

    public GameObject weaponSelectPanel;
    public GameObject enchantDraftPanel;
    public GameObject enchantResultPanel;
    public GameObject panel;   // ステージ選択、装備等の選択肢


    public int weaponIndexA = -1;   // 武器画像取得のためpublicにしておく
    int weaponIndexB = -1;
    public int createdWeaponIndex = -1;

    [SerializeField] private MeneManager menuManager;

    [SerializeField] private WeaponInventory inventory;

    public void ChangeState(FusionUIState newState)
    {
        state = newState;

        weaponSelectPanel.SetActive(state == FusionUIState.WeaponSelect);
        enchantDraftPanel.SetActive(state == FusionUIState.EnchantDraft);
        enchantResultPanel.SetActive(state == FusionUIState.EnchantResultPanel);
    }

    public enum FusionUIState
    {
        WeaponSelect,       // 武器2本選ぶ
        EnchantDraft,       // 付呪3つ選ぶ
        EnchantResultPanel, // 合成結果表示
    }

    public void CloseUI()
    {
        ChangeState(FusionUIState.WeaponSelect);
        gameObject.SetActive(false);
        panel.SetActive(true);
        menuManager._panelFlg = true;
    }

    private void OnEnable()
    {
        ChangeState(FusionUIState.WeaponSelect);
    }

    public void OpenEnchantDraft(int indexA, int indexB)
    {
        weaponIndexA = indexA;
        weaponIndexB = indexB;

        var candidates = WeaponFusion.GenerateCandidates(WeaponInventory.Instance.GetWeapon(weaponIndexA),
                                                        WeaponInventory.Instance.GetWeapon(weaponIndexB));

        enchantDraftPanel.GetComponent<EnchantDraftPanel>().SetCandidates(candidates);
        ChangeState(FusionUIState.EnchantDraft);
    }

    public void OnDraftStart()
    {
        if (state != FusionUIState.EnchantDraft)
            return;

        var draftPanel = enchantDraftPanel.GetComponent<EnchantDraftPanel>();

        //if (!draftPanel.IsSelectionComplete)
        //     return;

        // 選択付呪取得
        var selectedEnchants = draftPanel.GetSelectedEnchantments();

        // 元武器取得
        var weaponA = WeaponInventory.Instance.GetWeapon(weaponIndexA);
        var weaponB = WeaponInventory.Instance.GetWeapon(weaponIndexB);

        // 新武器生成
        var newWeapon = WeaponFusion.CreateResultWeapon(
            weaponA,
            selectedEnchants,
            WeaponInventory.Instance.defaultEnchantmentDB
        );

        // インベントリ更新
        SafeRemoveWeapons(weaponIndexA, weaponIndexB);
        WeaponInventory.Instance.AddWeapon(newWeapon);

        // 生成武器のインデックス保存
        createdWeaponIndex = WeaponInventory.Instance.GetAllWeapons().Count - 1;

        // 生成した武器の保存
        inventory.SaveInventory();

        // 結果表示パネルへ
        ChangeState(FusionUIState.EnchantResultPanel);
    }

    void SafeRemoveWeapons(int indexA, int indexB)
    {
        var inventory = WeaponInventory.Instance;

        bool removedEquipped =
            inventory.equipmentIndex == indexA ||
            inventory.equipmentIndex == indexB;

        // 大きい index から消す（重要）
        int max = Mathf.Max(indexA, indexB);
        int min = Mathf.Min(indexA, indexB);

        inventory.RemoveWeapon(max);
        inventory.RemoveWeapon(min);

        if (removedEquipped)
        {
            inventory.equipmentIndex = 0; // 一個目の奴に変更
            Debug.Log("装備中武器が合成されたため解除しました");
        }
    }


}
