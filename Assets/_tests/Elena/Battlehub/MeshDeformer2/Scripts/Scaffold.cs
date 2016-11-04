using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Battlehub.MeshDeformer2
{
    [Serializable]
    public class Slice
    {
        [SerializeField]
        private Vector3 m_center;

        [SerializeField]
        private int m_curveIndex;

        [SerializeField]
        private float m_t; //[0, 1] within curve

        [SerializeField]
        private int[] m_indices;


        public Vector3 Center
        {
            get { return m_center; }
        }

        public int CurveIndex
        {
            get { return m_curveIndex; }
            set { m_curveIndex = value; }
        }

        public float T
        {
            get { return m_t; }
        }

        public int[] Indices
        {
            get { return m_indices; }
        }

        public Slice(Vector3 center, int curveIndex, float t, int[] vertexIndices)
        {
            m_center = center;
            m_curveIndex = curveIndex;
            m_t = Mathf.Clamp01(t);
            m_indices = vertexIndices;
        }
    }

    [ExecuteInEditMode]
    public class Scaffold : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        private Slice[] m_slices;

        [SerializeField]
        [HideInInspector]
        private Slice[] m_colliderSlices;

        [SerializeField]
       // [HideInInspector]
        private Mesh m_mesh;

        [SerializeField]
        [HideInInspector]
        private Mesh m_colliderMesh;

        [SerializeField]
        [HideInInspector]
        private Vector3 m_up;

        [SerializeField]
        [HideInInspector]
        private Quaternion m_axisRotation;

        [SerializeField]
        [HideInInspector]
        private MeshDeformer m_deformer;

        [SerializeField]
        [HideInInspector]
        private int m_instanceId;

        [SerializeField]
        [HideInInspector]
        private bool m_initialized;

        public int InstanceId
        {
            get { return m_instanceId; }
        }

        public int SliceCount
        {
            get { return m_slices == null ? 0 : m_slices.Length; }
        }

        [SerializeField]
        [HideInInspector]
        private MeshCollider m_collider;


        private void Awake()
        {
         

            if(!m_initialized)
            {
                m_instanceId = GetInstanceID();
                m_initialized = true;
            }

            m_deformer = GetComponentInParent<MeshDeformer>();
            m_collider = GetComponent<MeshCollider>();
        }

        private void Start()
        {
            if(m_deformer == null)
            {
                m_deformer = GetComponentInParent<MeshDeformer>();
            }

#if UNITY_EDITOR
    
            //Something strange happening when removing MeshDeformerExt obj and then pressing CTRL+Z. (Can't deform recovered object)
            //And now I am unable to find good solution.
            //This needed to fix this behavior...
            ScaffoldWrapper wrapper = m_deformer.Scaffolds.Where( s => s.ObjInstanceId == InstanceId).FirstOrDefault();
            if (wrapper != null)
            {
                wrapper.Obj = this;
            }
#endif
        }

        public void Shift(int delta)
        {
            for(int s = 0; s < m_slices.Length; ++s)
            {
                Slice slice = m_slices[s];
                slice.CurveIndex += delta;

                Slice colliderSlice = m_colliderSlices[s];
                colliderSlice.CurveIndex += delta;
            }
        }

        public void Wrap(Mesh mesh, Mesh colliderMesh, Axis axis, int[] curveIndices, int sliceCount)
        {
            m_collider = GetComponent<MeshCollider>();
            if (curveIndices.Length < 1)
            {
                throw new ArgumentException("at least one curveIndex required", "curveIndices");
            }

            m_up = MeshDeformer.Up(axis);
            if (axis == Axis.Z)
            {
                m_axisRotation = Quaternion.identity;
            }
            else if (axis == Axis.X)
            {
                m_axisRotation = Quaternion.AngleAxis(-90.0f, Vector3.up);
            }
            else
            {
                m_axisRotation = Quaternion.AngleAxis(90.0f, Vector3.right);
            }

            if (mesh == null)
            {
                m_mesh = null;
                m_colliderMesh = null;
                m_slices = new Slice[curveIndices.Length * (sliceCount + 1)];
                m_colliderSlices = new Slice[curveIndices.Length * (sliceCount + 1)];
                for (int i = 0; i < m_slices.Length; ++i)
                {
                    m_slices[i] = new Slice(Vector3.zero, -1, 0, new int[0]);
                    m_colliderSlices[i] = new Slice(Vector3.zero, -1, 0, new int[0]);
                }
                return;
            }

            sliceCount = Math.Max(sliceCount / curveIndices.Length, 1);

            Vector3 boundsFrom;
            Vector3 boundsTo;
            mesh.GetBounds(axis, out boundsFrom, out boundsTo);

            m_mesh = mesh;
            m_slices = CreateSlices(m_mesh, boundsFrom, boundsTo, axis, curveIndices, sliceCount);

            if(colliderMesh == null)
            {
                m_colliderMesh = null;
                m_colliderSlices = new Slice[curveIndices.Length * (sliceCount + 1)];
                for (int i = 0; i < m_colliderSlices.Length; ++i)
                {
                    m_colliderSlices[i] = new Slice(Vector3.zero, -1, 0, new int[0]);
                }
            }
            else
            {
                m_colliderMesh = colliderMesh;
                m_colliderSlices = CreateSlices(m_colliderMesh, boundsFrom, boundsTo, axis, curveIndices, sliceCount);
            }
        }

        private Slice[] CreateSlices(Mesh mesh, Vector3 boundsFrom, Vector3 boundsTo, Axis axis, int[] curveIndices, int sliceCount)
        {
            Slice[] result = new Slice[curveIndices.Length * (sliceCount + 1)];
            Vector3[] vertices = mesh.vertices;
            List<int>[,] slices = new List<int>[curveIndices.Length, sliceCount + 1];
            for (int i = 0; i < curveIndices.Length; ++i)
            {
                for (int s = 0; s <= sliceCount; ++s)
                {
                    slices[i, s] = new List<int>(vertices.Length / result.Length);
                }
            }

            Vector3 delta = (boundsTo - boundsFrom) / curveIndices.Length;
            for (int v = 0; v < vertices.Length; ++v)
            {
                Vector3 vertex = vertices[v];

                int minI = -1;
                int minS = -1;
                float minMag = float.MaxValue;

                Vector3 offset = boundsFrom;
                for (int i = 0; i < curveIndices.Length; ++i)
                {
                    float t = 0.0f;
                    for (int s = 0; s <= sliceCount; ++s)
                    {
                        Vector3 point = Vector3.Lerp(offset, offset + delta, t);
                        float sqrMag = (vertex - point).sqrMagnitude;
                        if (sqrMag < minMag)
                        {
                            minMag = sqrMag;
                            minI = i;
                            minS = s;
                        }
                        t += 1.0f / sliceCount;
                    }
                    offset += delta;
                }
                slices[minI, minS].Add(v);
            }

            {
                Vector3 offset = boundsFrom;
                for (int i = 0; i < curveIndices.Length; ++i)
                {

                    int curveIndex = curveIndices[i];
                    float t = 0.0f;
                    for (int s = 0; s <= sliceCount; ++s)
                    {
                        result[i * (sliceCount + 1) + s] = new Slice(Vector3.Lerp(offset, offset + delta, t), curveIndex, t, slices[i, s].ToArray());
                        t += 1.0f / sliceCount;
                    }
                    offset += delta;

                }
            }

            return result;
        }

        public void RecalculateNormals()
        {
            if(m_mesh != null)
            {
                m_mesh.RecalculateNormals();
            }

            if (m_colliderMesh != null && m_collider != null)
            {
                m_colliderMesh.RecalculateNormals();
                m_collider.sharedMesh = null;
                m_collider.sharedMesh = m_colliderMesh;
            }
        }

        public void Deform(MeshDeformer deformer, Mesh original, Mesh colliderOriginal,  bool isRigid)
        {
            if (original != null)
            {
                m_mesh.vertices = Deform(m_slices, original, deformer, isRigid);
                m_mesh.RecalculateBounds();
            }

            if(colliderOriginal != null && m_collider != null)
            {
                m_colliderMesh.vertices = Deform(m_colliderSlices, colliderOriginal, deformer, isRigid);
                m_colliderMesh.RecalculateBounds();
                m_collider.sharedMesh = null;
                m_collider.sharedMesh = m_colliderMesh;
            }
        }

        private Vector3[] Deform(Slice[] slices, Mesh mesh, MeshDeformer deformer, bool isRigid)
        {
            Vector3[] vertices = mesh.vertices;
            for (int s = 0; s < slices.Length; ++s)
            {
                Slice slice = slices[s];

                Vector3 center =  deformer.GetPoint(slice.T, slice.CurveIndex);
                center = deformer.transform.InverseTransformPoint(center);

                Vector3 dir = deformer.transform.InverseTransformVector(deformer.GetDirection(slice.T, slice.CurveIndex));
                float t = slice.T;
                if (isRigid)
                {
                    t = 1.0f;
                }

                float twistAngle = deformer.GetTwist(t, slice.CurveIndex);
                Vector3 thickness = deformer.GetThickness(t, slice.CurveIndex);

                if (dir == Vector3.zero)
                {
                    continue;
                }

           

                Quaternion rotation = Quaternion.AngleAxis(twistAngle, dir) * Quaternion.LookRotation(dir, m_up) * m_axisRotation;
                Matrix4x4 matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
                int[] indices = slice.Indices;
                for (int i = 0; i < indices.Length; ++i)
                {
                    int index = indices[i];
                    Vector3 vertex = vertices[index];
                    vertex = AxisTransform(deformer, vertex, slice.Center, thickness);
                    vertex = matrix.MultiplyPoint(vertex);
                    vertices[index] = vertex;
                }
            }
            return vertices;
        }

        public void SlerpContacts(MeshDeformer deformer, Mesh original, Mesh colliderOriginal, Scaffold prev, Scaffold next, bool isRigid)
        {
            if (isRigid)
            {
                return;
            }

            Mesh prevMesh = null;
            Mesh nextMesh = null;
            if(prev != null)
            {
                prevMesh = prev.m_mesh;
            }

            if(next != null)
            {
                nextMesh = next.m_mesh;
            }
            SlerpContacts(deformer, m_mesh, deformer.Contacts, prev, prevMesh, next, nextMesh);


            if(colliderOriginal == null)
            {
                return;
            }
            if (prev != null)
            {
                prevMesh = prev.m_colliderMesh;
            }

            if (next != null)
            {
                nextMesh = next.m_colliderMesh;
            }
            SlerpContacts(deformer, m_colliderMesh, deformer.ColliderContacts, prev, prevMesh, next, nextMesh);

            if(m_collider != null)
            {
                m_collider.sharedMesh = null;
                m_collider.sharedMesh = m_colliderMesh;
            }
        }

        private void SlerpContacts(MeshDeformer deformer, Mesh mesh, Contact[] contacts, Scaffold prev, Mesh prevMesh, Scaffold next, Mesh nextMesh)
        {
            Vector3[] normals = null;
            Vector3[] prevNormals = null;
            Vector3[] nextNormals = null;
            if(mesh == null)
            {
                return;
            }

            if (prev != null || next != null)
            {
                normals = mesh.normals;
            }

            if (prevMesh != null && prev != null && (prev != this || m_deformer.Scaffolds.Length == 1 && m_deformer.Loop))
            {
                prevNormals = prevMesh.normals;
                for (int i = 0; i < contacts.Length; ++i)
                {
                    Contact contact = contacts[i];
                    Vector3 prevNormal = prevNormals[contact.Index2];
                    Vector3 normal = normals[contact.Index1];
                    Vector3 slerped = Vector3.Slerp(prevNormal, normal, 0.5f);
                    prevNormals[contact.Index2] = slerped;
                    normals[contact.Index1] = slerped;
                }
            }

            if (nextMesh != null && next != null && (next != this || m_deformer.Scaffolds.Length == 1 && m_deformer.Loop))
            {

                nextNormals = nextMesh.normals;
                for (int i = 0; i < contacts.Length; ++i)
                {
                    Contact contact = contacts[i];
                    Vector3 normal = normals[contact.Index2];
                    Vector3 nextNormal = nextNormals[contact.Index1];
                    Vector3 slerped = Vector3.Slerp(normal, nextNormal, 0.5f);

                    normals[contact.Index2] = slerped;
                    nextNormals[contact.Index1] = slerped;
                }
            }

            if (prev != null)
            {
                if(mesh != null)
                {
                    mesh.normals = normals;
                }
                
                if (this != prev)
                {
                    if(prevMesh != null)
                    {
                        prevMesh.normals = prevNormals;
                    }
                    
                }

                if (next != null && next != prev)
                {
                    if(nextMesh != null)
                    {
                        nextMesh.normals = nextNormals;
                    }
                }
            }
            else if (next != null)
            {
                if (mesh != null)
                {
                    mesh.normals = normals;
                }

                if (prev != null && prev != next)
                {
                    if(prevMesh != null)
                    {
                        prevMesh.normals = prevNormals;
                    }
                }

                if (this != next)
                {
                    if(nextMesh != null)
                    {
                        nextMesh.normals = nextNormals;
                    }
                }
            }
        }

        private static Vector3 AxisTransform(MeshDeformer deformer, Vector3 vertex, Vector3 center, Vector3 scale)
        {
            Vector3 toVertex = vertex - center;
            if (deformer.Axis == Axis.X)
            {
                toVertex.x = 0;
                center.x = vertex.x - center.x;
            }
            else if (deformer.Axis == Axis.Y)
            {
                toVertex.y = 0;
                center.y = vertex.y - center.y;
            }
            else
            {
                toVertex.z = 0;
                center.z = vertex.z - center.z;
            }

            return center + Vector3.Scale(toVertex, scale);
        }

#if UNITY_EDITOR && MD_GIZMOS
        private void OnDrawGizmosSelected()
        {
            
            if (m_deformer == null)
            {
                return;
            }

            if (Selection.activeGameObject != gameObject)
            {
                return;
            }

            if(!m_deformer.DrawGizmos)
            {
                return;
            }
         
            Gizmos.color = Color.white;

            ScaffoldWrapper scaffold = m_deformer.Scaffolds.Where(s => s.Obj == this).FirstOrDefault();
            if (scaffold != null && !scaffold.IsEmptySpace)
            {
                if (scaffold.Obj != null)
                {
                    MeshFilter filter = scaffold.Obj.GetComponent<MeshFilter>();
                    if (filter != null)
                    {
                        Mesh mesh = filter.sharedMesh;
                        if (mesh != null)
                        {
                            Gizmos.matrix = m_deformer.transform.localToWorldMatrix;
                            Gizmos.DrawWireMesh(mesh);
                        }
                    }
                }
            }
        }
#endif

    }

}

