using System.Collections;
using TMPro;
using UnityEngine;


public class ResultTextScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] scoreText;   // スライドさせるテキスト
    [SerializeField] private ResultImage resultImage;

    private int stageNo;    // ステージ番号
    private int higeScore;  // ハイスコア

    private int scoreNum = 5;

    private Vector2[] defaultPositions; // 元のテキスト座標の保存

    /// <summary>
    /// スライド開始(resultScoreText→ScoreManager)
    /// </summary>
    public void ScoreSlide()
    {
        TextChange();
        InitTextPositions();

        // スコアスライド開始
        StartCoroutine(SlideTexts());
    }

    /// <summary>
    /// スコア受け取り
    /// </summary>
    public void GetScore()
    {
        stageNo = ScoreData.instance.stageNoData;
        higeScore = PlayerPrefs.GetInt("HigeScore" + stageNo);
        // ステージ数の受け取り
    }

    /// <summary>
    /// テキストの内容を変更
    /// </summary>
    private void TextChange()
    {
        scoreText[0].text = ScoreData.instance.totalHPDate.ToString();
        scoreText[1].text = ScoreData.instance.totalComboDate.ToString();
        scoreText[2].text = ScoreData.instance.totalParryDate.ToString();
        scoreText[3].text = ScoreData.instance.totalTimeDate.ToString();
        scoreText[4].text = ScoreData.instance.totalScoreDate.ToString();
        if (higeScore <= ScoreData.instance.higeScoreDate)
        {
            scoreText[5].text = ScoreData.instance.higeScoreDate.ToString();
        }
        else
        {
            scoreText[5].text = higeScore.ToString();
        }
    }

    /// <summary>
    /// テキストを画面外に移動
    /// </summary>
    private void InitTextPositions()
    {
        defaultPositions = new Vector2[scoreText.Length];

        for (int i = 0; i < scoreText.Length; i++)
        {
            RectTransform rt = scoreText[i].rectTransform;

            // 元の位置を保存
            defaultPositions[i] = rt.anchoredPosition;

            // 画面外へ移動
            rt.anchoredPosition = defaultPositions[i] + new Vector2(-1500f, 0f);
        }
    }

    /// <summary>
    /// テキストスライド用
    /// </summary>
    private IEnumerator SlideTexts()
    {
        for (int i = 0; i < scoreText.Length; i++)
        {
            yield return StartCoroutine(SlideFromOutside(scoreText[i], defaultPositions[i]));
        }
    }
    private IEnumerator SlideFromOutside(TextMeshProUGUI tmp, Vector2 targetPos)
    {
        RectTransform rt = tmp.rectTransform;

        Vector2 startPos = rt.anchoredPosition;
        float duration = 0.6f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;

            rt.anchoredPosition = Vector2.Lerp(startPos, targetPos, t / duration);

            yield return null;
        }

        rt.anchoredPosition = targetPos;

        scoreNum--;
        if (scoreNum == 0)
        {
            resultImage.RankImageChange();
        }
    }
}
