using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

/// <summary>
/// サポートキャラの選択画面。左に使えるサポートの一覧、右にXのサポート / Yのサポートの2枠を表示する。
/// 一覧にカーソルを合わせても情報は出さず、Xボタンで選んだサポートがXの枠に、Yボタンで同様にYの枠に入る。
/// 決定した内容はSupportLoadoutに保存され、戦闘シーンはそこから読む。
/// 進行度(_unlockedCount)により、_supportsの先頭からその数までのサポートだけが一覧に表示される。
/// 武器の装備画面(InventoryEquipmentController)と同じ作りで、一覧の行にはInventoryWeponDataViewを使い回す。
/// </summary>
public class SupportEquipmentController : MonoBehaviour
{
    [Header("サポート一覧(使えるようになる順に並べる)")]
    [SerializeField] private SupportBase[] _supports;

    [Header("進行度: 一覧の先頭からこの数までのサポートだけが表示される(ステージが進んだら増やす)")]
    [SerializeField, Min(0)] private int _unlockedCount = 2;

    [Header("左: サポート一覧")]
    // _printedは一覧のContent。_dataListの行を再利用し、足りない分だけ生成する。
    [SerializeField] private Transform _printed;
    [SerializeField] private InventoryWeponDataView _dataPrefab;
    [SerializeField] private List<InventoryWeponDataView> _dataList = new();
    [SerializeField] private GameObject _screenRoot;
    [SerializeField, Min(0f)] private float _scrollVisibilityMargin = 8f;

    [Header("右: Xのサポート / Yのサポート(要素0 = X、要素1 = Y)")]
    [SerializeField] private UnityEngine.UI.Image[] _slotIcons = new UnityEngine.UI.Image[SupportLoadout.SlotCount];
    [SerializeField] private TMP_Text[] _slotNames = new TMP_Text[SupportLoadout.SlotCount];            // サポート名(アイコン横の1つ目のテキスト)
    [SerializeField] private TMP_Text[] _slotEffects = new TMP_Text[SupportLoadout.SlotCount];          // 効果内容(アイコン横の2つ目のテキスト)
    [FormerlySerializedAs("_slotDescriptions")]   // 前の名前で設定していたInspectorの参照を引き継ぐ
    [SerializeField] private TMP_Text[] _slotLongDescriptions = new TMP_Text[SupportLoadout.SlotCount];   // 詳しい説明文(下の説明欄)

    [Header("案内(任意)")]
    [SerializeField] private TMP_Text _instructionText;

    // 一覧の行に付けるラベル(装備されている枠)
    private static readonly string[] SlotLabels = { "X", "Y" };

    private SupportLoadout _loadout;
    private WeaponListScrollHelper _scroll;
    private InputAction _backAction;
    private InputAction _setXAction;
    private InputAction _setYAction;
    private Coroutine _focusCoroutine;
    private InventoryWeponDataView _focusedView;
    private int _focusedIndex;
    private bool _initialized;
    private int _openedFrame;

    // 進行度の範囲で、実際に一覧へ表示するサポートの数
    private int VisibleCount => _supports == null ? 0 : Mathf.Clamp(_unlockedCount, 0, _supports.Length);

    private void Awake()
    {
        _backAction = new InputAction("SupportBack", InputActionType.Button, "<Gamepad>/buttonEast");
        _setXAction = new InputAction("SupportSetX", InputActionType.Button, "<Gamepad>/buttonWest");
        _setYAction = new InputAction("SupportSetY", InputActionType.Button, "<Gamepad>/buttonNorth");
        _scroll = new WeaponListScrollHelper(_printed, _scrollVisibilityMargin);
    }

    // 初回表示はStartで初期化する。2回目以降は最新の状態で開き直す。
    private void OnEnable()
    {
        _openedFrame = Time.frameCount;
        _backAction.Enable();
        _setXAction.Enable();
        _setYAction.Enable();
        if (_initialized) OpenScreen();
    }

    private void Start()
    {
        _initialized = true;
        OpenScreen();
    }

    // 閉じた画面が入力や通知を受け続けないように解除する。
    private void OnDisable()
    {
        _backAction.Disable();
        _setXAction.Disable();
        _setYAction.Disable();
        if (_loadout != null) _loadout.OnLoadoutChanged -= RefreshMarks;
        if (_focusCoroutine != null) StopCoroutine(_focusCoroutine);
        _focusCoroutine = null;
        var selected = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
        if (selected != null && _screenRoot != null && selected.transform.IsChildOf(_screenRoot.transform))
            EventSystem.current.SetSelectedGameObject(null);
    }

