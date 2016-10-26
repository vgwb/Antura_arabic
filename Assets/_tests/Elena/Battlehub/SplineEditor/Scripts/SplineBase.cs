using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using Battlehub.RTHandles;
namespace Battlehub.SplineEditor
{
    public enum ControlPointMode
    {
        Free,
        Aligned,
        Mirrored
    }

    [Serializable]
    public struct Vector3Serialziable
    {
        public float X;
        public float Y;
        public float Z;

        public Vector3Serialziable(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static implicit operator Vector3 (Vector3Serialziable v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }
        
        public static implicit operator Vector3Serialziable(Vector3 v)
        {
            return new Vector3Serialziable(v.x, v.y, v.z);
        }
    }

    [Serializable]
    public class Vector3SerialziableArray : List<Vector3Serialziable>
    {
        public static implicit operator Vector3[](Vector3SerialziableArray v)
        {
            Vector3[] result = new Vector3[v.Count];
            for(int i = 0; i < v.Count; ++i)
            {
                result[i] = v[i];
            }
            return result;
        }

        public static implicit operator Vector3SerialziableArray(Vector3[] v)
        {
            Vector3SerialziableArray result = new Vector3SerialziableArray();
            for (int i = 0; i < v.Length; ++i)
            {
                result.Add(v[i]);
            }
            return result;
        }
    }

    [Serializable]
    public struct SplineSnapshot
    {
        [SerializeField]
        private Vector3SerialziableArray m_points;

        [SerializeField]
        private ControlPointSetting[] m_controlPointSettings;

        [SerializeField]
        private ControlPointMode[] m_modes;

        [SerializeField]
        private bool m_loop;

        public int CurveCount
        {
            get { return (m_points.Count - 1) / 3; }
        }

        public Vector3SerialziableArray Points
        {
            get { return m_points; }
        }

        public ControlPointSetting[] ControlPointSettings
        {
            get { return m_controlPointSettings; }
        }

        public ControlPointMode[] Modes
        {
            get { return m_modes; }
        }

        public bool Loop
        {
            get { return m_loop; }
        }

        public SplineSnapshot(Vector3[] points, ControlPointSetting[] settings, ControlPointMode[] modes, bool loop)
        {
            int modeLength = (points.Length - 1) / 3;
            int settingLength = (points.Length - 1) / 2;
            int pointsLength = modeLength * 3 + 1;
            modeLength += 1;
            if (modeLength < 1)
            {
                throw new ArgumentException("too few points. at least 4 required");
            }

            m_points = points;
            if (pointsLength != m_points.Count)
            {
                Array.Resize(ref points, pointsLength);
            }

            m_controlPointSettings = settings;
            if (settingLength != m_controlPointSettings.Length)
            {
                Array.Resize(ref settings, settingLength);
            }

            m_modes = modes;
            if (modeLength != m_modes.Length)
            {
                Array.Resize(ref m_modes, modeLength);
            }

            m_loop = loop;
        }
    }

    public delegate void ControlPointChanged(int pointIndex);

    [Serializable]
    public struct Twist
    {
        public readonly static Twist Null = new Twist();

        public float Data;
        public float T1;
        public float T2;

        public Twist(float data, float t1, float t2)
        {
            Data = data;
            T1 = t1;
            T2 = t2;
        }
    }

    [Serializable]
    public struct Thickness
    {
        public readonly static Thickness Null = new Thickness();

        public Vector3Serialziable Data;
        public float T1;
        public float T2;

        public Thickness(Vector3 data, float t1, float t2)
        {
            Data = data;
            T1 = t1;
            T2 = t2;
        }
    }

    [Serializable]
    public struct ControlPointSetting
    {
        public Twist Twist;
        public Thickness Thickness;

        public ControlPointSetting(Twist twist, Thickness thickness)
        {
            Twist = twist;
            Thickness = thickness;
        }
    }

    [ExecuteInEditMode]
    public class SplineBase : MonoBehaviour, IGL
    {
        public event ControlPointChanged ControlPointPositionChanged;
        public event ControlPointChanged ControlPointModeChanged;

        private void RaiseControlPointChanged(int index)
        {
            if (ControlPointPositionChanged != null)
            {
                ControlPointPositionChanged(index);
            }
        }

        private void RaiseControlPointModeChanged(int modeIndex)
        {
            if (ControlPointModeChanged != null)
            {
                int pointIndex = modeIndex * 3 - 1;

                ControlPointModeChanged(pointIndex);
                ControlPointModeChanged(pointIndex + 1);
                ControlPointModeChanged(pointIndex + 2);
            }
        }

        [SerializeField, HideInInspector]
        private ControlPointMode[] m_modes;

        [SerializeField, HideInInspector]
        private Vector3[] m_points;

        [SerializeField, HideInInspector]
        private ControlPointSetting[] m_settings;

        [SerializeField, HideInInspector]
        protected bool m_loop;

        [SerializeField, HideInInspector]
        private bool m_isSelected;
        public bool IsSelected
        {
            get { return m_isSelected; }
        }

        public virtual bool Loop
        {
            get
            {
                return m_loop;
            }
            set
            {
                m_loop = value;
                if (value)
                {
                    m_settings[0] = m_settings[m_settings.Length - 1];
                    m_modes[m_modes.Length - 1] = m_modes[0];
                    RaiseControlPointModeChanged(m_modes.Length - 1);
                    SetControlPointLocal(m_points.Length - 1, m_points[0]);
                }
            }
        }

        public int CurveCount
        {
            get
            {
                return (m_points.Length - 1) / 3;
            }
        }

