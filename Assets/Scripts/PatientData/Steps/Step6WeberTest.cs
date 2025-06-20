using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.PatientData.Steps
{
    /// <summary>
    /// Handles the Weber test step UI, including patient and doctor dialogues and patient image.
    /// </summary>
    public class Step6WeberTest : MonoBehaviour
    {
        [Header("Dialogue field")]
        [SerializeField] private TextMeshProUGUI patientDialogueField;
        [SerializeField] private TextMeshProUGUI doctorDialogueField;

        [Header("Patient images")]
        [SerializeField] private Image patient;

        private readonly string doctorDialogue = "De quel côté avez-vous entendu le son ?";

        /// <summary>
        /// Sets the dialogue text for the doctor and patient.
        /// </summary>
        /// <param name="patientContext">Patient's dialogue or context.</param>
        public void SetTextDialogue(string patientContext)
        {
            doctorDialogueField.text = doctorDialogue;
            patientDialogueField.text = patientContext;
        }

        /// <summary>
        /// Sets the patient sprite image.
        /// </summary>
        /// <param name="spritePatient">Sprite of the patient.</param>
        public void SetImage(Sprite spritePatient)
        {
            patient.sprite = spritePatient;
            patient.SetNativeSize();
        }
    }
}