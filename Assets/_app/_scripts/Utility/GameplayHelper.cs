using UnityEngine;
using System.Collections;

namespace EA4S
{
    public class GameplayHelper
    {

        /// <summary>
        /// Get random point on walkable area of navmesh (id area 1).
        /// </summary>
        /// <param name="_center"></param>
        /// <param name="_range"></param>
        /// <param name="_result"></param>
        /// <returns></returns>
        public static bool RandomPointInWalkableArea(Vector3 _center, float _range, out Vector3 _result, int _areaMask = 1)
        {
            Vector3 randomPoint = _center + Random.insideUnitSphere * (_range + Random.Range(-_range / 2f, _range / 2f));
            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(randomPoint, out hit, 10.0f, _areaMask)) {
                _result = hit.position;
                return true;
            }
            _result = Vector3.zero;
            return false;
        }
    }
}
