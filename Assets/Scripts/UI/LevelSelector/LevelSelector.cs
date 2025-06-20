using Assets.Scripts.Managers;
using Assets.Scripts.PatientData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.Managers.GameStateManager;

/// <summary>
/// Manages level selection, patient case buttons, and game start logic in the level selection menu.
/// </summary>
/// <remarks>
/// Handles UI elements for selecting levels and patients, including dynamically instantiating buttons,
/// updating descriptions, and launching the game with the selected configuration.
/// </remarks>
public class LevelSelector : MonoBehaviour
{
    // buttons, patient name and descriptions per levels
    /// <summary>
    /// Container for each level's difficulty button.
    /// </summary>
    [System.Serializable]
    public class LevelComposition
    {
        public ButtonPressDetector difficultiesBt;
    }

    [SerializeField, Header("Ui elements")] private GameObject levelSelector; // Use to desactivate levelSelector.
    [SerializeField] private GameObject[] patientsList; // Uis to desactive when tuto selected (scroll view gameObject + Separator gameObject).
    [SerializeField] private GameObject patientButtonsContainer, patientButtonPref; // container and pref to instantiate inside.
    [SerializeField] private TextMeshProUGUI description; // descriptions text ui.
    [SerializeField, Header("Difficulty buttons")] private LevelComposition[] levelCompos;
    [SerializeField, Header("Start button")] private Button startGameButton;

    private LevelsData levelsData;
    private int currentIndexLevelSelected = 0;
    private int currentIndexPatientCase = 0;

    /// <summary>
    /// Initializes level selection, default UI state, and button listeners on startup.
    /// </summary>
    void Start()
    {
        // add listerner to stats button
        startGameButton.onClick.AddListener(LoadPatientCase);

        // Load levels
        levelsData = GameManager.Instance.LevelsData;

        // Le tutoriel et selectionner de base.
        ButtonsManager.SetButtonFocused(levelCompos[0].difficultiesBt);
        ActivePatientUiElements(false);
        ChangeDescription(0);
        for (int i = 0; i < levelCompos.Length; i++)
        {
            int index = i;
            levelCompos[i].difficultiesBt.OnPress.AddListener(delegate { ChangeLevel(index); });
        }

    }

    /// <summary>
    /// Activates or deactivates patient-related UI elements (e.g., scroll view and separators).
    /// </summary>
    /// <param name="active">Whether to enable or disable the patient UI elements.</param>
    private void ActivePatientUiElements(bool active)
    {
        foreach (GameObject patientElem in patientsList)
        {
            patientElem.SetActive(active);
        }
    }

    /// <summary>
    /// Changes the selected level and updates UI elements accordingly, such as patient buttons and description.
    /// </summary>
    /// <param name="index">Index of the selected level.</param>
    private void ChangeLevel(int index)
    {
        // get level composition
        currentIndexLevelSelected = index;

        // Get nb patient case
        int nbPatient = levelsData.patientByLevel[currentIndexLevelSelected].patientsCase.Count;

        if (nbPatient > 1)
        {
            
            // assign current description and patient buttons
            ShowPatientButtons(nbPatient);
            ActivePatientUiElements(true);
            ChangeDescription(0);
            ButtonsManager.SetButtonFocused(patientButtonsContainer.transform.GetChild(0).GetComponent<ButtonPressDetector>());
        }
        else
        {
            // assign current description
            ActivePatientUiElements(false);
            ChangeDescription(0);
        }
    }

    /// <summary>
    /// Instantiates and configures patient selection buttons for the selected level.
    /// </summary>
    /// <param name="nbPatient">Number of patient cases to display for the selected level.</param>
    private void ShowPatientButtons(int nbPatient)
    {
        int currentPatient = GameManager.Instance.GameStateManager.GetCurrentPatientCase();

        // Get nb patient case
        if (patientButtonsContainer.transform.childCount < nbPatient)
        {
            int childNumb = patientButtonsContainer.transform.childCount;
            for (int i = 0; i < nbPatient - childNumb; i++)
            {
                Instantiate(patientButtonPref, patientButtonsContainer.transform);
            }
        }
        // configure buttons
        for (int b = 0; b < patientButtonsContainer.transform.childCount; b++)
        {
            int bindex = b;
            if (bindex < nbPatient)
            {
                string name = levelsData.patientByLevel[currentIndexLevelSelected].patientsCase[bindex].firstName;
                patientButtonsContainer.transform.GetChild(bindex).GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
                
                patientButtonsContainer.transform.GetChild(bindex).GetComponent<ButtonPressDetector>().OnPress.AddListener(delegate { ChangeDescription(bindex); });                
                patientButtonsContainer.transform.GetChild(bindex).gameObject.SetActive(true);

                if (bindex > currentPatient) patientButtonsContainer.transform.GetChild(bindex).GetComponent<ButtonPressDetector>().AssignState(ButtonPressDetector.ButtonState.Disable);
                else patientButtonsContainer.transform.GetChild(bindex).GetComponent<ButtonPressDetector>().AssignState(ButtonPressDetector.ButtonState.None, true);
            }
        }
    }

    /// <summary>
    /// Updates the description text for the currently selected patient case.
    /// </summary>
    /// <param name="indexPatientCase">Index of the selected patient case within the level.</param>
    private void ChangeDescription(int indexPatientCase)
    {
        currentIndexPatientCase = indexPatientCase;
        string infoLevel = levelsData.patientByLevel[currentIndexLevelSelected].patientsCase[indexPatientCase].descriptionLevel;
        description.text = infoLevel;
    }

    /// <summary>
    /// Visually updates the difficulty button for the current level to reflect its selected state.
    /// </summary>
    public void UpdateLevelButton()
    {
        int currentLevel = GameManager.Instance.GameStateManager.GetCurrentLevel();
        levelCompos[currentLevel].difficultiesBt.AssignState(ButtonPressDetector.ButtonState.None, true);
    }

    /// <summary>
    /// Sets the selected level and patient case in the game state and transitions to the game menu.
    /// </summary>
    // Call on "commencer" button click by player
    private void LoadPatientCase()
    {
        // SET Game state level and patient case
        GameManager.Instance.GameStateManager.SetLevel((LevelState) currentIndexLevelSelected);
        GameManager.Instance.GameStateManager.SetPatientCase((PatientCase)currentIndexPatientCase, levelsData.patientByLevel[currentIndexLevelSelected].patientsCase[currentIndexPatientCase]);
        // Re-load game menus (need to change patient sprite)
        GameManager.Instance.LoadGameMenu();
    }

}
