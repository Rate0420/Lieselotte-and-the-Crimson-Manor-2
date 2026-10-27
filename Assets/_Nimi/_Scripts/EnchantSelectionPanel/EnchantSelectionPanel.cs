using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 選択した武器の付呪候補を表示し、最大3つまで選択できるパネル。
/// 同じ名前の付呪でも、選択した項目（インデックス）だけを選択状態にする。
/// </summary>
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
    private readonly List<int> _selectedEnchantIndexes = new();

    private const int MaxSelectCount = 3;

    /// <summary>
    /// 付呪候補を表示する
    /// </summary>
    public void SetUp(List<string> enchantments)
    {
        Clear();

        if (_printed == null || _dataPrefab == null)
        {
            Debug.LogError("生成先または付呪表示プレハブが設定されていません。", this);
            return;
        }

        if (enchantments == null)
        {
            Debug.LogError("付呪候補が null です。", this);
            return;
        }

        for (int i = 0; i < enchantments.Count; i++)
        {
            var data = GetOrCreateData(i);
            data.SetUp(i, enchantments[i]);
            data.gameObject.SetActive(true);

            int index = i;

            data.OnSelectClicked += (selectedIndex, isSelected) =>
            {
                SelectEnchantment(selectedIndex, isSelected);
            };
        }

        UpdateSelectCount();
    }

    /// <summary>
    /// 表示用オブジェクトを取得する。なければ生成する
    /// </summary>
    private EnchantSelectionData GetOrCreateData(int index)
    {
        if (index < _dataList.Count && _dataList[index] != null)
        {
            return _dataList[index];
        }

        var data = Instantiate(_dataPrefab, _printed);

        while (_dataList.Count <= index)
        {
            _dataList.Add(null);
        }

        _dataList[index] = data;
        return data;
    }

    /// <summary>
    /// 付呪を選択または選択解除する
    /// </summary>
    private void SelectEnchantment(int index, bool isSelected)
    {
        if (index < 0 || index >= _dataList.Count || _dataList[index] == null)
        {
            return;
        }

        if (isSelected)
        {
            if (_selectedEnchantIndexes.Count >= MaxSelectCount)
            {
                Debug.Log($"選択できる付呪は{MaxSelectCount}個までです。");
                _dataList[index].Selected(false);
                return;
            }

            if (!_selectedEnchantIndexes.Contains(index))
            {
                _selectedEnchantIndexes.Add(index);
            }
        }
        else
        {
            _selectedEnchantIndexes.Remove(index);
        }

        UpdateSelectCount();
        UpdatePreview();
    }

    /// <summary>
    /// デバッグ用：指定したインデックスの付呪を選択状態にする
    /// </summary>
    public void SetSelectedEnchantmentsForDebug(List<int> enchantIndexes)
    {
        _selectedEnchantIndexes.Clear();

        if (enchantIndexes != null)
        {
            foreach (int index in enchantIndexes)
            {
                if (index >= 0 &&
                    index < _dataList.Count &&
                    _dataList[index] != null &&
                    !_selectedEnchantIndexes.Contains(index))
                {
                    _selectedEnchantIndexes.Add(index);
                }
            }
        }

        foreach (var data in _dataList)
        {
            if (data == null || !data.gameObject.activeSelf) continue;

            bool isSelected = _selectedEnchantIndexes.Contains(data.EnchantmentIndex);

            data.Selected(isSelected);
        }

        UpdateSelectCount();
        UpdatePreview();
    }

    /// <summary>
    /// 選択数表示を更新する
    /// </summary>
    private void UpdateSelectCount()
    {
        if (_selectCountText != null)
        {
            _selectCountText.text = $"{_selectedEnchantIndexes.Count} / {MaxSelectCount}";
        }
    }

    /// <summary>
    /// 選択した付呪をプレビュー03へ反映する
    /// </summary>
    private void UpdatePreview()
    {
        if (_previewPlate == null) return;

        var selectedTexts = new List<string>();

        foreach (int index in _selectedEnchantIndexes)
        {
            if (index >= 0 &&
                index < _dataList.Count &&
                _dataList[index] != null)
            {
                selectedTexts.Add(_dataList[index].EnchantmentText);
            }
        }

        _previewPlate.SetResultEnchantments(selectedTexts);
    }

    /// <summary>
    /// 付呪候補と選択状態をすべて初期化する
    /// </summary>
    public void Clear()
    {
        foreach (var data in _dataList)
        {
            if (data != null)
            {
                data.gameObject.SetActive(false);
                data.Selected(false);
            }
        }

        _selectedEnchantIndexes.Clear();
        UpdateSelectCount();
        UpdatePreview();
    }
}