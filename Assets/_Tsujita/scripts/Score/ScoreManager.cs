using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    // スコア数値
    [SerializeField] private int scoreHP = 500;     // 残HPスコア
    [SerializeField] private int scoreCombo = 1000; // 最大コンボスコア
    [SerializeField] private int scoreParry = 1500; // パリィ数スコア
    [SerializeField]
    private int[] scoreTime =      // 経過時間スコア
        { 25000, 15000, 10000, 5000 };

    [SerializeField]
    private int[] timeSetting =  // 時間設定
        { 300, 480, 600, 9999 };

    // スコア要素管理
    private int gameHP;     // 残HP
    private int gameCombo;  // 最大コンボ数
    private int gameParry;  // パリィ数
    private float gameTime;   // 経過時間

    [SerializeField] private ResultTextScore resultScoreText;

    // 計算結果用
    private int totalHP;    // 残HP
    private int totalCombo; // 最大コンボ数
    private int totalParry; // パリィ数
    private int totalTime;  // 経過時間
    private int higeScore;  // ハイスコア
    private int totalScore; //合計スコア

    private int stageNo;    // ステージ番号
    private int rankNo;     // ランク番号

    private void Start()
    {
        ScoreReceipt();
        higeScore = PlayerPrefs.GetInt("HigeScore" + stageNo);
        ScoreCalculation();
    }

    /// <summary>
    /// スコア要素受け取り
    /// </summary>
    private void ScoreReceipt()
    {
        gameHP = GameSceneData.instance.gsdHPDeta;          // 残HP
        gameCombo = GameSceneData.instance.gsdComboDeta;    // コンボ数
        gameParry = GameSceneData.instance.gsdParryDeta;    // パリィ数
        gameTime = GameSceneData.instance.gsdTimeDeta;      // 経過時間
        stageNo = ScoreData.instance.stageNoData;       // ステージ数

    }

    /// <summary>
    /// スコア計算
    /// </summary>
    private void ScoreCalculation()
    {
        totalHP = gameHP * scoreHP;
        totalCombo = gameCombo * scoreCombo;
        totalParry = gameParry * scoreParry;
        for (int i = 0; i < timeSetting.Length; i++)
        {
            if (gameTime <= timeSetting[i])
            {
                totalTime = scoreTime[i];
                break;
            }
        }

        // 最終計算
        totalScore = totalHP + totalCombo + totalParry + totalTime;

        // ハイスコアの計算
        if (totalScore > higeScore)
        {
            // もし現在ステージが5(最後)であり、更新前スコアが0(未クリア)であった場合、エンディングのフラグを立てる
            if (stageNo == 5 && higeScore >= 0)
            {
                Debug.Log("ステージナンバーは" + stageNo);
                Debug.Log("ハイスコアは" + higeScore);
                ResultManager.EndingFlg = true;
            }
            Debug.Log("ハイスコアだよ");
            higeScore = totalScore;
            PlayerPrefs.SetInt("HigeScore" + stageNo, totalScore);
            PlayerPrefs.Save();
        }
        Ranking();
        ScoreSave();

        resultScoreText.ScoreSlide();
    }

    /// <summary>
    /// スコア保存
    /// </summary>
    private void ScoreSave()
    {
        ScoreData.instance.totalHPDate = totalHP;
        ScoreData.instance.totalComboDate = totalCombo;
        ScoreData.instance.totalParryDate = totalParry;
        ScoreData.instance.totalTimeDate = totalTime;
        ScoreData.instance.totalScoreDate = totalScore;
        ScoreData.instance.higeScoreDate = higeScore;
        ScoreData.instance.rankNoDate = rankNo;
    }

    /// <summary>
    /// ランク決め
    /// </summary>
    private void Ranking()
    {
        if (100000 <= totalScore)
            rankNo = 0;
        else if (75000 <= totalScore)
            rankNo = 1;
        else if (50000 <= totalScore)
            rankNo = 2;
        else if (25000 <= totalScore)
            rankNo = 3;
        else if (0 <= totalScore)
            rankNo = 4;
    }}
