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
using ModularFramework.Core;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ModularFramework.Modules {

    public class UIContainer : MonoBehaviour {

        #region Properties
        public string Key;
        public List<UIContainer> Breadcrumbs = new List<UIContainer>();
        public Button CloseContainerButton;
        #endregion

        #region Activation State
        public enum States {
            hide,
            hiding,
            show,
            showing,
        }

        private States state;
        /// <summary>
        /// State
        /// </summary>
        public States State {
            get { return state; }
            set {
                if (state != value)
                    OnStateChanged(value, state);
                state = value; }
        }
        #endregion

        // If needed...
        public UIContainerTypes ContainerType;
        public enum UIContainerTypes{
            FullScreen,
            Popup,
        }

        RectTransform rectTransform;

        void Awake() {
            rectTransform = GetComponent<RectTransform>();
            State = States.hiding;
        }

        public virtual void OnEnable() {
            // Remove UniRx refactoring request: any reactive interaction within this class must be called manually.
        }


        #region StateMachine
        /// <summary>
        /// Called at any state value change.
        /// </summary>
        /// <param name="_newState"></param>
        /// <param name="_oldStates"></param>
        protected virtual void OnStateChanged(States _newState, States _oldStates) {
            switch (_newState) {
                case States.hide:
                    break;
                case States.hiding:
                    rectTransform.anchoredPosition = new Vector2(0, Screen.height * 2);
                    State = States.hide;
                    break;
                case States.show:
                    break;
                case States.showing:
                    rectTransform.anchoredPosition = new Vector2(0, 0);
                    State = States.show;
                    break;
            }
        }
        #endregion

        #region API
        public void Show() {
            State = States.showing; 
        }

        public void Hide() {
            State = States.hiding;
        }
        #endregion
    }
}