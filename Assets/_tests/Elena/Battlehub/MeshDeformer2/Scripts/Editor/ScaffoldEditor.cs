using UnityEngine;
using UnityEditor;
using System.Linq;
using Battlehub.SplineEditor;

namespace Battlehub.MeshDeformer2
{
    [CustomEditor(typeof(Scaffold))]
    public class ScaffoldEditor : MeshDeformerEditor
    {
        private MeshDeformer m_meshDeformer;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }

        protected override void OnInspectorGUIOverride()
        {
            if (m_meshDeformer == null)
            {
                m_meshDeformer = (MeshDeformer)GetTarget();
            }

            if (m_meshDeformer == null)
            {
                return;
            }

            Scaffold scaffold = (Scaffold)target;
            ScaffoldWrapper scaffoldWrapper = null;
            int scaffoldIndex = -1;
            for (int i = 0; i < m_meshDeformer.Scaffolds.Length; ++i)
            {
                if(scaffold == m_meshDeformer.Scaffolds[i].Obj)
                {
                    scaffoldIndex = i;
                    scaffoldWrapper = m_meshDeformer.Scaffolds[i];
                    break;
                }
            }

            if(scaffoldWrapper != null)
            {
              
                EditorGUI.BeginChangeCheck();
                bool isRigid = EditorGUILayout.Toggle("Is Rigid", scaffoldWrapper.IsRigid);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(m_meshDeformer, UNDO_CHANGEMODE);
                    RecordScaffolds(m_meshDeformer, "Battlehub.MeshDeformer.Prepend");
                    EditorUtility.SetDirty(m_meshDeformer);

                    for(int i = 0; i < scaffoldWrapper.CurveIndices.Length; ++i)
                    {
                        int curveIndex = scaffoldWrapper.CurveIndices[i];
                        m_meshDeformer.SetIsRigid(curveIndex * 3, isRigid);
                    } 
                }

            }
           
            if (scaffold.gameObject != m_meshDeformer.gameObject)
            {
                GUILayout.BeginHorizontal();
                {
                    if (scaffoldIndex == m_meshDeformer.Scaffolds.Length - 1)
                    {
                        if (GUILayout.Button("Append"))
                        {
                            Undo.RecordObject(m_meshDeformer, "Battlehub.MeshDeformer.Append");
                            Undo.RegisterCreatedObjectUndo(m_meshDeformer.Extend(), "Battlehub.MeshDeformer.Append");
                            EditorUtility.SetDirty(m_meshDeformer);
                            Selection.activeGameObject = m_meshDeformer.GetComponentsInChildren<ControlPoint>(true).Last().gameObject;
                        }
                    }

                    if (scaffoldIndex == 0)
                    {
                        if (GUILayout.Button("Prepend"))
                        {
                            Undo.RecordObject(m_meshDeformer, "Battlehub.MeshDeformer.Prepend");
                            RecordScaffolds(m_meshDeformer, "Battlehub.MeshDeformer.Prepend");
                            Undo.RegisterCreatedObjectUndo(m_meshDeformer.Extend(true), "Battlehub.MeshDeformer.Prepend");
                            EditorUtility.SetDirty(m_meshDeformer);
                            Selection.activeGameObject = m_meshDeformer.GetComponentsInChildren<ControlPoint>(true).First().gameObject;
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }

            if (scaffoldWrapper != null)
            {
                if (GUILayout.Button("Straighten"))
                {
                    Undo.RecordObject(m_meshDeformer, "Battlehub.MeshDeformer.Straighten");
                    RecordScaffolds(m_meshDeformer, "Battlehub.MeshDeformer.Straighten");
                    EditorUtility.SetDirty(m_meshDeformer);
                    m_meshDeformer.Straighten(scaffoldWrapper.CurveIndices.Min() * 3 + 1);
                }
            }
        }


        protected override SplineBase GetTarget()
        {
            Scaffold scaffold = (Scaffold)target;
            if(scaffold != null)
            {
                SplineBase spline = scaffold.GetComponentInParent<SplineBase>();
                return spline;
            }
            return null;
        }

        private void OnSceneGUI()
        {
            SceneGUIOverride();
        }

    }

}
