using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIExtensions;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 武器合成結果画面を管理するクラス。
/// 
/// 主な処理：
/// ・合成した武器の情報を表示
/// ・武器の登場アニメーションを再生
/// ・アニメーション終了後にパーティクルを再生
/// ・装備・キャンセルボタンを表示
/// ・選択中のボタンのハイライトを更新
/// ・プレイヤーの選択結果を通知
/// </summary>
public class WeaponSynthesisResultView : MonoBehaviour
{
    // ==============================
    // 演出関連
    // ==============================

    [SerializeField] private AudioClip _sound;                  // 武器合成成功時の効果音
    [SerializeField] private InventoryWeponDataView _weaponView; // 合成した武器の情報表示
    [SerializeField] private Animator _weaponAnimator;          // 武器の登場アニメーション
    [SerializeField] private UIParticle _particle;              // アニメーション終了後のパーティクル

    // ==============================
    // ボタン関連
    // ==============================

    [SerializeField] private GameObject _buttonsRoot;           // ボタン全体の親オブジェクト
    [SerializeField] private UnityEngine.UI.Button _equipButton; // 装備ボタン
    [SerializeField] private UnityEngine.UI.Button _cancelButton;// キャンセルボタン

    [SerializeField] private GameObject _equipHighlightImage;   // 装備ボタンの選択ハイライト
    [SerializeField] private GameObject _cancelHighlightImage;  // キャンセルボタンの選択ハイライト

    // ==============================
    // イベント・公開プロパティ
    // ==============================

    /// <summary>
    /// プレイヤーが装備またはキャンセルを選択した際に発行するイベント。
    /// true：装備
    /// false：キャンセル
    /// </summary>
    public event Action<bool> ChoiceSubmitted;

    /// <summary>合成結果画面が開いているかどうか。</summary>
    public bool IsOpen { get; private set; }

    /// <summary>演出が終了し、ボタン操作が可能になっているかどうか。</summary>
    public bool IsReady { get; private set; }

    /// <summary>
    /// 最初に選択するUI要素を取得する。
    /// 操作可能になるまではnullを返す。
    /// </summary>
    public UnityEngine.UI.Selectable FirstSelectable => IsReady ? _equipButton : null;

    // ==============================
    // 内部管理
    // ==============================

    private Coroutine _presentation;               // 演出進行を管理するコルーチン
    private int _readyFrame;                        // ボタン操作が可能になったフレーム
    private CanvasGroup _equipCanvasGroup;          // 装備ボタンの透明度制御
    private CanvasGroup _cancelCanvasGroup;         // キャンセルボタンの透明度制御

    /// <summary>
    /// 初期化処理。
    /// ボタンの表示設定とクリックイベントの登録を行う。
    /// </summary>
    private void Awake()
    {
        // 各ボタンのCanvasGroupを取得
        _equipCanvasGroup = _equipButton.GetComponent<CanvasGroup>();
        _cancelCanvasGroup = _cancelButton.GetComponent<CanvasGroup>();

        // Unity標準のボタン遷移を無効化
        // ハイライトと透明度を独自に制御するため
        _equipButton.transition = UnityEngine.UI.Selectable.Transition.None;
        _cancelButton.transition = UnityEngine.UI.Selectable.Transition.None;

        // ボタンの初期表示を更新
        UpdateButtonVisuals();

        // ボタンクリック時のイベントを登録
        _equipButton.onClick.AddListener(Equip);
        _cancelButton.onClick.AddListener(Cancel);
    }

    /// <summary>
    /// 毎フレーム、ボタンの選択状態に合わせて表示を更新する。
    /// </summary>
    private void LateUpdate()
    {
        UpdateButtonVisuals();
    }

