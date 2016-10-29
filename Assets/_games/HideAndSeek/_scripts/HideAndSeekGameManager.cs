using UnityEngine;
using System.Collections;

namespace EA4S.HideAndSeek
{
	public class HideAndSeekGameManager : MonoBehaviour {
		void OnEnable(){
			HideAndSeekTreeController.onTreeTouched += MoveObject;
			HideAndSeekLetterController.onLetterTouched += CheckResult;
		}
		void OnDisable(){
			HideAndSeekTreeController.onTreeTouched -= MoveObject;
			HideAndSeekLetterController.onLetterTouched -= CheckResult;
		}
		// Use this for initialization
		void Start () {

		}
	
		// Update is called once per frame
		void Update () {
	
		}
		void MoveObject(int id){
            if (ArrayLetters.Length > 0)
            {
                script = ArrayLetters[id].GetComponent<HideAndSeekLetterController>();
                script.Move();
            }

		}
		void CheckResult(int id)
		{
			Debug.Log (id);
		}

        void Init()
        {
            //TODO NOW
            //ArrayLetters[0].GetComponent<HideAndSeekLetterController>().SetQuestionText(game.QuestionsManager.currentQuestion.GetAnswer());
        }

		//var
		public GameObject[] ArrayTrees; 
		public GameObject[] ArrayLetters;

		private Transform[] rt = new Transform[2];
		private HideAndSeekLetterController script;

        public HideAndSeekGame game;
	}
}