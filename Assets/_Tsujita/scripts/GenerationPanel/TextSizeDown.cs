using System.Collections;
using UnityEngine;

public class TextSizeDown : MonoBehaviour
{
    [SerializeField] ResultManager resultManager;
    [SerializeField] RankSlide rankSlide;

    [SerializeField] RectTransform[] scoreText;
    [SerializeField] private float reductionTime;

    public void ReductionTexts()
    {
        StartCoroutine(SizeReduction());
    }

    private IEnumerator SizeReduction()
    {
        Vector3 startScale = Vector3.one;
        Vector3 endScale = Vector3.zero;

        float t = 0;

        while (t < reductionTime)
        {
            t += Time.deltaTime;
            float progress = Mathf.Clamp01(t / reductionTime);

            float ease = Mathf.SmoothStep(0f, 1f, progress);

            for (int i = 0; i < scoreText.Length; i++)
            {
                scoreText[i].localScale = Vector3.Lerp(startScale, endScale, ease);
            }

            yield return null;
        }

        for (int i = 0; i < scoreText.Length; i++)
        {
            scoreText[i].localScale = endScale;
        }

        rankSlide.SlideStart();
    }
}
