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
using System.Collections;
using ModularFramework.Core;
using System;

namespace ModularFramework.Modules {
    /// <summary>
    /// This is the concrete implementation of SceneModule type with all functionalities.
    /// Strategy pattern.
    /// </summary>
    public class SceneModule : ISceneModule {

        #region IModule
        public ISceneModule ConcreteModuleImplementation { get; set; }
        public IModuleSettings Settings { get; set; }

        public ISceneModule SetupModule(ISceneModule _concreteModule, IModuleSettings _settings = null) {
            ConcreteModuleImplementation = _concreteModule.SetupModule(_concreteModule, _settings);
            if (ConcreteModuleImplementation == null)
                OnSetupError();
            SetSettings(new SceneTransition());
            return ConcreteModuleImplementation;
        }

        /// <summary>
        /// Called if an error occurred during the setup.
        /// </summary>
        void OnSetupError() {
            Debug.LogErrorFormat("Module {0} setup return an error.", this.GetType());
        }
        #endregion

        #region module API
        /// <summary>
        /// Actual transition environment settings.
        /// </summary>
        public SceneTransition TransitionSettings = new SceneTransition();

        /// <summary>
        /// Set scene transition settings.
        /// </summary>
        /// <param name="_transitionSettings"></param>
        public void SetSettings(SceneTransition _transitionSettings) {
            TransitionSettings = _transitionSettings;
        }

        /// <summary>
        /// Load scene. Call just once, otherwise transition will be resetted and not triggered.
        /// </summary>
        /// <param name="_sceneToLoad"></param>
        public void LoadSceneWithTransition(string _sceneToLoad) {
            LoadSceneWithTransition(_sceneToLoad, TransitionSettings);
        }

        /// <summary>
        /// Load scene.
        /// </summary>
        /// <param name="_sceneToLoad"></param>
        /// <param name="_transitionSettings"></param>
        public void LoadSceneWithTransition(string _sceneToLoad, SceneTransition _transitionSettings) {
            ConcreteModuleImplementation.LoadSceneWithTransition(_sceneToLoad, _transitionSettings);
        }

        /// <summary>
        /// Unload scene.
        /// </summary>
        public void UnloadSceneWithTransition() {
            UnloadSceneWithTransition(TransitionSettings);
        }

        /// <summary>
        /// Scene loaded behaviour.
        /// </summary>
        public void SceneLoadedBehaviour() {
            ConcreteModuleImplementation.SceneLoadedBehaviour();
        }

        /// <summary>
        /// Unload scene.
        /// </summary>
        /// <param name="_transitionSettings"></param>
        public void UnloadSceneWithTransition(SceneTransition _transitionSettings) {
            ConcreteModuleImplementation.UnloadSceneWithTransition(_transitionSettings);
        }
        #endregion

        #region Presets

        public string GetActualSceneNamePrefix() {
            return string.Format("{0}_",GameManager.Instance.GetActualGame().GameID);
        }

        /// <summary>
        /// Return sufix (ex: 01_{sufix}) for scene type.
        /// </summary>
        /// <param name="_sceneType"></param>
        /// <returns></returns>
        public string GetSufixSceneNameByType(SceneType _sceneType) {
            // TODO: https://trello.com/c/okVYR5x0
            switch (_sceneType) {
                case SceneType.Main:
                case SceneType.GameSelect:
                case SceneType.WoldSelect:
                case SceneType.LevelSelect:
                    return _sceneType.ToString();
                case SceneType.Custom:
                    return "";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Return exact scene name for scene type in param of actual game.
        /// </summary>
        /// <param name="_sceneType"></param>
        /// <returns></returns>
        public string GetActualGameSceneName(SceneType _sceneType) {
            return GetSceneName(_sceneType, GameManager.Instance.GetActualGame().GameID);
        }

        /// <summary>
        /// Return exact scene name for scene type and gameid in param.
        /// </summary>
        /// <param name="_sceneType"></param>
        /// <param name="_gameId"></param>
        /// <returns></returns>
        public string GetSceneName(SceneType _sceneType, string _gameId) {
            return GetSceneName(GetSufixSceneNameByType(_sceneType), _gameId);
        }

        /// <summary>
        /// Return exact scene name for scene type and gameid in param.
        /// </summary>
        /// <param name="_sceneType"></param>
        /// <param name="_gameId"></param>
        /// <returns></returns>
        public string GetSceneName(string _sceneTypeName, string _gameId) {
            return string.Format("{1}{0}{2}", (_sceneTypeName == string.Empty || _gameId == string.Empty) ? string.Empty : "_", _gameId, _sceneTypeName);
        }

        #endregion
    }

    public interface ISceneModule : IModule<ISceneModule> {
        void LoadSceneWithTransition(string _sceneToLoad, SceneTransition _transitionSettings);
        void UnloadSceneWithTransition(SceneTransition _transitionSettings);
        void SceneLoadedBehaviour();
    }

    public enum SceneType {
        Main,
        GameSelect,
        WoldSelect,
        LevelSelect,
        Custom,
    }

}
