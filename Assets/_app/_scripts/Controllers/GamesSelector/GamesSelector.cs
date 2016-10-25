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
    [RequireComponent(typeof(GamesSelectorTrailsManager))]
    public class GamesSelector : MonoBehaviour
    {
        #region Events

        public static event Action OnComplete;
        static void DispatchOnComplete() { if (OnComplete != null) OnComplete(); }

        #endregion

        [Tooltip("Delay between the moment the last bubble is opened and the OnComplete event is dispatched")]
        public float EndDelay = 2;
        [Header("Debug")]
        public bool SimulateForDebug; // If TRUE creates games list for debug

        const string ResourceID = "GamesSelector";
        const string ResourcePath = "Prefabs/UI/" + ResourceID;
        static GamesSelector instance;
        GamesSelectorTrailsManager trailsManager;
        GamesSelectorTutorial tutorial;
        List<MiniGameData> games; // Set by Show
        GamesSelectorBubble mainBubble;
        readonly List<GamesSelectorBubble> bubbles = new List<GamesSelectorBubble>();
        TrailRenderer currTrail;
        Camera cam;
        Transform camT;
        bool cutAllowed = true;
        bool isDragging;
        int totOpenedBubbles;
        Sequence showTween;

        #region Unity

        void Awake()
        {
            instance = this;
            cam = Camera.main;
            camT = cam.transform;
            trailsManager = this.GetComponent<GamesSelectorTrailsManager>();
            tutorial = this.GetComponentInChildren<GamesSelectorTutorial>(true);
        }

        void Start()
        {
            if (SimulateForDebug) {
                games = new List<MiniGameData>() {
                    new MiniGameData() { Code = MiniGameCode.FastCrowd_alphabet },
                    new MiniGameData() { Code = MiniGameCode.DancingDots},
                    new MiniGameData() { Code = MiniGameCode.Balloons_counting}
                };
                Show(games);
            } else if (mainBubble == null) {
                mainBubble = this.GetComponentInChildren<GamesSelectorBubble>();
                mainBubble.gameObject.SetActive(false);
            }
        }

        void OnDestroy()
        {
            if (instance == this) instance = null;
            this.StopAllCoroutines();
            showTween.Kill(true);
        }

        void Update()
        {
            if (Time.timeScale <= 0) {
                // Prevent actions when pause menu is open
                if (isDragging) StopDrag();
                return;
            }

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.R)) {
                Destroy(this.gameObject);
                instance = null;
                Show(new List<MiniGameData>() {
                    new MiniGameData() { Code = MiniGameCode.FastCrowd_alphabet },
                    new MiniGameData() { Code = MiniGameCode.DancingDots  },
                    new MiniGameData() { Code = MiniGameCode.Balloons_counting },
                    new MiniGameData() { Code = MiniGameCode.Balloons_counting },
                    new MiniGameData() { Code = MiniGameCode.Maze }
                });
                return;
            }
#endif

            if (!Input.GetMouseButton(0) && !Input.GetMouseButtonUp(0)) return;

            Vector3 mouseP = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -camT.transform.position.z));
            if (Input.GetMouseButtonDown(0)) {
                // Start drag/click
                isDragging = true;
                currTrail = trailsManager.Spawn(mouseP);
            }
            if (isDragging) Update_Dragging(mouseP);
            if (Input.GetMouseButtonUp(0)) {
                // Stop drag/click
                StopDrag();
            }
        }

        void Update_Dragging(Vector3 _mouseP)
        {
            trailsManager.SetPosition(currTrail, _mouseP);
            if (cutAllowed) Update_CheckHitBubble(_mouseP);
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

            if (tutorial.isPlaying) tutorial.Stop();
            hitBubble.Open();
            totOpenedBubbles++;
            if (totOpenedBubbles == bubbles.Count) {
                // All bubbles opened: final routine
                cutAllowed = false;
                this.StartCoroutine(CO_EndCoroutine());
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
            gs.SimulateForDebug = false;
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
            float bubbleW = mainBubble.Main.GetComponent<Renderer>().bounds.size.x;
            float area = totBubbles * bubbleW + (totBubbles - 1) * bubblesDist;
            float startX = -area * 0.5f + bubbleW * 0.5f;
            for (int i = 0; i < totBubbles; ++i) {
                GamesSelectorBubble bubble = i == 0 ? mainBubble : (GamesSelectorBubble)Instantiate(mainBubble, this.transform);
                bubble.Setup(games[i].GetIconResourcePath(), startX + (bubbleW + bubblesDist) * i);
                Debug.Log("ResetAndLayout " + games[i].GetId() + " " + games[i].GetIconResourcePath());
                bubbles.Add(bubble);
            }
        }

        void StopDrag()
        {
            isDragging = false;
            if (currTrail != null) {
                trailsManager.Despawn(currTrail);
                currTrail = null;
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
                showTween.Insert(i * 0.05f, bubble.transform.DOScale(0.0001f, 0.6f).From().SetEase(Ease.OutElastic, 1, 0))
                    .InsertCallback(i * 0.1f, ()=> AudioManager.I.PlaySfx(Sfx.BaloonPop));
            }
            yield return showTween.WaitForCompletion();

            if (totOpenedBubbles == 0) tutorial.Play(bubbles);
        }

        IEnumerator CO_EndCoroutine()
        {
            yield return new WaitForSeconds(EndDelay);
            Debug.Log("GamesSelector > Complete");
            DispatchOnComplete();
        }

        #endregion
    }
}