using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class MenuPanel : MonoBehaviour
{
    [SerializeField] private GameObject[] explanationButton; // メニューパネル内のボタン
    [SerializeField] private GameObject[] explanationPanel;  // メニューパネル内のパネル
    [SerializeField] private GameObject[] endButton;         // ゲーム終了確認ボタン

    [SerializeField] private GameObject menuPanel;  // メニューパネル

    [SerializeField] private float animationTime = 0.2f; // アニメーション時間
    [SerializeField] private PlayerInput playerInput;

    private int selectNo;
    private bool isPanelFlg;

    private void Awake()
    {
        StartChange();
        isPanelFlg = false;
        menuPanel.SetActive(false);
    }

    public void StartChange()
    {
        for(int i = 0; i < explanationButton.Length; i++)
            explanationPanel[i].SetActive(false);
    }

    // ボタンを押したときに呼び出す
    public void PanelChange(int index)
    {
        for(int i = 0; i < explanationButton.Length; i++)
        {
            if(i == index)
            {
                explanationPanel[i].SetActive(true);
                explanationButton[i].SetActive(true);
                StartCoroutine(OpenAnimation(i));
                selectNo = i;

                if(i == 5)
                    endButton[0].GetComponent<Button>().Select();
                else
                    EventSystem.current.SetSelectedGameObject(null);
            }
            else
            {
                explanationPanel[i].SetActive(false);
                explanationButton[i].SetActive(false);
            }
        }
    }

    // 選択中のパネルの非表示
    public void CloseAllPanel()
    {
        for (int i = 0; i < explanationButton.Length; i++)
        {
            StartCoroutine(CloseAnimation(i));
            StartCoroutine(SelectFirstButton(selectNo));
        }
    }

    // メニューパネルの表示
    public void OpenMenu()
    {
        if (FadeSceneChanger.Instance.isFading) return;
        Time.timeScale = 0f;
        playerInput.SwitchCurrentActionMap("UI");
        menuPanel.SetActive(true);
        StartCoroutine(OpenAnimation(7));
        StartCoroutine(SelectFirstButton(0));
        isPanelFlg = true;
    }

    // メニューパネルの非表示
    public void BuckMenu()
    {
        isPanelFlg = false;
        playerInput.SwitchCurrentActionMap("Player");
        StartCoroutine(CloseMenuAnimation());
    }

    // チュートリアルのリタイア
    public void EndScnenChange()
    {
        Debug.Log("aaa");
        Time.timeScale = 1f;
        FadeSceneChanger.ChangeScene("SelectScene");
    }

    private IEnumerator SelectFirstButton(int i)
    {
        yield return new WaitForEndOfFrame();

        yield return new WaitForEndOfFrame();

        explanationButton[i].GetComponent<Button>().Select();
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

    private IEnumerator CloseAnimation(int i)
    {
        float time = 0;

        while (time < animationTime)
        {
            float scale = Mathf.Lerp(1f, 0f, time / animationTime);
            explanationPanel[i].transform.localScale = new Vector3(scale, scale, scale);

            time += Time.unscaledDeltaTime;
            yield return null;
        }

        if(isPanelFlg)
        {
            explanationPanel[i].SetActive(false);
            explanationButton[i].SetActive(true);
        }
    }

    private IEnumerator CloseMenuAnimation()
    {
        yield return StartCoroutine(CloseAnimation(7));

        menuPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
