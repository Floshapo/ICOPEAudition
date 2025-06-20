using Assets.Scripts.PatientData.AlgoData;
using DG.Tweening;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.PatientData.Steps
{
    /// <summary>
    /// Manages the Whisper Test step UI, including doctor dialogue animations and patient sprite display.
    /// </summary>
    public class Step2WisperTest : MonoBehaviour
    {
        // DOCTOR POSITION
        [Header("Doctor position")]
        [SerializeField] private GameObject doctorPos1;
        [SerializeField] private GameObject doctorPos2;

        // DOCTOR TEXT
        [Header("Doctor texts")]
        [SerializeField] private GameObject goDoctorText1;
        [SerializeField] private GameObject goDoctorText2;
        [SerializeField] private TextMeshProUGUI targerDoctorText1;
        [SerializeField] private TextMeshProUGUI targerDoctorText2;

        // PATIENT SPRITE
        [Header("Patient sprite")]
        [SerializeField] private Image patientSprite;

        // PATIENT TEXT
        [Header("Patient text")]
        [SerializeField] private GameObject goPatientText;
        [SerializeField] private TextMeshProUGUI targetPatientText;

        // Animation timing delays
        [Header("Delay animation")]
        [SerializeField] private float delayBetweenWords = 2f;
        [SerializeField] private float delayBetweenText = 2f;


        // Words spoken by the doctor (can be moved to ScriptableObject for configurability)
        private readonly string[] doctorWords = { "Ami", "Bateau", "Bureau", "Chameau", "Cheval", "Hibou", "Journal", "Lama", "Lapin", "Moto", "Mouton", "Parfait", "Pompier", "Salon", "Serpent"};
        private string patientText;

        // Clears all text UI fields
        private void ClearTexts()
        {
            targerDoctorText1.text = "";
            targerDoctorText2.text = "";
            targetPatientText.text = "";
        }

        // Disables all dialogue boxes
        private void ClearDialogueBox()
        {
            goDoctorText1.SetActive(false);
            goDoctorText2.SetActive(false);
            goPatientText.SetActive(false);
        }

        // Hides doctor position sprites
        private void ClearDoctorSprite()
        {
            doctorPos1.SetActive(false);
            doctorPos2.SetActive(false);
        }

        // Returns an array of 4 random words from doctorWords
        private string[] GetRandomListWord()
        {
            string[] strings = new string[4];
            
            for (int i = 0; i < strings.Length; i++)
            {
                int nRandom = Random.Range(0, doctorWords.Length);
                strings[i] = doctorWords[nRandom];
            }

            return strings;
        }

        /// <summary>
        /// Animates the display of words sequentially in the given TextMeshProUGUI target.
        /// </summary>
        /// <param name="goTarget">GameObject containing the text to show/hide.</param>
        /// <param name="target">TextMeshProUGUI component to update.</param>
        /// <param name="words">Words to animate.</param>
        /// <param name="startDelay">Delay before starting animation.</param>
        /// <param name="onComplete">Callback after animation completes.</param>
        private void AnimateText(GameObject goTargert, TextMeshProUGUI target, string[] words, float startDelay = 0f, TweenCallback onComplete = null)
        {
            target.text = "";
            goTargert.SetActive(false);

            for (int i = 0; i < words.Length; i++)
            { 
                string word = words[i];
                float delay = startDelay + i * delayBetweenWords;

                DOVirtual.DelayedCall(delay, () =>
                {
                    target.text = word;
                    goTargert.SetActive(true);
                });

                float hideDelay = delay + delayBetweenWords * 0.8f;
                DOVirtual.DelayedCall(hideDelay, () =>
                {
                    goTargert.SetActive(false);
                });
            }

            if (onComplete != null)
            {
                float totalTime = startDelay + words.Length * delayBetweenWords;
                DOVirtual.DelayedCall(totalTime, onComplete);
            }
        }

        /// <summary>
        /// Sets the patient sprite for display.
        /// </summary>
        public void SetPatient(Sprite patient)
        {
            if (patient == null) return;
                
            patientSprite.sprite = patient;
            patientSprite.SetNativeSize();
        }

        /// <summary>
        /// Starts the first doctor text animation and queues subsequent animations.
        /// </summary>
        public void PlayFirstText(AlgoStep step)
        {
            // Load in memory patient text form algoStep
            patientText = step.contextDescription;

            // Clear texts & docotor sprite
            ClearTexts();
            ClearDialogueBox();
            ClearDoctorSprite();

            // Activate doctor sprite position 1
            doctorPos1.SetActive(true);

            string[] strings = GetRandomListWord();
            AnimateText(goDoctorText1, targerDoctorText1, strings, 0f, () =>
            {
                DOVirtual.DelayedCall(delayBetweenText, PlaySecondText);
            });
        }

        /// <summary>
        /// Plays the second doctor text animation then shows patient text.
        /// </summary>
        private void PlaySecondText()
        {
            ClearTexts();
            ClearDialogueBox();
            ClearDoctorSprite();
            
            doctorPos2.SetActive(true);

            string[] strings = GetRandomListWord();
            AnimateText(goDoctorText2 ,targerDoctorText2, strings, 0f, () =>
            {
                DOVirtual.DelayedCall(delayBetweenText, ShowPatientText);
            });
        }

        /// <summary>
        /// Displays the patient's text in the UI.
        /// </summary>
        private void ShowPatientText()
        {
            ClearTexts();
            ClearDialogueBox();

            goPatientText.SetActive(true);
            targetPatientText.text = patientText;
        }
    }
}
