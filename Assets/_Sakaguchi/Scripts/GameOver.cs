using System.Collections;
using TMPro;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    // ゲームオーバー画面の表示、ゲームオーバー時の三つのボタンを管理するクラス
    // 三つのボタンは、リトライ、拠点に戻る、タイトルに戻るで、どれもゲームオーバー画面が表示され切った時に有効になり、コントローラーで選択。

    [SerializeField] Image GameOverPanel;
    [SerializeField] Image GameOverImage;
    [SerializeField] TextMeshProUGUI GameOverText;
    [SerializeField] float FadeDuration = 1f;
    [SerializeField] GameObject RetryButton;
    [SerializeField] GameObject ReturnToBaseButton;
    [SerializeField] GameObject ReturnToTitleButton;
    [SerializeField] int selectedButtonIndex = 0; // 0: Retry, 1: Return to Base, 2: Return to Title
    bool isGameOver = false;
    bool isChoosed = false;
    Color defaultColor;

    [SerializeField] float moveInterval = 0.2f;
    float moveTimer = 0f;



    void Update()
    {
        if (isGameOver)
        {
            moveTimer -= Time.unscaledDeltaTime;
            HandleInput();
        }
        
        // デバッグ用: Gキーを押すとゲームオーバーを表示
        if (Input.GetKeyDown(KeyCode.G))
        {
            ShowGameOver();
        }
    }


    public void ShowGameOver()
    {
        StartCoroutine(GameOverCoroutine());
    }

    /// <summary>
    /// ゲームオーバーのフェードイン処理を行うコルーチン
    /// </summary>
    /// <returns></returns>
    IEnumerator GameOverCoroutine()
    {
        defaultColor = RetryButton.GetComponent<Image>().color; // デフォルトの色を保存
        SoundManager.Instance.GameOverVoice(); // ゲームオーバーのボイスを再生

        float elapsedTime = 0f;
        Color panelColor = GameOverPanel.color;
        Color imageColor = GameOverImage.color;
        Color textColor = GameOverText.color;

        // フェードイン処理
        while (elapsedTime < FadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / FadeDuration);
            GameOverPanel.color = new Color(panelColor.r, panelColor.g, panelColor.b, alpha);
            GameOverImage.color = new Color(imageColor.r, imageColor.g, imageColor.b, alpha);
            GameOverText.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 最終的に完全に表示されるようにする
        GameOverPanel.color = new Color(panelColor.r, panelColor.g, panelColor.b, 1f);
        GameOverImage.color = new Color(imageColor.r, imageColor.g, imageColor.b, 1f);
        GameOverText.color = new Color(textColor.r, textColor.g, textColor.b, 1f);

        // ボタンを有効にする
        RetryButton.SetActive(true);
        ReturnToBaseButton.SetActive(true);
        ReturnToTitleButton.SetActive(true);

        // ボタンもフェードインさせる
        Image retryButtonImage = RetryButton.GetComponent<Image>();
        Image returnToBaseButtonImage = ReturnToBaseButton.GetComponent<Image>();
        Image returnToTitleButtonImage = ReturnToTitleButton.GetComponent<Image>();
        elapsedTime = 0f;
        while (elapsedTime < FadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / FadeDuration);
            retryButtonImage.color = new Color(retryButtonImage.color.r, retryButtonImage.color.g, retryButtonImage.color.b, alpha);
            returnToBaseButtonImage.color = new Color(returnToBaseButtonImage.color.r, returnToBaseButtonImage.color.g, returnToBaseButtonImage.color.b, alpha);
            returnToTitleButtonImage.color = new Color(returnToTitleButtonImage.color.r, returnToTitleButtonImage.color.g, returnToTitleButtonImage.color.b, alpha);
            // ボタンの子オブジェクトのテキストもフェードインさせる
            TextMeshProUGUI retryButtonText = RetryButton.GetComponentInChildren<TextMeshProUGUI>();
            TextMeshProUGUI returnToBaseButtonText = ReturnToBaseButton.GetComponentInChildren<TextMeshProUGUI>();
            TextMeshProUGUI returnToTitleButtonText = ReturnToTitleButton.GetComponentInChildren<TextMeshProUGUI>();
            retryButtonText.color = new Color(retryButtonText.color.r, retryButtonText.color.g, retryButtonText.color.b, alpha);
            returnToBaseButtonText.color = new Color(returnToBaseButtonText.color.r, returnToBaseButtonText.color.g, returnToBaseButtonText.color.b, alpha);
            returnToTitleButtonText.color = new Color(returnToTitleButtonText.color.r, returnToTitleButtonText.color.g, returnToTitleButtonText.color.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 最初の選択を更新(最初の選択肢をハイライトする)
        UpdateButtonSelection();

        if (!isGameOver)
        {
            isGameOver = true;
        }
    }

    // ここからコントローラー操作

    void HandleInput()
    {
        // 上下の入力を処理して選択を変更
        if (!isGameOver) return;
        else
        {
            if (moveTimer > 0f) return; // 入力間隔がまだ残っている場合は処理しない

            float verticalInput = Input.GetAxis("Vertical");
            if (verticalInput > 0.1f) // 上方向の入力
            {
                moveTimer = moveInterval;
                selectedButtonIndex = (selectedButtonIndex - 1 + 3) % 3; // 0, 1, 2の範囲で循環
                UpdateButtonSelection();
            }
            else if (verticalInput < -0.1f) // 下方向の入力
            {

                moveTimer = moveInterval;
                selectedButtonIndex = (selectedButtonIndex + 1) % 3; // 0, 1, 2の範囲で循環
                UpdateButtonSelection();
            }
        }
    }

    void UpdateButtonSelection()
    {
        // 選択されているボタンの色を変更して視覚的にわかりやすくする
        Color selectedColor = Color.yellow; // 選択されたボタンの色
        RetryButton.GetComponent<Image>().color = (selectedButtonIndex == 0) ? selectedColor : defaultColor;
        ReturnToBaseButton.GetComponent<Image>().color = (selectedButtonIndex == 1) ? selectedColor : defaultColor;
        ReturnToTitleButton.GetComponent<Image>().color = (selectedButtonIndex == 2) ? selectedColor : defaultColor;
    }

    public void Cont_Select(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isGameOver && !isChoosed)
            { 
                switch(selectedButtonIndex)
                {
                    case 0: // Retry
                        Debug.Log("Retry selected");
                        // 現在再生中のシーンを再読み込みする
                        FadeSceneChanger.ChangeScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                        isChoosed = true; // 選択されたことを記録して、以降の入力を無視する
                        break;
                    case 1: // Return to Base
                        Debug.Log("Return to Base selected");
                        FadeSceneChanger.ChangeScene("SelectScene");
                        isChoosed = true; // 選択されたことを記録して、以降の入力を無視する
                        break;
                    case 2: // Return to Title
                        Debug.Log("Return to Title selected");
                        FadeSceneChanger.ChangeScene("TitleScene");
                        isChoosed = true; // 選択されたことを記録して、以降の入力を無視する
                        break;
                }
            }
        }
    }
}
