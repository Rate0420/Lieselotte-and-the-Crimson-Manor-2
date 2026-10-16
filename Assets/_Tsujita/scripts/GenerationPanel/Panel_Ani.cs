using System.Collections;
using UnityEngine;

public class Panel_Ani : MonoBehaviour
{
    [SerializeField] private RectTransform panel;   // ターゲットパネル
    [SerializeField] private float duration = 0.3f; // 拡大縮小時間

    private Vector3 defaultScale;
    private Coroutine currentCoroutine;

    private void Awake()
    {
        defaultScale = panel.localScale;
    }

    /// <summary>
    /// パネル表示（拡大）
    /// </summary>
    public void OpenPanel()
    {
        panel.gameObject.SetActive(true);

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(ScalePanel(Vector3.zero, defaultScale));
    }

    /// <summary>
    /// パネル非表示（縮小）
    /// </summary>
    public void ClosePanel()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(ScalePanel(panel.localScale, Vector3.zero, true));
    }

    private IEnumerator ScalePanel(Vector3 start, Vector3 end, bool deactivate = false)
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = duration <= 0 ? 1f : time / duration;

            panel.localScale = Vector3.Lerp(start, end, t);
            yield return null;
        }

        panel.localScale = end;

        if (deactivate)
            panel.gameObject.SetActive(false);
    }
}
