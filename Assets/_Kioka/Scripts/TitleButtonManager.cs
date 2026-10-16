using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TitleButtonManager : MonoBehaviour
{
    [SerializeField] private Button[] button;            // ボタン配列
    [SerializeField] private GameObject[] ui;            // UI配列
    [SerializeField] private CanvasGroup[] canvasGroup;  // キャンバスグループ配列
    [SerializeField] private Slider bgmSlider;           // BGMスライダー
    [SerializeField] private TextMeshProUGUI text;       // テキスト

    [SerializeField] private StoryData storyData;
    [SerializeField] private string[] scene;

    private string scene2;

    [SerializeField] private DeleteSaveDada deleteSaveDada;
    [SerializeField] private SoundPush soundPush;

    bool delete;  // セーブデータ削除かゲーム終了かを判定

    // 緊急時の選択中ボタン変更フラグ
    // 設定、確認
    private bool isTitleFlg = false;    // 初期画面
    private bool isStartFlg = false;    // 始めから、続きから
    private bool isOptionFlg = false;   // 設定画面
    private bool isVolumeFlg = false;   // 音量設定
    private bool isConfFlg = false;     // 確認画面

    [SerializeField] private GameObject[] escapeButton;   // 緊急ボタン

    private void Start()
    {
        SoundManager.Instance.PlayBGM("TitleBGM");  // タイトル用BGM

        for (int i = 0; i < canvasGroup.Length; i++)
        {
            canvasGroup[i] = ui[i].GetComponent<CanvasGroup>();  // パネルのキャンバスグループを取得
            canvasGroup[i].alpha = 0;  // パネルを非表示にする
        }
        canvasGroup[4].alpha = 1;
        scene2 = storyData.sceneName[0];
        button[0].Select();  // Startボタンを選択中にする

        isTitleFlg = true;
    }

    /// <summary>
    /// ボタンを押したときの処理
    /// </summary>
    /// <param name="number"></param>
    public void OnClickButton(int number)
    {
        // 押されたボタンによって処理を実行する
        switch (number)
        {
            case 0:  // ゲーム開始ボタン　 スタートパネル
                soundPush.DecisionPush();
                canvasGroup[4].alpha = 0f;  // タイトルボタン非表示
                canvasGroup[0].alpha = 1f;  // 初めから、続きからボタン表示
                button[1].Select();  　　   // 初めからボタンを選択中にする
                isStartFlg = true;
                break;
            case 1:  // 初めからボタン
                soundPush.DecisionPush();
                PlayerPrefs.DeleteAll();  // 全てのキーとデータを削除
                deleteSaveDada.Delete();
                Debug.Log("aaaaaaaaaaaaaaaaaaa");
                DontDestroyStory.instance.story = storyData;  // ストーリーシーンの名前を変更
                FadeSceneChanger.ChangeScene("StoryScene_SakaguchiTest");  // ストーリーシーンに遷移
                break;
            case 2:  // 続きからボタン
                soundPush.DecisionPush();
                FadeSceneChanger.ChangeScene(scene[2]);  // 選択シーンに遷移
                break;
            case 3:  // 設定ボタン
                soundPush.DecisionPush();
                canvasGroup[1].alpha = 1f;  // 設定パネル表示
                button[2].Select();         // 音量設定ボタンを選択中にする
                isOptionFlg = true;
                isTitleFlg = false;
                break;
            case 4:  // 音量設定ボタン
                soundPush.DecisionPush();
                canvasGroup[1].alpha = 0f;  // 設定パネル非表示
                canvasGroup[2].alpha = 1f;  // 音量設定パネル表示
                bgmSlider.Select();         // BGMスライダーを選択中にする
                isVolumeFlg = true;
                break;
            case 5:  // 戻るボタン（音量設定画面）
                soundPush.DecisionPush();
                canvasGroup[1].alpha = 1f;  // 設定パネル表示
                canvasGroup[2].alpha = 0f;  // 音量設定パネル非表示
                button[2].Select();         // 音量設定ボタンを選択中にする
                isVolumeFlg = false;
                // スライダーの数値の保存
                break;
            case 6:  // セーブデータ削除ボタン
                soundPush.DecisionPush();
                canvasGroup[1].alpha = 0f;  // 設定パネル非表示
                canvasGroup[3].alpha = 1f;  // 確認パネル表示
                button[3].Select();         // はいボタンを選択中にする
                isConfFlg = true;
                text.text = "セーブデータを削除しますか？";
                delete = true;
                break;
            case 7:  // 戻るボタン（設定画面）
                soundPush.DecisionPush();
                canvasGroup[1].alpha = 0f;  // 設定パネル非表示
                button[0].Select();         // Startボタンを選択中にする
                isOptionFlg = false;
                isTitleFlg = true;
                break;
            case 8:  // ゲーム終了ボタン
                soundPush.DecisionPush();
                canvasGroup[3].alpha = 1f;  // 確認パネル表示
                button[3].Select();         // はいボタンを選択中にする
                text.text = "ゲームを終了しますか？";
                isConfFlg = true;
                delete = false;
                break;
            case 9:  // はいボタン（確認画面）
                soundPush.DecisionPush();
                // セーブデータ削除、ゲーム終了
                if (delete)
                {
                    PlayerPrefs.DeleteAll();  // 全てのキーとデータを削除
                    deleteSaveDada.Delete();
                }
                else
                {
                    Invoke(nameof(EndGame), 1.0f);
                }
                break;
            case 10:  // いいえボタン（確認画面）
                soundPush.DecisionPush();
                if (delete)
                {
                    canvasGroup[1].alpha = 1f;  // 設定パネル表示
                    canvasGroup[3].alpha = 0f;  // 確認パネル非表示
                    button[2].Select();         // 音量設定ボタンを選択中にする
                    isConfFlg = false;
                }
                else
                {
                    canvasGroup[3].alpha = 0f;  // 確認パネル非表示
                    button[0].Select();         // Startボタンを選択中にする
                    isConfFlg = false;
                }
                break;
            case 11:  // 戻るボタン（初めから、続きからボタンの下）
                soundPush.DecisionPush();
                canvasGroup[4].alpha = 1f;  // タイトルボタン表示
                canvasGroup[0].alpha = 0f;  // 初めから、続きからボタン非表示
                button[0].Select();         // Startボタンを選択中にする
                isStartFlg = false;
                break;
        }
    }

    public void OnLT(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Debug.Log("0");
        int buttonIndex = -1;

        if ((isOptionFlg && isConfFlg) || (isTitleFlg && isConfFlg)) // データ削除確認画面
            buttonIndex = 4;
        else if (isOptionFlg && isVolumeFlg)    // 音量設定画面時
            buttonIndex = 3;
        else if (!isTitleFlg && isOptionFlg)     // 設定画面
            buttonIndex = 2;
        else if (isTitleFlg && isStartFlg)       // スタート画面
            buttonIndex = 1;
        else if (isTitleFlg && !isStartFlg)      // 初期画面時
            buttonIndex = 0;

        if (buttonIndex < 0) return;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(escapeButton[buttonIndex]);

    }

    private void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;  //ゲームプレイ終了
#else
    Application.Quit();  //ゲームプレイ終了
#endif
    }
}
