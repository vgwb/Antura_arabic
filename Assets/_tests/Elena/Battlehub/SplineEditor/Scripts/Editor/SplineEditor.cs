using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Battlehub.SplineEditor
{
    [CustomEditor(typeof(Spline))]
    public class SplineEditor : SplineBaseEditor
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
                m_spline = (Spline)GetTarget();
            }

            if (m_spline == null)
            {
                return;
            }

            EditorGUI.BeginChangeCheck();
            bool loop = EditorGUILayout.Toggle("Loop", m_spline.Loop);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(m_spline, UNDO_TOGGLELOOP);
                EditorUtility.SetDirty(m_spline);
                m_spline.Loop = loop;
            }

            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Append"))
                {
                    Undo.RecordObject(m_spline, "Battlehub.Spline.Append");
                    m_spline.Extend();
                    EditorUtility.SetDirty(m_spline);
                }

                if (GUILayout.Button("Prepend"))
                {
                    Undo.RecordObject(m_spline, "Battlehub.Spline.Prepend");
                    m_spline.Extend(true);
                    EditorUtility.SetDirty(m_spline);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Set Free Mode"))
                {
                    Undo.RecordObject(m_spline, "Battlehub.Spline.SetMode");
                    m_spline.SetControlPointMode(ControlPointMode.Free);
                    EditorUtility.SetDirty(m_spline);
                }

                if (GUILayout.Button("Set Aligned Mode"))
                {
                    Undo.RecordObject(m_spline, "Battlehub.Spline.SetMode");
                    m_spline.SetControlPointMode(ControlPointMode.Aligned);
                    EditorUtility.SetDirty(m_spline);
                }

                if (GUILayout.Button("Set Mirrored Mode"))
                {
                    Undo.RecordObject(m_spline, "Battlehub.Spline.SetMode");
                    m_spline.SetControlPointMode(ControlPointMode.Mirrored);
                    EditorUtility.SetDirty(m_spline);
                }
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Smooth"))
            {
                Undo.RecordObject(m_spline, "Battlehub.Spline.SetMode");
                m_spline.Smooth();
                EditorUtility.SetDirty(m_spline);
            }
        }

        protected override void SceneGUIOverride()
        {
            base.SceneGUIOverride();
        }

        protected override SplineBase GetTarget()
        {
            Spline spline = (Spline)target;
            return spline;
        }

        private void OnSceneGUI()
        {
            SceneGUIOverride();
        }
    }
}

