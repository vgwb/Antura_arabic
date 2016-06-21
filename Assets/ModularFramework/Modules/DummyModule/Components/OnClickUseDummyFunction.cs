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

namespace ModularFramework.Modules {

    [RequireComponent(typeof(Button))]
    public class OnClickUseDummyFunction : MonoBehaviour {

        public enum DummyModuleFunctions {
            Started,
            Ended,
        }

        public DummyModuleFunctions CallbackFunction;

        void Start() {
            gameObject.GetComponent<Button>().onClick.AddListener(OnClick);
        }

        public void OnClick() {
            switch (CallbackFunction) {
                case DummyModuleFunctions.Started:
                    GameManager.Instance.Modules.DummyModule.LevelStarted();
                    break;
                case DummyModuleFunctions.Ended:
                    GameManager.Instance.Modules.DummyModule.LevelFinisched();
                    break;
                default:
                    // 
                    break;
            }
        }

    }
}