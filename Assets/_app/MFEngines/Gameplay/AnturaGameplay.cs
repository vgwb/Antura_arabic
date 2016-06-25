using UnityEngine;
using System.Collections;
using ModularFramework.Core;
using ModularFramework.Modules;
using System;

namespace CGL.Antura {
    /// <summary>
    /// Concrete implementation for module type GameplayModule.
    /// </summary>
    public class GameplayModuleAnturaGameplay : IGameplayModule {

        #region IModule Implementation
        public IGameplayModule ConcreteModuleImplementation { get; set; }
        public IModuleSettings Settings { get; set; }

        public IGameplayInfo ActualGameplayInfo { get; set; }

        public IGameplayModule SetupModule(IGameplayModule _concreteModule, IModuleSettings _settings = null) {
            Settings = _settings;
            // Add Here setup stuffs for this concrete implementation
            return this;
        }

        #endregion

        public void GameplayResult(IGameplayResult _result) {
            throw new NotImplementedException();
        }

        public void GameplayStart(IGameplayInfo _gameplayInfo) {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Gameplay result class data structure.
    /// </summary>
    public class AnturaGameplayResult : IGameplayResult {
        public IGameplayInfo GameplayInfo { get; set; }
    }

    /// <summary>
    /// Gameplay info class data structure.
    /// </summary>
    public class AnturaGameplayInfo : IGameplayInfo {
        public string GameId;
    }
}
        
