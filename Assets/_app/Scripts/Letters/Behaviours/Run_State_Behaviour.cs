using UnityEngine;
using System.Collections;
using UniRx;


namespace CGL.Antura {
    public class Run_State_Behaviour : BaseBehaviour {

        public float Speed;

        public GameObject WayPointPrefab;
        /// <summary>
        /// Target.
        /// </summary>
        Transform Target;
        
        NavMeshAgent agent;

        #region Behaviour base
        public override void OnStartBehaviour() {
            base.OnStartBehaviour();

            agent = GetComponent<NavMeshAgent>();
            if (!Target) { 
                Target = Instantiate<Transform>(WayPointPrefab.transform);
                Target.SetParent(transform.parent, false);
                Target.position = transform.position;
            }
            agent.SetDestination(Target.position);

            /// <summary>
            /// Monitoring Target
            /// </summary>
            this.ObserveEveryValueChanged(x => Target.position).Subscribe(_ => {
                if (Target) {
                    agent.SetDestination(Target.position);
                }
            }).AddTo(this);

            /// <summary>
            /// Monitoring Speed
            /// </summary>
            this.ObserveEveryValueChanged(x => Speed).Subscribe(_ => {
                if (Target) {
                    agent.speed = Speed;
                }
            }).AddTo(this);

        }

        public override void OnEndBehaviour() {
            base.OnEndBehaviour();
        }

        public override void OnUpdateBehaviour() {
            base.OnUpdateBehaviour();
        }

        #endregion

        void OnDrawGizmos() {
            if (Target != null) {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, Target.position);
            }
        }

        void RepositioningWaypoint() {
            Vector3 randomValidPosition;
            //RandomPoint(Target.position, 10f, out randomValidPosition);
            RandomPointInWalkableArea(Target.position, 15f, out randomValidPosition);
            Target.position = randomValidPosition;
            agent.SetDestination(Target.position);
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
                Vector3 randomPoint = _center + Random.insideUnitSphere * (_range + Random.Range(-_range/2f, _range / 2f));
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, 1)) {
                    _result = hit.position;
                    return true;
                }
            }
            _result = Vector3.zero;
            return false;
        }

        #region collisions
        void OnTriggerStay(Collider other) {
        //void OnTriggerEnter(Collider other) {
            if(Target && other != Target.GetComponent<Collider>())
                return;
            RepositioningWaypoint();
        }
        #endregion

    }
}
