using UnityEngine;

public class CharaChoice : MonoBehaviour
{
    [SerializeField] private string sceneName;


    // 仮置き
    
    /// <summary>
    /// キャラ選択
    /// </summary>
    public void OnCharactor(int buttonNo)
    {
        if(buttonNo == 0)
        {
            Debug.Log("リーゼロッテモード");
        }
        else if(buttonNo == 1)
        {
            Debug.Log("ヴェルミリオンモード");
        }

        FadeSceneChanger.ChangeScene(sceneName);
    }
}
