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

        public void SetTutorial(bool _enabled) {
            this.m_bTutorialEnabled = _enabled;
        }

        public void NewRound()
        {
            mGame.m_bInIdle = false;
            ExitCurrentScene();
            
            if (mRoundType == RoundType.WORD)
            {
                NextWordQuestion();
            }
            else
            {
                NextSentenceQuestion();
            }


            if (mGame.GetCurrentState() == mGame.PlayState || m_bTutorialEnabled)
            {
                EnterCurrentScene();
            }
        }

        public void Terminate()
        {
            if(mGame.mCurrentRound < mGame.mRoundsLimit)
                ExitCurrentScene();
        }

        public GameObject GetCorrectLLObject()
        {
            foreach (GameObject _obj in mCurrentAnswerScene) {
                if(_obj.GetComponent<LetterBehaviour>().LetterData.Key == mCurrQuestionPack.GetCorrectAnswers().ElementAt(0).Key)
                {
                    return _obj;
                }
            }
            return null;
        }

        public ILivingLetterData GetCorrectLetterData()
        {
            return mCurrQuestionPack.GetCorrectAnswers().ElementAt(0);
        }

        void NextWordQuestion() {
            
            mCurrQuestionPack = MissingLetterConfiguration.Instance.PipeQuestions.GetNextQuestion();
            ILivingLetterData questionData = mCurrQuestionPack.GetQuestion();

            var _wrongAnswers = mCurrQuestionPack.GetWrongAnswers().ToList();
            var _correctAnswers = mCurrQuestionPack.GetCorrectAnswers().ToList();

            GameObject oQuestion = mQuestionPool.GetElement();

            LetterBehaviour qstBehaviour = oQuestion.GetComponent<LetterBehaviour>();
            qstBehaviour.Reset();
            qstBehaviour.LetterData = questionData;

            //tmp solution for remove letter
            LL_WordData tmp = (LL_WordData)qstBehaviour.LetterData;
            qstBehaviour.mLetter.Lable.text = tmp.Data.Arabic;

            qstBehaviour.endTransformToCallback += qstBehaviour.Speak;
            qstBehaviour.onLetterBecameInvisible += OnQuestionLetterBecameInvisible;
            qstBehaviour.m_oDefaultIdleAnimation = LLAnimationStates.LL_idle;

            mCurrentQuestionScene.Add(oQuestion);

            GameObject _correctAnswerObject = mAnswerPool.GetElement();
            LetterBehaviour corrAnsBheaviour = _correctAnswerObject.GetComponent<LetterBehaviour>();
            corrAnsBheaviour.Reset();
            corrAnsBheaviour.LetterData = _correctAnswers.ElementAt(0);
            corrAnsBheaviour.onLetterBecameInvisible += OnAnswerLetterBecameInvisible;
            corrAnsBheaviour.onLetterClick += OnAnswerClicked;

            corrAnsBheaviour.m_oDefaultIdleAnimation = m_bTutorialEnabled ? LLAnimationStates.LL_still : LLAnimationStates.LL_idle;

            mCurrentAnswerScene.Add(_correctAnswerObject);

            for (int i = 1; i < mGame.mNumberOfPossibleAnswers && i < _wrongAnswers.Count(); ++i) {
                GameObject _wrongAnswerObject = mAnswerPool.GetElement();
                LetterBehaviour wrongAnsBheaviour = _wrongAnswerObject.GetComponent<LetterBehaviour>();
                wrongAnsBheaviour.Reset();
                wrongAnsBheaviour.LetterData = _wrongAnswers.ElementAt(i);
                wrongAnsBheaviour.onLetterBecameInvisible += OnAnswerLetterBecameInvisible;

                if(!m_bTutorialEnabled)
                    wrongAnsBheaviour.onLetterClick += OnAnswerClicked;
                wrongAnsBheaviour.m_oDefaultIdleAnimation = m_bTutorialEnabled ? LLAnimationStates.LL_still : LLAnimationStates.LL_idle;

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
            corrAnsBheaviour.LetterData = _correctAnswers.ElementAt(0);
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

                ((MissingLetterQuestionProvider)MissingLetterConfiguration.Instance.PipeQuestions).Restore();

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

        public void OnAnswerClicked(string _key) {
            Debug.Log("Answer: " + _key);

            mGame.SetInIdle(false);
            if(mCurrQuestionPack.GetCorrectAnswers().ElementAt(0).Key == _key) {
                AudioManager.I.PlaySfx(Sfx.LetterHappy);
                DoWinAnimations(_key);
            }
            else {
                AudioManager.I.PlaySfx(Sfx.LetterSad);
                DoLoseAnimations(_key);
            }

            foreach (GameObject _obj in mCurrentAnswerScene) {
                _obj.GetComponent<LetterBehaviour>().SetEnableCollider(false);
            }


            if (onAnswered != null) {
                mGame.StartCoroutine(Utils.LaunchDelay(1.5f, onAnswered, mCurrQuestionPack.GetCorrectAnswers().ElementAt(0).Key == _key));
            }

            mGame.StartCoroutine(Utils.LaunchDelay(2.5f, mGame.SetInIdle, true));
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
                    mCurrentAnswerScene[i].GetComponent<LetterBehaviour>().PlayAnimation(LLAnimationStates.LL_dancing);
                    mCurrentAnswerScene[i].GetComponent<LetterBehaviour>().mLetter.DoDancingWin();
                }
                else
                {
                    mCurrentAnswerScene[i].GetComponent<LetterBehaviour>().mLetter.DoHorray();
                }
            }

            for (int i = 0; i < mCurrentQuestionScene.Count; ++i)
            {
                mCurrentQuestionScene[i].GetComponent<LetterBehaviour>().mLetter.DoHighFive();
            }
        }
            
        private void DoLoseAnimations(string _key)
        {
            for (int i = 0; i < mCurrentQuestionScene.Count; ++i)
            {
                mCurrentQuestionScene[i].GetComponent<LetterBehaviour>().PlayAnimation(LLAnimationStates.LL_dancing);
                mCurrentQuestionScene[i].GetComponent<LetterBehaviour>().mLetter.DoDancingLose();
            }

            for (int i = 0; i < mCurrentAnswerScene.Count; ++i)
            {
                mCurrentAnswerScene[i].GetComponent<LetterBehaviour>().PlayAnimation(LLAnimationStates.LL_dancing);
                mCurrentAnswerScene[i].GetComponent<LetterBehaviour>().mLetter.DoDancingLose();
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

        private string msRemovedData;

        public event Action<bool> onAnswered;

        private RoundType mRoundType;
        private bool m_bTutorialEnabled;
        #endregion

    }

    public enum RoundType
    {
        WORD = 0,
        SENTENCE = 1
    }
}