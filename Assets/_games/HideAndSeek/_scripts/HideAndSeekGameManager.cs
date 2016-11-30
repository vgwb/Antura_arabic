using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;


namespace EA4S.HideAndSeek
{
	public class HideAndSeekGameManager : MonoBehaviour {
		void OnEnable()
        {
			HideAndSeekTreeController.onTreeTouched += MoveObject;
			HideAndSeekLetterController.onLetterTouched += CheckResult;
		}
		void OnDisable()
        {
			HideAndSeekTreeController.onTreeTouched -= MoveObject;
			HideAndSeekLetterController.onLetterTouched -= CheckResult;
		}
		
		void Start ()
        {
            for(int i = 0; i < MAX_OBJECT; ++i)
            {
                UsedPlaceholder[i] = false;
            }
            AnturaEnterScene();
        }
	
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

        private LL_LetterData GetCorrectAnswer()
        {
            //correctAnswer is the first answer
            return (LL_LetterData)currentQuestion.GetCorrectAnswers().ToList()[0];
        }

        public void RepeatAudio()
        {
            AudioManager.I.PlayLetter(GetCorrectAnswer().Id);
        }


        private IEnumerator DelayAnimation()
        {
            game.PlayState.gameTime.Stop();

            var initialDelay = 3f;
            yield return new WaitForSeconds(initialDelay);

            foreach (GameObject x in ArrayLetters)
            {
                x.GetComponent<LetterObjectView>().Poof();
                AudioManager.I.PlaySfx(Sfx.Poof);
                x.SetActive(false);
            }

            var delay = 0.5f;
            yield return new WaitForSeconds(delay);

            StartNewRound = true;
            SetTime();
        }

        void CheckResult(int id)
		{
            letterInAnimation = GetIdFromPosition(id);
            HideAndSeekLetterController script = ArrayLetters[letterInAnimation].GetComponent<HideAndSeekLetterController>();
            if (script.view.Data.Id == GetCorrectAnswer().Id)
            {
                LockTrees();
                StartCoroutine(DelayAnimation());
                script.resultAnimation(true);
                game.OnResult();
                buttonRepeater.SetActive(false);
                AudioManager.I.PlaySfx(Sfx.Win);
            }
            else
            {
                RemoveLife();
                script.resultAnimation(false);
                if (lifes == 0)
                {
                    LockTrees();
                    AudioManager.I.PlaySfx(Sfx.Lose);
                    StartCoroutine(DelayAnimation());
                    buttonRepeater.SetActive(false);
                }
            }
        }

        void RemoveLife()
        {
            switch (--lifes)
            {
                case 2:
                    game.Context.GetOverlayWidget().SetLives(2);
                    break;
                case 1:
                    game.Context.GetOverlayWidget().SetLives(1);
                    break;
                case 0:
                    game.Context.GetOverlayWidget().SetLives(0);
                    break;
            }
        }

        void SetFullLife()
        {
            lifes = 3;
            game.Context.GetOverlayWidget().SetLives(3);
        }

        public void SetTime()
        {
            time = Time.time;
        }

        public void LockTrees()
        {
            for (int i = 0; i < MAX_OBJECT; ++i)
            {
                ArrayTrees[i].GetComponent<CapsuleCollider>().enabled = false;
            }
        }
        public void ClearRound()
        {
            for(int i = 0; i < MAX_OBJECT; ++i)
            {
                ArrayLetters[i].SetActive(true);
                ArrayLetters[i].transform.position = originLettersPlaceholder.position;
                ArrayLetters[i].GetComponent<HideAndSeekLetterController>().ResetLetter();
                UsedPlaceholder[i] = false;
            }
        }

