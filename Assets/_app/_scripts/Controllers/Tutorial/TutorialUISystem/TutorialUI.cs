// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/01

using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace EA4S
{
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
        TutorialUITrailGroup currTrail;

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

        public static void DrawLine(Vector3 _from, Vector3 _to, DrawLineMode _mode, bool _persistent = false, bool _overlayed = true)
        {
            Init();
            I.DoDrawLine(new[]{_from, _to}, PathType.Linear, _mode, _persistent, _overlayed);
        }

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
            TutorialUITrailGroup tr = currTrail = Pools.SpawnTrailGroup(this.transform, startPos, _overlayed);
            if (hasFinger) Finger.Show(tr.transform, startPos);
            if (hasArrow) arrow = Pools.SpawnArrow(this.transform, startPos, _overlayed);
            TweenParams parms = TweenParams.Params.SetSpeedBased().SetEase(Ease.OutSine).SetId(TweenId)
                .OnComplete(() => {
                    if (hasFinger && tr == currTrail) Finger.Hide();
                });
            tr.transform.DOPath(_path, actualDrawSpeed, _pathType).SetAs(parms);
            if (hasArrow) {
                arrow.transform.DOPath(_path, actualDrawSpeed, _pathType).SetLookAt(0.01f, arrow.transform.forward, arrow.transform.up)
                    .SetAs(parms)
                    .OnComplete(() => {
                        DOVirtual.DelayedCall(Mathf.Max(tr.Time - 0.2f, 0), () => arrow.Hide(), false).SetId(TweenId);
                    });
            }
        }

        #endregion
    }
}