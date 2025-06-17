using System.Collections.Generic;
using UnityEngine;

public static class ButtonsManager
{
    public static Dictionary<int, List<ButtonPressDetector>> connectedButtons = new();

    public static void Connectbutton(ButtonPressDetector button)
    {
        //Debug.Log("" + button.groupId);
        if (connectedButtons.ContainsKey(button.groupId))
            connectedButtons[button.groupId].Add(button);
        else connectedButtons.Add(button.groupId, new List<ButtonPressDetector> { button });
    }

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
