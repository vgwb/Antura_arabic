using UnityEngine;
using System.Collections;
using Panda;

namespace EA4S {
    /// <summary>
    /// Add AI Nav logic to letter puppet object.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class LetterNavBehaviour : MonoBehaviour {
        public GameObject WayPointPrefab;
        NavMeshAgent agent;
        Transform wayPoint;
        Vector3 LookAtCameraPosition = new Vector3(0, 0.2f, -19);

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
                    agent.SetDestination(LookAtCameraPosition);
                    agent.speed = 0.1f;
                    agent.angularSpeed = 900;
                    agent.Resume();
                    break;
                case "Walk":
                    RepositioningWaypoint();
                    agent.speed = 3.5f;
                    agent.angularSpeed = 200;
                    agent.Resume();
                    break;
                case "Hold":
                    agent.SetDestination(LookAtCameraPosition);
                    agent.angularSpeed = 200;
                    agent.speed = 3.5f;
                    agent.Resume();
                    break;
                case "Run":
                    RepositioningWaypoint();
                    agent.angularSpeed = 200;
                    agent.speed = 10f;
                    agent.Resume();
                    break;
                case "Ninja":
                    RepositioningWaypoint();
                    agent.angularSpeed = 200;
                    agent.speed = 3.5f;
                    agent.Resume();
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
            if (!wayPoint)
                return;
            Vector3 randomValidPosition;
            //RandomPoint(Target.position, 10f, out randomValidPosition);
            GameplayHelper.RandomPointInWalkableArea(wayPoint.position, 15f, out randomValidPosition);
            wayPoint.position = randomValidPosition;
            agent.SetDestination(wayPoint.position);
        }

        void OnDestroy() {
            if (!wayPoint)
                return;
            GameObject.Destroy(wayPoint.gameObject);
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
