using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Battlehub.SplineEditor
{
    [CustomEditor(typeof(SplineControlPoint))]
    public class SplineControlPointEditor : SplineBaseEditor
    {
        private Spline m_spline;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }

        protected override void OnInspectorGUIOverride()
        {
            if (m_spline == null)
            {
                m_spline = GetTarget() as Spline;
            }

            if (m_spline == null)
            {
                return;
            }

            int curveIndex = (SelectedIndex - 1) / 3;
            GUILayout.BeginHorizontal();
            {
                if (curveIndex == m_spline.CurveCount - 1)
                {
                    if (GUILayout.Button("Append"))
                    {
                        Undo.RecordObject(m_spline, "Battlehub.Spline.Append");
                        m_spline.Extend();
                        EditorUtility.SetDirty(m_spline);
                        Selection.activeGameObject = m_spline.GetComponentsInChildren<SplineControlPoint>(true).Last().gameObject;
                    }

                }

                if (curveIndex == 0)
                {
                    if (GUILayout.Button("Prepend"))
                    {
                        Undo.RecordObject(m_spline, "Battlehub.Spline.Prepend");
                        m_spline.Extend(true);
                        EditorUtility.SetDirty(m_spline);
                        Selection.activeGameObject = m_spline.GetComponentsInChildren<SplineControlPoint>(true).First().gameObject;
                    }
                }
            }
            GUILayout.EndHorizontal();

            if (SelectedIndex >= 0 && curveIndex < m_spline.CurveCount)
            {
                if (GUILayout.Button("Remove"))
                {
                    Remove();
                }
            }

            base.OnInspectorGUIOverride();
        }

        private void Remove()
        {
            int curveIndex = (SelectedIndex - 1) / 3;
            Spline spline = m_spline;
            Undo.RecordObject(spline, "Battlehub.Spline.Remove");
            if(!spline.Remove(curveIndex))
            {
                EditorUtility.DisplayDialog("Action cancelled", "Unable to remove last curve", "OK");   
            }
            else
            {
                EditorUtility.SetDirty(spline);
            }
            
        }

        protected override void SceneGUIOverride()
        {
            base.SceneGUIOverride();
        }

        protected override SplineBase GetTarget()
        {
            SplineControlPoint controlPoint = (SplineControlPoint)target;
            if(controlPoint)
            {
                SplineBase spline = controlPoint.GetComponentInParent<SplineBase>();
                return spline;
            }
            return null;
        }

        private void OnSceneGUI()
        {
            SplineControlPoint controlPoint = (SplineControlPoint)target;
            SelectedIndex = controlPoint.Index;
            SceneGUIOverride();
        }

    }
}
