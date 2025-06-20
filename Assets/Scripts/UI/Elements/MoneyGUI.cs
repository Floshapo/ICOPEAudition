using Assets.Scripts.Managers;
using UnityEngine;

/// <summary>
/// Handles displaying the player's current money and the amount of change in the UI,
/// updating the display and playing an animation whenever the money amount changes.
/// </summary>
public class MoneyGUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMPro.TextMeshProUGUI _moneyText;
    [SerializeField] private TMPro.TextMeshProUGUI _amountChangeText;
    [SerializeField] private Animation _animation;

    [Header("Colors")]
    [SerializeField] private Color _positiveColor;
    [SerializeField] private Color _negativeColor;

    /// <summary>
    /// Initializes the money display with the current amount and subscribes to money change events.
    /// </summary>
    private void Start()
    {
        _moneyText.text = GameManager.Instance.Money.ToString();
        GameManager.Instance.OnMoneyChanged += OnMoneyChanged;
    }

    /// <summary>
    /// Updates the displayed money amount and the change amount with proper sign and color,
    /// then plays an animation to highlight the change.
    /// </summary>
    /// <param name="money">The new total money amount.</param>
    /// <param name="amountChange">The change in money (positive or negative).</param>
    private void OnMoneyChanged(int money, int amountChange)
    {
        _moneyText.text = money.ToString();
        string displayText = amountChange > 0 ? "+ " : "- ";
        displayText += Mathf.Abs(amountChange).ToString();
        _amountChangeText.text = displayText;
        _amountChangeText.color = amountChange > 0 ? _positiveColor : _negativeColor;

        _animation.Stop();
        _animation.Play();
    }
}