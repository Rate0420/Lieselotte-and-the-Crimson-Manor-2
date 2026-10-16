using UnityEngine;
using UnityEngine.UI;

public class ResultBuckChange : MonoBehaviour
{
    // ゲームシーン終了時の画面をリザルト画面で反映する場合は再調整

    [SerializeField] private Image targetImage;
    [SerializeField] private Sprite[] backGroundImages;

    private int stageNo;

    private void Awake()
    {
        stageNo = ScoreData.instance.stageNoData;
        ImageChange();
    }

    private void ImageChange()
    {
        if (stageNo >= 0 && stageNo < backGroundImages.Length)
        {
            targetImage.sprite = backGroundImages[stageNo];
        }
    }
}