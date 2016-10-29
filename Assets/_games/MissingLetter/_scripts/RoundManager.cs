using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using ModularFramework.Helpers;
using System;
using ArabicSupport;
using Google2u;


namespace EA4S.MissingLetter
{

    //TODO merge with gameManager missingLetterGame
    public class RoundManager
    {

        public RoundManager(MissingLetterGame game)
        {
            mGame = game;
        }

        public void Initialize()
        {
            mCurrQuestionPack = null;
            mRoundType = RoundType.WORD;

            mQstPos = mGame.mQuestionCamera.position + new Vector3(0, mGame.mQuestionHeightOffset, 20);
            mAnsPos = mGame.mAnswerCamera.position + new Vector3(0, mGame.mAnswerHeightOffset, 20);
            mGame.mLetterPrefab.GetComponent<LetterBehaviour>().mfDistanceBetweenLetters = mGame.mfDistanceBetweenLetters;

            int maxSentenceSize = 5;
            int qstPoolSize = 3;
            qstPoolSize *= (mRoundType == RoundType.WORD) ? 1 : maxSentenceSize;
            mGame.mLetterPrefab.GetComponent<LetterBehaviour>().SetPositions(mQstPos + Vector3.right * mGame.mQuestionINOffset, mQstPos, mQstPos + Vector3.right * mGame.mQuestionOUTOffset);
            mQuestionPool = new GameObjectPool(mGame.mLetterPrefab, 3, false);

            int ansPoolSize = mGame.mNumberOfPossibleAnswers * 4;
            mGame.mLetterPrefab.GetComponent<LetterBehaviour>().SetPositions(mAnsPos + Vector3.right * mGame.mAnswerINOffset, mAnsPos, mAnsPos + Vector3.right * mGame.mAnswerOUTOffset);
            mAnswerPool = new GameObjectPool(mGame.mLetterPrefab, ansPoolSize, false);

        }

        public void NewRound()
        {
            ExitCurrentScene();

            if (mRoundType == RoundType.WORD)
            {
                NextWordQuestion();
            }
            else
            {
                NextSentenceQuestion();
            }


            if (mGame.GetCurrentState() == mGame.PlayState) {
                EnterCurrentScene();
            }
            
        }

        public void Terminate()
        {
            if(mGame.mCurrentRound < mGame.mRoundsLimit)
                ExitCurrentScene();
        }


        void NextWordQuestion() {
            mCurrQuestionPack = MissingLetterConfiguration.Instance.PipeQuestions.GetNextQuestion();
            ILivingLetterData questionData = mCurrQuestionPack.GetQuestion();

            var _wrongAnswers = mCurrQuestionPack.GetWrongAnswers().ToList();
            var _correctAnswers = mCurrQuestionPack.GetCorrectAnswers().ToList();

            GameObject oQuestion = mQuestionPool.GetElement();

            miCorrectAnswerIndex = RemoveLetterFromWord((LL_WordData)questionData);

            LetterBehaviour qstBehaviour = oQuestion.GetComponent<LetterBehaviour>();
            qstBehaviour.Reset();
            qstBehaviour.LetterData = questionData;
            //qstBehaviour.onLetterClick += qstBehaviour.Speak; DELETE
            qstBehaviour.endTransformToCallback += qstBehaviour.Speak;
            qstBehaviour.onLetterBecameInvisible += OnQuestionLetterBecameInvisible;

            mCurrentQuestionScene.Add(oQuestion);

            //GameObject _correctAnswerObject = mAnswerPool.GetElement();
            //LetterBehaviour corrAnsBheaviour = _correctAnswerObject.GetComponent<LetterBehaviour>();
            //corrAnsBheaviour.Reset();
            //corrAnsBheaviour.LetterData = _correctAnswers.ElementAt(m_iCorrectAnswerIndex);
            //corrAnsBheaviour.onLetterBecameInvisible += OnAnswerLetterBecameInvisible;
            ////corrAnsBheaviour.onLetterClick += corrAnsBheaviour.Speak;
            //corrAnsBheaviour.onLetterClick_s += OnAnswerClicked;

            //mCurrentAnswerScene.Add(_correctAnswerObject);

            //add other old correct answers to wrong answers
            for(int i=0; i < _correctAnswers.Count; ++i)
            {
                if(i!= miCorrectAnswerIndex)
                {
                    _wrongAnswers.Add(_correctAnswers.ElementAt(i));
                }
            }

            for (int i = 1; i < mGame.mNumberOfPossibleAnswers && i < _wrongAnswers.Count(); ++i) {
                GameObject _wrongAnswerObject = mAnswerPool.GetElement();
                LetterBehaviour wrongAnsBheaviour = _wrongAnswerObject.GetComponent<LetterBehaviour>();
                wrongAnsBheaviour.Reset();
                wrongAnsBheaviour.LetterData = _wrongAnswers.ElementAt(i);
                wrongAnsBheaviour.onLetterBecameInvisible += OnAnswerLetterBecameInvisible;
                //wrongAnsBheaviour.onLetterClick += wrongAnsBheaviour.Speak; DELETE
                wrongAnsBheaviour.onLetterClick += OnAnswerClicked;

                mCurrentAnswerScene.Add(_wrongAnswerObject);
            }


            mCurrentAnswerScene.Shuffle();
        }

