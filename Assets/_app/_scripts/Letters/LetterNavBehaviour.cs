using UnityEngine;
using ModularFramework.Helpers;
using System.Collections.Generic;
using DG.Tweening;
using Panda;

namespace EA4S
{
    /// <summary>
    /// Add AI Nav logic to letter puppet object.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class LetterNavBehaviour : MonoBehaviour
    {
        public GameObject WayPointPrefab;
        NavMeshAgent agent;
        Transform wayPoint;
        public Transform LockCameraPH;
        Vector3 LookAtCameraPosition = new Vector3(Camera.main.transform.position.x, 0, Camera.main.transform.position.z);
        public Vector3 HidePositionRight = new Vector3(17, 0.2f, -11);
        public Vector3 HidePositionLeft = new Vector3(-17, 0.2f, -11);
        bool lookAtCamera = false;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            wayPoint = Instantiate<Transform>(WayPointPrefab.transform);
        }

        #region Tasks

        [Task] 
        public void SetNavigation(string _stateName)
        {
            if (!agent)
                return;
            switch (_stateName) {
                case "Stop":
                    agent.Stop();
                    if (lookAtCamera)
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
                    agent.Stop();
                    transform.DOLookAt(LookAtCameraPosition, 0.1f).OnComplete(delegate
                        {
                            transform.LookAt(LookAtCameraPosition);
                        });
                    agent.speed = 3.5f;
                    //agent.Resume();
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
                    agent.enabled = false;
                    agent.transform.DOMove(HidePositionRight, 2).SetDelay(1).OnComplete(delegate
                        {
                            agent.transform.position = HidePositionRight;
                        });
                    
                    //agent.Resume();
                    //agent.speed = 10f;
                    break;
                case "BumpOut":
                    agent.ResetPath();
                    Vector3 newDestination = new List<Vector3>() { HidePositionRight, HidePositionLeft }.GetRandomElement();
                    agent.SetDestination(newDestination);
                    agent.speed = 10f; 
                    agent.Resume();
                    break;
                default:
                    break;
            }
            Task.current.Succeed();
        }

        [Task]
        public bool IsAnturaMoment()
        {
            return FastCrowd.FastCrowd.Instance.IsAnturaMoment;
        }

        #endregion

        #region Navigation

        /// <summary>
        /// Debug waypoint.
        /// </summary>
        void OnDrawGizmos()
        {
            if (wayPoint != null) {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, wayPoint.position);
            }
        }

        /// <summary>
        /// Repositioning waypoint.
        /// </summary>
        void RepositioningWaypoint(int _areaMask = 1)
        {
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
        void setLookAtCamera()
        {
            agent.Stop();
            if (!wayPoint)
                return;
            wayPoint.position = LookAtCameraPosition;
            agent.SetDestination(LookAtCameraPosition);
            agent.Resume();
        }

        void OnDestroy()
        {
            if (!wayPoint)
                return;
            GameObject.Destroy(wayPoint.gameObject);
        }

        #endregion

        #region Collisions

        void OnTriggerEnter(Collider other)
        {
            //void OnTriggerEnter(Collider other) {
            if (!agent)
                return;
            if (wayPoint && other == wayPoint.GetComponent<Collider>()) {
                RepositioningWaypoint();
            } else if (wayPoint && other.GetComponent<FastCrowd.AnturaController>()) {
                wayPoint.position = -wayPoint.position;
            }
        }

        #endregion
    }
}
