using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReceiver : MonoBehaviour
{
    public void OnSelect(InputValue value)
    {
        if (!value.isPressed) return;

        TutorialManager.Instance.OnNext();
    }
}
