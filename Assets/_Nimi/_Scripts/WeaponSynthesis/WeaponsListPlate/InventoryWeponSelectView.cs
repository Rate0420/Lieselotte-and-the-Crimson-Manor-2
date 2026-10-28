using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>武器選択 → 付呪選択 → 合成を管理する。</summary>
public class InventoryWeponSelectView : MonoBehaviour
{
    [Header("表示設定")]
    [SerializeField] private Transform _printed;
    [SerializeField] private InventoryWeponDataView _dataPrefab;
    [SerializeField] private EnchantSelectionPanel _enchantSelectionPanel;
    [SerializeField] private WeaponsPreviewPlate _previewPlate;
    [SerializeField] private List<InventoryWeponDataView> _dataList = new();
    [Header("画面操作")]
    [SerializeField] private GameObject _screenRoot;
    [SerializeField] private TMP_Text _instructionText;
    [SerializeField, Min(0f)] private float _scrollVisibilityMargin = 8f;
    [Header("デバッグ設定")]
    [SerializeField] private bool _randomSynthesisOnStart;

    private WeaponInventory _inventory;
    private readonly List<WeaponInstance> _selectedWeapons = new();
    private WeaponListScrollHelper _scroll;
    private InputAction _backAction;
    private InputAction _synthesisAction;
    private InputAction _previousEnchantPageAction;
    private InputAction _nextEnchantPageAction;
    private Coroutine _focusCoroutine;
    private bool _initialized;
    private bool _selectingEnchantments;
    private int _openedFrame;
    private const int MaxSelectCount = 2;

    private void Awake()
    {
        _backAction = new InputAction("SynthesisBack", InputActionType.Button, "<Gamepad>/buttonEast");
        _synthesisAction = new InputAction("SynthesisStart", InputActionType.Button, "<Gamepad>/start");
        _previousEnchantPageAction = new InputAction("SynthesisPreviousPage", InputActionType.Button, "<Gamepad>/leftShoulder");
        _nextEnchantPageAction = new InputAction("SynthesisNextPage", InputActionType.Button, "<Gamepad>/rightShoulder");
        if (_enchantSelectionPanel != null) _enchantSelectionPanel.PageChanged += OnEnchantPageChanged;
        if (_screenRoot == null)
        {
            var canvas = GetComponentInParent<Canvas>();
            if (canvas != null) _screenRoot = canvas.gameObject;
        }
        _scroll = new WeaponListScrollHelper(_printed, _scrollVisibilityMargin);
    }

    private void OnEnable()
    {
        _openedFrame = Time.frameCount;
        _backAction.Enable();
        _synthesisAction.Enable();
        _previousEnchantPageAction.Enable();
        _nextEnchantPageAction.Enable();
        if (_initialized) OpenSelection();
    }

    private void Start()
    {
        _inventory = WeaponInventory.Instance;
        _initialized = true;
        OpenSelection();
        if (_randomSynthesisOnStart) RandomSynthesisDebug();
    }

    private void OnDisable()
    {
        _backAction.Disable();
        _synthesisAction.Disable();
        _previousEnchantPageAction.Disable();
        _nextEnchantPageAction.Disable();
        if (_inventory != null) _inventory.OnInventoryChanged -= OnInventoryChanged;
        if (_focusCoroutine != null) StopCoroutine(_focusCoroutine);
        _focusCoroutine = null;
        var selected = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
        if (selected != null && _screenRoot != null && selected.transform.IsChildOf(_screenRoot.transform))
            EventSystem.current.SetSelectedGameObject(null);
    }

    private void OnDestroy()
    {
        _backAction?.Dispose();
        _synthesisAction?.Dispose();
        _previousEnchantPageAction?.Dispose();
        _nextEnchantPageAction?.Dispose();
        if (_enchantSelectionPanel != null) _enchantSelectionPanel.PageChanged -= OnEnchantPageChanged;
    }

    private void OpenSelection()
    {
        if (_inventory == null) _inventory = WeaponInventory.Instance;
        if (_inventory == null || _printed == null || _dataPrefab == null ||
            _enchantSelectionPanel == null || _previewPlate == null)
        {
            Debug.LogError("武器合成画面の参照が設定されていません。", this);
            return;
        }
        _inventory.OnInventoryChanged -= OnInventoryChanged;
        _inventory.OnInventoryChanged += OnInventoryChanged;
        ResetSelection();
    }

    private void Update()
    {
        if (!_initialized || Time.frameCount == _openedFrame) return;
        if (_backAction.WasPerformedThisFrame()) Back();
        else if (_synthesisAction.WasPerformedThisFrame()) TrySynthesize();
        else if (_selectingEnchantments && _previousEnchantPageAction.WasPerformedThisFrame()) _enchantSelectionPanel.ChangePage(-1);
        else if (_selectingEnchantments && _nextEnchantPageAction.WasPerformedThisFrame()) _enchantSelectionPanel.ChangePage(1);
    }

