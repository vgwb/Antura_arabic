using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Battlehub.SplineEditor;
namespace Battlehub.MeshDeformer2
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter), typeof(Scaffold))]
    public class MeshDeformer : SplineBase
    {
        private const int WRAP_DEFORM_VERSION = 1;

        [HideInInspector]
        public bool Internal_HasChanged;

        [SerializeField]
        [HideInInspector]
        private Mesh m_original;

        [SerializeField]
        [HideInInspector]
        private Mesh m_colliderOriginal;

        [SerializeField]
        [HideInInspector]
        private Contact[] m_contacts;

        [SerializeField]
        [HideInInspector]
        private Contact[] m_colliderContacts;

        [SerializeField]
        [HideInInspector]
        private ScaffoldWrapper[] m_scaffolds;

        [SerializeField]
        [HideInInspector]
        private Axis m_axis = Axis.Z;

        [SerializeField]
        [HideInInspector]
        private int m_sliceCount = 5;

        [SerializeField]
        [HideInInspector]
        private float m_spacing;

        [SerializeField]
        [HideInInspector]
        private int m_curvesPerMesh = 1;

        private MeshFilter m_filter;
        private MeshCollider m_collider;

        public Axis Axis
        {
            get { return m_axis; }
            set
            {
                m_axis = value;
                WrapAndDeformAll();
            }
        }

        public virtual int Approximation
        {
            get { return m_sliceCount; }
            set
            {
                m_sliceCount = value;
                WrapAndDeformAll();
            }
        }

        public virtual float Spacing
        {
            get { return m_spacing; }
            set
            {
                ChangeSpacing(value);
            }
        }

        public virtual int CurvesPerMesh
        {
            get { return m_curvesPerMesh; }
            set
            {
                ChangeCurvesPerMesh(value);
            }
        }

        public override bool Loop
        {
            get
            {
                return base.Loop;
            }
            set
            {
                if (m_spacing <= 0)
                {
                    base.Loop = value;
                }
                else
                {
                    ChangeLoop(value);
                }
            }
        }

        public ScaffoldWrapper[] Scaffolds
        {
            get { return m_scaffolds; }
        }

        public Contact[] Contacts
        {
            get { return m_contacts; }
        }

        public Mesh Original
        {
            get { return m_original; }
            set
            {
                m_original = value;
                if (m_original == null)
                {
                    m_contacts = null;
                }
                else
                {
                    m_contacts = m_original.FindContacts(m_axis);
                }
            }
        }

        public Contact[] ColliderContacts
        {
            get { return m_colliderContacts; }
        }

        public Mesh ColliderOriginal
        {
            get { return m_colliderOriginal; }
            set
            {
                m_colliderOriginal = value;
                if (m_colliderOriginal == null)
                {
                    m_colliderContacts = null;
                }
                else
                {
                    m_colliderContacts = m_colliderOriginal.FindContacts(m_axis);
                }
            }
        }


        protected override void AwakeOverride()
        {
            base.AwakeOverride();
            m_filter = GetComponent<MeshFilter>();
            m_collider = GetComponent<MeshCollider>();

            //if (m_original == null && m_filter != null && m_filter.sharedMesh != null)
            //{
            //    ResetDeformer();
            //}

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Internal_HasChanged = true;
            }
#endif
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (!Application.isPlaying)
            {
                if (Internal_HasChanged)
                {
                    Original = Original;
                    ColliderOriginal = ColliderOriginal;
                    WrapAndDeformAll();

                    Internal_HasChanged = false;
                }
            }
        }
