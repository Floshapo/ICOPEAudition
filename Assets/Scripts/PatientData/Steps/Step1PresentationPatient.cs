using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.PatientData.Steps
{
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

        public void SetSprites(Sprite patient)
        {
            patientSprite.sprite = patient;
            patientSprite.SetNativeSize();
        }
    }
}
