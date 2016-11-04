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
        public const float WALKING_SPEED = 0.0f;
        public const float RUN_SPEED = 1.0f;

        float idleTimer = 3;

        #region public properties

        [Header("GO Elements")]
        public Transform innerTransform;
        public TMP_Text Lable;
        public SpriteRenderer ImageSprite;
        public float maxSize = 1.5f;

        public GameObject[] normalGraphics;
        public GameObject[] limblessGraphics;

        public GameObject poofPrefab;

        #endregion

        #region runtime variables
        /// <summary>
        /// Gets the data.
        /// </summary>
        public ILivingLetterData Data
        {
            get
            {
                return Model.Data;
            }
        }
        
        private LLController model;
        LLController Model
        {
            get
            {
                if (model == null)
                    InitAsDummy();
                return model;
            }
        }

        Animator animator
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

        System.Action onTwirlCallback;

        #endregion

        #region Init
        /// <summary>
        /// Fallback function to set dummy data to letter if no data is provided.
        /// </summary>
        void InitAsDummy()
        {
            var letterData = AppManager.Instance.DB.GetLetterDataById("alef");
            Init(new LL_LetterData(letterData.GetId()));
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

                    innerTransform.localScale =  Vector3.one * Mathf.Min(maxSize, Mathf.Max(1, Lable.GetPreferredValues().x/8.0f));
                }
            }
        }

        /// <summary>
        /// Initializes  object with the specified data.
        /// </summary>
        /// <param name="_data">The data.</param>
        public void Init(ILivingLetterData _data)
        {
            idleTimer = Random.Range(3, 8);

            model = new LLController(_data);

            // Init state change listener
            model.OnStateChanged = null;
            model.OnStateChanged += OnStateChanged;

            OnModelChanged();
        }
        #endregion

        #region events handlers

        void OnStateChanged(LLAnimationStates _oldState, LLAnimationStates _newState)
        {
            //Debug.Log((int)_newState);

            animator.SetBool("dancing", false);
            animator.SetBool("rocketing", false);
            animator.SetBool("dragging", false);
            animator.SetBool("dancing", false);
            animator.SetBool("hanging", false);
            animator.SetBool("idle", true);

            if (_oldState != LLAnimationStates.LL_limbless && _newState == LLAnimationStates.LL_limbless)
            {
                // going limbless
                Poof();

                for (int i=0; i< normalGraphics.Length; ++i)
                    normalGraphics[i].SetActive(false);
                for (int i = 0; i < limblessGraphics.Length; ++i)
                    limblessGraphics[i].SetActive(true);
            }
            else if (_oldState == LLAnimationStates.LL_limbless && _newState != LLAnimationStates.LL_limbless)
            {
                Poof();
                for (int i = 0; i < normalGraphics.Length; ++i)
                    normalGraphics[i].SetActive(true);
                for (int i = 0; i < limblessGraphics.Length; ++i)
                    limblessGraphics[i].SetActive(false);
            }

            switch (_newState)
            {
                case LLAnimationStates.LL_idle:
                case LLAnimationStates.LL_still:
                    animator.SetBool("idle", true);
                    break;
                case LLAnimationStates.LL_walking:
                    animator.SetBool("idle", false);
                    break;
                case LLAnimationStates.LL_rocketing:
                    animator.SetBool("rocketing", true);
                    break;
                case LLAnimationStates.LL_dancing:
                    animator.SetBool("dancing", true);
                    break;
                case LLAnimationStates.LL_dragging:
                    animator.SetBool("dragging", true);
                    break;
                case LLAnimationStates.LL_hanging:
                    animator.SetBool("hanging", true);
                    break;
                default:
                    // No specific visual behaviour for this state
                    break;

            }

        }

        void Update()
        {
            if (model != null)
            {
                if (model.State == LLAnimationStates.LL_idle)
                {
                    idleTimer -= Time.deltaTime;

                    if (idleTimer < 0.0f)
                    {
                        idleTimer = Random.Range(3, 8);
                        animator.SetFloat("random", Random.value);
                        animator.SetTrigger("doAlternative");
                    }

                }

                float oldSpeed = animator.GetFloat("walkSpeed");

                animator.SetFloat("walkSpeed", Mathf.Lerp(oldSpeed, model.walkingSpeed, Time.deltaTime*4.0f));
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

        public bool Crouching
        {
            get { return Model.crouch; }
            set
            {
                Model.crouch = value;
                animator.SetBool("crouch", value);

            }
        }

        public bool Falling
        {
            get { return Model.falling; }
            set
            {
                Model.falling = value;
                animator.SetBool("falling", value);

            }
        }

        public bool HasFear
        {
            get { return Model.fear; }
            set
            {
                Model.fear = value;
                animator.SetBool("fear", value);
            }
        }

        public bool Horraying
        {
            get { return Model.hooraying; }
            set
            {
                Model.hooraying = value;
                animator.SetBool("holdHorray", value);
            }
        }

        

        /// <summary>
        /// Speed is 0 (walk) to 1 (running).
        /// </summary>
        public void SetWalkingSpeed(float speed = WALKING_SPEED)
        {
            Model.walkingSpeed = speed;
        }

        public void DoHorray()
        {
            if (!Model.hooraying)
                animator.SetTrigger("doHorray");
        }

        public void DoAngry()
        {
            animator.SetFloat("random", Random.value);
            animator.SetTrigger("doAngry");
        }

        public void DoHighFive()
        {
            animator.SetTrigger("doHighFive");
        }

        public void DoDancingWin()
        {
            animator.SetTrigger("doDancingWin");
        }

        public void DoDancingLose()
        {
            animator.SetTrigger("doDancingLose");
        }

        public void ToggleDance()
        {
            model.State = LLAnimationStates.LL_dancing;
            animator.SetTrigger("toggleDancing");
        }

        /// <summary>
        /// onLetterShowingBack is called when the letter is twirling and it shows you the back;
        /// so you can swap letter in that moment!
        /// </summary>
        public void DoTwirl(System.Action onLetterShowingBack)
        {
            onTwirlCallback = onLetterShowingBack;
        }

        public void OnJumpStart()
        {
            animator.SetBool("jumping", true);
            animator.SetBool("falling", true);
        }

        public void OnJumpMaximumHeightReached()
        {
            animator.SetBool("jumping", false);
            animator.SetBool("falling", true);
        }

        public void OnJumpEnded()
        {
            animator.SetBool("jumping", false);
            animator.SetBool("falling", false);
        }

        /// <summary>
        /// Produces a poof nearby the LL
        /// </summary>
        public void Poof()
        {
            var puffGo = GameObject.Instantiate(poofPrefab);
            puffGo.AddComponent<AutoDestroy>().duration = 2;
            puffGo.SetActive(true);
            puffGo.transform.position = transform.position + transform.up * 3 + transform.forward*2;
            puffGo.transform.localScale *= 0.75f;
        }

        #endregion

        void OnTwirlBack()
        {
            if (onTwirlCallback != null)
            {
                onTwirlCallback();
                onTwirlCallback = null;
            }
        }
    }
}

