using UnityEngine;

/// <summary>
/// Manages a panel that displays a random tip from a collection of tip GameObjects.
/// Allows showing the panel with one random tip visible and hiding the entire panel.
/// </summary>
public class TipsPanel : MonoBehaviour
{
    [SerializeField] private GameObject[] _tipsGameObjects;

    /// <summary>
    /// Shows the panel and activates one random tip while deactivating the others.
    /// </summary>
    public void Display()
    {
        gameObject.SetActive(true);

        foreach (GameObject tip in _tipsGameObjects)
        {
            tip.SetActive(false);
        }

        int randomIndex = Random.Range(0, _tipsGameObjects.Length);
        _tipsGameObjects[randomIndex].SetActive(true);
    }

    /// <summary>
    /// Hides the entire tips panel.
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
