using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InventoryEquipmentController : MonoBehaviour
{
    [SerializeField] private Transform _printed;
    [SerializeField] private InventoryWeponDataView _dataPrefab;
    [SerializeField] private List<InventoryWeponDataView> _dataList = new();
    [SerializeField] private InventoryEquipmentPreview _preview;
    [SerializeField] private GameObject _screenRoot;
    [SerializeField] private TMP_Text _instructionText;
    [SerializeField, Min(0f)] private float _scrollVisibilityMargin = 8f;

    private WeaponInventory _inventory;
    private WeaponInstance _focusedWeapon;
    private WeaponInstance _lastEquippedWeapon;
    private InventoryWeponDataView _focusedView;
    private WeaponListScrollHelper _scroll;
    private InputAction _backAction;
    private Coroutine _focusCoroutine;
    private bool _initialized;
    private int _openedFrame;

    private void Awake()
    {
        _backAction = new InputAction("EquipmentBack", InputActionType.Button, "<Gamepad>/buttonEast");
        _scroll = new WeaponListScrollHelper(_printed, _scrollVisibilityMargin);
    }

    private void OnEnable()
    {
        _openedFrame = Time.frameCount;
        _backAction.Enable();
        if (_initialized) OpenInventory();
    }

    private void Start()
    {
        _initialized = true;
        OpenInventory();
    }

    private void OnDisable()
    {
        _backAction.Disable();
        if (_inventory != null)
        {
            _inventory.OnInventoryChanged -= RefreshInventory;
            _inventory.OnEquipmentChanged -= RefreshEquipment;
        }
        if (_focusCoroutine != null) StopCoroutine(_focusCoroutine);
        _focusCoroutine = null;
        var selected = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
        if (selected != null && _screenRoot != null && selected.transform.IsChildOf(_screenRoot.transform))
            EventSystem.current.SetSelectedGameObject(null);
    }

    private void OnDestroy() => _backAction?.Dispose();

    private void OpenInventory()
    {
        _inventory = WeaponInventory.Instance;
        if (_inventory == null || _printed == null || _dataPrefab == null || _preview == null)
        {
            Debug.LogError("装備画面の参照が設定されていません。", this);
            return;
        }
        _inventory.OnInventoryChanged -= RefreshInventory;
        _inventory.OnInventoryChanged += RefreshInventory;
        _inventory.OnEquipmentChanged -= RefreshEquipment;
        _inventory.OnEquipmentChanged += RefreshEquipment;
        _focusedWeapon = _inventory.GetWeapon(_inventory.equipmentIndex);
        RefreshInventory();
    }

    private void Update()
    {
        if (_initialized && Time.frameCount != _openedFrame && _backAction.WasPerformedThisFrame()) Back();
    }

    private void LateUpdate()
    {
        if (!_initialized || _inventory == null) return;
        if (_lastEquippedWeapon != _inventory.GetWeapon(_inventory.equipmentIndex)) RefreshEquipment();
        if (_focusCoroutine != null || EventSystem.current == null) return;
        var selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null)
        {
            FocusAfterFrame(FindWeaponIndex(_focusedWeapon));
            return;
        }
        if (!selected.transform.IsChildOf(_printed)) return;
        var view = selected.GetComponentInParent<InventoryWeponDataView>();
        if (view == null) return;
        if (_focusedView != view)
        {
            _focusedView = view;
            _focusedWeapon = _inventory.GetWeapon(_dataList.IndexOf(view));
            ShowComparison();
        }
        _scroll.EnsureVisible(view);
    }

    private void RefreshInventory()
    {
        int index = FindWeaponIndex(_focusedWeapon);
        if (index < 0) index = _inventory.equipmentIndex;
        var weapons = _inventory.GetAllWeapons();
        foreach (var view in _dataList)
            if (view != null) { view.ResetSelection(); view.gameObject.SetActive(false); }
        for (int i = 0; i < weapons.Count; i++)
        {
            var view = GetOrCreateView(i);
            view.SetUpData(weapons[i].Icon, InventoryEquipmentPreview.GetEnchantmentTexts(weapons[i]));
            view.OnSelectClicked -= EquipWeapon;
            view.OnSelectClicked += EquipWeapon;
            view.gameObject.SetActive(true);
            view.Selectable.navigation = new UnityEngine.UI.Navigation
            {
                mode = UnityEngine.UI.Navigation.Mode.Explicit,
                selectOnUp = i > 0 ? _dataList[i - 1].Selectable : null
            };
            if (i > 0)
            {
                var navigation = _dataList[i - 1].Selectable.navigation;
                navigation.selectOnDown = view.Selectable;
                _dataList[i - 1].Selectable.navigation = navigation;
            }
        }
        _focusedView = null;
        _focusedWeapon = weapons.Count > 0 ? _inventory.GetWeapon(Mathf.Clamp(index, 0, weapons.Count - 1)) : null;
        RefreshEquipment();
        _scroll.UpdatePadding(_dataList);
        if (_instructionText != null)
            _instructionText.text = weapons.Count == 0 ? "所持武器がありません <color=red>B</color>=戻る"
                : "スティック/十字キー=移動 <color=green>A</color>=装備する <color=red>B</color>=戻る";
        FocusAfterFrame(FindWeaponIndex(_focusedWeapon));
    }

    private InventoryWeponDataView GetOrCreateView(int index)
    {
        if (index < _dataList.Count && _dataList[index] != null) return _dataList[index];
        var view = Instantiate(_dataPrefab, _printed);
        while (_dataList.Count <= index) _dataList.Add(null);
        _dataList[index] = view;
        return view;
    }

    private void EquipWeapon(InventoryWeponDataView view)
    {
        var weapon = _inventory.GetWeapon(_dataList.IndexOf(view));
        if (weapon == null) return;
        _focusedView = view;
        _focusedWeapon = weapon;
        _inventory.TryEquipWeapon(weapon);
        RefreshEquipment();
    }

    private void RefreshEquipment()
    {
        var weapons = _inventory.GetAllWeapons();
        for (int i = 0; i < weapons.Count; i++)
            if (i == _inventory.equipmentIndex) _dataList[i].Selection("装備中");
            else _dataList[i].ResetSelection();
        ShowComparison();
    }

    private void ShowComparison()
    {
        _lastEquippedWeapon = _inventory.GetWeapon(_inventory.equipmentIndex);
        _preview.ShowComparison(_focusedWeapon, _lastEquippedWeapon);
    }

    private int FindWeaponIndex(WeaponInstance weapon)
    {
        var weapons = _inventory.GetAllWeapons();
        for (int i = 0; i < weapons.Count; i++) if (weapons[i] == weapon) return i;
        return -1;
    }

    private void FocusAfterFrame(int index)
    {
        if (_focusCoroutine != null) StopCoroutine(_focusCoroutine);
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
        _focusCoroutine = index >= 0 && index < _inventory.GetAllWeapons().Count ? StartCoroutine(SelectAfterFrame(index)) : null;
    }

    private IEnumerator SelectAfterFrame(int index)
    {
        yield return null;
        if (EventSystem.current != null && index < _inventory.GetAllWeapons().Count)
            EventSystem.current.SetSelectedGameObject(_dataList[index].Selectable.gameObject);
        _focusCoroutine = null;
    }

    public void Back()
    {
        var menu = FindFirstObjectByType<MeneManager>();
        if (menu == null || _screenRoot == null) return;
        _screenRoot.SetActive(false);
        menu.ShowHome();
    }
}
