using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>合成・装備画面で共用する武器一覧の1行。表示と選択入力の通知を担当する。</summary>
public class InventoryWeponDataView : MonoBehaviour
{
    [Header("武器情報")]
    [SerializeField] private UnityEngine.UI.Image _iconImage;

    [Header("付呪表示")]
    [SerializeField] private TMP_Text[] _enchantmentTexts = new TMP_Text[3];

    [Header("選択表示")]
    [SerializeField] private GameObject _selectedPlane;
    [SerializeField] private UnityEngine.UI.Image _highlightImage;
    [SerializeField] private TMP_Text _selectedText;
    [SerializeField] private UnityEngine.UI.Toggle _selectedToggle;

    public UnityEngine.UI.Selectable Selectable => _selectedToggle;

    public event Action<InventoryWeponDataView> OnSelectClicked;
    // プログラムによる表示変更中は、Toggleの通知をユーザー入力として処理しない。
    private bool _isUpdatingToggle;

    private void Awake()
    {
        ResetSelection();

        if (_selectedToggle != null)
        {
            _selectedToggle.transition = UnityEngine.UI.Selectable.Transition.None;
            _selectedToggle.onValueChanged.AddListener(isSelected =>
            {
                if (_isUpdatingToggle) return;

                OnSelectClicked?.Invoke(this);
            });
        }

        if (_highlightImage != null) _highlightImage.raycastTarget = false;
    }

    private void LateUpdate()
    {
        UpdateHighlight();
    }

    private void OnDisable()
    {
        if (_highlightImage != null) _highlightImage.enabled = false;
    }

    /// <summary>カーソルがある行だけハイライト画像を表示する。素材・装備の確定表示とは区別する。</summary>
    private void UpdateHighlight()
    {
        if (_highlightImage == null) return;
        _highlightImage.enabled = _selectedToggle != null && _selectedToggle.IsInteractable() &&
            EventSystem.current != null && EventSystem.current.currentSelectedGameObject == _selectedToggle.gameObject;
    }

    /// <summary>行を再利用するときに武器アイコンと付呪の文字列を差し替える。</summary>
    public void SetUpData(Sprite icon, List<string> enchantments)
    {
        SetIcon(icon);
        SetEnchantments(enchantments);
    }

    private void SetIcon(Sprite icon)
    {
        if (_iconImage == null)
        {
            Debug.LogError("IconImage が設定されていません。", this);
            return;
        }

        _iconImage.sprite = icon;
        _iconImage.enabled = icon != null;
    }

    private void SetEnchantments(List<string> enchantments)
    {
        if (_enchantmentTexts == null) return;

        for (int i = 0; i < _enchantmentTexts.Length; i++)
        {
            var text = _enchantmentTexts[i];

            if (text == null)
            {
                Debug.LogWarning($"EnchantmentTexts[{i}] が設定されていません。", this);
                continue;
            }

            text.text = string.Empty;

            if (enchantments != null && i < enchantments.Count)
            {
                text.text = enchantments[i];
            }
        }
    }

    /// <summary>確定状態のチェックとラベルを解除し、カーソルのハイライトは現在位置に合わせる。</summary>
    public void ResetSelection()
    {
        SetToggleWithoutEvent(false);

        if (_selectedPlane != null)
        {
            _selectedPlane.SetActive(false);
        }

        if (_selectedText != null)
        {
            _selectedText.text = string.Empty;
        }

        UpdateHighlight();
    }


    /// <summary>合成素材として選ばれた順番を表示する。</summary>
    public void Selection(int index)
    {
        Selection($"素材 {index}");
    }

    /// <summary>確定状態にし、「素材1」や「装備中」など画面ごとのラベルを表示する。</summary>
    public void Selection(string label)
    {
        SetToggleWithoutEvent(true);

        if (_selectedPlane != null)
        {
            _selectedPlane.SetActive(true);
        }

        if (_selectedText != null)
        {
            _selectedText.text = label;
        }
    }


    /// <summary>表示更新でToggleを変更しても、ユーザーの再選択として通知されないようにする。</summary>
    private void SetToggleWithoutEvent(bool value)
    {
        if (_selectedToggle == null) return;

        _isUpdatingToggle = true;
        _selectedToggle.isOn = value;
        _isUpdatingToggle = false;
    }
}
