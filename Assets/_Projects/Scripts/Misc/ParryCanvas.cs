using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ParryCanvas : MonoBehaviour
{
    // シングルトン処理
    [SerializeField] Image Image;
    [SerializeField] Sprite[] parryDirSprites;
    float rotate;
    float rotateEndTime = 0.05f;

    private static ParryCanvas instance;
    public static ParryCanvas Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ParryCanvas>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("ParryCanvas");
                    instance = obj.AddComponent<ParryCanvas>();
                }
            }
            return instance;
        }
    }

    public void ShowDirection(int a)
    { 
        if(Image!=null)
            //Image.sprite = parryDirSprites[a];

            // 数字に応じてImageを回転させる(z)
            // 右が-90。
            // case 4の場合はどこも刺さないため、x軸を90にする。
            // 4から他に移動する場合は、x軸を0に戻す。

            //rotateEndTimeかけて回転させる。 

            switch (a)
            {
                case 0:
                    StartCoroutine(RotateImage(0, 0));
                    break;
                case 1:
                    StartCoroutine(RotateImage(-180, 0));
                    break;
                case 2:
                    StartCoroutine(RotateImage(-270, 0));
                    break;
                case 3:
                    StartCoroutine(RotateImage(-90, 0));
                    break;
                case 4:
                    StartCoroutine(RotateImage(0, 90));
                    break;
            }





        //switch (a)
        //{
        //    case 0:
        //        Image.transform.rotation = Quaternion.Euler(0, 0, 0);
        //        break;
        //    case 1:
        //        Image.transform.rotation = Quaternion.Euler(0, 0, -180);
        //        break;
        //    case 2:
        //        Image.transform.rotation = Quaternion.Euler(0, 0, -270);
        //        break;
        //    case 3:
        //        Image.transform.rotation = Quaternion.Euler(0, 0, -90);
        //        break;
        //}
    }

    IEnumerator RotateImage(float targetZ,float targetX)
    {
        float elapsedTime = 0f;
        Quaternion startRotation = Image.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(targetX, 0, targetZ);
        while (elapsedTime < rotateEndTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / rotateEndTime);
            Image.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null;
        }
        Image.transform.rotation = targetRotation;
    }



}
