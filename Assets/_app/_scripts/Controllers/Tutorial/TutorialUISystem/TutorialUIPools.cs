// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/01

using System.Collections.Generic;
using UnityEngine;

namespace EA4S
{
    public class TutorialUIPools : MonoBehaviour
    {
        public TutorialUITrailGroup TrailGroupPrefab;
        public TutorialUIProp ArrowPrefab;

        readonly List<TutorialUITrailGroup> trailsPool = new List<TutorialUITrailGroup>();
        readonly List<TutorialUIProp> arrowsPool = new List<TutorialUIProp>();

        #region Unity

        void Start()
        {
            TrailGroupPrefab.gameObject.SetActive(false);
            ArrowPrefab.gameObject.SetActive(false);
        }

        #endregion

        #region Public Methods

        public void DespawnAll()
        {
            foreach (TutorialUITrailGroup tr in trailsPool) tr.Despawn();
            foreach (TutorialUIProp arrow in arrowsPool) arrow.Hide(true);
        }

        public TutorialUITrailGroup SpawnTrailGroup(Transform _parent, Vector3 _position, bool _overlayed)
        {
            TutorialUITrailGroup trailG = null;
            foreach (TutorialUITrailGroup tr in trailsPool) {
                if (tr.gameObject.activeSelf) continue;
                trailG = tr;
                break;
            }
            if (trailG == null) {
                trailG = Instantiate(TrailGroupPrefab, _parent) as TutorialUITrailGroup;
                trailsPool.Add(trailG);
            }
            trailG.Spawn(_position, _overlayed);
            return trailG;
        }

        public TutorialUIProp SpawnArrow(Transform _parent, Vector3 _position, bool _overlayed)
        {
            TutorialUIProp arrow = null;
            foreach (TutorialUIProp arr in arrowsPool) {
                if (arr.gameObject.activeSelf) continue;
                arrow = arr;
                break;
            }
            if (arrow == null) {
                arrow = Instantiate(ArrowPrefab, _parent) as TutorialUIProp;
                arrowsPool.Add(arrow);
            }
            arrow.Show(_parent, _position, _overlayed);
            return arrow;
        }

        #endregion
    }
}