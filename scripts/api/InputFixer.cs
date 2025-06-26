using System;
using System.Collections.Generic;
using Godot;

namespace Parallas;

public static class InputFixer
{
    public static bool IsGamepad { get; private set; }
    public static bool SwapAB { get; private set; }
    public static bool ForceSwapAB { get; set; } = false;
    public static List<String> AutoSwapGamepads { get; private set; } = ["PRO CONTROLLER"];
    public static bool GamepadSwapState => IsGamepad && (SwapAB ^ ForceSwapAB);

    public static void UpdateInput(InputEvent inputEvent)
    {
        IsGamepad = inputEvent is InputEventJoypadButton or InputEventJoypadMotion;
        var joyName = Input.GetJoyName(inputEvent.Device);
        SwapAB = AutoSwapGamepads.Contains(joyName.Trim().ToUpper());
    }

    public static bool ConfirmActionPressed()
    {
        return Input.IsActionJustPressed(!GamepadSwapState ? "confirm" : "cancel");
    }

    public static bool ConfirmActionReleased()
    {
        return Input.IsActionJustReleased(!GamepadSwapState ? "confirm" : "cancel");
    }

    public static bool ConfirmActionHeld()
    {
        return Input.IsActionPressed(!GamepadSwapState ? "confirm" : "cancel");
    }

    public static bool CancelActionPressed()
    {
        return Input.IsActionJustPressed(GamepadSwapState ? "confirm" : "cancel");
    }

    public static bool CancelActionReleased()
    {
        return Input.IsActionJustReleased(GamepadSwapState ? "confirm" : "cancel");
    }

    public static bool CancelActionHeld()
    {
        return Input.IsActionPressed(GamepadSwapState ? "confirm" : "cancel");
    }
}
