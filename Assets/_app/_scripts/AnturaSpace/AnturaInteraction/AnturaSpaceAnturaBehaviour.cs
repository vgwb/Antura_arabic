using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using DG.Tweening;

namespace EA4S
{

    [RequireComponent(typeof(AnturaAnimationController))]
    public class AnturaSpaceAnturaBehaviour : MonoBehaviour
    {
        #region API
        public void AddBone(GameObject Bone)
        {
            m_aoBones.Add(Bone);
            m_bMovingToDestination = true;
        }
        #endregion

        #region INTERNAL_FUNCTION
        void Start()
        {
            m_oAnturaCtrl = GetComponent<AnturaAnimationController>();
            Assert.IsNotNull<AnturaAnimationController>(m_oAnturaCtrl, "Add Antura Script to " + name);

            m_iAnims = Enum.GetNames(typeof(AnturaAnimationStates)).Length;
            m_bMovingToDestination = false;
            m_v3StartPos = transform.position;
            m_v3IdleRotation = transform.forward;
            m_oAnturaCtrl.State = AnturaAnimationStates.sitting;
        }

        void OnMouseDown()
        {
            
            if (!m_bMovingToDestination)
            {
                int iRnd = UnityEngine.Random.Range(0, m_iAnims);
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
                GameObject Bone = m_aoBones[0];
                Vector3 target = Bone.transform.position;
                target.y = transform.position.y;
                MoveTo(target);
            }
            else
            {
                if (!m_bMovingToDestination)
                {
                    m_fTimer -= Time.deltaTime;
                    if (m_fTimer < 0)
                    {
                        m_fTimer = 2;
                        ResetPosition();
                    }
                }
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (IsBoneValid(collision.gameObject))
            {
                BoneReached(collision.gameObject);
            }
        }

        void BoneReached(GameObject Bone)
        {
            m_oAnturaCtrl.State = AnturaAnimationStates.idle;
            m_oAnturaCtrl.DoShout();

            m_aoBones.Remove(Bone);
            if (m_aoBones.Count == 0)
            {
                m_bMovingToDestination = false;
            }

            if (onBoneReached != null)
            {
                onBoneReached(Bone);
            }

        }
        #endregion

        #region PRIVATE_FUNCTION

        private void ResetPosition()
        {
            if ((m_v3StartPos - transform.position).sqrMagnitude > 0.1f)
            {
                m_bMovingToDestination = true;
                m_oAnturaCtrl.State = AnturaAnimationStates.sheeping;

                float time = (m_v3StartPos - transform.position).magnitude / m_fMovementSpeed;
                m_oTweener = transform.DOMove(m_v3StartPos, time).OnComplete(() =>
                {
                    m_bMovingToDestination = false;
                    m_oAnturaCtrl.State = AnturaAnimationStates.sitting;

                    Vector3 _rot = new Vector3(0, Vector3.Angle(Vector3.forward, m_v3IdleRotation), 0);
                    _rot = (Vector3.Cross(Vector3.forward, Vector3.back).y < 0) ? -_rot : _rot;
                    transform.DORotate(_rot, 0.5f);
                });

                Vector3 rot = new Vector3(0, Vector3.Angle(Vector3.forward, Vector3.back), 0);
                rot = (Vector3.Cross(Vector3.forward, Vector3.back).y < 0) ? -rot : rot;
                transform.DORotate(rot, 0.5f);
            }
        }

        private void MoveTo(Vector3 v3Destination)
        {
            Vector3 _v3MaxMovement = v3Destination - gameObject.transform.position;
            Vector3 _v3PartialMovement = _v3MaxMovement.normalized * m_fMovementSpeed * Time.deltaTime;
            if (_v3MaxMovement.sqrMagnitude < _v3PartialMovement.sqrMagnitude) //if we reached the destination
            {
                m_oAnturaCtrl.State = AnturaAnimationStates.sitting;

                //position on the destination
                gameObject.transform.position = v3Destination;

                m_bMovingToDestination = false;
            }
            else //make the progress for this frame
            {
                m_oAnturaCtrl.State = AnturaAnimationStates.sheeping;

                gameObject.transform.Translate(_v3PartialMovement, Space.World);
                gameObject.transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, Quaternion.LookRotation(_v3MaxMovement), m_fRotationSpeed * Time.deltaTime);
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
        private float m_fTimer = 2.0f;

        private bool m_bMovingToDestination;
        private bool m_bRotatingToTarget;


        [SerializeField]
        private float m_fMovementSpeed = 10; //Movement speed
        [SerializeField]
        private float m_fRotationSpeed = 180; //Rotation speed by degree

        [HideInInspector]
        public Action<GameObject> onBoneReached;
        public Action onAnimationByClick;
        #endregion
    }
}