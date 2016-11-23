using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

using EA4S;

namespace EA4S.ThrowBalls
{
    public class ThrowBallsGame : MiniGame
    {
        public GameState GameState { get; private set; }

        public static ThrowBallsGame instance;

        public GameObject ball;
        public BallController ballController;

        public GameObject letterWithPropsPrefab;

        public GameObject poofPrefab;

        public GameObject environment;
        
        protected override void OnInitialize(IGameContext context)
        {
            instance = this;
            
            GameState = new GameState(this);
        }

        protected override IGameConfiguration GetConfiguration()
        {
            return ThrowBallsConfiguration.Instance;
        }

        protected override IGameState GetInitialState()
        {
            return GameState;
        }
    }
}