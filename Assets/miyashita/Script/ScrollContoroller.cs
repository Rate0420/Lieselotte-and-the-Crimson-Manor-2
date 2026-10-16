using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ScrollBar : MonoBehaviour
{
    public ScrollRect ScrollRect;
    public float scrollSpeed = 0.5f;   // スティックの感度
    public float smooth = 0.2f;        // スムーズ係数

    void Update()
    {
        if (Gamepad.current == null)
            return;

        float v = Gamepad.current.leftStick.ReadValue().y;

        if (Mathf.Abs(v) > 0.1f)
        {
            float target = ScrollRect.verticalNormalizedPosition + (v * scrollSpeed * Time.deltaTime);

            // 0～1で制限
            target = Mathf.Clamp01(target);

            // スムーズにスクロール
            ScrollRect.verticalNormalizedPosition =
                Mathf.Lerp(ScrollRect.verticalNormalizedPosition, target, smooth);
        }
    }
}
