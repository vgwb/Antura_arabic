using UnityEngine;
using System;
using System.Linq;

using Battlehub.RTHandles;
using Battlehub.UIControls;
using UnityEngine.EventSystems;

namespace Battlehub.SplineEditor
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(GLRenderer))]
    public class SplineRuntimeEditor : MonoBehaviour
    {
        public static event EventHandler Created;
        public static event EventHandler Destroyed;

        public Camera Camera;
        public float SelectionMargin = 10;
        public float Smoothness = 5.0f;
        public static readonly Color SplineColor = Color.green;
        public static readonly Color MirroredModeColor = Color.red;
        public static readonly Color AlignedModeColor = Color.blue;
        public static readonly Color FreeModeColor = Color.yellow;
        public static readonly Color ControlPointLineColor = Color.gray;

        private Material m_splineMaterial;
        private Material m_normalMaterial;
        private Material m_mirroredModeMaterial;
        private Material m_alignedModeMaterial;
        private Material m_freeModeMaterial;
        private Mesh m_controlPointMesh;

        private bool m_unselectControlPoint;
        private RuntimeHierarchy m_hierarchy;
        
        public Mesh ControlPointMesh
        {
            get { return m_controlPointMesh; }
        }

        public Material MirroredModeMaterial
        {
            get { return m_mirroredModeMaterial; }
        }

        public Material AlignedModeMaterial
        {
            get { return m_alignedModeMaterial; }
        }

        public Material FreeModeMaterial
        {
            get { return m_freeModeMaterial; }
        }

        public Material NormalMaterial
        {
            get { return m_normalMaterial; }
        }

        public Material SplineMaterial
        {
            get { return m_splineMaterial; }
        }

        private static SplineRuntimeEditor m_instance;
        public static SplineRuntimeEditor Instance
        {
            get { return m_instance; }
        }

        private SplineBase m_selectedSpline;
        public SplineBase SelectedSpline
        {
            get { return m_selectedSpline; }
            set
            {
                if(m_selectedSpline != value)
                {
                    SplineBase oldSpline = m_selectedSpline;
                    m_selectedSpline = value;
                    OnSelectedSplineChanged(oldSpline, m_selectedSpline);
                }
            }
        }

        private void Awake()
        {
            if (m_instance != null)
            {
                Debug.LogWarning("Another instance of SplineEditorSettings already exist");
            }

            if (m_mirroredModeMaterial == null)
            {
                Shader shader = Shader.Find("Battlehub/SplineEditor/SSBillboard");
           
                m_mirroredModeMaterial = new Material(shader);
                m_mirroredModeMaterial.name = "MirroredModeMaterial";
                m_mirroredModeMaterial.color = MirroredModeColor;
                m_mirroredModeMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                m_mirroredModeMaterial.SetInt("_ZWrite", 1);
                m_mirroredModeMaterial.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
            }

            if (m_alignedModeMaterial == null)
            {
                m_alignedModeMaterial = Instantiate(m_mirroredModeMaterial);
                m_alignedModeMaterial.name = "AlignedModeMaterial";
                m_alignedModeMaterial.color = AlignedModeColor;
            }

            if (m_freeModeMaterial == null)
            {
                m_freeModeMaterial = Instantiate(m_mirroredModeMaterial);
                m_freeModeMaterial.name = "FreeModeMaterial";
                m_freeModeMaterial.color = FreeModeColor;
            }

            if (m_normalMaterial == null)
            {
                m_normalMaterial = Instantiate(m_mirroredModeMaterial);
                m_normalMaterial.name = "SplineMaterial";
                m_normalMaterial.color = SplineColor;
            }

            if(m_splineMaterial == null)
            {
                Shader shader = Shader.Find("Battlehub/SplineEditor/Spline");
                m_splineMaterial = new Material(shader);
                m_splineMaterial.name = "SplineMaterial";
                m_splineMaterial.color = SplineColor;
            }

            if (m_controlPointMesh == null)
            {
                m_controlPointMesh = new Mesh();
                m_controlPointMesh.name = "control point mesh";
                m_controlPointMesh.vertices = new[]
                {
                    new Vector3(0, 0, 0),
                    new Vector3(0, 0, 0),
                    new Vector3(0, 0, 0),
                    new Vector3(0, 0, 0)
                };
                m_controlPointMesh.triangles = new[]
                {
                    0, 1, 2, 0, 2, 3
                };
                m_controlPointMesh.uv = new[]
                {
                    new Vector2(-1, -1),
                    new Vector2(1, -1),
                    new Vector2(1, 1),
                    new Vector2(-1, 1)
                };
                m_controlPointMesh.RecalculateBounds();
            }

            m_instance = this;
            EnableRuntimeEditing();

            m_hierarchy = GetComponentInChildren<RuntimeHierarchy>();
            if(m_hierarchy != null)
            {
                m_hierarchy.TypeCriteria = typeof(SplineBase);
                m_hierarchy.SelectionChanged += OnHierarchySelectionChanged;

#if UNITY_EDITOR
                m_hierarchy.SelectedItem = UnityEditor.Selection.activeGameObject;
#endif
            }
        }

        private void Start()
        {
            if (Created != null)
            {
                Created(this, EventArgs.Empty);
            }

            if(Camera == null)
            {
                Camera = Camera.main;
            }


        }

        private void OnDestroy()
        {
            if (!Application.isPlaying)
            {
                DisableRuntimeEditing();
            }

            if (Destroyed != null)
            {
                Destroyed(this, EventArgs.Empty);
            }

            if (m_hierarchy != null)
            {
                m_hierarchy.SelectionChanged -= OnHierarchySelectionChanged;
            }


            m_instance = null;

        }



        private void DisableRuntimeEditing()
        {
            Camera[] allCameras = Camera.allCameras;
            for (int i = 0; i < allCameras.Length; ++i)
            {
                Camera camera = allCameras[i];
                GLCamera glLinesCamera = camera.GetComponent<GLCamera>();
                if (glLinesCamera != null)
                {
                    DestroyImmediate(glLinesCamera);
                }
            }
        }

        private void EnableRuntimeEditing()
        {
            Camera[] allCameras = Camera.allCameras;
            for (int i = 0; i < allCameras.Length; ++i)
            {
                Camera camera = allCameras[i];
                if (!camera.GetComponent<GLCamera>())
                {
                    camera.gameObject.AddComponent<GLCamera>();
                }
            }
        }

        private void LateUpdate()
        {
            if (Application.isPlaying)
            {
                if (SelectedSpline != null)
                {
                    if (Input.GetMouseButtonDown(0) && (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject()))
                    {
                        int selectedIndex = HitTest();
                        if (selectedIndex != -1)
                        {
                            SplineControlPoint ctrlPoint = SelectedSpline.GetComponentsInChildren<SplineControlPoint>().Where(p => p.Index == selectedIndex).FirstOrDefault();
                            RuntimeSelection.activeGameObject = ctrlPoint.gameObject;
                        }
                        else
                        {
                            if (RuntimeTools.Current != RuntimeTool.View)
                            {
                                if (RuntimeSelection.activeGameObject != null)
                                {
                                    if(PositionHandle.Current != null && !PositionHandle.Current.IsDragging)
                                    {
                                        if (SelectedSpline != null)
                                        {
                                            RuntimeSelection.activeGameObject = SelectedSpline.gameObject;
                                        }
                                    }    
                                }
                            }
                        }
                    }
                }
            }

            if (m_instance == null)
            {
                m_instance = this;
                SplineBase[] splines = FindObjectsOfType<SplineBase>();
                for (int i = 0; i < splines.Length; ++i)
                {
                    SplineBase spline = splines[i];
                    if (spline.IsSelected)
                    {
                        spline.Select();
                    }
                }
            }
        }

        private void OnHierarchySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.NewItem != null && e.NewItem is SplineBase)
            {
                SplineBase newSpline = (SplineBase)e.NewItem;
                if (!newSpline.IsSelected)
                {
                    newSpline.Select();
                }

#if UNITY_EDITOR
                if(UnityEditor.Selection.activeGameObject == null || UnityEditor.Selection.activeGameObject.GetComponentInParent<SplineBase>() != m_hierarchy.SelectedItem)
                {
                    UnityEditor.Selection.activeObject = m_hierarchy.SelectedItem;
                }
#endif
            }
            else if(e.OldItem != null && e.OldItem is SplineBase)
            {
                SplineBase oldSpline = (SplineBase)e.OldItem;
                if(oldSpline.IsSelected)
                {
                    oldSpline.Unselect();
                }

#if UNITY_EDITOR
                if (UnityEditor.Selection.activeObject == (UnityEngine.Object)e.OldItem)
                {
                    UnityEditor.Selection.activeObject = null;
                }

#endif
            }
        }

        private void OnSelectedSplineChanged(SplineBase oldSpline, SplineBase newSpline)
        {
            if (Application.isPlaying)
            {
                if (oldSpline != null)
                {
                    oldSpline.Unselect();
                    if(RuntimeSelection.activeGameObject != null && RuntimeSelection.activeGameObject.GetComponentInParent<SplineBase>() == oldSpline)
                    {
                        RuntimeSelection.activeGameObject = null;
                    }
                }

                if (newSpline != null)
                {
                    RuntimeSelection.activeGameObject = newSpline.gameObject;
                }
                m_hierarchy.SelectedItem = newSpline;


            }
            else
            {
                if (oldSpline != null)
                {
                    oldSpline.Unselect();
                }
            }

        }


        private int HitTest()
        {
            if(Camera == null)
            {
                Debug.LogError("Camera is null");
                return -1;
            }

            int count = SelectedSpline.ControlPointCount;
            Vector3[] ctrlPoints = new Vector3[count];
            for(int i = 0; i < count; ++i)
            {
                ctrlPoints[i] = SelectedSpline.GetControlPoint(i);
            }


            float minMag = SelectionMargin * SelectionMargin;
            int selectedIndex = -1;
            Vector2 mousePositon = Input.mousePosition;
            for(int i = 0; i < count; ++i)
            {
                Vector3 ctrlPoint = ctrlPoints[i];
                Vector2 pt = Camera.WorldToScreenPoint(ctrlPoint);
                float mag = (pt - mousePositon).sqrMagnitude;
                if (mag < minMag )
                {
                    minMag = mag;
                    selectedIndex = i;
                }
            }

            return selectedIndex;
        }
    }

}
