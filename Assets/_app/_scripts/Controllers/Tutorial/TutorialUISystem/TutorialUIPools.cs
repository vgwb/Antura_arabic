// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/01

using System.Collections.Generic;
using UnityEngine;

namespace EA4S
{
    public class TutorialUIPools : MonoBehaviour
    {
        public TutorialUITrailGroup TrailGroupPrefab;
        public TutorialUILineGroup LineGroupPrefab;
        public TutorialUIProp ArrowPrefab;
        public TutorialUIClicker ClickerPrefab;

        readonly List<TutorialUITrailGroup> trailsPool = new List<TutorialUITrailGroup>();
        readonly List<TutorialUILineGroup> linesPool = new List<TutorialUILineGroup>();
        readonly List<TutorialUIProp> arrowsPool = new List<TutorialUIProp>();
        readonly List<TutorialUIProp> clickersPool = new List<TutorialUIProp>();

        #region Unity

        void Start()
        {
            TrailGroupPrefab.gameObject.SetActive(false);
            LineGroupPrefab.gameObject.SetActive(false);
            ArrowPrefab.gameObject.SetActive(false);
            ClickerPrefab.gameObject.SetActive(false);
        }

        #endregion

        #region Public Methods

        public void DespawnAll()
        {
            foreach (TutorialUITrailGroup tr in trailsPool) tr.Despawn();
            foreach (TutorialUILineGroup lr in linesPool) lr.Despawn();
            foreach (TutorialUIProp arrow in arrowsPool) arrow.Hide(true);
            foreach (TutorialUIProp clicker in clickersPool) clicker.Hide(true);
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

        public TutorialUILineGroup SpawnLineGroup(Transform _parent, Vector3 _position, bool _overlayed)
        {
            TutorialUILineGroup lineG = null;
            foreach (TutorialUILineGroup lr in linesPool) {
                if (lr.gameObject.activeSelf) continue;
                lineG = lr;
                break;
            }
            if (lineG == null) {
                lineG = Instantiate(LineGroupPrefab, _parent) as TutorialUILineGroup;
                linesPool.Add(lineG);
            }
            lineG.Spawn(_position, _overlayed);
            return lineG;
        }

        public TutorialUIProp SpawnArrow(Transform _parent, Vector3 _position, bool _overlayed)
        { return SpawnProp(arrowsPool, ArrowPrefab, _parent, _position, _overlayed); }

        public TutorialUIProp SpawnClicker(Transform _parent, Vector3 _position, bool _overlayed)
        { return SpawnProp(clickersPool, ClickerPrefab, _parent, _position, _overlayed); }

        #endregion

        #region Methods

        public TutorialUIProp SpawnProp(List<TutorialUIProp> _propList, TutorialUIProp _propPrefab, Transform _parent, Vector3 _position, bool _overlayed)
        {
            TutorialUIProp prop = null;
            foreach (TutorialUIProp p in _propList) {
                if (p.gameObject.activeSelf) continue;
                prop = p;
                break;
            }
            if (prop == null) {
                prop = Instantiate(_propPrefab, _parent) as TutorialUIProp;
                _propList.Add(prop);
            }
            prop.Show(_parent, _position, _overlayed);
            return prop;
        }

        #endregion
    }
}