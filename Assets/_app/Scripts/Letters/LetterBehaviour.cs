using UnityEngine;
using System;
using System.Collections;
using Panda;
using DG.Tweening;
using UniRx;

namespace CGL.Antura {
    public class LetterBehaviour : MonoBehaviour {

        #region public properties
        public BehaviourSettings Settings;
        #endregion

        #region runtime variables
        [Task]
        public bool IsLookingToTarget() {
            return Target != null;
        }

        public Transform Target = null;

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

        #region Tasks

        #region Animation
        /// <summary>
        /// Play anim with name as param.
        /// </summary>
        /// <param name="_animationName"></param>
        [Task]
        public void SetAnimation(string _animationName) {
            Anim.Play(_animationName);
            Task.current.Succeed();
        }
        #endregion

        #region LookAt
        public Vector3 WorldUpForT = new Vector3(0, 1, 0);
        public Transform RotateBonesTransform;
        public float TimeRotation = 0.4f;

        [Task]
        public void LookAtTarget() {
            RotateBonesTransform.DOLookAt(-Target.position, TimeRotation);
            Task.current.Succeed();
        }
        #endregion

        #region common
        [Task]
        public void HoldState(string _stateName) {
            switch (_stateName) {
                case "Idle":
                    Wait(Settings.IdleDuration);
                    //Task.current.Succeed();
                    break;
                case "Walk":
                    Wait(Settings.WalkDuration);
                    //Task.current.Succeed();
                    break;
                default:
                    
                    Task.current.Succeed();
                    break;
            }
        }

        
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

        /// <summary>
        /// Define variables for behaviours variations.
        /// </summary>
        [Serializable]
        public class BehaviourSettings {
            [Header("LookAtTarget behaviour settings")]
            [Range(1, 10)]
            public float LookAtTargetDuration = 1;
            [Range(0, 10)]
            public float NotLookAtTargetDuration = 1;

            [Header("LookAtTarget behaviour settings")]
            [Range(0, 6)]
            public float DurationRandomDelta = 2;
            [Range(1, 10)]
            public float IdleDuration = 5;
            [Range(0, 10)]
            public float WalkDuration = 2;
            
        }

}
}