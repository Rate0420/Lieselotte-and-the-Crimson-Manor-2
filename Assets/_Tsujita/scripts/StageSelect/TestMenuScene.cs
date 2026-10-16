using UnityEngine;
using UnityEngine.SceneManagement;

public class TestMenuScene : MonoBehaviour
{
    [SerializeField] int stageNo = 3;

    private void Awake()
    {
        //stageNo = PlayerPrefs.GetInt("stageNo", stageNo);
        PlayerPrefs.SetInt("stageNo", stageNo);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < stageNo; i++)
            {
                StageClearCheck.SetStageClear(i);
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
            SceneManager.LoadScene("SelectScene");
    }
}