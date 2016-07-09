using UnityEngine;
using System.Collections;
using Panda;

namespace CGL.Antura {
    [RequireComponent(typeof(NavMeshAgent))]
    public class LetterNavBehaviour : MonoBehaviour {
        public GameObject WayPointPrefab;
        NavMeshAgent agent;
        Transform wayPoint;

        void Start() {
            agent = GetComponent<NavMeshAgent>();
            //wayPoint = new GameObject().transform;
            //wayPoint.gameObject.AddComponent<BoxCollider>().size = new Vector3(2,2,2);
            wayPoint = Instantiate<Transform>(WayPointPrefab.transform);
        }

        #region Tasks
        [Task]
        public void SetNavigation(string _stateName) {
            switch (_stateName) {
                case "Stop":
                    if (agent) {
                        agent.Stop();
                    }
                    break;
                case "Walk":
                    RepositioningWaypoint();
                    agent.speed = 3.5f;
                    agent.Resume();
                    break;
                case "Hold":
                    agent.SetDestination(new Vector3(0, 9, -33));
                    break;
                default:
                    break;
            }
            Task.current.Succeed();
        }
        #endregion

        #region Navigation
        /// <summary>
        /// Debug waypoint.
        /// </summary>
        void OnDrawGizmos() {
            if (wayPoint != null) {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, wayPoint.position);
            }
        }

        /// <summary>
        /// Repositioning waypoint.
        /// </summary>
        void RepositioningWaypoint() {
            Vector3 randomValidPosition;
            //RandomPoint(Target.position, 10f, out randomValidPosition);
            RandomPointInWalkableArea(wayPoint.position, 15f, out randomValidPosition);
            wayPoint.position = randomValidPosition;
            agent.SetDestination(wayPoint.position);
        }

        /// <summary>
        /// Get random point on 
        /// </summary>
        /// <param name="_center"></param>
        /// <param name="_range"></param>
        /// <param name="_result"></param>
        /// <returns></returns>
        bool RandomPointInWalkableArea(Vector3 _center, float _range, out Vector3 _result) {
            for (int i = 0; i < 30; i++) {
                Vector3 randomPoint = _center + Random.insideUnitSphere * (_range + Random.Range(-_range / 2f, _range / 2f));
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, 1)) {
                    _result = hit.position;
                    return true;
                }
            }
            _result = Vector3.zero;
            return false;
        }

        #endregion

        #region Collisions
        void OnTriggerEnter(Collider other) {
            //void OnTriggerEnter(Collider other) {
            if (agent && wayPoint && other != wayPoint.GetComponent<Collider>()) {

            } else {
                RepositioningWaypoint();
            }
        }
        #endregion
    }
}