        void NextSentenceQuestion()
        {
            mCurrQuestionPack = MissingLetterConfiguration.Instance.PipeQuestions.GetNextQuestion();
            //mCurrQuestionPack.GetQuestion() must return a list of word
            List<LL_WordData> questionData = new List<LL_WordData>();//mCurrQuestionPack.GetQuestion();

            var _wrongAnswers = mCurrQuestionPack.GetWrongAnswers();
            var _correctAnswers = mCurrQuestionPack.GetCorrectAnswers();

            miCorrectAnswerIndex = RemoveWordFromSentences(questionData);

            foreach(LL_WordData _word in questionData)
            {
                GameObject oQuestion = mQuestionPool.GetElement();
                LetterBehaviour qstBehaviour = oQuestion.GetComponent<LetterBehaviour>();

                qstBehaviour.Reset();
                qstBehaviour.LetterData = _word;
                qstBehaviour.onLetterBecameInvisible += OnQuestionLetterBecameInvisible;

                mCurrentQuestionScene.Add(oQuestion);
            }


            GameObject _correctAnswerObject = mAnswerPool.GetElement();
            LetterBehaviour corrAnsBheaviour = _correctAnswerObject.GetComponent<LetterBehaviour>();

            corrAnsBheaviour.Reset();
            corrAnsBheaviour.LetterData = _correctAnswers.ElementAt(miCorrectAnswerIndex);
            corrAnsBheaviour.onLetterBecameInvisible += OnAnswerLetterBecameInvisible;

            mCurrentAnswerScene.Add(_correctAnswerObject);

            for (int i = 1; i < mGame.mNumberOfPossibleAnswers && i < _wrongAnswers.Count(); ++i)
            {
                GameObject _wrongAnswerObject = mAnswerPool.GetElement();
                LetterBehaviour wrongAnsBheaviour = _wrongAnswerObject.GetComponent<LetterBehaviour>();
                wrongAnsBheaviour.Reset();
                wrongAnsBheaviour.LetterData = _wrongAnswers.ElementAt(i - 1);
                wrongAnsBheaviour.onLetterBecameInvisible += OnAnswerLetterBecameInvisible;

                mCurrentAnswerScene.Add(_wrongAnswerObject);
            }

            mCurrentAnswerScene.Shuffle();
        }

        void EnterCurrentScene() {
            Vector3 startPos = mQstPos + new Vector3(mGame.mQuestionINOffset, 0, 0);
            foreach (GameObject _obj in mCurrentQuestionScene) {
                _obj.GetComponent<LetterBehaviour>().EnterScene();
            }

            startPos = mAnsPos + new Vector3(mGame.mAnswerINOffset, 0, 0);
            int _pos = 0;
            foreach (GameObject _obj in mCurrentAnswerScene) {
                _obj.GetComponent<LetterBehaviour>().EnterScene( _pos, mCurrentAnswerScene.Count());
                ++_pos;
            }
        }

        void ExitCurrentScene() {
            if (mCurrQuestionPack != null) {

                if(mRoundType == RoundType.WORD)
                {
                    RestoreRemovedLetter();
                }
                else
                {
                    RestoreRemovedWord();
                }

                foreach (GameObject _obj in mCurrentQuestionScene) {
                    _obj.GetComponent<LetterBehaviour>().ExitScene();
                }
                mCurrentQuestionScene.Clear();

                foreach (GameObject _obj in mCurrentAnswerScene) {
                    _obj.GetComponent<LetterBehaviour>().ExitScene();
                }
                mCurrentAnswerScene.Clear();
            }
        }


        void OnQuestionLetterBecameInvisible(GameObject _obj) {
            mQuestionPool.FreeElement(_obj);
            _obj.GetComponent<LetterBehaviour>().onLetterBecameInvisible -= OnQuestionLetterBecameInvisible;
        }

