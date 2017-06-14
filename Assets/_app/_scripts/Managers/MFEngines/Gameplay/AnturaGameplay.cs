using UnityEngine;

namespace EA4S.Core
{
    /// <summary>
    /// Concrete implementation for module type GameplayModule.
    /// </summary>
    // refactor: can we remove the ModularFramerwork?
    public class GameplayModuleAnturaGameplay : IGameplayModule
    {

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
            AnturaGameplayResult result = _result as AnturaGameplayResult;
            AnturaGameplayInfo gameplayInfo = result.GameplayInfo as AnturaGameplayInfo;

            Debug.LogFormat("End {0} gameplay result : {1} with {2} star/s.", gameplayInfo.GameId, result.PositiveResult, result.Stars);
        }

        public void GameplayStart(IGameplayInfo _gameplayInfo) {
            ActualGameplayInfo = _gameplayInfo;
        }
    }

    /// <summary>
    /// Gameplay result class data structure.
    /// </summary>
    public class AnturaGameplayResult : IGameplayResult {
        public IGameplayInfo GameplayInfo { get; set; }
        public bool PositiveResult;
        public int Stars;
    }

    /// <summary>
    /// Gameplay info class data structure.
    /// </summary>
    public class AnturaGameplayInfo : IGameplayInfo {
        public string GameId;
    }
}