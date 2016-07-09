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

namespace ModularFramework.Modules {
    /// <summary>
    /// USE THIS DUMMY MODULE TO CREATE A COMMON MODULE CLASS FOR YOUR NEW MODULE.
    /// This is the concrete implementation of DummyModule type with all functionalities.
    /// Strategy pattern.
    /// </summary>
    public class DummyModule : iDummyModule {
        public iDummyModule ConcreteModuleImplementation { get; set; }
        public IModuleSettings Settings { get; set; }
        public int LevelId { get; set; }
        public int LevelProgression { get; set; }
        public int LevelGoal { get; set; }

        public iDummyModule SetupModule(iDummyModule _concreteModule, IModuleSettings _settings = null) {
            ConcreteModuleImplementation = _concreteModule.SetupModule(_concreteModule, _settings);
            if (ConcreteModuleImplementation == null)
                OnSetupError();
            return ConcreteModuleImplementation;
        }

        /// <summary>
        /// Called if an error occurred during the setup.
        /// </summary>
        void OnSetupError() {
            Debug.LogErrorFormat("Module {0} setup return an error.", this.GetType());
        }

        public void LevelStarted() {
            ConcreteModuleImplementation.LevelStarted();
        }

        public void LevelFinisched() {
            ConcreteModuleImplementation.LevelFinisched();
        }
    }

    public interface iDummyModule : IModule<iDummyModule> {
        int LevelId { get; set; }
        int LevelProgression { get; set; }
        int LevelGoal { get; set; }

        void LevelStarted();
        void LevelFinisched();
    }
}