using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RankSlide : MonoBehaviour
{
    [SerializeField] Image rankImage;
    [SerializeField] private float slideTime = 0.5f;
    [SerializeField] private float moveDistance = 155;

    [SerializeField] private ResultManager resultManager;
    [SerializeField] private DropTextSlide dropTextSlide;

    private RectTransform rt;
    
    private void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    /// <summary>
    /// ƒ‰ƒ“ƒN‰æ‘œˆÚ“®
    /// </summary>
    public void SlideStart()
    {
        StartCoroutine(ImageSlide());
    }

    private IEnumerator ImageSlide()
    {
        Vector2 startPos = rt.anchoredPosition;
        Vector2 endPos = startPos + new Vector2(0, moveDistance);

        float time = 0;

        while (time < slideTime)
        {
            time += Time.deltaTime;

            float t = time / slideTime;

            rt.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            yield return null;
        }

        rt.anchoredPosition = endPos;

        resultManager.GenePanel();
        dropTextSlide.DropSlideStart();
    }
}
