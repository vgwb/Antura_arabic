using System;
using EA4S.Antura;
using EA4S.Helpers;
using UnityEngine;

namespace EA4S.AnturaSpace
{
    public class AnturaLocomotion : MonoBehaviour
    {
        Transform target;
        bool rotateAsTarget;

        const float WALK_SPEED = 5.0f;
        const float RUN_SPEED = 15.0f;

        [NonSerialized]
        public AnturaAnimationController AnimationController;

        public bool HasReachedTarget
        {
            get
            {
                return IsNearTargetPosition && IsNearTargetRotation;
            }
        }

        bool IsNearTargetPosition
        {
            get
            {
                if (target == null)
                    return true;

                var distance = target.position - transform.position;
                distance.y = 0;

                return distance.magnitude < 0.5f;
            }
        }

        bool IsNearTargetRotation
        {
            get
            {
                if (target == null || !rotateAsTarget)
                    return true;

                var dot = Mathf.Max(0, Vector3.Dot(target.forward.normalized, transform.forward.normalized));
                return dot > 0.9f;
            }
        }

        bool IsSleeping
        {
            get
            {
                return AnimationController.State == AnturaAnimationStates.sleeping;
            }
        }

        public bool IsJumping
        {
            get
            {
                return AnimationController.IsJumping || AnimationController.IsAnimationActuallyJumping;
            }
        }

        public float PlanarDistanceFromTarget { get
            {
                if (target == null)
                    return 0;

                var distance = target.position - transform.position;
                distance.y = 0;

                return distance.magnitude;
            } }

        public float DistanceFromTarget
        {
            get
            {
                if (target == null)
                    return 0;

                var distance = target.position - transform.position;

                return distance.magnitude;
            }
        }

        public float TargetHeight
        {
            get
            {
                if (target == null)
                    return 0;

                return target.position.y;
            }
        }

        public void SetTarget(Transform target, bool rotateAsTarget)
        {
            this.target = target;
            this.rotateAsTarget = rotateAsTarget;
        }


        void Awake()
        {
            AnimationController = GetComponent<AnturaAnimationController>();
        }

        void Update()
        {
            if (!IsSleeping && !IsJumping && target != null)
            {
                var distance = target.position - transform.position;
                distance.y = 0;

                var distMagnitude = distance.magnitude;
                float speed = 0;

                if (!IsNearTargetPosition)
                {
                    float speedFactor = Mathf.Lerp(0, 1, distMagnitude / 10);
                    speed = Mathf.Lerp(WALK_SPEED, RUN_SPEED, speedFactor) * Mathf.Lerp(0, 1, distMagnitude);
                    AnimationController.SetWalkingSpeed(speedFactor);
                }

                if (speed > 0.05f)
                {
                    AnimationController.State = AnturaAnimationStates.walking;

                    if (AnimationController.IsAnimationActuallyWalking)
                    {
                        distance.Normalize();

                        var steeringMovement = transform.forward * speed * Time.deltaTime;
                        var normalMovement = distance * Mathf.Abs(Vector3.Dot(distance, transform.forward)) * speed * Time.deltaTime;

                        transform.position += Vector3.Lerp(steeringMovement, normalMovement, 10.0f*Vector3.Dot(transform.forward.normalized, distance.normalized));

                        GameplayHelper.LerpLookAtPlanar(transform, target.position, Time.deltaTime * 2);
                    }
                }
                else
                {
                    var dot = Mathf.Max(0, Vector3.Dot(target.forward.normalized, transform.forward.normalized));

                    if (rotateAsTarget)
                        transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, Time.deltaTime * 4 * (0.2f + 0.8f*dot));

                    if ((!rotateAsTarget || dot > 0.9f) && AnimationController.State == AnturaAnimationStates.walking)
                        AnimationController.State = AnturaAnimationStates.idle;
                }
            }
        }
    }
}
