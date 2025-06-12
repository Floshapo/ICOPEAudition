using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonPressDetector : Graphic, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public enum ButtonState
    {
        None,
        Hovered,
        Pressed,
        HoveredPressed,
        Released,
        Disable
    }

    [System.Serializable]
    private class IconColor
    {
        public Image buttonIcone;
        public ColorBlock colorBlock;
    }

    [SerializeField] private List<IconColor> iconColors;

    public UnityEvent OnPress, OnPressExit, OnHover, OnHoverExit;

    private ButtonState currentButtonState = ButtonState.None;

    protected override void OnEnable()
    {
        base.OnEnable();
        AssignColor();
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        currentButtonState = ButtonState.Pressed;
        AssignColor();
        OnPress.Invoke();
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        currentButtonState = ButtonState.Released;
        AssignColor();
        OnPressExit.Invoke();
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        currentButtonState = currentButtonState == ButtonState.Pressed ? ButtonState.HoveredPressed : ButtonState.Hovered;
        AssignColor();
        OnHover.Invoke();
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        currentButtonState = currentButtonState == ButtonState.HoveredPressed ? ButtonState.Pressed : ButtonState.None;
        AssignColor();
        OnHoverExit.Invoke();
    }

    private void AssignColor()
    {
        int index = (int)currentButtonState;
        foreach (IconColor col in iconColors)
        {
            col.buttonIcone.color = index == 0 ? col.colorBlock.normalColor : index == 1 || index == 3 ? col.colorBlock.highlightedColor : index == 2 ? col.colorBlock.pressedColor : index == 4 ? col.colorBlock.selectedColor : col.colorBlock.disabledColor;
        }
    }
}