    private void OnEnchantPageChanged()
    {
        if (_selectingEnchantments && isActiveAndEnabled) FocusAfterFrame(_enchantSelectionPanel.FirstSelectable);
    }

    private void LateUpdate()
    {
        if (!_initialized || _inventory == null || EventSystem.current == null || _focusCoroutine != null) return;
        var selected = EventSystem.current.currentSelectedGameObject;
        // 背景クリックなどでフォーカスが外れても、パッドで操作を再開できる。
        if (selected == null)
        {
            var target = _selectingEnchantments ? _enchantSelectionPanel.FirstSelectable : GetWeaponSelectable(0);
            if (target != null) FocusAfterFrame(target);
            return;
        }
        if (_selectingEnchantments || !selected.transform.IsChildOf(_printed)) return;
        _scroll.EnsureVisible(selected.GetComponentInParent<InventoryWeponDataView>());
    }

    private void OnInventoryChanged()
    {
        // 外部から武器が増減した場合も、古いインデックスや素材を使わない。
        ResetSelection();
    }

    private void ResetSelection()
    {
        _selectedWeapons.Clear();
        _enchantSelectionPanel.Clear();
        _previewPlate.ClearPreview();
        InitializeWeaponList();
        SetSelectionStage(false);
        _scroll.ScrollToTop();
        FocusAfterFrame(GetWeaponSelectable(0));
    }

    private void InitializeWeaponList()
    {
        var weapons = _inventory.GetAllWeapons();
        foreach (var view in _dataList)
        {
            if (view == null) continue;
            view.ResetSelection();
            view.gameObject.SetActive(false);
        }
        for (int i = 0; i < weapons.Count; i++)
        {
            var view = GetOrCreateDataView(i);
            view.SetUpData(weapons[i].Icon, CreateEnchantmentTexts(weapons[i].enchantmentSlots));
            view.OnSelectClicked -= OnWeaponClicked;
            view.OnSelectClicked += OnWeaponClicked;
            view.gameObject.SetActive(true);
        }
        _scroll.UpdatePadding(_dataList);
    }

    private InventoryWeponDataView GetOrCreateDataView(int index)
    {
        if (index < _dataList.Count && _dataList[index] != null) return _dataList[index];
        var view = Instantiate(_dataPrefab, _printed);
        while (_dataList.Count <= index) _dataList.Add(null);
        _dataList[index] = view;
        return view;
    }

    private static List<string> CreateEnchantmentTexts(EnchantmentSlot[] slots)
    {
        var texts = new List<string>();
        for (int i = 0; i < 3; i++)
            texts.Add(EnchantSelectionPanel.FormatEnchantment(slots != null && i < slots.Length ? slots[i] : null));
        return texts;
    }

    private void OnWeaponClicked(InventoryWeponDataView view)
    {
        SelectWeapon(_dataList.IndexOf(view));
    }

    private void SelectWeapon(int index)
    {
        var weapons = _inventory.GetAllWeapons();
        if (_selectingEnchantments || index < 0 || index >= weapons.Count) return;
        var weapon = weapons[index];
        if (_selectedWeapons.Contains(weapon)) _selectedWeapons.Remove(weapon);
        else if (_selectedWeapons.Count < MaxSelectCount) _selectedWeapons.Add(weapon);
        RefreshSelection();
        UpdatePreview();
        UpdateEnchantPanel();
        if (_selectedWeapons.Count == MaxSelectCount)
        {
            SetSelectionStage(true);
            FocusAfterFrame(_enchantSelectionPanel.FirstSelectable);
        }
    }

    private void UpdateEnchantPanel()
    {
        var first = _selectedWeapons.Count > 0 ? _selectedWeapons[0] : null;
        var second = _selectedWeapons.Count > 1 ? _selectedWeapons[1] : null;
        _enchantSelectionPanel.SetUp(WeaponSynthesisCandidates.Create(first, second));
    }

    private void RefreshSelection()
    {
        var weapons = _inventory.GetAllWeapons();
        for (int i = 0; i < weapons.Count; i++)
        {
            int selectionIndex = _selectedWeapons.IndexOf(weapons[i]);
            if (selectionIndex < 0) _dataList[i].ResetSelection();
            else _dataList[i].Selection(selectionIndex + 1);
        }
    }

    private void UpdatePreview()
    {
        _previewPlate.ClearPreview();
        for (int i = 0; i < _selectedWeapons.Count; i++)
            _previewPlate.SetWeaponPreview(i, _selectedWeapons[i].Icon, CreateEnchantmentTexts(_selectedWeapons[i].enchantmentSlots));
        if (_selectedWeapons.Count > 0) _previewPlate.SetResultIcon(_selectedWeapons[0].Icon);
    }

