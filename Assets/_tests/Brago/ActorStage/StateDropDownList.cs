using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using EA4S;

namespace EA4S.Test {

    public class StateDropDownList : Dropdown {

        new void Start() {
            options.Clear();
            options.AddRange(new List<OptionData>(){
                new OptionData() { text = LetterObjectState.Idle_State.ToString() },
                new OptionData() { text = LetterObjectState.Walk_State.ToString() },
                new OptionData() { text = LetterObjectState.Run_State.ToString() },
                new OptionData() { text = LetterObjectState.Ninja_State.ToString() },
                new OptionData() { text = LetterObjectState.FrontOfCamera_State.ToString() },
                new OptionData() { text = LetterObjectState.GoOut_State.ToString() },
                new OptionData() { text = LetterObjectState.BumpOut_State.ToString() },
                new OptionData() { text = LetterObjectState.Grab_State.ToString() },
                new OptionData() { text = LetterObjectState.Terrified_State.ToString() },
            });

            onValueChanged.AddListener(delegate {
                foreach (var l in FindObjectsOfType<LetterObjectView>()) {
                    l.DebugStates((LetterObjectState)Enum.Parse(typeof(LetterObjectState), options[value].text));
                }
            });
        }
    }
}