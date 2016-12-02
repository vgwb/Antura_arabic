using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using DG.Tweening;

namespace EA4S
{

    [RequireComponent(typeof(AnturaAnimationController))]
    public class AnturaBehaviour : MonoBehaviour
    {
        #region API
        public void AddBone(GameObject Bone)
        {
            float delay = 0f;
            if(m_oAnturaCtrl.State != AnturaAnimationStates.walking)
            {
                delay = 1.0f;
                m_oAnturaCtrl.State = AnturaAnimationStates.idle;
            }
            StartCoroutine(EA4S.MissingLetter.Utils.LaunchDelay<GameObject>(delay, InternalAddBone, Bone));
        }

        public void Reset()
        {
            foreach(GameObject Bone in m_aoBones)
            {
                if(onBoneReached != null)
                {
                    onBoneReached(Bone);
                }
            }
            m_aoBones.Clear();

            ResetPosition();       
        }

        public bool IsInCustomization { get; set; }
        #endregion

        #region INTERNAL_FUNCTION

        void Start()
        {
            m_oAnturaCtrl = GetComponent<AnturaAnimationController>();
            Assert.IsNotNull<AnturaAnimationController>(m_oAnturaCtrl, "Add Antura Script to " + name);

            m_iAnims = Enum.GetNames(typeof(AnturaAnimationStates)).Length;
            m_bMovingToDestination = false;
            m_bEatingBone = false;
            m_v3StartPos = transform.position;
            m_v3IdleRotation = transform.forward;
            m_oAnturaCtrl.State = AnturaAnimationStates.sitting;
        }

        void OnMouseDown()
        {   
            if(IsInCustomization)
            {
                if(m_oAnturaCtrl.State == AnturaAnimationStates.idle)
                    m_oAnturaCtrl.State = AnturaAnimationStates.bitingTail;
                else
                    m_oAnturaCtrl.State = AnturaAnimationStates.idle;

            }
            else if (!m_bMovingToDestination)
            {
                AudioManager.I.PlaySfx(m_oSfxOnClick);
                int iRnd;
                do
                {
                    iRnd = UnityEngine.Random.Range(0, m_iAnims);
                }
                while (iRnd == (int)AnturaAnimationStates.sucking || iRnd == (int)m_oAnturaCtrl.State);

                m_oAnturaCtrl.State = (AnturaAnimationStates)iRnd;

                if (onAnimationByClick != null)
                {
                    onAnimationByClick();
                }
            }
            else
            {
                AudioManager.I.PlaySfx(Sfx.DogBarking); //bark when clicked and going to a bone
            }
        }

        void Update()
        {
            if (m_aoBones.Count > 0)
            {
                if (m_oTweener != null)
                {
                    m_oTweener.Kill();
                }

                //check if not eating
                if(!m_bEatingBone)
                {
                    GameObject Bone = m_aoBones[0];
                    //check if the bone is out of scene
                    m_fTimerDeleteBoneSafe -= Time.deltaTime;
                    if (m_fTimerDeleteBoneSafe < 0)
                    {
                        BoneReached(m_aoBones[0]);
                        return;
                    }

                    Vector3 targetToReach = Bone.transform.position;
                    targetToReach.y = transform.position.y;

                    Vector3 targetToLook = targetToReach;
                    //if the bone is in dragged stop near  
                    if (Bone.GetComponent<BoneBehaviour>().isDragging())
                    {
                        targetToReach.z += 3;
                    }


                    MoveTo(targetToReach, targetToLook);
                }
            }
            else if (!m_bEatingBone)
            {
                //if you not walking on target for 2 seconds return to start position
                if (!m_bMovingToDestination)
                {
                    m_fTimerWaitForReturn -= Time.deltaTime;
                    if (m_fTimerWaitForReturn < 0)
                    {
                        m_fTimerWaitForReturn = 1.0f;
                        ResetPosition();
                    }
                }
                else
                {
                    m_fTimerWaitForReturn = 1.0f;
                }
            }
        }

