using System.Collections.Generic;
using UnityEngine;

/// <summary>武器の背景・ハイライトまで含めて見えるよう、余白とスクロールを調整する。</summary>
public sealed class WeaponListScrollHelper
{
    private readonly UnityEngine.UI.ScrollRect _scroll;
    private readonly UnityEngine.UI.VerticalLayoutGroup _layout;
    private readonly float _margin;
    private readonly int _originalTop;
    private readonly int _originalBottom;
    private readonly Vector3[] _corners = new Vector3[4];

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
        float offset = bounds.size.y > maxY - minY ? bounds.center.y - viewport.rect.center.y
            : bounds.min.y < minY ? bounds.min.y - minY
            : bounds.max.y > maxY ? bounds.max.y - maxY : 0f;
        if (Mathf.Abs(offset) <= 0.01f) return;
        _scroll.StopMovement();
        _scroll.verticalNormalizedPosition = Mathf.Clamp01(_scroll.verticalNormalizedPosition + offset / hiddenHeight);
    }

    public void UpdatePadding(IReadOnlyList<InventoryWeponDataView> items)
    {
        if (_layout == null || _scroll == null || _scroll.content == null || _scroll.viewport == null) return;
        Canvas.ForceUpdateCanvases();
        var content = _scroll.content;
        float scale = Mathf.Abs(_scroll.viewport.InverseTransformVector(content.TransformVector(Vector3.up)).y);
        float margin = _margin / Mathf.Max(scale, 0.0001f);
        float top = _originalTop;
        float bottom = _originalBottom;
        foreach (var item in items)
        {
            if (item == null || !item.gameObject.activeSelf) continue;
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

    private Bounds GetRectBounds(Transform relativeTo, RectTransform rect)
    {
        rect.GetWorldCorners(_corners);
        var bounds = new Bounds(relativeTo.InverseTransformPoint(_corners[0]), Vector3.zero);
        for (int i = 1; i < _corners.Length; i++) bounds.Encapsulate(relativeTo.InverseTransformPoint(_corners[i]));
        return bounds;
    }
}
