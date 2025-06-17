using Assets.Scripts.PatientData;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.PatientData
{
    [CreateAssetMenu(fileName = "LevelsData", menuName = "Medical/LevelsData")]
    public class LevelsData : ScriptableObject
    {
        public List<PatientCaseData> patientByLevel;
    }


    [System.Serializable]
    public class PatientCaseData
    {
        public string levelName;
        public List<NewPatientData> patientsCase;
    }
}