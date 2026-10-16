using UnityEngine;

public class StageClearCheck : MonoBehaviour
{
    /// <summary>
    /// ステージ解放
    /// </summary>
    public static void SetStageClear(int stageNo)
    {
        PlayerPrefs.SetInt($"StageClear_{stageNo}", 1);
        PlayerPrefs.SetInt($"StageClear_{stageNo + 1}", 1);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// ステージが解放されているか
    /// </summary>
    public static bool IsStageUnlocked(int stageNo)
    {
        return PlayerPrefs.GetInt($"StageClear_{stageNo}", 0) == 1;
    }

    /// <summary>
    /// 最初のステージだけ解放
    /// </summary>
    public static void Init()
    {
        if (!PlayerPrefs.HasKey("StageClear_1"))
        {
            PlayerPrefs.SetInt("StageClear_1", 1);
            PlayerPrefs.Save();
        }
    }
}
