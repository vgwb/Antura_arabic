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

namespace ModularFramework.Core {

    public class Singleton<T> : MonoBehaviour where T : Component {
        // Static singleton property
        public static T Instance { get; private set; }
        public string TypeName { get; private set; }

        public static bool IsActive {
            get {
                return Instance != null;
            }
        }

        void Awake() {
            TypeName = typeof(T).FullName;

            // First we check if there are any other instances conflicting then destroy this and return
            if (Instance != null) {
                if (Instance != this)
                    Destroy(gameObject);
                return;             // return is my addition so that the inspector in unity still updates
            }

            // Here we save our singleton instance
            Instance = this as T;

            // setup specifics.
            GameSetup();
        }

        void OnDestroy() {
            if (Instance == this)
                GameDestroy();
        }

        protected virtual void GameSetup() {
        }

        protected virtual void GameDestroy() {
        }
    }
}
