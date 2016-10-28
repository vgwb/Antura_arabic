using UnityEngine;
using System.Collections;

namespace EA4S
{
    /// <summary>
    /// Base component for LivingLetter.
    /// Old data for this letter.
    /// Old state for this letter.
    /// </summary>
    public class LLController {

        public ILivingLetterData Data;

        public LLController(ILivingLetterData _data) {
            Data = _data;
        }

        public float walkingSpeed;
        public bool crouch;
        public bool falling;
        public bool jumping;
        public bool fear;

        #region States

        public LLAnimationStates State {
            get { return state; }
            set {
                if (state != value) {
                    var oldState = state;
                    state = value;
                    if (OnStateChanged != null)
                        OnStateChanged(oldState, state);
                }
            }
        }
        private LLAnimationStates state = LLAnimationStates.LL_idle;

        #endregion

        #region events
        public delegate void StateEvent(LLAnimationStates _oldState, LLAnimationStates _newState);

        public StateEvent OnStateChanged;
        #endregion
    }

    public enum LLAnimationStates {
        LL_idle,
        LL_walking,
        LL_dragging,
        LL_hanging,
        LL_dancing,
        LL_rocketing,
        LL_limbless
    }
}
