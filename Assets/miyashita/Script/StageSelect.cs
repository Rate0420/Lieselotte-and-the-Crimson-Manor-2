using Unity.Multiplayer.Center.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class StageSelect : MonoBehaviour
{
    //ステージをクリアしたかどうか
    public static bool Stage1Clearflag = false;
    public static bool Stage2Clearflag = false;
    public static bool Stage3Clearflag = false;
    public static bool Stage4Clearflag = false;
    public static bool Stage5Clearflag = false;
    //選択時に表示するパネル
    public GameObject[] StagePnanel;
    public GameObject[] Stage2Panel;
    public GameObject[] Stage3Panel;
    public GameObject[] Stage4Panel;
    public GameObject[] Stage5Panel; 
    public GameObject[] Stage6Panel;




    private void Start()
    {
     
    }

    //初期化
    void StagePnanelReset()
    {
        StagePnanel[0].SetActive(false);
        StagePnanel[1].SetActive(false);
        StagePnanel[2].SetActive(false);
        StagePnanel[3].SetActive(false);
        StagePnanel[4].SetActive(false);
        StagePnanel[5].SetActive(false);
        Stage2Panel[0].SetActive(false);
        Stage2Panel[1].SetActive(false);
        Stage3Panel[0].SetActive(false);
        Stage3Panel[1].SetActive(false);
        Stage4Panel[0].SetActive(false);
        Stage4Panel[1].SetActive(false);
        Stage5Panel[0].SetActive(false);
        Stage5Panel[1].SetActive(false);
        Stage6Panel[0].SetActive(false);
        Stage6Panel[1].SetActive(false);

    }

    

    
    void Update()
    {
        
        // EventSystem を取得
        var eventSystem = EventSystem.current;
        // 現在選択されている GameObject を取得
        GameObject selectedGameObject = eventSystem.currentSelectedGameObject;
        // 選択されている GameObject がボタンかどうかを確認
        if (selectedGameObject != null && selectedGameObject.GetComponent<UnityEngine.UI.Button>() != null)
        {
            //選択状態のボタンが指定のボタンの時
            //選択されているステージが１の時
            if (selectedGameObject.name == "stage1")
            {
                StagePnanelReset();
                StagePnanel[0].SetActive(true);
            }
            //選択されているステージが２の時
            else if (selectedGameObject.name == "stage2")
            {
                StagePnanelReset();
                StagePnanel[1].SetActive(true);
                //ステージ１をクリアしていない場合ステージ２をロック
                if (Stage1Clearflag == false)
                {Stage2Panel[0].SetActive(true);}
                //ステージ１をクリアしていたらステージ２を解放
                else{Stage2Panel[1].SetActive(true);}
               
            }
            //選択されているステージが３の時
            else if (selectedGameObject.name == "stage3")
            {
                StagePnanelReset();
                StagePnanel[2].SetActive(true);
                //ステージ2をクリアしていない場合ステージ3をロック
                if (Stage2Clearflag == false)
                {Stage3Panel[0].SetActive(true);}
                //ステージ2をクリアしていたらステージ3を解放
                else{Stage3Panel[1].SetActive(true);}
            }
            //選択されているステージが４の時
            else if (selectedGameObject.name == "stage4")
            {
                StagePnanelReset();
                StagePnanel[3].SetActive(true);

                //ステージ3をクリアしていない場合ステージ4をロック
                if (Stage3Clearflag == false)
                {Stage4Panel[0].SetActive(true);}
                //ステージ3をクリアしていたらステージ4を解放
                else {Stage4Panel[1].SetActive(true);}

            }
            //選択されているステージが５の時
            else if (selectedGameObject.name == "stage5")
            {
                StagePnanelReset();
                StagePnanel[4].SetActive(true);
                //ステージ4をクリアしていない場合ステージ5をロック
                if (Stage4Clearflag == false)
                { Stage5Panel[0].SetActive(true); }
                //ステージ4をクリアしていたらステージ5を解放
                else { Stage5Panel[1].SetActive(true); }
            }
            //選択されているステージが６の時
            else if (selectedGameObject.name == "stage6")
            {
                StagePnanelReset();
                StagePnanel[5].SetActive(true);
                //ステージ5をクリアしていない場合ステージ6をロック
                if (Stage5Clearflag == false)
                { Stage6Panel[0].SetActive(true); }
                //ステージ5をクリアしていたらステージ6を解放
                else { Stage6Panel[1].SetActive(true); }
            }
        }
            




    }
}
