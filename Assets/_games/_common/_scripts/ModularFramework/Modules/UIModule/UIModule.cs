
using UnityEngine;
using System;
using System.Collections.Generic;

namespace EA4S.Core
{


    public enum UIWindowTypes {
        NULL,
        PlayerProfile_Selection,
        PlayerProfile_CreateNew,
    }

    /// <summary>
    /// This is the Context implementation of this Module type with all functionalities provided from the Strategy interface.
    /// </summary>
    public class UIModule : IUIModule {

        public List<UIWindow> OpenedWindows {
            get { return ConcreteModuleImplementation.OpenedWindows; }
            set { ConcreteModuleImplementation.OpenedWindows = value; }
        }

        public UIManager UIManager {
            get { return ConcreteModuleImplementation.UIManager; }
        }

        #region IModule implementation
        /// <summary>
        /// Concrete Module Implementation.
        /// </summary>
        public IUIModule ConcreteModuleImplementation { get; set; }
        public IModuleSettings Settings { get; set; }

        /// <summary>
        /// Module Setup.
        /// </summary>
        /// <param name="_concreteModule">Concrete module implementation to set as active module behaviour.</param>
        /// <returns></returns>
        public IUIModule SetupModule(IUIModule _concreteModule, IModuleSettings _settings = null) {
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

        #endregion

        /// <summary>
        /// Show container.
        /// </summary>
        /// <param name="_UIContainerKey"></param>
        /// <param name="_withBreadcrumbs"></param>
        public void ShowUIContainer(string _UIContainerKey, OpenUIContainerSettings _openingSettins = null) { 
            ConcreteModuleImplementation.ShowUIContainer(_UIContainerKey, _openingSettins);
        }

        /// <summary>
        /// Hide container.
        /// </summary>
        public void HideUIContainer(string _UIContainerKey) {
            ConcreteModuleImplementation.HideUIContainer(_UIContainerKey);
        }

        /// <summary>
        /// Get UIContainer container in UIRoot if exist, otherwise null.
        /// </summary>
        /// <param name="_UIContainerKey"></param>
        /// <returns></returns>
        public UIContainer GetUIContainerByKey(string _UIContainerKey) {
            foreach (var item in UIManager.GetComponentsInChildren<UIContainer>()) {
                if (item.Key == _UIContainerKey)
                    return item;
            }
            return null;
        }

        public void HideAllActiveWindows(UIWindowTypes _windowType = UIWindowTypes.NULL) {
            foreach (UIWindow w in OpenedWindows) {
                if (w.Type != _windowType)
                    w.Hide();
            }
        }
    }

    /// <summary>
    /// Strategy interface. 
    /// Provide All the functionalities required for any Concrete implementation of the module.
    /// </summary>
    public interface IUIModule : IModule<IUIModule> {
        List<UIWindow> OpenedWindows { get; set; }
        UIManager UIManager { get; }

        void ShowUIContainer(string _UIContainerKey, OpenUIContainerSettings _openingSettins = null);
        void HideUIContainer(string _UIContainerKey);
    }

    public class OpenUIContainerSettings {
        /// <summary>
        /// If true active UIContainer will added to the Breadcrumbs of new opened UIContainer for manage back button.
        /// </summary>
        public bool WithBreadcrumbs = false;
        /// <summary>
        /// If > 0 new UIContainer will be closed in TimeLife seconds.
        /// </summary>
        public float TimeLife = 0;

        public OpenUIContainerSettings() { }
    }
}