using Assets.Scripts.PatientData;
using Assets.Scripts.PatientData.AlgoData;
using Assets.Scripts.PatientData.Steps;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


namespace Assets.Scripts.Managers
{
    public class StepManagerN : MonoBehaviour
    {
        [SerializeField] private Button returnButton;
        [SerializeField] private Button confirmNextButton;

        // GameObject
        [Header("Steps gameObject")]
        [SerializeField] private GameObject patientDisplay;
        [SerializeField] private List<GameObject> displayList;

        [Header("Question gameObject")]
        [SerializeField] private GameObject questionsDisplay;
        [SerializeField] private Button[] choiceButtons;

        [Header("Correction gameObject")]
        [SerializeField] private GameObject correctionDisplay;
        [SerializeField] private TextMeshProUGUI answerText;
        [SerializeField] private TextMeshProUGUI answerSelected;
        [SerializeField] private GameObject answerJustification;
        [SerializeField] private List<GameObject> answerGameObjectSprites;

        [Header("Colors answer")]
        [SerializeField] private Color correctColor;
        [SerializeField] private Color incorrectColor;
        [SerializeField] private Image backgroudAnswer;

        // Sprite Doctor (1st position happy expression, 2nd position sad expression, 3rd position talking)
        // For the future to change to allow player to choose his character.
        [Header("Sprite doctor")]
        [SerializeField] private Sprite[] doctorSprite;

        [Header("GameObject Image correction")]
        [SerializeField] private Image doctorExpressionsImages;
        
        // Script of each step display
        private Step1PresentationPatient step1PresentationPatient;
        private Step2WisperTest step2WisperTest;
        private Step3And4Questionnary step4And5Questionnary;
        private Step5Otoscopie Step5Otoscopie;
        private Step6WeberTest step6HhiesTest;
        private Step7HhiesTest step7HhiesTest;
        private Step8Audiometrie Step8Audiometrie;

        // Enums
        private enum InteractionState { ISREADING, ISANSWERING, ISCORRECTION};
        private InteractionState interactionState;

        private enum AnswerState { DIAGNOSTIC, ACTION }
        private AnswerState answerState;

        // Private class 
        private NewPatientData patientData;
        
        // Private variables
        private int _indexStep;
        private int _currentDisplay;
        private bool _isDiagnosticValid;
        private bool _isActionValid;
        private Step step;

        private Dictionary<Step, int> mappingDisplays;

        /// <summary>
        /// Initializes the patient data and resets the step index and display index
        /// to start fresh for a new patient case.
        /// </summary>
        /// <param name="newPatient">The new patient data to initialize.</param>
        public void Initialize(NewPatientData newPatient)
        {
            _indexStep = 0; // reset current step to 0
            _currentDisplay = 0;
            patientData = newPatient;
        }

