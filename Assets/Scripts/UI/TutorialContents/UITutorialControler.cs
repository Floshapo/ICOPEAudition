using Assets.Scripts.Managers;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI.TutorialContents
{
    /// <summary>
    /// Controler that manage text placement for the Tutorial.
    /// </summary>
    public class UITutorialControler : MonoBehaviour
    {
        #region SerializedField TMP_text
        [SerializeField] private TMP_Text _intituleText;
        [SerializeField] private TMP_Text _Text;
        #endregion

        #region Local variable
        private int indexText = 0;
        private string nameStep;
        #endregion

        #region Public methods
        /// <summary>
        /// Advances to the next tutorial text by incrementing the index and loading the corresponding text.
        /// </summary>
        public void NextTextButton()
        {
            indexText++;
            SetTexts(nameStep, indexText);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Sets the UI text fields based on XML tutorial data.
        /// </summary>
        /// <remarks>
        /// Loads a tutorial entry by its step name and text ID, then updates UI elements with the entry's title and text.
        /// If no entry is found, resets the tutorial UI and index.
        /// </remarks>
        /// <param name="nameStep">The name of the current tutorial step (used to locate data in the XML).</param>
        /// <param name="idSteps">The ID of the specific text entry to load.</param>
        private void SetTexts(string nameStep, int idSteps)
        {
            var pathToXml = Resources.Load<TextAsset>($"Data/Tutorial");
            if ( pathToXml != null )
            {
                TutorialEntry tutoEntry = XmlManager.LoadTutoriaDataByID(pathToXml, nameStep, idSteps);
                if (tutoEntry != null)
                {
                    _intituleText.text = tutoEntry.Intitule;
                    _Text.text = tutoEntry.Text;
                }
                else
                {
                    GameManager.Instance.SetTutorialUI();
                    indexText = 0;
                }
            }
        }

        /// <summary>
        /// Checks for input from mouse click ("Fire1") or keyboard (Enter key).
        /// If input is detected, triggers the NextTextButton() method.
        /// </summary>
        private void GetInputs()
        {
            if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Return))
            {
                NextTextButton();
            }
        }
        #endregion

        #region Unity method
        /// <summary>
        /// Called when the object becomes enabled and active.
        /// Initializes the tutorial step name and sets related texts.
        /// </summary>
        private void OnEnable()
        {
            nameStep = "Waiting_room"; 
            SetTexts(nameStep, indexText);
        }

        /// <summary>
        /// Called once per frame.
        /// If the tutorial panel is active, it processes user input.
        /// </summary>
        private void Update()
        {
            if (GameManager.Instance._tutorialPanel.activeSelf) GetInputs();
        }
        #endregion
    }
}
