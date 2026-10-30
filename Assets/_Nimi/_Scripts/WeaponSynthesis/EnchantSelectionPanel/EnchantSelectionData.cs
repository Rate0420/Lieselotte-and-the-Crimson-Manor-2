using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>付呪候補1件の表示とToggle入力を担当し、候補番号と選択状態を親パネルへ通知する。</summary>
public class EnchantSelectionData : MonoBehaviour
{
    [SerializeField] private TMP_Text _enchantmentText;

    [SerializeField] private UnityEngine.UI.Image _highlightImage;
    [SerializeField] private UnityEngine.UI.Toggle _selectedToggle;

    public UnityEngine.UI.Selectable Selectable => _selectedToggle;

    public string EnchantmentText { get; private set; }
    public int EnchantmentIndex { get; private set; }

    public event Action<int, bool> OnSelectClicked;

    private bool _isUpdatingToggle;

    private void Awake()
    {
        if (_selectedToggle != null)
        {
            _selectedToggle.transition = UnityEngine.UI.Selectable.Transition.None;
            _selectedToggle.onValueChanged.AddListener(isSelected =>
            {
                if (_isUpdatingToggle) return;
                if (!NimiUIConfirmInput.TryConsume())
                {
                    // 武器選択時に押したAを保持したまま、付呪が選択されることも防ぐ。
                    _selectedToggle.SetIsOnWithoutNotify(!isSelected);
                    return;
                }

                OnSelectClicked?.Invoke(EnchantmentIndex, isSelected);
            });
        }

        if (_highlightImage != null)
        {
            _highlightImage.raycastTarget = false;
            _highlightImage.enabled = false;
        }
    }

    /// <summary>現在のカーソル位置だけをハイライトする。チェック済みかどうかとは独立した表示にする。</summary>
    private void LateUpdate()
    {
        if (_highlightImage == null) return;
        _highlightImage.enabled = _selectedToggle != null && _selectedToggle.IsInteractable() &&
            EventSystem.current != null && EventSystem.current.currentSelectedGameObject == _selectedToggle.gameObject;
    }

    private void OnDisable()
    {
        if (_highlightImage != null) _highlightImage.enabled = false;
    }

    /// <summary>ページ内の行を再利用し、候補一覧全体での番号と表示文字列を更新する。</summary>
    public void SetUp(int index, string enchantment)
    {
        EnchantmentIndex = index;
        EnchantmentText = enchantment;

        if (_enchantmentText != null)
        {
            _enchantmentText.text = enchantment;
        }
    }

    /// <summary>パネル側から選択表示を同期する。Toggle変更の通知が再帰的に発生するのを防ぐ。</summary>
    public void Selected(bool isSelected)
    {
        if (_selectedToggle == null) return;

        _isUpdatingToggle = true;
        _selectedToggle.isOn = isSelected;
        _isUpdatingToggle = false;
    }
}
