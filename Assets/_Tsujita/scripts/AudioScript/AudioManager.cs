using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip decisionSound;     // 決定音
    [SerializeField] private AudioClip cursorSound;       // カーソル音
    [SerializeField] private AudioClip backSound;         // キャンセル音

    private float EffectVolume; // 効果音の音量
    private AudioSource audioSource;
    
    private void Awake()
    {
        // 音量設定の受け取り
        EffectVolume = 1.0f; // 仮

        audioSource = GetComponent<AudioSource>();

    }

    /// <summary>
    /// 決定音
    /// </summary>
    public void DecisionSound()
    {
        audioSource.PlayOneShot(decisionSound);
    }

    /// <summary>
    /// カーソル音
    /// </summary>
    public void CursorSound()
    {
        audioSource.PlayOneShot(cursorSound);
    }

    /// <summary>
    /// キャンセル音
    /// </summary>
    public void BackSound()
    {
        audioSource.PlayOneShot(backSound);
    }
}
