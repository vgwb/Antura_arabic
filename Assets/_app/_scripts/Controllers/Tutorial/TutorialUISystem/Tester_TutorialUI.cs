// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/01

using System.Collections.Generic;
using UnityEngine;

namespace EA4S
{
    public class Tester_TutorialUI : MonoBehaviour
    {
        public float CameraDistance = 20;
        public float MinPointDistance = 0.3f;

        readonly List<Vector3> storedPs = new List<Vector3>();
        bool isDragging;

        #region Unity

        void Update()
        {
            if (Input.GetMouseButtonDown(0)) {
                isDragging = true;
                storedPs.Add(MouseWorldPosition());
            } else if (isDragging) {
                Vector3 p = MouseWorldPosition();
                if (Vector3.Distance(storedPs[storedPs.Count - 1], p) >= MinPointDistance) storedPs.Add(p);
            }

            if (Input.GetMouseButtonUp(0)) {
                isDragging = false;
                storedPs.Add(MouseWorldPosition());
                TutorialUI.DrawLine(storedPs.ToArray(), false, false);
                storedPs.Clear();
            }
        }

        #endregion

        #region Helpers

        Vector3 MouseWorldPosition()
        {
            return Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane + CameraDistance));
        }

        #endregion
    }
}