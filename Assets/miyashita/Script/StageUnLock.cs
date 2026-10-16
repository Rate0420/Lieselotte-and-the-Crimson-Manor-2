using UnityEngine;
using UnityEngine.SceneManagement;

public class StageUnLock : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       

    }
    public void Clear1()
    {
        StageSelect.Stage1Clearflag = true;
        SceneManager.LoadScene("SomaScene");
    }
    public void Clear2()
    {
        StageSelect.Stage2Clearflag = true;
        SceneManager.LoadScene("SomaScene");
    }
    public void Clear3()
    {
        StageSelect.Stage3Clearflag = true;
        SceneManager.LoadScene("SomaScene");
    }
    public void Clear4()
    {
        StageSelect.Stage4Clearflag = true;
        SceneManager.LoadScene("SomaScene");
    }
    public void Clear5()
    {
        StageSelect.Stage5Clearflag = true;
        SceneManager.LoadScene("SomaScene");
    }

}
