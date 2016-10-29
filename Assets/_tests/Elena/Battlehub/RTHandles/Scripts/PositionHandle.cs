using UnityEngine;
using System.Collections;
using System;

namespace Battlehub.RTHandles
{
    public class PositionHandle : MonoBehaviour, IGL
    {
        public Camera Camera;
        public float SelectionMargin = 10;
        public Transform Target;

        private RuntimeHandleAxis m_selectedAxis;
        private bool m_isDragging;
        private Vector3 m_prevPoint;
        private Plane m_dragPlane;
        
        public bool IsDragging
        {
            get { return m_isDragging; }
        }

        public static PositionHandle Current
        {
            get;
            private set;
        }

        private void Start()
        {
            if (GLRenderer.Instance == null)
            {
                GameObject glRenderer = new GameObject();
                glRenderer.name = "GLRenderer";
                glRenderer.AddComponent<GLRenderer>();

                Camera[] cameras = Camera.allCameras;
                for (int i = 0; i < cameras.Length; ++i)
                {
                    Camera cam = cameras[i];
                    if (!cam.GetComponent<GLCamera>())
                    {
                        cam.gameObject.AddComponent<GLCamera>();
                    }
                }
            }

            if(Camera == null)
            {
                Camera = Camera.main;
            }

            if (GLRenderer.Instance != null)
            {
                GLRenderer.Instance.Add(this);
            }

            if (Target != null && Target.position != transform.position)
            {
                transform.position = Target.position;
            }

            Current = this;
        }

        private void OnEnable()
        {
            if (GLRenderer.Instance != null)
            {
                GLRenderer.Instance.Add(this);
            }
        }

        private void OnDisable()
        {
            if (GLRenderer.Instance != null)
            {
                GLRenderer.Instance.Remove(this);
            }
        }

        private void OnDestroy()
        {
            if (GLRenderer.Instance != null)
            {
                GLRenderer.Instance.Remove(this);
            }

            if (Current == this)
            {
                Current = null;
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if(RuntimeTools.Current != RuntimeTool.Move && RuntimeTools.Current != RuntimeTool.None)
                {
                    return;
                }

                if (Camera == null)
                {
                    Debug.LogError("Camera is null");
                    return;
                }

                float scale = RuntimeHandles.GetScreenScale(transform.position, Camera);
                Matrix4x4 matrix = Matrix4x4.TRS(transform.position, transform.rotation, new Vector3(scale, scale, scale));

                float distToYAxis;
                float distToZAxis;
                float distToXAxis;
                bool hit = HitAxis(Vector3.up, matrix, out distToYAxis);
                hit |= HitAxis(Vector3.forward, matrix, out distToZAxis);
                hit |= HitAxis(Vector3.right, matrix, out distToXAxis);

                if (hit)
                {
                    if (distToYAxis <= distToZAxis && distToYAxis <= distToXAxis)
                    {
                        m_selectedAxis = RuntimeHandleAxis.Y;
                    }
                    else if (distToXAxis <= distToYAxis && distToXAxis <= distToZAxis)
                    {
                        m_selectedAxis = RuntimeHandleAxis.X;
                    }
                    else
                    {
                        m_selectedAxis = RuntimeHandleAxis.Z;
                    }

                    m_dragPlane = GetDragPlane();
                    m_isDragging = GetPointOnDragPlane(Input.mousePosition, out m_prevPoint);
                }
                else
                {
                    m_selectedAxis = RuntimeHandleAxis.None;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                m_isDragging = false;
            }
            else
            {
                if (m_isDragging)
                {
                    Vector3 point;
                    if (GetPointOnDragPlane(Input.mousePosition, out point))
                    {
                        Vector3 offset = transform.InverseTransformVector(point - m_prevPoint);
                        float mag = offset.magnitude;
                        if (m_selectedAxis == RuntimeHandleAxis.X)
                        {
                            offset.y = offset.z = 0.0f;
                        }
                        else if (m_selectedAxis == RuntimeHandleAxis.Y)
                        {
                            offset.x = offset.z = 0.0f;
                        }
                        else
                        {
                            offset.x = offset.y = 0.0f;
                        }

                        offset = transform.TransformVector(offset).normalized * mag;
                        transform.position += offset;
                        m_prevPoint = point;
                    }
                }
            }

            if (Target != null && Target.position != transform.position)
            {
                if (m_isDragging)
                {
                    Target.position = transform.position;
                }
                else
                {
                    transform.position = Target.position;
                }
            }
            
        
        }

        private bool HitAxis(Vector3 axis, Matrix4x4 matrix, out float distanceToAxis)
        {
            axis = matrix.MultiplyVector(axis);
            Vector2 screenVectorBegin = Camera.WorldToScreenPoint(transform.position);
            Vector2 screenVectorEnd = Camera.WorldToScreenPoint(axis + transform.position);

            Vector3 screenVector = screenVectorEnd - screenVectorBegin;
            float screenVectorMag = screenVector.magnitude;
            screenVector.Normalize();
            if (screenVector != Vector3.zero)
            {
                Vector2 perp = PerpendicularClockwise(screenVector).normalized;
                Vector2 mousePosition = Input.mousePosition;
                Vector2 relMousePositon = mousePosition - screenVectorBegin;

                distanceToAxis = Mathf.Abs(Vector2.Dot(perp, relMousePositon));
                Vector2 hitPoint = (relMousePositon - perp * distanceToAxis);
                float vectorSpaceCoord = Vector2.Dot(screenVector, hitPoint);

                bool result = vectorSpaceCoord <= screenVectorMag + SelectionMargin && vectorSpaceCoord >= -SelectionMargin && distanceToAxis <= SelectionMargin;
                if (!result)
                {
                    distanceToAxis = float.PositiveInfinity;
                }
                else
                {
                    if (screenVectorMag < SelectionMargin)
                    {
                        distanceToAxis = 0.0f;
                    }
                }
                return result;
            }
            else
            {
                Vector2 mousePosition = Input.mousePosition;

                distanceToAxis = (screenVectorBegin - mousePosition).magnitude;
                bool result = distanceToAxis <= SelectionMargin;
                if (!result)
                {
                    distanceToAxis = float.PositiveInfinity;
                }
                else
                {
                    distanceToAxis = 0.0f;
                }
                return result;
            }
        }


        private Plane GetDragPlane()
        {
            Vector3 toCam = Camera.transform.position - transform.position;
            Plane dragPlane = new Plane(-toCam.normalized, transform.position);
            return dragPlane;
        }

        private bool GetPointOnDragPlane(Vector3 screenPos, out Vector3 point)
        {

            Ray ray = Camera.ScreenPointToRay(screenPos);
            float distance;
            if (m_dragPlane.Raycast(ray, out distance))
            {
                point = ray.GetPoint(distance);
                return true;
            }

            point = Vector3.zero;
            return false;
        }

        private static Vector2 PerpendicularClockwise(Vector2 vector2)
        {
            return new Vector2(-vector2.y, vector2.x);
        }

        public void Draw()
        {
            RuntimeHandles.DoPositionHandle(transform.position, transform.rotation, m_selectedAxis);
        }

    }

}
