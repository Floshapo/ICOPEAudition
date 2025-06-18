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
        /// Change indexText value for loading next text.
        /// </summary>
        public void NextTextButton()
        {
            indexText++;
            SetTexts(nameStep, indexText);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Set text with the xml information.
        /// </summary>
        /// <remarks>
        /// Set text for an intitule and Text (dialogue).
        /// </remarks>
        /// <param name="nameStep">The name of the current state for the XML file.</param>
        /// <param name="idSteps">The ids of the current text to load.</param>
        private void SetTexts(string nameStep, int idSteps)
        {
            Debug.Log("Here");
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
        /// Check if input button like mouse click (Fire1) and keyboard (enter) is hit and call fuction "nextTextButton".
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
        private void OnEnable()
        {
            nameStep = "Waiting_room"; 
            SetTexts(nameStep, indexText);
        }

        private void Update()
        {
            if (GameManager.Instance._tutorialPanel.activeSelf) GetInputs();
        }
        #endregion
    }
}
