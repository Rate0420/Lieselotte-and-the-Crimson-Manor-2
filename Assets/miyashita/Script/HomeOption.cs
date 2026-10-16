using UnityEngine;
using UnityEngine.EventSystems;

public class HomeOption : MonoBehaviour
{

    public GameObject[] HomeSelect;
    public GameObject Home;
    public GameObject StageFirstBotton;


    public void OnClick(int num)
    {
        switch (num) 
        {
            case 1:
                HomeSelect[0].SetActive(true);
                Home.SetActive(false);
                //初期選択ボタンの初期化
                EventSystem.current.SetSelectedGameObject(null);
                //初期選択ボタンの再指定
                EventSystem.current.SetSelectedGameObject(StageFirstBotton);
                break;
            case 2:
                Debug.Log("2");
                HomeSelect[1].SetActive(true);
                Home.SetActive(false);
                break;
            case 3:
                Debug.Log("3");
                HomeSelect[2].SetActive(true);
                Home.SetActive(false);
                break;
        }

    }
   
}