        public int ControlPointCount
        {
            get
            {
                return m_points.Length;
            }
        }


#if UNITY_EDITOR
        [SerializeField, HideInInspector]
        private int[] m_persistentVersions = new int[2];

        private int[] m_currentVersions = new int[2];

        protected void OnVersionChanged()
        {
            SyncVersions();
        }

        private void SyncVersions()
        {
            Array.Copy(m_persistentVersions, m_currentVersions, m_persistentVersions.Length);
        }

        protected int[] PersistentVersions
        {
            get { return m_persistentVersions; }
            set { m_persistentVersions = value; }
        }

        private int[] ChangedVersions()
        {
            List<int> changed = new List<int>();
            for (int i = 0; i < m_currentVersions.Length; ++i)
            {
                if (m_currentVersions[i] != m_persistentVersions[i])
                {
                    changed.Add(i);
                }
            }

            if (changed.Count > 0)
            {
                return changed.ToArray();
            }

            return null;
        }

        private void OnUndoRedoPerformed()
        {
            int[] changed = ChangedVersions();
            if (changed != null)
            {

                SyncVersions();
                SyncCtrlPoints(true);
                OnSplineUndoRedo(changed);
            }
        }

        protected virtual void OnSplineUndoRedo(int[] changed)
        {
        }

        private void Awake()
        {
            SplineRuntimeEditor.Created += OnRuntimeEditorCreated;
            SplineRuntimeEditor.Destroyed += OnRuntimeEditorDestroyed;

            SyncArrays();
            SyncVersions();
            Undo.undoRedoPerformed += OnUndoRedoPerformed;

            if(m_points == null)
            {
                Reset();
            }

            AwakeOverride();
        }

        private void Start()
        {
            StartOverride();
            if (m_isSelected && Selection.activeObject != gameObject)
            {
                Unselect();
            }
        }

        private void OnDestroy()
        {
            SplineRuntimeEditor.Created -= OnRuntimeEditorCreated;
            SplineRuntimeEditor.Destroyed -= OnRuntimeEditorDestroyed;
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
            if(SplineRuntimeEditor.Instance != null && SplineRuntimeEditor.Instance.SelectedSpline == this)
            {
                SplineRuntimeEditor.Instance.SelectedSpline = null;
            }
        }
#else
        
        private void Awake()
        {
            SplineRuntimeEditor.Created += OnRuntimeEditorCreated;
            SplineRuntimeEditor.Destroyed += OnRuntimeEditorDestroyed;

            SyncArrays();
            AwakeOverride();
        }

        private void OnDestroy()
        {
            SplineRuntimeEditor.Created -= OnRuntimeEditorCreated;
            SplineRuntimeEditor.Destroyed -= OnRuntimeEditorDestroyed;
            OnDestroyOverride();
            if(SplineRuntimeEditor.Instance != null && SplineRuntimeEditor.Instance.SelectedSpline == this)
            {
                SplineRuntimeEditor.Instance.SelectedSpline = null;
            }
        }

        private void Start()
        {
            StartOverride();
            if (m_isSelected)
            {
                Unselect();
            }
        }
#endif
        private void OnRuntimeEditorCreated(object sender, EventArgs e)
        {
            if (m_isSelected)
            {
                if (GLRenderer.Instance != null)
                {
                    GLRenderer.Instance.Add(this);
                }
            }
        }

        private void OnRuntimeEditorDestroyed(object sender, EventArgs e)
        {
            if (GLRenderer.Instance != null)
            {
                GLRenderer.Instance.Remove(this);
            }
        }

       
        private void OnEnable()
        {   
            if(m_isSelected)
            {
                if (GLRenderer.Instance != null)
                {
                    GLRenderer.Instance.Add(this);
                }
            }

            OnEnableOverride();
        }

        private void OnDisable()
        {
            if (GLRenderer.Instance != null)
            {
                GLRenderer.Instance.Remove(this);
            }

            OnDisableOverride();
        }

        protected virtual void OnEnableOverride()
        {

        }

        protected virtual void OnDisableOverride()
        {

        }

        protected virtual void AwakeOverride()
        {

        }


        protected virtual void StartOverride()
        {

        }

        protected virtual void OnDestroyOverride()
        {

        }
     

        private void SyncArrays()
        {
            if (m_points != null && m_points.Length > 0)
            {
                int modesLength = m_points.Length / 3 + 1;
                if (m_modes.Length != modesLength)
                {
                    Debug.Log("Synchronize modes");
                    Array.Resize(ref m_modes, modesLength);
                }

                if (m_settings == null)
                {
                    m_settings = new ControlPointSetting[0];
                }
                if (m_settings.Length != modesLength)
                {
                    Debug.Log("Synchronize settings");
                    int oldLength = m_settings.Length;
                    Array.Resize(ref m_settings, modesLength);
                    for (int i = oldLength; i < m_settings.Length; ++i)
                    {
                        m_settings[i].Thickness = new Thickness(Vector3.one, 0.0f, 1.0f);
                        m_settings[i].Twist = new Twist(0.0f, 0.0f, 1.0f);
                    }
                }
            }
        }

        public void Select()
        {
            if (GLRenderer.Instance != null)
            {
                GLRenderer.Instance.Add(this);
            }

            SplineControlPoint[] ctrlPoints = GetControlPoints();
            for(int i = 0; i < ctrlPoints.Length; ++i)
            {
                SplineControlPoint ctrlPoint = ctrlPoints[i];
                ctrlPoint.gameObject.SetActive(true);
            }

            m_isSelected = true;

            if (SplineRuntimeEditor.Instance != null)
            {
                if(SplineRuntimeEditor.Instance.SelectedSpline != this)
                {
                    SplineRuntimeEditor.Instance.SelectedSpline = this;
                }        
            }
        }

