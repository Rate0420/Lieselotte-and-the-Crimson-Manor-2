using UnityEngine;
using UnityEngine.InputSystem.UI;

public sealed class SelectSceneInputModule : InputSystemUIInputModule
{
    [SerializeField] private SelectSceneKonamiReward _konamiReward;

    public override void Process()
    {
        bool sendNavigationEvents = eventSystem.sendNavigationEvents;
        if (_konamiReward == null || !_konamiReward.isActiveAndEnabled ||
            !_konamiReward.ConsumeNavigation(eventSystem))
        {
            base.Process();
            return;
        }

        // コマンドの A を Submit として送らず、ポインター入力は通常どおり処理する。
        eventSystem.sendNavigationEvents = false;
        try
        {
            base.Process();
        }
        finally
        {
            eventSystem.sendNavigationEvents = sendNavigationEvents;
        }
    }
}
