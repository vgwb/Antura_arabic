using UnityEngine;
using System.Collections;

namespace EA4S
{
    /// <summary>
    /// Base component for LivingLetter.
    /// Old data for this letter.
    /// Old state for this letter.
    /// </summary>
    public class LetterObject {

        public ILivingLetterData Data;

        public LetterObject(ILivingLetterData _data) {
            Data = _data;
        }

        #region States

        /// <summary>
        /// Old State.
        /// </summary>
        public LetterObjectState OldState = LetterObjectState.Idle_State;

        /// <summary>
        /// State
        /// </summary>
        public LetterObjectState State {
            get { return state; }
            set {
                if (state != value) {
                    if(OnStateChanged != null)
                        OnStateChanged(state, value);
                    OldState = state;
                    state = value;
                } 
                //else
                //    state = value;
            }
        }
        private LetterObjectState state = LetterObjectState.Idle_State;

        #endregion

        #region events
        public delegate void StateEvent(LetterObjectState _oldState, LetterObjectState _newState);

        public StateEvent OnStateChanged;
        #endregion

        ///// <summary>
        ///// Called at any State variation.
        ///// </summary>
        ///// <param name="_oldState"></param>
        ///// <param name="_newState"></param>
        //protected virtual void OnStateChanged(LetterObjectState _oldState, LetterObjectState _newState) {
        //    switch (_newState) {
        //        case LetterObjectState.Idle_State:

        //            break;
        //        case LetterObjectState.Run_State:

        //            break;
        //        case LetterObjectState.Grab_State:

        //            break;
        //        default:
        //            Debug.Log("State not found");
        //            break;
        //    }
        //}
    }

    public enum LetterObjectState {
        Idle_State,
        Walk_State,
        Run_State,
        Ninja_State,
        FrontOfCamera_State,
        GoOut_State,
        BumpOut_State,
        Grab_State,
        Terrified_State,
    }
}
