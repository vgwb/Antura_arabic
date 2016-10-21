using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EA4S;
using TMPro;

namespace Balloons
{
    public class LetterController : MonoBehaviour
    {
        public FloatingLetterController parentFloatingLetter;
        public Animator animator;
        public Collider letterCollider;
        public Rigidbody body;
        public LetterData letter;
        public int associatedPromptIndex;
        public bool isRequired;
        public LLController LetterModel;
        public TMP_Text LetterView;

        [Header("Letter Parameters")]
        [Tooltip("e.g: true")]
        public bool spinEnabled;
        [Range(0, 5)] [Tooltip("e.g.: 1")]
        public float spinSpeed;
        [Range(0, 360)] [Tooltip("e.g.: 90")]
        public float spinAngle;
        [Range(0, 5)] [Tooltip("e.g.: 0.25")]
        public float spinRandomnessFactor;

        [HideInInspector]
        public bool keepFocusingLetter = false;

        private bool keepSpinning;
        private float spinDirection = 1f;
        private float randomOffset = 0f;
        private Vector3 baseRotation;
        private Vector3 mousePosition = new Vector3();
        private float cameraDistance;
        private bool drop;
        private float focusDuration = 1f;
        private float focusProgress;
        private float focusProgressPercentage;
        private float unfocusDuration = 1f;
        private float unfocusProgress;
        private float unfocusProgressPercentage;


        void Start()
        {
            cameraDistance = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
            baseRotation = transform.rotation.eulerAngles;
            keepSpinning = spinEnabled;
            RandomizeSpin();
            RandomizeAnimation();
        }

        void Update()
        {
            Spin();

            if (keepFocusingLetter)
            {
                FocusLetter();
            }

            if (transform.position.y < -10)
            {
                Destroy(gameObject);
            }
        }

        public void Init(LetterData _data)
        {
            LetterModel = new LLController(_data);
            LetterView.text = _data.TextForLivingLetter; 
        }

        void OnMouseDown()
        {
            SpeakLetter();
            FocusLetter();

            mousePosition = Input.mousePosition;
            mousePosition.z = cameraDistance;

            parentFloatingLetter.MouseOffset = parentFloatingLetter.transform.position - Camera.main.ScreenToWorldPoint(mousePosition);
        }

        void OnMouseDrag()
        {
            FocusLetter();

            mousePosition = Input.mousePosition;
            mousePosition.z = cameraDistance;

            parentFloatingLetter.Drag(Camera.main.ScreenToWorldPoint(mousePosition));
        }

        void OnMouseUp()
        {
            ResetLetterFocusingParameters();
            ResetLetterUnfocusingParameters();
            parentFloatingLetter.ResetFocusingParameters();
        }

        private void RandomizeSpin()
        {
            randomOffset = Random.Range(0, 2 * Mathf.PI);
            spinSpeed += Random.Range(-spinRandomnessFactor * spinSpeed, spinRandomnessFactor * spinSpeed);
            spinAngle += Random.Range(-spinRandomnessFactor * spinAngle, spinRandomnessFactor * spinAngle);
            spinDirection *= (Random.Range(0, 2) > 0 ? -1 : 1);
        }

        private void RandomizeAnimation()
        {
            animator.speed *= Random.Range(0.75f, 1.25f);
            animator.SetFloat("Offset", Random.Range(0f, BalloonsGameManager.instance.letterAnimationLength));
        }

        private void SpeakLetter()
        {
            if (LetterModel != null && LetterModel.Data != null && LetterModel.Data.Key != null)
            {
                AudioManager.I.PlayLetter(LetterModel.Data.Key);
            }
        }

        private void FocusLetter()
        {
            keepSpinning = false;
            //transform.rotation = Quaternion.Euler(baseRotation);
            parentFloatingLetter.Focus();

            if (focusProgress < focusDuration)
            {
                focusProgress += Time.deltaTime;
                focusProgressPercentage = focusProgress / focusDuration;
            }
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(baseRotation), focusProgressPercentage);
        }

        public void ResetLetterFocusingParameters()
        {
            focusProgress = 0f;
            focusProgressPercentage = 0f;
        }

        public void ResetLetterUnfocusingParameters()
        {
            unfocusProgress = 0f;
            unfocusProgressPercentage = 0f;
        }

        private void Spin()
        {
            if (keepSpinning)
            {
                if (unfocusProgress < unfocusDuration)
                {
                    unfocusProgress += Time.deltaTime;
                    unfocusProgressPercentage = unfocusProgress / unfocusDuration;
                }
                var spinRotation = Quaternion.Euler(baseRotation.x, baseRotation.y + spinDirection * spinAngle * Mathf.Sin(spinSpeed * Time.time + randomOffset), baseRotation.z); 
                transform.rotation = Quaternion.Lerp(transform.rotation, spinRotation, unfocusProgressPercentage);
            }
            else
            {
                keepSpinning = spinEnabled;
            }
        }

        public void Drop()
        {
            StartCoroutine(Drop_Coroutine(BalloonsGameManager.instance.letterDropDelay)); 
        }

        private IEnumerator Drop_Coroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            AudioManager.I.PlaySfx(Sfx.LetterAngry);
            //drop = true;
            var dropSpeed = 2500f;
            body.AddForce(Vector3.down * dropSpeed);
        }

        public void DisableCollider()
        {
            letterCollider.enabled = false;
        }
    }
}