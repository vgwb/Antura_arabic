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
        [Tooltip("In units x second")]
        public float DrawSpeed = 2;
        public float TrailSize = 0.5f;
        [Header("References")]
        public TutorialUIPools Pools;

        static TutorialUI I;
        const string ResourcePath = "Prefabs/UI/TutorialUI";
        const string TweenId = "TutorialUI";
        float actualDrawSpeed;

        #region Unity

        void Awake()
        {
            I = this;
            Pools.TutorialUI = this;
            actualDrawSpeed = Camera.main.fieldOfView * DrawSpeed / 45f;
        }

        void OnDestroy()
        {
            if (I == this) I = null;
        }

        #endregion

        #region Public Methods

        public static void DrawLine(Vector3 _from, Vector3 _to, bool _hasEndArrow, bool _persistent)
        { I.DoDrawLine(new[]{_from, _to}, PathType.Linear, _hasEndArrow, _persistent); }

        public static void DrawLine(Vector3[] _path, bool _hasEndArrow, bool _persistent)
        { I.DoDrawLine(_path, PathType.Linear, _hasEndArrow, _persistent); }

        #endregion

        #region Methods

        static void Init()
        {
            if (I != null) return;

            GameObject go = Instantiate(Resources.Load<GameObject>(ResourcePath));
            go.name = "[TutorialUI]";
        }

        void DoDrawLine(Vector3[] _path, PathType _pathType, bool _hasEndArrow, bool _persistent)
        {
            Init();

            Debug.Log("TutorialUI.DrawLine > " + _path.Length);
            TrailRenderer tr = Pools.SpawnTrail(_path[0]);
            TweenParams parms = TweenParams.Params.SetSpeedBased().SetEase(Ease.OutSine);
            if (_path.Length == 2) tr.transform.DOMove(_path[1], actualDrawSpeed).SetAs(parms);
            else tr.transform.DOPath(_path, actualDrawSpeed, _pathType).SetAs(parms);
        }

        #endregion
    }
}