// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/01

using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace EA4S
{
    /// <summary>
    /// Tutorial framework
    /// </summary>
    [RequireComponent(typeof(TutorialUIPools))]
    public class TutorialUI : MonoBehaviour
    {
        public enum DrawLineMode
        {
            LineOnly,
            Finger,
            Arrow,
            FingerAndArrow
        }

        [Tooltip("In units x second")]
        public float DrawSpeed = 2;
        [Header("References")]
        public TutorialUIProp Finger;
        public TutorialUIPools Pools;

        public static TutorialUI I;
        [System.NonSerialized] public Camera Cam;
        const string ResourcePath = "Prefabs/UI/TutorialUI";
        const string TweenId = "TutorialUI";
        float actualDrawSpeed;
        Transform currMovingTarget;

        #region Unity

        void Awake()
        {
            I = this;
            Cam = Camera.main;
            actualDrawSpeed = Cam.fieldOfView * DrawSpeed / 45f;
        }

        void OnDestroy()
        {
            if (I == this) I = null;
            DOTween.Kill(TweenId);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Removes and tutorial element on screen
        /// </summary>
        /// <param name="_destroy">If TRUE, also destroys the TutorialUI gameObject</param>
        public static void Clear(bool _destroy)
        {
            if (I == null) return;

            if (_destroy) Destroy(I.gameObject);
            else {
                DOTween.Kill(TweenId);
                I.Finger.Hide(true);
                I.Pools.DespawnAll();
            }
        }

        /// <summary>
        /// Draws a straight line with the given options
        /// </summary>
        /// <param name="_from">Starting world position</param>
        /// <param name="_to">Ending world position</param>
        /// <param name="_mode">Draw mode (line only, finger, arrow, arrow + finger)</param>
        /// <param name="_persistent">If TRUE, the line will stay on screen until you <see cref="Clear"/> the TutorialUI,
        /// otherwise it will disappear automatically</param>
        /// <param name="_overlayed">If TRUE the line will always appear above other world elements,
        /// otherwise it will behave as a regular world object</param>
        public static void DrawLine(Vector3 _from, Vector3 _to, DrawLineMode _mode, bool _persistent = false, bool _overlayed = true)
        {
            Init();
            I.DoDrawLine(new[]{_from, _to}, PathType.Linear, _mode, _persistent, _overlayed);
        }

        /// <summary>
        /// Draws a curved line with the given options
        /// </summary>
        /// <param name="_path">A series of waypoints (world positions) between which the line will pass.
        /// IMPORTANT: the line drawn between the waypoints will use a CatmullRom curve, so you don't need too many waypoint to actually draw a curve</param>
        /// <param name="_mode">Draw mode (line only, finger, arrow, arrow + finger)</param>
        /// <param name="_persistent">If TRUE, the line will stay on screen until you <see cref="Clear"/> the TutorialUI,
        /// otherwise it will disappear automatically</param>
        /// <param name="_overlayed">If TRUE the line will always appear above other world elements,
        /// otherwise it will behave as a regular world object</param>
        public static void DrawLine(Vector3[] _path, DrawLineMode _mode, bool _persistent = false, bool _overlayed = true)
        {
            Init();
            I.DoDrawLine(_path, PathType.CatmullRom, _mode, _persistent, _overlayed);
        }

        #endregion

        #region Methods

        static void Init()
        {
            if (I != null) return;

            GameObject go = Instantiate(Resources.Load<GameObject>(ResourcePath));
            go.name = "[TutorialUI]";
        }

        void DoDrawLine(Vector3[] _path, PathType _pathType, DrawLineMode _mode, bool _persistent, bool _overlayed)
        {
            bool hasFinger = _mode == DrawLineMode.Finger || _mode == DrawLineMode.FingerAndArrow;
            bool hasArrow = _mode == DrawLineMode.Arrow || _mode == DrawLineMode.FingerAndArrow;
            TutorialUIProp arrow = null;
            Vector3 startPos = _path[0];

            TutorialUILineGroup lr = null;
            TutorialUITrailGroup tr = null;
            if (_persistent) {
                lr = Pools.SpawnLineGroup(this.transform, startPos, _overlayed);
                currMovingTarget = lr.transform;
            } else {
                tr = Pools.SpawnTrailGroup(this.transform, startPos, _overlayed);
                currMovingTarget = tr.transform;
            }

            if (hasFinger) Finger.Show(currMovingTarget, startPos);
            if (hasArrow) arrow = Pools.SpawnArrow(this.transform, startPos, _overlayed);

            TweenParams parms = TweenParams.Params.SetSpeedBased().SetEase(Ease.OutSine).SetId(TweenId);
            if (_persistent) {
                parms.OnUpdate(() => {
                    lr.AddPosition(lr.transform.position);
                });
                parms.OnComplete(() => {
                    if (hasFinger && lr.transform == currMovingTarget) Finger.Hide();
                });
            } else {
                parms.OnComplete(() => {
                    if (hasFinger && tr.transform == currMovingTarget) Finger.Hide();
                });
            }

            currMovingTarget.DOPath(_path, actualDrawSpeed, _pathType).SetAs(parms);
            if (hasArrow) {
                Tween t = arrow.transform.DOPath(_path, actualDrawSpeed, _pathType).SetLookAt(0.01f, arrow.transform.forward, arrow.transform.up)
                    .SetAs(parms);
                if (!_persistent) {
                    t.OnComplete(() => {
                        DOVirtual.DelayedCall(Mathf.Max(tr.Time - 0.2f, 0), () => arrow.Hide(), false).SetId(TweenId);
                    });
                }
            }
        }

        #endregion
    }
}