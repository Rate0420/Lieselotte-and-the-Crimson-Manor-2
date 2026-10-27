using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CharaSaveManager;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [SerializeField] private TextMeshProUGUI tutorialText;  // チュートリアル用テキスト
    [SerializeField] private TextMeshProUGUI tutorialGUI;   // 操作説明用テキスト
    [SerializeField] private TextMeshProUGUI countText;     // 回数表示用
    [SerializeField] private TextMeshProUGUI exprainText;   // チュートリアルの説明用テキスト

    [SerializeField] private GameObject buttonImage;
    [SerializeField] private Image tagetImage;
    [SerializeField] private Sprite[]controllerSprite;

    public int tutorialNum;
    [SerializeField] private int tutorialCount = 6;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private EnemyController enemyController;

    [SerializeField] StoryData tutorialEnd;

    private int EnemyPower; // チュートリアル用の敵の攻撃力保存用変数

    public bool isTutorialCompleted = false;
    public bool isNextFlg = false;
    public bool isGuardFlg = false;

    bool isloadead = false;

    float TutorialEnemyHpTimer = 0;

    private void Awake()
    {
        Instance = this;
        SoundManager.Instance.PlayBGM("Werewolf Warrior");
        CharacterType selectedCharacter =
            CharaSaveManager.Instance.GetSelectedCharacter();
        Debug.Log("選択キャラ" +  selectedCharacter);
    }

    private void Update()
    {
        if (!isTutorialCompleted) TutorialEnemyHpTimer += Time.deltaTime;
        // チュートリアルの内容に応じて増やす
        switch (tutorialNum)
        {
            case 0:
                tutorialText.text = "攻撃を成功させる";
                tutorialGUI.text = "RB(R1)";
                exprainText.text = "左スティックで移動\r\n続けて攻撃するとコンボができるぞ！";
                TextChenge();
                enemyController.tutorialState = EnemyController.TutorialState.Move;
                break;
            case 1:
                tutorialText.text = "ガードを成功させる";
                tutorialGUI.text = "LB(L1)";
                exprainText.text = "スタミナの残量は緑色のゲージに注目！\r\nガード等をすると減るぞ！";
                TextChenge();
                enemyController.tutorialState = EnemyController.TutorialState.Attack1;
                break;
            case 2:
            case 3:
            case 4:
                if (CharaSaveManager.Instance.GetSelectedCharacter() == CharacterType.Liselotte)
                    TutorialLiselotte();
                else
                    TutorialVelmilion();
                break;
            case 5:
                tutorialText.text = "サポートキャラを使おう";
                tutorialGUI.text = "X or Y";
                exprainText.text = "戦闘でそれぞれ1回だけ使える\r\n様々なバフを使いこなせ！";
                TextChenge();
                enemyController.tutorialState = EnemyController.TutorialState.Attack1;
                break;
            case 6:
                tutorialText.text = "敵を倒そう";
                tutorialGUI.text = "";
                exprainText.text = "";
                TextChenge();
                buttonImage.SetActive(false);
                if (!isTutorialCompleted)
                {
                    enemyController.tutorialState = EnemyController.TutorialState.Attack1;
                    enemyController.EndTutorial();
                    isTutorialCompleted = true;
                }
                break;
            case 7:
                Debug.Log("チュートリアル終了");
                // チュートリアル終了処理
                if (!isloadead)
                {
                    Debug.Log("ストーリーシーンに遷移");
                    isloadead = true;
                    DontDestroyStory.instance.story = tutorialEnd;
                    FadeSceneChanger.ChangeScene("StoryScene_SakaguchiTest");
                }
                break;
        }

        if (TutorialEnemyHpTimer > 5)
        {
            TutorialEnemyHpTimer = 0;
            enemyController.hpBar.nowHp = enemyController.hpBar.maxHp;
        }
    }

    /// <summary>
    /// リーゼロッテ用チュートリアル
    /// </summary>
    private void TutorialLiselotte()
    {
        switch (tutorialNum)
        {
            case 2:
                tutorialText.text = "パリィを成功させる";
                tutorialGUI.text = "右スティック";
                exprainText.text = "敵の攻撃に【タイミング・向き】を\r\n合わせてパリィ！";
                isGuardFlg = false;
                TextChenge();
                break;
            case 3:
                tutorialText.text = "ガードカウンターを成功させる";
                tutorialGUI.text = "LB(L1)→LT(L2)";
                exprainText.text = "ガード成功後\rLT(L2)でカウンター！";
                isGuardFlg = false;
                TextChenge();
                break;
            case 4:
                tutorialText.text = "戦技を成功させる";
                tutorialGUI.text = "RT(R2)";
                exprainText.text = "攻撃・パリィでコンボを増やす\r\n10コンボ以上で【戦技】を使えるぞ！";
                TextChenge();
                enemyController.tutorialState = EnemyController.TutorialState.Move;
                break;
            }
        }

        /// <summary>
        /// ヴェルミリオン専用チュートリアル
        /// </summary>
        private void TutorialVelmilion()
    {
        switch (tutorialNum)
        {
            case 2:
                tutorialText.text = "ジャスガを成功させる";
                tutorialGUI.text = "LB(L1)";
                exprainText.text = "【タイミング】を合わせてガード！\r\nスタミナの消費０・コンボ継続\r\n";
                isGuardFlg = true;
                TextChenge();
                break;
            case 3:
                tutorialText.text = "緊急回避を成功させる";
                tutorialGUI.text = "LT(L2)";
                exprainText.text = "被弾中でも発動可能\r\n迷ったら使おう！";
                isGuardFlg = false;
                TextChenge();
                break;
            case 4:
                tutorialText.text = "戦技を成功させる";
                tutorialGUI.text = "RT(R2)";
                exprainText.text = "攻撃・パリィでコンボを増やす\r\n10コンボ以上で【戦技】を使えるぞ！";
                TextChenge();
                enemyController.tutorialState = EnemyController.TutorialState.Move;
                break;
        }
    }

    private void TextChenge()
    {
        if (isNextFlg) 
        {
            tutorialText.text = "BACKを押して\r\n次に進もう！";
            exprainText.text = "";
        }

        if (tutorialNum < 4)
            countText.text = "あと" + tutorialCount + "回";
        else
            countText.text = "";

        if(tutorialNum < 5)
            tagetImage.sprite = controllerSprite[tutorialNum];
    }

    public void OnNext()
    {
        if (isNextFlg) TutorialProgress();
        
        isNextFlg = false;
    }

    /// <summary>
    /// 次のステップに進む
    /// </summary>
    private void TutorialProgress()
    {
        tutorialNum++;
        if(tutorialNum < 4)
            tutorialCount = 3;
        else
            tutorialCount = 1;
    }

    /// <summary>
    /// 回数カウント
    /// </summary>
    public void TutorialCount()
    {
        if (tutorialCount > 0) 
            tutorialCount--;

        if (tutorialCount == 0)
            isNextFlg = true;
    }
}
