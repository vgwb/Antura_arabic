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
        private LetterObject model;
        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        public LetterObject Model {
            get {
                if (model == null)
                    setDummyLetterObject();
                return model;
            }
            set {
                if (model != value) {
                    model = value;
                    OnModelChanged();
                } else {
                    model = value;
                }
            }
        }

        private Sequence sequenceExclamationMark;

        /// <summary>
        /// Animator
        /// </summary>
        public Animator Anim {
            get {
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
        [HideInInspector]
        public DropSingleArea ActualDropArea;
        //DropState dropState = DropState.off;
        #endregion

        #region Init
        /// <summary>
        /// Fallback function to set dummy data to letter if no data is provided.
        /// </summary>
        void setDummyLetterObject()
        {
            var letterData = AppManager.Instance.DB.GetLetterDataById("alef");
            Init(new LetterData(letterData.GetId()));
            //Model = new LetterObject(new LetterData(letters.Instance.rowNames[0], letRow));
        }

        /// <summary>
        /// Called when [model changed].
        /// </summary>
        void OnModelChanged()
        {
            if (model.Data.DataType == LivingLetterDataType.Image) {
                ImageSprite.sprite = model.Data.DrawForLivingLetter;
                ImageSprite.enabled = true;
                Lable.enabled = false;
            } else {
                ImageSprite.enabled = false;
                Lable.enabled = true;
                Lable.text = Model.Data.TextForLivingLetter;
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
            Model = new LetterObject(_data);
            // Init Animation sequenceExclamationMark
            sequenceExclamationMark = DOTween.Sequence();
            sequenceExclamationMark.SetLoops(-1);
            if (exclamationMark) {
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

        void OnStateChanged(LetterObjectState _oldState, LetterObjectState _newState)
        {
            // reset animation for Terrified_State
            if (exclamationMark && sequenceExclamationMark != null) {
                //exclamationMark.transform.localScale = Vector3.zero;
                if (sequenceExclamationMark.IsPlaying()) {
                    sequenceExclamationMark.Pause();
                    exclamationMark.DOScale(0, 0.3f);
                }
            }

            //Debug.Log((int)_newState);

            switch (_newState) {
                case LetterObjectState.LL_idle:
                    // Random from 4 idle animations
                    Anim.SetInteger("State", Random.Range(1, 4));
                    break;
                case LetterObjectState.LL_idle_1:
                case LetterObjectState.LL_idle_2:
                case LetterObjectState.LL_idle_3:
                case LetterObjectState.LL_idle_4:
                case LetterObjectState.LL_idle_5:
                case LetterObjectState.LL_walk:
                case LetterObjectState.LL_walk_L:
                case LetterObjectState.LL_walk_R:
                case LetterObjectState.LL_run:
                case LetterObjectState.LL_run_happy:
                case LetterObjectState.LL_run_fear:
                case LetterObjectState.LL_run_fear_L:
                case LetterObjectState.LL_run_fear_R:
                case LetterObjectState.LL_drag_idle:
                case LetterObjectState.LL_jump_loop:
                case LetterObjectState.LL_jump:
                case LetterObjectState.LL_fall_down:
                case LetterObjectState.LL_land:
                case LetterObjectState.LL_standup:
                case LetterObjectState.LL_dancing:
                case LetterObjectState.LL_dancing_win:
                case LetterObjectState.LL_twirl:
                case LetterObjectState.LL_turn_180:
                case LetterObjectState.LL_win:
                case LetterObjectState.LL_horray:
                case LetterObjectState.LL_highfive:
                case LetterObjectState.LL_lose:
                //case LetterObjectState.LL_get_angry:
                case LetterObjectState.LL_get_angry_1:
                case LetterObjectState.LL_get_angry_2:
                case LetterObjectState.LL_balance:
                case LetterObjectState.LL_balance_L:
                case LetterObjectState.LL_balance_R:
                case LetterObjectState.LL_crouching:
                case LetterObjectState.LL_crouching_up:
                case LetterObjectState.LL_ride_rocket_idle:
                case LetterObjectState.LL_ride_rocket_horray:
                    Anim.SetInteger("State", (int)_newState);
                    break;
                case LetterObjectState.FrontOfCamera_State:
                    // Verify
                    break;
                case LetterObjectState.GoOut_State:
                    // Verify
                    break;
                case LetterObjectState.Ninja_State:
                    // Verify
                    break;
                case LetterObjectState.BumpOut_State:
                    // Verify
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
        public void SetState(LetterObjectState _newState)
        {
            Model.State = _newState;
        }

        #endregion

    }
}

