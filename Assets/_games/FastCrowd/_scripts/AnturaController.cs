using UnityEngine;
using System.Collections;

namespace EA4S {
    public class AnturaController : MonoBehaviour {

        public GameObject WayPointPrefab;
        NavMeshAgent agent;
        Transform wayPoint;
        public Vector3 HidePosition = new Vector3(25, 0, -20);
        public bool IsAnturaTime = false;

        // Use this for initialization
        void Start() {
            agent = GetComponentInChildren<NavMeshAgent>();
            wayPoint = Instantiate<Transform>(WayPointPrefab.transform);
            wayPoint.name = "AnturaWP";
            wayPoint.position = new Vector3(-18, 0, -10);
            agent.SetDestination(HidePosition);
        }

        // Update is called once per frame
        void Update() {
              
        }
        
        void setAnturaTime(bool _isAnturaTime) {
            IsAnturaTime = _isAnturaTime;
            if(IsAnturaTime)
                RepositioningWaypoint();
            else
                agent.SetDestination(HidePosition);
        }

        void OnTriggerEnter(Collider other) {
            //void OnTriggerEnter(Collider other) {
            if (agent && wayPoint && other == wayPoint.GetComponent<Collider>() && IsAnturaTime) {
                RepositioningWaypoint();
            }
        }

        void OnTriggerStay(Collider other) {
            if (!agent)
                return;
            if (wayPoint && other == wayPoint.GetComponent<Collider>() && IsAnturaTime) {
                RepositioningWaypoint();
            }
        }

        void RepositioningWaypoint(int _areaMask = 1) {
            if (!wayPoint)
                return;
            Vector3 randomValidPosition;
            //RandomPoint(Target.position, 10f, out randomValidPosition);
            do {
                GameplayHelper.RandomPointInWalkableArea(transform.position, 30f, out randomValidPosition, _areaMask);
                wayPoint.position = randomValidPosition;
            } while (randomValidPosition == Vector3.zero);
            agent.SetDestination(wayPoint.position);
        }

        /// <summary>
        /// Debug waypoint.
        /// </summary>
        void OnDrawGizmos() {
            if (wayPoint != null) {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, wayPoint.position);
            }
        }

        #region events delegates
        /// <summary>
        /// Timer custom events delegates.
        /// </summary>
        /// <param name="_data"></param>
        private void GameplayTimer_OnCustomEvent(GameplayTimer.CustomEventData _data) {
            //Debug.LogFormat("Custom Event {0} at {1} sec.", _data.Name, _data.Time);
            switch (_data.Name) {
                case "AnturaStart":
                    setAnturaTime(true);
                    break;
                case "AnturaEnd":
                    setAnturaTime(false);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region events subscription

        void OnEnable() {
            GameplayTimer.OnCustomEvent += GameplayTimer_OnCustomEvent;
        }

        void OnDisable() {
            GameplayTimer.OnCustomEvent -= GameplayTimer_OnCustomEvent;
        }

        #endregion
    }
}
