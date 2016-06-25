using UnityEngine;
using System.Collections;
using CGL.Antura;

namespace CGL.Antura.WanderingLetters {
    public class WanderingLetters : AnturaMiniGame {

        protected override void ReadyForGameplay() {
            base.ReadyForGameplay();
            Debug.LogFormat("Start {0} gameplay : {1} ", GameId, GameplayInfo);
        }

        public void SimulateEndGameplay() {
            Debug.LogFormat("End {0} gameplay : {1} ", GameId, GameplayInfo);
        }
    }
}
