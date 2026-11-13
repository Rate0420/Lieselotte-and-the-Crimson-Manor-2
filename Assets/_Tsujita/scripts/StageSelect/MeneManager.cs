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
    [SerializeField] private GameObject supportsPanel;   // サポートキャラクター変更パネル

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
    private GameObject _homeReturnButton;
    private Coroutine _pendingPanelFocus;

    public void Awake()
    {

    }

    public void Start()
    {
        inventory = WeaponInventory.Instance;
        inventory.LoadInventory();
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
        ClearPanelFocus();
        if (weponPanel != null) weponPanel.SetActive(false);
        if (synthesisPanel != null) synthesisPanel.SetActive(false);
        startPanel.SetActive(true);
        stagePanel.SetActive(false);
        _panelFlg = true;
        _stageFlg = false;
        soundPush.CancelPush();
        //cursorSE.IgnoreNextSelection();
        _pendingPanelFocus = StartCoroutine(SelectAfterPanelSwitch(GetHomeFocus()));
    }

    /// <summary>
    /// ステージ選択パネル表示
    /// </summary>
    public void ShowStageSelect()
    {
        RememberHomeFocus();
        ClearPanelFocus();
        startPanel.SetActive(false);
        stagePanel.SetActive(true);
        _panelFlg = false;
        _stageFlg = true;
        soundPush.DecisionPush();
        //cursorSE.IgnoreNextSelection();
        _pendingPanelFocus = StartCoroutine(SelectAfterPanelSwitch(stageSelectFirstButton));
    }

    /// <summary>
    /// 武器変更パネル表示
    /// </summary>
    public void ShowWepon() 
    {
        RememberHomeFocus();
        ClearPanelFocus();
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
        RememberHomeFocus();
        ClearPanelFocus();
        if (weponPanel != null) weponPanel.SetActive(false);
        _stageFlg = false;
        startPanel.SetActive(false);
        synthesisPanel.SetActive(true);
        _panelFlg = false;
        //soundPush.DecisionPush();
    }

    /// <summary>
    /// サポートキャラクター変更パネル表示
    /// </summary>
    public void ShowSupports()
    {
        RememberHomeFocus();
        ClearPanelFocus();
        _stageFlg = false;
        startPanel.SetActive(false);
        supportsPanel.SetActive(true);
        _panelFlg = false;
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

    private void RememberHomeFocus()
    {
        if (!startPanel.activeInHierarchy) return;

        var events = EventSystem.current;
        var selected = events != null ? events.currentSelectedGameObject : null;
        _homeReturnButton = IsAvailableHomeButton(selected) ? selected : null;
    }

    private GameObject GetHomeFocus()
    {
        return IsAvailableHomeButton(_homeReturnButton) ? _homeReturnButton : homeFirstButton;
    }

    private bool IsAvailableHomeButton(GameObject target)
    {
        return target != null && target.activeInHierarchy && target.transform.IsChildOf(startPanel.transform) &&
            target.TryGetComponent<UnityEngine.UI.Button>(out var button) && button.isActiveAndEnabled && button.IsInteractable();
    }

    private void ClearPanelFocus()
    {
        if (_pendingPanelFocus != null)
        {
            StopCoroutine(_pendingPanelFocus);
            _pendingPanelFocus = null;
        }

        // パネルを隠す前に選択を解除し、ボタンの選択色を元に戻す。
        var events = EventSystem.current;
        if (events != null) events.SetSelectedGameObject(null);
    }

    private IEnumerator SelectAfterPanelSwitch(GameObject target)
    {
        yield return new WaitForEndOfFrame();

        _pendingPanelFocus = null;
        var events = EventSystem.current;
        if (events != null && target != null && target.activeInHierarchy)
            events.SetSelectedGameObject(target);
    }
}