        public void Unselect()
        {
            if (GLRenderer.Instance != null)
            {
                GLRenderer.Instance.Remove(this);
            }

            SplineControlPoint[] ctrlPoints = GetControlPoints();
            for (int i = 0; i < ctrlPoints.Length; ++i)
            {
                SplineControlPoint ctrlPoint = ctrlPoints[i];
                if(ctrlPoint)
                {
                    ctrlPoint.gameObject.SetActive(false);
                }
            }

            m_isSelected = false;

            if (SplineRuntimeEditor.Instance != null)
            {
                if(SplineRuntimeEditor.Instance.SelectedSpline == this)
                {
                    SplineRuntimeEditor.Instance.SelectedSpline = null;
                }
            }
        }

        public float EvalLength(int curveIndex)
        {
            Vector3 prev = GetPoint(0.0f, curveIndex);
            Vector3 next = GetPoint(1.0f, curveIndex);

            return (next - prev).magnitude;
        }

        public float EvalCurveLength(int curveIndex, int steps)
        {
            float length = 0.0f;
            Vector3 prevPoint = GetPoint(0.0f, curveIndex);
            for (int i = 1; i < 4; ++i)
            {
                float t = i;
                t /= 3;
                Vector3 point = GetPoint(t, curveIndex);
                length += (point - prevPoint).magnitude;
                prevPoint = point;
            }

            return length;
        }

        public Vector3 GetPoint(float t, int curveIndex)
        {
            curveIndex *= 3;
            return transform.TransformPoint(CurveUtils.GetPoint(m_points[curveIndex], m_points[curveIndex + 1], m_points[curveIndex + 2], m_points[curveIndex + 3], t));
        }

        public Vector3 GetPointLocal(float t, int curveIndex)
        {
            curveIndex *= 3;
            return CurveUtils.GetPoint(m_points[curveIndex], m_points[curveIndex + 1], m_points[curveIndex + 2], m_points[curveIndex + 3], t);
        }

        public Vector3 GetPoint(float t)
        {
            int i;
            if (t >= 1f)
            {
                t = 1f;
                i = (m_points.Length - 1) / 3 - 1;
            }
            else
            {
                t = Mathf.Clamp01(t) * CurveCount;
                i = (int)t;
                t -= i;
            }
            return GetPoint(t, i);
        }


        public Vector3 GetPointLocal(float t)
        {
            int i;
            if (t >= 1f)
            {
                t = 1f;
                i = (m_points.Length - 1) / 3 - 1;
            }
            else
            {
                t = Mathf.Clamp01(t) * CurveCount;
                i = (int)t;
                t -= i;
            }
            return GetPointLocal(t, i);
        }


        public float GetTwist(float t, int curveIndex)
        {
            Twist current = m_settings[curveIndex].Twist;
            Twist next = m_settings[curveIndex + 1].Twist;

            float t1 = Mathf.Clamp01(current.T1);
            float t2 = Mathf.Clamp01(current.T2);

            if (t <= t1)
            {
                t = 0.0f;
            }
            else if (t >= t2)
            {
                t = 1.0f;
            }
            else
            {
                t = Mathf.Clamp01((t - t1) / (t2 - t1));
            }

            return Mathf.Lerp(current.Data, next.Data, t);
        }

        public float GetTwist(float t)
        {
            int i;
            if (t >= 1f)
            {
                t = 1f;
                i = (m_points.Length - 1) / 3 - 1;
            }
            else
            {
                t = Mathf.Clamp01(t) * CurveCount;
                i = (int)t;
                t -= i;
            }
            return GetTwist(t, i);
        }


        public Vector3 GetThickness(float t, int curveIndex)
        {
            Thickness current = m_settings[curveIndex].Thickness;
            Thickness next = m_settings[curveIndex + 1].Thickness;

            float t1 = Mathf.Clamp01(current.T1);
            float t2 = Mathf.Clamp01(current.T2);

            if (t <= t1)
            {
                t = 0.0f;
            }
            else if (t >= t2)
            {
                t = 1.0f;
            }
            else
            {
                t = Mathf.Clamp01((t - t1) / (t2 - t1));
            }

            return Vector3.Lerp(current.Data, next.Data, t);
        }

        public Vector3 GetThickness(float t)
        {
            int i;
            if (t >= 1f)
            {
                t = 1f;
                i = (m_points.Length - 1) / 3 - 1;
            }
            else
            {
                t = Mathf.Clamp01(t) * CurveCount;
                i = (int)t;
                t -= i;
            }
            return GetThickness(t, i);
        }

        public Vector3 GetControlPoint(int index)
        {
            return transform.TransformPoint(m_points[index]);
        }

        public Vector3 GetControlPointLocal(int index)
        {
            return m_points[index];
        }

        public ControlPointSetting GetSetting(int index)
        {
            return m_settings[(index + 1) / 3];
        }

        public Twist GetTwist(int index)
        {
            return m_settings[(index + 1) / 3].Twist;
        }

        public Thickness GetThickness(int index)
        {
            return m_settings[(index + 1) / 3].Thickness;
        }

        public void SetTwist(int index, Twist twist)
        {
            int settingIndex = (index + 1) / 3;
            m_settings[settingIndex].Twist = twist;

            if (m_loop)
            {
                if (settingIndex == m_settings.Length - 1)
                {
                    m_settings[0] = m_settings[m_settings.Length - 1];
                }
                else if (settingIndex == 0)
                {
                    m_settings[m_settings.Length - 1] = m_settings[0];
                }
            }

#if UNITY_EDITOR
            m_persistentVersions[0]++;
            OnVersionChanged();
#endif
            OnCurveChanged(index, Math.Max(0, (index - 1) / 3));
        }

