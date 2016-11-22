using UnityEngine;
using System.Collections;

namespace EA4S {

    public class PlaySessionResultManager : MonoBehaviour {

        void Start() {
            // Navigation manager 
            NavigationManager.I.CurrentScene = AppScene.PlaySessionResult;

            GameResultUI.ShowEndsessionResult(
                new System.Collections.Generic.List<EndsessionResultData>(),
                2
                );
        }

    }
}