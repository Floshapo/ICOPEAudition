using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Static manager for handling button focus states across grouped UI buttons.
/// </summary>
/// <remarks>
/// Buttons are grouped by a group ID, allowing only one button in a group to be "pressed" (focused) at a time.
/// This is useful for tab selectors, difficulty buttons, etc.
/// </remarks>
public static class ButtonsManager
{
    public static Dictionary<int, List<ButtonPressDetector>> connectedButtons = new();

    /// <summary>
    /// Adds a button to its corresponding group in the manager, allowing it to be managed for focus state.
    /// </summary>
    /// <param name="button">The button to connect, which contains a group ID.</param>
    public static void Connectbutton(ButtonPressDetector button)
    {
        if (connectedButtons.ContainsKey(button.groupId))
            connectedButtons[button.groupId].Add(button);
        else connectedButtons.Add(button.groupId, new List<ButtonPressDetector> { button });
    }

    /// <summary>
    /// Sets the specified button as the focused (pressed) one within its group, and resets others to normal state.
    /// </summary>
    /// <param name="button">The button to set as focused (pressed).</param>
    public static void SetButtonFocused(ButtonPressDetector button)
    {
        foreach (ButtonPressDetector connectButton in connectedButtons[button.groupId])
        {
            if (connectButton == button) connectButton.AssignState(ButtonPressDetector.ButtonState.Pressed);
            else connectButton.AssignState(ButtonPressDetector.ButtonState.None);
        }
        button.AssignState(ButtonPressDetector.ButtonState.Pressed);
    }
}
