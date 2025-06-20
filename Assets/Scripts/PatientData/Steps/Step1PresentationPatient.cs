using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.PatientData.Steps
{
    /// <summary>
    /// Manages the UI presentation of patient data in Step 1 of the algorithm.
    /// </summary>
    public class Step1PresentationPatient : MonoBehaviour
    {
        [Header("Sprites")]
        [SerializeField] private Image patientSprite;
        
        // PROFILE DATA
        [Header("Profile fields")]
        [SerializeField] private TextMeshProUGUI nameFields;
        [SerializeField] private TextMeshProUGUI surnameFields;
        [SerializeField] private TextMeshProUGUI ageFields;
        [SerializeField] private TextMeshProUGUI situationFields;
        [SerializeField] private TextMeshProUGUI activitiesFields;

        // CONTEXT DATA
        [Header("Context field")]
        [SerializeField] private TextMeshProUGUI contextField;

        // MEDICAL AUTONOMIE DATA
        [Header("Medical fields")]
        [SerializeField] private TextMeshProUGUI adlField;
        [SerializeField] private TextMeshProUGUI iadlField;

        // MEDICAL HISTORY DATA
        [Header("Medical history field")]
        [SerializeField] private TextMeshProUGUI historyField;

        /// <summary>
        /// Populates UI text fields with patient data.
        /// </summary>
        /// <param name="patientData">Data about the patient to display.</param>
        public void SetPresentationTexts(NewPatientData patientData)
        {
            // Set profil data
            nameFields.text = patientData.surname;
            surnameFields.text = patientData.firstName;
            ageFields.text = patientData.age.ToString();
            situationFields.text = patientData.familySituation;
            activitiesFields.text = patientData.occupationalActivities;

            // Set contexte data
            contextField.text = patientData.context;

            // Set autonomie data
            adlField.text = patientData.autonomies[0].value;
            iadlField.text = patientData.autonomies[1].value;

            // Set medical history data
            historyField.text = patientData.medicalHistory;
        }

        /// <summary>
        /// Sets and adjusts the patient's sprite image.
        /// </summary>
        /// <param name="patient">The sprite representing the patient.</param>
        public void SetSprites(Sprite patient)
        {
            patientSprite.sprite = patient;
            patientSprite.SetNativeSize();
        }
    }
}
