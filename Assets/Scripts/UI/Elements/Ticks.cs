using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the display of yes/no tick images based on a boolean value.
/// Shows the appropriate checked or unchecked images for yes and no states.
/// </summary>
public class Ticks : MonoBehaviour
{
    [SerializeField] private Image _yesTickChecked;
    [SerializeField] private Image _yesTickUnchecked;
    [SerializeField] private Image _noTickChecked;
    [SerializeField] private Image _noTickUnchecked;

    /// <summary>
    /// Updates the visibility of yes/no tick images based on the boolean value.
    /// Shows checked "yes" and unchecked "no" if true,
    /// otherwise shows unchecked "yes" and checked "no".
    /// </summary>
    /// <param name="value">Boolean value to display ticks for.</param>
    public void DisplayValue(bool value)
    {
        _yesTickChecked.gameObject.SetActive(value);
        _yesTickUnchecked.gameObject.SetActive(!value);
        _noTickChecked.gameObject.SetActive(!value);
        _noTickUnchecked.gameObject.SetActive(value);
    }
}
