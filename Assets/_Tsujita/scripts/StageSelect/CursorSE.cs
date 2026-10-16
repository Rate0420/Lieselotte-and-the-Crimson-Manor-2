using UnityEngine;
using UnityEngine.EventSystems;

public class CursorSE : MonoBehaviour
{
    [SerializeField] private SoundPush soundPush;

    private GameObject lastSelected;
    private bool ignoreNext = true;

    private int lastPlayFrame = -1;

    void Update()
    {
        if (EventSystem.current == null) return;
        if (!EventSystem.current.enabled) return;

        var current = EventSystem.current.currentSelectedGameObject;

        if (current == null) return;

        if (current != lastSelected)
        {
            if (!ignoreNext)
            {
                // ìØÇ∂ÉtÉåÅ[ÉÄÇ≈2âÒñ¬ÇÈÇÃÇñhÇÆ
                if (lastPlayFrame != Time.frameCount)
                {
                    soundPush.CursorPush();
                    lastPlayFrame = Time.frameCount;
                }
            }

            ignoreNext = false;
            lastSelected = current;
        }
    }

    public void IgnoreNextSelection()
    {
        ignoreNext = true;
    }
}