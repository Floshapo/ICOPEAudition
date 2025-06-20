using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.PatientData.AlgoData
{
    /// <summary>
    /// Simple enum to represent a Yes or No response.
    /// </summary>
    public enum YesNo {Yes, no }

    /// <summary>
    /// ScriptableObject holding a list of questions for a questionnaire.
    /// </summary>
    [CreateAssetMenu(fileName = "QuestionnaireData", menuName = "Medical/QuestionnaireData")]
    public class QuestionnaireData : ScriptableObject
    {
        public List<QuestionData> questions;
    }

    /// <summary>
    /// Serializable class representing a single question, with the question text.
    /// </summary>
    [System.Serializable]
    public class QuestionData
    {
        [TextArea]
        public string questionText;
    }
}
