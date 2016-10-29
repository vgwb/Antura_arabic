using UnityEngine;
using System.Collections.Generic;
using System;

namespace Battlehub.MeshDeformer2
{
    public enum Axis
    {
        X,
        Y,
        Z
    }

    [Serializable]
    public struct Contact
    {
        public int Index1;
        public int Index2;

        public Contact(int index1, int index2)
        {
            Index1 = index1;
            Index2 = index2;
        }
    }

    public class Vector3ToHash
    {
        private int m_hashCode;
        private Vector3 m_v;

   
        public Vector3ToHash(Vector3 v)
        {
            m_v = v;
            m_hashCode = new
            {
                x = Math.Round(v.x, 4),
                y = Math.Round(v.y, 4),
                z = Math.Round(v.z, 4), 
            }.GetHashCode();
        }

        public override int GetHashCode()
        {
            return m_hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Vector3ToHash other = (Vector3ToHash)obj;
            return other.m_v == m_v;
        }
    }


    public static class MeshExtensions
    {
        public static Contact[] FindContacts(this Mesh mesh, Axis axis, float threshold = 0.999f)
        {
            Vector3 from;
            Vector3 to;
            mesh.GetBounds(axis, out from, out to);
            return mesh.FindContacts(from, to, axis, threshold);
        }

        public static Contact[] FindContacts(this Mesh mesh, Vector3 from, Vector3 to, Axis axis, float threshold = 0.999f)
        {
            Vector3[] vertices = mesh.vertices;
            Vector3[] normals = mesh.normals;

            Dictionary<Vector3ToHash, List<int>> vertexToIndices = new Dictionary<Vector3ToHash, List<int>>(vertices.Length);
            for (int i = 0; i < vertices.Length; ++i)
            {
                Vector3 vertex = vertices[i];
                Vector3ToHash vToHash = new Vector3ToHash(vertex);
                if (!vertexToIndices.ContainsKey(vToHash))
                {
                    vertexToIndices.Add(vToHash, new List<int>());
                }

                List<int> indices = vertexToIndices[vToHash];
                indices.Add(i);
            }

            List<Contact> result = new List<Contact>();
          

            Vector3 offset = to - from;
            Matrix4x4 matrix = Matrix4x4.TRS(offset, Quaternion.identity, Vector3.one);

            for (int i1 = 0; i1 < vertices.Length; ++i1)
            {
                Vector3 norm1 = normals[i1];

                Vector3 vertex = matrix.MultiplyPoint(vertices[i1]);
                Vector3ToHash vToHash = new Vector3ToHash(vertex);
                if(vertexToIndices.ContainsKey(vToHash))
                {
                    List<int> indices = vertexToIndices[vToHash];
                    for(int i = 0; i < indices.Count; ++i)
                    {
                        int i2 = indices[i];
                        Vector3 norm2 = normals[i2];
                        if(Vector3.Dot(norm1, norm2) > threshold)
                        {
                            result.Add(new Contact(i1, i2));
                        }
                    }
                }
            }

            return result.ToArray();
        }

        public static float GetSize(this Mesh mesh, Axis axis)
        {
            Vector3 from;
            Vector3 to;

            mesh.GetBounds(axis, out from, out to);
            return (to - from).magnitude;
        }

        public static void GetBounds(this Mesh mesh, Axis axis, out Vector3 from, out Vector3 to)
        {
            if(mesh == null)
            {
                from = new Vector3(-0.5f, -0.5f, -0.5f);
                to = new Vector3(0.5f, 0.5f, 0.5f);
            }
            else
            {
                Bounds bounds = mesh.bounds;
                from = bounds.center - bounds.extents;
                to = bounds.center + bounds.extents;
            }

            if (axis == Axis.X)
            {
                from.y = to.y = 0.0f;
                from.z = to.z = 0.0f;
            }
            else if (axis == Axis.Y)
            {
                from.x = to.x = 0.0f;
                from.z = to.z = 0.0f;
            }
            else if (axis == Axis.Z)
            {
                from.x = to.x = 0.0f;
                from.y = to.y = 0.0f;
            }
        }

    }
}
