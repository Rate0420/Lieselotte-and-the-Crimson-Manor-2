//using System;
//using Unity.Burst.Intrinsics;
//using UnityEditor.Build.Content;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class test02 : MonoBehaviour
//{


//    public int hp;
//    public int parry;
//    public int combo;
//    public int time;
//    public int stageNo;

//    private void Awake()
//    {
//        GameSceneData.instance.gsdHPDeta = hp;
//        GameSceneData.instance.gsdComboDeta = combo;
//        GameSceneData.instance.gsdParryDeta = parry;
//        GameSceneData.instance.gsdTimeDeta = time;
//        ScoreData.instance.stageNoData = stageNo;

//        hp = GameSceneData.instance.gsdHPDeta;
//        combo = GameSceneData.instance.gsdComboDeta;
//        parry = GameSceneData.instance.gsdParryDeta;
//        time = GameSceneData.instance.gsdTimeDeta;
//        stageNo = ScoreData.instance.stageNoData;
//    }

//    private void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.Return))
//        {
//            SceneManager.LoadScene("ResultScene");
//        }
//    }
//}
