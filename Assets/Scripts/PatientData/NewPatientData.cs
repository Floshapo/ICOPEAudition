using Assets.Scripts.PatientData.AlgoData;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.PatientData
{
    /// <summary>
    /// ScriptableObject representing detailed patient data for medical scenarios.
    /// </summary>
    [CreateAssetMenu(fileName = "NewPatientData", menuName = "Medical/Patient")]
    public class NewPatientData : ScriptableObject
    {
        [Header("Profil")]
        public Sprite[] characterSprites;
        public string surname;
        public string firstName;
        public int age;

        [Header("Family Situation")]
        public string familySituation;

        [Header("Occupational Activities")]
        public string occupationalActivities;

        [Header("Context")]
        [TextArea]
        public string context;

        [Header("Autonomies")]
        public List<NamedValue> autonomies;

        [Header("Medical History")]
        [TextArea]
        public string medicalHistory;
        
        [TextArea, Header("Description level")] public string descriptionLevel;

        [Header("Algoritm steps")]
        public List<AlgoStep> steps;

    }

    /// <summary>
    /// Simple name-value pair structure.
    /// </summary>
    [System.Serializable]
    public class NamedValue
    {
        public string name;
        public string value;
    }
}