    /// <summary>
    /// 現在選択されているボタンに応じて、
    /// 透明度とハイライトの表示状態を更新する。
    /// </summary>
    private void UpdateButtonVisuals()
    {
        // 結果画面が開いており、操作可能な場合のみ選択状態を取得
        var selected = IsOpen && IsReady && EventSystem.current != null
            ? EventSystem.current.currentSelectedGameObject
            : null;

        // 各ボタンの表示を更新
        SetButtonVisual(
            _equipButton,
            _equipCanvasGroup,
            _equipHighlightImage,
            selected
        );

        SetButtonVisual(
            _cancelButton,
            _cancelCanvasGroup,
            _cancelHighlightImage,
            selected
        );
    }

    /// <summary>
    /// 指定したボタンの選択状態に応じて表示を変更する。
    /// 選択中：透明度100%、ハイライト表示
    /// 非選択：透明度50%、ハイライト非表示
    /// </summary>
    /// <param name="button">表示を更新するボタン</param>
    /// <param name="group">透明度を制御するCanvasGroup</param>
    /// <param name="highlight">選択時に表示するハイライト</param>
    /// <param name="selected">現在選択されているGameObject</param>
    private static void SetButtonVisual(
        UnityEngine.UI.Button button,
        CanvasGroup group,
        GameObject highlight,
        GameObject selected)
    {
        // ボタンが操作可能で、現在選択されているか判定
        bool focused = button != null &&
                       button.IsInteractable() &&
                       selected == button.gameObject;

        // 選択状態に応じて透明度を変更
        if (group != null)
            group.alpha = focused ? 1f : 0.5f;

        // 選択中のボタンのみハイライトを表示
        if (highlight != null && highlight.activeSelf != focused)
            highlight.SetActive(focused);
    }

    /// <summary>
    /// オブジェクト破棄時にボタンイベントを解除する。
    /// </summary>
    private void OnDestroy()
    {
        // 登録したクリックイベントを解除
        if (_equipButton != null)
            _equipButton.onClick.RemoveListener(Equip);

        if (_cancelButton != null)
            _cancelButton.onClick.RemoveListener(Cancel);
    }

    /// <summary>
    /// 武器合成結果画面を表示する。
    /// 武器情報を設定し、登場アニメーションと効果音を再生する。
    /// </summary>
    /// <param name="weapon">合成によって完成した武器</param>
    public void Show(WeaponInstance weapon)
    {
        // 前回の演出状態を初期化
        Hide();

        // 結果画面を表示
        gameObject.SetActive(true);
        IsOpen = true;

        // ==============================
        // 武器情報の設定
        // ==============================

        // 武器に設定されている付呪情報を取得
        var texts = new List<string>();

        // 付呪スロット3つ分の表示文字列を生成
        for (int i = 0; i < 3; i++)
        {
            texts.Add(EnchantSelectionPanel.FormatEnchantment(
                weapon.enchantmentSlots != null &&
                i < weapon.enchantmentSlots.Length
                    ? weapon.enchantmentSlots[i]
                    : null
            ));
        }

        // 武器表示の選択状態をリセット
        _weaponView.ResetSelection();

        // 武器アイコンと付呪情報を設定
        _weaponView.SetUpData(weapon.Icon, texts);

        // 結果画面では武器自体を選択できないようにする
        if (_weaponView.Selectable != null)
            _weaponView.Selectable.interactable = false;

        // ==============================
        // 武器登場アニメーション
        // ==============================

        if (_weaponAnimator != null &&
            _weaponAnimator.runtimeAnimatorController != null)
        {
            // Time.timeScaleが0でもアニメーションを再生できるようにする
            _weaponAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;

            // カメラの描画範囲外でもアニメーションを更新する
            _weaponAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

            // Animatorの状態を初期化
            _weaponAnimator.Rebind();

            // 最初のステートを先頭から再生
            _weaponAnimator.Play(0, 0, 0f);

            // Animatorの状態を即座に反映
            _weaponAnimator.Update(0f);
        }

        // アニメーション終了を待機するコルーチンを開始
        _presentation = StartCoroutine(WaitForWeaponAnimation());

        // 武器合成成功時の効果音を再生
        SoundManager.Instance.PlaySE_Story(_sound);
    }

