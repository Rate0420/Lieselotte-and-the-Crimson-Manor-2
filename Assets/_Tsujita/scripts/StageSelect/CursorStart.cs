using UnityEngine;
using UnityEngine.EventSystems;

public class CursorStart : MonoBehaviour
{
    [SerializeField] private GameObject startButton;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(startButton);
    }
}
