using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Battlehub.SplineEditor
{
    public class SplineBaseEditor : Editor
    {
        public const string UNDO_ADDCURVE = "Battlehub.MeshDeformer.AddCurve";
        public const string UNDO_MOVEPOINT = "Battlehub.MeshDeformer.MovePoint";
        public const string UNDO_CHANGEMODE = "Battlehub.MeshDeformer.ChangePointMode";
        public const string UNDO_TOGGLELOOP = "Battlehub.MeshDeformer.ToggleLoop";

        private const int StepsPerCurve = 5;
        private const float DirectionScale = 0.0f;
        protected const float TwistAngleScale = 0.25f;
        private const float HandleSize = 0.04f;
        private const float PickSize = 0.15f;

        private int m_selectedIndex = -1;
        private SplineBase m_splineBase;
        private Transform m_handleTransform;
        private Quaternion m_handleRotation;

        protected int SelectedIndex
        {
            get { return m_selectedIndex; }
            set { m_selectedIndex = value; }
        }

        private static readonly Color[] ModeColors = {
            Color.yellow,
            Color.blue,
            Color.red,
        };


        private void OnEnable()
        {
            OnEnableOverride();

            SplineBase spline = GetTarget();
            if(spline)
            {
                spline.Select();
            }
        }

        protected virtual void OnEnableOverride()
        {

        }

        private void OnDisable()
        {
            OnDisableOverride();

            SplineBase spline = GetTarget();
            if(spline)
            {
                spline.Unselect();
            }
            
        }

        protected virtual void OnDisableOverride()
        {

        }

        private void OnDestroy()
        {

        }


        public override void OnInspectorGUI()
        {
            SerializedObject sObj = GetSerializedObject();
            sObj.Update();
            if (m_splineBase == null)
            {
                m_splineBase = GetTarget();
            }

            if (m_splineBase == null)
            {
                return;
            }


            if (m_selectedIndex >= 0 && m_selectedIndex < m_splineBase.ControlPointCount)
            {
                DrawSelectedPointInspector();
            }

            OnInspectorGUIOverride();


            if (target != null)
            {
                sObj.ApplyModifiedProperties();
            }

        }

        protected virtual void OnInspectorGUIOverride()
        {
            EditorGUI.BeginChangeCheck();
            bool loop = EditorGUILayout.Toggle("Loop", m_splineBase.Loop);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(m_splineBase, UNDO_TOGGLELOOP);
                EditorUtility.SetDirty(m_splineBase);
                m_splineBase.Loop = loop;
            }
        }


        private void OnSceneGUI()
        {
            SceneGUIOverride();
        }

     
        protected virtual void SceneGUIOverride()
        {
            if (m_splineBase == null)
            {
                m_splineBase = GetTarget();
            }

            if (m_splineBase == null)
            {
                return;
            }

            m_handleTransform = m_splineBase.transform;
            m_handleRotation = Tools.pivotRotation == PivotRotation.Local ?
                m_handleTransform.rotation : Quaternion.identity;

            Vector3 p0 = m_handleTransform.TransformPoint(m_splineBase.GetControlPointLocal(0));
            ShowPoint(0, p0);
            for (int i = 1; i < m_splineBase.ControlPointCount; i += 3)
            {
                Vector3 p1 = m_handleTransform.TransformPoint(m_splineBase.GetControlPointLocal(i));
                Vector3 p2 = m_handleTransform.TransformPoint(m_splineBase.GetControlPointLocal(i + 1));
                Vector3 p3 = m_handleTransform.TransformPoint(m_splineBase.GetControlPointLocal(i + 2));
                

                Handles.color = Color.gray;
                Handles.DrawLine(p0, p1);
                Handles.DrawLine(p2, p3);

                //if(m_spline.DrawGizmos)
                {
                    Handles.DrawBezier(p0, p3, p1, p2, Color.green, null, 1.8f);
                }

                ShowPoint(i, p1);
                ShowPoint(i + 1, p2);
                ShowPoint(i + 2, p3);
                p0 = p3;
            }
            //ShowDirections();
            //if(m_spline.DrawGizmos)
            {
                ShowTwistAngles();
            }

        }

        protected virtual int GetStepsPerCurve()
        {
            return StepsPerCurve;
        }

        protected virtual Vector3 GetUpVector()
        {
            return Vector3.up;
        }

        protected virtual Vector3 GetSideVector()
        {
            return Vector3.forward;
        }

        protected virtual void ShowTwistAngles()
        {
            Handles.color = Color.green;
           
            int steps = GetStepsPerCurve() * m_splineBase.CurveCount;
            for (int i = 0; i <= steps; i++)
            {
                Vector3 dir = m_splineBase.GetDirection(i / (float)steps);
                Vector3 point = m_splineBase.GetPoint(i / (float)steps);

                float t = i / (float)steps;
                float twistAngle = m_splineBase.GetTwist(t);
                Vector3 v3;
                Vector3 up = GetUpVector();
                if (Math.Abs(Vector3.Dot(dir, up)) < 1.0f)
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

        private void ShowDirections()
        {
            Handles.color = Color.gray;
            Vector3 point = m_splineBase.GetPoint(0f);
            Handles.DrawLine(point, point + m_splineBase.GetDirection(0f) * DirectionScale);
            int steps = StepsPerCurve * m_splineBase.CurveCount;

            for (int i = 1; i <= steps; i++)
            {
                point = m_splineBase.GetPoint(i / (float)steps);
                Handles.DrawLine(point, point + m_splineBase.GetDirection(i / (float)steps) * DirectionScale);
            }
        }

        private void ShowPoint(int index, Vector3 point)
        {
            
            if (!CanShowPoint(index))
            {
                return;
            }

            Handles.color = ModeColors[(int)m_splineBase.GetControlPointMode(index)];
            if (index % 3 == 0)
            {
                Handles.color = Color.green;
            }

            float size = HandleUtility.GetHandleSize(point);

            Handles.DrawCapFunction dcf = Handles.DotCap;
          
            if (index == 0)
            {
                size *= 2f;
            }

            if (Handles.Button(point, m_handleRotation, size * HandleSize, size * PickSize, dcf))
            {
                m_selectedIndex = index;

                SplineControlPoint controlPoint = m_splineBase.GetComponentsInChildren<SplineControlPoint>(true).Where(cpt => cpt.Index == index).FirstOrDefault();
                if (controlPoint != null)
                {
                    Selection.activeGameObject = controlPoint.gameObject;
                }

                
                    
                Repaint();
            }

            if (m_selectedIndex == index)
            {
                int curveIndex = (m_selectedIndex - 1) / 3;
                int prevCurveIndex = curveIndex - 1;
                int nextCurveIndex = curveIndex + 1;
                if(m_splineBase.Loop)
                {
                    if(prevCurveIndex < 0)
                    {
                        prevCurveIndex = m_splineBase.CurveCount - 1;
                    }

                    if(nextCurveIndex > m_splineBase.CurveCount - 1)
                    {
                        nextCurveIndex = 0;
                    }
                }

                
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.green;

                if(prevCurveIndex >= 0)
                {
                    float prevLen = m_splineBase.EvalLength(prevCurveIndex);
                    float prevCur = m_splineBase.EvalCurveLength(prevCurveIndex, GetStepsPerCurve());
                    Handles.Label(m_splineBase.GetPoint(0.5f, prevCurveIndex), string.Format("Len: {0:0.00} m, Cur: {1:0.00} m", prevLen, prevCur), style);
                }
                if(nextCurveIndex < m_splineBase.CurveCount )
                {
                    float nextLen = m_splineBase.EvalLength(nextCurveIndex);
                    float nextCur = m_splineBase.EvalCurveLength(nextCurveIndex, GetStepsPerCurve());
                    Handles.Label(m_splineBase.GetPoint(0.5f, nextCurveIndex), string.Format("Len: {0:0.00} m, Cur: {1:0.00} m", nextLen, nextCur), style);
                }

                float cur = m_splineBase.EvalCurveLength(curveIndex, GetStepsPerCurve());
                float len = m_splineBase.EvalLength(curveIndex);
                Handles.Label(m_splineBase.GetPoint(0.5f, curveIndex), string.Format("Len: {0:0.00} m, Cur: {1:0.00} m",  len, cur), style);

                EditorGUI.BeginChangeCheck();
                point = Handles.DoPositionHandle(point, m_handleRotation);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(m_splineBase, UNDO_MOVEPOINT);
                    EditorUtility.SetDirty(m_splineBase);
                    m_splineBase.SetControlPointLocal(index, m_handleTransform.InverseTransformPoint(point));
                }
            }
        }

        

        protected virtual bool CanShowPoint(int index)
        {
            return true;
        }

        private void DrawSelectedPointInspector()
        {
            if (DrawSelectedPointInspectorOverride())
            {
                EditorGUI.BeginChangeCheck();
                ControlPointMode mode = (ControlPointMode)
                EditorGUILayout.EnumPopup("Mode", m_splineBase.GetControlPointMode(m_selectedIndex));
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(m_splineBase, UNDO_CHANGEMODE);
                    EditorUtility.SetDirty(m_splineBase);
                    m_splineBase.SetControlPointMode(m_selectedIndex, mode);
                }

                EditorGUI.BeginChangeCheck();

                int index = (m_selectedIndex / 3) * 3;
                Twist twist = m_splineBase.GetTwist(index);
                EditorGUI.BeginChangeCheck();
                float twistAngle = EditorGUILayout.FloatField("Twist Angle", twist.Data);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(m_splineBase, "Battlehub.MeshDeformer2 Twist Angle");
                    EditorUtility.SetDirty(m_splineBase);
                    twist.Data = twistAngle;
                    m_splineBase.SetTwist(index, twist);
                }


                if (m_splineBase.Loop || m_selectedIndex / 3 < m_splineBase.CurveCount)
                {
                    float t1 = twist.T1;
                    float t2 = twist.T2;
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.MinMaxSlider(new GUIContent("Twist Offset"), ref t1, ref t2, 0.0f, 1.0f);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(m_splineBase, "Battlehub.MeshDeformer2 Twist Offset");
                        EditorUtility.SetDirty(m_splineBase);
                        twist.T1 = t1;
                        twist.T2 = t2;
                        m_splineBase.SetTwist(index, twist);
                    }
                }

                
                Thickness thickness = m_splineBase.GetThickness(index);
                EditorGUI.BeginChangeCheck();
                Vector3 thicknessValue = EditorGUILayout.Vector3Field("Thickness", thickness.Data);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(m_splineBase, "Battlehub.MeshDeformer2 Thickness");
                    EditorUtility.SetDirty(m_splineBase);
                    thickness.Data = thicknessValue;
                    m_splineBase.SetThickness(index, thickness);
                }

                if (m_splineBase.Loop || m_selectedIndex / 3 < m_splineBase.CurveCount)
                {
                    float t1 = thickness.T1;
                    float t2 = thickness.T2;
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.MinMaxSlider(new GUIContent("Thickness Offset"), ref t1, ref t2, 0.0f, 1.0f);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(m_splineBase, "Battlehub.MeshDeformer2 Thickness Offset");
                        EditorUtility.SetDirty(m_splineBase);
                        thickness.T1 = t1;
                        thickness.T2 = t2;
                        m_splineBase.SetThickness(index, thickness);
                    }
                }
            }
            else
            {
                EditorGUI.BeginChangeCheck();

                int index = (m_selectedIndex / 3) * 3;
                Twist twist = m_splineBase.GetTwist(index);
                EditorGUI.BeginChangeCheck();
                float twistAngle = EditorGUILayout.FloatField("Twist Angle", twist.Data);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(m_splineBase, "Battlehub.MeshDeformer2 Twist Angle");
                    EditorUtility.SetDirty(m_splineBase);
                    twist.Data = twistAngle;
                    m_splineBase.SetTwist(index, twist);
                }

                Thickness thickness = m_splineBase.GetThickness(index);
                EditorGUI.BeginChangeCheck();
                Vector3 thicknessValue = EditorGUILayout.Vector3Field("Thickness", thickness.Data);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(m_splineBase, "Battlehub.MeshDeformer2 Thickness");
                    EditorUtility.SetDirty(m_splineBase);
                    thickness.Data = thicknessValue;
                    m_splineBase.SetThickness(index, thickness);
                }
            }
        }

        protected virtual bool DrawSelectedPointInspectorOverride()
        {
            return true;
        }

        protected virtual SplineBase GetTarget()
        {
            return (SplineBase)target;
        }

        protected virtual SerializedObject GetSerializedObject()
        {
            return serializedObject;
        }

 
    }
}

