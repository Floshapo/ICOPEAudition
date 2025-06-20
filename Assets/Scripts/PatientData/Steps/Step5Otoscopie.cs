using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.PatientData.Steps
{
    /// <summary>
    /// Manages display of otoscopy video and patient sprite images.
    /// </summary>
    public class Step5Otoscopie : MonoBehaviour
    {
        [Header("Patient video otoscopie field")]
        [SerializeField] private Image videoOtoscopiePatient;

        [Header("Patient sprite")]
        [SerializeField] private Image patient;

        /// <summary>
        /// Sets the otoscopy video sprite and patient sprite.
        /// </summary>
        /// <param name="sprite">Otoscopy video sprite.</param>
        /// <param name="spritePatient">Patient sprite.</param>
        public void SetImages(Sprite sprite, Sprite spritePatient)
        {
            if (sprite == null) return; 
            if (spritePatient == null) return;
            videoOtoscopiePatient.sprite = sprite;

            patient.sprite = spritePatient;
            patient.SetNativeSize();
        }
    }
}
