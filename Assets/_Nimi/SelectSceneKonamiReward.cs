using System;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using EnchantmentName = EnchantmentDataBase.EnchantmentName;

public sealed class SelectSceneKonamiReward : MonoBehaviour
{
    [SerializeField] private GameObject _homePanel;
    [SerializeField] private GameObject _stagePanel;
    [SerializeField] private WeaponSynthesisResultView _resultSource;

    private enum CommandButton { Up, Down, Left, Right, B, A }

    private static readonly CommandButton[] Command =
    {
        CommandButton.Up, CommandButton.Up, CommandButton.Down, CommandButton.Down,
        CommandButton.Left, CommandButton.Right, CommandButton.Left, CommandButton.Right,
        CommandButton.B, CommandButton.A
    };
    private const int RewardLevel = 999;

    private Gamepad _commandPad;
    private int _commandIndex;
    private WeaponSynthesisResultView _result;
    private WeaponInstance _reward;
    private GameObject _previousSelection;

    internal bool ConsumeNavigation(EventSystem events)
    {
        var pad = Gamepad.current;
        if (_reward != null)
        {
            var first = _result.FirstSelectable;
            if (first == null) return true;

            var selected = events.currentSelectedGameObject;
            if (selected == null || !selected.transform.IsChildOf(_result.transform))
                events.SetSelectedGameObject(first.gameObject);

            if (pad?.buttonEast.wasPressedThisFrame != true) return false;
            _result.Cancel();
            return true;
        }

        if ((FadeSceneChanger.Instance != null && FadeSceneChanger.Instance.isFading) ||
            (!_homePanel.activeInHierarchy && !_stagePanel.activeInHierarchy))
        {
            _commandIndex = 0;
            return false;
        }

        if (pad != _commandPad)
        {
            _commandPad = pad;
            _commandIndex = 0;
        }
        if (pad == null) return false;

        bool pressedA = pad.buttonSouth.wasPressedThisFrame;
        var expected = Command[_commandIndex];
        var input = expected == CommandButton.A && pressedA
            ? CommandButton.A : ReadButton(pad);
        if (!input.HasValue) return false;
        if (input.Value != expected)
        {
            // 「上・上・上」の末尾2回もコマンドの先頭として扱う。
            _commandIndex = input.Value == CommandButton.Up ? (_commandIndex == 2 ? 2 : 1) : 0;
            LogCommandInput(input.Value, expected);
            return false;
        }

        _commandIndex++;
        // 最後の B と A が同じ入力更新に届いた場合もコマンドとして消費する。
        bool pressedTogether = _commandIndex == Command.Length - 1 && pressedA;
        if (pressedTogether) _commandIndex++;
        LogCommandInput(input.Value, pressedTogether: pressedTogether);
        if (_commandIndex < Command.Length) return false;

        _commandIndex = 0;
        _previousSelection = events.currentSelectedGameObject;
        events.SetSelectedGameObject(null);
        GrantReward();
        return true;
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    private void LogCommandInput(CommandButton button, CommandButton? expected = null, bool pressedTogether = false)
    {
        string input = pressedTogether ? "B → A" : FormatButton(button);
        string status = "コマンド成立";
        if (expected.HasValue)
            status = $"順番が違います（期待入力 {FormatButton(expected.Value)}）";
        else if (_commandIndex < Command.Length)
            status = $"次は {FormatButton(Command[_commandIndex])}";
        Debug.Log($"[コナミコマンド] {input}  {_commandIndex}/{Command.Length}  {status}", this);
    }

    private static string FormatButton(CommandButton button) => button switch
    {
        CommandButton.Up => "↑",
        CommandButton.Down => "↓",
        CommandButton.Left => "←",
        CommandButton.Right => "→",
        _ => button.ToString()
    };

    private static CommandButton? ReadButton(Gamepad pad)
    {
        if (pad.dpad.up.wasPressedThisFrame) return CommandButton.Up;
        if (pad.dpad.down.wasPressedThisFrame) return CommandButton.Down;
        if (pad.dpad.left.wasPressedThisFrame) return CommandButton.Left;
        if (pad.dpad.right.wasPressedThisFrame) return CommandButton.Right;
        if (pad.buttonEast.wasPressedThisFrame) return CommandButton.B;
        if (pad.buttonSouth.wasPressedThisFrame) return CommandButton.A;
        return null;
    }

    private void GrantReward()
    {
        var inventory = WeaponInventory.Instance;
        var database = inventory != null ? inventory.defaultEnchantmentDB : null;
        if (_resultSource == null || database == null || database.EnchantmentData == null) return;

        var equipped = inventory.GetWeapon(inventory.equipmentIndex);
        var template = inventory.GetTemplateByID(equipped?.templateName) ?? inventory.sword_ShieldTemplate;
        if (template == null) return;

        var candidates = database.EnchantmentData.FindAll(entry =>
            entry?.Efficacy is { Length: > 1 } && entry.Name != EnchantmentName.なし &&
            // 現行テーブルでは移動速度上昇の効果が負で、+999にすると逆方向へ移動する。
            (entry.Name != EnchantmentName.移動速度上昇 ||
             new EnchantmentSlot(entry.Name, RewardLevel).GetEfficacy(database) >= 0f));
        if (candidates.Count == 0) return;

        var name = candidates[UnityEngine.Random.Range(0, candidates.Count)].Name;
        var weapon = new WeaponInstance(template, database);
        weapon.SetEnchantments(new[] { new EnchantmentSlot(name, RewardLevel) });
        try
        {
            if (!inventory.AddWeapon(weapon)) return;
        }
        catch (Exception exception) when (exception is IOException || exception is UnauthorizedAccessException)
        {
            Debug.LogError($"獲得した武器を保存できませんでした: {exception.Message}", this);
            return;
        }

        if (_result == null)
        {
            _result = Instantiate(_resultSource, _homePanel.transform.parent, false);
            _result.ChoiceSubmitted += OnChoiceSubmitted;
        }
        _reward = weapon;
        _result.Show(weapon, "武器獲得", "装備しない");
    }

    private void OnChoiceSubmitted(bool equip)
    {
        if (_reward == null) return;
        if (equip && !WeaponInventory.Instance.TryEquipWeapon(_reward)) return;

        _result.Hide();
        _reward = null;
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
    }

    private void LateUpdate()
    {
        // 結果を閉じた入力でステージ選択などが決定されないよう、UI入力処理の後に戻す。
        if (_reward != null || _previousSelection == null) return;
        if (_previousSelection.activeInHierarchy && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(_previousSelection);
        _previousSelection = null;
    }

    private void OnDisable()
    {
        _commandIndex = 0;
        _reward = null;
        _previousSelection = null;
        if (_result != null) _result.Hide();
    }

    private void OnDestroy()
    {
        if (_result == null) return;
        _result.ChoiceSubmitted -= OnChoiceSubmitted;
        Destroy(_result.gameObject);
    }
}
