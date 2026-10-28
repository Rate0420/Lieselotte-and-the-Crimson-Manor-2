using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>合成候補から最大3枠を選ぶ。候補が多い場合は6件ずつ表示する。</summary>
public class EnchantSelectionPanel : MonoBehaviour
{
    [Header("生成設定")]
    [SerializeField] private Transform _printed;
    [SerializeField] private EnchantSelectionData _dataPrefab;
    [Header("選択数表示")]
    [SerializeField] private TMP_Text _selectCountText;
    [Header("プレビュー")]
    [SerializeField] private WeaponsPreviewPlate _previewPlate;

    private readonly List<EnchantSelectionData> _dataList = new();
    private IReadOnlyList<EnchantmentSlot> _candidates = Array.Empty<EnchantmentSlot>();
    private readonly List<int> _selectedEnchantIndexes = new();
    private bool _interactionEnabled;
    private int _pageIndex;
    private GameObject _pageControls;
    private UnityEngine.UI.Button _previousPageButton;
    private UnityEngine.UI.Button _nextPageButton;
    private TMP_Text _pageText;
    private const int MaxSelectCount = 3;
    private const int PageSize = 6;

    public event Action PageChanged;
    public int CandidateCount => _candidates.Count;
    public int PageCount => Math.Max(1, (_candidates.Count + PageSize - 1) / PageSize);
    private int PageStart => _pageIndex * PageSize;
    private int VisibleCount => Math.Min(PageSize, _candidates.Count - PageStart);
    public UnityEngine.UI.Selectable FirstSelectable => VisibleCount > 0 ? _dataList[0].Selectable : null;

    public void SetUp(IReadOnlyList<EnchantmentSlot> enchantments)
    {
        Clear();
        if (_printed == null || _dataPrefab == null)
        {
            Debug.LogError("生成先または付呪表示プレハブが設定されていません。", this);
            return;
        }
        if (enchantments == null) return;
        if (enchantments is WeaponSynthesisCandidates)
            _candidates = enchantments;
        else
        {
            var candidates = new List<EnchantmentSlot>();
            foreach (var slot in enchantments)
                if (slot != null && slot.enchantName != EnchantmentDataBase.EnchantmentName.なし) candidates.Add(slot);
            _candidates = candidates;
        }
        RenderPage();
    }

    public static string FormatEnchantment(EnchantmentSlot slot)
    {
        return slot == null || slot.enchantName == EnchantmentDataBase.EnchantmentName.なし
            ? "なし" : $"{slot.enchantName} + {slot.enchantLevel}";
    }

    private void RenderPage()
    {
        for (int i = 0; i < _dataList.Count; i++)
            if (_dataList[i] != null) _dataList[i].gameObject.SetActive(false);
        for (int i = 0; i < VisibleCount; i++)
        {
            int index = PageStart + i;
            var data = GetOrCreateData(i);
            data.SetUp(index, FormatEnchantment(_candidates[index]));
            data.Selected(_selectedEnchantIndexes.Contains(index));
            data.gameObject.SetActive(true);
        }
        SetInteraction(_interactionEnabled);
    }

    private EnchantSelectionData GetOrCreateData(int index)
    {
        if (index < _dataList.Count && _dataList[index] != null) return _dataList[index];
        var data = Instantiate(_dataPrefab, _printed);
        data.OnSelectClicked += SelectEnchantment;
        while (_dataList.Count <= index) _dataList.Add(null);
        _dataList[index] = data;
        return data;
    }

    private void SelectEnchantment(int index, bool isSelected)
    {
        int row = index - PageStart;
        if (row < 0 || row >= VisibleCount) return;
        if (!_interactionEnabled || (isSelected && !_selectedEnchantIndexes.Contains(index) &&
            _selectedEnchantIndexes.Count >= MaxSelectCount))
        {
            _dataList[row].Selected(_selectedEnchantIndexes.Contains(index));
            return;
        }
        if (isSelected)
        {
            if (!_selectedEnchantIndexes.Contains(index)) _selectedEnchantIndexes.Add(index);
        }
        else _selectedEnchantIndexes.Remove(index);
        UpdatePreview();
    }

    public bool ChangePage(int direction)
    {
        if (!_interactionEnabled) return false;
        int page = (int)Math.Max(0, Math.Min(PageCount - 1, (long)_pageIndex + direction));
        if (page == _pageIndex) return false;
        _pageIndex = page;
        RenderPage();
        PageChanged?.Invoke();
        return true;
    }

    public bool UndoLastSelection()
    {
        if (_selectedEnchantIndexes.Count == 0) return false;
        int index = _selectedEnchantIndexes[_selectedEnchantIndexes.Count - 1];
        _selectedEnchantIndexes.RemoveAt(_selectedEnchantIndexes.Count - 1);
        if (index / PageSize != _pageIndex)
        {
            _pageIndex = index / PageSize;
            RenderPage();
            PageChanged?.Invoke();
        }
        else _dataList[index - PageStart].Selected(false);
        UpdatePreview();
        return true;
    }

    public List<EnchantmentSlot> GetSelectedEnchantments()
    {
        var result = new List<EnchantmentSlot>();
        foreach (int index in _selectedEnchantIndexes)
        {
            var slot = _candidates[index];
            result.Add(new EnchantmentSlot(slot.enchantName, slot.enchantLevel));
        }
        return result;
    }

