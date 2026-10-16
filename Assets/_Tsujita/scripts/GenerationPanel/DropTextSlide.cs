using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropTextSlide : MonoBehaviour
{
    [SerializeField] private Panel_Ani panelAni;

    // 武器排出、フレーム
    [SerializeField] private TextMeshProUGUI weaponDropText;    // 武器排出テキスト
    [SerializeField] private Image dropFrameImage;              // 武器フレーム画像

    // 武器画像、武器名
    [SerializeField] private Image weaponImage;                 // 武器画像
    [SerializeField] private TextMeshProUGUI weaponNameText;    // 武器名テキスト

    // 付呪表示
    [SerializeField] private TextMeshProUGUI[] enchantTexts;    // 付呪テキスト

    [SerializeField] private float duration = 0.3f;         // 拡大時間
    [SerializeField] private float standbyTime;             // 拡大待機時間
    [SerializeField] private GameObject enchantListPanel;   // 付呪一覧パネル

    private float startScale = 0.0f;    // 初期サイズ
    private bool _enchantListFlgOK;       // 付呪一覧パネル表示許可
    private bool _enchantListFlg = false;       // 付呪一覧パネル切り替え

    // 最終サイズの保存用
    private Vector3 weaponDropTextEnd;
    private Vector3 dropFrameEnd;
    private Vector3 weaponImageEnd;
    private Vector3 weaponNameEnd;
    private Vector3[] enchantEndScales;

    private void Awake()
    {
        weaponDropTextEnd = weaponDropText.rectTransform.localScale;
        dropFrameEnd = dropFrameImage.rectTransform.localScale;
        weaponImageEnd = weaponImage.rectTransform.localScale;
        weaponNameEnd = weaponNameText.rectTransform.localScale;

        enchantEndScales = new Vector3[enchantTexts.Length];
        for (int i = 0; i < enchantTexts.Length; i++)
        {
            enchantEndScales[i] = enchantTexts[i].rectTransform.localScale;
        }

        SetAllScaleZero();
    }

    private void Update()
    {
        if (_enchantListFlgOK)
        {
            if(Input.GetKeyDown(KeyCode.X))
            {
                if(!_enchantListFlg)
                {
                    _enchantListFlg = true;
                    panelAni.OpenPanel();
                }
                else
                {
                    _enchantListFlg = false;
                    panelAni.ClosePanel();
                }
            }
        }
    }

    /// <summary>
    /// 武器排出アニメーション開始
    /// </summary>
    public void DropSlideStart()
    {
        StopAllCoroutines();
        StartCoroutine(ShowSequence());
    }

    /// <summary>
    /// 最終サイズ保存
    /// </summary>
    private void SetAllScaleZero()
    {
        weaponDropText.rectTransform.localScale = Vector3.zero;
        dropFrameImage.rectTransform.localScale = Vector3.zero;
        weaponImage.rectTransform.localScale = Vector3.zero;
        weaponNameText.rectTransform.localScale = Vector3.zero;

        foreach (var txt in enchantTexts)
        {
            txt.rectTransform.localScale = Vector3.zero;
        }
    }

    private IEnumerator ShowSequence()
    {
        yield return StartCoroutine(ScaleDropStartUI());
        yield return new WaitForSeconds(standbyTime);
        yield return StartCoroutine(ScaleWeaponUI());
        yield return new WaitForSeconds(standbyTime);
        yield return StartCoroutine(ScaleEnchantUI());
        yield return new WaitForSeconds(standbyTime);
        ResultManager.nextSceneFlg = true;
        _enchantListFlgOK = true;
    }

    /// <summary>
    /// 武器排出、フレーム
    /// </summary>
    private IEnumerator ScaleDropStartUI()
    {
        Debug.Log("a");
        float time = 0f;

        Vector3 textStart = weaponDropTextEnd * startScale;
        Vector3 frameStart = dropFrameEnd * startScale;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            weaponDropText.rectTransform.localScale = Vector3.Lerp(textStart, weaponDropTextEnd, t);
            dropFrameImage.rectTransform.localScale =　Vector3.Lerp(frameStart, dropFrameEnd, t);

            yield return null;
        }

        weaponDropText.rectTransform.localScale = weaponDropTextEnd;
        dropFrameImage.rectTransform.localScale = dropFrameEnd;
    }

    /// <summary>
    /// 武器画像、武器名
    /// </summary>
    private IEnumerator ScaleWeaponUI()
    {
        float time = 0f;

        Vector3 imageStart = weaponImageEnd * startScale;
        Vector3 nameStart = weaponNameEnd * startScale;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            weaponImage.rectTransform.localScale = Vector3.Lerp(imageStart, weaponImageEnd, t);

            weaponNameText.rectTransform.localScale = Vector3.Lerp(nameStart, weaponNameEnd, t);

            yield return null;
        }

        weaponImage.rectTransform.localScale = weaponImageEnd;
        weaponNameText.rectTransform.localScale = weaponNameEnd;
    }

    /// <summary>
    /// 付呪表示
    /// </summary>
    private IEnumerator ScaleEnchantUI()
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            for (int i = 0; i < enchantTexts.Length; i++)
            {
                enchantTexts[i].rectTransform.localScale = Vector3.Lerp(enchantEndScales[i] * startScale, enchantEndScales[i], t);
            }

            yield return null;
        }

        for (int i = 0; i < enchantTexts.Length; i++)
        {
            enchantTexts[i].rectTransform.localScale = enchantEndScales[i];
        }
    }
}
