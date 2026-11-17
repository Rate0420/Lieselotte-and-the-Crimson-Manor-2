using UnityEngine;
using UnityEngine.UI;

public class TestSupport : MonoBehaviour
{
    [SerializeField] private float supportTime;
    [SerializeField] private Sprite supportSprite;
    [SerializeField] private Image supportImage;
    [SerializeField] private Color nolmalColor = default;
    float time;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        supportImage.color = nolmalColor;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            supportImage.color = HexColor("#FF0000");
            time += Time.deltaTime;
        }

        if(time >= supportTime)
        {
            supportImage.color = HexColor("#000000");
        }
    }

    private Color HexColor(string hex)
    {
        Color color;
        ColorUtility.TryParseHtmlString(hex, out color);
        return color;
    }
}
