// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/10/23

using System;
using System.Collections.Generic;
using DG.DeExtensions;
using EA4S.Db;
using UnityEngine;

namespace EA4S
{
    /// <summary>
    /// The GamesSelector can be instantiated in any scene.
    /// To create and start it automatically, just call the static <see cref="Show"/> method.
    /// When the user has opened all bubbles and the app should move on, the <see cref="OnComplete"/> event will be dispatched.
    /// </summary>
    public class GamesSelector : MonoBehaviour
    {
        #region Events

        public static event Action OnComplete;
        static void DispatchOnComplete() { if (OnComplete != null) OnComplete(); }

        #endregion

        public bool SimulateForDebug; // If TRUE creates games list for debug

        const string ResourceID = "GamesSelector";
        const string ResourcePath = ResourceID;
        static GamesSelector instance;
        List<MiniGameData> games; // Set by Show
        GamesSelectorBubble mainBubble;
        readonly List<GamesSelectorBubble> bubbles = new List<GamesSelectorBubble>();

        #region Unity

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            if (SimulateForDebug) {
                games = new List<MiniGameData>();
                for (int i = 0; i < 3; ++i) {
                    MiniGameData mgData = new MiniGameData() { Id = "DancingDots" };
                    games.Add(mgData);
                }
                ResetAndLayout();
            }
        }

        void OnDestroy()
        {
            if (instance == this) instance = null;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Instantiates and starts the GamesSelector routine,
        /// or just resets and starts it if it's already present in the scene.
        /// </summary>
        /// <param name="_games">The list of selected games, ordered as they will happen in the session</param>
        public static void Show(List<MiniGameData> _games)
        {
            GamesSelector gs;
            if (instance != null) {
                gs = instance;
            } else {
                GameObject go = Instantiate(Resources.Load<GameObject>(ResourcePath));
                go.name = ResourceID;
                gs = go.GetComponent<GamesSelector>();
            }
            gs.games = _games;
            gs.ResetAndLayout();
        }

        #endregion

        #region Methods

        void ResetAndLayout()
        {
            // Reset
            if (mainBubble == null) mainBubble = this.GetComponentInChildren<GamesSelectorBubble>();
            foreach (GamesSelectorBubble bubble in bubbles) {
                if (bubble != mainBubble) Destroy(bubble.gameObject);
            }

            // Layout
            int totBubbles = games.Count;
            float bubbleW = mainBubble.Bg.GetComponent<Renderer>().bounds.size.x;
            const float bubblesDist = 0.1f;
            float area = totBubbles * bubbleW + (totBubbles - 1) * bubblesDist;
            float startX = -area * 0.5f + bubbleW * 0.5f;
            for (int i = 0; i < totBubbles; ++i) {
                GamesSelectorBubble bubble = i == 0 ? mainBubble : (GamesSelectorBubble)Instantiate(mainBubble, this.transform);
                bubble.Setup(games[i].GetIconResourcePath(), startX + (bubbleW + bubblesDist) * i);
            }
        }

        #endregion
    }
}