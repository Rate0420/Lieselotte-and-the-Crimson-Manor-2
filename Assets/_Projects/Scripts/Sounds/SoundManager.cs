using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System.Security.Cryptography;


public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource bgmAudioSource; // BGM用のAudioSource
    [SerializeField] AudioSource seAudioSource;  // SE（効果音）用のAudioSource
    [SerializeField] AudioSource voiceAudioSource;  // ボイス用のAudioSource
    [SerializeField] AudioMixerGroup mixerGroup; // 出力先のAudioMixer

    [SerializeField] List<BGMSoundData> bgmSoundDatas; // BGMのデータ一覧
    [SerializeField] List<SESoundData> seSoundDatas;   // SEのデータ一覧
    [SerializeField] List<VoiceSoundData> voiceSoundDatas;   // ボイスのデータ一覧
    [SerializeField] List<VoiceSoundData> gameOverSoundDatas;   // ゲームオーバーのボイスデータ一覧


    public float masterVolume = 1;     // 全体の音量
    public float bgmMasterVolume = 1;  // BGMのマスター音量
    public float seMasterVolume = 1;   // SEのマスター音量
    public float voiceMasterVolume = 1;// ボイスのマスター音量

    int maxSeCount = 10; // 同時に鳴らせるSEの最大数
    //List<GameObject> seGameObjects = new(); // 再生中のSE用GameObjectのリスト

    public static SoundManager Instance { get; private set; } // シングルトンインスタンス
    private string currentBGM = null; // 現在再生中のBGM名

    /// <summary>
    /// シングルトン処理
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // シーンをまたいでも破棄されない
        }
        else
        {
            Destroy(gameObject);
        }
    }


    /// <summary>
    /// BGMを再生する関数
    /// </summary>
    /// <param name="bgmName"></param>
    public void PlayBGM(string bgmName)
    {
        StopBGM();
        // すでに同じBGMが再生中なら処理しない
        if (currentBGM != null && currentBGM.ToString() == bgmName) return;

        // 指定名のBGMデータを探す
        BGMSoundData data = bgmSoundDatas.Find(data => data.bgmName == bgmName);
        if (data == null)
        {
            Debug.LogWarning($"BGM '{bgmName}' が見つかりませんでした．");
            return;
        }

        // AudioClipと音量を設定して再生
        bgmAudioSource.clip = data.audioClip;
        bgmAudioSource.volume = data.volume * bgmMasterVolume * masterVolume;
        bgmAudioSource.Play();

        currentBGM = bgmName;  // 現在のBGM名を記録
    }

    public void RestartBGM()
    {
        if (currentBGM != null)
        {
            StopBGM();
            PlayBGM(currentBGM);
        }
    }

    public void StopBGM()
    {
        bgmAudioSource.Stop();
        currentBGM = null;
    }

    /// <summary>
    /// 効果音を再生する関数
    /// </summary>
    /// <param name="seName"></param>
    public void PlaySE(string seName)
    {
        // nullになったGameObjectを削除（破棄されたSEの後処理）
        //seGameObjects.RemoveAll(x => x == null);

        // 同時に鳴らせるSEの上限チェック
        //if (seGameObjects.Count > maxSeCount) return;

        // SEデータを検索
        SESoundData data = seSoundDatas.Find(d => d.seName == seName);
        if (data == null)
        {
            Debug.LogWarning($"SE '{seName}' が見つかりませんでした．");
            return;
        }

        // 一時的なGameObjectを生成し，そこにAudioSourceをアタッチ
        GameObject tempGO = new GameObject("TempSE_" + data.audioClip.name);
        AudioSource tempSource = tempGO.AddComponent<AudioSource>();
        tempSource.outputAudioMixerGroup = mixerGroup;

        // SE再生用GameObjectを管理リストに追加し，破棄されないように設定
        //seGameObjects.Add(tempGO);
        DontDestroyOnLoad(tempGO);

        // SEのAudioClipと音量を設定して再生
        tempSource.clip = data.audioClip;
        tempSource.volume = data.volume * seMasterVolume * masterVolume;
        tempSource.Play();

        // 再生終了後に自動でGameObjectを破棄
        Destroy(tempGO, data.audioClip.length);
    }

    public void PlayBGM_Story(AudioClip clip)
    {
        // すでに同じBGMが再生中なら処理しない
        if (bgmAudioSource.clip == clip) return;
        bgmAudioSource.clip = clip;
        bgmAudioSource.volume = bgmMasterVolume * masterVolume;
        bgmAudioSource.Play();
    }

    public void PlaySE_Story(AudioClip clip)
    {
        seAudioSource.clip = clip;
        seAudioSource.volume = seMasterVolume * masterVolume;
        seAudioSource.Play();
    }

    public void PlayVoice_Story(AudioClip clip)
    {
        voiceAudioSource.clip = clip;
        voiceAudioSource.volume = voiceMasterVolume * masterVolume;
        voiceAudioSource.Play();
    }

    /// <summary>
    /// 被弾時に呼び出す。VoiceSoundDataの中からランダムに1つ選んで再生する。
    /// </summary>
    public void DamageVoice()
    {
        if (voiceSoundDatas.Count == 0)
        {
            Debug.LogWarning("再生可能なボイスがありません。");
            return;
        }
        int randomIndex = Random.Range(0, voiceSoundDatas.Count);
        VoiceSoundData data = voiceSoundDatas[randomIndex];
        voiceAudioSource.clip = data.audioClip;
        voiceAudioSource.volume = data.volume * voiceMasterVolume * masterVolume;
        voiceAudioSource.Play();
    }

    public void GameOverVoice()
    {
        if (gameOverSoundDatas.Count == 0)
        {
            Debug.LogWarning("再生可能なゲームオーバーボイスがありません。");
            return;
        }
        int randomIndex = Random.Range(0, gameOverSoundDatas.Count);
        VoiceSoundData data = gameOverSoundDatas[randomIndex];
        voiceAudioSource.clip = data.audioClip;
        voiceAudioSource.volume = data.volume * voiceMasterVolume * masterVolume;
        voiceAudioSource.Play();
    }

    /// <summary>
    /// BGM音量設定
    /// </summary>
    /// <param name="value"></param>
    public void SetBGMVolume(float value)
    {
        bgmMasterVolume = value;

        if(bgmAudioSource.clip != null)
            bgmAudioSource.volume = bgmMasterVolume;
    }

    /// <summary>
    /// SE音量設定
    /// </summary>
    /// <param name="value"></param>
    public void SetSEVolume(float value)
    {
        seMasterVolume = value;
    }

    /// <summary>
    /// Voice音量設定
    /// </summary>
    /// <param name="value"></param>
    public void SetVoiceVolume(float value)
    {
        voiceMasterVolume = value;

        if (voiceAudioSource.clip != null)
            voiceAudioSource.volume = voiceMasterVolume;
    }
}

[System.Serializable]
public class BGMSoundData
{
    public string bgmName;          // BGMの識別名
    public AudioClip audioClip;     // BGMの実データ
    [Range(0, 1)]
    public float volume = 1;        // 個別音量（0〜1）
}

[System.Serializable]
public class SESoundData
{
    public string seName;           // SEの識別名
    public AudioClip audioClip;     // SEの実データ
    [Range(0, 1)]
    public float volume = 1;        // 個別音量（0〜1）
}

[System.Serializable]
public class VoiceSoundData
{
    public string voiceName;        // ボイスの識別名
    public AudioClip audioClip;     // ボイスの実データ
    [Range(0, 1)]
    public float volume = 1;        // 個別音量（0〜1）
}