    private void OnDestroy()
    {
        _backAction?.Dispose();
        _setXAction?.Dispose();
        _setYAction?.Dispose();
    }

    /// <summary>
    /// 進行度を設定する(進行度のデータができたら、画面を開く前にここへ渡す)。
    /// 表示されなくなったサポートがセットされていた場合は外す。
    /// </summary>
    public void SetUnlockedCount(int count)
    {
        _unlockedCount = Mathf.Max(0, count);
        if (!_initialized || _loadout == null) return;
        RemoveLockedFromLoadout();
        BuildList();
        _focusedIndex = Mathf.Clamp(_focusedIndex, 0, Mathf.Max(0, VisibleCount - 1));
        FocusAfterFrame(_focusedIndex);
    }

    /// <summary>一覧を組み直し、セット中のサポート(なければ先頭)へカーソルを置く。</summary>
    private void OpenScreen()
    {
        _loadout = SupportLoadout.Instance;
        if (_loadout == null || _printed == null || _dataPrefab == null || _supports == null)
        {
            Debug.LogError("サポート選択画面の参照が設定されていません。", this);
            return;
        }

        // 進行度より先のサポートがセットされたままなら外してから、通知を受け取る。
        _loadout.OnLoadoutChanged -= RefreshMarks;
        RemoveLockedFromLoadout();
        _loadout.OnLoadoutChanged += RefreshMarks;

        BuildList();

        _focusedIndex = 0;
        for (int slot = 0; slot < SupportLoadout.SlotCount; slot++)
        {
            int found = System.Array.IndexOf(_supports, _loadout.Get(slot));
            if (found >= 0 && found < VisibleCount) { _focusedIndex = found; break; }
        }
        if (_instructionText != null)
            _instructionText.text = VisibleCount == 0 ? "使えるサポートがありません <color=red>B</color>=戻る"
                : "スティック/十字キー=移動 <color=blue>X</color>=Xにセット <color=yellow>Y</color>=Yにセット <color=red>B</color>=戻る";
        FocusAfterFrame(_focusedIndex);
    }

    // 表示されないサポート(進行度より先、または一覧にないもの)がセットされていたら外す
    private void RemoveLockedFromLoadout()
    {
        for (int slot = 0; slot < SupportLoadout.SlotCount; slot++)
        {
            var support = _loadout.Get(slot);
            if (support == null) continue;
            int index = System.Array.IndexOf(_supports, support);
            if (index < 0 || index >= VisibleCount) _loadout.TrySet(slot, null);
        }
    }