        public void SetThickness(int index, Thickness thickness)
        {
            int settingIndex = (index + 1) / 3;
            m_settings[settingIndex].Thickness = thickness;

            if (m_loop)
            {
                if (settingIndex == m_settings.Length - 1)
                {
                    m_settings[0] = m_settings[m_settings.Length - 1];
                }
                else if (settingIndex == 0)
                {
                    m_settings[m_settings.Length - 1] = m_settings[0];
                }
            }

#if UNITY_EDITOR
            m_persistentVersions[0]++;
            OnVersionChanged();
#endif
            OnCurveChanged(index, Math.Max(0, (index - 1) / 3));
        }


        protected void SetPoints(int curveIndex, Vector3[] points, ControlPointMode mode, bool raiseCurveChanged)
        {
            int index = curveIndex * 3;

            for (int i = 0; i < points.Length; ++i)
            {
                m_points[index] = points[i];
                RaiseControlPointChanged(index);

                SetControlPointMode(index, mode, raiseCurveChanged);
                index++;
            }

            EnforceMode(index);
#if UNITY_EDITOR
            m_persistentVersions[0]++;
            OnVersionChanged();
#endif

            if (raiseCurveChanged)
            {
                OnCurveChanged(index, Math.Max(0, (index - 1) / 3));
            }
        }

        public void SetControlPoint(int index, Vector3 point)
        {
            SetControlPointLocal(index, transform.InverseTransformPoint(point));
        }

        public void SetControlPointLocal(int index, Vector3 point)
        {
            if (index % 3 == 0)
            {
                Vector3 delta = point - m_points[index];
                if (m_loop)
                {
                    if (index == 0)
                    {
                        m_points[1] += delta;
                        RaiseControlPointChanged(1);

                        m_points[m_points.Length - 2] += delta;
                        RaiseControlPointChanged(m_points.Length - 2);

                        m_points[m_points.Length - 1] = point;
                        RaiseControlPointChanged(m_points.Length - 1);
                    }
                    else if (index == m_points.Length - 1)
                    {
                        m_points[0] = point;
                        RaiseControlPointChanged(0);

                        m_points[1] += delta;
                        RaiseControlPointChanged(1);

                        m_points[index - 1] += delta;
                        RaiseControlPointChanged(index - 1);
                    }
                    else
                    {
                        m_points[index - 1] += delta;
                        RaiseControlPointChanged(index - 1);

                        m_points[index + 1] += delta;
                        RaiseControlPointChanged(index + 1);
                    }
                }
                else
                {
                    if (index > 0)
                    {
                        m_points[index - 1] += delta;
                        RaiseControlPointChanged(index - 1);
                    }

                    if (index + 1 < m_points.Length)
                    {
                        m_points[index + 1] += delta;
                        RaiseControlPointChanged(index + 1);

                    }
                }
            }

            m_points[index] = point;

            RaiseControlPointChanged(index);
          
            EnforceMode(index);
#if UNITY_EDITOR
            m_persistentVersions[0]++;
            OnVersionChanged();
#endif
            OnCurveChanged(index, Math.Max(0, (index - 1) / 3));
        }

        public ControlPointMode GetControlPointMode(int index)
        {
            return m_modes[(index + 1) / 3];
        }

        public void SetControlPointMode(ControlPointMode mode)
        {
            for (int i = 0; i <= CurveCount; ++i)
            {
                SetControlPointMode(i * 3, mode);
            }
        }

        public void SetControlPointMode(int index, ControlPointMode mode, bool raiseCurveChanged = true)
        {
            int modeIndex = (index + 1) / 3;
            m_modes[modeIndex] = mode;
            RaiseControlPointModeChanged(modeIndex);
            if (m_loop)
            {
                if (modeIndex == 0)
                {
                    m_modes[m_modes.Length - 1] = mode;
                    RaiseControlPointModeChanged(m_modes.Length - 1);
                }
                else if (modeIndex == m_modes.Length - 1)
                {
                    m_modes[0] = mode;
                    RaiseControlPointModeChanged(0);
                }
            }
            EnforceMode(index);
#if UNITY_EDITOR
            m_persistentVersions[0]++;
            OnVersionChanged();
#endif

            if (raiseCurveChanged)
            {
                OnCurveChanged(index, Math.Max(0, (index - 1) / 3));
            }
        }

        private void EnforceMode(int index)
        {
            int modeIndex = (index + 1) / 3;
            ControlPointMode mode = m_modes[modeIndex];
            if (mode == ControlPointMode.Free || !m_loop && (modeIndex == 0 || modeIndex == m_modes.Length - 1))
            {
                return;
            }

            int middleIndex = modeIndex * 3;
            int fixedIndex, enforcedIndex;
            if (index <= middleIndex)
            {
                fixedIndex = middleIndex - 1;
                if (fixedIndex < 0)
                {
                    fixedIndex = m_points.Length - 2;
                }
                enforcedIndex = middleIndex + 1;
                if (enforcedIndex >= m_points.Length)
                {
                    enforcedIndex = 1;
                }
            }
            else
            {
                fixedIndex = middleIndex + 1;
                if (fixedIndex >= m_points.Length)
                {
                    fixedIndex = 1;
                }
                enforcedIndex = middleIndex - 1;
                if (enforcedIndex < 0)
                {
                    enforcedIndex = m_points.Length - 2;
                }
            }

            Vector3 middle = m_points[middleIndex];
            Vector3 enforcedTangent = middle - m_points[fixedIndex];
            if (mode == ControlPointMode.Aligned)
            {
                enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, m_points[enforcedIndex]);
            }
            m_points[enforcedIndex] = middle + enforcedTangent;
            RaiseControlPointChanged(enforcedIndex);
        }

