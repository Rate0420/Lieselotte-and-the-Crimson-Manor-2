using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class WeaponSelectPanel : MonoBehaviour
{

    // 合成UI

    [Header("参照するもの")]
    [SerializeField] private WeaponInventory inventory;  // 武器関連
    [SerializeField] private ItemSlotUI slotPrefab;  // ボタン型のUI
    [SerializeField] private Transform gridRoot;

    [Header("武器詳細UI")]
    [SerializeField] private RawImage selectedWeaponIcon;
    [SerializeField] private Text[] enchantmentTexts;

    [SerializeField] private RawImage[] equipmentWeaponIcon;
    [SerializeField] private Text[] equipmentEnchantmentTexts;

    private List<ItemSlotUI> slotList = new List<ItemSlotUI>();
    private int selectIndex = 0;

    public WeaponTemplate sword_ShieldTemplate;
    public WeaponTemplate greatswordTemplate;
    public WeaponTemplate rapierTemplate;


    private Vector2 moveInput;　// スティック入力用

    [SerializeField] float moveInterval = 0.2f;
    float moveTimer = 0f;

    public GameObject panel;   // ステージ選択、装備等の選択肢

    public GameObject StageSelectMenu;

    int firstSelectedIndex = -1;
    int secondSelectedIndex = -1;

    bool restIsFirst = false;

    public FusionUIManager fusionUIManager;
    [SerializeField] private MeneManager menuManager;

    [SerializeField] ScrollRect scrollRect;

    float targetScrollPos = 1f;
    [SerializeField] float scrollSmoothSpeed = 10f;

    [SerializeField] private SoundPush soundPush;

    [SerializeField]
    private void Awake()
    {
        // 持っている武器のデータを格納する
        if (inventory == null)
            inventory = WeaponInventory.Instance;
        UpdateSelectionUI();
    }

    // このオブジェクトがアクティブの時
    private void OnEnable()
    {
        inventory.OnInventoryChanged += RefreshSlots;
        // スロット生成
        RefreshSlots();
        // 選択中のUIを更新する
        UpdateSelectionUI();
        ResetUI();
    }

    // このオブジェクトが非アクティブになった時
    private void OnDisable()
    {
        inventory.OnInventoryChanged -= RefreshSlots;
        RefreshSlots();
    }

    void Update()
    {
        moveTimer -= Time.unscaledDeltaTime;
        HandleInput();
        UpdateSelectionUI();

        scrollRect.verticalNormalizedPosition =
            Mathf.Lerp(
            scrollRect.verticalNormalizedPosition,
            targetScrollPos,
            scrollSmoothSpeed * Time.unscaledDeltaTime);
    }

    // 入力処理
    private void HandleInput()
    {
        if (slotList.Count == 0) return;

        // コントローラー操作

        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");

        if (moveTimer > 0f) return;

        if (moveInput.x > 0.5f)
        {
            selectIndex = Mathf.Clamp(selectIndex + 1, 0, slotList.Count - 1);
            moveTimer = moveInterval;
            DisplaySelectedWeapon();
        }
        else if (moveInput.x < -0.5f)
        {
            selectIndex = Mathf.Clamp(selectIndex - 1, 0, slotList.Count - 1);
            moveTimer = moveInterval;
            DisplaySelectedWeapon();
        }

        // 上下移動
        if (moveInput.y > 0.5f)
        {
            selectIndex = Mathf.Clamp(selectIndex - 5, 0, slotList.Count - 1);
            moveTimer = moveInterval;
            DisplaySelectedWeapon();
        }
        else if (moveInput.y < -0.5f)
        {
            selectIndex = Mathf.Clamp(selectIndex + 5, 0, slotList.Count - 1);
            moveTimer = moveInterval;
            DisplaySelectedWeapon();
        }
    }
    // 下の2つはPlayerInputでやっているのでこのオブジェクトが非アクティブでも動く

    // 決定
    public void Cont_Select(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // もし自オブジェクトが非アクティブなら何もしない
            if (!gameObject.activeInHierarchy) return;
            SelectWeaponForFusion();
        }
    }

    public void Cont_Start(InputAction.CallbackContext context)
    {
        if (!context.canceled) return;
        // もし自オブジェクトが非アクティブなら何もしない
        if (!gameObject.activeInHierarchy) return;

        if (firstSelectedIndex == -1 || secondSelectedIndex == -1)
            return;

        // FusionUIManager に通知
        fusionUIManager.OpenEnchantDraft(firstSelectedIndex, secondSelectedIndex);
        soundPush.DecisionPush();
    }

    public void ResetUI()
    {
        RefreshSlots();
        firstSelectedIndex = -1;
        secondSelectedIndex = -1;
        restIsFirst = false;   // ←これを追加

        equipmentWeaponIcon[0].texture = null;
        equipmentWeaponIcon[1].texture = null;

        foreach (var text in equipmentEnchantmentTexts)
        {
            text.text = "";
        }

        UpdateSelectionUI();
    }

    void SelectWeaponForFusion()
    {
        if (firstSelectedIndex == selectIndex || secondSelectedIndex == selectIndex)
            return; // 同じ武器は2回選べない

        // 前回選択したものがfirstかsecondかで分岐
        if (!restIsFirst)
        {
            // restisFirstがfalseならfirstを更新
            firstSelectedIndex = selectIndex;
            restIsFirst = true;
            equipmentUI(0);
        }
        else
        {
            secondSelectedIndex = selectIndex;
            restIsFirst = false;
            equipmentUI(1);
        }      
    }

    // 戻る
    public void Cont_Back(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // もし自オブジェクトが非アクティブなら何もしない
            if (!gameObject.activeInHierarchy) return;
            Debug.Log("戻るボタンが押されました");
            // 一つ上の親オブジェクトのsetActiveをfalseにする
            transform.parent.gameObject.SetActive(false);
            soundPush.CancelPush();
            panel.SetActive(true);
            StageSelectMenu.SetActive(false);
            menuManager._panelFlg = true;
        }
    }

    // スロット生成
    private void RefreshSlots()
    {
        // 既存削除
        foreach (Transform child in gridRoot)
            Destroy(child.gameObject);

        slotList.Clear();

        //// 再生成
        foreach (var weapon in inventory.GetAllWeapons())
        {
            // インベントリにボタンを追加する
            var slot = Instantiate(slotPrefab, gridRoot);
            // 
            slot.WeaponBind(weapon);
            slotList.Add(slot);
            slot.SetIcon(weapon);
        }
        selectIndex = Mathf.Clamp(selectIndex, 0, slotList.Count - 1);
    }

    // 選択中の UI 更新
    private void UpdateSelectionUI()
    {
        if (slotList.Count == 0) return;

        // スロットのハイライト更新
        for (int i = 0; i < slotList.Count; i++)
        {
            bool isCursor = (i == selectIndex);
            bool isSelected = (i == firstSelectedIndex || i == secondSelectedIndex);

            slotList[i].SetHighlight(isCursor, isSelected);
        }

        // 選択中武器の詳細表示
        var weapon = inventory.GetWeapon(selectIndex);

        // 付呪の効果をテキストに反映する
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
        ScrollToSelection();
    }

    // 選択中の武器を装備スロットに入れる
    void equipmentUI(int i)
    {
        equipmentWeaponIcon[i].texture = selectedWeaponIcon.texture;
        equipmentEnchantmentTexts[i * 3 + 0].text = enchantmentTexts[0].text;
        equipmentEnchantmentTexts[i * 3 + 1].text = enchantmentTexts[1].text;
        equipmentEnchantmentTexts[i * 3 + 2].text = enchantmentTexts[2].text;
        soundPush.DecisionPush();
    }


    // カーソルで選択している武器を表示する
    void DisplaySelectedWeapon()
    {
        var weapon = inventory.GetWeapon(selectIndex);
        selectedWeaponIcon.texture = weapon.weaponTemplate.weaponIcon.texture;
        UpdateSelectionUI();
    }

    void ScrollToSelection()
    {
        RectTransform content = scrollRect.content;
        RectTransform viewport = scrollRect.viewport;
        RectTransform target = slotList[selectIndex].GetComponent<RectTransform>();

        float contentHeight = content.rect.height;
        float viewportHeight = viewport.rect.height;

        float targetCenter =
            Mathf.Abs(target.anchoredPosition.y) - target.rect.height * 0.5f;

        float normalized =
            targetCenter / (contentHeight - viewportHeight);

        targetScrollPos = Mathf.Clamp01(1f - normalized);
    }

}
