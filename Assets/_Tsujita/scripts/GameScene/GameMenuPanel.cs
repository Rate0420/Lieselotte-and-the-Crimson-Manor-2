using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class GameMenuPanel : MonoBehaviour
{
    [SerializeField] private GameObject[] explanationButton; // メニューパネル内のボタン
    [SerializeField] private GameObject[] explanationPanel;  // メニューパネル内のパネル
    [SerializeField] private GameObject[] endButton;         // ゲーム終了確認ボタン
    [SerializeField] private GameObject[] fastButton;        // 初期ボタン

    [SerializeField] private GameObject menuPanel;  // メニューパネル

    [SerializeField] private float animationTime = 0.2f; // アニメーション時間
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private SoundPush soundPush;
    [SerializeField] private CursorSE cursorSE;

    private int selectNo;
    private bool isControllerFlg = false;

    private void Awake()
    {
        StartChange();
        menuPanel.SetActive(false);
    }

    public void StartChange()
    {
        for (int i = 0; i < explanationButton.Length; i++)
            explanationPanel[i].SetActive(false);
    }

    // ボタンを押したときに呼び出す
    public void PanelChange(int index)
    {
        cursorSE.IgnoreNextSelection();
        soundPush.DecisionPush();
        isControllerFlg = true;
        for (int i = 0; i < explanationButton.Length; i++)
        {
            if (i == index)
            {
                selectNo = i;
                explanationPanel[i].SetActive(true);
                explanationButton[i].SetActive(true);
                StartCoroutine(OpenAnimation(index));
            }
        }
        StartCoroutine(SelectFirstButton(index));
    }

    // 選択中のパネルの非表示
    public void CloseAllPanel(int index)
    {
        cursorSE.IgnoreNextSelection();
        soundPush.CancelPush();
        isControllerFlg = false;
        StartCoroutine(CloseAnimation(index));
        StartCoroutine(SelectFirstButton(selectNo));
    }

    // メニューパネルの表示
    public void OpenMenu()
    {
        cursorSE.IgnoreNextSelection();
        soundPush.DecisionPush();
        menuPanel.SetActive(true);
        Time.timeScale = 0f;
        playerInput.SwitchCurrentActionMap("UI");
        StartCoroutine(OpenAnimation(0));
        StartCoroutine(SelectFirstButton(selectNo));
    }

    // メニューパネルの非表示
    public void BuckMenu()
    {
        cursorSE.IgnoreNextSelection();
        soundPush.CancelPush();
        playerInput.SwitchCurrentActionMap("Player");
        StartCoroutine(CloseMenuAnimation());
    }

    // あきらめる
    public void EndScnenChange()
    {
        cursorSE.IgnoreNextSelection();
        soundPush.DecisionPush();
        Time.timeScale = 1.0f;
        FadeSceneChanger.ChangeScene("SelectScene");
    }

    public void OnLT()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(fastButton[selectNo]);
    }

    private IEnumerator SelectFirstButton(int i)
    {
        yield return new WaitForEndOfFrame();

        yield return new WaitForEndOfFrame();

        EventSystem.current.SetSelectedGameObject(null);

        if (!isControllerFlg)
            explanationButton[i].GetComponent<Button>().Select();
        else
            fastButton[i].GetComponent<Button>().Select();
    }

    private IEnumerator OpenAnimation(int i)
    {
        float time = 0;

        while (time < animationTime)
        {
            float scale = Mathf.Lerp(0f, 1f, time / animationTime);
            explanationPanel[i].transform.localScale = new Vector3(scale, scale, scale);

            time += Time.unscaledDeltaTime;
            yield return null;
        }

        explanationPanel[i].transform.localScale = Vector3.one;
    }

    // 詳細パネルの非表示
    private IEnumerator CloseAnimation(int index)
    {
        float time = 0;

        while (time < animationTime)
        {
            float scale = Mathf.Lerp(1f, 0f, time / animationTime);
            explanationPanel[index].transform.localScale = new Vector3(scale, scale, scale);

            time += Time.unscaledDeltaTime;
            yield return null;
        }
        explanationPanel[index].SetActive(false);
    }

    private IEnumerator CloseMenuAnimation()
    {
        yield return StartCoroutine(CloseAnimation(0));

        menuPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