        public Vector3 GetVelocity(float t, int curveIndex)
        {
            curveIndex *= 3;
            return transform.TransformPoint(
                CurveUtils.GetFirstDerivative(m_points[curveIndex], m_points[curveIndex + 1], m_points[curveIndex + 2], m_points[curveIndex + 3], t)) - transform.position;
        }

        public Vector3 GetVelocity(float t)
        {
            int i;
            if (t >= 1f)
            {
                t = 1f;
                i = (m_points.Length - 1) / 3 - 1;
            }
            else
            {
                t = Mathf.Clamp01(t) * CurveCount;
                i = (int)t;
                t -= i;
            }
            return GetVelocity(t, i);
        }


        public Vector3 GetDirection(float t, int curveIndex)
        {
            return GetVelocity(t, curveIndex).normalized;
        }

        public Vector3 GetDirection(float t)
        {
            return GetVelocity(t).normalized;
        }

        public void Smooth()
        {
            int n = m_points.Length / 3;

            float[] a = new float[n];
            float[] b = new float[n];
            float[] c = new float[n];
            Vector3[] r = new Vector3[n];

            a[0] = 0.0f;
            b[0] = 2.0f;
            c[0] = 1.0f;
            r[0] = m_points[0] + 2 * m_points[3];

            for (int i = 1; i < n - 1; i++)
            {
                a[i] = 1.0f;
                b[i] = 4.0f;
                c[i] = 1.0f;
                r[i] = 4 * m_points[i * 3] + 2 * m_points[(i + 1) * 3];
            }

            a[n - 1] = 2.0f;
            b[n - 1] = 7.0f;
            c[n - 1] = 0.0f;
            r[n - 1] = 8 * m_points[(n - 1) * 3] + m_points[n * 3];

            for (int i = 1; i < n; i++)
            {
                float m = a[i] / b[i - 1];
                b[i] = b[i] - m * c[i - 1];
                r[i] = r[i] - m * r[i - 1];
            }

            m_points[(n - 1) * 3 + 1] = r[n - 1] / b[n - 1];
            for (int i = n - 2; i >= 0; --i)
            {
                m_points[i * 3 + 1] = (r[i] - c[i] * m_points[(i + 1) * 3 + 1]) / b[i];
            }


            for (int i = 0; i < n - 1; i++)
            {
                m_points[i * 3 + 2] = 2.0f * m_points[(i + 1) * 3] - m_points[(i + 1) * 3 + 1];
            }

            m_points[(n - 1) * 3 + 2] = 0.5f * (m_points[n * 3] + m_points[(n - 1) * 3 + 1]);

            if (Loop)
            {
                EnforceMode(m_points.Length - 2);
            }

            SyncCtrlPoints();
            OnCurveChanged();
        }

        protected void LoadSpline(SplineSnapshot settings)
        {
            m_points = settings.Points;
            m_settings = settings.ControlPointSettings;
            m_modes = settings.Modes;
            m_loop = settings.Loop;
            SyncCtrlPoints();
        }

        public virtual SplineSnapshot Save()
        {
            return new SplineSnapshot(m_points, m_settings, m_modes, m_loop);
        }

        public virtual void Load(SplineSnapshot snapshot)
        {
            LoadSpline(snapshot);
        }

        protected void AlignCurve(int curveIndex, float length, bool toLast = true)
        {
            int firstPointIndex = curveIndex * 3;
            int lastPointIndex = firstPointIndex + 3;
            Vector3 lastPoint = m_points[lastPointIndex];
            Vector3 firstPoint = m_points[firstPointIndex];

            Vector3 dir;
            if (toLast)
            {
                dir = transform.InverseTransformDirection(GetDirection(1.0f, curveIndex));
            }
            else
            {
                dir = transform.InverseTransformDirection(GetDirection(0.0f, curveIndex));
            }

            if (toLast)
            {
                for (int i = lastPointIndex - 1; i >= firstPointIndex; --i)
                {
                    lastPoint -= dir * length / 3;
                    m_points[i] = lastPoint;
                    RaiseControlPointChanged(i);
                }

                Vector3 offset = (firstPoint - m_points[firstPointIndex]);
                for (int i = firstPointIndex - 1; i >= 0; i--)
                {
                    m_points[i] -= offset;
                    RaiseControlPointChanged(i);
                }
            }
            else
            {
                for (int i = firstPointIndex + 1; i <= lastPointIndex; ++i)
                {
                    firstPoint += dir * length / 3;
                    m_points[i] = firstPoint;
                    RaiseControlPointChanged(i);
                }

                Vector3 offset = (lastPoint - m_points[lastPointIndex]);
                for (int i = lastPointIndex + 1; i < m_points.Length; i++)
                {
                    m_points[i] -= offset;
                    RaiseControlPointChanged(i);
                }
            }

            EnforceMode(firstPointIndex - 1);
            EnforceMode(lastPointIndex + 1);
        }


