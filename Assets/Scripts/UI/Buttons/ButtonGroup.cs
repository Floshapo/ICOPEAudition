using UnityEngine;

/// <summary>
/// Manages a group of AnswerButton components, handling selection,
/// interaction states, and answer validation feedback.
/// </summary>
public class ButtonGroup : MonoBehaviour
{

    /// <summary>
    /// Event triggered when a button is selected.
    /// </summary>
    public event System.Action OnButtonSelected;
    public int SelectedButtonIndex { get; private set; }

    private AnswerButton[] _answerButtons = new AnswerButton[0];

    /// <summary>
    /// Initializes the AnswerButton array by retrieving all child AnswerButton components (including inactive ones),
    /// attaches click listeners to each button to handle clicks,
    /// and resets the buttons to their default state.
    /// </summary>
    private void Start()
    {
        _answerButtons = GetComponentsInChildren<AnswerButton>(true);
        foreach (AnswerButton answerButton in _answerButtons)
        {
            answerButton.AssociatedButton.onClick.AddListener(() => OnButtonClicked(answerButton));
        }

        Reset();
    }

    /// <summary>
    /// Called when a button is clicked.
    /// Disables all buttons except the clicked one (unless confirmed).
    /// Sets the selected index and invokes the selection event.
    /// </summary>
    private void OnButtonClicked(AnswerButton clickedButton)
    {
        for (int i = 0; i < _answerButtons.Length; i++)
        {
            _answerButtons[i].SetInteractable(_answerButtons[i] != clickedButton && !_answerButtons[i].IsConfirmed);
            if (_answerButtons[i] == clickedButton)
            {
                SelectedButtonIndex = i;
            }
        }
        OnButtonSelected?.Invoke();
    }

    /// <summary>
    /// Resets all buttons to normal state and clears selection.
    /// </summary>
    public void Reset()
    {
        foreach (AnswerButton answerButton in _answerButtons) answerButton.Reset();
        SelectedButtonIndex = -1;
    }

    /// <summary>
    /// Marks the button at the given index as correct and disables all buttons.
    /// </summary>
    public void SetRightAnswer(int index)
    {
        _answerButtons[index].SetCorrect();
        foreach (AnswerButton answerButton in _answerButtons)
        {
            answerButton.SetInteractable(false);
        }
    }


    /// <summary>
    /// Marks the button at the given index as incorrect and clears selection.
    /// </summary>
    public void SetWrongAnswer(int index)
    {
        _answerButtons[index].SetIncorrect();
        SelectedButtonIndex = -1;
    }

    /// <summary>
    /// Sets the answer validity of a button by index.
    /// Calls SetRightAnswer or SetWrongAnswer accordingly.
    /// Logs an error if index is out of range.
    /// </summary>
    public void SetAnswerValidity(int index, bool isCorrect)
    {
        if (index < 0 || index >= _answerButtons.Length)
        {
            Debug.LogError("Button index out of range");
            return;
        }
        if (isCorrect) SetRightAnswer(index);
        else SetWrongAnswer(index);
    }
}
