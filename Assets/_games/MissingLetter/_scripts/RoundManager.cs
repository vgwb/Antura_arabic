using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using ModularFramework.Helpers;
using System;

namespace EA4S.MissingLetter
{

    public class RoundManager
    {
        public RoundManager(MissingLetterGame _game)
        {
            m_oGame = _game;
        }

        public void Initialize()
        {
            m_eCurrQuestionPack = null;

            //set the answers and questions center position
            m_v3QstPos = m_oGame.m_oQuestionCamera.position + new Vector3(0, m_oGame.m_fQuestionHeightOffset, 20);
            m_v3AnsPos = m_oGame.m_oAnswerCamera.position + new Vector3(0, m_oGame.m_fAnswerHeightOffset, 20);
            m_oGame.m_oLetterPrefab.GetComponent<LetterBehaviour>().mfDistanceBetweenLetters = m_oGame.m_fDistanceBetweenLetters;

            int qstPoolSize = 3;
            qstPoolSize *= (m_oGame.m_eGameType == GameType.WORD) ? 1 : m_oGame.m_iMaxSentenceSize;
            m_oGame.m_oLetterPrefab.GetComponent<LetterBehaviour>().SetPositions(m_v3QstPos + Vector3.right * m_oGame.m_fQuestionINOffset, m_v3QstPos, m_v3QstPos + Vector3.right * m_oGame.m_fQuestionOUTOffset);
            m_oQuestionPool = new GameObjectPool(m_oGame.m_oLetterPrefab, 3, false);

            int ansPoolSize = m_oGame.m_iNumberOfPossibleAnswers * 4;
            m_oGame.m_oLetterPrefab.GetComponent<LetterBehaviour>().SetPositions(m_v3AnsPos + Vector3.right * m_oGame.m_fAnswerINOffset, m_v3AnsPos, m_v3AnsPos + Vector3.right * m_oGame.m_fAnswerOUTOffset);
            m_oAnswerPool = new GameObjectPool(m_oGame.m_oLetterPrefab, ansPoolSize, false);

            m_oEmoticonsController = new MissingLetterEmoticonsController(m_oGame.m_oEmoticonsController);
        }

        public void SetTutorial(bool _enabled) {
            this.m_bTutorialEnabled = _enabled;
        }

        public void NewRound()
        {
            m_oGame.SetInIdle(false);
            ExitCurrentScene();

            if (m_oGame.m_eGameType == GameType.WORD)
            {
                NextWordQuestion();
            }
            else
            {
                NextSentenceQuestion();
            }

            if (m_oGame.GetCurrentState() == m_oGame.PlayState || m_bTutorialEnabled)
            {
                EnterCurrentScene();
                m_oGame.StartCoroutine(Utils.LaunchDelay(2.0f, m_oGame.SetInIdle, true));
            }
        }

        public void Terminate()
        {
            if(m_oGame.m_iCurrentRound < m_oGame.m_iRoundsLimit)
                ExitCurrentScene();
        }

        public GameObject GetCorrectLLObject()
        {
            foreach (GameObject _obj in m_aoCurrentAnswerScene) {
                if (_obj.GetComponent<LetterBehaviour>().LetterData.Id == m_eCurrQuestionPack.GetCorrectAnswers().ElementAt(0).Id)
                {
                    return _obj;
                }
            }
            return null;
        }

