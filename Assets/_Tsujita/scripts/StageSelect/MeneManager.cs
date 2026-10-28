using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MeneManager : MonoBehaviour
{
    [SerializeField] private GameObject startPanel;     // 初期パネル
    [SerializeField] private GameObject stagePanel;     // ステージセレクトパネル
    [SerializeField] private GameObject weponPanel;     // 武器変更パネル
    [SerializeField] private GameObject synthesisPanel; // 合成用パネル

    [SerializeField] private GameObject homeFirstButton;        // 初期パネルの初期ボタン
    [SerializeField] private GameObject stageSelectFirstButton; // ステージセレクトの初期ボタン

    [SerializeField] private GameObject fadeObd;    // フェード用オブジェクト

    [SerializeField] private WeaponInventory inventory;
    [SerializeField] private SoundPush soundPush;
    [SerializeField] private CursorSE cursorSE;

    [SerializeField] private Sprite spriteStage5;
    [SerializeField] private Image imageStage5;

    public bool _panelFlg;
    private bool _stageFlg;

    public void Awake()
    {
        inventory.LoadInventory();
    }

    public void Start()
    {
        SoundManager.Instance.PlayBGM("SelectBGM");
        fadeObd.SetActive(true);
        _panelFlg = true;
        ShowHome();
        ChangeStageImage();
    }

    /// <summary>
    /// 初期設定パネルの表示
    /// </summary>
    public void ShowHome()
    {
        if (weponPanel != null) weponPanel.SetActive(false);
        if (synthesisPanel != null) synthesisPanel.SetActive(false);
        startPanel.SetActive(true);
        stagePanel.SetActive(false);
        _panelFlg = true;
        _stageFlg = false;
        soundPush.CancelPush();
        //cursorSE.IgnoreNextSelection();
        StartCoroutine(SelectAfterPanelSwitch(homeFirstButton));
    }

    /// <summary>
    /// ステージ選択パネル表示
    /// </summary>
    public void ShowStageSelect()
    {
        startPanel.SetActive(false);
        stagePanel.SetActive(true);
        _panelFlg = false;
        _stageFlg = true;
        soundPush.DecisionPush();
        //cursorSE.IgnoreNextSelection();
        StartCoroutine(SelectAfterPanelSwitch(stageSelectFirstButton));
    }

    /// <summary>
    /// 武器変更パネル表示
    /// </summary>
    public void ShowWepon() 
    {
        _stageFlg = false;
        if (synthesisPanel != null) synthesisPanel.SetActive(false);
        startPanel.SetActive(false);
        weponPanel.SetActive(true);
        _panelFlg = false;
        //cursorSE.IgnoreNextSelection();
        soundPush.DecisionPush();
    }

    /// <summary>
    /// 装備合成用パネル表示
    /// </summary>
    public void ShowSynthesis()
    {
        if (weponPanel != null) weponPanel.SetActive(false);
        _stageFlg = false;
        startPanel.SetActive(false);
        synthesisPanel.SetActive(true);
        _panelFlg = false;
        //soundPush.DecisionPush();
    }

    // 緊急
    public void OnLT(InputAction.CallbackContext context)
    {
        if(_panelFlg)
        {
            if (!context.performed) return;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(homeFirstButton);
        }
        else if(_stageFlg)
        {
            if (!context.performed) return;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(stageSelectFirstButton);
        }
    }

    // タイトルに戻る
    public void TitleChenge()
    {
        soundPush.DecisionPush();
        FadeSceneChanger.ChangeScene("TitleScene");
    }

    private void ChangeStageImage()
    {
        if(HigeScoreLoad.instans._isStage5Flg == true)
        {
            imageStage5.sprite = spriteStage5;
        }
    }

    private IEnumerator SelectAfterPanelSwitch(GameObject target)
    {
        yield return new WaitForEndOfFrame();

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(target);
    }
}
