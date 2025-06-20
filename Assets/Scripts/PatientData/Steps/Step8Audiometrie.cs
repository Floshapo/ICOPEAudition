using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.PatientData.Steps
{
    /// <summary>
    /// Manages the Audiometrie step UI, displaying the audiometry image and patient sprite.
    /// </summary>
    public class Step8Audiometrie : MonoBehaviour
    {
        [Header("Sprite patient")]
        [SerializeField] private Image patientImage;
        [Header("Sprite Audiometrie")]
        [SerializeField] private Image imageAudiometrie;

        /// <summary>
        /// Sets the audiometry and patient sprites for display.
        /// </summary>
        /// <param name="spriteAudio">The audiometry test image sprite.</param>
        /// <param name="spritePatient">The patient sprite.</param>
        public void SetSprite(Sprite spriteAudio, Sprite spritePatient)
        {
            imageAudiometrie.sprite = spriteAudio;
            patientImage.sprite = spritePatient;
            patientImage.SetNativeSize();
        }
    }
}