    public void SetInteraction(bool enabled)
    {
        _interactionEnabled = enabled;
        var grid = _printed != null ? _printed.GetComponent<UnityEngine.UI.GridLayoutGroup>() : null;
        int columns = grid != null && grid.constraint == UnityEngine.UI.GridLayoutGroup.Constraint.FixedColumnCount
            ? Mathf.Max(1, grid.constraintCount) : 3;
        for (int i = 0; i < VisibleCount; i++)
        {
            var selectable = _dataList[i].Selectable;
            if (selectable == null) continue;
            selectable.interactable = enabled;
            selectable.navigation = new UnityEngine.UI.Navigation
            {
                mode = UnityEngine.UI.Navigation.Mode.Explicit,
                selectOnLeft = i % columns > 0 ? VisibleSelectable(i - 1) : null,
                selectOnRight = i % columns < columns - 1 ? VisibleSelectable(i + 1) : null,
                selectOnUp = VisibleSelectable(i - columns),
                selectOnDown = i / columns < (VisibleCount - 1) / columns
                    ? VisibleSelectable(Math.Min(i + columns, VisibleCount - 1)) : null
            };
        }
        UpdatePageControls();
    }

    private UnityEngine.UI.Selectable VisibleSelectable(int index)
    {
        return index >= 0 && index < VisibleCount ? _dataList[index].Selectable : null;
    }

    private void UpdatePageControls()
    {
        if (_pageControls == null && PageCount > 1) CreatePageControls();
        if (_pageControls == null) return;
        _pageControls.SetActive(PageCount > 1);
        _previousPageButton.interactable = _interactionEnabled && _pageIndex > 0;
        _nextPageButton.interactable = _interactionEnabled && _pageIndex < PageCount - 1;
        _pageText.text = $"{_pageIndex + 1} / {PageCount}";
    }

    private void CreatePageControls()
    {
        _pageControls = new GameObject("EnchantPages", typeof(RectTransform));
        _pageControls.layer = gameObject.layer;
        var rect = (RectTransform)_pageControls.transform;
        rect.SetParent(transform, false);
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(0f, 18f);
        rect.sizeDelta = new Vector2(420f, 38f);
        _previousPageButton = CreatePageButton(rect, "PreviousPage", "<", 0f, () => ChangePage(-1));
        _nextPageButton = CreatePageButton(rect, "NextPage", ">", 1f, () => ChangePage(1));
        _pageText = CreatePageText(rect, "PageNumber", string.Empty);
        var textRect = _pageText.rectTransform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(70f, 0f);
        textRect.offsetMax = new Vector2(-70f, 0f);
        _pageText.enableAutoSizing = true;
        _pageText.fontSizeMin = 12f;
        _pageText.fontSizeMax = 24f;
    }

    private UnityEngine.UI.Button CreatePageButton(Transform parent, string name, string label, float anchor, Action clicked)
    {
        var buttonObject = new GameObject(name, typeof(RectTransform), typeof(UnityEngine.UI.Image), typeof(UnityEngine.UI.Button));
        buttonObject.layer = gameObject.layer;
        var rect = (RectTransform)buttonObject.transform;
        rect.SetParent(parent, false);
        rect.anchorMin = new Vector2(anchor, 0f);
        rect.anchorMax = new Vector2(anchor, 1f);
        rect.pivot = new Vector2(anchor, 0.5f);
        rect.sizeDelta = new Vector2(60f, 0f);
        buttonObject.GetComponent<UnityEngine.UI.Image>().color = new Color(0.25f, 0.05f, 0.03f, 0.95f);
        var button = buttonObject.GetComponent<UnityEngine.UI.Button>();
        button.targetGraphic = buttonObject.GetComponent<UnityEngine.UI.Image>();
        button.navigation = new UnityEngine.UI.Navigation { mode = UnityEngine.UI.Navigation.Mode.None };
        button.onClick.AddListener(() => clicked());
        var text = CreatePageText(rect, "Label", label);
        text.rectTransform.anchorMin = Vector2.zero;
        text.rectTransform.anchorMax = Vector2.one;
        text.rectTransform.offsetMin = text.rectTransform.offsetMax = Vector2.zero;
        return button;
    }

    private TMP_Text CreatePageText(Transform parent, string name, string label)
    {
        var textObject = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.layer = gameObject.layer;
        textObject.transform.SetParent(parent, false);
        var text = textObject.GetComponent<TextMeshProUGUI>();
        if (_selectCountText != null) text.font = _selectCountText.font;
        text.text = label;
        text.fontSize = 24f;
        text.alignment = TextAlignmentOptions.Center;
        text.color = new Color(1f, 0.85f, 0.45f, 1f);
        text.raycastTarget = false;
        return text;
    }

    public void SetSelectedEnchantmentsForDebug(List<int> enchantIndexes)
    {
        _selectedEnchantIndexes.Clear();
        if (enchantIndexes != null)
            foreach (int index in enchantIndexes)
            {
                if (_selectedEnchantIndexes.Count == MaxSelectCount) break;
                if (index >= 0 && index < _candidates.Count && !_selectedEnchantIndexes.Contains(index))
                    _selectedEnchantIndexes.Add(index);
            }
        _pageIndex = _selectedEnchantIndexes.Count > 0 ? _selectedEnchantIndexes[0] / PageSize : 0;
        RenderPage();
        UpdatePreview();
    }

    private void UpdatePreview()
    {
        if (_selectCountText != null) _selectCountText.text = $"{_selectedEnchantIndexes.Count} / {MaxSelectCount}";
        if (_previewPlate == null) return;
        var texts = new List<string>();
        foreach (int index in _selectedEnchantIndexes) texts.Add(FormatEnchantment(_candidates[index]));
        _previewPlate.SetResultEnchantments(texts);
    }

    public void Clear()
    {
        foreach (var data in _dataList)
        {
            if (data == null) continue;
            data.Selected(false);
            data.gameObject.SetActive(false);
        }
        _candidates = Array.Empty<EnchantmentSlot>();
        _pageIndex = 0;
        _selectedEnchantIndexes.Clear();
        UpdatePageControls();
        UpdatePreview();
    }
}