        protected bool RemoveCurve(int curveIndex)
        {
            if (m_points.Length <= 4)
            {
                return false;
            }

            if (curveIndex >= CurveCount || curveIndex < 0)
            {
                throw new ArgumentOutOfRangeException("curveIndex");
            }

            int pointIndex = curveIndex * 3;
            bool enforceMode = true;
            if (curveIndex == CurveCount - 1)
            {
                enforceMode = false;
                pointIndex++;
            }

            for (int i = pointIndex; i < m_points.Length - 3; ++i)
            {
                m_points[i] = m_points[i + 3];
            }

            int modeIndex = (pointIndex + 1) / 3;
            for (int i = modeIndex; i < m_modes.Length - 1; ++i)
            {
                m_settings[i] = m_settings[i + 1];
                m_modes[i] = m_modes[i + 1];
                RaiseControlPointModeChanged(i);
            }

            Array.Resize(ref m_points, m_points.Length - 3);
            Array.Resize(ref m_settings, m_settings.Length - 1);
            Array.Resize(ref m_modes, m_modes.Length - 1);

            if (enforceMode)
            {
                EnforceMode(pointIndex + 1);
            }

            if (m_loop)
            {
                m_points[m_points.Length - 1] = m_points[0];
                m_settings[0] = m_settings[m_settings.Length - 1];
                m_modes[m_modes.Length - 1] = m_modes[0];
                RaiseControlPointModeChanged(m_modes.Length - 1);
                EnforceMode(1);
            }

            SyncCtrlPoints();
            return true;
        }

        protected void Subdivide(int firstCurveIndex, int lastCurveIndex, int curveCount)
        {
            int firstPointIndex = firstCurveIndex * 3;
            int lastPointIndex = lastCurveIndex * 3 + 3;
            int pointsLength = m_points.Length;

            int midPointsCount = lastPointIndex - firstPointIndex - 1;
            int newMidPointsCount = curveCount * 3 - 1;
            int deltaPoints = newMidPointsCount - midPointsCount;

            Vector3[] midPoints = new Vector3[newMidPointsCount];
            Vector3 firstPoint = m_points[firstPointIndex];
            Vector3 lastPoint = m_points[lastPointIndex];

            ControlPointSetting midPointSetting = m_settings[(firstPointIndex + 1) / 3];
            ControlPointMode midMode = m_modes[(firstPointIndex + 1) / 3];

            float deltaT = 1.0f / (newMidPointsCount + 1);
            float t = 0.0f;

            for (int i = 0; i < newMidPointsCount; ++i)
            {
                t += deltaT;
                midPoints[i] = Vector3.Lerp(firstPoint, lastPoint, t);
            }

            if (deltaPoints > 0)
            {
                Array.Resize(ref m_points, m_points.Length + deltaPoints);
                Array.Resize(ref m_settings, m_settings.Length + deltaPoints / 3);
                Array.Resize(ref m_modes, m_modes.Length + deltaPoints / 3);

                for (int i = pointsLength - 1; i >= lastPointIndex; i--)
                {
                    m_points[i + deltaPoints] = m_points[i];
                }

                for (int i = pointsLength / 3; i >= (lastPointIndex + 1) / 3; i--)
                {
                    m_settings[i + deltaPoints / 3] = m_settings[i];
                    m_modes[i + deltaPoints / 3] = m_modes[i];
                    RaiseControlPointModeChanged(i + deltaPoints / 3);
                }
            }
            else if (deltaPoints < 0)
            {
                for (int i = lastPointIndex; i < pointsLength; i++)
                {
                    m_points[i + deltaPoints] = m_points[i];
                }

                for (int i = (lastPointIndex + 1) / 3; i < (pointsLength + 1) / 3; i++)
                {
                    m_settings[i + deltaPoints / 3] = m_settings[i];
                    m_modes[i + deltaPoints / 3] = m_modes[i];
                    RaiseControlPointModeChanged(i + deltaPoints / 3);
                }

                Array.Resize(ref m_points, m_points.Length + deltaPoints);
                Array.Resize(ref m_settings, m_settings.Length + deltaPoints / 3);
                Array.Resize(ref m_modes, m_modes.Length + deltaPoints / 3);
            }

            for (int i = 0; i < newMidPointsCount; ++i)
            {
                m_points[firstPointIndex + i + 1] = midPoints[i];
            }
            for (int i = 0; i < newMidPointsCount / 3; ++i)
            {
                m_settings[(firstPointIndex + 1) / 3 + i + 1] = midPointSetting;
                m_modes[(firstPointIndex + 1) / 3 + i + 1] = midMode;
                RaiseControlPointModeChanged((firstPointIndex + 1) / 3 + i + 1);
            }

            int prevPointIndex = firstPointIndex - 1;
            int nextPointIndex = firstPointIndex + newMidPointsCount + 2;
            if (m_loop)
            {
                if (prevPointIndex == -1)
                {
                    prevPointIndex = m_points.Length - 1;
                }
                if (nextPointIndex == m_points.Length)
                {
                    nextPointIndex = 0;
                }
            }

            if (nextPointIndex < m_points.Length)
            {
                EnforceMode(nextPointIndex);
            }

            if (prevPointIndex >= 0)
            {
                EnforceMode(prevPointIndex);
            }

            SyncCtrlPoints();
        }

