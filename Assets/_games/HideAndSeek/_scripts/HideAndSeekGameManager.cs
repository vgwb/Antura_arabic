using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
		void Start ()
        {
            for(int i = 0; i < 8; ++i)
            {
                UsedPlaceholder[i] = false;
            }
        }
	
		// Update is called once per frame
		void Update ()
        {
            if (StartNewRound && game.inGame && Time.time > time + timeToWait)
            {

                ClearRound();

                NewRound();
            }

        }

		void MoveObject(int id){
            if (ArrayLetters.Length > 0)
            {
                script = ArrayLetters[GetIdFromPosition(id)].GetComponent<HideAndSeekLetterController>();
                script.Move();
            }

		}

        int GetIdFromPosition(int index)
        {
            for(int i = 0; i < ArrayLetters.Length; ++i)
            {
                if (ArrayLetters[i].GetComponent<HideAndSeekLetterController>().id == index)
                    return i;
            }
            return -1;
        }
		void CheckResult(int id)
		{

            if (ArrayLetters[GetIdFromPosition(id)].GetComponent<HideAndSeekLetterController>().view.Model.Data.Key == currentQuestion.GetAnswer().Key)
            {
                Debug.Log("Hai vintooooooo");
                //feedback o animazione vittoria
                StartNewRound = true;
                SetTime();
            }
            else
            {
                Debug.Log("Hai sbagliato");
                // togliere simbolo vita
                //feedback
                if(--lives == 0)
                {
                    StartNewRound = true;
                    // mandare lettera sbagliata a teacher?
                    SetTime();
                }
            }
                
        }

        public void SetTime()
        {
            time = Time.time;
        }


        public void ClearRound()
        {
            for(int i = 0; i < MAX_TREE; ++i)
            {
                ArrayTrees[i].GetComponent<MeshCollider>().enabled = false;
                UsedPlaceholder[i] = false;
            }
            
        }

        public void NewRound()
        {
            currentQuestion = (HideAndSeekQuestionsPack)questionProvider.GetQuestion();
            StartNewRound = false;
            lives = 3;
            FreePlaceholder = MAX_TREE;
            ActiveLetters = currentQuestion.GetLetters().Count;

            List<ILivingLetterData> letterList = currentQuestion.GetLetters();

            // metodo dato active letters ci restituisce una lista di placeholders(tra quelli di arrayplaceholder)
            // posiziono lettera tramite placeh, setto il giusto id e attivo colliders alberi (che aggiungo alla loro lista)

            for(int i = 0; i < ActiveLetters; ++i)
            {
                int index = getRandomPlaceholder();
                if(index != -1)
                {
                    
                    ArrayTrees[index].GetComponent<MeshCollider>().enabled = true;
                    
                    ArrayLetters[i].transform.position = ArrayPlaceholder[index].transform.position;
                    HideAndSeekLetterController scriptComponent = ArrayLetters[i].GetComponent<HideAndSeekLetterController>();
                    scriptComponent.SetStartPosition();
                    scriptComponent.id = index;
                    ArrayLetters[i].GetComponentInChildren<LetterObjectView>().Init(letterList[i]);
                }
                
            }
            

            AudioManager.I.PlayLetter(currentQuestion.GetAnswer().Key);

        }

        public int getRandomPlaceholder()
        {
            int result = 0;
            int position = Random.Range(0, --FreePlaceholder);
            
            for(int i = 0; i < UsedPlaceholder.Length; ++i)
            {
                if (UsedPlaceholder[i] == true)
                    continue;
                if (result == position)
                {
                    UsedPlaceholder[i] = true;
                    return i;
                }
                    
                result++;
            }

            return -1;
        }  


        //var
        bool StartNewRound = true;
        int lives;
        int ActiveLetters;
        private const int MAX_TREE = 8;
        private int FreePlaceholder;

		public GameObject[] ArrayTrees;
        
        public Transform[] ArrayPlaceholder;
        private bool[] UsedPlaceholder = new bool[8];

		public GameObject[] ArrayLetters;
        

		private HideAndSeekLetterController script;

        public HideAndSeekGame game;


        public HideAndSeekQuestionsProvider questionProvider;
        public HideAndSeekQuestionsPack currentQuestion;

        public float timeToWait = 1.0f;
        private float time;
    }
}