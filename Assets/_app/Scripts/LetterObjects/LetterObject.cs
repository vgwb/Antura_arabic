using UnityEngine;
using System.Collections;
using ModularFramework.Core;
using ModularFramework.Modules;
using Google2u;

namespace CGL.Antura {

    public class LetterObject {

        letters Data;

        private LetterState state = LetterState.Idle_State;
        public LetterState State {
            get { return state; }
            set { state = value; }
        }

    }


    public enum LetterState {
        Idle_State,
        Run_State,
        Grab_State,
    }
}