        private void InsertCurve(Vector3[] points, ControlPointSetting setting, ControlPointMode mode, int curveIndex, float length, bool enforceNeighbour)
        {
            Array.Resize(ref m_points, m_points.Length + points.Length);
            Array.Resize(ref m_settings, m_settings.Length + points.Length / 3);
            Array.Resize(ref m_modes, m_modes.Length + points.Length / 3);

            int pointIndex = curveIndex * 3;
            int modeIndex = (pointIndex + 1) / 3;
            for (int i = m_points.Length - 1; i >= pointIndex + points.Length; --i)
            {
                m_points[i] = m_points[i - points.Length];
            }

            for (int i = m_modes.Length - 1; i >= modeIndex + points.Length / 3; --i)
            {
                m_settings[i] = m_settings[i - points.Length / 3];
                m_modes[i] = m_modes[i - points.Length / 3];
                RaiseControlPointModeChanged(i);
            }

            for (int i = pointIndex; i < pointIndex + points.Length; ++i)
            {
                m_points[i] = points[i - pointIndex];
            }

            for (int i = modeIndex; i < modeIndex + points.Length / 3; ++i)
            {
                m_settings[i] = setting;
                m_modes[i] = mode;
                RaiseControlPointModeChanged(i);
            }

            Vector3 dir = transform.InverseTransformDirection(GetDirection(0.0f, curveIndex));
            for (int i = pointIndex - 1; i >= 0; i--)
            {
                m_points[i] -= dir * length;
            }

            if (enforceNeighbour)
            {
                EnforceMode(pointIndex + points.Length + 1);
            }
            else
            {
                EnforceMode(pointIndex + points.Length - 1);
            }

            if (m_loop)
            {
                m_points[m_points.Length - 1] = m_points[0];
                m_settings[0] = m_settings[m_settings.Length - 1];
                m_modes[m_modes.Length - 1] = m_modes[0];
                RaiseControlPointModeChanged(m_modes.Length - 1);
                EnforceMode(1);
            }

            SyncCtrlPoints();
        }

        protected void PrependCurve(Vector3[] points, int curveIndex, float length, bool enforceNeighbour)
        {
            InsertCurve(points, GetSetting(curveIndex * 3), GetControlPointMode(curveIndex * 3), curveIndex, length, enforceNeighbour);
        }


        protected void AppendCurve(Vector3[] points, bool enforceNeighbour)
        {
            AppendCurve(points, GetSetting(m_points.Length - 1), GetControlPointMode(m_points.Length - 1), enforceNeighbour);
        }

        protected void AppendCurve(Vector3[] points, ControlPointSetting setting, ControlPointMode mode, bool enforceNeighbour)
        {
            Array.Resize(ref m_points, m_points.Length + points.Length);
            Array.Resize(ref m_settings, m_settings.Length + points.Length / 3);
            Array.Resize(ref m_modes, m_modes.Length + points.Length / 3);

            for (int i = 0; i < points.Length; i++)
            {
                m_points[m_points.Length - points.Length + i] = points[i];
            }

            for (int i = 0; i < points.Length / 3; i++)
            {
                m_settings[m_settings.Length - points.Length / 3 + i] = setting;
                m_modes[m_modes.Length - points.Length / 3 + i] = mode;
                RaiseControlPointModeChanged(m_modes.Length - points.Length / 3 + i);
            }

            if (enforceNeighbour)
            {
                EnforceMode(m_points.Length - points.Length - 2);
            }
            else
            {
                EnforceMode(m_points.Length - points.Length);
            }

            if (m_loop)
            {
                m_points[0] = m_points[m_points.Length - 1];
                m_settings[0] = m_settings[m_settings.Length - 1];
                m_modes[0] = m_modes[m_modes.Length - 1];
                RaiseControlPointModeChanged(0);
                EnforceMode(m_points.Length - 1);
            }
            SyncCtrlPoints();
        }

        protected virtual float GetMag()
        {
            return 1.0f;
        }

        private void Reset()
        {
            m_points = new Vector3[]
            {
                new Vector3(0f, 0f, 0f),
                new Vector3(1.0f / 3.0f * GetMag(), 0f, 0f),
                new Vector3(2.0f / 3.0f * GetMag(), 0f, 0f),
                new Vector3(1.0f  * GetMag(), 0f, 0f)
            };

            m_settings = new ControlPointSetting[]
            {
                new ControlPointSetting(new Twist(0.0f, 0.0f, 1.0f), new Thickness(Vector3.one, 0.0f, 1.0f)),
                new ControlPointSetting(new Twist(0.0f, 0.0f, 1.0f), new Thickness(Vector3.one, 0.0f, 1.0f)),
            };

            m_modes = new ControlPointMode[]
            {
                ControlPointMode.Free,
                ControlPointMode.Free
            };
            ResetOverride();
            SyncCtrlPoints();
        }

        protected virtual void ResetOverride()
        {

        }

        protected virtual void OnCurveChanged(int pointIndex, int curveIndex)
        {

        }

        protected virtual void OnCurveChanged()
        {

        }

        protected virtual SplineControlPoint[] GetControlPoints()
        {
            return GetComponentsInChildren<SplineControlPoint>(true);
        }

        protected virtual void AddControlPointComponent(GameObject ctrlPoint)
        {
            ctrlPoint.AddComponent<SplineControlPoint>();
        }

