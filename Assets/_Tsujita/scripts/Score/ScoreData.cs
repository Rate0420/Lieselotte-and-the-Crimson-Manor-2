using UnityEngine;

public class ScoreData : MonoBehaviour
{
    public static ScoreData instance;

    public int totalHPDate;
    public int totalComboDate;
    public int totalParryDate;
    public int totalTimeDate;
    public int totalScoreDate;
    public int higeScoreDate;
    public int rankNoDate;
    public int stageNoData;

    private int stageNo;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StageNoUp()
    {
        ScoreData.instance.stageNoData = stageNo;
    }
}
