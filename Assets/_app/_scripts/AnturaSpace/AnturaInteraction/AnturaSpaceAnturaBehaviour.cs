using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using DG.Tweening;

[RequireComponent (typeof(AnturaAnimationController))]
public class AnturaSpaceAnturaBehaviour : MonoBehaviour
{
    #region API
    public void AddBone(GameObject Bone)
    {
        m_aoBones.Add(Bone);
    }
    #endregion

    #region INTERNAL_FUNCTION
    void Start () {
        m_oAnturaCtrl = GetComponent<AnturaAnimationController>();
        Assert.IsNotNull<AnturaAnimationController>(m_oAnturaCtrl, "Add Antura Script to " + name);

        m_iAnims = Enum.GetNames(typeof(AnturaAnimationStates)).Length;
        m_bIsGoToBone = false;
    }

    void OnMouseDown()
    {
        if(!m_bIsGoToBone)
        {
            int iRnd = UnityEngine.Random.Range(0, m_iAnims);
            m_oAnturaCtrl.State = (AnturaAnimationStates)iRnd;
        }
    }

    void Update()
    {
        //test movement
        if(Input.GetButtonDown("Jump"))
        {
            GameObject go = new GameObject();
            go.transform.position = transform.position + Vector3.back * 4;
            BoxCollider b = go.AddComponent<BoxCollider>();
            b.isTrigger = true;
            AddBone(go);
            m_bMovingToDestination = true;
        }

        if(m_aoBones.Count > 0)
        {
            if (m_bMovingToDestination)
            {
                GameObject Bone = m_aoBones[0];
                m_oAnturaCtrl.State = AnturaAnimationStates.sheeping;
                MoveTo(Bone.transform.position);
            }
        }
        else
        {
            //Vector3 dir = (Camera.main.transform.position - transform.position).normalized;

            //Vector3 rot = new Vector3(0, Vector3.Angle(Vector3.forward, dir), 0);
            //rot = (Vector3.Cross(Vector3.forward, dir).y < 0) ? -rot : rot;
            //transform.DORotate(rot, 0.5f);
        }
    }

    //void OnCollisionEnter(Collision collision)
    //{
    //    if (IsBoneValid(collision.gameObject))
    //    {
    //        BoneReached(collision.gameObject);
    //    }
    //}

    void OnTriggerEnter(Collider other)
    {
        
        if (IsBoneValid(other.gameObject))
        {
            BoneReached(other.gameObject);
        }
    }

    void BoneReached(GameObject Bone)
    {
        m_oAnturaCtrl.State = AnturaAnimationStates.idle;
        m_oAnturaCtrl.DoShout();

        m_bMovingToDestination = false;
        m_aoBones.Remove(Bone);

        if (onBoneReached != null)
        {
            onBoneReached(Bone);
        }

    }
    #endregion

    #region PRIVATE_FUNCTION
    private void MoveTo(Vector3 v3Destination)
    {
        Vector3 _v3MaxMovement = v3Destination - gameObject.transform.position;
        Vector3 _v3PartialMovement = _v3MaxMovement.normalized * m_fMovementSpeed * Time.deltaTime;

        if (_v3PartialMovement.sqrMagnitude >= _v3MaxMovement.sqrMagnitude) //if we reached the destination
        {
            //position on the destination
            gameObject.transform.Translate(_v3MaxMovement, Space.World);

            m_bMovingToDestination = false;

        }
        else //make the progress for this frame
        {
            gameObject.transform.Translate(_v3PartialMovement, Space.World);
            gameObject.transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, Quaternion.LookRotation(_v3MaxMovement), m_fRotationSpeed * Time.deltaTime);
        }
    }

    private bool IsBoneValid(GameObject _Bone)
    {
        foreach(GameObject Bone in m_aoBones)
        {
            if(Bone == _Bone)
            {
                return true;
            }
        }
        return false;
    }
    #endregion


    #region VARS
    private List<GameObject> m_aoBones = new List<GameObject>();
    private AnturaAnimationController m_oAnturaCtrl;
    private int m_iAnims;
    private bool m_bIsGoToBone;

    private bool m_bMovingToDestination;
    private bool m_bRotatingToTarget;

    
    [SerializeField]
    private float m_fMovementSpeed = 10; //Movement speed
    [SerializeField]
    private float m_fRotationSpeed = 180; //Rotation speed by degree

    [HideInInspector]
    public Action<GameObject> onBoneReached;
    #endregion
}
