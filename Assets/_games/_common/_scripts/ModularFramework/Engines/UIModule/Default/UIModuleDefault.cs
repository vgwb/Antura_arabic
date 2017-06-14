using UnityEngine;
using System.Collections.Generic;
using System;
using EA4S;

namespace EA4S.Core {
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
            foreach (var win in EA4S.AppManager.Instance.UIModule.UIManager.GetComponentsInChildren<UIWindow>()) {
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
            UIContainer c = EA4S.AppManager.Instance.UIModule.GetUIContainerByKey(_UIContainerKey);
            if (c) {
                c.Show();
            } else {
                Debug.LogWarningFormat("UIContainer with key {0} not found as child of UIRoot.", _UIContainerKey);
            }
                
        }

        public void HideUIContainer(string _UIContainerKey) {
            UIContainer c = EA4S.AppManager.Instance.UIModule.GetUIContainerByKey(_UIContainerKey);
            if (c) {
                c.Hide();
            } else {
                Debug.LogWarningFormat("UIContainer with key {0} not found as child of UIRoot.", _UIContainerKey);
            }
        }
    }
}
