using TMPro;
using UnityEngine;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private GameObject scorePanel;         // スコア表示用パネル
    [SerializeField] private GameObject generationPanel;    // 武器生成用パネル
    [SerializeField] private GameObject listPanel;          // 付呪一覧パネル

    [SerializeField] private ResultTextScore resultScoreText;
    [SerializeField] private ResultImage resultImage;
    [SerializeField] private TextSizeDown textSizeDown;
    [SerializeField] private SoundPush soundPush;

    [SerializeField] private StoryData EndingData; // エンディング用のストーリーデータ

    [SerializeField] private TextMeshProUGUI stageText; // ステージ番号用テキスト

    private bool _getFlg;   // 入力を受け付けるかどうか
    private bool _listFlg;  // 付呪一覧パネルフラグ

    public static bool panelFlg;      // パネル表示フラグ(ResultTextScoredeで変更)
    public static bool nextSceneFlg;  // 次のシーンにいけるか(途中)
    public static bool EndingFlg;     // エンディングをだすかどうか

    private int stageNo;

    private void Awake()
    {
        SoundManager.Instance.PlayBGM("ResultBGM");
    }

    private void Start()
    {
        stageNo = ScoreData.instance.stageNoData;
        stageText.text = "STAGE : " + stageNo;
        resultScoreText.GetScore();
        resultImage.GetRank();
        StageClearCheck.SetStageClear(stageNo);

        panelFlg = false;
        nextSceneFlg = false;
        _getFlg = true;
        _listFlg = false;

        scorePanel.SetActive(true);
        generationPanel.SetActive(false);
        listPanel.SetActive(false);
    }

    /// <summary>
    /// パネルの切り替え処理
    /// </summary>
    public void GenePanel()
    {
        panelFlg = false;
        scorePanel.SetActive(false);
        generationPanel.SetActive(true);
    }

    /// <summary>
    /// 次のシーン
    /// </summary>
    public void NextScene()
    {
        ImageChange();
        // セレクトシーン(拠点)
        if (EndingFlg)
        { 
            DontDestroyStory.instance.story = EndingData;
            FadeSceneChanger.ChangeScene("StoryScene_SakaguchiTest");
            EndingFlg = false;
        }
        else
            FadeSceneChanger.ChangeScene("SelectScene");
        //SceneManager.LoadScene("SelectScene");
    }

    /// <summary>
    /// 付呪一覧パネル切り替え
    /// </summary>
    public void OnMenu()
    {
        if (_getFlg) return;

        if (_listFlg) soundPush.CancelPush();
        else if (!_listFlg) soundPush.DecisionPush();
        _listFlg = !_listFlg;
        listPanel.SetActive(_listFlg);
    }

    /// <summary>
    /// 決定で進む
    /// </summary>
    public void OnSubmit()
    {
        if (panelFlg && _getFlg)
        {
            soundPush.DecisionPush();
            _getFlg = false;
            textSizeDown.ReductionTexts();
        }

        if (!panelFlg && nextSceneFlg)
        {
            soundPush.DecisionPush();
            NextScene();
        }
    }

    /// <summary>
    ///  最終ステージクリアでシルエットからカラーに変更
    /// </summary>
    private void ImageChange()
    {
        if(stageNo == 5)
        {
            HigeScoreLoad.instans._isStage5Flg = true;
        }
    }
}