#endif

        public override void Load(SplineSnapshot snapshot)
        {
            if(snapshot.CurveCount < 1)
            {
                throw new ArgumentException("At least one curve required");
            }

            int expectedCurveCount = snapshot.CurveCount;
            int expectedSegmentsCount = expectedCurveCount / CurvesPerMesh;
            if(expectedCurveCount != expectedCurveCount * CurvesPerMesh)
            {
                throw new ArgumentException("snapshot.CurveCount should be evenly divisible by CurvesPerMesh");
            }

            int deltaSegments = expectedSegmentsCount - m_scaffolds.Length;
            if(deltaSegments > 0)
            {
                for(int i = 0; i < deltaSegments; ++i)
                {
                    Extend();
                }
            }
            else if(deltaSegments < 0)
            {
                deltaSegments = -deltaSegments;
                for(int i = CurveCount - 1; i >= 0; --i)
                {
                    if(deltaSegments == 0)
                    {
                        break;
                    }

                    if (Remove(i))
                    {
                        deltaSegments--;
                    }
                }
            }
     
            base.Load(snapshot);
            WrapAndDeformAll();
        }


        public void ResetDeformer()
        {
            if (m_original == null)
            {
                m_original = m_filter.sharedMesh;
            }

            if (m_original == null)
            {
                m_original = new Mesh();
            }

            if (m_collider != null)
            {
                if (m_colliderOriginal == null)
                {
                    m_colliderOriginal = m_collider.sharedMesh;
                }

                if (m_colliderOriginal == null)
                {
                    m_colliderOriginal = new Mesh();
                }
            }
            else
            {
                m_colliderOriginal = new Mesh();
            }

            Vector3 from;
            Vector3 to;
            m_original.GetBounds(m_axis, out from, out to);

            m_contacts = m_original.FindContacts(m_axis);
            m_colliderContacts = m_colliderOriginal.FindContacts(from, to, m_axis);

            m_filter.sharedMesh = Instantiate(m_original);
            m_filter.sharedMesh.name = m_original.name + " Deformed";

            if (m_collider != null)
            {
                m_collider.sharedMesh = Instantiate(m_colliderOriginal);
                m_collider.sharedMesh.name = m_colliderOriginal.name + " Deformed";
            }

            Vector3[] points = new[]
            {
                    from,
                    from + (to - from) * (1.0f / 3.0f),
                    from + (to - from) * (2.0f / 3.0f),
                    to
            };

            ControlPointSetting[] settings = new[]
            {
                new ControlPointSetting(new Twist(0.0f, 0.0f, 1.0f), new Thickness(Vector3.one, 0.0f, 1.0f)),
                new ControlPointSetting(new Twist(0.0f, 0.0f, 1.0f), new Thickness(Vector3.one, 0.0f, 1.0f)),
            };
            ControlPointMode[] modes = new[]
            {
                ControlPointMode.Mirrored,
                ControlPointMode.Mirrored,
            };
            LoadSpline(new SplineSnapshot(points, settings, modes, false));

            ScaffoldWrapper scaffold = new ScaffoldWrapper(gameObject.GetComponent<Scaffold>(), false);
            Mesh colliderMesh = null;
            if (m_collider != null)
            {
                colliderMesh = m_collider.sharedMesh;
            }

            scaffold.Wrap(m_filter.sharedMesh, colliderMesh, m_axis, new[] { 0 }, m_sliceCount);
            m_scaffolds = new[] { scaffold };

            scaffold.Deform(this, m_original, m_colliderOriginal);
            scaffold.RecalculateNormals();
        }

        protected override void OnCurveChanged()
        {
            for (int i = 0; i < m_scaffolds.Length; ++i)
            {
                ScaffoldWrapper scaffold = m_scaffolds[i];

                if (scaffold.IsRigid)
                {
                    Vector3[] points;
                    GetRigidPoints(scaffold.CurveIndices.Min() * 3, scaffold.CurveIndices.Min(), out points);
                    SetPoints(scaffold.CurveIndices.Min(), points, ControlPointMode.Free, false);
                }
            }

            DeformAll();

#if UNITY_EDITOR
            PersistentVersions[0]++;
            OnVersionChanged();
#endif
        }

        private void DeformAll()
        {
            for (int i = 0; i < m_scaffolds.Length; ++i)
            {
                ScaffoldWrapper scaffold = m_scaffolds[i];
                scaffold.Deform(this, m_original, m_colliderOriginal);
                scaffold.RecalculateNormals();
            }

            ScaffoldWrapper prev = null;
            if (Loop)
            {
                prev = m_scaffolds[m_scaffolds.Length - 1];
            }
            for (int i = 0; i < m_scaffolds.Length; ++i)
            {
                ScaffoldWrapper scaffold = m_scaffolds[i];
                scaffold.SlerpContacts(this, m_original, m_colliderOriginal, prev, null);
                prev = scaffold;
            }
        }

        protected override void OnCurveChanged(int pointIndex, int curveIndex)
        {
            ScaffoldWrapper scaffold = FindScaffold(curveIndex);
            if (scaffold != null)
            {
                if (scaffold.IsRigid)
                {
                    if (pointIndex == 0 && curveIndex == 0)
                    {
                        int firstPointIndex = scaffold.CurveIndices.Min() * 3;
                        int lastPointIndex = scaffold.CurveIndices.Max() * 3 + 3;
                        int midPointIndex = (firstPointIndex + lastPointIndex + 1) / 2;

                        Vector3[] points;
                        GetRigidPoints(midPointIndex, curveIndex, out points);
                        SetPoints(scaffold.CurveIndices.Min(), points, ControlPointMode.Free, false);
                    }
                    else
                    {
                        Vector3[] points;
                        GetRigidPoints(pointIndex, curveIndex, out points);
                        SetPoints(scaffold.CurveIndices.Min(), points, ControlPointMode.Free, false);
                    }

                }
                Deform(curveIndex);
            }

        }

        private ScaffoldWrapper Prev(ScaffoldWrapper scaffold)
        {
            if (scaffold == null)
            {
                return null;
            }
            int prevIndex = scaffold.CurveIndices.Min() - 1;
            if (Loop)
            {
                if (prevIndex < 0)
                {
                    prevIndex = CurveCount - 1;
                }
            }

            if (prevIndex >= 0)
            {
                return m_scaffolds.Where(s => s != null && s.CurveIndices.Contains(prevIndex)).FirstOrDefault();
            }

            return null;
        }

        private ScaffoldWrapper Next(ScaffoldWrapper scaffold)
        {
            if (scaffold == null)
            {
                return null;
            }
            int nextIndex = scaffold.CurveIndices.Max() + 1;
            if (Loop)
            {
                if (nextIndex >= CurveCount)
                {
                    nextIndex = 0;
                }
            }

            if (nextIndex < CurveCount)
            {
                return m_scaffolds.Where(s => s != null && s.CurveIndices.Contains(nextIndex)).FirstOrDefault();
            }

            return null;
        }

        private void Deform(ScaffoldWrapper scaffold)
        {
            ScaffoldWrapper prev = Prev(scaffold);
            ScaffoldWrapper next = Next(scaffold);
            ForceRigid(scaffold);
            ForceRigid(prev);
            ForceRigid(next);
            if (scaffold != null)
            {
                scaffold.Deform(this, m_original, m_colliderOriginal);
                if (prev != null)
                {
                    prev.Deform(this, m_original, m_colliderOriginal);
                }
                if (next != null)
                {
                    next.Deform(this, m_original, m_colliderOriginal);
                }
                for (int i = 0; i < m_scaffolds.Length; ++i)
                {
                    scaffold = m_scaffolds[i];
                    scaffold.RecalculateNormals();
                }

                prev = null;
                if (Loop)
                {
                    prev = m_scaffolds[m_scaffolds.Length - 1];
                }

                for (int i = 0; i < m_scaffolds.Length; ++i)
                {
                    scaffold = m_scaffolds[i];
                    scaffold.SlerpContacts(this, m_original, m_colliderOriginal, prev, null);
                    prev = scaffold;
                }
            }
        }

        private void Deform(int curveIndex)
        {
            ScaffoldWrapper scaffold = FindScaffold(curveIndex);
            Deform(scaffold);
        }

        protected override void ResetOverride()
        {
            base.ResetOverride();

            m_filter = GetComponent<MeshFilter>();
            if (m_original != null)
            {
                m_filter.sharedMesh = m_original;
            }

            m_collider = GetComponent<MeshCollider>();
            if (m_collider != null)
            {
                m_collider.sharedMesh = m_colliderOriginal;
            }

            m_original = null;
            m_colliderOriginal = null;
            ResetDeformer();

        }

        public void WrapAndDeformAll()
        {

            for (int i = 0; i < m_scaffolds.Length; ++i)
            {
                ScaffoldWrapper scaffold = m_scaffolds[i];
                WrapAndDeform(scaffold);
            }

            ScaffoldWrapper prev = null;
            if (Loop)
            {
                prev = m_scaffolds[m_scaffolds.Length - 1];
            }
            for (int i = 0; i < m_scaffolds.Length; ++i)
            {
                ScaffoldWrapper scaffold = m_scaffolds[i];
                scaffold.SlerpContacts(this, m_original, m_colliderOriginal, prev, null);
                prev = scaffold;
            }

#if UNITY_EDITOR
            PersistentVersions[WRAP_DEFORM_VERSION]++;
            OnVersionChanged();
#endif

        }

        private void WrapAndDeform(ScaffoldWrapper scaffold)
        {
            if (scaffold.Obj != null)
            {
                Mesh colliderMesh = null;
                MeshCollider collider = scaffold.Obj.GetComponent<MeshCollider>();
                if (collider != null)
                {
                    collider.sharedMesh = Instantiate(m_colliderOriginal);
                    collider.sharedMesh.name = m_colliderOriginal.name + " Deformed";
                    colliderMesh = collider.sharedMesh;
                }

                MeshFilter filter = scaffold.Obj.GetComponent<MeshFilter>();
                if (filter != null)
                {
                    if (m_original == null)
                    {
                        return;
                    }
                    filter.sharedMesh = Instantiate(m_original);
                    filter.sharedMesh.name = m_original.name + " Deformed";

                    scaffold.Wrap(filter.sharedMesh, colliderMesh, m_axis, scaffold.CurveIndices, m_sliceCount);
                    scaffold.Deform(this, m_original, m_colliderOriginal);
                    scaffold.RecalculateNormals();
                }
            }
        }

        private void ChangeLoop(bool loop)
        {
            if (m_spacing > 0)
            {
                if (!loop)
                {
                    ScaffoldWrapper scaffold = FindScaffold(0);
                    if (scaffold != null)
                    {
                        if (scaffold.IsEmptySpace)
                        {
                            GameObject objToRemove;
                            Remove(0, out objToRemove);
                        }
                    }
                }
                else
                {
                    bool isRigid = false;
                    ScaffoldWrapper previous = FindScaffold(0);
                    if (previous != null)
                    {
                        isRigid = previous.IsRigid;
                    }

                    if (m_spacing > 0)
                    {
                        PrependCurve(m_spacing, 0, true);

                        ScaffoldWrapper emptySpace = new ScaffoldWrapper(null, isRigid);
                        emptySpace.Wrap(null, null, Axis, new[] { 0 }, Approximation);

                        Array.Resize(ref m_scaffolds, m_scaffolds.Length + 1);
                        ShiftAndInsert(0, emptySpace);
                    }

                    ChangeSpacing(m_spacing);
                }
            }

            base.Loop = loop;
        }



        private GameObject[] ChangeSpacing(float spacing)
        {
            GameObject[] objectsToRemove = null;
            if (m_spacing <= 0)
            {
                if (spacing > 0)
                {
                    float mag = spacing;

                    List<ScaffoldWrapper> scaffoldsList = new List<ScaffoldWrapper>(m_scaffolds.OrderBy(s => s.CurveIndices[0]));
                    int offset = scaffoldsList.Count - 1;
                    int to = 0;
                    if (Loop)
                    {
                        offset++;
                        to = -1;
                    }

                    Vector3 mainControlPoint = GetControlPointLocal(ControlPointCount - 1);
                    Vector3 lastPoint = GetControlPointLocal(ControlPointCount - 2);

                    for (int s = scaffoldsList.Count - 1; s > to; s--)
                    {
                        ScaffoldWrapper scaffold = scaffoldsList[s];
                        int curveIndex = scaffold.CurveIndices.Min();
                        scaffold.Shift(offset);

                        ScaffoldWrapper emptySpace = new ScaffoldWrapper(null, scaffold.IsRigid);
                        emptySpace.Wrap(null, null, Axis, new[] { scaffold.CurveIndices.Min() - 1 }, Approximation);
                        scaffoldsList.Insert(s, emptySpace);

                        Vector3 dir;
                        if (curveIndex != 0)
                        {
                            dir = GetDirection(0.0f, curveIndex);
                        }
                        else
                        {
                            dir = GetDirection(1.0f);
                        }

                        Vector3 point = GetPoint(0.0f, curveIndex);
                        point = transform.InverseTransformPoint(point);
                        dir = transform.InverseTransformDirection(dir);

                        Vector3[] points = new[]
                        {
                            point - dir * mag,
                            point - dir * mag * (2.0f / 3.0f),
                            point - dir * mag * (1.0f / 3.0f)
                        };

                        PrependCurve(points, curveIndex, mag, true);
                        offset--;
                    }
                    m_scaffolds = scaffoldsList.ToArray();

                    for (int s = 0; s < m_scaffolds.Length; ++s)
                    {
                        ScaffoldWrapper scaffold = m_scaffolds[s];
                        if (scaffold.IsEmptySpace)
                        {
                            for (int i = 0; i < scaffold.CurveIndices.Length; ++i)
                            {
                                AlignCurve(scaffold.CurveIndices[i], spacing);
                            }
                        }
                    }

                    ScaffoldWrapper prev = null;
                    if (Loop)
                    {
                        prev = m_scaffolds[m_scaffolds.Length - 1];
                        SetControlPointLocal(ControlPointCount - 1, mainControlPoint);
                        SetControlPointLocal(0, mainControlPoint);
                        SetControlPointLocal(ControlPointCount - 2, lastPoint);

                    }

                    for (int s = 0; s < m_scaffolds.Length; ++s)
                    {
                        ScaffoldWrapper scaffold = m_scaffolds[s];
                        scaffold.Deform(this, m_original, m_colliderOriginal);
                        scaffold.RecalculateNormals();
                    }

                    for (int s = 0; s < m_scaffolds.Length; ++s)
                    {
                        ScaffoldWrapper scaffold = m_scaffolds[s];
                        scaffold.SlerpContacts(this, m_original, m_colliderOriginal, prev, null);
                        prev = scaffold;
                    }
                }
            }
            else
            {
                if (spacing < 0)
                {
                    spacing = 0;
                }

                Vector3 mainControlPoint = GetControlPointLocal(ControlPointCount - 1);
                Vector3 lastPoint = GetControlPointLocal(ControlPointCount - 2);

                for (int s = 0; s < m_scaffolds.Length; ++s)
                {
                    ScaffoldWrapper scaffold = m_scaffolds[s];
                    if (scaffold.IsEmptySpace)
                    {
                        for (int i = 0; i < scaffold.CurveIndices.Length; ++i)
                        {
                            AlignCurve(scaffold.CurveIndices[i], spacing);
                        }
                    }
                }


                if (Loop)
                {
                    SetControlPointLocal(ControlPointCount - 1, mainControlPoint);
                    SetControlPointLocal(0, mainControlPoint);
                    SetControlPointLocal(ControlPointCount - 2, lastPoint);
                }

                if (spacing <= 0)
                {
                    List<ScaffoldWrapper> scaffoldsList = new List<ScaffoldWrapper>(m_scaffolds.OrderBy(s => s.CurveIndices[0]));

                    for (int s = scaffoldsList.Count - 1; s >= 0; s--)
                    {
                        ScaffoldWrapper scaffold = scaffoldsList[s];
                        if (scaffold.IsEmptySpace)
                        {
                            int shift = 0;
                            for (int i = 0; i < scaffold.CurveIndices.Length; ++i)
                            {
                                RemoveCurve(scaffold.CurveIndices[i]);
                                shift--;
                            }
                            scaffoldsList.RemoveAt(s);
                            for (int ss = s; ss < scaffoldsList.Count; ++ss)
                            {
                                scaffoldsList[ss].Shift(shift);
                            }
                        }
                    }
                    m_scaffolds = scaffoldsList.ToArray();


                    if (Loop)
                    {
                        Deform(CurveCount - 1);
                    }
                }

                ScaffoldWrapper prev = null;
                if (Loop)
                {
                    prev = m_scaffolds.Last();
                }

                for (int s = 0; s < m_scaffolds.Length; ++s)
                {
                    ScaffoldWrapper scaffold = m_scaffolds[s];
                    scaffold.Deform(this, m_original, m_colliderOriginal);
                    scaffold.RecalculateNormals();
                }

                for (int s = 0; s < m_scaffolds.Length; ++s)
                {
                    ScaffoldWrapper scaffold = m_scaffolds[s];
                    scaffold.SlerpContacts(this, m_original, m_colliderOriginal, prev, null);
                    prev = scaffold;
                }
            }

            m_spacing = spacing;

#if UNITY_EDITOR
            PersistentVersions[0]++;
            OnVersionChanged();
#endif

            return objectsToRemove;
        }

        private void ChangeCurvesPerMesh(int curvesPerMesh)
        {
            curvesPerMesh = Math.Max(1, curvesPerMesh);
            if (m_curvesPerMesh == curvesPerMesh)
            {
                return;
            }

            ChangeCurvesPerMeshStep1(0, curvesPerMesh);

            for (int i = 0; i < m_scaffolds.Length; ++i)
            {
                ScaffoldWrapper scaffold = m_scaffolds[i];
                ChangeCurvesPerMeshStep2(curvesPerMesh, scaffold);
            }

            m_curvesPerMesh = curvesPerMesh;
            WrapAndDeformAll();
        }

        private void ChangeCurvesPerMeshStep1(int scaffoldIndex, int curvesPerMesh)
        {
            int offset = 0;
            for (int i = scaffoldIndex; i < m_scaffolds.Length; ++i)
            {
                ScaffoldWrapper scaffold = m_scaffolds[i];
                int[] curveIndices = scaffold.CurveIndices.OrderBy(c => c).ToArray();
                for (int c = 0; c < curveIndices.Length; ++c)
                {
                    curveIndices[c] += offset;
                }
                scaffold.CurveIndices = curveIndices;

                if (!scaffold.IsEmptySpace)
                {
                    int currentCurvesPerMesh = scaffold.CurveIndices.Length;
                    int deltaOffset = curvesPerMesh - currentCurvesPerMesh;
                    offset += deltaOffset;
                }
            }
        }

        private void ChangeCurvesPerMeshStep2(int curvesPerMesh, ScaffoldWrapper scaffold)
        {
            int[] curveIndices = scaffold.CurveIndices;
            if (!scaffold.IsEmptySpace)
            {
                int currentCurvesPerMesh = scaffold.CurveIndices.Length;
                Subdivide(scaffold.CurveIndices.Min(), scaffold.CurveIndices.Max(), curvesPerMesh);
                Array.Resize(ref curveIndices, curvesPerMesh);
                for (int c = currentCurvesPerMesh; c < curveIndices.Length; ++c)
                {
                    curveIndices[c] = curveIndices[c - 1] + 1;
                }
            }
            scaffold.CurveIndices = curveIndices;

        }

        public bool Remove(int curveIndex)
        {
            GameObject objectToRemove;
            bool result = Remove(curveIndex, out objectToRemove);
            if(objectToRemove != null)
            {
                Destroy(objectToRemove);
            }
            return result;
        }

        public bool Remove(int curveIndex, out GameObject objectToRemove)
        {
#if UNITY_EDITOR
            int versionIndex = 0;
#endif
            objectToRemove = null;
            ScaffoldWrapper removeScaffold = m_scaffolds.Where(s => s != null && s.CurveIndices.Contains(curveIndex)).FirstOrDefault();
            if (removeScaffold == null)
            {
                return false;
            }

            if (removeScaffold.Obj != null && removeScaffold.CurveIndices.Length <= 1 && removeScaffold.Obj.GetComponent<MeshDeformer>() != null)
            {
#if UNITY_EDITOR
                UnityEditor.EditorUtility.DisplayDialog("Can't remove", "Can't to remove root curve", "OK");
#endif
                Debug.LogWarning("Unable to Remove curve attached to MeshDeformer");
                return false;
            }

            if (!RemoveCurve(curveIndex))
            {
                return false;
            }


            int scaffoldIndex;
            int shift = -1;
            int[] curveIndices;
            if (removeScaffold.Obj != null && removeScaffold.CurveIndices.Length > 1)
            {
                int indexOfIndex = Array.IndexOf(removeScaffold.CurveIndices, curveIndex);
                curveIndices = removeScaffold.CurveIndices.Where(index => index != curveIndex).ToArray();
                for (int i = indexOfIndex; i < curveIndices.Length; i++)
                {
                    curveIndices[i]--;
                }
                MeshFilter filter = removeScaffold.Obj.GetComponent<MeshFilter>();
                filter.sharedMesh = Instantiate(m_original);
                filter.sharedMesh.name = m_original.name + " Deformed";

                Mesh colliderMesh = null;
                MeshCollider collider = removeScaffold.Obj.GetComponent<MeshCollider>();
                if (collider != null)
                {
                    collider.sharedMesh = Instantiate(m_colliderOriginal);
                    collider.sharedMesh.name = m_colliderOriginal.name + " Deformed";
                    colliderMesh = collider.sharedMesh;
                }

                removeScaffold.Wrap(filter.sharedMesh, colliderMesh, m_axis, curveIndices, m_sliceCount);

#if UNITY_EDITOR
                versionIndex = WRAP_DEFORM_VERSION;
#endif
                scaffoldIndex = ToScaffoldIndex(removeScaffold.CurveIndices.Max() + 1);
            }
            else
            {
                m_scaffolds = m_scaffolds.Where(s => s != removeScaffold).ToArray();
                if (removeScaffold.Obj != null)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(removeScaffold.Obj.gameObject);
                    }
                    else
                    {
                        objectToRemove = removeScaffold.Obj.gameObject;
                    }
                }


                if (curveIndex == CurveCount)
                {
                    ScaffoldWrapper prevScaffold = FindScaffold(curveIndex - 1);
                    if (prevScaffold != null && prevScaffold.IsEmptySpace)
                    {
                        if (RemoveCurve(curveIndex - 1))
                        {
                            shift--;
                            m_scaffolds = m_scaffolds.Where(s => s != prevScaffold).ToArray();
                        }
                    }
                }
                else
                {
                    ScaffoldWrapper nextScaffold = FindScaffold(curveIndex + 1);
                    if (nextScaffold != null && nextScaffold.IsEmptySpace)
                    {
                        if (RemoveCurve(curveIndex))
                        {
                            shift--;
                            m_scaffolds = m_scaffolds.Where(s => s != nextScaffold).ToArray();
                        }
                    }
                }

                scaffoldIndex = ToScaffoldIndex(curveIndex);
                curveIndices = new[] { curveIndex };
            }

            if (scaffoldIndex > -1)
            {
                for (int i = scaffoldIndex; i < m_scaffolds.Length; ++i)
                {
                    ScaffoldWrapper scaffold = m_scaffolds[i];
                    if (scaffold == null)
                    {
                        continue;
                    }

                    scaffold.Shift(shift);
                    scaffold.Deform(this, m_original, m_colliderOriginal);
                }
            }


            if (curveIndices.Length > 0)
            {
                Deform(curveIndices.First());
            }

            if (Loop)
            {
                Deform(CurveCount - 1);
            }

