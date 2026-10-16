using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeSceneChanger : MonoBehaviour
{
    // シーン遷移の際にフェードイン・フェードアウトを行うクラス
    // 他クラスからシーン名を指定して呼び出すと、フェードアウトしてからシーン遷移し、遷移後にフェードインする

    [SerializeField] float fadeDuration = 1f; // フェードの時間
    [SerializeField] float waitDuration = 0.5f; // フェードアウトとフェードインの間の待機時間
    [SerializeField] Image fadeImage; // 黒一色の画像。これのalphaを操作してフェードイン・フェードアウトを実現する

    public static FadeSceneChanger Instance { get; private set; }
    public bool isFading; // フェード中かどうか
    private void Awake()
    {
        // シングルトンパターンの実装
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーン遷移してもこのオブジェクトを破壊しない
        }
        else
        {
            Destroy(gameObject); // 既にインスタンスが存在する場合はこのオブジェクトを破壊
        }
    }

    /// <summary>
    /// 他クラスから引数にシーン名を指定して呼び出すと、フェードアウトしてからシーン遷移し、遷移後にフェードインする
    /// </summary>
    public static void ChangeScene(string sceneName)
    {
        if (Instance != null)
        {
            Instance.StartCoroutine(Instance.FadeAndChangeScene(sceneName));
        }
    }

    IEnumerator FadeAndChangeScene(string sceneName)
    {
        isFading = true;
        while(fadeImage.color.a < 1f)
        {
            Color color = fadeImage.color;
            color.a += Time.deltaTime / fadeDuration;
            fadeImage.color = color;
            yield return null;
        }
        yield return null;
        yield return new WaitForSeconds(waitDuration);
        // シーン遷移し、遷移後にフェードインする処理
        // 非同期でシーン移動
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // シーン移動が完了するまで待機
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        while (fadeImage.color.a > 0f)
        {
            Color color = fadeImage.color;
            color.a -= Time.deltaTime / fadeDuration;
            fadeImage.color = color;
            yield return null;
        }
        yield return null;
        isFading = false;
    }

    public static void WaitAndFade(float time, string scenename)
    {
        // 引数に時間を指定して呼び出すと、その時間待機してからフェードアウトする
        Instance.StartCoroutine(Instance.WaitAndFadeCoroutine(time, scenename));
    }

    IEnumerator WaitAndFadeCoroutine(float time,string scenename)
    {
        yield return new WaitForSeconds(time);
        StartCoroutine(FadeAndChangeScene(scenename));// 現在のシーン名を引数にしてフェードアウトする
    }
}
