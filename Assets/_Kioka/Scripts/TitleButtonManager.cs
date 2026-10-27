using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TitleButtonManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button[] button;
    [SerializeField] private GameObject[] ui;
    [SerializeField] private CanvasGroup[] canvasGroup;
    [SerializeField] private GameObject[] escapeButton;

    [Header("音量")]
    [SerializeField] private Slider bgmSlider;

    [Header("テキスト")]
    [SerializeField] private TextMeshProUGUI text;

    [Header("ストーリー")]
    [SerializeField] private StoryData storyData;
    [SerializeField] private string[] scene;

    [Header("セーブデータ")]
    [SerializeField] private DeleteSaveDada deleteSaveDada;

    [Header("サウンド")]
    [SerializeField] private SoundPush soundPush;

    // ストーリーシーン
    private string scene2;

    // 確認画面で削除を行うか
    private bool delete;

    // 現在の画面状態
    private bool isTitleFlg = false;
    private bool isStartFlg = false;
    private bool isOptionFlg = false;
    private bool isVolumeFlg = false;
    private bool isConfFlg = false;
    private bool isCharaFlg = false;

    private void Start()
    {
        Initialize();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void Initialize()
    {
        SoundManager.Instance.PlayBGM("TitleBGM");
        InitializeCanvasGroups();
        scene2 = storyData.sceneName[0];
        ShowTitle();
        SelectButton(0);
    }

    /// <summary>
    /// CanvasGroupの初期化
    /// </summary>
    private void InitializeCanvasGroups()
    {
        for (int i = 0; i < ui.Length; i++)
        {
            canvasGroup[i] = ui[i].GetComponent<CanvasGroup>();
            canvasGroup[i].alpha = 0f;
        }
    }

    /// <summary>
    /// タイトル画面の表示
    /// </summary>
    private void ShowTitle()
    {
        ShowUI(4);
        HideUI(0);
        HideUI(1);
        HideUI(2);
        HideUI(3);

        isTitleFlg = true;
        isStartFlg = false;
        isOptionFlg = false;
        isVolumeFlg = false;
        isConfFlg = false;
    }

    /// <summary>
    /// ボタンを押したときの処理
    /// </summary>
    public void OnClickButton(int number)
    {
        switch (number)
        {
            case 0:
                StartGameSelect();
                break;
            case 1:
                StartNewGame();
                break;
            case 2:
                ContinueGame();
                break;
            case 3:
                OpenOption();
                break;
            case 4:
                OpenVolume();
                break;
            case 5:
                CloseVolume();
                break;
            case 6:
                OpenDeleteConfirm();
                break;
            case 7:
                CloseOption();
                break;
            case 8:
                OpenExitConfirm();
                break;
            case 9:
                ConfirmYes();
                break;
            case 10:
                ConfirmNo();
                break;
            case 11:
                CloseGameStart();
                break;
            case 12:
                StartLiselotte();
                break;
            case 13:
                StartVelmilion();
                break;
        }
    }

    /// <summary>
    /// 「ゲーム開始」を押したとき
    /// </summary>
    private void StartGameSelect()
    {
        PlayDecisionSound();
        HideUI(4);
        ShowUI(0);          // 「初めから・続きから」を表示
        SelectButton(1);    // 初めからを選択
        isTitleFlg = false;
        isStartFlg = true;
    }

    /// <summary>
    /// 「初めから」を押したとき
    /// </summary>
    private void StartNewGame()
    {
        PlayDecisionSound();

        // セーブデータを削除
        PlayerPrefs.DeleteAll();
        deleteSaveDada.Delete();

        HideUI(0);
        ShowUI(5);
        SelectButton(4);    // リーゼロッテを選択

        isStartFlg = false;
        isCharaFlg = true;
    }

    /// <summary>
    /// 「リーゼロッテ」を押したとき
    /// </summary>
    private void StartLiselotte()
    {
        PlayDecisionSound();
        Debug.Log("リーゼロッテ");
        DontDestroyStory.instance.story = storyData;
        FadeSceneChanger.ChangeScene("StoryScene_SakaguchiTest");
    }

    /// <summary>
    /// 「ヴェルミリオン」を押したとき
    /// </summary>
    private void StartVelmilion()
    {
        PlayDecisionSound();
        Debug.Log("ヴェルミリオン");
        DontDestroyStory.instance.story = storyData;
        FadeSceneChanger.ChangeScene("StoryScene_SakaguchiTest");
    }

    /// <summary>
    /// 「続きから」を押したとき
    /// </summary>
    private void ContinueGame()
    {
        PlayDecisionSound();

        FadeSceneChanger.ChangeScene(scene[2]);
    }

    /// <summary>
    /// 設定画面を開く
    /// </summary>
    private void OpenOption()
    {
        PlayDecisionSound();
        ShowUI(1);          // 設定画面を表示
        SelectButton(2);    // 音量設定ボタンを選択

        isTitleFlg = false;
        isStartFlg = false;
        isOptionFlg = true;
    }

    /// <summary>
    /// 音量設定画面を開く
    /// </summary>
    private void OpenVolume()
    {
        PlayDecisionSound();
        HideUI(1);        // 設定画面を非表示
        ShowUI(2);        // 音量設定画面を表示
        bgmSlider.Select();

        isVolumeFlg = true;
    }

    /// <summary>
    /// 音量設定画面を閉じる
    /// </summary>
    private void CloseVolume()
    {
        PlayDecisionSound();
        HideUI(2);        // 音量設定画面を非表示
        ShowUI(1);        // 設定画面を表示
        SelectButton(2);  // 音量設定ボタンを選択

        isVolumeFlg = false;
    }

    /// <summary>
    /// 設定画面を閉じる
    /// </summary>
    private void CloseOption()
    {
        PlayDecisionSound();
        HideUI(1);        // 設定画面を非表示
        ShowUI(4);        // タイトル画面を表示
        SelectButton(0);  // Startボタンを選択

        isOptionFlg = false;
        isTitleFlg = true;
    }

    /// <summary>
    /// セーブデータ削除確認画面を開く
    /// </summary>
    private void OpenDeleteConfirm()
    {
        PlayDecisionSound();
        HideUI(1);
        ShowUI(3);
        SelectButton(3);    // はい

        text.text = "セーブデータを削除しますか？";

        delete = true;
        isOptionFlg = false;
        isConfFlg = true;
    }

    /// <summary>
    /// ゲーム終了確認画面を開く
    /// </summary>
    private void OpenExitConfirm()
    {
        Debug.Log("aaaa");
        PlayDecisionSound();
        ShowUI(3);
        SelectButton(3);    // はい

        text.text = "ゲームを終了しますか？";

        delete = false;
        isConfFlg = true;
    }

    /// <summary>
    /// 確認画面で「はい」を押したとき
    /// </summary>
    private void ConfirmYes()
    {
        PlayDecisionSound();

        if (delete)
        {
            PlayerPrefs.DeleteAll();        // セーブデータ削除
            deleteSaveDada.Delete();
        }
        else
        {
            Invoke(nameof(EndGame), 1.0f);  // ゲーム終了
        }
    }

    /// <summary>
    /// 確認画面で「いいえ」を押したとき
    /// </summary>
    private void ConfirmNo()
    {
        PlayDecisionSound();
        HideUI(3);

        isConfFlg = false;

        if (delete)
        {
            ShowUI(1);          // 設定画面へ戻る
            SelectButton(2);
        }
        else
        {
            SelectButton(0);    // タイトル画面へ戻る
        }
    }

    /// <summary>
    /// 「初めから・続きから」画面を閉じる
    /// </summary>
    private void CloseGameStart()
    {
        PlayDecisionSound();
        HideUI(0);        // ゲーム開始選択画面を非表示
        ShowUI(4);        // タイトル画面を表示
        SelectButton(0);  // Startボタンを選択

        isStartFlg = false;
        isTitleFlg = true;
    }

    /// <summary>
    /// 指定したUIを表示
    /// </summary>
    private void ShowUI(int index)
    {
        canvasGroup[index].alpha = 1f;
    }

    /// <summary>
    /// 指定したUIを非表示
    /// </summary>
    private void HideUI(int index)
    {
        canvasGroup[index].alpha = 0f;
    }

    /// <summary>
    /// 指定したボタンを選択
    /// </summary>
    private void SelectButton(int index)
    {
        button[index].Select();
    }

    /// <summary>
    /// 決定音を鳴らす
    /// </summary>
    private void PlayDecisionSound()
    {
        soundPush.DecisionPush();
    }

    // 緊急用　LTボタンで反応
    public void OnLT(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        int escapeButtonIndex = GetEscapeButtonIndex();

        if (escapeButtonIndex < 0)
            return;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(
            escapeButton[escapeButtonIndex]
        );
    }

    /// <summary>
    /// 現在の画面に対応する緊急ボタン番号を取得
    /// </summary>
    private int GetEscapeButtonIndex()
    {
        // 確認画面
        if (isConfFlg)
            return 4;

        // 音量設定画面
        if (isVolumeFlg)
            return 3;

        // 設定画面
        if (isOptionFlg)
            return 2;

        // 初めから・続きから画面
        if (isStartFlg)
            return 1;

        // タイトル画面
        if (isTitleFlg)
            return 0;

        return -1;
    }

    private void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}