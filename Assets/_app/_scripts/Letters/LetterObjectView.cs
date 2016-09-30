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
            Model = new LetterObject(new LetterData(letters.Instance.rowNames[0], letRow));
        }

        void OnModelChanged() {
            Lable.text = Model.Data.TextForLivingLetter;
            Init(Model.Data);
        }

        public void Init(ILivingLetterData _data, LetterBehaviour.BehaviourSettings _behaviourSettingsOverride) {
            Init(_data);
            GetComponent<LetterBehaviour>().Settings = _behaviourSettingsOverride;
        }

        public void Init(ILivingLetterData _data) {
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
                case LetterObjectState.Idle_State:
                    Anim.SetInteger("State", 0);
                    break;
                case LetterObjectState.Walk_State:
                    Anim.SetInteger("State", 1);
                    break;
                case LetterObjectState.Run_State:
                    Anim.SetInteger("State", 2);
                    break;
                case LetterObjectState.Ninja_State:
                    Anim.SetInteger("State", 4);
                    break;
                case LetterObjectState.FrontOfCamera_State:
                    Anim.SetInteger("State", 0);
                    break;
                case LetterObjectState.GoOut_State:
                    Anim.SetInteger("State", 2);
                    break;
                case LetterObjectState.BumpOut_State:
                    Anim.SetInteger("State", 2);
                    break;
                case LetterObjectState.Grab_State:
                    Anim.SetInteger("State", 3);
                    break;
                case LetterObjectState.Terrified_State:
                    Anim.SetInteger("State", 2);
                    exclamationMark.DOScale(1, 0.3f);
                    sequenceExclamationMark.Play();
                    AudioManager.I.PlaySfx(Sfx.LetterFear);
                    break;
                default:
                    break;
            }
        }
        #endregion

        public void DebugStates(LetterObjectState _stateToSet) {
            Model.State = _stateToSet;
        }

    }
}

