using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using EA4S;

namespace EA4S.Test {

    public class StateDropDownList : Dropdown {

        new void Start() {
            options.Clear();
            options.AddRange(addOptionsFromEnum<LLAnimationStates>());
            onValueChanged.AddListener(delegate {
                foreach (var l in FindObjectsOfType<LetterObjectView>()) {
                    l.SetState((LLAnimationStates)Enum.Parse(typeof(LLAnimationStates), options[value].text));
                }
            });

        }

        List<OptionData> addOptionsFromEnum<T>() {
            List<OptionData> optionsToAdd = new List<OptionData>();
            foreach (var val in Enum.GetValues(typeof(T))) {
                optionsToAdd.Add(new OptionData() { text = val.ToString() });
            }
            return optionsToAdd;
        }

        void OnDisable() {
            onValueChanged.RemoveAllListeners();
        }
    }
}