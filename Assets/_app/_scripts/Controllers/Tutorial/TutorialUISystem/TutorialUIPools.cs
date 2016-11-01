// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/01

using System.Collections.Generic;
using UnityEngine;

namespace EA4S
{
    public class TutorialUIPools : MonoBehaviour
    {
        public TrailRenderer TrailPrefab;

        [System.NonSerialized] public TutorialUI TutorialUI;
        readonly List<TrailRenderer> trailsPool = new List<TrailRenderer>();

        #region Public Methods

        public TrailRenderer SpawnTrail(Vector3 _position)
        {
            foreach (TrailRenderer tr in trailsPool) {
                if (tr.gameObject.activeSelf) continue;
                tr.transform.position = _position;
                tr.Clear();
                return tr;
            }
            TrailRenderer newTrail = Instantiate(TrailPrefab, _position, Quaternion.identity, TrailPrefab.transform.parent) as TrailRenderer;
            trailsPool.Add(newTrail);

            // Adapt size to camera
            // 1 : 45 = x : fov
            newTrail.startWidth = Camera.main.fieldOfView * TutorialUI.TrailSize / 45f;
            newTrail.endWidth = newTrail.startWidth * 0.4f;

            return newTrail;
        }

        #endregion
    }
}