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
using System.Collections.Generic;
using ModularFramework.Core;
using ModularFramework.Components;
using System;

namespace ModularFramework.Modules {
    /// <summary>
    /// Concrete implementation for module type UIModule.
    /// </summary>
    public class UIModuleDefault : IUIModule {

        #region IModule Implementation
        public IUIModule ConcreteModuleImplementation { get; set; }
        public IModuleSettings Settings { get; set; }

        public IUIModule SetupModule(IUIModule _concreteModule, IModuleSettings _settings = null) {
            Settings = _settings;
            // Add Here setup stuffs for this concrete implementation
            return this;
        }
        #endregion

        public List<UIWindow> OpenedWindows { get; set; }
        public UIManager UIManager {
            get {
                return ((UIModuleDefaultSettings)Settings).UIManager;
            }
        }

        /// <summary>
        /// Show windows of selected type.
        /// </summary>
        /// <param name="_windowType"></param>
        public void ShowWindow(UIWindowTypes _windowType) {
            // TODO: Enable multiple UIWindow components for UIWindow Type.
            bool winFound = false;
            foreach (var win in GameManager.Instance.UIModule.UIManager.GetComponentsInChildren<UIWindow>()) {
                if (win.Type == _windowType) { 
                    win.Show();
                    winFound = true;
                }
            }
            if(!winFound)
                Debug.LogWarningFormat("Window requested type {0} not found as child of UIRoot.",_windowType);
        }

        /// <summary>
        /// Hide window.
        /// </summary>
        public void HideWindow(UIWindow _winToHide) {
            _winToHide.Hide();
        }

        /// <summary>
        /// Return UIManager in the root.
        /// </summary>
        /// <returns></returns>
        public UIManager GetUIRootManager() {
            return ((UIModuleDefaultSettings)Settings).UIManager;
        }


        public void ShowUIContainer(string _UIContainerKey, OpenUIContainerSettings _openingSettins) {
            UIContainer c = GameManager.Instance.UIModule.GetUIContainerByKey(_UIContainerKey);
            if (c) {
                c.Show();
            } else {
                Debug.LogWarningFormat("UIContainer with key {0} not found as child of UIRoot.", _UIContainerKey);
            }
                
        }

        public void HideUIContainer(string _UIContainerKey) {
            UIContainer c = GameManager.Instance.UIModule.GetUIContainerByKey(_UIContainerKey);
            if (c) {
                c.Hide();
            } else {
                Debug.LogWarningFormat("UIContainer with key {0} not found as child of UIRoot.", _UIContainerKey);
            }
        }
    }
}
