//using TMPro;
//using UnityEngine;
//using UnityEngine.InputSystem;

//public class ResultManager2 : MonoBehaviour
//{
//    [SerializeField] private GameObject scorePanel;         // スコア表示用パネル
//    [SerializeField] private GameObject generationPanel;    // 武器生成用パネル
//    [SerializeField] private GameObject listPanel;          // 付呪一覧パネル

//    [SerializeField] private ResultTextScore resultScoreText;
//    [SerializeField] private ResultImage resultImage;
//    [SerializeField] private TextSizeDown textSizeDown;

//    [SerializeField] private TextMeshProUGUI stageText; // ステージ番号用テキスト

//    private bool _getFlg;   // 入力を受け付けるかどうか
//    private bool _listFlg;  // 付呪一覧パネルフラグ

//    public static bool panelFlg;      // パネル表示フラグ(ResultTextScoredeで変更)
//    public static bool nextSceneFlg;  // 次のシーンにいけるか(途中)

//    private int stageNo;

//    private void Start()
//    {
//        stageNo = ScoreData.instance.stageNoData;
//        stageText.text = "STAGE : " + stageNo;
//        resultScoreText.GetScore();
//        resultImage.GetRank();

//        panelFlg = false;
//        nextSceneFlg = false;
//        _getFlg = true;
//        _listFlg = false;

//        scorePanel.SetActive(true);
//        generationPanel.SetActive(false);
//        listPanel.SetActive(false);
//    }

//    private void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.Return))
//        {
//            if (panelFlg && _getFlg)
//            {
//                _getFlg = false;
//                textSizeDown.ReductionTexts();
//            }

//            if (!panelFlg && nextSceneFlg)
//            {
//                NextScene();
//            }
//        }

//        if (Input.GetKeyDown(KeyCode.V))
//        {
//            DebugDataDelete();
//        }
//    }

//    /// <summary>
//    /// パネルの切り替え処理
//    /// </summary>
//    public void GenePanel()
//    {
//        panelFlg = false;
//        scorePanel.SetActive(false);
//        generationPanel.SetActive(true);
//    }

//    /// <summary>
//    /// 次のシーン
//    /// </summary>
//    public void NextScene()
//    {
//        // セレクトシーン(拠点)
//        Debug.Log("次のシーン");
//    }

//    /// <summary>
//    /// データの削除
//    /// </summary>
//    private void DebugDataDelete()
//    {
//        PlayerPrefs.DeleteKey("HigeScore" + stageNo);
//        PlayerPrefs.Save();
//        Debug.Log("消去");
//    }

//    /// <summary>
//    /// 付呪一覧パネル切り替え
//    /// </summary>
//    public void OnSubmit(InputAction.CallbackContext context)
//    {
//        Debug.Log("next");
//        if (!panelFlg && !_listFlg)
//        {
//            panelFlg = true;
//            listPanel.SetActive(true);
//        }

//        if (!panelFlg && _listFlg)
//        {
//            panelFlg = false;
//            listPanel.SetActive(false);
//        }
//    }
//}