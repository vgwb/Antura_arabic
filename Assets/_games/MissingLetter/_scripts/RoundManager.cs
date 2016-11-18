using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using ModularFramework.Helpers;
using System;

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
            mGame.mLetterPrefab.GetComponent<LetterBehaviour>().mfDistanceBetweenLetters = mGame.m_fDistanceBetweenLetters;

            int maxSentenceSize = 5;
            int qstPoolSize = 3;
            qstPoolSize *= (mRoundType == RoundType.WORD) ? 1 : maxSentenceSize;
            mGame.mLetterPrefab.GetComponent<LetterBehaviour>().SetPositions(mQstPos + Vector3.right * mGame.mQuestionINOffset, mQstPos, mQstPos + Vector3.right * mGame.mQuestionOUTOffset);
            mQuestionPool = new GameObjectPool(mGame.mLetterPrefab, 3, false);

            int ansPoolSize = mGame.m_iNumberOfPossibleAnswers * 4;
            mGame.mLetterPrefab.GetComponent<LetterBehaviour>().SetPositions(mAnsPos + Vector3.right * mGame.mAnswerINOffset, mAnsPos, mAnsPos + Vector3.right * mGame.mAnswerOUTOffset);
            mAnswerPool = new GameObjectPool(mGame.mLetterPrefab, ansPoolSize, false);

            m_oEmoticonsController = new MissingLetterEmoticonsController(mGame.m_oEmoticonsController);
        }

        public void SetTutorial(bool _enabled) {
            this.m_bTutorialEnabled = _enabled;
        }

        public void NewRound()
        {
            mGame.SetInIdle(false);
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
                mGame.StartCoroutine(Utils.LaunchDelay(2.0f, mGame.SetInIdle, true));
            }
        }

        public void Terminate()
        {
            if(mGame.mCurrentRound < mGame.m_iRoundsLimit)
                ExitCurrentScene();
        }

        public GameObject GetCorrectLLObject()
        {
            foreach (GameObject _obj in mCurrentAnswerScene) {
                if (_obj.GetComponent<LetterBehaviour>().LetterData.Key == mCurrQuestionPack.GetCorrectAnswers().ElementAt(0).Key)
                {
                    return _obj;
                }
            }
            return null;
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
            qstBehaviour.endTransformToCallback += qstBehaviour.Speak;
            qstBehaviour.onLetterBecameInvisible += OnQuestionLetterBecameInvisible;
            qstBehaviour.m_oDefaultIdleAnimation = LLAnimationStates.LL_idle;
            mCurrentQuestionScene.Add(oQuestion);

            m_oEmoticonsController.init(qstBehaviour.transform);

            //after insert in mCurrentQuestionScene
            RemoveLetterfromQuestion();

            GameObject _correctAnswerObject = mAnswerPool.GetElement();
            LetterBehaviour corrAnsBheaviour = _correctAnswerObject.GetComponent<LetterBehaviour>();
            corrAnsBheaviour.Reset();
            corrAnsBheaviour.LetterData = _correctAnswers.ElementAt(0);
            corrAnsBheaviour.onLetterBecameInvisible += OnAnswerLetterBecameInvisible;
            corrAnsBheaviour.onLetterClick += OnAnswerClicked;

            corrAnsBheaviour.m_oDefaultIdleAnimation = m_bTutorialEnabled ? LLAnimationStates.LL_still : LLAnimationStates.LL_idle;

            mCurrentAnswerScene.Add(_correctAnswerObject);

            for (int i = 1; i < mGame.m_iNumberOfPossibleAnswers && i < _wrongAnswers.Count(); ++i) {
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

            for (int i = 1; i < mGame.m_iNumberOfPossibleAnswers && i < _wrongAnswers.Count(); ++i)
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

        void RemoveLetterfromQuestion()
        {
            LL_WordData word = (LL_WordData)mCurrQuestionPack.GetQuestion();
            var Letters = ArabicAlphabetHelper.LetterDataListFromWord(word.Data.Arabic, AppManager.Instance.Teacher.GetAllTestLetterDataLL());

            LL_LetterData letter = (LL_LetterData)mCurrQuestionPack.GetCorrectAnswers().ToList()[0];
            int index = 0;
            for(; index < Letters.Count; ++index)
            {
                if(Letters[index].Key == letter.Key)
                {
                    break;
                }
            }

            LetterObjectView tmp = mCurrentQuestionScene[0].GetComponent<LetterBehaviour>().mLetter;
            tmp.Lable.text = tmp.Lable.text.Remove(index, 1);
            tmp.Lable.text = tmp.Lable.text.Insert(index, mkRemovedLetterChar);

        }

        void RestoreQuestion(bool result)
        {

            LetterObjectView tmp = mCurrentQuestionScene[0].GetComponent<LetterBehaviour>().mLetter;
            int index = tmp.Lable.text.IndexOf(mkRemovedLetterChar);

            foreach (GameObject _obj in mCurrentQuestionScene)
            {
                _obj.GetComponent<LetterBehaviour>().Refresh();
            }
            if(result)
                m_oEmoticonsController.EmoticonPositive();
            else
                m_oEmoticonsController.EmoticonNegative();
            
            //change restored color letter with tag
            string color = result ? "#4CAF50" : "#DD2C00";
            string first = tmp.Lable.text[index].ToString();
            tmp.Lable.text = tmp.Lable.text.Replace(first, "<color="+ color + ">" + first + "</color>");
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

        private bool isCorrectAnswer(string _key)
        {
            return mCurrQuestionPack.GetCorrectAnswers().ElementAt(0).Key == _key;
        }

        private LetterBehaviour GetAnswerById(string _key)
        {
            foreach (GameObject _obj in mCurrentAnswerScene)
            {
                if (_obj.GetComponent<LetterBehaviour>().mLetterData.Key == _key)
                    return _obj.GetComponent<LetterBehaviour>();
            }
            return null;
        }

        public void OnAnswerClicked(string _key) {
            Debug.Log("Answer: " + _key);
            mGame.SetInIdle(false);

            //refresh the data (for graphics)
            RestoreQuestion(isCorrectAnswer(_key));

            foreach (GameObject _obj in mCurrentAnswerScene) {
                _obj.GetComponent<LetterBehaviour>().SetEnableCollider(false);
            }

            //letter animation wait for ending dancing animation, wait animator fix
            LetterBehaviour clicked = GetAnswerById(_key);
            if (isCorrectAnswer(_key))
            {
                clicked.PlayAnimation(LLAnimationStates.LL_still);
                clicked.mLetter.DoHorray();
                clicked.LightOn();

                mGame.mParticleSystem.SetActive(true);
                mGame.mParticleSystem.transform.position = mCurrentQuestionScene[0].transform.position + Vector3.forward + Vector3.up * 2;
                mGame.mParticleSystem.GetComponent<ParticleSystem>().Play();
                mGame.StartCoroutine(Utils.LaunchDelay(1.5f, delegate { mGame.mParticleSystem.SetActive(false); }));

                mGame.StartCoroutine(Utils.LaunchDelay(0.5f, OnResponse, true));
            }
            else
            {
                clicked.PlayAnimation(LLAnimationStates.LL_still);
                //clicked.mLetter.DoAngry();
                OnResponse(false);
            }

            
        }

        //call after clicked answer animation
        private void OnResponse(bool correct)
        {
            //sad, happy sound -> onclick or on response??
            if (correct)
            {
                AudioManager.I.PlaySfx(Sfx.LetterHappy);
                DoWinAnimations();
            }
            else
            {
                AudioManager.I.PlaySfx(Sfx.LetterSad);
                DoLoseAnimations();
            }

            if (onAnswered != null)
            {
                mGame.StartCoroutine(Utils.LaunchDelay(2.0f, onAnswered, correct));
            }

        }

        //shuffle current answer order and tell to letter change pos
        public void ShuffleLetters(float duration)
        {

            if (mGame.IsInIdle()) {
                mGame.SetInIdle(false);
                mCurrentAnswerScene.Shuffle();
                for (int i = 0; i < mCurrentAnswerScene.Count; ++i) {
                    float offsetDuration = UnityEngine.Random.Range(-2.0f, 0.0f);
                    mCurrentAnswerScene[i].GetComponent<LetterBehaviour>().ChangePos(i, mCurrentAnswerScene.Count, duration + offsetDuration);
                }
                mGame.StartCoroutine(Utils.LaunchDelay(duration, mGame.SetInIdle, true));
            }
            
        }

        //win animation: quesion high five, correct answer dancing other horray
        private void DoWinAnimations()
        {
            for (int i = 0; i < mCurrentAnswerScene.Count; ++i)
            {
                if(isCorrectAnswer(mCurrentAnswerScene[i].GetComponent<LetterBehaviour>().LetterData.Key))
                {
                    mCurrentAnswerScene[i].GetComponent<LetterBehaviour>().PlayAnimation(LLAnimationStates.LL_dancing);
                    mCurrentAnswerScene[i].GetComponent<LetterBehaviour>().mLetter.DoDancingWin();
                }
                else
                {
                    //random delay poof of wrong answer
                    //mCurrentAnswerScene[i].GetComponent<LetterBehaviour>().mLetter.DoHorray();
                    mGame.StartCoroutine(Utils.LaunchDelay(UnityEngine.Random.Range(0, 0.5f), delegate (int index)
                    {
                        mCurrentAnswerScene[index].GetComponent<LetterBehaviour>().mLetter.Poof();
                        mCurrentAnswerScene[index].transform.position = Vector3.zero;
                        AudioManager.I.PlaySfx(Sfx.Poof);
                    }, i));
                }
            }

            for (int i = 0; i < mCurrentQuestionScene.Count; ++i)
            {
                mCurrentQuestionScene[i].GetComponent<LetterBehaviour>().mLetter.DoHighFive();
                mCurrentQuestionScene[i].GetComponent<LetterBehaviour>().LightOn();
            }
        }

        //lose animation: quesion and correct answer angry other crouch
        private void DoLoseAnimations()
        {
            for (int i = 0; i < mCurrentQuestionScene.Count; ++i)
            {
                mCurrentQuestionScene[i].GetComponent<LetterBehaviour>().mLetter.DoAngry();
                mCurrentQuestionScene[i].GetComponent<LetterBehaviour>().LightOn();
            }

            for (int i = 0; i < mCurrentAnswerScene.Count; ++i)
            {
                if (mCurrentAnswerScene[i].GetComponent<LetterBehaviour>().LetterData.Key == mCurrQuestionPack.GetCorrectAnswers().ElementAt(0).Key)
                {
                    mCurrentAnswerScene[i].GetComponent<LetterBehaviour>().mLetter.DoAngry();
                    mCurrentAnswerScene[i].GetComponent<LetterBehaviour>().LightOn();
                }
                else
                {
                    mCurrentAnswerScene[i].GetComponent<LetterBehaviour>().mLetter.Crouching = true;
                    //random delay poof of wrong answer
                    //mGame.StartCoroutine(Utils.LaunchDelay(UnityEngine.Random.Range(0, 0.5f), delegate (int index)
                    //{
                    //    mCurrentAnswerScene[index].GetComponent<LetterBehaviour>().mLetter.Poof();
                    //    mCurrentAnswerScene[index].transform.position = Vector3.zero;
                    //    AudioManager.I.PlaySfx(Sfx.Poof);
                    //}, i));
                }
            }
        }

        #region VARS
        private const string mkRemovedLetterChar = "_";

        private MissingLetterGame mGame;

        private IQuestionPack mCurrQuestionPack;

        private GameObjectPool mAnswerPool;
        private GameObjectPool mQuestionPool;

        private List<GameObject> mCurrentQuestionScene = new List<GameObject>();
        private List<GameObject> mCurrentAnswerScene = new List<GameObject>();

        private Vector3 mAnsPos;
        private Vector3 mQstPos;

        public event Action<bool> onAnswered;

        private RoundType mRoundType;
        private bool m_bTutorialEnabled;

        private MissingLetterEmoticonsController m_oEmoticonsController;
        #endregion

    }

    public enum RoundType
    {
        WORD = 0,
        SENTENCE = 1
    }
}