        /// <summary>
        /// Loads the specified step by updating the UI elements and setting the interaction state.
        /// Clears all previous displays and activates the relevant display based on the current step.
        /// Resets validation flags and sets the current display index accordingly.
        /// </summary>
        /// <param name="currentStep">The step to load and display.</param>
        public void LoadStep(Step currentStep)
        {
            ClearAllDisplay();
            
            step = currentStep;

            interactionState = InteractionState.ISREADING;

            // Set bools to fasle each step
            _isDiagnosticValid = false;
            _isActionValid = false;
            _currentDisplay = mappingDisplays[currentStep];

            Sprite patientSprite = null;

            switch (currentStep)
            {
                case Step.Case_presentation:
                    // Load patient sprite & patient text
                    step1PresentationPatient.SetSprites(patientData.characterSprites[0]);
                    step1PresentationPatient.SetPresentationTexts(patientData);
                    // Display current step

                    displayList[_currentDisplay].SetActive(true);
                    break;
                case Step.Wisper_test:
                    // Load wisper text (animation with dotween)
                    if (patientData.characterSprites.Length > 1)
                    {
                        patientSprite = patientData.characterSprites[1];
                    }
                    step2WisperTest.SetPatient(patientSprite); 
                    step2WisperTest.PlayFirstText(patientData.steps[_indexStep]);
                    // Display current step
                    displayList[_currentDisplay].SetActive(true);
                    break;
                case Step.Questionnary:
                    // Load questionary & answer
                    List<QuestionData> questions = patientData.steps[_indexStep].questionnaireData.questions;
                    List<PatientQuestionAnswer> answers = patientData.steps[_indexStep].predefinedAnwser;
                    // Set texts
                    step4And5Questionnary.SetQuestionayText(questions, answers);
                    //Set Patient Sprite
                    step4And5Questionnary.SetPatientSprite(patientData.characterSprites[0]);
                    // Display current step
                    displayList[_currentDisplay].SetActive(true);
                    break;
                case Step.Additional_questionnaire:
                    // Load questionary & answer
                    List<QuestionData> questions2 = patientData.steps[_indexStep].questionnaireData.questions;
                    List<PatientQuestionAnswer> answers2 = patientData.steps[_indexStep].predefinedAnwser;
                    step4And5Questionnary.SetQuestionayText(questions2, answers2);
                    // Display current step
                    displayList[_currentDisplay].SetActive(true);
                    break;
                case Step.Otoscopy:
                    // Load patient ear image
                    if (patientData.characterSprites.Length > 1)
                    {
                        patientSprite = patientData.characterSprites[1];
                    }

                    Step5Otoscopie.SetImages(patientData.steps[_indexStep].spriteEarExams, patientSprite);
                    // Display current step 
                    displayList[_currentDisplay].SetActive(true);
                    break;
                case Step.Weber_test:
                    // Load texts dialogue & sprite
                    step6HhiesTest.SetTextDialogue(patientData.steps[_indexStep].contextDescription);
                    step6HhiesTest.SetImage(patientData.characterSprites[0]);
                    // Display current step
                    displayList[_currentDisplay].SetActive(true);
                    break;
                case Step.HHIES_test:
                    // Load patient ear image
                    step7HhiesTest.SetImages(patientData.steps[_indexStep].spriteEarExams, patientData.characterSprites[0]);
                    // Display current step
                    displayList[_currentDisplay].SetActive(true);
                    break;
                case Step.Audiometry:
                    // Load patient audiometrie + patient sprite
                    Step8Audiometrie.SetSprite(patientData.steps[_indexStep].spriteEarExams, patientData.characterSprites[0]);
                    // Display current step
                    displayList[_currentDisplay].SetActive(true);
                    break;
            }

            // set navigation button (Buttons)
            SetTextButtonsNavigation();
        }

        // SET TEXT AND INTERACTION
        /// <summary>
        /// Updates the navigation buttons' interactability and text based on the current interaction state.
        /// - In ISREADING state: disables the return button, enables the confirm/next button with text "Répondre".
        /// - In ISANSWERING state: enables the return button, disables the confirm/next button with text "Suivant".
        /// - In ISCORRECTION state: 
        ///     - Checks if the current answer is valid (diagnostic or action).
        ///     - If valid, enables confirm/next button and disables return button, setting confirm button text to "Suivant".
        ///     - If not valid, disables confirm/next button and enables return button.
        /// </summary>
        private void SetTextButtonsNavigation()
        {
            TextMeshProUGUI confirmeNextText = confirmNextButton.GetComponentInChildren<TextMeshProUGUI>();
               
            if (interactionState == InteractionState.ISREADING)
            {
                // update buttons and text
                returnButton.interactable = false;
                confirmNextButton.interactable = true;
                confirmeNextText.text = "Répondre";
            }
            if (interactionState == InteractionState.ISANSWERING)
            {
                // Update button and text
                returnButton.interactable = true;
                
                confirmNextButton.interactable = false;
                confirmeNextText.text = "Suivant";
            }            
            if ( interactionState == InteractionState.ISCORRECTION)
            {
                bool isValid = false;
                switch (answerState)
                {
                    case AnswerState.DIAGNOSTIC:
                        isValid = _isDiagnosticValid;
                        break;
                    case AnswerState.ACTION:  
                        isValid = _isActionValid;
                        break;
                }

                if (isValid)
                {
                    confirmeNextText.text = "Suivant";
                    returnButton.interactable = false;
                    confirmNextButton.interactable = true;
                    confirmNextButton.gameObject.SetActive(true);
                }
                else
                {
                    confirmNextButton.interactable = false;
                    returnButton.interactable = true;
                }
            }
        }

