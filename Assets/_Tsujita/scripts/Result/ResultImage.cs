using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ResultImage : MonoBehaviour
{
    [SerializeField] private Image rankImage;               // ランク表示画像
    [SerializeField] private Sprite[] rankSprite;           // ランクごとに画像を設定
    [SerializeField] private RectTransform targetRankImage; // 拡大したい画像

    [SerializeField] private float duration;        // 縮小時間
    [SerializeField] private float startScale;      // 縮小前のサイズ
    [SerializeField] private float endScale;        // 縮小後のサイズ

    [SerializeField] private float standbyTime;             // 待機時間

    private int rankNo;     // ランク番号

    public void GetRank()
    {
        rankNo = ScoreData.instance.rankNoDate;
    }

    public void ImageStart()
    {
        RankImageChange();
    }

    /// <summary>
    /// ランク画像変更
    /// </summary>
    public void RankImageChange()
    {
        rankImage.sprite = rankSprite[rankNo];
        StartCoroutine(RankReduction());
    }

    // 画像の縮小
    private IEnumerator RankReduction()
    {
        Vector3 start = Vector3.one * startScale;
        Vector3 end = Vector3.one * endScale;
        float time = 0f;

        targetRankImage.localScale = start;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            targetRankImage.localScale = Vector3.Lerp(start, end, t);
            yield return null;
        }

        targetRankImage.localScale = end;
        yield return new WaitForSeconds(standbyTime);
        ResultManager.panelFlg = true;
    }
}
