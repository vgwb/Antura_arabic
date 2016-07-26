using UnityEngine;
using System;
using System.Collections;
using Panda;
using DG.Tweening;
using UniRx;
using ModularFramework.Helpers;

namespace EA4S {
    public class LetterBehaviour : MonoBehaviour {

        #region public properties
        public BehaviourSettings Settings;

        [Header("GO Elements")]
        public Transform exclamationMark;
        private Sequence sequenceExclamationMark;
        #endregion

        #region runtime variables

        /// <summary>
        /// Animator
        /// </summary>
        public Animator Anim {
            get {
                if (!anim)
                    anim = GetComponent<Animator>();
                return anim;
            }
            set { anim = value; }
        }
        private Animator anim;
        #endregion

        void Start() {
            sequenceExclamationMark = DOTween.Sequence();
            sequenceExclamationMark.SetLoops(-1);
            //exclamationMark.transform.localScale = Vector3.zero;
            if (exclamationMark) {
                // Sequence
                sequenceExclamationMark.Append(exclamationMark.DOShakePosition(0.9f));
                //sequenceExclamationMark.AppendInterval(0.5f);
                sequenceExclamationMark.Pause();
            }
        }

        #region Tasks

        #region properties
        [Task]
        public bool IsOut = false;

        [Task]
        public void SetIsOut(bool _isOut) {
            IsOut = _isOut;
            Task.current.Succeed();
        }
        #endregion

        #region Animation
        /// <summary>
        /// Play anim with name as param.
        /// </summary>
        /// <param name="_animationName"></param>
        [Task]
        public void SetAnimation(string _animationName) {
            if (exclamationMark && sequenceExclamationMark != null) {
                //exclamationMark.transform.localScale = Vector3.zero;
                if (sequenceExclamationMark.IsPlaying()) {
                    sequenceExclamationMark.Pause();
                    exclamationMark.DOScale(0, 0.3f);
                }
            }

            switch (_animationName) {
                case "Idle":
                    Anim.SetInteger("State", 0);
                    break;
                case "Walk":
                    Anim.SetInteger("State", 1);
                    break;
                case "Run":
                    Anim.SetInteger("State", 2);
                    break;
                case "Hold":
                    Anim.SetInteger("State", 3);
                    break;
                case "Ninja":
                    Anim.SetInteger("State", 4);
                    break;
                case "Terrified":
                    Anim.SetInteger("State", 2);
                    exclamationMark.DOScale(1, 0.3f);
                    sequenceExclamationMark.Play();
                    break;
                default:
                    Debug.Log("Animation not found");
                    break;
            }
            
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
        public bool IsTarget() {
            return Target;
        }

        /// <summary>
        /// Do look at target action.
        /// </summary>
        [Task]
        public void LookAtTarget() {
            if (DoLookTarget)
                RotateBonesTransform.DOLookAt(-Target.position, TimeRotation, AxisConstraint.Y, WorldUpForTarget);
            Task.current.Succeed();
        }

        [Task]
        public void LookAtForward() {
            // TODO: improve -> verify if trasform already look forward. 
            RotateBonesTransform.DOLookAt(LookForwardTransform.position, TimeRotation, AxisConstraint.Y, WorldUpForTarget);
            Task.current.Succeed();
        }

        /// <summary>
        /// Turn all the body in front of camera.
        /// </summary>
        [Task]
        public void TurnAllFrontOfTarget() {
            if (DoLookTarget)
                RotateAllBodyTransform.DOLookAt(-Target.position, TimeRotation, AxisConstraint.Y, WorldUpForTarget);
            Task.current.Succeed();
        }

        #endregion

        #region States
        float runtimeWaitTime = 0;
        /// <summary>
        /// Stay in actual game for specific duration time.
        /// </summary>
        /// <param name="_stateName"></param>
        [Task]
        public void HoldState(string _stateName) {
            switch (_stateName) {
                case "Idle":
                    if(Task.current.isStarting)
                        runtimeWaitTime = GenericHelper.GetValueWithRandomVariation(Settings.IdleDuration, Settings.DurationRandomDelta);
                    Wait(runtimeWaitTime);
                    break;
                case "Walk":
                    if (Task.current.isStarting)
                        runtimeWaitTime = GenericHelper.GetValueWithRandomVariation(Settings.WalkDuration, Settings.DurationRandomDelta);
                    Wait(runtimeWaitTime);
                    break;
                case "Run":
                    if (Task.current.isStarting)
                        runtimeWaitTime = GenericHelper.GetValueWithRandomVariation(Settings.RunDuration, Settings.DurationRandomDelta);
                    Wait(runtimeWaitTime);
                    break;
                case "Ninja":
                    if (Task.current.isStarting)
                        runtimeWaitTime = GenericHelper.GetValueWithRandomVariation(Settings.NinjaDuration, Settings.DurationRandomDelta);
                    Wait(runtimeWaitTime);
                    break;
                case "TurnFrontOfCamera":
                    if (Task.current.isStarting)
                        runtimeWaitTime = GenericHelper.GetValueWithRandomVariation(0.4f, Settings.DurationRandomDelta);
                    Wait(runtimeWaitTime);
                    break;
                case "GoOut":
                    if (Task.current.isStarting)
                        runtimeWaitTime = 3.0f;
                    Wait(runtimeWaitTime);
                    break;
                case "BumpOut":
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
        public void HoldLookAtCamera(string _stateName) {
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
        public void Wait(float duration) {
            var task = Task.current;
            if (task.isStarting) {
                task.item = Time.time;
            }

            float elapsedTime = Time.time - (float)Task.current.item;

            float tta = duration - elapsedTime;
            if (tta < 0.0f) tta = 0.0f;

            if (Task.isInspected)
                task.debugInfo = string.Format("t-{0:0.000}", tta);

            if (elapsedTime >= duration) {
                task.debugInfo = "t-0.000";
                task.Succeed();
            }
        }
        #endregion

        #endregion

        #region BehaviourSettings

        /// <summary>
        /// Define variables for behaviours variations.
        /// </summary>
        [Serializable]
        public class BehaviourSettings {
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