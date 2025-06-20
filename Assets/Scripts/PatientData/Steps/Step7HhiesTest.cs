using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.PatientData.Steps
{
    /// <summary>
    /// Manages the HHIES test step UI, displaying the HHIES image and the patient sprite.
    /// </summary>
    public class Step7HhiesTest : MonoBehaviour
    {
        [Header("Sprite patient")]
        [SerializeField] private Image patient;
        [Header("Sprite HHIES")]
        [SerializeField] private Image imageHHIES;

        /// <summary>
        /// Sets the sprites for the HHIES image and the patient.
        /// </summary>
        /// <param name="sprite">HHIES test image sprite.</param>
        /// <param name="spritePatient">Patient sprite.</param>
        public void SetImages(Sprite sprite, Sprite spritePatient)
        {
            imageHHIES.sprite = sprite;
            patient.sprite = spritePatient;
            patient.SetNativeSize();
        }
    }
}
