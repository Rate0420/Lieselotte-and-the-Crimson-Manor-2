using UnityEngine;

public class GameSceneData : MonoBehaviour
{
    public static GameSceneData instance;

    public int gsdHPDeta;       // 残HP
    public int gsdComboDeta;    // 最大コンボ
    public int gsdParryDeta;    // 合計パリィ数
    public float gsdTimeDeta;   // 経過時間

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
