using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class FadeController : MonoBehaviour
{
    [SerializeField] private RectTransform fadeImage;
    [SerializeField] private float fadeTime = 0.5f;
    [SerializeField] private GameObject fadeObject;

    [SerializeField] private PlayerInput inputPlayer;

    private Vector2 rightOffScreenPos;
    private Vector2 leftOffScreenPos;
    private Vector2 centerPos;

    public bool IsFading { get; private set; }

    private void Awake()
    {
        centerPos = Vector2.zero;

        rightOffScreenPos = new Vector2(2000, 0);
        leftOffScreenPos = new Vector2(-2000, 0);

        // 初期状態：右画面外
        fadeImage.anchoredPosition = rightOffScreenPos;
    }

    /// <summary>
    /// フェードイン（右 → 中央）
    /// </summary>
    public IEnumerator FadeInFromRight()
    {
        Debug.Log("インだよ");
        IsFading = true;
        inputPlayer.enabled = false;
        EventSystem.current.enabled = false;

        float time = 0f;
        fadeImage.anchoredPosition = rightOffScreenPos;

        while (time < fadeTime)
        {
            time += Time.deltaTime;
            float t = time / fadeTime;
            fadeImage.anchoredPosition =
                Vector2.Lerp(rightOffScreenPos, centerPos, t);
            yield return null;
        }

        fadeImage.anchoredPosition = centerPos;

        IsFading = false;
        inputPlayer.enabled = true;
    }
    /// <summary>
    /// フェードアウト（中央 → 左）
    /// </summary>
    public IEnumerator FadeOutToLeft()
    {
        Debug.Log("アウトだよ");
        IsFading = true;
        inputPlayer.enabled = false;
        EventSystem.current.enabled = true;

        float time = 0f;
        Vector2 startPos = centerPos;

        while (time < fadeTime)
        {
            time += Time.deltaTime;
            float t = time / fadeTime;
            fadeImage.anchoredPosition =
                Vector2.Lerp(startPos, leftOffScreenPos, t);
            yield return null;
        }

        fadeImage.anchoredPosition = leftOffScreenPos;

        IsFading = false;
        inputPlayer.enabled = true;
    }
}