        void OnCollisionStay(Collision collision)
        {
            if (IsBoneValid(collision.gameObject))
            {
                BoneReached(collision.gameObject);
            }
        }

        void BoneReached(GameObject Bone)
        {
            m_bEatingBone = true;
            StartCoroutine(EA4S.MissingLetter.Utils.LaunchDelay(1.0f, () => { m_bEatingBone = false; }));

            m_oAnturaCtrl.State = AnturaAnimationStates.idle;
            m_oAnturaCtrl.DoShout();
            Bone.transform.position = transform.position + transform.forward * 2;
            Bone.GetComponent<BoneBehaviour>().Poof();

            m_aoBones.Remove(Bone);
            if (m_aoBones.Count == 0)
            {
                m_bMovingToDestination = false;
            }

            if (onBoneReached != null)
            {
                onBoneReached(Bone);
            }

            m_fTimerDeleteBoneSafe = 7.0f;
        }
        #endregion

        #region PRIVATE_FUNCTION

        private void InternalAddBone(GameObject Bone)
        {
            m_aoBones.Add(Bone);
            m_bMovingToDestination = true;
        }

        public void ResetPosition()
        {
            Vector3 dir = m_v3StartPos - transform.position;
            if (dir.sqrMagnitude > 0.1f)
            {
                m_bMovingToDestination = true;
                m_oAnturaCtrl.State = AnturaAnimationStates.walking;

                float time = dir.magnitude / m_fMovementSpeed;
                m_oTweener = transform.DOMove(m_v3StartPos, time).OnComplete(() =>
                {
                    m_bMovingToDestination = false;
                    m_oAnturaCtrl.State = AnturaAnimationStates.sitting;

                    Vector3 _rot = new Vector3(0, Vector3.Angle(Vector3.forward, m_v3IdleRotation), 0);
                    _rot = (Vector3.Cross(Vector3.forward, m_v3IdleRotation).y < 0) ? -_rot : _rot;
                    transform.DORotate(_rot, 0.5f);
                });

                dir.y = transform.position.y;
                transform.DOLookAt(dir, 0.5f);
            }
        }

        private void MoveTo(Vector3 v3Destination, Vector3 LookAt)
        {
            Vector3 _v3MaxMovement = v3Destination - gameObject.transform.position;
            Vector3 _v3PartialMovement = _v3MaxMovement.normalized * m_fMovementSpeed * Time.deltaTime;
            if (_v3MaxMovement.sqrMagnitude <= _v3PartialMovement.sqrMagnitude) //if we reached the destination
            {
                m_oAnturaCtrl.State = AnturaAnimationStates.sitting;

                //position on the destination
                gameObject.transform.position = v3Destination;

                m_bMovingToDestination = false;
            }
            else //make the progress for this frame
            {
                m_oAnturaCtrl.State = AnturaAnimationStates.walking;

                gameObject.transform.Translate(_v3PartialMovement, Space.World);

                Quaternion targetRot = Quaternion.LookRotation(LookAt - transform.position);
                gameObject.transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, targetRot, m_fRotationSpeed * Time.deltaTime);
            }
        }

        private bool IsBoneValid(GameObject _Bone)
        {
            foreach (GameObject Bone in m_aoBones)
            {
                if (Bone == _Bone)
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

        private Tweener m_oTweener;
        private Vector3 m_v3StartPos;
        private Vector3 m_v3IdleRotation;
        private float m_fTimerWaitForReturn = 1.0f;
        private float m_fTimerDeleteBoneSafe = 7.0f;

        private bool m_bMovingToDestination;
        private bool m_bRotatingToTarget;
        private bool m_bEatingBone;

        [SerializeField]
        private float m_fMovementSpeed = 10; //Movement speed
        [SerializeField]
        private float m_fRotationSpeed = 180; //Rotation speed by degree
        [SerializeField]
        private Sfx m_oSfxOnClick;

        [HideInInspector]
        public Action<GameObject> onBoneReached;
        public Action onAnimationByClick;
        #endregion
    }
}