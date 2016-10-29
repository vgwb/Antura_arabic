using UnityEngine;
using UnityEditor;
using System.Linq;

using Battlehub.SplineEditor;

namespace Battlehub.MeshDeformer2
{
    [CustomEditor(typeof(MeshDeformer))]
    public class MeshDeformerEditor : SplineEditor.SplineBaseEditor
    {
        private MeshDeformer m_deformer;

        protected override void OnEnableOverride()
        {
            base.OnEnableOverride();
        }

        public static void RecordScaffolds(MeshDeformer deformer, string name)
        {
            Scaffold[] scaffolds = deformer.GetComponentsInChildren<Scaffold>();
            for (int i = 0; i < scaffolds.Length; ++i)
            {
                Scaffold scaffold = scaffolds[i];
                Undo.RecordObject(scaffold, name);
            }
        }

        private void DoOriginalMeshes()
        {

            EditorGUI.BeginChangeCheck();
            Mesh originalMesh = (Mesh)EditorGUILayout.ObjectField("OriginalMesh", m_deformer.Original, typeof(Mesh), false);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(m_deformer);
                Undo.RecordObject(m_deformer, "Battlehub.MeshDeformer.OriginalMeshes");
                m_deformer.Original = originalMesh;
                m_deformer.Internal_HasChanged = true;
            }

            EditorGUI.BeginChangeCheck();
            Mesh colliderOriginalMesh = (Mesh)EditorGUILayout.ObjectField("Original Collider Mesh", m_deformer.ColliderOriginal, typeof(Mesh), false);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(m_deformer);
                Undo.RecordObject(m_deformer, "Battlehub.MeshDeformer.OriginalMeshes");

                MeshCollider collider = m_deformer.GetComponent<MeshCollider>();
                if(collider != null)
                {
                    Undo.RecordObject(collider, "Battlehub.MeshDeformer.OriginalMeshes");
                    if(colliderOriginalMesh)
                    {
                        collider.sharedMesh = GameObject.Instantiate(colliderOriginalMesh);
                        collider.sharedMesh.name = colliderOriginalMesh.name + " Collider";
                    }
                    else
                    {
                        collider.sharedMesh = new Mesh();
                    }

                    m_deformer.ColliderOriginal = collider.sharedMesh;
                }
                
