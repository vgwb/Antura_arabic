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
        public LetterData letter;
        public int associatedPromptIndex;
        public bool isRequired;
        public LetterObject LetterModel;
        public TMP_Text LetterView;

        private Vector3 mousePosition = new Vector3();
        private float cameraDistance;
        private bool drop;


        void Start()
        {
            cameraDistance = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
            RandomizeAnimation();
        }

        void Update()
        {
            if (drop)
            {
                transform.Translate(Vector3.down * Time.deltaTime * 50f);
            }

            if (transform.position.y < -10)
            {
                Destroy(gameObject);
            }
        }

        public void Init(LetterData _data)
        {
            LetterModel = new LetterObject(_data);
            LetterView.text = ArabicAlphabetHelper.GetLetterFromUnicode(LetterModel.Data.Isolated_Unicode);
        }

        void OnMouseDrag()
        {
            mousePosition = Input.mousePosition;
            mousePosition.z = cameraDistance;

            parentFloatingLetter.MoveHorizontally(Camera.main.ScreenToWorldPoint(mousePosition).x);
        }

        private void RandomizeAnimation()
        {
            animator.speed *= Random.Range(0.75f, 1.25f);
            animator.SetFloat("Offset", Random.Range(0f, BalloonsGameManager.instance.letterAnimationLength));
        }

        public void Drop()
        {
            StartCoroutine(Drop_Coroutine(BalloonsGameManager.instance.letterDropDelay)); 
        }

        private IEnumerator Drop_Coroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            drop = true;
        }
    }
}