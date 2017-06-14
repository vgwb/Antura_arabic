using EA4S.Core;
using UnityEngine;

namespace EA4S.MinigamesAPI
{
    // refactor: is this used at all? It is not clear.
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

            AnturaGameplayInfo gameInfo = (AppManager.Instance as AppManager).Modules.GameplayModule.ActualGameplayInfo as AnturaGameplayInfo;

            (AppManager.Instance as AppManager).Modules.GameplayModule.GameplayResult(
                new AnturaGameplayResult() {

                    GameplayInfo = new AnturaGameplayInfo() { GameId = gameInfo.GameId },
                    Stars = _stars,
                    PositiveResult = positiveResult
                }
            );
        }
    }
}