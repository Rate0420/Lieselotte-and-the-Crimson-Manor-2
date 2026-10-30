using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.UI;

/// <summary>武器・付呪一覧で共用し、決定ボタンを離すまで同じ押下を再び受け付けない。</summary>
public static class NimiUIConfirmInput
{
    private static readonly HashSet<ButtonControl> ConsumedButtons = new();
    private static bool _listening;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reset()
    {
        InputSystem.onAfterUpdate -= ReleaseCompletedPresses;
        ConsumedButtons.Clear();
        _listening = false;
    }

    /// <summary>今回の決定を処理してよい場合にtrue。表示更新からは呼ばず、ユーザー入力だけに使う。</summary>
    public static bool TryConsume()
    {
        if (!_listening)
        {
            InputSystem.onAfterUpdate += ReleaseCompletedPresses;
            _listening = true;
        }

        // Toggleのマウスクリックはボタンを離したときに届くため、パッドの長押しとは別に扱う。
        if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame) return true;

        var module = EventSystem.current != null ? EventSystem.current.currentInputModule as InputSystemUIInputModule : null;
        var button = module?.submit?.action?.activeControl as ButtonControl;
        if (button == null || !button.isPressed) button = Gamepad.current?.buttonSouth;
        if (button == null || !button.isPressed) return true;

        // 行ごとに保持するとカーソル移動や画面切替で再決定できてしまうため、押下状態を共有する。
        return ConsumedButtons.Add(button);
    }

    private static void ReleaseCompletedPresses()
    {
        // 入力更新直後に解除を検知し、次に押したときは待ち時間なしで受け付ける。
        ConsumedButtons.RemoveWhere(button => !button.device.added || !button.isPressed);
    }
}
