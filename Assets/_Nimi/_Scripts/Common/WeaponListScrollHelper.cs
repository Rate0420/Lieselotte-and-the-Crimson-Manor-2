using System.Collections.Generic;
using UnityEngine;

/// <summary>武器の背景・ハイライトまで含めて見えるよう、余白とスクロールを調整する。</summary>
public sealed class WeaponListScrollHelper
{
    private readonly UnityEngine.UI.ScrollRect _scroll;
    private readonly UnityEngine.UI.VerticalLayoutGroup _layout;
    private readonly float _margin;
    // 初期余白を下限として記録し、一覧更新のたびに余白が増え続けるのを防ぐ。
    private readonly int _originalTop;
    private readonly int _originalBottom;
    private readonly Vector3[] _corners = new Vector3[4];

    /// <summary>合成画面と装備画面で共通利用する。contentにはScrollRect内の一覧を指定する。</summary>
    public WeaponListScrollHelper(Transform content, float margin)
    {
        _margin = Mathf.Max(0f, margin);
        if (content == null) return;
        _scroll = content.GetComponentInParent<UnityEngine.UI.ScrollRect>();
        _layout = content.GetComponent<UnityEngine.UI.VerticalLayoutGroup>();
        if (_layout != null)
        {
            _originalTop = _layout.padding.top;
            _originalBottom = _layout.padding.bottom;
        }
    }

    public void ScrollToTop()
    {
        if (_scroll != null) _scroll.verticalNormalizedPosition = 1f;
    }

    /// <summary>選択した行の背景やハイライトまで、表示領域の内側に収める。</summary>
    public void EnsureVisible(InventoryWeponDataView item)
    {
        if (item == null || _scroll == null || _scroll.content == null || _scroll.viewport == null) return;
        Canvas.ForceUpdateCanvases();
        var viewport = _scroll.viewport;
        float hiddenHeight = GetRectBounds(viewport, _scroll.content).size.y - viewport.rect.height;
        if (hiddenHeight <= 0f) return;
        var bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(viewport, item.transform);
        float minY = viewport.rect.yMin + _margin;
        float maxY = viewport.rect.yMax - _margin;
        // 行が上下にはみ出した分だけ動かす。表示領域より大きい行は中央を合わせる。
        float offset = bounds.size.y > maxY - minY ? bounds.center.y - viewport.rect.center.y
            : bounds.min.y < minY ? bounds.min.y - minY
            : bounds.max.y > maxY ? bounds.max.y - maxY : 0f;
        if (Mathf.Abs(offset) <= 0.01f) return;
        _scroll.StopMovement();
        // 座標上のはみ出し量を、ScrollRectの0～1の位置へ換算する。
        _scroll.verticalNormalizedPosition = Mathf.Clamp01(_scroll.verticalNormalizedPosition + offset / hiddenHeight);
    }

    /// <summary>先頭・末尾でも行の装飾が切れないよう、必要な上下余白を計算する。</summary>
    public void UpdatePadding(IReadOnlyList<InventoryWeponDataView> items)
    {
        if (_layout == null || _scroll == null || _scroll.content == null || _scroll.viewport == null) return;
        Canvas.ForceUpdateCanvases();
        var content = _scroll.content;
        // 表示領域の余白をContentの座標へ換算し、拡大率が違っても同じ見え方にする。
        float scale = Mathf.Abs(_scroll.viewport.InverseTransformVector(content.TransformVector(Vector3.up)).y);
        float margin = _margin / Mathf.Max(scale, 0.0001f);
        float top = _originalTop;
        float bottom = _originalBottom;
        foreach (var item in items)
        {
            if (item == null || !item.gameObject.activeSelf) continue;
            // 行本体と子の装飾を含む範囲との差が、外にはみ出している量になる。
            var rectBounds = GetRectBounds(content, (RectTransform)item.transform);
            var visualBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(content, item.transform);
            top = Mathf.Max(top, visualBounds.max.y - rectBounds.max.y + margin);
            bottom = Mathf.Max(bottom, rectBounds.min.y - visualBounds.min.y + margin);
        }
        var padding = _layout.padding;
        padding.top = Mathf.CeilToInt(top);
        padding.bottom = Mathf.CeilToInt(bottom);
        _layout.padding = padding;
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(content);
    }

    /// <summary>子の装飾を含めず、指定したRectTransform自身の四角を基準座標へ変換する。</summary>
    private Bounds GetRectBounds(Transform relativeTo, RectTransform rect)
    {
        rect.GetWorldCorners(_corners);
        var bounds = new Bounds(relativeTo.InverseTransformPoint(_corners[0]), Vector3.zero);
        for (int i = 1; i < _corners.Length; i++) bounds.Encapsulate(relativeTo.InverseTransformPoint(_corners[i]));
        return bounds;
    }
}