                m_deformer.Internal_HasChanged = true;
            }
        }

        protected override void OnInspectorGUIOverride()
        {
            if(m_deformer == null)
            {
                m_deformer = GetTarget() as MeshDeformer;
            }

            if(m_deformer == null)
            {
                return;
            }

            DoOriginalMeshes();

            EditorGUI.BeginChangeCheck();
            Axis axis = (Axis)EditorGUILayout.EnumPopup("Deformation Axis", m_deformer.Axis);
            if (EditorGUI.EndChangeCheck())
            {
                ChangeAxis(m_deformer, axis);
            }

            EditorGUI.BeginChangeCheck();
            int sliceCount = EditorGUILayout.IntField("Approximation", m_deformer.Approximation);
            if(EditorGUI.EndChangeCheck())
            {
                if (sliceCount > 100)
                {
                    if (!EditorUtility.DisplayDialog("Are you sure", "Are you sure you want to set Approximation = " + sliceCount, "Yes", "No"))
                    {
                        sliceCount = m_deformer.Approximation;
                    }
                }
                Undo.RecordObject(m_deformer, "Battlehub.MeshDeformer.Approximation Changed");
                RecordScaffolds(m_deformer, "Battlehub.MeshDeformer.Approximation Changed");
                EditorUtility.SetDirty(m_deformer);

                Debug.Log("Undo Operations will be Collapsed");
                Undo.CollapseUndoOperations(Undo.GetCurrentGroup());

                m_deformer.Approximation = sliceCount;
            }

            EditorGUI.BeginChangeCheck();
            float spacing = EditorGUILayout.FloatField("Spacing", m_deformer.Spacing);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(m_deformer, "Battlehub.MeshDeformer.Spacing Changed");
                RecordScaffolds(m_deformer, "Battlehub.MeshDeformer.Spacing Changed");
                EditorUtility.SetDirty(m_deformer);
                m_deformer.Spacing = spacing;
            }

            EditorGUI.BeginChangeCheck();
            int curvesPerMesh = EditorGUILayout.IntField("Curves Per Mesh", m_deformer.CurvesPerMesh);
            if (EditorGUI.EndChangeCheck())
            {
                Selection.activeGameObject = m_deformer.gameObject;
                Undo.RecordObject(m_deformer, "Battlehub.MeshDeformer.CurvesPerMesh Changed");
                RecordScaffolds(m_deformer, "Battlehub.MeshDeformer.CurvesPerMesh Changed");
                EditorUtility.SetDirty(m_deformer);
                m_deformer.CurvesPerMesh = curvesPerMesh;
            }

            EditorGUI.BeginChangeCheck();
            bool loop = EditorGUILayout.Toggle("Loop", m_deformer.Loop);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(m_deformer, UNDO_TOGGLELOOP);
                EditorUtility.SetDirty(m_deformer);
                m_deformer.Loop = loop;
            }


            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Append"))
                {
                    Undo.RecordObject(m_deformer, "Battlehub.MeshDeformer.Append");
                    Undo.RegisterCreatedObjectUndo(m_deformer.Extend(), "Battlehub.MeshDeformer.Append");
                    EditorUtility.SetDirty(m_deformer);
                    //Selection.activeGameObject = m_deformer.GetComponentsInChildren<ControlPoint>().Last().gameObject;
                }

                if (GUILayout.Button("Prepend"))
                {
                    Undo.RecordObject(m_deformer, "Battlehub.MeshDeformer.Prepend");
                    RecordScaffolds(m_deformer, "Battlehub.MeshDeformer.Prepend");
                    Undo.RegisterCreatedObjectUndo(m_deformer.Extend(true), "Battlehub.MeshDeformer.Prepend");
                    EditorUtility.SetDirty(m_deformer);
                    //Selection.activeGameObject = m_deformer.GetComponentsInChildren<ControlPoint>().First().gameObject;
                }
            }
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Smooth Spline"))
            {
                Undo.RecordObject(m_deformer, "Battlehub.MeshDeformer.Fit");
                RecordScaffolds(m_deformer, "Battlehub.MeshDeformer.Fit");
                EditorUtility.SetDirty(m_deformer);
                m_deformer.Smooth();
            }

            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                {
                    if (GUILayout.Button("Set Rigid Mode"))
                    {
                        Undo.RecordObject(m_deformer, "Battlehub.MeshDeformer.SetMode");
                        RecordScaffolds(m_deformer, "Battlehub.MeshDeformer.SetMode");
                        EditorUtility.SetDirty(m_deformer);
                        SetMode(m_deformer, ControlPointMode.Free, true);
                    }

                    if (GUILayout.Button("Set Free Mode"))
                    {
                        Undo.RecordObject(m_deformer, "Battlehub.MeshDeformer.SetMode");
                        RecordScaffolds(m_deformer, "Battlehub.MeshDeformer.SetMode");
                        EditorUtility.SetDirty(m_deformer);
                        SetMode(m_deformer, ControlPointMode.Free, false);
                    }

                }
                GUILayout.EndVertical();
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                {
                    
                    if (GUILayout.Button("Set Aligned Mode"))
                    {
                        Undo.RecordObject(m_deformer, "Battlehub.MeshDeformer.SetMode");
                        RecordScaffolds(m_deformer, "Battlehub.MeshDeformer.SetMode");
                        EditorUtility.SetDirty(m_deformer);
                        SetMode(m_deformer, ControlPointMode.Free, false);
                        SetMode(m_deformer, ControlPointMode.Aligned, false);
                    }

                    if (GUILayout.Button("Set Mirrored Mode"))
                    {
                        Undo.RecordObject(m_deformer, "Battlehub.MeshDeformer.SetMode");
                        RecordScaffolds(m_deformer, "Battlehub.MeshDeformer.SetMode");
                        EditorUtility.SetDirty(m_deformer);
                        SetMode(m_deformer, ControlPointMode.Free, false);
                        SetMode(m_deformer, ControlPointMode.Mirrored, false);
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();



#if MD_GIZMOS
            EditorGUI.BeginChangeCheck();
            bool drawGizoms = EditorGUILayout.Toggle("Draw Gizmos", m_deformer.DrawGizmos);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(m_deformer, "Battlehub.MeshDeformer.Approximation Changed");
                EditorUtility.SetDirty(m_deformer);
                m_deformer.DrawGizmos = drawGizoms;
            }
#endif
        }

    

        public static void ChangeAxis(MeshDeformer deformer, Axis axis)
        {
            Undo.RecordObject(deformer, "Battlehub.MeshDeformer.Axis Changed");
            RecordScaffolds(deformer, "Battlehub.MeshDeformer.Axis Changed");
            EditorUtility.SetDirty(deformer);
            deformer.Axis = axis;
        }

        public static void SetMode(MeshDeformer deformer, ControlPointMode mode, bool isRigid)
        {
            ScaffoldWrapper[] scaffolds = deformer.Scaffolds;
            for (int s = 0; s < scaffolds.Length; ++s)
            {
                ScaffoldWrapper scaffold = scaffolds[s];
                for (int i = 0; i < scaffold.CurveIndices.Length; ++i)
                {
                    int curveIndex = scaffold.CurveIndices[i];
                    if (mode == ControlPointMode.Free)
                    {
                        deformer.SetIsRigid(curveIndex * 3, isRigid);
                    }

                    if (!isRigid)
                    {
                        deformer.SetControlPointMode(curveIndex * 3, mode);
                        deformer.SetControlPointMode(curveIndex * 3 + 3, mode);
                    }
                }
            }
        }

        private void OnSceneGUI()
        {
            SceneGUIOverride();
        }

        protected override void SceneGUIOverride()
        {
            if (m_deformer == null)
            {
                m_deformer = GetTarget() as MeshDeformer;
            }

            base.SceneGUIOverride();
        }

        protected override bool CanShowPoint(int index)
        {
            return true;
        }

        protected override bool DrawSelectedPointInspectorOverride()
        {
            if(m_deformer == null)
            {
                return true;
            }

            EditorGUI.BeginChangeCheck();
            int curveIndex = SelectedIndex  / 3;
            if(curveIndex == m_deformer.CurveCount)
            {
                if(m_deformer.Loop)
                {
                    curveIndex = 0;
                }
                else
                {
                    curveIndex--;
                }
            }

            ScaffoldWrapper scaffold = m_deformer.FindScaffold(curveIndex);
            if(scaffold == null)
            {
                return true;
            }

            bool isRigid = EditorGUILayout.Toggle("Is Rigid", scaffold.IsRigid);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(m_deformer, UNDO_CHANGEMODE);
                EditorUtility.SetDirty(m_deformer);

                m_deformer.SetIsRigid(SelectedIndex, isRigid);
            }

            return !isRigid;
        }

        protected override void ShowTwistAngles()
        { 
            if(m_deformer == null)
            {
                base.ShowTwistAngles();
                return;
            }

            Handles.color = Color.green;
            int steps = GetStepsPerCurve();
            if(steps <= 0)
            {
                steps = 1;
            }
            else if(steps > 5)
            {
                steps = 5;
            }
            
            ScaffoldWrapper[] scaffolds = m_deformer.Scaffolds;
            for (int i = 0; i < scaffolds.Length; ++i)
            {
                ScaffoldWrapper scaffold = m_deformer.Scaffolds[i];
                for(int ci = 0; ci < scaffold.CurveIndices.Length; ++ci)
                {
                    int curveIndex = scaffold.CurveIndices[ci];
                    for (int j = 0; j <= steps; j++)
                    {
                        float t = (float)j / steps;
                        Vector3 dir = m_deformer.GetDirection(t, curveIndex);
                        Vector3 point = m_deformer.GetPoint(t, curveIndex);

                        if (scaffold.IsRigid)
                        {
                            t = 1.0f;
                        }
                        float twistAngle = m_deformer.GetTwist(t, curveIndex);

                        Vector3 v3;
                        Vector3 up = GetUpVector();
                        if (Mathf.Abs(Vector3.Dot(dir, up)) < 1.0f)
                        {
                            v3 = Vector3.Cross(dir, up).normalized;
                        }
                        else
                        {
                            v3 = Vector3.Cross(dir, GetSideVector()).normalized;
                        }

                        if (dir == Vector3.zero)
                        {
                            continue;
                        }

                        Handles.DrawLine(point, point + Quaternion.AngleAxis(twistAngle, dir) * Quaternion.LookRotation(v3, up) * Vector3.forward * TwistAngleScale);
                    }
                }
             
            }

        }


        protected override int GetStepsPerCurve()
        {
            if(m_deformer == null)
            {
                return base.GetStepsPerCurve();
            }
            return m_deformer.Approximation;
        }

        protected override Vector3 GetSideVector()
        {
            if (m_deformer == null)
            {
                return Vector3.right;
            }
            return MeshDeformer.Side(m_deformer.Axis);
        }

        protected override Vector3 GetUpVector()
        {
            if(m_deformer == null)
            {
                return Vector3.up;
            }
            return MeshDeformer.Up(m_deformer.Axis);
        }

      
    }
}
