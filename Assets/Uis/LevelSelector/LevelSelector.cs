using Assets.Scripts;
using Assets.Scripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.Managers.GameStateManager;

public class LevelSelector : MonoBehaviour
{
    // buttons, patient name and descriptions per levels
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

    private void ActivePatientUiElements(bool active)
    {
        foreach (GameObject patientElem in patientsList)
        {
            patientElem.SetActive(active);
        }
    }

    private void ChangeLevel(int index)
    {
        // get level composition
        currentIndexLevelSelected = index;
        // check if level have patient

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

    private void ShowPatientButtons(int nbPatient)
    {
        // instatiate missing buttons

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
                // TODO : REMPLACER PAR TON GETNAME (tu peux utiliser currentIndexSelected pour avoir l'id du niveau actuel)
                string name = levelsData.patientByLevel[currentIndexLevelSelected].patientsCase[bindex].fisrtName;
                patientButtonsContainer.transform.GetChild(bindex).GetChild(0).GetComponent<TextMeshProUGUI>().text = name;

                patientButtonsContainer.transform.GetChild(bindex).GetComponent<ButtonPressDetector>().OnPress.AddListener(delegate { ChangeDescription(bindex); });
                patientButtonsContainer.transform.GetChild(bindex).gameObject.SetActive(true);
            }
            else patientButtonsContainer.transform.GetChild(bindex).gameObject.SetActive(false);
        }
    }


    private void ChangeDescription(int indexPatientCase)
    {
        currentIndexPatientCase = indexPatientCase;
        string infoLevel = levelsData.patientByLevel[currentIndexLevelSelected].patientsCase[indexPatientCase].descriptionLevel;
        description.text = infoLevel;
    }

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