    /// <summary>
    /// 武器の登場アニメーションが終了するまで待機する。
    /// 終了後、パーティクルを再生してボタン操作を有効化する。
    /// </summary>
    private IEnumerator WaitForWeaponAnimation()
    {
        // 表示したフレームの決定入力が
        // 結果画面のボタン操作に引き継がれないようにする
        yield return null;

        // ==============================
        // アニメーション終了待機
        // ==============================

        if (_weaponAnimator != null &&
            _weaponAnimator.runtimeAnimatorController != null)
        {
            // ステート遷移中、またはアニメーションが
            // 1周終了するまで待機
            while (_weaponAnimator.IsInTransition(0) ||
                   _weaponAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
                yield return null;
            }
        }

        // ==============================
        // パーティクル再生
        // ==============================

        if (_particle != null)
        {
            // パーティクルオブジェクトを有効化
            _particle.gameObject.SetActive(true);

            // 前回のパーティクルを削除
            _particle.Clear();

            // パーティクルを再生
            _particle.Play();
        }

        // ==============================
        // ボタン操作の有効化
        // ==============================

        // 装備・キャンセルボタンを操作可能にする
        _equipButton.interactable = true;
        _cancelButton.interactable = true;

        // ボタン全体を表示
        _buttonsRoot.SetActive(true);

        // 操作可能になったフレームを記録
        _readyFrame = Time.frameCount;

        // 演出終了状態に変更
        IsReady = true;

        // ボタンの選択表示を更新
        UpdateButtonVisuals();

        // コルーチンの管理参照を解除
        _presentation = null;
    }

    /// <summary>
    /// 装備ボタンが押された際の処理。
    /// </summary>
    private void Equip()
    {
        Submit(true);
    }

    /// <summary>
    /// キャンセルボタンが押された際の処理。
    /// </summary>
    public void Cancel()
    {
        Submit(false);
    }

    /// <summary>
    /// プレイヤーの選択結果を通知する。
    /// 演出中や操作可能になった直後のフレームでは処理しない。
    /// </summary>
    /// <param name="equip">true：装備、false：キャンセル</param>
    private void Submit(bool equip)
    {
        // 画面が閉じている場合は処理しない
        // 演出中の場合は処理しない
        // 操作可能になったフレームでは入力を受け付けない
        if (!IsOpen || !IsReady || Time.frameCount <= _readyFrame)
            return;

        // 選択結果を外部へ通知
        ChoiceSubmitted?.Invoke(equip);
    }

    /// <summary>
    /// 武器合成結果画面を閉じる。
    /// 演出状態を初期化してオブジェクトを非表示にする。
    /// </summary>
    public void Hide()
    {
        // 再生中の演出を停止し、状態を初期化
        ClearPresentation();

        // 結果画面を非表示
        gameObject.SetActive(false);
    }

    /// <summary>
    /// オブジェクトが無効化された際に演出状態を初期化する。
    /// </summary>
    private void OnDisable()
    {
        ClearPresentation();
    }

    /// <summary>
    /// 演出を停止し、画面の状態を初期化する。
    /// コルーチン・ボタン・パーティクルの状態をリセットする。
    /// </summary>
    private void ClearPresentation()
    {
        // ==============================
        // コルーチン停止
        // ==============================

        // 演出中の場合はコルーチンを停止
        if (_presentation != null)
            StopCoroutine(_presentation);

        _presentation = null;

        // ==============================
        // 画面状態の初期化
        // ==============================

        // 画面を閉じた状態に変更
        IsOpen = false;

        // ボタン操作を無効な状態に変更
        IsReady = false;

        // ボタンのハイライトを更新
        UpdateButtonVisuals();

        // ボタン全体を非表示
        if (_buttonsRoot != null)
            _buttonsRoot.SetActive(false);

        // ==============================
        // パーティクル停止
        // ==============================

        if (_particle != null)
        {
            // パーティクルの再生を停止
            _particle.Stop();

            // 残っているパーティクルを削除
            _particle.Clear();

            // パーティクルオブジェクトを非表示
            _particle.gameObject.SetActive(false);
        }
    }
}