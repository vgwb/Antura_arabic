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

            //TODO only one pool -> problem because setting the positions of qst/ans on prefabs
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

            EnterCurrentScene();
        }

        public void Terminate()
        {
            ExitCurrentScene();
        }


        void NextWordQuestion() {
            mCurrQuestionPack = MissingLetterConfiguration.Instance.PipeQuestions.GetNextQuestion();
            ILivingLetterData questionData = mCurrQuestionPack.GetQuestion();

            var _wrongAnswers = mCurrQuestionPack.GetWrongAnswers().ToList();
            var _correctAnswers = mCurrQuestionPack.GetCorrectAnswers().ToList();

            GameObject oQuestion = mQuestionPool.GetElement();

            m_iCorrectAnswerIndex = RemoveLetterFromWord((LL_WordData)questionData);

            LetterBehaviour qstBehaviour = oQuestion.GetComponent<LetterBehaviour>();
            qstBehaviour.Reset();
            qstBehaviour.LetterData = questionData;
            qstBehaviour.onLetterClick += qstBehaviour.Speak;
            qstBehaviour.endTransformToCallback += qstBehaviour.Speak;
            qstBehaviour.onLetterBecameInvisible += OnQuestionLetterBecameInvisible;

            mCurrentQuestionScene.Add(oQuestion);

            GameObject _correctAnswerObject = mAnswerPool.GetElement();
            LetterBehaviour corrAnsBheaviour = _correctAnswerObject.GetComponent<LetterBehaviour>();
            corrAnsBheaviour.Reset();
            corrAnsBheaviour.LetterData = _correctAnswers.ElementAt(m_iCorrectAnswerIndex);
            corrAnsBheaviour.onLetterBecameInvisible += OnAnswerLetterBecameInvisible;
            //corrAnsBheaviour.onLetterClick += corrAnsBheaviour.Speak;
            corrAnsBheaviour.onLetterClick_s += OnAnswerClicked;

            mCurrentAnswerScene.Add(_correctAnswerObject);

            //add other old correct answers to wrong answers
            for(int i=0; i < _correctAnswers.Count; ++i)
            {
                if(i!= m_iCorrectAnswerIndex)
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
                //wrongAnsBheaviour.onLetterClick += wrongAnsBheaviour.Speak;
                wrongAnsBheaviour.onLetterClick_s += OnAnswerClicked;

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

            m_iCorrectAnswerIndex = RemoveWordFromSentences(questionData);

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
            corrAnsBheaviour.LetterData = _correctAnswers.ElementAt(m_iCorrectAnswerIndex);
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
            //Debug.Log("New Question/Answers are running IN");
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
                //Debug.Log("Current Question/Answers are running OUT");

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
            if(mCurrQuestionPack.GetCorrectAnswers().ElementAt(m_iCorrectAnswerIndex).Key == _key) {
                Debug.Log("Yes! Correct Answer");
                AudioManager.I.PlaySfx(Sfx.LetterHappy);
            }
            else {
                Debug.Log("Noo :( Wrong Answer");
                AudioManager.I.PlaySfx(Sfx.LetterSad);
            }

            if (onAnswered != null) {
                onAnswered(mCurrQuestionPack.GetCorrectAnswers().ElementAt(m_iCorrectAnswerIndex).Key == _key);
            }
        }

        int RemoveLetterFromWord(LL_WordData word)
        {
            char[] caQuestion = ArabicFixer.Fix(word.Word, false, false).ToCharArray();
            int index = UnityEngine.Random.Range(0, caQuestion.Length);
            m_sRemovedData = caQuestion[index].ToString();
            caQuestion[index] = ' ';
            word.Word = caQuestion.ToString();
            return index;
        }

        int RemoveWordFromSentences(List<LL_WordData> sentence)
        {
            int index = UnityEngine.Random.Range(0, sentence.Count());
            LL_WordData result = sentence.ElementAt(index);
            m_sRemovedData = sentence[index].Word;
            sentence[index].Word = "";
            return index;
        }

        void RestoreRemovedLetter()
        {
            LL_WordData word = (LL_WordData)mCurrentQuestionScene[0].GetComponent<LetterBehaviour>().LetterData;
            word.Word = word.Word.Replace(' ', m_sRemovedData[0]);
        }

        void RestoreRemovedWord()
        {
            LL_WordData word = (LL_WordData)mCurrentQuestionScene[m_iCorrectAnswerIndex].GetComponent<LetterBehaviour>().LetterData;
            word.Word = m_sRemovedData;
        }


        public void ShuffleLetters()
        {
            mCurrentAnswerScene.Shuffle();
            for(int i=0; i < mCurrentAnswerScene.Count; ++i)
            {
                mCurrentAnswerScene[i].GetComponent<LetterBehaviour>().ChangePos(i, mCurrentAnswerScene.Count);
            }
        }

        #region VARS

        MissingLetterGame mGame;

        private IQuestionPack mCurrQuestionPack;

        private GameObjectPool mAnswerPool;
        private GameObjectPool mQuestionPool;

        private List<GameObject> mCurrentQuestionScene = new List<GameObject>();
        private List<GameObject> mCurrentAnswerScene = new List<GameObject>();

        private Vector3 mAnsPos;
        private Vector3 mQstPos;

        private int m_iCorrectAnswerIndex;
        private string m_sRemovedData;

        public event Action<bool> onAnswered;

        public enum RoundType
        {
            WORD = 0,
            SENTENCE
        }

        private RoundType mRoundType;
        #endregion

    }
}