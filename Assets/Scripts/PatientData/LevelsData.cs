using Assets.Scripts.PatientData;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.PatientData
{
    /// <summary>
    /// ScriptableObject storing patient cases organized by levels.
    /// </summary>
    [CreateAssetMenu(fileName = "LevelsData", menuName = "Medical/LevelsData")]
    public class LevelsData : ScriptableObject
    {
        public List<PatientCaseData> patientByLevel;
    }

    /// <summary>
    /// Represents a collection of patient cases for a specific level.
    /// </summary>
    [System.Serializable]
    public class PatientCaseData
    {
        public string levelName;
        public List<NewPatientData> patientsCase;
    }
}