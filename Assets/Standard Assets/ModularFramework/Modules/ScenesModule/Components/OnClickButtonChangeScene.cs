/* --------------------------------------------------------------
*   Indie Contruction : Modular Framework for Unity
*   Copyright(c) 2016 Indie Construction / Paolo Bragonzi
*   All rights reserved. 
*   For any information refer to http://www.indieconstruction.com
*   
*   This library is free software; you can redistribute it and/or
*   modify it under the terms of the GNU Lesser General Public
*   License as published by the Free Software Foundation; either
*   version 3.0 of the License, or(at your option) any later version.
*   
*   This library is distributed in the hope that it will be useful,
*   but WITHOUT ANY WARRANTY; without even the implied warranty of
*   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
*   Lesser General Public License for more details.
*   
*   You should have received a copy of the GNU Lesser General Public
*   License along with this library.
* -------------------------------------------------------------- */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ModularFramework.Core;
using ModularFramework.Modules;

namespace ModularFramework.Components {

    /// <summary>
    /// This component monitoring UnityEngine.UI.Button click if button exist or react to click over mesh and loading scene using SceneModule loaded in system.
    /// </summary>
    public class OnClickButtonChangeScene : MonoBehaviour {
        public SceneType SceneTypeToLoad;
        public string SceneNameCustom;

        public bool ActualGame = true;
        public string GameId;

        void Start() {
            if(gameObject.GetComponent<Button>())
                gameObject.GetComponent<Button>().onClick.AddListener(OnClick);
        }

        public void OnMouseDown() {
            OnClick();
        }

        public virtual void OnClick() {

            string sceneName = getFullSelectedSceneName();
            GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition(sceneName);
        }

        #region For Editor and click...

        protected string getSelectedSceneName() {
            return SceneTypeToLoad == SceneType.Custom
                        ? SceneNameCustom
                        : GameManager.Instance.Modules.SceneModule.GetSufixSceneNameByType(SceneTypeToLoad);
        }

        protected string getSelectedGameID() {
            return ActualGame == false
                        ? GameId
                        : GameManager.Instance.GetActualGame().GameID;
        }

        protected string getFullSelectedSceneName() {
            return GameManager.Instance.Modules.SceneModule.GetSceneName(getSelectedSceneName(), getSelectedGameID());
        }
        #endregion
    }
}