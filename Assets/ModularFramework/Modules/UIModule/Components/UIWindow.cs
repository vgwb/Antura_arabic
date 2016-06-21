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

namespace ModularFramework.Modules {

    public abstract class UIWindow : MonoBehaviour {

        public UIWindowTypes Type;

        RectTransform rectTransform;

        void Awake() {
            rectTransform = GetComponent<RectTransform>();
            Type = SetWindowType();
        }
        
        /// <summary>
        /// Force set window type for any UIWindow derived classes.
        /// </summary>
        /// <returns></returns>
        public abstract UIWindowTypes SetWindowType();

        public void Show() {
            rectTransform.anchoredPosition = Vector2.zero;
        }

        public void Hide() {
            rectTransform.anchoredPosition = new Vector2(0, Screen.height);
        }
    }
}