using UnityEngine;
using System;
using System.Collections;
using Panda;
using DG.Tweening;
using ModularFramework.Helpers;

namespace EA4S {
    /// <summary>
    /// Add AI logic to letter puppet object.
    /// </summary>
    [RequireComponent(typeof(LetterObjectView))]
    public class LetterBehaviour : MonoBehaviour {

        #region public properties

        public BehaviourSettings Settings;

        #endregion

        #region Runtime properties
        LetterObjectView View;
        #endregion

        void Start() {
            View = GetComponent<LetterObjectView>();
            //HoldState("Idle");
        }

        #region Tasks

        #region properties

        [Task]
        public bool IsOut = false;

        [Task]
        public void SetIsOut(bool _isOut)
        {
            IsOut = _isOut;
            Task.current.Succeed();
        }

        #endregion

        #region LookAt

        [Header("Look at Target runtime variables")]
        public Transform Target = null;
        public Transform LookForwardTransform = null;
        public Vector3 WorldUpForTarget = new Vector3(0, 1, 0);
        public Transform RotateBonesTransform;
        public Transform RotateAllBodyTransform;
        public float TimeRotation = 0.4f;
        /// <summary>
        /// Flag indicate if lettera must look at target (if target != null).
        /// </summary>
        [Task]
        public bool DoLookTarget = false;

        /// <summary>
        /// True if Target != null.
        /// </summary>
        /// <returns></returns>
        [Task]
        public bool IsTarget()
        {
            return Target;
        }

        /// <summary>
        /// Do look at target action.
        /// </summary>
        [Task]
        public void LookAtTarget()
        {
            if (DoLookTarget)
                RotateBonesTransform.DOLookAt(-Target.position, TimeRotation, AxisConstraint.Y, WorldUpForTarget);
            Task.current.Succeed();
        }

        [Task]
        public void LookAtForward()
        {
            // TODO: improve -> verify if trasform already look forward. 
            RotateBonesTransform.DOLookAt(LookForwardTransform.position, TimeRotation, AxisConstraint.Y, WorldUpForTarget);
            Task.current.Succeed();
        }

        /// <summary>
        /// Turn all the body in front of camera.
        /// </summary>
        [Task]
        public void TurnAllFrontOfTarget()
        {
            if (DoLookTarget)
                RotateAllBodyTransform.DOLookAt(-Target.position, TimeRotation, AxisConstraint.Y, WorldUpForTarget);
            Task.current.Succeed();
        }

        #endregion

        #region States

        float runtimeWaitTime = 0;

        /// <summary>
        /// Stay in actual state for specific duration time.
        /// </summary>
        /// <param name="_stateName"></param>
        [Task]
        public void HoldState(string _stateName) {
            if (Task.current == null)
                return;
            switch (_stateName) {
                case "Idle":
                    View.Model.State = LetterObjectState.Idle_State;
                    if (Task.current.isStarting)
                        runtimeWaitTime = GenericHelper.GetValueWithRandomVariation(Settings.IdleDuration, Settings.DurationRandomDelta);
                    Wait(runtimeWaitTime);
                    break;
                case "Walk":
                    View.Model.State = LetterObjectState.Walk_State;
                    if (Task.current.isStarting)
                        runtimeWaitTime = GenericHelper.GetValueWithRandomVariation(Settings.WalkDuration, Settings.DurationRandomDelta);
                    Wait(runtimeWaitTime);
                    break;
                case "Run":
                    View.Model.State = LetterObjectState.Run_State;
                    if (Task.current.isStarting)
                        runtimeWaitTime = GenericHelper.GetValueWithRandomVariation(Settings.RunDuration, Settings.DurationRandomDelta);
                    Wait(runtimeWaitTime);
                    break;
                case "Ninja":
                    View.Model.State = LetterObjectState.Ninja_State;
                    if (Task.current.isStarting)
                        runtimeWaitTime = GenericHelper.GetValueWithRandomVariation(Settings.NinjaDuration, Settings.DurationRandomDelta);
                    Wait(runtimeWaitTime);
                    break;
                case "TurnFrontOfCamera":
                    View.Model.State = LetterObjectState.FrontOfCamera_State;
                    if (Task.current.isStarting)
                        runtimeWaitTime = GenericHelper.GetValueWithRandomVariation(0.4f, Settings.DurationRandomDelta);
                    Wait(runtimeWaitTime);
                    break;
                case "GoOut":
                    View.Model.State = LetterObjectState.GoOut_State;
                    if (Task.current.isStarting)
                        runtimeWaitTime = 3.0f;
                    Wait(runtimeWaitTime);
                    break;
                case "BumpOut":
                    View.Model.State = LetterObjectState.BumpOut_State;
                    if (Task.current.isStarting)
                        runtimeWaitTime = 1.0f;
                    Wait(runtimeWaitTime);
                    break;
                default:
                    Task.current.Succeed();
                    break;
            }
        }

        [Task]
        public void HoldLookAtCamera(string _stateName)
        {
            switch (_stateName) {
                case "Look":
                    if (Task.current.isStarting)
                        runtimeWaitTime = GenericHelper.GetValueWithRandomVariation(Settings.LookAtTargetDuration, Settings.DurationRandomDelta);
                    Wait(runtimeWaitTime);
                    break;
                case "NoLook":
                    if (Task.current.isStarting)
                        runtimeWaitTime = GenericHelper.GetValueWithRandomVariation(Settings.NoLookAtTargetDuration, Settings.DurationRandomDelta);
                    Wait(runtimeWaitTime);
                    break;
                default:

                    Task.current.Succeed();
                    break;
            }
        }

        #endregion

        #region Common

        /// <summary>
        /// Wait base task.
        /// </summary>
        /// <param name="duration"></param>
        public void Wait(float duration)
        {
            var task = Task.current;
            if (task.isStarting) {
                task.item = Time.time;
            }

            float elapsedTime = Time.time - (float)Task.current.item;

            float tta = duration - elapsedTime;
            if (tta < 0.0f)
                tta = 0.0f;

            if (Task.isInspected)
                task.debugInfo = string.Format("t-{0:0.000}", tta);

            if (elapsedTime >= duration) {
                task.debugInfo = "t-0.000";
                task.Succeed();
            }
        }

        #endregion

        #region ToBeDeleted
        [Task]
        public void SetAnimation(string _animationName) {
            //Task.current.Fail();
        }
        #endregion

            #endregion

            #region BehaviourSettings

            /// <summary>
            /// Define variables for behaviours variations.
            /// </summary>
        [Serializable]
        public class BehaviourSettings
        {
            [Header("LookAtTarget behaviour settings")]
            [Range(0, 5)]
            public float LookAtTargetRandomDelta = 0.5f;
            [Range(1, 10)]
            public float LookAtTargetDuration = 2;
            [Range(0, 10)]
            public float NoLookAtTargetDuration = 4;

            [Header("Behaviour variation settings")]
            [Range(0, 6)]
            public float DurationRandomDelta = 1;
            [Range(1, 10)]
            public float IdleDuration = 2.5f;
            [Range(0, 10)]
            public float WalkDuration = 2;
            [Range(0, 10)]
            public float RunDuration = 2;
            [Range(0, 10)]
            public float NinjaDuration = 3;
        }

        #endregion
    }
}