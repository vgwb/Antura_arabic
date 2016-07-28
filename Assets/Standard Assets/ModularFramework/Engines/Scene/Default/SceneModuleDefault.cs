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
using System;
using ModularFramework.Core;

namespace ModularFramework.Modules {

    public class SceneModuleDefault : ISceneModule {

        #region IModule
        public ISceneModule ConcreteModuleImplementation { get; set; }
        public IModuleSettings Settings { get; set; }

        public ISceneModule SetupModule(ISceneModule _concreteModule, IModuleSettings _settings = null) {
            Settings = _settings;
            return this;
        }
        #endregion

        public virtual void LoadSceneWithTransition(string _sceneToLoad, SceneTransition _transitionSettings) {
            UnityEngine.SceneManagement.SceneManager.LoadScene(_sceneToLoad);
        }

        public void UnloadSceneWithTransition(SceneTransition _transitionSettings) {
        }

        public virtual void SceneLoadedBehaviour() {
        }
    }

}