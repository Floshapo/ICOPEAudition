using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages an answer button in the UI with different visual states
/// for normal, correct, and incorrect answers.
/// Requires Unity UI Button and Image components.
/// </summary>
[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class AnswerButton : MonoBehaviour
{
    public bool IsConfirmed { get; private set; }
    public Button AssociatedButton
    {
        get { return _button; }
    }
    private Button _button;
    private TMP_Text _text;
    private Image _image;

    [Header("Colors")]
    [SerializeField] private Color _normalColor;
    [SerializeField] private Color _correctColor;
    [SerializeField] private Color _incorrectColor;

    /// <summary>
    /// Initializes references to the Button, TMP_Text, and Image components on the GameObject and its children.
    /// </summary>
    void Awake()
    {
        _button = gameObject.GetComponent<Button>();
        _text = gameObject.GetComponentInChildren<TMP_Text>();
        _image = gameObject.GetComponent<Image>();
    }

    /// <summary> Sets the button's displayed text. </summary>
    public void SetText(string text)
    {
        _text.text = text;
    }

    /// <summary> Enables or disables interaction on the button. </summary>
    public void SetInteractable(bool interactable)
    {
        _button.interactable = interactable;
    }

    /// <summary> Resets the button to its normal state and makes it interactable. </summary>
    public void Reset()
    {
        IsConfirmed = false;
        _image.color = _normalColor;
        SetInteractable(true);
    }

    /// <summary> Marks the button as correct, changes color, and disables interaction. </summary>
    public void SetCorrect()
    {
        IsConfirmed = true;
        _image.color = _correctColor;
        SetInteractable(false);
    }

    /// <summary> Marks the button as incorrect, changes color, and disables interaction. </summary>
    public void SetIncorrect()
    {
        IsConfirmed = true;
        _image.color = _incorrectColor;
        SetInteractable(false);
    }
}