        void NextWordQuestion() {

            m_eCurrQuestionPack = MissingLetterConfiguration.Instance.PipeQuestions.GetNextQuestion();
            ILivingLetterData questionData = m_eCurrQuestionPack.GetQuestion();

            var _wrongAnswers = m_eCurrQuestionPack.GetWrongAnswers().ToList();
            var _correctAnswers = m_eCurrQuestionPack.GetCorrectAnswers().ToList();

            GameObject oQuestion = m_oQuestionPool.GetElement();

            LetterBehaviour qstBehaviour = oQuestion.GetComponent<LetterBehaviour>();
            qstBehaviour.Reset();
            qstBehaviour.LetterData = questionData;
            qstBehaviour.endTransformToCallback += qstBehaviour.Speak;
            qstBehaviour.onLetterBecameInvisible += OnQuestionLetterBecameInvisible;
            qstBehaviour.m_oDefaultIdleAnimation = LLAnimationStates.LL_idle;
            m_aoCurrentQuestionScene.Add(oQuestion);

            m_oEmoticonsController.init(qstBehaviour.transform);

            //after insert in mCurrentQuestionScene
            RemoveLetterfromQuestion();

            GameObject _correctAnswerObject = m_oAnswerPool.GetElement();
            LetterBehaviour corrAnsBheaviour = _correctAnswerObject.GetComponent<LetterBehaviour>();
            corrAnsBheaviour.Reset();
            corrAnsBheaviour.LetterData = _correctAnswers.ElementAt(0);
            corrAnsBheaviour.onLetterBecameInvisible += OnAnswerLetterBecameInvisible;
            corrAnsBheaviour.onLetterClick += OnAnswerClicked;

            corrAnsBheaviour.m_oDefaultIdleAnimation = m_bTutorialEnabled ? LLAnimationStates.LL_still : LLAnimationStates.LL_idle;

            m_aoCurrentAnswerScene.Add(_correctAnswerObject);

            for (int i = 1; i < m_oGame.m_iNumberOfPossibleAnswers && i < _wrongAnswers.Count(); ++i) {
                GameObject _wrongAnswerObject = m_oAnswerPool.GetElement();
                LetterBehaviour wrongAnsBheaviour = _wrongAnswerObject.GetComponent<LetterBehaviour>();
                wrongAnsBheaviour.Reset();
                wrongAnsBheaviour.LetterData = _wrongAnswers.ElementAt(i);
                wrongAnsBheaviour.onLetterBecameInvisible += OnAnswerLetterBecameInvisible;

                if(!m_bTutorialEnabled)
                wrongAnsBheaviour.onLetterClick += OnAnswerClicked;
                wrongAnsBheaviour.m_oDefaultIdleAnimation = m_bTutorialEnabled ? LLAnimationStates.LL_still : LLAnimationStates.LL_idle;

                m_aoCurrentAnswerScene.Add(_wrongAnswerObject);
            }


            m_aoCurrentAnswerScene.Shuffle();
        }

        void NextSentenceQuestion()
        {
            m_eCurrQuestionPack = MissingLetterConfiguration.Instance.PipeQuestions.GetNextQuestion();
            //mCurrQuestionPack.GetQuestion() must return a list of word
            List<LL_WordData> questionData = new List<LL_WordData>();//mCurrQuestionPack.GetQuestion();

            var _wrongAnswers = m_eCurrQuestionPack.GetWrongAnswers();
            var _correctAnswers = m_eCurrQuestionPack.GetCorrectAnswers();


            foreach(LL_WordData _word in questionData)
            {
                GameObject oQuestion = m_oQuestionPool.GetElement();
                LetterBehaviour qstBehaviour = oQuestion.GetComponent<LetterBehaviour>();

                qstBehaviour.Reset();
                qstBehaviour.LetterData = _word;
                qstBehaviour.onLetterBecameInvisible += OnQuestionLetterBecameInvisible;

                m_aoCurrentQuestionScene.Add(oQuestion);
            }


            GameObject _correctAnswerObject = m_oAnswerPool.GetElement();
            LetterBehaviour corrAnsBheaviour = _correctAnswerObject.GetComponent<LetterBehaviour>();

            corrAnsBheaviour.Reset();
            corrAnsBheaviour.LetterData = _correctAnswers.ElementAt(0);
            corrAnsBheaviour.onLetterBecameInvisible += OnAnswerLetterBecameInvisible;

            m_aoCurrentAnswerScene.Add(_correctAnswerObject);

            for (int i = 1; i < m_oGame.m_iNumberOfPossibleAnswers && i < _wrongAnswers.Count(); ++i)
            {
                GameObject _wrongAnswerObject = m_oAnswerPool.GetElement();
                LetterBehaviour wrongAnsBheaviour = _wrongAnswerObject.GetComponent<LetterBehaviour>();
                wrongAnsBheaviour.Reset();
                wrongAnsBheaviour.LetterData = _wrongAnswers.ElementAt(i - 1);
                wrongAnsBheaviour.onLetterBecameInvisible += OnAnswerLetterBecameInvisible;

                m_aoCurrentAnswerScene.Add(_wrongAnswerObject);
            }

            m_aoCurrentAnswerScene.Shuffle();
        }

        void RemoveLetterfromQuestion()
        {
            LL_WordData word = (LL_WordData)m_eCurrQuestionPack.GetQuestion();
            var Letters = ArabicAlphabetHelper.LetterDataListFromWord(word.Data.Arabic, AppManager.Instance.Teacher.GetAllTestLetterDataLL());

            LL_LetterData letter = (LL_LetterData)m_eCurrQuestionPack.GetCorrectAnswers().ToList()[0];
            int index = 0;
            for(; index < Letters.Count; ++index)
            {
                if(Letters[index].Id == letter.Id)
                {
                    break;
                }
            }

            LetterObjectView tmp = m_aoCurrentQuestionScene[0].GetComponent<LetterBehaviour>().mLetter;
            tmp.Label.text = tmp.Label.text.Remove(index, 1);
            tmp.Label.text = tmp.Label.text.Insert(index, mk_sRemovedLetterChar);
        }

