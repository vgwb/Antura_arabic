using UnityEngine;
using DG.Tweening;
using TMPro;
using Google2u;

namespace EA4S {
    /// <summary>
    /// View object for letter puppets.
    /// - init component by data
    /// - manage animation
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class LetterObjectView : MonoBehaviour {

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
                    anim = GetComponent<Animator>();
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
        void setDummyLetterObject() {
            lettersRow letRow = letters.Instance.GetRow(letters.Instance.rowNames[0]);
            Init(new LetterData(letters.Instance.rowNames[0], letRow));
            //Model = new LetterObject(new LetterData(letters.Instance.rowNames[0], letRow));
        }

        /// <summary>
        /// Called when [model changed].
        /// </summary>
        void OnModelChanged() {
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
        public void Init(ILivingLetterData _data, LetterBehaviour.BehaviourSettings _behaviourSettingsOverride) {
            Init(_data);
            GetComponent<LetterBehaviour>().Settings = _behaviourSettingsOverride;
        }

        /// <summary>
        /// Initializes  object with the specified data.
        /// </summary>
        /// <param name="_data">The data.</param>
        public void Init(ILivingLetterData _data) {
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
        void OnEnable() {

        }
        void OnDisable() {
            Model.OnStateChanged -= OnStateChanged;
        }
        #endregion

        #region events handlers
        void OnStateChanged(LetterObjectState _oldState, LetterObjectState _newState) {
            // reset animation for Terrified_State
            if (exclamationMark && sequenceExclamationMark != null) {
                //exclamationMark.transform.localScale = Vector3.zero;
                if (sequenceExclamationMark.IsPlaying()) {
                    sequenceExclamationMark.Pause();
                    exclamationMark.DOScale(0, 0.3f);
                }
            }

            switch (_newState) {
                case LetterObjectState.LL_idle:
                    // Random from 4 idle animations
                    Anim.SetInteger("State", Random.Range(1, 4));
                    break;
                case LetterObjectState.LL_walk:
                    Anim.SetInteger("State", (int)LetterObjectState.LL_walk);
                    break;
                case LetterObjectState.LL_run:
                    Anim.SetInteger("State", (int)LetterObjectState.LL_run);
                    break;
                case LetterObjectState.LL_drag_idle:
                    // Todo: disable animator and start ragdoll
                    break;
                case LetterObjectState.LL_jump:
                    Anim.SetInteger("State", (int)LetterObjectState.LL_jump);
                    break;
                case LetterObjectState.LL_land:
                    Anim.SetInteger("State", (int)LetterObjectState.LL_land);
                    break;

                case LetterObjectState.Ninja_State:
                    // deprecated?
                    break;

                case LetterObjectState.BumpOut_State:
                    
                    break;

                case LetterObjectState.LL_run_fear:
                    // Todo: make this with new 'emoticons'

                    //Anim.SetInteger("State", 2);
                    //exclamationMark.DOScale(1, 0.3f);
                    //sequenceExclamationMark.Play();
                    //AudioManager.I.PlaySfx(Sfx.LetterFear);
                    break;
                case LetterObjectState.FrontOfCamera_State:
                    // Verify
                    break;
                case LetterObjectState.GoOut_State:
                    // Verify
                    break;
                default:
                    // No specific visual behaviour for this state
                    break;
            }
        }
        #endregion

        public void DebugStates(LetterObjectState _stateToSet) {
            Model.State = _stateToSet;
        }

    }
}

