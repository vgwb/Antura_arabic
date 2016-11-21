using UnityEngine;
using System.Collections;

namespace EA4S.ColorTickle
{
    public enum eHitState
    {
        HIT_NONE = 0, HIT_LETTERINSIDE_AND_BODY, HIT_LETTERINSIDE, HIT_LETTEROUTSIDE
    }

    public class HitStateLLController : MonoBehaviour
    {
        //        public enum eLLState
        //        {
        //            IDLE, SCARED, TICKLING
        //        }

        #region EXPOSED MEMBERS
        [SerializeField]
        private Sfx m_oLLTickleSfx;
        #endregion

        #region PRIVATE MEMBERS
        LetterObjectView m_LetterObjectView;
        eHitState m_HitState;
        bool m_Tickle;
        float m_TickleTime;
        #endregion

        #region EVENTS
        public event System.Action LoseLife;
        public event System.Action EnableAntura;
        public event System.Action EnableTutorial;
        #endregion

        #region GETTER/SETTER
        public eHitState hitState
        {
            get { return m_HitState; }
        }

        public Sfx LLTickleSfx
        {
            get { return m_oLLTickleSfx; }
            set { m_oLLTickleSfx = value; }
        }
        #endregion

        // Use this for initialization
        void Start()
        {
            m_LetterObjectView = gameObject.GetComponent<LetterObjectView>();
            m_HitState = eHitState.HIT_NONE;
            m_Tickle = false;
            m_TickleTime = 2.0f;
            gameObject.GetComponent<TMPTextColoring>().OnShapeHit += ShapeTouched;
            gameObject.GetComponent<SurfaceColoring>().OnBodyHit += BodyTouched;
        }

        // Update is called once per frame
        void Update()
        {
            TickleController();
        }

        private void ShapeTouched(bool shapeHitted)
        {
            //when the hit is inside
            if (shapeHitted)
            {
                // Call this function before we set m_HitState = HIT_LETTERINSIDE 
                m_HitState = eHitState.HIT_LETTERINSIDE;
                if (EnableAntura != null)
                {
                    EnableAntura();
                }
            }
            //when the hit is outside
            else
            {
                m_HitState = eHitState.HIT_LETTEROUTSIDE;
                if (!m_Tickle)
                {
                    TicklesLetter();
                }
            }
        }

        private void BodyTouched(bool bodyHitted)
        {
            if (bodyHitted)
            {
                if (m_HitState != eHitState.HIT_LETTERINSIDE)
                {
                    m_HitState = eHitState.HIT_LETTEROUTSIDE;
                    if (!m_Tickle)
                    {
                        TicklesLetter();
                    }
                }
                else
                {
                    m_HitState = eHitState.HIT_LETTERINSIDE_AND_BODY;
                }
            }
            else
            {
                m_HitState = eHitState.HIT_NONE;
            }
        }

        private void TicklesLetter()
        {
            m_Tickle = true;
            m_LetterObjectView.SetState(LLAnimationStates.LL_tickling);

            AudioManager.I.PlaySfx(m_oLLTickleSfx);

            if (LoseLife != null)
            {
                LoseLife();
            }
        }

        private void TickleController()
        {
            if (m_Tickle)
            {
                m_TickleTime -= Time.deltaTime;
                if (m_TickleTime < 0)
                {
                    m_Tickle = false;
                    m_TickleTime = 2.0f;
                    m_LetterObjectView.SetState(LLAnimationStates.LL_still);
                    if (EnableTutorial != null)
                    {
                        EnableTutorial();
                    }
                }
            }
        }

    }
}


