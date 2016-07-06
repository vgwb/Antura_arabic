using UnityEngine;
using System.Collections;
using CGL.Antura;
using ModularFramework.Modules;

namespace CGL.Antura.WanderingLetters {
    public class WanderingLetters : AnturaMiniGame {
        protected override void ReadyForGameplay() {
            base.ReadyForGameplay();
            Debug.LogFormat("Start {0} gameplay : {1} ", GameplayInfo.GameId, GameplayInfo);
        }

        /// <summary>
        /// Manual implementation for gameplay result.
        /// </summary>
        /// <param name="_stars"></param>
        public void SimulateEndGameplay(int _stars) {
            bool positiveResult = false;
            if (_stars > 0)
                positiveResult = true;

            AnturaGameManager.Instance.Modules.GameplayModule.GameplayResult(
                    new AnturaGameplayResult() {
                        GameplayInfo = new AnturaGameplayInfo() { GameId = this.GameplayInfo.GameId },
                        Stars = _stars,
                        PositiveResult = positiveResult
                    }
            );
        }
    }
}