    /// <summary>行を再利用して一覧を組み直す。表示するのは進行度の範囲だけで、足りない分だけ行を生成する。</summary>
    private void BuildList()
    {
        foreach (var view in _dataList)
            if (view != null) { view.ResetSelection(); view.gameObject.SetActive(false); }

        int count = VisibleCount;
        for (int i = 0; i < count; i++)
        {
            var view = GetOrCreateView(i);
            var support = _supports[i];
            view.SetUpData(support != null ? support.icon : null, GetRowTexts(support));
            view.OnSelectClicked -= OnRowClicked;
            view.OnSelectClicked += OnRowClicked;
            view.gameObject.SetActive(true);
            // 右側の枠へ飛ばないよう、一覧の上下だけを移動先に指定する。
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
        RefreshMarks();
        _scroll.UpdatePadding(_dataList);
    }

    private InventoryWeponDataView GetOrCreateView(int index)
    {
        if (index < _dataList.Count && _dataList[index] != null) return _dataList[index];
        var view = Instantiate(_dataPrefab, _printed);
        while (_dataList.Count <= index) _dataList.Add(null);
        _dataList[index] = view;
        return view;
    }

    // 行に出す文字(1つ目: サポート名、2つ目: 効果内容)。行のプレハブは付呪テキストを2つにしたものを想定
    private static List<string> GetRowTexts(SupportBase support)
    {
        if (support == null) return new List<string> { "-", "" };
        return new List<string> { support.displayName, support.description };
    }

    /// <summary>
    /// A入力やクリックでは何もしない(セットはX/Yボタン)。
    /// ただしToggleの見た目だけ切り替わるので、装備の状態に合わせて元に戻す。
    /// </summary>
    private void OnRowClicked(InventoryWeponDataView view)
    {
        RefreshMarks();
    }

    /// <summary>今カーソルのあるサポートを、指定の枠(0 = X, 1 = Y)に入れる。同じ枠にすでに入っていれば外す。</summary>
    private void AssignFocused(int slot)
    {
        if (_loadout == null) return;
        int index = GetFocusedIndex();
        if (index < 0 || index >= VisibleCount || _supports[index] == null) return;
        _loadout.Assign(slot, _supports[index]);
        // 表示の更新はOnLoadoutChanged(RefreshMarks)で行われる
    }

    // 入力と同じフレームでカーソルが動いていても正しい行になるよう、EventSystemから直接調べる
    private int GetFocusedIndex()
    {
        if (EventSystem.current != null)
        {
            var selected = EventSystem.current.currentSelectedGameObject;
            if (selected != null && selected.transform.IsChildOf(_printed))
            {
                var view = selected.GetComponentInParent<InventoryWeponDataView>();
                int index = view != null ? _dataList.IndexOf(view) : -1;
                if (index >= 0) return index;
            }
        }
        return _focusedIndex;
    }

    /// <summary>各行の「X」「Y」のマークと、右側のXのサポート / Yのサポートの表示を更新する。</summary>
    private void RefreshMarks()
    {
        if (_loadout == null) return;

        int count = Mathf.Min(VisibleCount, _dataList.Count);
        for (int i = 0; i < count; i++)
        {
            var view = _dataList[i];
            if (view == null) continue;
            int slot = _loadout.IndexOf(_supports[i]);
            if (slot >= 0) view.Selection(slot < SlotLabels.Length ? SlotLabels[slot] : (slot + 1).ToString());
            else view.ResetSelection();
        }

        for (int slot = 0; slot < SupportLoadout.SlotCount; slot++)
        {
            var support = _loadout.Get(slot);
            if (_slotIcons != null && slot < _slotIcons.Length && _slotIcons[slot] != null)
            {
                _slotIcons[slot].sprite = support != null ? support.icon : null;
                _slotIcons[slot].enabled = _slotIcons[slot].sprite != null;
            }
            SetSlotText(_slotNames, slot, support != null ? support.displayName : "");
            SetSlotText(_slotEffects, slot, support != null ? support.description : "");
            SetSlotText(_slotLongDescriptions, slot, support != null ? support.longDescription : "");
        }
    }

    // 指定した枠(0 = X, 1 = Y)のテキストを書き換える。テキストが未設定なら何もしない
    private static void SetSlotText(TMP_Text[] targets, int slot, string value)
    {
        if (targets != null && slot < targets.Length && targets[slot] != null) targets[slot].text = value;
    }

    private void Update()
    {
        // 画面を開いたフレームの入力を「戻る」「セット」として扱わない。
        if (!_initialized || Time.frameCount == _openedFrame) return;
        if (_backAction.WasPerformedThisFrame())
        {
            Back();
            return;
        }
        if (_setXAction.WasPerformedThisFrame()) AssignFocused(0);
        if (_setYAction.WasPerformedThisFrame()) AssignFocused(1);
    }

    // UIのカーソル移動後に、選択行が見える位置までスクロールする。
    private void LateUpdate()
    {
        if (!_initialized || _loadout == null) return;
        if (_focusCoroutine != null || EventSystem.current == null) return;
        var selected = EventSystem.current.currentSelectedGameObject;
        // 背景クリックなどで選択が外れても、パッドで操作を再開できるように戻す。
        if (selected == null)
        {
            FocusAfterFrame(_focusedIndex);
            return;
        }
        if (!selected.transform.IsChildOf(_printed)) return;
        var view = selected.GetComponentInParent<InventoryWeponDataView>();
        if (view == null) return;
        if (_focusedView != view)
        {
            _focusedView = view;
            _focusedIndex = _dataList.IndexOf(view);
        }
        _scroll.EnsureVisible(view);
    }

    private void FocusAfterFrame(int index)
    {
        if (_focusCoroutine != null) StopCoroutine(_focusCoroutine);
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
        _focusCoroutine = (index >= 0 && index < VisibleCount) ? StartCoroutine(SelectAfterFrame(index)) : null;
    }

    // 行の有効化とUI更新を1フレーム待ってからカーソルを設定する。
    private IEnumerator SelectAfterFrame(int index)
    {
        yield return null;
        if (EventSystem.current != null && index < _dataList.Count && _dataList[index] != null)
            EventSystem.current.SetSelectedGameObject(_dataList[index].Selectable.gameObject);
        _focusCoroutine = null;
    }

    /// <summary>B入力で選択画面を閉じ、Menuへ操作を戻す。</summary>
    public void Back()
    {
        var menu = FindFirstObjectByType<MeneManager>();
        if (menu == null || _screenRoot == null) return;
        _screenRoot.SetActive(false);
        menu.ShowHome();
    }
}