        /// <summary>
        /// Sets up the current response buttons based on the patient's current step data.
        /// - If the step has a diagnostic phase and the diagnostic answer is not yet valid,
        ///   it sets the answer state to DIAGNOSTIC and creates the corresponding answer buttons.
        /// - If the diagnostic phase is already valid, it marks diagnostic as valid.
        /// - If the step has an action phase and the diagnostic phase is valid,
        ///   it sets the answer state to ACTION and creates the corresponding action answer buttons.
        /// </summary>
        private void SetResponses()
        {
            if (patientData.steps[_indexStep].hasDiagnosticPhase && !_isDiagnosticValid)
            {
                answerState = AnswerState.DIAGNOSTIC;
                CreateAnwserButtons(patientData.steps[_indexStep].diagnosticPhase);
            } 
            else
            {
                _isDiagnosticValid = true;
            }

            if (patientData.steps[_indexStep].hasActionPhase && _isDiagnosticValid)
            {
                answerState = AnswerState.ACTION;
                CreateAnwserButtons(patientData.steps[_indexStep].actionPhase);
            }
        }

        /// <summary>
        /// Clears existing answer buttons and creates new ones based on the provided phase data.
        /// For each answer in the phase data (up to the number of available buttons):
        /// - Sets the button text to the answer's text.
        /// - Resets the button state.
        /// - Adds a click listener that triggers OnAnswerCorrect with the corresponding answer index.
        /// - Enables and makes the button visible.
        /// </summary>
        /// <param name="phaseData">Data containing the possible answers for the current phase.</param>
        private void CreateAnwserButtons(PhaseData phaseData)
        {
            ClearQuestion();
            
            int max = Mathf.Min(phaseData.answerData.Count, choiceButtons.Length);

            for (int i = 0; i < max; i++)
            {
                int index = i;
                TextMeshProUGUI text = choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>();                
                text.text = phaseData.answerData[i].answerText;
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].GetComponent<AnswerButton>().Reset();
                choiceButtons[i].onClick.AddListener(() => OnAnswerCorrect(phaseData, index));
                choiceButtons[i].interactable = true;
                choiceButtons[i].enabled = true;
                choiceButtons[i].gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Switches the UI to the question answering display.
        /// Sets the interaction state to answering,
        /// clears all current displays, activates the question display,
        /// sets up the possible responses, and updates the navigation buttons.
        /// </summary>
        private void GoToQuestionDisplay()
        {
            interactionState = InteractionState.ISANSWERING;
            ClearAllDisplay();
            questionsDisplay.SetActive(true);
            SetResponses();
            SetTextButtonsNavigation();
        }

        /// <summary>
        /// Returns from the correction state back to the question answering display,
        /// clearing all displays and resetting the interaction state and buttons accordingly.
        /// Only works if currently in the correction state.
        /// </summary>
        private void BackToQuestion()
        {
            if (interactionState == InteractionState.ISCORRECTION)
            {
                ClearAllDisplay();
                
                interactionState = InteractionState.ISANSWERING;
                questionsDisplay.SetActive(true);
                SetTextButtonsNavigation();
            }
        }


        /// <summary>
        /// Returns from the answering state back to the document (reading) display,
        /// clearing all displays and resetting interaction state and navigation buttons.
        /// Only works if currently in the answering state.
        /// </summary>
        private void BackToDocument()
        {
            if (interactionState == InteractionState.ISANSWERING)
            {
                ClearAllDisplay();

                interactionState = InteractionState.ISREADING;
                displayList[_currentDisplay].SetActive(true);
                SetTextButtonsNavigation();
            }
        }

        /// <summary>
        /// Deactivates all UI displays including the general display list,
        /// the question display, and the correction display.
        /// Used to reset the UI before showing new content.
        /// </summary>
        private void ClearAllDisplay()
        {
            
            for(int i = 0; i < displayList.Count; i++)
            {
                displayList[i].SetActive(false);
            }

            questionsDisplay.SetActive(false);
            correctionDisplay.SetActive(false);
        }

        /// <summary>
        /// Deactivates all answer choice buttons.
        /// Clears the current question’s answer options from the UI.
        /// </summary>
        private void ClearQuestion()
        {
            for (int i = 0; i < choiceButtons.Length; i++)
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Handles the logic when the player selects an answer button.
        /// Checks if the selected answer is correct and updates the state accordingly.
        /// Provides feedback by marking incorrect answers and displaying messages.
        /// Records the answer in the game data for persistence.
        /// </summary>
        /// <param name="phaseData">The phase data containing possible answers.</param>
        /// <param name="index">The index of the selected answer button.</param>
        private void OnAnswerCorrect(PhaseData phaseData, int index)
        {
            bool isCorrect = false;
            bool isDiagnosticAnswer = false;
            bool isActionAnswer = false;

            bool isCorrectAnswer = IsAnswerCorrect(phaseData.answerData, index);
            isCorrect = isCorrectAnswer;
            string feedBackText = isCorrectAnswer ? "Bonne réponse !" : "Mauvaise réponse !";

            if (!isCorrectAnswer)
            {
                choiceButtons[index].GetComponent<AnswerButton>().SetIncorrect();
            }

            switch (answerState)
            {
                case AnswerState.DIAGNOSTIC:
                    _isDiagnosticValid = isCorrectAnswer;
                    isDiagnosticAnswer = true;
                    break;
                case AnswerState.ACTION:
                    _isActionValid = isCorrectAnswer;
                    isActionAnswer = true;
                    break;
            }
            Debug.Log("Saving step...");

            GameManager.Instance.GameData.RecordsSteps(step, isDiagnosticAnswer, isActionAnswer, choiceButtons[index].GetComponentInChildren<TextMeshProUGUI>().text);
            ShowAnswerDetail(phaseData.answerData[index], feedBackText, isCorrect);
        }

        /// <summary>
        /// Checks if the answer at the specified index in the answer list is correct.
        /// </summary>
        /// <param name="answerData">List of possible answers.</param>
        /// <param name="index">Index of the selected answer.</param>
        /// <returns>True if the answer is correct; otherwise, false.</returns>
        private static bool IsAnswerCorrect(List<AnswerData> answerData, int index)
        {
            if (index < 0 || index >= answerData.Count)
            {
                Debug.LogError("index out of bound");
                return false;
            }
            return answerData[index].isCorrect;
        }

        /// <summary>
        /// Displays detailed feedback for the selected answer, including text, justification, 
        /// related images, and visual cues indicating whether the answer was correct or incorrect.
        /// Also updates UI states and navigation buttons accordingly.
        /// </summary>
        /// <param name="answer">The answer data containing text, correction, and images.</param>
        /// <param name="feedBackText">Feedback message to display ("Correct!" or "Incorrect!").</param>
        /// <param name="anwserCorrect">Indicates if the selected answer was correct.</param>
        private void ShowAnswerDetail(AnswerData answer, string feedBackText, bool anwserCorrect)
        {
            interactionState = InteractionState.ISCORRECTION;

            // Clear images
            foreach(GameObject go in answerGameObjectSprites)
            {
                go.SetActive(false);
            }

            // Load texts
            answerText.text = feedBackText;
            answerSelected.text = answer.answerText;

            // Set background color
            if (anwserCorrect)
            {
                backgroudAnswer.color = correctColor;
                doctorExpressionsImages.sprite = doctorSprite[0]; // Happy expression
            }
            else
            { 
                backgroudAnswer.color = incorrectColor;
                doctorExpressionsImages.sprite = doctorSprite[1]; // Sad expression
            }

            // Load correction text if not null
            if (answer.correctionText != "")
            {
                answerJustification.GetComponent<TextMeshProUGUI>().text = "<u><b>Justification :</b></u> " + answer.correctionText;
            }
            // Load image if not null
            if (answer.sprites.Count > 0)
            {
                int max = Mathf.Min(answer.sprites.Count, answerGameObjectSprites.Count);
                for (int i = 0; i < max; i++)
                {
                    answerGameObjectSprites[i].GetComponent<Image>().sprite = answer.sprites[i];
                    // Set gameobject actif
                    answerGameObjectSprites[i].SetActive(true);
                }
            }
            // Set GameObject active
            questionsDisplay.SetActive(false);
            correctionDisplay.SetActive(true);

            // Set bottom buttons
            SetTextButtonsNavigation();
        }

        /// <summary>
        /// Advances the game to the next step if the current step's diagnostic and action phases are completed.
        /// If the current step is marked as terminating, it triggers saving and displaying the player's scores.
        /// </summary>
        private void GoToNextStep()
        {
            // Control if dignostic & action is completed
            bool isStepCompleted = IsStepCompleted(patientData.steps[_indexStep]);

            if (isStepCompleted && patientData.steps[_indexStep].isTerminatingStep )
            {
                // Show player scores
                GameManager.Instance.GameStateManager.SaveShowScores();
            } else if (isStepCompleted && _indexStep < patientData.steps.Count)
            {
                // LOAD NEXT STEP 
                _indexStep++;
                print("Increased indexstep : " + _indexStep);
                GameManager.Instance.GameStateManager.NextStep(patientData.steps[_indexStep]);
            }   
        }


        /// <summary>
        /// Checks whether the current step's diagnostic and action phases are completed and valid.
        /// </summary>
        /// <param name="step">The current AlgoStep to check.</param>
        /// <returns>True if the step is completed, false otherwise.</returns>
        private bool IsStepCompleted(AlgoStep step)
        {
            bool diagnoticOK = !step.hasDiagnosticPhase || _isDiagnosticValid;
           
            return diagnoticOK && _isActionValid;
        }

        /// <summary>
        /// Initializes component references by searching through the displayList GameObjects,
        /// safely assigning components if they exist without causing errors.
        ///
        /// Also sets up button listeners for navigation and interaction,
        /// and initializes a dictionary mapping each Step enum to its corresponding display index.
        /// </summary>
        private void Awake()
        {

            // WARNING : don't trigger error if component not found. 
            foreach(GameObject go in displayList)
            {
                if (go.TryGetComponent<Step1PresentationPatient>(out Step1PresentationPatient component)) step1PresentationPatient = component;
                if (go.TryGetComponent<Step2WisperTest>(out Step2WisperTest component1)) step2WisperTest = component1;
                if (go.TryGetComponent<Step3And4Questionnary>(out Step3And4Questionnary component2)) step4And5Questionnary = component2;
                if (go.TryGetComponent<Step5Otoscopie>(out Step5Otoscopie component3)) Step5Otoscopie = component3;
                if (go.TryGetComponent<Step6WeberTest>(out Step6WeberTest component4)) step6HhiesTest = component4;
                if (go.TryGetComponent<Step7HhiesTest>(out Step7HhiesTest component5)) step7HhiesTest = component5;
                if (go.TryGetComponent<Step8Audiometrie>(out Step8Audiometrie component6)) Step8Audiometrie = component6;
            }


            //SET LISTENER
            confirmNextButton.onClick.AddListener(GoToQuestionDisplay);
            confirmNextButton.onClick.AddListener(GoToNextStep);
            returnButton.onClick.AddListener(BackToDocument);
            returnButton.onClick.AddListener(BackToQuestion);

            mappingDisplays = new Dictionary<Step, int>()
            {
                { Step.Case_presentation, 0 },
                { Step.Wisper_test, 1 },
                { Step.Questionnary, 2 },
                { Step.Additional_questionnaire, 2 },
                { Step.Otoscopy, 3 },
                { Step.Weber_test, 4 },
                { Step.HHIES_test, 5 },
                { Step.Audiometry, 6 },
            };
        }

    }
}
