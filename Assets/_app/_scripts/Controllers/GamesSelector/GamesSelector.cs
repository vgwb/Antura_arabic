// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/10/23

using System;
using System.Collections;
using System.Collections.Generic;
using DG.DeExtensions;
using DG.Tweening;
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

        [Header("Debug")]
        public bool SimulateForDebug; // If TRUE creates games list for debug

        const string ResourceID = "GamesSelector";
        const string ResourcePath = ResourceID;
        static GamesSelector instance;
        List<MiniGameData> games; // Set by Show
        GamesSelectorBubble mainBubble;
        readonly List<GamesSelectorBubble> bubbles = new List<GamesSelectorBubble>();
        Camera cam;
        Transform camT;
        bool active, isDragging;
        int totOpenedBubbles;
        Sequence showTween;

        #region Unity

        void Awake()
        {
            instance = this;
            cam = Camera.main;
            camT = cam.transform;
        }

        void Start()
        {
            if (SimulateForDebug) {
                games = new List<MiniGameData>();
                for (int i = 0; i < 3; ++i) {
                    MiniGameData mgData = new MiniGameData() {  Code = MiniGameCode.FastCrowd_alphabet };
                    games.Add(mgData);
                }
                Show(games);
            }
        }

        void OnDestroy()
        {
            if (instance == this) instance = null;
            this.StopAllCoroutines();
            showTween.Kill();
        }

        void Update()
        {
            if (!active || !Input.GetMouseButton(0) && !Input.GetMouseButtonUp(0)) return;

            Vector3 mouseP = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -camT.transform.position.z));
            if (Input.GetMouseButtonDown(0)) {
                // Start drag/click
                isDragging = true;
            }
            if (isDragging) UpdateDragging(mouseP);
            if (Input.GetMouseButtonUp(0)) {
                // Stop drag/click
                isDragging = false;
            }
        }

        void UpdateDragging(Vector3 _mouseP)
        {
            Update_CheckHitBubble(_mouseP);
        }

        void Update_CheckHitBubble(Vector3 _mouseP)
        {
            _mouseP.z -= 10;
            RaycastHit hit;
            if (!Physics.Raycast(new Ray(_mouseP, camT.forward), out hit)) return;

            GamesSelectorBubble hitBubble = null;
            foreach (GamesSelectorBubble bubble in bubbles) {
                if (hit.transform != bubble.Cover.transform) continue;
                hitBubble = bubble;
                break;
            }
            if (hitBubble == null) return;

            hitBubble.Open();
            totOpenedBubbles++;
            if (totOpenedBubbles == bubbles.Count) {
                // All bubbles opened: final routine
                active = false;
            }
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
            gs.StartCoroutine(gs.CO_AnimateEntrance());
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
            bubbles.Clear();

            // Layout
            const float bubblesDist = 0.1f;
            int totBubbles = games.Count;
            float bubbleW = mainBubble.Bg.GetComponent<Renderer>().bounds.size.x;
            float area = totBubbles * bubbleW + (totBubbles - 1) * bubblesDist;
            float startX = -area * 0.5f + bubbleW * 0.5f;
            for (int i = 0; i < totBubbles; ++i) {
                GamesSelectorBubble bubble = i == 0 ? mainBubble : (GamesSelectorBubble)Instantiate(mainBubble, this.transform);
                bubble.Setup(games[i].GetIconResourcePath(), startX + (bubbleW + bubblesDist) * i);
                bubbles.Add(bubble);
            }
        }

        IEnumerator CO_AnimateEntrance()
        {
            foreach (GamesSelectorBubble bubble in bubbles) bubble.gameObject.SetActive(false);
            yield return null;
            yield return null;

            showTween = DOTween.Sequence();
            for (int i = 0; i < bubbles.Count; ++i) {
                GamesSelectorBubble bubble = bubbles[i];
                bubble.gameObject.SetActive(true);
                showTween.Insert(i * 0.05f, bubble.transform.DOScale(0.0001f, 0.6f).From().SetEase(Ease.OutElastic, 1, 0));
            }
            yield return showTween.WaitForCompletion();

            active = true;
        }

        #endregion
    }
}