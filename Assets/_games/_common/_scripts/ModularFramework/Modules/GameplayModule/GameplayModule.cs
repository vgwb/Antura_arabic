using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Modules System with strategy pattern implementation.
/// </summary>
namespace EA4S.Core
{
    /// <summary>
    /// This is the Context implementation of this Module type with all functionalities provided from the Strategy interface.
    /// </summary>
    public class GameplayModule : IGameplayModule {

        #region IModule implementation
        /// <summary>
        /// Concrete Module Implementation.
        /// </summary>
        public IGameplayModule ConcreteModuleImplementation { get; set; }
        public IModuleSettings Settings { get; set; }

        /// <summary>
        /// Module Setup.
        /// </summary>
        /// <param name="_concreteModule">Concrete module implementation to set as active module behaviour.</param>
        /// <returns></returns>
        public IGameplayModule SetupModule(IGameplayModule _concreteModule, IModuleSettings _settings = null) {
            ConcreteModuleImplementation = _concreteModule.SetupModule(_concreteModule, _settings);
            return ConcreteModuleImplementation;
        }

        #endregion

        #region Properties

        public IGameplayInfo ActualGameplayInfo {
            get { return ConcreteModuleImplementation.ActualGameplayInfo; }
            set { ConcreteModuleImplementation.ActualGameplayInfo = value; }
        }

        #endregion

        #region API

        public void GameplayResult(IGameplayResult _result) {
            ConcreteModuleImplementation.GameplayResult(_result);
        }

        public void GameplayStart(IGameplayInfo _gameplayInfo) {
            ConcreteModuleImplementation.GameplayStart(_gameplayInfo);
        }

        #endregion
    }

    /// <summary>
    /// Strategy interface. 
    /// Provide All the functionalities required for any Concrete implementation of the module.
    /// </summary>
    public interface IGameplayModule : IModule<IGameplayModule> {
        IGameplayInfo ActualGameplayInfo { get; set; }
        void GameplayResult(IGameplayResult _result);
        void GameplayStart(IGameplayInfo _gameplayInfo);
    }

    /// <summary>
    /// Interface for gameplay result data.
    /// </summary>
    public interface IGameplayResult {
        IGameplayInfo GameplayInfo { get; set; }
    }

    /// <summary>
    /// Interface for gameplay info data.
    /// </summary>
    public interface IGameplayInfo {

    }
}