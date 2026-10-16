using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class EnchantDraftPanel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Transform gridRoot;    // スロット配置先
    [SerializeField] EnchantmentSlotUI slotPrefab;  // スロットUIプレハブ
    [SerializeField] Text[] enchantTexts; // 選択中の付呪表示用テキスト 

    [Header("Input")]
    [SerializeField] float moveInterval = 0.2f; // 移動インターバル

    List<EnchantmentSlot> candidates;           // 付呪候補リスト
    List<EnchantmentSlot> selected = new(3);    // 選択中の付呪リスト
    List<EnchantmentSlotUI> slotUIs = new();    // 付呪スロットUIリスト

    int selectIndex = 0;
    float moveTimer;

    public FusionUIManager fusionUIManager;

    public RawImage weaponIcon;

    [SerializeField] private SoundPush soundPush;

    //public bool IsSelectionComplete => selected.Count == 3;

    public List<EnchantmentSlot> GetSelectedEnchantments()
    {
        return new List<EnchantmentSlot>(selected);
    }


    /// <summary>
    /// 付呪候補リストをセットする
    /// </summary>
    /// <param name="list"></param>
    public void SetCandidates(List<EnchantmentSlot> list)
    {
        candidates = list;
        BuildSlots();
    }

    /// <summary>
    /// 付呪スロットUIを生成する
    /// </summary>
    void BuildSlots()
    {
        // 既存のUIを削除
        foreach (Transform c in gridRoot)
            Destroy(c.gameObject);

        // リスト初期化
        slotUIs.Clear();
        selected.Clear();

        // スロット生成
        foreach (var e in candidates)
        {
            var ui = Instantiate(slotPrefab, gridRoot);
            ui.Bind(e);
            slotUIs.Add(ui);
        }

        selectIndex = 0;
        // weponIconに選択中の武器のアイコンを表示
        weaponIcon.texture = WeaponInventory.Instance.GetWeapon(fusionUIManager.weaponIndexA).Icon.texture;
        UpdateHighlight();
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        moveTimer -= Time.unscaledDeltaTime;
        if (moveTimer > 0f) return;

        // 左右移動
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        if (x > 0.5f)
        {
            selectIndex = Mathf.Clamp(selectIndex + 1, 0, slotUIs.Count - 1);
            moveTimer = moveInterval;
            UpdateHighlight();
        }
        else if (x < -0.5f)
        {
            selectIndex = Mathf.Clamp(selectIndex - 1, 0, slotUIs.Count - 1);
            moveTimer = moveInterval;
            UpdateHighlight();
        }

        // 上下移動
        else if (y > 0.5f)
        {
            selectIndex = Mathf.Clamp(selectIndex - 2, 0, slotUIs.Count - 1);
            moveTimer = moveInterval;
            UpdateHighlight();
        }
        else if (y < -0.5f)
        {
            selectIndex = Mathf.Clamp(selectIndex + 2, 0, slotUIs.Count - 1);
            moveTimer = moveInterval;
            UpdateHighlight();
        }
    }

    /// <summary>
    /// 付呪を選択する
    /// </summary>
    public void Cont_Select()
    {
        if (!gameObject.activeInHierarchy) return;

        var slot = candidates[selectIndex];
        if (selected.Contains(slot)) return;
        if (selected.Count >= 3) return;

        selected.Add(slot);
        soundPush.ChoicePush();
        UpdatePreviewUI();
        UpdateHighlight();
        // もし、選択肢が3つより少なく、表示した付呪を全て選択したら、IsSelectionCompleteをtrueにする
        
     }


    /// <summary>
    /// 直前に選択した付呪を取り消す
    /// </summary>
    public void Cont_Back(InputAction.CallbackContext context)
    {
        if (!gameObject.activeInHierarchy) return;
        if (!context.performed) return;

        HandleBack();
    }

    /// <summary>
    /// 合成開始
    /// </summary>
    public void Cont_Start(InputAction.CallbackContext context)
    {
        if (!gameObject.activeInHierarchy) return;
        if (!context.performed) return;

        fusionUIManager.OnDraftStart();
        soundPush.EnchantPush();
    }

    void HandleBack()
    {
        if (selected.Count == 0)
        {
            ReturnToWeaponSelect();
            return;
        }

        selected.RemoveAt(selected.Count - 1);
        UpdatePreviewUI();
        UpdateHighlight();
    }

    void ReturnToWeaponSelect()
    {
        fusionUIManager.ChangeState(
            FusionUIManager.FusionUIState.WeaponSelect);
    }


    /// <summary>
    /// ハイライト更新
    /// </summary>
    void UpdateHighlight()
    {
        for (int i = 0; i < slotUIs.Count; i++)
        {
            bool isCursor = (i == selectIndex);
            bool isSelected = selected.Contains(candidates[i]);

            slotUIs[i].SetHighlight(isCursor, isSelected);
        }
    }

    void UpdatePreviewUI()
    {
        // 全消去
        for (int i = 0; i < enchantTexts.Length; i++)
            enchantTexts[i].text = "";

        // 選択中の分だけ再表示
        for (int i = 0; i < selected.Count; i++)
        {
            var s = selected[i];
            enchantTexts[i].text =
                $"{s.enchantName}+{s.enchantLevel}";
        }
    }
}