        void RestoreQuestion(bool result)
        {
            LetterObjectView tmp = m_aoCurrentQuestionScene[0].GetComponent<LetterBehaviour>().mLetter;
            int index = tmp.Label.text.IndexOf(mk_sRemovedLetterChar);

            foreach (GameObject _obj in m_aoCurrentQuestionScene)
            {
                _obj.GetComponent<LetterBehaviour>().Refresh();
            }
            if(result)
                m_oEmoticonsController.EmoticonPositive();
            else
                m_oEmoticonsController.EmoticonNegative();
            
            //change restored color letter with tag
            string color = result ? "#4CAF50" : "#DD2C00";
            string first = tmp.Label.text[index].ToString();
            tmp.Label.text = tmp.Label.text.Replace(first, "<color="+ color + ">" + first + "</color>");
        }

        void EnterCurrentScene() {
            Vector3 startPos = m_v3QstPos + new Vector3(m_oGame.m_fQuestionINOffset, 0, 0);
            foreach (GameObject _obj in m_aoCurrentQuestionScene) {
                _obj.GetComponent<LetterBehaviour>().EnterScene();
            }

            startPos = m_v3AnsPos + new Vector3(m_oGame.m_fAnswerINOffset, 0, 0);
            int _pos = 0;
            foreach (GameObject _obj in m_aoCurrentAnswerScene) {
                _obj.GetComponent<LetterBehaviour>().EnterScene( _pos, m_aoCurrentAnswerScene.Count());
                ++_pos;
            }
        }

        void ExitCurrentScene() {
            if (m_eCurrQuestionPack != null) {

                foreach (GameObject _obj in m_aoCurrentQuestionScene) {
                    _obj.GetComponent<LetterBehaviour>().ExitScene();
                }
                m_aoCurrentQuestionScene.Clear();

                foreach (GameObject _obj in m_aoCurrentAnswerScene) {
                    _obj.GetComponent<LetterBehaviour>().ExitScene();
                }
                m_aoCurrentAnswerScene.Clear();
            }
        }


        void OnQuestionLetterBecameInvisible(GameObject _obj) {
            m_oQuestionPool.FreeElement(_obj);
            _obj.GetComponent<LetterBehaviour>().onLetterBecameInvisible -= OnQuestionLetterBecameInvisible;
        }

        void OnAnswerLetterBecameInvisible(GameObject _obj) {
            m_oAnswerPool.FreeElement(_obj);
            _obj.GetComponent<LetterBehaviour>().onLetterBecameInvisible -= OnAnswerLetterBecameInvisible;
        }

        private bool isCorrectAnswer(string _key)
        {
            return m_eCurrQuestionPack.GetCorrectAnswers().ElementAt(0).Id == _key;
        }

        private LetterBehaviour GetAnswerById(string _key)
        {
            foreach (GameObject _obj in m_aoCurrentAnswerScene)
            {
                if (_obj.GetComponent<LetterBehaviour>().mLetterData.Id == _key)
                    return _obj.GetComponent<LetterBehaviour>();
            }
            return null;
        }

        public void OnAnswerClicked(string _key) {
            Debug.Log("Answer: " + _key);
            m_oGame.SetInIdle(false);

            //refresh the data (for graphics)
            RestoreQuestion(isCorrectAnswer(_key));

            foreach (GameObject _obj in m_aoCurrentAnswerScene) {
                _obj.GetComponent<LetterBehaviour>().SetEnableCollider(false);
            }

            //letter animation wait for ending dancing animation, wait animator fix
            LetterBehaviour clicked = GetAnswerById(_key);
            if (isCorrectAnswer(_key))
            {
                clicked.PlayAnimation(LLAnimationStates.LL_still);
                clicked.mLetter.DoHorray();
                clicked.LightOn();

                PlayPartcileSystem(m_aoCurrentQuestionScene[0].transform.position + Vector3.up * 2);

                m_oGame.StartCoroutine(Utils.LaunchDelay(0.5f, OnResponse, true));
            }
            else
            {
                clicked.PlayAnimation(LLAnimationStates.LL_still);
                OnResponse(false);
            }

            
        }

        //call after clicked answer animation
        private void OnResponse(bool correct)
        {
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
                m_oGame.StartCoroutine(Utils.LaunchDelay(2.0f, onAnswered, correct));
            }
        }

