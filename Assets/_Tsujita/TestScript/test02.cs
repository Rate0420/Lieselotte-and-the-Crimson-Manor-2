using System;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class test02 : MonoBehaviour
{


    public int hp;
    public int parry;
    public int combo;
    public float time;
    public int stageNo;

    private void Awake()
    {

        //hp =  GameSceneData.instance.gsdHPDeta;
        //combo =  GameSceneData.instance.gsdComboDeta;
        //parry = GameSceneData.instance.gsdParryDeta;
        //time = GameSceneData.instance.gsdTimeDeta;
        //stageNo = ScoreData.instance.stageNoData;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GameSceneData.instance.gsdHPDeta = hp;
            GameSceneData.instance.gsdComboDeta = combo;
            GameSceneData.instance.gsdParryDeta = parry;
            GameSceneData.instance.gsdTimeDeta = time;

            SceneManager.LoadScene("ResultScene");
        }
    }
}
