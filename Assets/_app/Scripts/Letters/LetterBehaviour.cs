using UnityEngine;
using System.Collections;
using Panda;
using DG.Tweening;

public class LetterBehaviour : MonoBehaviour {

    #region public properties

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
    [Task]
    public void SetAnimation(string _animationName) {
        Anim.Play(_animationName);
        Task.current.Succeed();
    }
    #endregion

    public Vector3 WorldUpForT = new Vector3(0,1,0);
    public Transform RotateBonesTransform;
    public float TimeRotation = 0.4f;

    //void LateUpdate() {
    //    if (IsLookingToTarget) { 
    //        RotateBonesTransform.DOLookAt(-Target.position, TimeRotation);
    //    }
    //}

    [Task]
    public void LookAtTarget() {
        RotateBonesTransform.DOLookAt(-Target.position, TimeRotation);
        Task.current.Succeed();
    }

    //[Task]
    //public void LookAtTarget() {
    //    Transform targetTransform = RotateBonesTransform;
    //    Vector3 targetPosition = -Target.position;

    //    var targetDelta = (targetPosition - targetTransform.position);
    //    var targetDir = targetDelta.normalized;

    //    if (targetDelta.magnitude > 0.2f) {


    //        Vector3 axis = Vector3.up * Mathf.Sign(Vector3.Cross(targetTransform.forward, targetDir).y);

    //        var rot = Quaternion.AngleAxis(TimeRotation * Time.deltaTime, axis);
    //        targetTransform.rotation = rot * targetTransform.rotation;

    //        Vector3 newAxis = Vector3.up * Mathf.Sign(Vector3.Cross(targetTransform.forward, targetDir).y);

    //        float dot = Vector3.Dot(axis, newAxis);

    //        if (dot < 0.01f) {// We overshooted the target
    //            var snapRot = Quaternion.FromToRotation(targetTransform.forward, targetDir);
    //            targetTransform.rotation = snapRot * targetTransform.rotation;
    //            Task.current.Succeed();
    //        }

    //        var straighUp = Quaternion.FromToRotation(targetTransform.up, Vector3.up);
    //        targetTransform.rotation = straighUp * targetTransform.rotation;
    //    } else {
    //        Task.current.Succeed();
    //    }

    //    if (Task.isInspected)
    //        Task.current.debugInfo = string.Format("angle={0}", Vector3.Angle(targetDir, targetTransform.forward));


    //}
}