        private void PlayPartcileSystem(Vector3 _pos)
        {
            m_oGame.m_oParticleSystem.SetActive(true);
            m_oGame.m_oParticleSystem.transform.position = _pos;
            m_oGame.m_oParticleSystem.GetComponent<ParticleSystem>().Play();
            m_oGame.StartCoroutine(Utils.LaunchDelay(1.5f, delegate { m_oGame.m_oParticleSystem.SetActive(false); }));
        }

        //shuffle current answer order and tell to letter change pos
        public void ShuffleLetters(float duration)
        {
            if (m_oGame.IsInIdle()) {
                m_oGame.SetInIdle(false);
                m_aoCurrentAnswerScene.Shuffle();
                for (int i = 0; i < m_aoCurrentAnswerScene.Count; ++i) {
                    float offsetDuration = UnityEngine.Random.Range(-2.0f, 0.0f);
                    m_aoCurrentAnswerScene[i].GetComponent<LetterBehaviour>().ChangePos(i, m_aoCurrentAnswerScene.Count, duration + offsetDuration);
                }
                m_oGame.StartCoroutine(Utils.LaunchDelay(duration, m_oGame.SetInIdle, true));
            }         
        }

        //win animation: quesion high five, correct answer dancing other horray
        private void DoWinAnimations()
        {
            for (int i = 0; i < m_aoCurrentAnswerScene.Count; ++i)
            {
                if(isCorrectAnswer(m_aoCurrentAnswerScene[i].GetComponent<LetterBehaviour>().LetterData.Id))
                {
                    m_aoCurrentAnswerScene[i].GetComponent<LetterBehaviour>().PlayAnimation(LLAnimationStates.LL_dancing);
                    m_aoCurrentAnswerScene[i].GetComponent<LetterBehaviour>().mLetter.DoDancingWin();
                }
                else
                {
                    //random delay poof of wrong answer
                    m_oGame.StartCoroutine(Utils.LaunchDelay(UnityEngine.Random.Range(0, 0.5f), PoofLetter, m_aoCurrentAnswerScene[i]));
                }
            }

            for (int i = 0; i < m_aoCurrentQuestionScene.Count; ++i)
            {
                m_aoCurrentQuestionScene[i].GetComponent<LetterBehaviour>().mLetter.DoHighFive();
                m_aoCurrentQuestionScene[i].GetComponent<LetterBehaviour>().LightOn();
            }
        }

        //poof the gameobject letter
        private void PoofLetter(GameObject letter)
        {
            if (!letter.GetComponent<LetterBehaviour>())
            {
                Debug.LogWarning("Cannot poof letter " + letter.name);
                return;
            }

            letter.GetComponent<LetterBehaviour>().mLetter.Poof();
            letter.transform.position = Vector3.zero;
            AudioManager.I.PlaySfx(Sfx.Poof);
        }

        //lose animation: quesion and correct answer angry other crouch
        private void DoLoseAnimations()
        {
            for (int i = 0; i < m_aoCurrentQuestionScene.Count; ++i)
            {
                m_aoCurrentQuestionScene[i].GetComponent<LetterBehaviour>().mLetter.DoAngry();
                m_aoCurrentQuestionScene[i].GetComponent<LetterBehaviour>().LightOn();
            }

            for (int i = 0; i < m_aoCurrentAnswerScene.Count; ++i)
            {
                if (m_aoCurrentAnswerScene[i].GetComponent<LetterBehaviour>().LetterData.Id == m_eCurrQuestionPack.GetCorrectAnswers().ElementAt(0).Id)
                {
                    m_aoCurrentAnswerScene[i].GetComponent<LetterBehaviour>().mLetter.DoAngry();
                    m_aoCurrentAnswerScene[i].GetComponent<LetterBehaviour>().LightOn();
                }
                else
                {
                    m_aoCurrentAnswerScene[i].GetComponent<LetterBehaviour>().mLetter.Crouching = true;
                }
            }
        }

        #region VARS
        private const string mk_sRemovedLetterChar = "_";

        private MissingLetterGame m_oGame;

        private IQuestionPack m_eCurrQuestionPack;

        private GameObjectPool m_oAnswerPool;
        private GameObjectPool m_oQuestionPool;

        private List<GameObject> m_aoCurrentQuestionScene = new List<GameObject>();
        private List<GameObject> m_aoCurrentAnswerScene = new List<GameObject>();

        private Vector3 m_v3AnsPos;
        private Vector3 m_v3QstPos;

        public event Action<bool> onAnswered;

        private bool m_bTutorialEnabled;

        private MissingLetterEmoticonsController m_oEmoticonsController;
        #endregion

    }
}