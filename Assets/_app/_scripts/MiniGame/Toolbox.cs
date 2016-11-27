using UnityEngine;
using System.Collections;
using ModularFramework.Core;
using ModularFramework.Modules;

namespace EA4S
{
    public class Toolbox : MonoBehaviour
    {

        /// <summary>
        /// Manual implementation for gameplay result.
        /// </summary>
        /// <param name="_stars"></param>
        public void SimulateEndGameplay(int _stars)
        {
            bool positiveResult = false;
            if (_stars > 0)
                positiveResult = true;

            AnturaGameplayInfo gameInfo = AppManager.I.Modules.GameplayModule.ActualGameplayInfo as AnturaGameplayInfo;

            AppManager.I.Modules.GameplayModule.GameplayResult(
                new AnturaGameplayResult() {

                    GameplayInfo = new AnturaGameplayInfo() { GameId = gameInfo.GameId },
                    Stars = _stars,
                    PositiveResult = positiveResult
                }
            );
        }
    }
}