        void OnAnswerLetterBecameInvisible(GameObject _obj) {
            mAnswerPool.FreeElement(_obj);
            _obj.GetComponent<LetterBehaviour>().onLetterBecameInvisible -= OnAnswerLetterBecameInvisible;
        }

        void OnAnswerClicked(string _key) {
            Debug.Log("Answer: " + _key);

            if(/*mCurrQuestionPack.GetCorrectAnswers().ElementAt(m_iCorrectAnswerIndex).Key == _key*/true) {
                AudioManager.I.PlaySfx(Sfx.LetterHappy);
                DoWinAnimations(_key);
            }
            else {
                AudioManager.I.PlaySfx(Sfx.LetterSad);

                DoLoseAnimations(_key);
            }

            if (onAnswered != null) {
                mGame.StartCoroutine(Utils.LaunchDelay(1.5f, onAnswered, /*mCurrQuestionPack.GetCorrectAnswers().ElementAt(miCorrectAnswerIndex).Key == _key*/true));
            }
        }

        int RemoveLetterFromWord(LL_WordData word)
        {
            char[] caQuestion = ArabicFixer.Fix(word.Data.Arabic, false, false).ToCharArray();
            int index = UnityEngine.Random.Range(0, caQuestion.Length);
            msRemovedData = caQuestion[index].ToString();
            caQuestion[index] = ' ';
            word.Data.Arabic = caQuestion.ToString();
            return index;
        }

        int RemoveWordFromSentences(List<LL_WordData> sentence)
        {
            int index = UnityEngine.Random.Range(0, sentence.Count());
            LL_WordData result = sentence.ElementAt(index);
            msRemovedData = sentence[index].Data.Arabic;
            sentence[index].Data.Arabic = "";
            return index;
        }

        void RestoreRemovedLetter()
        {
            LL_WordData word = (LL_WordData)mCurrentQuestionScene[0].GetComponent<LetterBehaviour>().LetterData;
            word.Data.Arabic = word.Data.Arabic.Replace(' ', msRemovedData[0]);
        }

        void RestoreRemovedWord()
        {
            LL_WordData word = (LL_WordData)mCurrentQuestionScene[miCorrectAnswerIndex].GetComponent<LetterBehaviour>().LetterData;
            word.Data.Arabic = msRemovedData;
        }


        public void ShuffleLetters(float duration)
        {
            mCurrentAnswerScene.Shuffle();
            for(int i=0; i < mCurrentAnswerScene.Count; ++i)
            {
                mCurrentAnswerScene[i].GetComponent<LetterBehaviour>().ChangePos(i, mCurrentAnswerScene.Count, duration);
            }
        }

        private void DoWinAnimations(string _key)
        {
            for (int i = 0; i < mCurrentAnswerScene.Count; ++i)
            {
                if(mCurrentAnswerScene[i].GetComponent<LetterBehaviour>().LetterData.Key == _key)
                {
                    mCurrentAnswerScene[i].GetComponent<LetterBehaviour>().PlayAnimation(LLAnimationStates.LL_ride_rocket_horray);
                }
                else
                {
                    mCurrentAnswerScene[i].GetComponent<LetterBehaviour>().PlayAnimation(LLAnimationStates.LL_twirl);
                }
            }

            for (int i = 0; i < mCurrentQuestionScene.Count; ++i)
            {
                mCurrentQuestionScene[i].GetComponent<LetterBehaviour>().PlayAnimation(LLAnimationStates.LL_highfive);
            }
        }
            
        private void DoLoseAnimations(string _key)
        {
            for (int i = 0; i < mCurrentAnswerScene.Count; ++i)
            {
                mCurrentAnswerScene[i].GetComponent<LetterBehaviour>().PlayAnimation(LLAnimationStates.LL_lose);
            }
        }

        #region VARS

        private MissingLetterGame mGame;

        private IQuestionPack mCurrQuestionPack;

        private GameObjectPool mAnswerPool;
        private GameObjectPool mQuestionPool;

        private List<GameObject> mCurrentQuestionScene = new List<GameObject>();
        private List<GameObject> mCurrentAnswerScene = new List<GameObject>();

        private Vector3 mAnsPos;
        private Vector3 mQstPos;

        private int miCorrectAnswerIndex;
        private string msRemovedData;

        public event Action<bool> onAnswered;

        public enum RoundType
        {
            WORD = 0,
            SENTENCE = 1
        }

        private RoundType mRoundType;
        #endregion

    }
}