using UnityEngine;

public class BGMStarter : MonoBehaviour
{

    // シーンに配置してSoundManagerがBGMを再生するようにするスクリプト
    [SerializeField] string bgmName; // 再生するBGMの名前
    void Start()
    {
        SoundManager.Instance.PlayBGM(bgmName);
    }
}
