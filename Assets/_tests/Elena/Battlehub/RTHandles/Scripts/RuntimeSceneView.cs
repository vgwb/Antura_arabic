using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Battlehub.RTHandles
{
    public enum RuntimeTool
    {
        None,
        Move,
        View,
    }

    public class RuntimeTools
    {
        private static RuntimeTool m_current;
        public static RuntimeTool Current
        {
            get { return m_current; }
            set { m_current = value; }
        }
    }

    public delegate void RuntimeSelectionChanged();
    public static class RuntimeSelection
    {
        public static event RuntimeSelectionChanged SelectionChanged;

        private static GameObject m_activeGameObject;

        public static GameObject activeGameObject
        {
            get { return m_activeGameObject; }
            set
            {
                if(m_activeGameObject != value)
                {
                    m_activeGameObject = value;
                    if (SelectionChanged != null)
                    {
                        SelectionChanged();
                    }
                }
            }
        }

        public static Transform activeTransform
        {
            get
            {
                if(m_activeGameObject == null)
                {
                    return null;
                }
                return m_activeGameObject.transform;
            }
            set
            {
                if(value)
                {
                    m_activeGameObject = value.gameObject;
                }
                else
                {
                    m_activeGameObject = null;
                }
            }
        }
    }

    public class RuntimeSceneView : MonoBehaviour
    {
        private bool m_handleInput;        
        private Vector3 m_pivot;
        public Vector3 Pivot
        {
            get { return m_pivot; }
        }

        private Vector3 m_lastMousePosition;
        public Camera Camera;
        private MouseOrbit m_mouseOrbit;
        public float RotationSensitivity = 1f;
        public float ZoomSensitivity = 8f;
        public float PanSensitivity = 100f;

        private PositionHandle m_positionHandle;
      
        private void Awake()
        {
            RuntimeTools.Current = RuntimeTool.Move;
            GameObject positionHandle = new GameObject();
            positionHandle.name = "PositionHandle";
            positionHandle.transform.SetParent(transform, false);
            m_positionHandle = positionHandle.AddComponent<PositionHandle>();
            positionHandle.SetActive(false);
            RuntimeSelection.SelectionChanged += OnRuntimeSelectionChanged;
        }

        private void OnDestroy()
        {
            RuntimeTools.Current = RuntimeTool.None;
            RuntimeSelection.SelectionChanged -= OnRuntimeSelectionChanged;
        }

        private void Start()
        {
            if (Camera == null)
            {
                Camera = Camera.main;
            }

            
            m_mouseOrbit = Camera.gameObject.GetComponent<MouseOrbit>();
            if(m_mouseOrbit == null)
            {
                m_mouseOrbit = Camera.gameObject.AddComponent<MouseOrbit>();
            }
            m_pivot = Camera.transform.position + Camera.transform.forward * m_mouseOrbit.Distance;
            m_mouseOrbit.Target = m_pivot;
            m_mouseOrbit.Init();
            m_mouseOrbit.enabled = false;
        }

        private void Update()
        {
            HandleInput();
        }

        private void OnRuntimeSelectionChanged()
        {
            if(RuntimeSelection.activeGameObject == null)
            {
                if(m_positionHandle != null)
                {
                    m_positionHandle.gameObject.SetActive(false);
                }
            }
            else
            {
                if (m_positionHandle != null)
                {
                    m_positionHandle.gameObject.SetActive(true);
                    m_positionHandle.transform.position = RuntimeSelection.activeTransform.position;
                    m_positionHandle.Target = RuntimeSelection.activeTransform;
                }
            }
        }

        private void HandleInput()
        {
           

            if (Input.GetKeyDown(KeyCode.F))
            {
                Focus();
            }

            if (Input.GetKeyUp(KeyCode.AltGr) || Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
            {
                RuntimeTools.Current = RuntimeTool.Move;
            }

            bool pan = Input.GetMouseButton(2) || Input.GetMouseButton(1);
            bool alt = Input.GetKey(KeyCode.AltGr) || Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
            bool view = alt || pan;
            if (view)
            {
                RuntimeTools.Current = RuntimeTool.View;
            }

            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                m_handleInput = !m_positionHandle.IsDragging;
                m_lastMousePosition = Input.mousePosition;
                if(alt)
                {
                    m_mouseOrbit.Target = m_pivot;
                    m_mouseOrbit.enabled = true;
                }
            }

            if(Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
            {
                m_handleInput = false;
                
                m_mouseOrbit.enabled = false;
                              
            }

            float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
            if (mouseWheel != 0) 
            {
                m_mouseOrbit.Zoom();
            }
   
            if(m_handleInput)
            {
                if(view)
                {
                    if (!alt)
                    {
                        Pan();
                    }
                }
            }
        }


        private void Focus()
        {
            if(RuntimeSelection.activeTransform == null)
            {
                return;
            }

            Bounds worldBounds = CalculateBounds(RuntimeSelection.activeTransform);
            Bounds bounds = worldBounds;

            float fov = Camera.fieldOfView * Mathf.Deg2Rad;

            float objSize = Mathf.Max(bounds.extents.y, bounds.extents.x, bounds.extents.z) * 2.0f;
            float distance = Math.Max(5, Math.Abs(objSize / Mathf.Sin(fov / 2.0f)));

            m_pivot = worldBounds.center;

            Camera.transform.position = m_pivot -  distance * Camera.transform.forward;
            m_mouseOrbit.Distance = distance;
            m_mouseOrbit.Target = m_pivot;
           
        }

        private Bounds CalculateBounds(Transform t)
        {
            Renderer renderer = t.GetComponentInChildren<Renderer>();
            if(renderer)
            {
                Bounds bounds = renderer.bounds;
                if(bounds.size == Vector3.zero && bounds.center != renderer.transform.position)
                {
                    bounds = TransformBounds(renderer.transform.localToWorldMatrix, bounds);
                }
                CalculateBounds(t, ref bounds);
                return bounds;
            }

            return new Bounds(t.position, Vector3.zero);
        }

        private void CalculateBounds(Transform t, ref Bounds totalBounds)
        {
            foreach (Transform child in t)
            {
                Renderer renderer = child.GetComponent<Renderer>();
                if (renderer)
                {
                    Bounds bounds = renderer.bounds;
                    if (bounds.size == Vector3.zero && bounds.center != renderer.transform.position)
                    {
                        bounds = TransformBounds(renderer.transform.localToWorldMatrix, bounds);
                    }
                    totalBounds.Encapsulate(bounds.min);
                    totalBounds.Encapsulate(bounds.max);
                }

                CalculateBounds(child, ref totalBounds);
            }
        }
        public static Bounds TransformBounds(Matrix4x4 matrix, Bounds bounds)
        {
            var center = matrix.MultiplyPoint(bounds.center);

            // transform the local extents' axes
            var extents = bounds.extents;
            var axisX = matrix.MultiplyVector(new Vector3(extents.x, 0, 0));
            var axisY = matrix.MultiplyVector(new Vector3(0, extents.y, 0));
            var axisZ = matrix.MultiplyVector(new Vector3(0, 0, extents.z));

            // sum their absolute value to get the world extents
            extents.x = Mathf.Abs(axisX.x) + Mathf.Abs(axisY.x) + Mathf.Abs(axisZ.x);
            extents.y = Mathf.Abs(axisX.y) + Mathf.Abs(axisY.y) + Mathf.Abs(axisZ.y);
            extents.z = Mathf.Abs(axisX.z) + Mathf.Abs(axisY.z) + Mathf.Abs(axisZ.z);

            return new Bounds { center = center, extents = extents };
        }

        private void Pan()
        {
            Vector3 delta = m_lastMousePosition - Input.mousePosition;

            delta = delta / Mathf.Sqrt(Camera.pixelHeight * Camera.pixelHeight + Camera.pixelWidth * Camera.pixelWidth);

            delta *= PanSensitivity;

            delta = Camera.cameraToWorldMatrix.MultiplyVector(delta);
            Camera.transform.position += delta;
            m_pivot += delta;
            m_mouseOrbit.Target = m_pivot;

            m_lastMousePosition = Input.mousePosition;
        }

    }



}
