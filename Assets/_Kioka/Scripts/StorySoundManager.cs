using UnityEngine;

public class StorySoundManager : MonoBehaviour
{
    [SerializeField] private StoryData storyData;

    [SerializeField] private AudioSource[] audioSource;
    [SerializeField] private AudioClip[] bgmClip;
    [SerializeField] private AudioClip[] seClip;
    [SerializeField] private AudioClip[] voiceClip;

    void Start()
    {
        bgmClip = storyData.bgm;
        seClip = storyData.se;
        voiceClip = storyData.voice;
    }

    void Update()
    {
        // É^ÉCÉgÉãÇ≈ê›íËÇµÇΩâπó Ç…í≤êﬂ
        //audioSource[0].volume = SliderController.bgmValue / 100;    // BGM
        //audioSource[1].volume = SliderController.seValue / 100;     // SE
        //audioSource[2].volume = SliderController.voiceValue / 100;  // Voice
    }

    /// <summary>
    /// BGMçƒê∂
    /// </summary>
    public void PlayBGM(int numer)
    {
        audioSource[0].clip = bgmClip[numer];
        audioSource[0].Play();
    }

    /// <summary>
    /// SEçƒê∂
    /// </summary>
    public void PlaySE(int number)
    {
        audioSource[1].PlayOneShot(seClip[number]);
    }

    /// <summary>
    /// Voiceçƒê∂
    /// </summary>
    public void PlayVoice(int number)
    {
        audioSource[2].PlayOneShot(voiceClip[number]);
    }
}
