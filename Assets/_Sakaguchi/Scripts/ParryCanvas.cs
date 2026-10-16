using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ParryCanvas : MonoBehaviour
{
    // ƒVƒ“ƒOƒ‹ƒgƒ“ˆ—
    [SerializeField] Image Image;
    [SerializeField] Sprite[] parryDirSprites;

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
        Image.sprite = parryDirSprites[a];
    }

}
