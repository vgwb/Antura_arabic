using UnityEngine;
using System.Collections;

namespace EA4S {

    public class PlaySessionResultManager : MonoBehaviour {

        void Start() {
            // Navigation manager 
            NavigationManager.I.CurrentScene = AppScene.PlaySessionResult;

            GameObject[] objs = GameResultUI.ShowEndsessionResult(NavigationManager.I.UseEndSessionResults(),2);
        }

    }
}