    private void SetSelectionStage(bool enchantments)
    {
        _selectingEnchantments = enchantments;
        _enchantSelectionPanel.SetInteraction(enchantments);
        int count = _inventory.GetAllWeapons().Count;
        for (int i = 0; i < count; i++)
        {
            var selectable = GetWeaponSelectable(i);
            if (selectable == null) continue;
            selectable.interactable = !enchantments;
            selectable.navigation = new UnityEngine.UI.Navigation
            {
                mode = UnityEngine.UI.Navigation.Mode.Explicit,
                selectOnUp = GetWeaponSelectable(i - 1),
                selectOnDown = GetWeaponSelectable(i + 1)
            };
        }
        if (_instructionText != null)
            _instructionText.text = enchantments
                ? "<color=green>A</color>=付呪の選択・解除 <color=red>B</color>=戻る <color=#808080>Start</color>=合成（0～3個）"
                : count < 2 ? "合成には武器が2つ必要です <color=red>B</color>=戻る"
                : "武器を2つ選択 <color=green>A</color>=選択・解除 <color=red>B</color>=戻る";
        if (_instructionText != null && enchantments && _enchantSelectionPanel.PageCount > 1)
            _instructionText.text += " LB/RB=ページ切替";
    }

    private UnityEngine.UI.Selectable GetWeaponSelectable(int index)
    {
        return index >= 0 && index < _inventory.GetAllWeapons().Count && index < _dataList.Count
            ? _dataList[index].Selectable : null;
    }

    private void FocusAfterFrame(UnityEngine.UI.Selectable target)
    {
        if (_focusCoroutine != null) StopCoroutine(_focusCoroutine);
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
        _focusCoroutine = StartCoroutine(SelectAfterFrame(target));
    }

    private IEnumerator SelectAfterFrame(UnityEngine.UI.Selectable target)
    {
        // 同じA入力を次のパネルへ持ち越さない。
        yield return null;
        Canvas.ForceUpdateCanvases();
        if (target != null && target.IsInteractable() && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(target.gameObject);
        _focusCoroutine = null;
    }

    public void Back()
    {
        if (_selectingEnchantments)
        {
            if (_enchantSelectionPanel.UndoLastSelection()) return;
            SetSelectionStage(false);
            int index = _selectedWeapons.Count > 0
                ? FindWeaponIndex(_selectedWeapons[_selectedWeapons.Count - 1]) : 0;
            FocusAfterFrame(GetWeaponSelectable(index));
            return;
        }
        if (_selectedWeapons.Count > 0)
        {
            var last = _selectedWeapons[_selectedWeapons.Count - 1];
            _selectedWeapons.RemoveAt(_selectedWeapons.Count - 1);
            RefreshSelection();
            UpdatePreview();
            UpdateEnchantPanel();
            FocusAfterFrame(GetWeaponSelectable(FindWeaponIndex(last)));
            return;
        }
        var menu = FindFirstObjectByType<MeneManager>();
        if (menu != null && _screenRoot != null)
        {
            _screenRoot.SetActive(false);
            menu.ShowHome();
        }
    }

    private int FindWeaponIndex(WeaponInstance weapon)
    {
        var weapons = _inventory.GetAllWeapons();
        for (int i = 0; i < weapons.Count; i++) if (weapons[i] == weapon) return i;
        return -1;
    }

    public bool TrySynthesize()
    {
        if (!_selectingEnchantments || _selectedWeapons.Count != MaxSelectCount) return false;
        var first = _selectedWeapons[0];
        var second = _selectedWeapons[1];
        if (FindWeaponIndex(first) < 0 || FindWeaponIndex(second) < 0 || first.weaponTemplate == null) return false;
        var result = new WeaponInstance(first.weaponTemplate, _inventory.defaultEnchantmentDB);
        result.SetEnchantments(_enchantSelectionPanel.GetSelectedEnchantments().ToArray());
        // 保存・装備の補正・一覧への通知をまとめて行う。
        return _inventory.TryReplaceWeapons(first, second, result);
    }

    [ContextMenu("Random Synthesis Debug")]
    public void RandomSynthesisDebug()
    {
        if (_inventory == null || _inventory.GetAllWeapons().Count < 2) return;
        ResetSelection();
        var indexes = Enumerable.Range(0, _inventory.GetAllWeapons().Count).OrderBy(_ => Random.value).Take(2).ToArray();
        SelectWeapon(indexes[0]);
        SelectWeapon(indexes[1]);
        var selectedIndexes = new List<int>();
        int count = _enchantSelectionPanel.CandidateCount;
        while (selectedIndexes.Count < Mathf.Min(3, count))
        {
            int index = Random.Range(0, count);
            if (!selectedIndexes.Contains(index)) selectedIndexes.Add(index);
        }
        _enchantSelectionPanel.SetSelectedEnchantmentsForDebug(selectedIndexes);
    }
}
