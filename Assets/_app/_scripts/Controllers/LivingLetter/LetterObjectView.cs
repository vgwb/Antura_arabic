using UnityEngine;
using DG.Tweening;
using TMPro;
using Google2u;

namespace EA4S
{
    /// <summary>
    /// View object for letter puppets.
    /// - init component by data
    /// - manage animation
    /// </summary>
    public class LetterObjectView : MonoBehaviour
    {

        #region public properties

        [Header("GO Elements")]
        public TMP_Text Lable;
        public SpriteRenderer ImageSprite;
        public Transform exclamationMark;

        #endregion

        #region runtime variables
        private LLController model;
        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        public LLController Model {
            get {
                if (model == null)
                    setDummyLetterObject();
                return model;
            }
            set
            {
                if (model != value)
                {
                    model = value;
                    OnModelChanged();
                }
                else
                {
                    model = value;
                }
            }
        }

        private Sequence sequenceExclamationMark;

        /// <summary>
        /// Animator
        /// </summary>
        public Animator Anim
        {
            get
            {
                if (!anim)
                    anim = GetComponentInChildren<Animator>();
                return anim;
            }
            set { anim = value; }
        }
        private Animator anim;

        #endregion

        #region properties to be verified
        [HideInInspector]
        public float MergedElementsDistance = 1;
        [HideInInspector]
        public bool IsMerged;
        #endregion

        #region Init
        /// <summary>
        /// Fallback function to set dummy data to letter if no data is provided.
        /// </summary>
        void setDummyLetterObject()
        {
            var letterData = AppManager.Instance.DB.GetLetterDataById("alef");
            Init(new LL_LetterData(letterData.GetId()));
            //Model = new LetterObject(new LetterData(letters.Instance.rowNames[0], letRow));
        }

        /// <summary>
        /// Called when [model changed].
        /// </summary>
        void OnModelChanged()
        {
            if (model.Data == null)
            {
                ImageSprite.enabled = false;
                Lable.enabled = false;
            }
            else
            {
                if (model.Data.DataType == LivingLetterDataType.Image)
                {
                    ImageSprite.sprite = model.Data.DrawForLivingLetter;
                    ImageSprite.enabled = true;
                    Lable.enabled = false;
                }
                else
                {
                    ImageSprite.enabled = false;
                    Lable.enabled = true;
                    Lable.text = Model.Data.TextForLivingLetter;
                }
            }
        }

        /// <summary>
        /// Initializes object with the specified data.
        /// </summary>
        /// <param name="_data">The data.</param>
        /// <param name="_behaviourSettingsOverride">The behaviour settings override.</param>
        public void Init(ILivingLetterData _data, LetterBehaviour.BehaviourSettings _behaviourSettingsOverride)
        {
            Init(_data);
            GetComponent<LetterBehaviour>().Settings = _behaviourSettingsOverride;
        }

        /// <summary>
        /// Initializes  object with the specified data.
        /// </summary>
        /// <param name="_data">The data.</param>
        public void Init(ILivingLetterData _data)
        {
            Model = new LLController(_data);
            // Init Animation sequenceExclamationMark
            sequenceExclamationMark = DOTween.Sequence();
            sequenceExclamationMark.SetLoops(-1);
            if (exclamationMark)
            {
                // Sequence
                sequenceExclamationMark.Append(exclamationMark.DOShakePosition(0.9f));
                sequenceExclamationMark.Pause();
            }

            // Init state change listener
            Model.OnStateChanged = null;
            Model.OnStateChanged += OnStateChanged;
        }
        #endregion

        #region event subscription
        void OnEnable()
        {

        }
        void OnDisable()
        {
            Model.OnStateChanged -= OnStateChanged;
        }
        #endregion

        #region events handlers

        void OnStateChanged(LLAnimationStates _oldState, LLAnimationStates _newState)
        {
            // reset animation for Terrified_State
            if (exclamationMark && sequenceExclamationMark != null)
            {
                //exclamationMark.transform.localScale = Vector3.zero;
                if (sequenceExclamationMark.IsPlaying())
                {
                    sequenceExclamationMark.Pause();
                    exclamationMark.DOScale(0, 0.3f);
                }
            }

            //Debug.Log((int)_newState);

            switch (_newState)
            {
                case LLAnimationStates.LL_idle:
                    // Random from 4 idle animations
                    Anim.SetInteger("State", Random.Range(1, 4));
                    break;
                case LLAnimationStates.LL_idle_1:
                case LLAnimationStates.LL_idle_2:
                case LLAnimationStates.LL_idle_3:
                case LLAnimationStates.LL_idle_4:
                case LLAnimationStates.LL_idle_5:
                case LLAnimationStates.LL_walk:
                case LLAnimationStates.LL_walk_L:
                case LLAnimationStates.LL_walk_R:
                case LLAnimationStates.LL_run:
                case LLAnimationStates.LL_run_happy:
                case LLAnimationStates.LL_run_fear:
                case LLAnimationStates.LL_run_fear_L:
                case LLAnimationStates.LL_run_fear_R:
                case LLAnimationStates.LL_drag_idle:
                case LLAnimationStates.LL_jump_loop:
                case LLAnimationStates.LL_jump:
                case LLAnimationStates.LL_fall_down:
                case LLAnimationStates.LL_land:
                case LLAnimationStates.LL_standup:
                case LLAnimationStates.LL_dancing:
                case LLAnimationStates.LL_dancing_win:
                case LLAnimationStates.LL_twirl:
                case LLAnimationStates.LL_turn_180:
                case LLAnimationStates.LL_win:
                case LLAnimationStates.LL_horray:
                case LLAnimationStates.LL_highfive:
                case LLAnimationStates.LL_lose:
                //case LetterObjectState.LL_get_angry:
                case LLAnimationStates.LL_get_angry_1:
                case LLAnimationStates.LL_get_angry_2:
                case LLAnimationStates.LL_balance:
                case LLAnimationStates.LL_balance_L:
                case LLAnimationStates.LL_balance_R:
                case LLAnimationStates.LL_crouching:
                case LLAnimationStates.LL_crouching_up:
                case LLAnimationStates.LL_ride_rocket_idle:
                case LLAnimationStates.LL_ride_rocket_horray:
                    Anim.SetInteger("State", (int)_newState);
                    break;
                default:
                    // No specific visual behaviour for this state
                    break;

            }

        }

        #endregion

        #region API

        /// <summary>
        /// Sets new state.
        /// </summary>
        /// <param name="_newState">The new state.</param>
        public void SetState(LLAnimationStates _newState)
        {
            Model.State = _newState;
        }

        #endregion

    }
}

