using UnityEngine;
using UnityEditor;
using Battlehub.MeshDeformer2;
using Battlehub.Integration;
using Battlehub.SplineEditor;
namespace Battlehub.Wire2
{
    [CustomEditor(typeof(Wire))]
    public class WireEditor : MeshDeformerEditor
    {
        private Wire m_wire;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }

        protected override void OnInspectorGUIOverride()
        {
            if (m_wire == null)
            {
                m_wire = (Wire)GetTarget();
            }

            if (m_wire == null)
            {
                return;
            }
         
            EditorGUI.BeginChangeCheck();
            int sliceCount = EditorGUILayout.IntField("LOD", m_wire.Approximation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(m_wire, "Battlehub.Wire.Approximation Changed");
                RecordScaffolds(m_wire, "Battlehub.Wire.Approximation Changed");
                EditorUtility.SetDirty(m_wire);
                m_wire.Approximation = sliceCount;
                WiresIntegration.RaiseWireParamsChanged(m_wire.gameObject, m_wire.Original);
            }

            EditorGUI.BeginChangeCheck();
            int sectors = EditorGUILayout.IntField("Sectors", m_wire.SectorsCount);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(m_wire, "Battlehub.Wire.Sectors Changed");
                RecordScaffolds(m_wire, "Battlehub.Wire.Sectors Changed");
                EditorUtility.SetDirty(m_wire);
                m_wire.SectorsCount = sectors;
                WiresIntegration.RaiseWireParamsChanged(m_wire.gameObject, m_wire.Original);
            }

            EditorGUI.BeginChangeCheck();
            float thikness = EditorGUILayout.FloatField("Thickness", m_wire.WireRadius);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(m_wire, "Battlehub.Wire.Thikness Changed");
                RecordScaffolds(m_wire, "Battlehub.Wire.Thikness Changed");
                EditorUtility.SetDirty(m_wire);
                m_wire.WireRadius = thikness;
                WiresIntegration.RaiseWireParamsChanged(m_wire.gameObject, m_wire.Original);
            }

            EditorGUI.BeginChangeCheck();
            int curvesPerMesh = EditorGUILayout.IntField("Curves Per Segment", m_wire.CurvesPerMesh);
            if (EditorGUI.EndChangeCheck())
            {
                Selection.activeGameObject = m_wire.gameObject;
                Undo.RecordObject(m_wire, "Battlehub.Wire.CurvesPerMesh Changed");
                RecordScaffolds(m_wire, "Battlehub.Wire.CurvesPerMesh Changed");
                EditorUtility.SetDirty(m_wire);
                m_wire.CurvesPerMesh = curvesPerMesh;
            }

            EditorGUI.BeginChangeCheck();
            bool loop = EditorGUILayout.Toggle("Loop", m_wire.Loop);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(m_wire, UNDO_TOGGLELOOP);
                EditorUtility.SetDirty(m_wire);
                m_wire.Loop = loop;
            }

            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                {
                    if (GUILayout.Button("Append"))
                    {
                        Undo.RecordObject(m_wire, "Battlehub.Wire.Append");
                        Undo.RegisterCreatedObjectUndo(m_wire.Extend(), "Battlehub.Wire.Append");
                        EditorUtility.SetDirty(m_wire);
                        //Selection.activeGameObject = m_wire.GetComponentsInChildren<ControlPoint>().Last().gameObject;
                    }

                    if (GUILayout.Button("Set Rigid Mode"))
                    {
                        Undo.RecordObject(m_wire, "Battlehub.Wire.SetMode");
                        RecordScaffolds(m_wire, "Battlehub.Wire.SetMode");
                        EditorUtility.SetDirty(m_wire);
                        SetMode(m_wire, ControlPointMode.Free, true);
                    }

                    if (GUILayout.Button("Set Free Mode"))
                    {
                        Undo.RecordObject(m_wire, "Battlehub.Wire.SetMode");
                        RecordScaffolds(m_wire, "Battlehub.Wire.SetMode");
                        EditorUtility.SetDirty(m_wire);
                        SetMode(m_wire, ControlPointMode.Free, false);
                    }

                }
                GUILayout.EndVertical();
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                {
                    if (GUILayout.Button("Prepend"))
                    {
                        Undo.RecordObject(m_wire, "Battlehub.Wire.Prepend");
                        RecordScaffolds(m_wire, "Battlehub.Wire.Prepend");
                        Undo.RegisterCreatedObjectUndo(m_wire.Extend(true), "Battlehub.Wire.Prepend");
                        EditorUtility.SetDirty(m_wire);
                        //Selection.activeGameObject = m_wire.GetComponentsInChildren<ControlPoint>().First().gameObject;
                    }

                    if (GUILayout.Button("Set Aligned Mode"))
                    {
                        Undo.RecordObject(m_wire, "Battlehub.Wire.SetMode");
                        RecordScaffolds(m_wire, "Battlehub.Wire.SetMode");
                        EditorUtility.SetDirty(m_wire);
                        SetMode(m_wire, ControlPointMode.Free, false);
                        SetMode(m_wire, ControlPointMode.Aligned, false);
                    }

                    if (GUILayout.Button("Set Mirrored Mode"))
                    {
                        Undo.RecordObject(m_wire, "Battlehub.Wire.SetMode");
                        RecordScaffolds(m_wire, "Battlehub.Wire.SetMode");
                        EditorUtility.SetDirty(m_wire);
                        SetMode(m_wire, ControlPointMode.Free, false);
                        SetMode(m_wire, ControlPointMode.Mirrored, false);
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();


        }

        protected override void SceneGUIOverride()
        {
            base.SceneGUIOverride();
        }

        protected override SplineBase GetTarget()
        {
            Wire wire = (Wire)target;
            if(wire != null)
            {
                return wire;
            }
            return null;
            
        }

        private void OnSceneGUI()
        {
            SceneGUIOverride();
        }

    }
}

