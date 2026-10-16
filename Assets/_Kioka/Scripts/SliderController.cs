using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderController : MonoBehaviour
{
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider seSlider;
    [SerializeField] Slider voiceSlider;

    [SerializeField] TextMeshProUGUI bgmValueText;
    [SerializeField] TextMeshProUGUI seValueText;
    [SerializeField] TextMeshProUGUI voiceValueText;

    private void Start()
    {
        // 現在の音量を取得してスライダーに反映
        bgmSlider.value = SoundManager.Instance.bgmMasterVolume * 10f;
        seSlider.value = SoundManager.Instance.seMasterVolume * 10f;
        voiceSlider.value = SoundManager.Instance.voiceMasterVolume * 10f;

        // テキスト更新
        ChangeBGM(bgmSlider.value);
        ChangeSE(seSlider.value);
        ChangeVoice(voiceSlider.value);

        // イベント登録
        bgmSlider.onValueChanged.AddListener(ChangeBGM);
        seSlider.onValueChanged.AddListener(ChangeSE);
        voiceSlider.onValueChanged.AddListener(ChangeVoice);
    }

    /// <summary>
    /// BGMの音量を変更
    /// </summary>
    /// <param name="value"></param>
    public void ChangeBGM(float value)
    {
        bgmValueText.text = value.ToString("F0");
        SoundManager.Instance.SetBGMVolume(bgmSlider.value / 10f);
    }

    /// <summary>
    /// SEの音量を変更
    /// </summary>
    /// <param name="value"></param>
    public void ChangeSE(float value)
    {
        seValueText.text = value.ToString("F0");
        SoundManager.Instance.SetSEVolume(seSlider.value / 10f);
    }

    /// <summary>
    /// Voiceの音量を変更
    /// </summary>
    /// <param name="value"></param>
    public void ChangeVoice(float value)
    {
        voiceValueText.text = value.ToString("F0");
        SoundManager.Instance.SetVoiceVolume(voiceSlider.value / 10f);
    }
}
