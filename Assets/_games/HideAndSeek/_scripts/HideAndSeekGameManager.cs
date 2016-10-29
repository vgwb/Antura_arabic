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

        void NewRoundSetup()
        {
           
            StartNewRound = true;
            SetTime();
            WidgetPopupWindow.I.Close();
            
        }

        private IEnumerator DelayAnimation(bool answer)
        {
            

            var initialDelay = 1f;
            yield return new WaitForSeconds(initialDelay);
            
            game.PlayState.gameTime.Stop();

            if(answer)
                WidgetPopupWindow.I.ShowSentenceWithMark(NewRoundSetup, "comment_welldone", true, image);
            else
                WidgetPopupWindow.I.ShowSentenceWithMark(NewRoundSetup, "comment_welldone", false, image);


        }

        void CheckResult(int id)
		{

            if (ArrayLetters[GetIdFromPosition(id)].GetComponent<HideAndSeekLetterController>().view.Model.Data.Key == currentQuestion.GetAnswer().Key)
            {
                ClearRound();
                StartCoroutine(DelayAnimation(true));
                
                Debug.Log("Hai vintooooooo");
            }
            else
            {
                Debug.Log("Hai sbagliato");
                RemoveLife();
                if(lifes == 0)
                {
                    ClearRound();
                    StartCoroutine(DelayAnimation(false));
                }
            }
                
        }

        void RemoveLife()
        {
            switch (--lifes)
            {
                case 2:
                    LifeSprite[0].SetActive(false);
                    break;
                case 1:
                    LifeSprite[1].SetActive(false);
                    break;
                case 0:
                    LifeSprite[2].SetActive(false);
                    break;
            }

        }

        void SetFullLife()
        {
            lifes = 3;

            for (int i = 0; i < LifeSprite.Length; ++i)
            {
                LifeSprite[i].SetActive(true);
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
                ArrayLetters[i].transform.position = originLettersPlaceholder.position;
                ArrayTrees[i].GetComponent<MeshCollider>().enabled = false;
                UsedPlaceholder[i] = false;
            }
            
        }

        public void NewRound()
        {
            currentQuestion = (HideAndSeekQuestionsPack)questionProvider.GetQuestion();
            StartNewRound = false;
            SetFullLife();
            FreePlaceholder = MAX_TREE;
            ActiveLetters = currentQuestion.GetLetters().Count;

            ActiveTrees = new List<GameObject>();

            List<ILivingLetterData> letterList = currentQuestion.GetLetters();

            // metodo dato active letters ci restituisce una lista di placeholders(tra quelli di arrayplaceholder)
            // posiziono lettera tramite placeh, setto il giusto id e attivo colliders alberi (che aggiungo alla loro lista)

            for(int i = 0; i < ActiveLetters; ++i)
            {
                int index = getRandomPlaceholder();
                if(index != -1)
                {

                    ActiveTrees.Add(ArrayTrees[index]);
       
                    //set bool for letters for correct/wrong anim
                    ArrayLetters[i].transform.position = ArrayPlaceholder[index].transform.position;
                    HideAndSeekLetterController scriptComponent = ArrayLetters[i].GetComponent<HideAndSeekLetterController>();
                    scriptComponent.SetStartPosition();
                    scriptComponent.id = index;
                    ArrayLetters[i].GetComponentInChildren<LetterObjectView>().Init(letterList[i]);
                }
                
            }
            
            WidgetPopupWindow.I.ShowSentence(BeginRound, "comment_welldone", image);

        }

        void BeginRound()
        {
            StartCoroutine(DisplayRound_Coroutine());
        }


        private IEnumerator DisplayRound_Coroutine()
        {
            WidgetPopupWindow.I.Close();

            foreach(GameObject tree in ActiveTrees)
            {
                tree.GetComponent<MeshCollider>().enabled = true;
            }

            var winInitialDelay = 1f;
            yield return new WaitForSeconds(winInitialDelay);

            AudioManager.I.PlayLetter(currentQuestion.GetAnswer().Key);
            game.PlayState.gameTime.Start();

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
        int lifes;
        int ActiveLetters;
        private const int MAX_TREE = 8;
        private int FreePlaceholder;

		public GameObject[] ArrayTrees;
        private List<GameObject> ActiveTrees;
        
        public Transform[] ArrayPlaceholder;
        private bool[] UsedPlaceholder = new bool[8];

        public Transform originLettersPlaceholder;

		public GameObject[] ArrayLetters;

        public GameObject[] LifeSprite;
        public GameObject LifeObj;
        

		private HideAndSeekLetterController script;

        public HideAndSeekGame game;


        public HideAndSeekQuestionsProvider questionProvider;
        public HideAndSeekQuestionsPack currentQuestion;

        public float timeToWait = 1.0f;
        private float time;

        public Sprite image;
    }
}