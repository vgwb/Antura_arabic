using UnityEngine;
using System.Collections;
using DG.Tweening;
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
        public Transform LockCameraPH;
        Vector3 LookAtCameraPosition = new Vector3(Camera.main.transform.position.x, 0, Camera.main.transform.position.z);
        public Vector3 HidePositionRight = new Vector3(25, 0, -20);
        public Vector3 HidePositionLeft = new Vector3(-25, 0, -20);
        bool lookAtCamera = false;

        void Start() {
            agent = GetComponent<NavMeshAgent>();
            wayPoint = Instantiate<Transform>(WayPointPrefab.transform);
        }

        #region Tasks
        [Task] 
        public void SetNavigation(string _stateName) {
            switch (_stateName) {
                case "Stop":
                    agent.Stop();
                    if(lookAtCamera)
                        transform.DOLookAt(LookAtCameraPosition, 0.1f);
                    lookAtCamera = !lookAtCamera;
                    break;
                case "Walk":
                    if (lookAtCamera)
                        setLookAtCamera();
                    else
                        RepositioningWaypoint();
                    lookAtCamera = !lookAtCamera;
                    agent.speed = 3.5f;
                    agent.Resume();
                    break;
                case "Hold":
                    transform.DOLookAt(LookAtCameraPosition, 0.1f);
                    agent.speed = 3.5f;
                    agent.Resume();
                    break;
                case "Run":
                    RepositioningWaypoint();
                    agent.speed = 10f;
                    agent.Resume();
                    break;
                case "Ninja":
                    RepositioningWaypoint();
                    agent.speed = 3f;
                    agent.Resume();
                    break;
                case "GoOut": 
                    agent.Stop(); 
                    agent.transform.position = HidePositionLeft;
                    //agent.Resume();
                    //agent.speed = 10f;
                    break;
                default:
                    break;
            }
            Task.current.Succeed();
        }

        [Task]
        public bool IsAnturaMoment() {
            return FastCrowd.FastCrowd.Instance.IsAnturaMoment;
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
        void RepositioningWaypoint(int _areaMask = 1) {
            if (!wayPoint)
                return;
            Vector3 randomValidPosition;
            //RandomPoint(Target.position, 10f, out randomValidPosition);
            GameplayHelper.RandomPointInWalkableArea(wayPoint.position, 15f, out randomValidPosition, _areaMask);
            wayPoint.position = randomValidPosition;
            agent.SetDestination(wayPoint.position);
        }

        /// <summary>
        /// Set waypoint to look in camera direction.
        /// </summary>
        void setLookAtCamera() {
            agent.Stop();
            if (!wayPoint)
                return;
            wayPoint.position = LookAtCameraPosition;
            agent.SetDestination(LookAtCameraPosition);
            agent.Resume();
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
            if (!agent)
                return;
            if (wayPoint && other == wayPoint.GetComponent<Collider>()) {
                RepositioningWaypoint();
            } else if(wayPoint && other.GetComponent<AnturaController>()) {
                wayPoint.position = -wayPoint.position;
            }
        }


            #endregion
        }
}