        private void SyncCtrlPoints(bool silent = false)
        {
            SplineRuntimeEditor runtimeEditor = SplineRuntimeEditor.Instance;
            SplineControlPoint[] ctrlPoints = GetControlPoints();
            int delta = ControlPointCount - ctrlPoints.Length;
            if (delta > 0)
            {
                for (int i = 0; i < delta; ++i)
                {
                    GameObject ctrlPoint = new GameObject();
                    ctrlPoint.SetActive(m_isSelected);
                    ctrlPoint.transform.parent = transform;
                    ctrlPoint.transform.rotation = Quaternion.identity;
                    ctrlPoint.transform.localScale = Vector3.one;

                    if(runtimeEditor != null)
                    {
                        MeshRenderer renderer = ctrlPoint.AddComponent<MeshRenderer>();
                        MeshFilter filter = ctrlPoint.AddComponent<MeshFilter>();
                        filter.sharedMesh = runtimeEditor.ControlPointMesh;
                        renderer.sharedMaterial = runtimeEditor.NormalMaterial;
                        renderer.enabled = true;// settings.RuntimeEditing;
                    }
                    
                    ctrlPoint.name = "ctrl point";
                    AddControlPointComponent(ctrlPoint);

#if UNITY_EDITOR
                    if (!silent)
                    {
                        Undo.RegisterCreatedObjectUndo(ctrlPoint, Undo.GetCurrentGroupName());
                        EditorUtility.SetDirty(ctrlPoint);
                    }
#endif
                }

                ctrlPoints = GetControlPoints();
            }
            else if (delta < 0)
            {
                delta = -delta;
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    for (int i = 0; i < delta; ++i)
                    {
                        SplineControlPoint ctrlPoint = ctrlPoints[i];
                        if (ctrlPoint.gameObject != null)
                        {
                            DestroyImmediate(ctrlPoint.gameObject);
                        }
                    }
                    ctrlPoints = GetControlPoints();
                }
                else
                {
                    if (silent)
                    {
                        for (int i = 0; i < delta; ++i)
                        {
                            SplineControlPoint ctrlPoint = ctrlPoints[i];
                            if (ctrlPoint.gameObject != null)
                            {
                                DestroyImmediate(ctrlPoint.gameObject);
                            }
                        }
                        ctrlPoints = GetControlPoints();
                    }
                    else
                    {
                        List<SplineControlPoint> ctrlPointsList = new List<SplineControlPoint>(ctrlPoints);
                        m_controlPointsToRemove = new SplineControlPoint[delta];
                        int skip = 0;
                        for (int i = 0; i < delta; ++i)
                        {
                            if (Selection.activeGameObject == ctrlPoints[i].gameObject)
                            {
                                delta++;
                                skip++;
                                continue;
                            }
                            m_controlPointsToRemove[i - skip] = ctrlPoints[i];
                            ctrlPointsList.Remove(ctrlPoints[i]);
                        }
                        if (m_removeCallback == null)
                        {
                            m_removeCallback = new EditorApplication.CallbackFunction(RemoveControlPoints);
                        }
                        EditorApplication.delayCall += m_removeCallback;
                        ctrlPoints = ctrlPointsList.ToArray();
                    }
                }
#else
                for (int i = 0; i < delta; ++i)
                {
                    SplineControlPoint ctrlPoint = ctrlPoints[i];
                    if (ctrlPoint.gameObject != null)
                    {
                        GameObject.DestroyImmediate(ctrlPoint.gameObject);
                    }
                }
                ctrlPoints = GetComponentsInChildren<SplineControlPoint>(true);
#endif

            }

            
            for (int i = 0; i < ControlPointCount; ++i)
            {
                SplineControlPoint ctrlPoint = ctrlPoints[i];
                ctrlPoint.Index = i;
                RaiseControlPointChanged(i);
                RaiseControlPointModeChanged(i);
            }
        }

#if UNITY_EDITOR
        private EditorApplication.CallbackFunction m_removeCallback;
        private SplineControlPoint[] m_controlPointsToRemove;
        private void RemoveControlPoints()
        {
            EditorApplication.delayCall -= m_removeCallback;
            for (int i = 0; i < m_controlPointsToRemove.Length; ++i)
            {
                SplineControlPoint controlPoint = m_controlPointsToRemove[i];
                if (controlPoint != null)
                {
                    Undo.DestroyObjectImmediate(controlPoint.gameObject);
                }
            }
        }
#endif

        public void Draw()
        {
            if (m_points.Length < 2)
            {
                return;
            }

            SplineRuntimeEditor runtimeEditor = SplineRuntimeEditor.Instance;
            if(runtimeEditor == null)
            {
                return;
            }
            runtimeEditor.SplineMaterial.SetPass(0);

            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix);
            GL.Begin(GL.LINES);
            Vector3 p0 = m_points[0];
            for (int i = 1; i < m_points.Length; i += 3)
            {
                Vector3 p1 = m_points[i];
                Vector3 p2 = m_points[i + 1];
                Vector3 p3 = m_points[i + 2];
         
                GL.Color(SplineRuntimeEditor.ControlPointLineColor);
                GL.Vertex(p0);
                GL.Vertex(p1);
      
                GL.Color(SplineRuntimeEditor.ControlPointLineColor);
                GL.Vertex(p2);
                GL.Vertex(p3);
               
                p0 = p3;
            }
            GL.End();
            GL.Begin(GL.LINES);
            GL.Color(SplineRuntimeEditor.SplineColor);
            p0 = m_points[0];
            for (int i = 1; i < m_points.Length; i += 3)
            {
                Vector3 p1 = m_points[i];
                Vector3 p2 = m_points[i + 1];
                Vector3 p3 = m_points[i + 2];

                float len = (p0 - p1).magnitude + (p1 - p2).magnitude + (p2 - p3).magnitude;
                int count = Mathf.CeilToInt(runtimeEditor.Smoothness * len);
                if(count <= 0)
                {
                    count = 1;
                }
                

                for (int j = 0; j < count; ++j)
                {
                    float t = ((float)j) / count;
                    Vector3 point = CurveUtils.GetPoint(p0, p1, p2, p3, t);

                    GL.Vertex(point);

                    t = ((float)j + 1) / count;
                    point = CurveUtils.GetPoint(p0, p1, p2, p3, t);

                    GL.Vertex(point);
                }

                p0 = p3;
            }
            GL.End();
            GL.PopMatrix();
        }
    }
}