        public void NewRound()
        {
            ClearRound();

            currentQuestion = HideAndSeekConfiguration.Instance.Questions.GetNextQuestion();
            StartNewRound = false;
            SetFullLife();
            FreePlaceholder = MAX_OBJECT;

            ActiveTrees = new List<GameObject>();

            List<ILivingLetterData> letterList = new List<ILivingLetterData>();
            foreach (LL_LetterData letter in currentQuestion.GetCorrectAnswers())
            {
                letterList.Add(letter);
            }

            ActiveLetters = letterList.Count;

            for (int i = 0; i < ActiveLetters; ++i)
            {
                int index = getRandomPlaceholder();
                if(index != -1)
                {

                    ActiveTrees.Add(ArrayTrees[index]);
                    Vector3 hiddenPosition = new Vector3(ArrayPlaceholder[index].transform.position.x, ArrayPlaceholder[index].transform.position.y-3f, ArrayPlaceholder[index].transform.position.z+3f);
                    ArrayLetters[i].transform.position = hiddenPosition;
                    HideAndSeekLetterController scriptComponent = ArrayLetters[i].GetComponent<HideAndSeekLetterController>();
                    scriptComponent.SetStartPosition(ArrayPlaceholder[index].transform.position);
                    scriptComponent.id = index;
                    SetLetterMovement(index, scriptComponent);
                    ArrayLetters[i].GetComponentInChildren<LetterObjectView>().Init(letterList[i]);

                    ArrayLetters[i].transform.DOMove(ArrayPlaceholder[index].transform.position, 0.5f);
                }
            }
            StartCoroutine(DisplayRound_Coroutine());

        }

        public void SetLetterMovement( int placeholder, HideAndSeekLetterController script)
        {
            if (placeholder == 1)
                script.SetMovement(MovementType.OnlyRight);
            else if(placeholder == 2)
                script.SetMovement(MovementType.OnlyLeft);
            else if(placeholder == 0 || placeholder == 6)
                script.SetMovement(MovementType.Enhanced);
            else
                script.SetMovement(MovementType.Normal);
        }
        
        private IEnumerator DisplayRound_Coroutine()
        {
            foreach(GameObject tree in ActiveTrees)
            {
                tree.GetComponent<CapsuleCollider>().enabled = true;
            }

            var winInitialDelay = 0.5f;
            yield return new WaitForSeconds(winInitialDelay);

            AudioManager.I.PlayLetter(GetCorrectAnswer().Id);
            game.PlayState.gameTime.Start();

            buttonRepeater.SetActive(true);
        }

        public int getRandomPlaceholder()
        {
            int result = 0;
            int position = Random.Range(0, FreePlaceholder--);
            
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

        void AnturaEnterScene()
        {
            List<Vector3> AnturaPath = new List<Vector3>();

            AnturaPath.Add(ArrayTrees[2].transform.position + Vector3.forward * 3);
            AnturaPath.Add(ArrayTrees[6].transform.position + Vector3.forward * 3);
            AnturaPath.Add(ArrayTrees[6].transform.position + Vector3.forward * 3 + Vector3.left * 3);

            //AnturaPath.Add(ArrayTrees[0].transform.position + Vector3.back * 3);
            AnturaPath.Add(ArrayTrees[1].transform.position + Vector3.forward * 3);

            AnturaPath.Add(transform.position + Vector3.left * 40);

            Vector3[] aAnturaPath = AnturaPath.ToArray();

            AnturaAnimationController anturaAC = Antura.GetComponent<AnturaAnimationController>();
            anturaAC.IsAngry = true;
            //anturaAC.IsSad = true;

            anturaAC.State = AnturaAnimationStates.walking;

            Antura.transform.DOPath(aAnturaPath, 10, PathType.CatmullRom).OnWaypointChange(delegate (int wayPoint) {
                Antura.transform.DOLookAt(aAnturaPath[wayPoint], 0.5f);
            });
        }


        #region VARIABLES
        bool StartNewRound = true;
        int lifes;
        int ActiveLetters;
        private const int MAX_OBJECT = 7;
        private int FreePlaceholder;

        public GameObject Antura;


        public GameObject[] ArrayTrees;
        private List<GameObject> ActiveTrees;
        
        public Transform[] ArrayPlaceholder;
        private bool[] UsedPlaceholder = new bool[MAX_OBJECT];

        public Transform originLettersPlaceholder;

		public GameObject[] ArrayLetters;

        private int letterInAnimation = -1;
        
		private HideAndSeekLetterController script;

        public HideAndSeekGame game;

        private IQuestionPack currentQuestion;

        public float timeToWait = 1.0f;
        private float time;

        public Sprite image;

        public GameObject buttonRepeater;
        #endregion
    }
}