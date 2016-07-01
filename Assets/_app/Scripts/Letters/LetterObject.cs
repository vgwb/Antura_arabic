using UnityEngine;
using System.Collections;
using ModularFramework.Core;
using ModularFramework.Modules;
using Google2u;

namespace CGL.Antura {

    public class LetterObject {

        public LetterData Data;

        public LetterObject(LetterData _data) {
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
                if(state != value) { 
                    OnStateChanged(state, value);
                    state = value;
                } else
                    state = value;
            }
        }
        private LetterObjectState state = LetterObjectState.Idle_State;

        #endregion

        /// <summary>
        /// Called at any State variation.
        /// </summary>
        /// <param name="_oldState"></param>
        /// <param name="_newState"></param>
        protected virtual void OnStateChanged(LetterObjectState _oldState, LetterObjectState _newState) {
            switch (_newState) {
                case LetterObjectState.Idle_State:

                    break;
                case LetterObjectState.Run_State:

                    break;
                case LetterObjectState.Grab_State:

                    break;
                default:
                    Debug.Log("State not found");
                    break;
            }
        }
    }

    public enum LetterObjectState {
        Idle_State,
        Run_State,
        Grab_State,
    }
}
