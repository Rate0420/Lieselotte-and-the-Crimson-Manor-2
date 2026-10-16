using UnityEngine;

public class HigeScoreLoad : MonoBehaviour
{
    public static HigeScoreLoad instans;

    [SerializeField, Header("ステージ数")] private int stageCount;

    private int[] higeScoreStage;

    public bool _isStage5Flg;


    private void Awake()
    {
        if(instans == null)
        {
            instans = this;
            DontDestroyOnLoad(gameObject);
            higeScoreStage = new int[stageCount];
        }
        else
            Destroy(gameObject);
    }




    /// <summary>
    /// ハイスコア受け取り用
    /// </summary>
    public void LoadScore(int score)
    {
        for (int i = 1; i <= stageCount; i++)
        {
            //score = HigeScoreLoad.instans.higeScoreStage[i];
            score = PlayerPrefs.GetInt("HighScore" + i, 0);
        }
    }
}
