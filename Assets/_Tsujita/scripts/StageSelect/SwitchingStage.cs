using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchingStage : MonoBehaviour
{
    [SerializeField] private string[] introduction; // ステージ紹介
    [SerializeField] private string[] enemyName;    // 敵の名前

    [SerializeField] private TextMeshProUGUI introductionText;  // ステージ紹介用テキスト
    [SerializeField] private TextMeshProUGUI enemyNameText;     // 敵の名前用テキスト

    [SerializeField] private GameObject loadPanel;  // スライド用テキスト

    [SerializeField] private FadeController fadeController;

    [SerializeField] private string[] stageSceneNames;  // 移動先のシーン名

    [SerializeField] private StoryData[] storyStageSceneNames;  // ストーリー用のシーン名

    [SerializeField] private float loadTime;    // 待機時間

    [SerializeField] private SoundPush soundPush;

    /// <summary>
    /// ボタンから番号を受け取る
    /// </summary>
    public void StageChange(int stageNo)
    {
        if (stageNo < 0 || stageNo >= stageSceneNames.Length)
        {
            Debug.LogError("ステージ番号が不正です: " + stageNo);
            return;
        }

        ScoreData.instance.stageNoData = stageNo;
        soundPush.DecisionPush();
        introductionText.text = introduction[stageNo];
        enemyNameText.text = enemyName[stageNo];

        // そのステージがクリアされていれば普通のシーンに、されていなければストーリー用のシーンに遷移する
        if (PlayerPrefs.GetInt("HigeScore" + stageNo) != 0)
        {
            StartCoroutine(Fade(stageSceneNames[stageNo]));
        }
        else
        {
            Debug.Log(PlayerPrefs.GetInt("HigeScore" + stageNo));
            // dontdestroyStoryのインスタンスにストーリー用のシーン名を渡す
            DontDestroyStory.instance.story = storyStageSceneNames[stageNo];
            StartCoroutine(Fade("StoryScene_SakaguchiTest"));
        }
    }

    // 暗転開始
    private IEnumerator Fade(string stageName)
    {
        yield return StartCoroutine(fadeController.FadeInFromRight());

        // 暗転が終わったら一時停止
        yield return new WaitForSeconds(loadTime);


         AsyncOperation async = SceneManager.LoadSceneAsync(stageName);
        async.allowSceneActivation = false;

        while (async.progress < 0.9f)
        {
            yield return null;
        }

        // シーン切り替え
        async.allowSceneActivation = true;
    }
}
