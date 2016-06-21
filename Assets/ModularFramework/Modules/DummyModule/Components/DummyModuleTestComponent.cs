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
using ModularFramework.Modules;

namespace ModularFramework.Test {

    public class DummyModuleTestComponent : MonoBehaviour {

        /// <summary>
        /// Module for Dummy functionality (in real context can be for example -> PlayerProgressModule).
        /// </summary>
        DummyModule dummyModule = new DummyModule();

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {
            // Module Setup
            if (Input.GetKeyDown(KeyCode.A)) {
                dummyModule.SetupModule(new DummyModuleDefault());
            } else if (Input.GetKeyDown(KeyCode.S)) {
                dummyModule.SetupModule(new DummyModuleAlternative());
            }

            // test functionality
            if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                dummyModule.LevelStarted();
            } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
                dummyModule.LevelFinisched();
            }
        }
    }
}