using Assets.Scripts.Managers;
using Assets.Scripts.PatientData.AlgoData;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.GameData;

namespace Assets.Scripts.UI.ScoreContents
{
    [System.Serializable]
    public enum FieldsName { 
        PatientName, 
        StepSuccess, 
        StepFailed, 
        DiagnoticsSuccess, 
        DiagnosticsFailed, 
        ActionSuccess, 
        ActionFailed, 
        Scores,
        SuccessRate,
        StepName,
        DiagnosticsDetails,
        ActionsDetails
    }

    
    [System.Serializable]
    public class FieldsTable
    {
        public FieldsName fieldsName;
        public TextMeshProUGUI fields;
    }

    [System.Serializable]
    public class DetailsFields
    {
        public FieldsName fieldsName;
        public TextMeshProUGUI fields;
        public Image image;
    }


    public class PlayerScoreDisplayManager : MonoBehaviour
    {
        [Header("Section synthese patient")]
        [SerializeField] private List<FieldsTable> fieldsList;

        [Header("Score calcul")]
        [SerializeField] private static int _stepMulticateur = 10;
        [SerializeField] private static int _diagActionMulticateur = 5;

        [Header("Script Open folder")]
        [SerializeField] private OpenFolder openFolder;

        // ---- PRIVATES VARIABLES ----
        private string _currentPatientName;
        private List<Step> stepsList;

        private static int CalculateTotalScore(int nbStepSucc, int nbStepFailed, int nbDiagSucc, int nbDiagFailed, int nbActionSucc, int nbActionFailed) 
        {  
            return nbStepSucc * _stepMulticateur - nbStepFailed * _stepMulticateur + nbDiagSucc * _diagActionMulticateur - nbDiagFailed * _diagActionMulticateur + nbActionSucc * _diagActionMulticateur + nbActionFailed * _diagActionMulticateur;
        }

        private void SetSyntheseScore(string patientName, int nbStepSucc, int nbStepFailed, int nbDiagSucc, int nbDiagFailed, int nbActionSucc, int nbActionFailed, float succesRate)
        {
            foreach (FieldsTable fieldsTable in fieldsList)
            {
                switch (fieldsTable.fieldsName)
                {
                    case FieldsName.PatientName:
                        fieldsTable.fields.text = patientName;
                        break;
                    case FieldsName.StepSuccess:
                        fieldsTable.fields.text = nbStepSucc.ToString();
                        break;
                    case FieldsName.StepFailed:
                        fieldsTable.fields.text = nbStepFailed.ToString();
                        break; 
                    case FieldsName.DiagnoticsSuccess:
                        fieldsTable.fields.text = nbDiagSucc.ToString();
                        break;
                    case FieldsName.DiagnosticsFailed:
                        fieldsTable.fields.text = nbDiagFailed.ToString();
                        break;
                    case FieldsName.ActionSuccess: 
                        fieldsTable.fields.text = nbActionSucc.ToString();
                        break;
                    case FieldsName.ActionFailed: 
                        fieldsTable.fields.text = nbActionFailed.ToString();
                        break;
                    case FieldsName.Scores:
                        fieldsTable.fields.text = CalculateTotalScore(nbStepSucc, nbStepFailed, nbDiagSucc, nbDiagFailed, nbActionSucc, nbActionFailed).ToString();
                        break;
                    case FieldsName.SuccessRate:
                        fieldsTable.fields.text = succesRate.ToString();
                        break;
                }
            }
        }

        public void GetSetDisplayScore(string patientName)
        {
            // Get Patient data form patient score
            var pRecords = GameManager.Instance.GameData.GetPatientCaseRecords(patientName);
            _currentPatientName = patientName;

            SetSyntheseScore(patientName, pRecords.numberStepSucceed, pRecords.numberStepFailed, pRecords.numberDiagCorrect, pRecords.numberDiagIncorrect, pRecords.numberActionCorrect, pRecords.numberActionIncorrect, pRecords.successRate);           
            openFolder.SetStepsRecords(pRecords.stepRecords);
        }

        //CALL BY 'RETURN TO WAITING ROOM' BUTTON FROM SCORE CONTENT
        public static void GoToMenu()
        {
            GameManager.Instance.GameStateManager.NextPatientCase();
        }
    }
}