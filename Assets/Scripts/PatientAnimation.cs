using Assets.Scripts.Managers;
using DG.Tweening;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class PatientAnimation : MonoBehaviour
    {
        // Sprite List
        [Header("Sprite characters list")]
        [SerializeField] private Sprite[] characterSprites;
        // Animation size variable
        [SerializeField] private float scaleFactor = 1.15f;
        [SerializeField] private float animationDuration = 2f;
       
        private Vector2 defaultImageSize; 
        
        // Interaction Area Variable
        [Header("Patient selection area")]
        [SerializeField] private RectTransform interactionArea;
        [SerializeField] private RectTransform imageCharacter;
       
        private Vector2 targetPosition;

        [Header("Animation patient area")]
        [SerializeField] public RectTransform spawnPatientArea;
        
        private readonly float imageRatio = 1.25f;
        
        [Header("Doors")]
        [SerializeField] private RectTransform leftDoor;
        [SerializeField] private RectTransform rightDoor;

        // Doors variables
        public float openAngle = -90f;
        public float duration = 0.5f;
        private bool isOpen = false;

        [Header("FadeAnimation")]
        [SerializeField] public CanvasGroup fadePanel;


        /// <summary>
        /// Set a new target position to the sprite to stimule life in the UI.
        /// </summary>
        private void SetNewTargetPosition(RectTransform targetArea)
        {
            // Set the sprite to new target (spawnArea & interactionArea)
            if (imageCharacter.transform.parent != targetArea)
            {
                imageCharacter.SetParent(targetArea);
            }

            // Reduce the width and height of the image in the spawnArea else we keep the default size of the image.
            if (targetArea.name == "SpawnArea")
            {
                defaultImageSize = imageCharacter.sizeDelta;
                imageCharacter.sizeDelta = defaultImageSize / imageRatio;
            }
            else
            {
                imageCharacter.sizeDelta = defaultImageSize;
            }

            // Move the sprite around the area
            float panelWidth = targetArea.GetComponent<RectTransform>().rect.width;
            float panelHeight = targetArea.GetComponent<RectTransform>().rect.height;

            float imageWidth = imageCharacter.rect.width;
            float imageHeight = imageCharacter.rect.height;

            float randomX = Random.Range(-panelWidth / 2 + imageWidth / 2, panelWidth / 2 - imageWidth / 2);
            float randomY = Random.Range(-panelHeight / 2 + imageHeight / 2, panelHeight / 2 - imageHeight / 2);

            targetPosition = new Vector2(randomX, randomY);

            imageCharacter.anchoredPosition = targetPosition;
        }

        /// <summary>
        /// Set new sprite to stimule life.
        /// </summary>
        private void SetNewSprite()
        {
            if (characterSprites == null)
            {
                Debug.LogError("no sprite set in the sprites library.");
                return;
            }

            int randIndex = Random.Range(0, characterSprites.Length - 1);
            Sprite newSprite = characterSprites[randIndex];
            imageCharacter.GetComponent<Image>().sprite = newSprite;
            // Set the gameObject rectTransfor with the new sprite size
            imageCharacter.sizeDelta = new Vector2(newSprite.rect.width, newSprite.rect.height);
        }

        /// <summary>
        /// Sets the sprite image for the character and adjusts the image size to match the sprite's dimensions.
        /// Logs an error if the sprite library is not initialized.
        /// </summary>
        /// <param name="characterSprite">The sprite to display for the character.</param>
        private void SetSprite(Sprite characterSprite)
        {
            if (characterSprites == null)
            {
                Debug.LogError("no sprite set in the sprites library.");
                return;
            }

            imageCharacter.GetComponent<Image>().sprite = characterSprite;
            imageCharacter.sizeDelta = new Vector2(characterSprite.rect.width, characterSprite.rect.height);
        }

        /// <summary>
        /// Make a yo-yo animation on the sprite.
        /// </summary>
        private void AnimationSizeImage()
        {
            RectTransform rectTransform = imageCharacter;
            rectTransform.DOSizeDelta(rectTransform.sizeDelta * scaleFactor, animationDuration / 2).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine).SetId("sizeAnim");
        }

        /// <summary>
        /// Stop the Dotween Yo-yo animation on the character sprite.
        /// </summary>
        public static void StopAnimation()
        {
            DOTween.Kill("sizeAnim");
        }

        /// <summary>
        /// Open/Close doors between 0 and a targetAngle (openAngle)
        /// </summary>
        public void ToggleDoor()
        {
            // move door postion and mor angle (-145/145 degree)
            float targetAngle = isOpen ? 0f : openAngle;
            rightDoor.rotation = Quaternion.Euler(0, targetAngle, 0);
            leftDoor.rotation = Quaternion.Euler(0, -targetAngle, 0);
            isOpen = !isOpen;
        }
        
    
        /// <summary>
        /// Play a sort of animations like open the doors and spawn the 'patient' in the doors area then fade out the screen by invoking 'FadeOut' function after a delai.
        /// </summary>
        public void SetNewCharacterInArea(Sprite sprite)
        {
            imageCharacter.GetComponent<Button>().enabled = false;
            ToggleDoor();
            //SetNewSprite();
            SetSprite(sprite);
            SetNewTargetPosition(spawnPatientArea);
            Invoke(nameof(FadeOut), 1.5f);
        }

        /// <summary>
        /// Fade out animation with DOTWeen and invoking 'ContinueSetNewCharacter' function after a short delai 1 sec.
        /// Black to transparency
        /// </summary>
        private void FadeOut()
        {
            fadePanel.DOFade(0f, 1f).SetEase(Ease.Linear);
            Invoke(nameof(ContinueSetNewCharacter), 1f);
        }


        /// <summary>
        /// Play the last animations like close door and set new patient in the interaction area then invoking 'FadeIn' function after a delai.
        /// </summary>
        private void ContinueSetNewCharacter()
        {
            ToggleDoor();
            SetNewTargetPosition(interactionArea);
            AnimationSizeImage();
            Invoke(nameof(FadeIn), 1f);
        }
        /// <summary>
        /// Fade in animation.
        /// Transparency to black.
        /// </summary>
        private void FadeIn()
        {
            fadePanel.DOFade(1f, 1f).SetEase(Ease.Linear);
            imageCharacter.GetComponent<Button>().enabled = true;
        }
    }
}