#if UNITY_EDITOR
            PersistentVersions[versionIndex]++;
            OnVersionChanged();
#endif

            return true;
        }

        public GameObject Extend(bool prepend = false)
        {
            int curveIndex = 0;

            GameObject extension = Instantiate(gameObject);
            extension.name = "segment";
            extension.transform.parent = transform;
            extension.transform.localPosition = Vector3.zero;
            extension.transform.localRotation = Quaternion.identity;
            extension.transform.localScale = Vector3.one;

            int childs = extension.transform.childCount;
            for (int i = childs - 1; i >= 0; i--)
            {
                DestroyImmediate(extension.transform.GetChild(i).gameObject);
            }

            Component[] components = extension.GetComponents<Component>();
            for (int i = components.Length - 1; i >= 0; i--)
            {
                Component component = components[i];
                if (component is Transform)
                {
                    continue;
                }

                if (component is MeshFilter)
                {
                    continue;
                }

                if (component is MeshRenderer)
                {
                    continue;
                }

                if (component is MeshCollider)
                {
                    continue;
                }

                if(component is Scaffold)
                {
                    continue;
                }

                DestroyImmediate(component);
            }

            Scaffold extensionScaffold = extension.GetComponent<Scaffold>();
            if(extensionScaffold != null)
            {
                DestroyImmediate(extensionScaffold);
            }


            Vector3 from;
            Vector3 to;
            m_original.GetBounds(m_axis, out from, out to);
            ScaffoldWrapper previous;
            if (prepend && !Loop)
            {
                previous = FindScaffold(curveIndex);
                bool isRigid = false;
                if (previous != null)
                {
                    isRigid = previous.IsRigid;
                }

                if (m_spacing > 0)
                {
                    PrependCurve(m_spacing, curveIndex, true);

                    ScaffoldWrapper emptySpace = new ScaffoldWrapper(null, isRigid);
                    emptySpace.Wrap(null, null, Axis, new[] { curveIndex }, Approximation);

                    Array.Resize(ref m_scaffolds, m_scaffolds.Length + 1);
                    ShiftAndInsert(curveIndex, emptySpace);
                }

                float mag = (to - from).magnitude;

                PrependCurve(mag, curveIndex, false);

                MeshFilter filter = extension.GetComponent<MeshFilter>();
                filter.sharedMesh = Instantiate(m_original);
                filter.sharedMesh.name = m_original.name + " Deformed";

                Mesh colliderMesh = null;
                MeshCollider collider = extension.GetComponent<MeshCollider>();
                if (collider != null)
                {
                    collider.sharedMesh = Instantiate(m_colliderOriginal);
                    collider.sharedMesh.name = m_colliderOriginal.name + " Deformed";
                    colliderMesh = collider.sharedMesh;
                }

                int[] curveIndices = new int[CurvesPerMesh];
                for (int i = 0; i < CurvesPerMesh; ++i)
                {
                    curveIndices[i] = curveIndex + i;
                }


                ScaffoldWrapper newScaffold = new ScaffoldWrapper(extension.AddComponent<Scaffold>(), isRigid);
                newScaffold.Wrap(filter.sharedMesh, colliderMesh, m_axis, curveIndices, m_sliceCount);

                Array.Resize(ref m_scaffolds, m_scaffolds.Length + 1);
                ShiftAndInsert(curveIndex, newScaffold);
            }
            else
            {
                previous = FindScaffold(CurveCount - 1);
                bool isRigid = false;
                if (previous != null)
                {
                    isRigid = previous.IsRigid;
                }

                if (m_spacing > 0)
                {
                    AppendCurve(m_spacing, true);

                    ScaffoldWrapper emptySpace = new ScaffoldWrapper(null, isRigid);
                    emptySpace.Wrap(null, null, Axis, new[] { CurveCount - 1 }, Approximation);

                    Array.Resize(ref m_scaffolds, m_scaffolds.Length + 1);
                    m_scaffolds[m_scaffolds.Length - 1] = emptySpace;
                }

                float mag = (to - from).magnitude;
                AppendCurve(mag, false);

                MeshFilter filter = extension.GetComponent<MeshFilter>();
                if (m_original != null)
                {
                    filter.sharedMesh = Instantiate(m_original);
                    filter.sharedMesh.name = m_original.name + " Deformed";
                }

                Mesh colliderMesh = null;
                MeshCollider collider = extension.GetComponent<MeshCollider>();
                if (collider != null)
                {
                    collider.sharedMesh = Instantiate(m_colliderOriginal);
                    collider.sharedMesh.name = m_colliderOriginal.name + " Deformed";
                    colliderMesh = collider.sharedMesh;
                }

                int[] curveIndices = new int[CurvesPerMesh];
                for (int i = 0; i < CurvesPerMesh; ++i)
                {
                    curveIndices[i] = CurveCount - CurvesPerMesh + i;
                }

                ScaffoldWrapper scaffold = new ScaffoldWrapper(extension.AddComponent<Scaffold>(), isRigid);
                scaffold.Wrap(filter.sharedMesh, colliderMesh, m_axis, curveIndices, m_sliceCount);

                Array.Resize(ref m_scaffolds, m_scaffolds.Length + 1);
                m_scaffolds[m_scaffolds.Length - 1] = scaffold;
                Deform(m_scaffolds.Length - 1);

                if (Loop)
                {
                    Deform(0);
                }
            }

            if (previous != null)
            {
                Deform(previous);
            }

#if UNITY_EDITOR
            PersistentVersions[0]++;
            OnVersionChanged();
#endif

            return extension;
        }

        private void ForceRigid(ScaffoldWrapper scaffold)
        {
            if (scaffold == null)
            {
                return;
            }

            if (!scaffold.IsRigid)
            {
                return;
            }

            int curveIndex = scaffold.CurveIndices.Min();
            int pointIndex = scaffold.CurveIndices.Max() * 3 + 3;

            Vector3[] points;
            GetRigidPoints(pointIndex, curveIndex, out points);
            SetPoints(curveIndex, points, ControlPointMode.Free, false);
        }

        private void ShiftAndInsert(int curveIndex, ScaffoldWrapper newScaffold)
        {
            int shift = newScaffold.CurveIndices.Length;
            int scaffoldIndex = ToScaffoldIndex(curveIndex);
            if (scaffoldIndex == -1)
            {
                Deform(newScaffold);
            }
            else
            {
                for (int i = m_scaffolds.Length - 1; i > scaffoldIndex; i--)
                {
                    m_scaffolds[i] = m_scaffolds[i - 1];
                }
                m_scaffolds[scaffoldIndex] = newScaffold;
                Deform(newScaffold);

                for (int i = m_scaffolds.Length - 1; i > scaffoldIndex; i--)
                {
                    ScaffoldWrapper scaffold = m_scaffolds[i];
                    if (scaffold == null)
                    {
                        continue;
                    }
                    scaffold.Shift(shift);
                }

                for (int i = 0; i <= scaffoldIndex; i++)
                {
                    ScaffoldWrapper scaffold = m_scaffolds[i];
                    if (scaffold == null)
                    {
                        continue;
                    }

                    scaffold.Deform(this, m_original, m_colliderOriginal);
                }

                if (Loop)
                {
                    Deform(m_scaffolds.Length - 1);
                }
                else
                {
                    for (int i = 0; i < m_scaffolds.Length; i++)
                    {
                        ScaffoldWrapper scaffold = m_scaffolds[i];
                        scaffold.RecalculateNormals();
                    }
                    ScaffoldWrapper prev = null;
                    for (int i = 0; i < m_scaffolds.Length; i++)
                    {
                        ScaffoldWrapper scaffold = m_scaffolds[i];
                        scaffold.SlerpContacts(this, m_original, m_colliderOriginal, prev, null);
                        prev = scaffold;
                    }
                }
            }
        }

        private void AppendCurve(float mag, bool enforceNeighbour)
        {
            Vector3 dir = transform.InverseTransformDirection(GetDirection(1.0f));
            Vector3 point = GetPoint(1.0f);
            point = transform.InverseTransformPoint(point);

            int pointsCount = CurvesPerMesh * 3;
            float deltaT = 1.0f / pointsCount;
            float t = deltaT;
            Vector3[] points = new Vector3[pointsCount];
            for (int i = 0; i < pointsCount; ++i)
            {
                points[i] = point + dir * mag * t;
                t += deltaT;
            }

            AppendCurve(points, enforceNeighbour);
        }

        private void PrependCurve(float mag, int curveIndex, bool enforceNeighbour)
        {
            Vector3 dir = GetDirection(0.0f, curveIndex);
            Vector3 point = GetPoint(0.0f, curveIndex);

            dir = transform.InverseTransformDirection(dir);
            point = transform.InverseTransformPoint(point);

            int pointsCount = CurvesPerMesh * 3;
            float deltaT = 1.0f / pointsCount;
            float t = 1.0f;
            Vector3[] points = new Vector3[pointsCount];
            for (int i = 0; i < pointsCount; ++i)
            {
                points[i] = point - dir * mag * t;
                t -= deltaT;
            }

            PrependCurve(points, curveIndex, mag, enforceNeighbour);
        }

        public void Straighten(int pointIndex)
        {
            int curveIndex = (pointIndex - 1) / 3;
            ScaffoldWrapper straightenScaffold = FindScaffold(curveIndex);
            if (straightenScaffold == null)
            {
                throw new ArgumentOutOfRangeException("pointIndex");
            }

            if (straightenScaffold.IsEmptySpace)
            {
                Debug.LogWarning("Unable to straighten empty space");
                return;
            }

            bool toLast = curveIndex == 0;
            float size = m_original.GetSize(m_axis);
            if (straightenScaffold != null)
            {
                for (int i = 0; i < straightenScaffold.CurveIndices.Length; ++i)
                {
                    AlignCurve(straightenScaffold.CurveIndices[i], size, toLast);
                }
            }

            for (int i = 0; i < m_scaffolds.Length; ++i)
            {
                ScaffoldWrapper scaffold = m_scaffolds[i];
                scaffold.Deform(this, m_original, m_colliderOriginal);
                scaffold.RecalculateNormals();
            }

            ScaffoldWrapper prev = null;
            if (Loop)
            {
                prev = m_scaffolds[m_scaffolds.Length - 1];
            }
            for (int i = 0; i < m_scaffolds.Length; ++i)
            {
                ScaffoldWrapper scaffold = m_scaffolds[i];
                scaffold.SlerpContacts(this, m_original, m_colliderOriginal, prev, null);
                prev = scaffold;
            }

#if UNITY_EDITOR
            PersistentVersions[0]++;
            OnVersionChanged();
#endif
        }


        public void SetIsRigid(int pointIndex, bool isRigid)
        {
            int curveIndex = pointIndex / 3;
            if (curveIndex == CurveCount)
            {
                if (Loop)
                {
                    curveIndex = 0;
                }
                else
                {
                    curveIndex--;
                }
            }
            ScaffoldWrapper scaffold = FindScaffold(curveIndex);
            if (scaffold == null)
            {
                throw new IndexOutOfRangeException("curveIndex");
            }

            if (isRigid)
            {
                Vector3[] points;
                GetRigidPoints(pointIndex, curveIndex, out points);
                SetPoints(scaffold.CurveIndices.Min(), points, ControlPointMode.Free, false);
                if (scaffold.Scaffold != null)
                {
                    scaffold.Scaffold.Deform(this, m_original, m_colliderOriginal, true);
                    scaffold.Scaffold.RecalculateNormals();
                }
            }
            else
            {
                Deform(scaffold);
            }

#if UNITY_EDITOR
            PersistentVersions[0]++;
            OnVersionChanged();
#endif
            scaffold.IsRigid = isRigid;
        }

        private void GetRigidPoints(int pointIndex, int curveIndex, out Vector3[] points)
        {
            ScaffoldWrapper scaffold = FindScaffold(curveIndex);
            int firstCurveIndex = scaffold.CurveIndices.Min();
            int lastCurveIndex = scaffold.CurveIndices.Max();
            int firstPointIndex = firstCurveIndex * 3;
            int lastPointIndex = lastCurveIndex * 3 + 3;
            int midPointIndex = (lastPointIndex - firstPointIndex + 1) / 2;

            Vector3 point;
            Vector3 dir;

            bool increment;
            if ((pointIndex - firstPointIndex) < midPointIndex)
            {
                increment = false;
                dir = (GetControlPointLocal(firstPointIndex) - GetControlPointLocal(lastPointIndex)).normalized;
                point = GetControlPointLocal(lastPointIndex);
            }
            else
            {
                increment = true;
                dir = (GetControlPointLocal(lastPointIndex) - GetControlPointLocal(firstPointIndex)).normalized;
                point = GetControlPointLocal(firstPointIndex);
            }

            float mag = (GetControlPointLocal(firstPointIndex) - GetControlPointLocal(lastPointIndex)).magnitude;

            int pointsCount = lastPointIndex - firstPointIndex + 1;
            points = new Vector3[pointsCount];

            float deltaT = 1.0f / (pointsCount - 1);
            float t = 0.0f;

            if (increment)
            {
                for (int i = 0; i < pointsCount; ++i)
                {
                    points[i] = point + dir * mag * t;
                    t += deltaT;
                }
            }
            else
            {
                for (int i = pointsCount - 1; i >= 0; --i)
                {
                    points[i] = point + dir * mag * t;
                    t += deltaT;
                }
            }
        }

        public ScaffoldWrapper FindScaffold(int curveIndex)
        {
            ScaffoldWrapper scaffold = m_scaffolds.Where(s => s != null && s.CurveIndices.Any(c => c == curveIndex)).FirstOrDefault();
            return scaffold;
        }

        private int ToScaffoldIndex(int curveIndex)
        {
            ScaffoldWrapper scaffold = m_scaffolds.Where(s => s != null && s.CurveIndices.Any(c => c >= curveIndex)).FirstOrDefault();
            if (scaffold == null)
            {
                return -1;
            }

            return Array.IndexOf(m_scaffolds, scaffold);
        }

        public static Vector3 Side(Axis axis)
        {
            if (axis == Axis.Z)
            {
                return Vector3.forward;
            }
            else if (axis == Axis.X)
            {
                return Vector3.forward;
            }
            else
            {
                return Vector3.up;
            }
        }

        public static Vector3 Up(Axis axis)
        {
            if (axis == Axis.Z)
            {
                return Vector3.up;
            }
            else if (axis == Axis.X)
            {
                return Vector3.up;
            }
            else
            {
                return Vector3.back;
            }
        }

        protected override void AddControlPointComponent(GameObject ctrlPoint)
        {
            ctrlPoint.AddComponent<ControlPoint>();
        }

        protected override SplineControlPoint[] GetControlPoints()
        {
            return GetComponentsInChildren<ControlPoint>(true);
        }

