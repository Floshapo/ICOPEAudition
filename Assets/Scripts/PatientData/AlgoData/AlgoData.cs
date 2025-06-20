using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.PatientData.AlgoData
{
    /// <summary>
    /// AlgoData is a ScriptableObject that stores a list of AlgoStep objects,
    /// representing the sequential steps in a medical algorithm or workflow.
    /// This asset can be created in the Unity Editor via the "Medical/AlgoData" menu.
    /// </summary>
    [CreateAssetMenu(fileName = "AlgoData", menuName = "Medical/AlgoData")]
    public class AlgoData : ScriptableObject
    {
        public List<AlgoStep> steps;
    }
}