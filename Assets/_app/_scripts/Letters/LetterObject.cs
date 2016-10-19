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
        public LetterObjectState OldState = LetterObjectState.LL_idle;

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
        private LetterObjectState state = LetterObjectState.LL_idle;

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
        // Idle
        LL_idle = 0, // Generic idle
        LL_idle_1 = 1,
        LL_idle_2 = 2,
        LL_idle_3 = 3,
        LL_idle_4 = 4,
        LL_idle_5 = 5,
        // Walk or run
        LL_walk = 10, // Generic walk
        LL_walk_L = 15,
        LL_walk_R = 16,
        LL_run = 21, // Generic run
        LL_run_happy = 22,
        LL_run_fear = 24,
        LL_run_fear_L = 25,
        LL_run_fear_R = 26,
        // Drag
        LL_drag_idle = 30,
        // Vertical situations
        LL_jump = 41,
        LL_jump_loop = 42,
        LL_fall_down = 43,
        LL_land = 45,
        LL_standup = 48,
        // Dance
        LL_dancing = 50, // Generic dancing
        LL_dancing_win = 52,
        LL_twirl = 54,
        LL_turn_180 = 55,
        // Win/Lose or extra espressions
        LL_win = 60,
        LL_horray = 61,
        LL_highfive = 63,
        LL_lose = 65,
        //LL_get_angry = 67, // Generic angry expression
        LL_get_angry_1 = 68,
        LL_get_angry_2 = 69,
        // Balance
        LL_balance = 70,
        LL_balance_L = 75,
        LL_balance_R = 76,
        // Ride Rocket
        LL_ride_rocket_idle = 81,
        LL_ride_rocket_horray = 85,
        // Crouching
        LL_crouching = 91,
        LL_crouching_up = 93,

        // to be check
        FrontOfCamera_State,
        GoOut_State,
        BumpOut_State,

        // deprecated
        Ninja_State,
    }
}
