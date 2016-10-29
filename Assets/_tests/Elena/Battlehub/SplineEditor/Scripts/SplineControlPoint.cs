using System;
using UnityEngine;

namespace Battlehub.SplineEditor
{
    [ExecuteInEditMode]
    public class SplineControlPoint : MonoBehaviour
    {
        private MeshRenderer m_renderer;
        private Vector3 m_localPosition;

        [SerializeField, HideInInspector]
        private int m_index;

        public int Index
        {
            get { return m_index; }
            set
            {
                m_index = value;
                UpdateMaterial();
            }
        }

        private SplineBase m_spline;
        public SplineBase Spline
        {
            get { return m_spline; }
        }

        private void UpdateMaterial()
        {
            if (m_renderer != null)
            {
                SplineRuntimeEditor runtimeEditor = SplineRuntimeEditor.Instance;
                if (runtimeEditor != null)
                {
                    if (m_index % 3 == 0)
                    {
                        m_renderer.sharedMaterial = runtimeEditor.NormalMaterial;
                    }
                    else
                    {
                        if(m_index >= m_spline.ControlPointCount)
                        {
                            return;
                        }

                        ControlPointMode mode = m_spline.GetControlPointMode(m_index);
                        if (mode == ControlPointMode.Mirrored)
                        {
                            m_renderer.sharedMaterial = runtimeEditor.MirroredModeMaterial;
                        }
                        else if (mode == ControlPointMode.Aligned)
                        {
                            m_renderer.sharedMaterial = runtimeEditor.AlignedModeMaterial;
                        }
                        else
                        {
                            m_renderer.sharedMaterial = runtimeEditor.FreeModeMaterial;
                        }
                    }

                }
            }
        }

        private void CreateRuntimeComponents()
        {
            SplineRuntimeEditor runtimeEditor = SplineRuntimeEditor.Instance;
            if (runtimeEditor != null)
            {
                m_renderer = GetComponent<MeshRenderer>();
                if (!m_renderer)
                {
                    m_renderer = gameObject.AddComponent<MeshRenderer>();
                    MeshFilter filter = gameObject.AddComponent<MeshFilter>();
                    filter.sharedMesh = runtimeEditor.ControlPointMesh;
                    UpdateMaterial();
                }
            }
        }

        private void DestroyRuntimeComponents()
        {
            SplineRuntimeEditor runtimeEditor = SplineRuntimeEditor.Instance;
            if (runtimeEditor != null)
            {
                MeshRenderer renderer = GetComponent<MeshRenderer>();
                if (renderer)
                {
                    DestroyImmediate(renderer);
                }

                MeshFilter filter = GetComponent<MeshFilter>();
                if (filter)
                {
                    DestroyImmediate(filter);
                }
            }
        }

        private void OnRuntimeEditorCreated(object sender, EventArgs e)
        {
            CreateRuntimeComponents();
        }

        private void OnRuntimeEditorDestroyed(object sender, EventArgs e)
        {
            DestroyRuntimeComponents();
        }

        private void OnEnable()
        {
            m_spline = GetComponentInParent<SplineBase>();
            if (m_spline == null)
            {
                return;
            }

            m_spline.ControlPointModeChanged -= OnControlPointModeChanged;
            m_spline.ControlPointModeChanged += OnControlPointModeChanged;
            m_spline.ControlPointPositionChanged -= OnControlPointPositionChanged;
            m_spline.ControlPointPositionChanged += OnControlPointPositionChanged;

            UpdateMaterial();
        }

        private void Start()
        {
            SplineRuntimeEditor.Created += OnRuntimeEditorCreated;
            SplineRuntimeEditor.Destroyed += OnRuntimeEditorDestroyed;

            CreateRuntimeComponents();
            if (m_spline == null)
            {
                m_spline = GetComponentInParent<SplineBase>();
                if (m_spline == null)
                {
                    Debug.LogError("Is not a child of gameobject with Spline or MeshDeformer component");
                    return;
                }
                m_spline.ControlPointModeChanged -= OnControlPointModeChanged;
                m_spline.ControlPointModeChanged += OnControlPointModeChanged;
                m_spline.ControlPointPositionChanged -= OnControlPointPositionChanged;
                m_spline.ControlPointPositionChanged += OnControlPointPositionChanged;
            }

            m_localPosition = m_spline.GetControlPointLocal(m_index);
            transform.localPosition = m_localPosition;

            UpdateMaterial();
        }

        private void OnDisable()
        {
            if (m_spline == null)
            {
                return;
            }

            m_spline.ControlPointModeChanged -= OnControlPointModeChanged;
            m_spline.ControlPointPositionChanged -= OnControlPointPositionChanged;
        }

        protected void OnDestroy()
        {
            if (m_spline != null)
            {
                m_spline.ControlPointModeChanged -= OnControlPointModeChanged;
                m_spline.ControlPointPositionChanged -= OnControlPointPositionChanged;
            }

            SplineRuntimeEditor.Created -= OnRuntimeEditorCreated;
            SplineRuntimeEditor.Destroyed -= OnRuntimeEditorDestroyed;
        }

        private void Update()
        {
            if (transform.localPosition != m_localPosition)
            {
                m_localPosition = transform.localPosition;
                if (m_spline != null)
                {
                    m_spline.SetControlPointLocal(m_index, m_localPosition);
                }
            }
        }

        private void OnControlPointModeChanged(int pointIndex)
        {
            if (pointIndex == m_index)
            {
                UpdateMaterial();
            }
        }

        private void OnControlPointPositionChanged(int pointIndex)
        {
            if (m_spline == null)
            {
                return;
            }

            if (pointIndex == m_index)
            {
                transform.position = m_spline.transform.TransformPoint(m_spline.GetControlPointLocal(pointIndex));
                m_localPosition = transform.localPosition;
            }
        }
    }
}