#if UNITY_EDITOR
        protected override void OnSplineUndoRedo(int[] changedVersions)
        {
            for (int i = 0; i < m_scaffolds.Length; ++i)
            {
                ScaffoldWrapper scaffold = m_scaffolds[i];
                if (scaffold.Obj != null)
                {
                    Mesh colliderMesh = null;
                    MeshCollider collider = scaffold.Obj.GetComponent<MeshCollider>();
                    if (collider != null)
                    {
                        collider.sharedMesh = Instantiate(m_colliderOriginal);
                        collider.sharedMesh.name = m_colliderOriginal.name + " Deformed";
                        colliderMesh = collider.sharedMesh;
                    }

                    MeshFilter filter = scaffold.Obj.GetComponent<MeshFilter>();
                    if (filter != null)
                    {
                        if (ArrayUtility.Contains(changedVersions, WRAP_DEFORM_VERSION))
                        {
                            filter.sharedMesh = Instantiate(Original);
                            filter.sharedMesh.name = Original.name + " Deformed";
                            scaffold.Wrap(filter.sharedMesh, colliderMesh, Axis, scaffold.CurveIndices, Approximation);
                        }

                        scaffold.Deform(this, m_original, m_colliderOriginal);
                        scaffold.RecalculateNormals();
                    }
                }
            }

            ScaffoldWrapper prev = null;
            if (Loop)
            {
                prev = m_scaffolds.Last();
            }
            for (int i = 0; i < m_scaffolds.Length; ++i)
            {
                ScaffoldWrapper scaffold = m_scaffolds[i];
                scaffold.SlerpContacts(this, m_original, m_colliderOriginal, prev, null);
                prev = scaffold;
            }
        }
#endif
    }

}
