// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/01

using DG.Tweening;
using UnityEngine;

namespace EA4S
{
    /// <summary>
    /// Can be a single trail, or a collection of multiple trails
    /// </summary>
    public class TutorialUITrailGroup : MonoBehaviour
    {
        [System.NonSerialized] public TrailRenderer[] Trails;
        bool initialized;
        float[] defStartWidths, defEndWidths;
        float time; // Highest trail time between all trails
        Vector3 lastPos;
        bool isWaitingToDespawn;
        Tween waitingTween;

        #region Unity + Init

        void Init()
        {
            if (initialized) return;

            initialized = true;

            Trails = this.GetComponentsInChildren<TrailRenderer>(true);
            int count = Trails.Length;
            defStartWidths = new float[count];
            defEndWidths = new float[count];
            for (int i = 0; i < count; ++i) {
                TrailRenderer tr = Trails[i];
                if (time < tr.time) time = tr.time;
                defStartWidths[i] = tr.startWidth;
                defEndWidths[i] = tr.endWidth;
            }
        }

        void Awake()
        {
            Init();
        }

        void OnDestroy()
        {
            waitingTween.Kill();
        }

        void LateUpdate()
        {
            if (isWaitingToDespawn) return;

            if (lastPos - this.transform.position == Vector3.zero) {
                isWaitingToDespawn = true;
                waitingTween = DOVirtual.DelayedCall(time, Despawn, false);
            } else lastPos = this.transform.position;
        }

        #endregion

        #region Public Methods

        public void Spawn(Vector3 _position)
        {
            Init();
            this.gameObject.SetActive(true);
            this.transform.position = _position;
            lastPos = _position - Vector3.one;
            for (int i = 0; i < Trails.Length; ++i) {
                TrailRenderer tr = Trails[i];
                tr.startWidth = TutorialUI.I.Cam.fieldOfView * defStartWidths[i] / 45f;
                tr.endWidth = TutorialUI.I.Cam.fieldOfView * defEndWidths[i] / 45f;
            }
        }

        public void Despawn()
        {
            isWaitingToDespawn = false;
            waitingTween.Kill();
            foreach (TrailRenderer tr in Trails) tr.Clear();
            this.gameObject.SetActive(false);
        }

        #endregion
    }
}