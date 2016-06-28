using UnityEngine;
using System.Collections;
using ModularFramework.Core;
using ModularFramework.Modules;

namespace CGL.Antura {

    public abstract class AnturaMiniGame : SubGame {

        public AnturaGameplayInfo GameplayInfo;

        void Start() {
            GameplayInfo = AnturaGameManager.Instance.Modules.GameplayModule.ActualGameplayInfo as AnturaGameplayInfo;
            ReadyForGameplay();
        }

        protected virtual void ReadyForGameplay() { }
    }
}