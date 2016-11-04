using UnityEngine;

using System.Collections.Generic;

namespace Battlehub.MeshDeformer2
{
    public class MeshSubdivider : MonoBehaviour
    {
        private static List<Vector3> vertices;
        private static List<Vector3> normals;
        private static List<Vector4> tangents;
        private static List<Color> colors;
        private static List<Vector2> uv;
        private static List<Vector2> uv2;
        private static List<Vector2> uv3;
        private static List<Vector2> uv4;

        private static List<int> indices;
        private static Dictionary<uint, int> newVectices;

        static void InitArrays(Mesh mesh)
        {
            vertices = new List<Vector3>(mesh.vertices);
            normals = new List<Vector3>(mesh.normals);
            tangents = new List<Vector4>(mesh.tangents);
            colors = new List<Color>(mesh.colors);
            uv = new List<Vector2>(mesh.uv);
            uv2 = new List<Vector2>(mesh.uv2);
            uv3 = new List<Vector2>(mesh.uv3);
            uv4 = new List<Vector2>(mesh.uv4);
            indices = new List<int>();
        }
        static void CleanUp()
        {
            vertices = null;
            normals = null;
            colors = null;
            uv = null;
            uv2 = null;
            uv3 = null;
            uv4 = null;
            indices = null;
        }

        #region Subdivide4 (2x2)
        static int GetNewVertex4(int i1, int i2)
        {
            int newIndex = vertices.Count;
            uint t1 = ((uint)i1 << 16) | (uint)i2;
            uint t2 = ((uint)i2 << 16) | (uint)i1;
            if (newVectices.ContainsKey(t2))
                return newVectices[t2];
            if (newVectices.ContainsKey(t1))
                return newVectices[t1];

            newVectices.Add(t1, newIndex);

            vertices.Add((vertices[i1] + vertices[i2]) * 0.5f);
            if (normals.Count > 0)
                normals.Add((normals[i1] + normals[i2]).normalized);
            if (tangents.Count > 0)
            {
                Vector4 newTangent = Vector4.Lerp(tangents[i1], tangents[i2], 0.5f).normalized;
                newTangent.w = Mathf.Lerp(tangents[i1].w, tangents[i2].w, 0.5f);
                tangents.Add(newTangent);
            }
            if (colors.Count > 0)
                colors.Add((colors[i1] + colors[i2]) * 0.5f);
            if (uv.Count > 0)
                uv.Add((uv[i1] + uv[i2]) * 0.5f);
            if (uv2.Count > 0)
                uv2.Add((uv2[i1] + uv2[i2]) * 0.5f);
            if (uv3.Count > 0)
                uv3.Add((uv3[i1] + uv3[i2]) * 0.5f);
            if (uv4.Count > 0)
                uv4.Add((uv4[i1] + uv4[i2]) * 0.5f);

            return newIndex;
        }

        /// <summary>
        /// Devides each triangles into 4. A quad(2 tris) will be splitted into 2x2 quads( 8 tris )
        /// </summary>
        /// <param name="mesh"></param>
        private static void Subdivide4Submesh(Mesh mesh)
        {
            newVectices = new Dictionary<uint, int>();

            InitArrays(mesh);

            int[] triangles = mesh.triangles;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                int i1 = triangles[i + 0];
                int i2 = triangles[i + 1];
                int i3 = triangles[i + 2];

                int a = GetNewVertex4(i1, i2);
                int b = GetNewVertex4(i2, i3);
                int c = GetNewVertex4(i3, i1);
                indices.Add(i1); indices.Add(a); indices.Add(c);
                indices.Add(i2); indices.Add(b); indices.Add(a);
                indices.Add(i3); indices.Add(c); indices.Add(b);
                indices.Add(a); indices.Add(b); indices.Add(c); // center triangle
            }
            mesh.vertices = vertices.ToArray();
            if (normals.Count > 0)
                mesh.normals = normals.ToArray();
            if (tangents.Count > 0)
                mesh.tangents = tangents.ToArray();
            if (colors.Count > 0)
                mesh.colors = colors.ToArray();
            if (uv.Count > 0)
                mesh.uv = uv.ToArray();
            if (uv2.Count > 0)
                mesh.uv2 = uv2.ToArray();
            if (uv3.Count > 0)
                mesh.uv3 = uv3.ToArray();
            if (uv4.Count > 0)
                mesh.uv4 = uv4.ToArray();

            mesh.triangles = indices.ToArray();

            CleanUp();
        }
        #endregion Subdivide4 (2x2)

        #region Subdivide9 (3x3)
        private static int GetNewVertex9(int i1, int i2, int i3)
        {
            int newIndex = vertices.Count;

            // center points don't go into the edge list
            if (i3 == i1 || i3 == i2)
            {
                uint t1 = ((uint)i1 << 16) | (uint)i2;
                if (newVectices.ContainsKey(t1))
                    return newVectices[t1];
                newVectices.Add(t1, newIndex);
            }

            // calculate new vertex
            vertices.Add((vertices[i1] + vertices[i2] + vertices[i3]) / 3.0f);
            if (normals.Count > 0)
                normals.Add((normals[i1] + normals[i2] + normals[i3]).normalized);
            if (tangents.Count > 0)
            {
                Vector4 newTangent = Vector4.Lerp(Vector4.Lerp(tangents[i1], tangents[i2], 0.5f), tangents[i3], 0.5f).normalized;
                newTangent.w = (tangents[i1].w + tangents[i2].w + tangents[i3].w) / 3.0f;
                tangents.Add(newTangent);
            }
            if (colors.Count > 0)
                colors.Add((colors[i1] + colors[i2] + colors[i3]) / 3.0f);
            if (uv.Count > 0)
                uv.Add((uv[i1] + uv[i2] + uv[i3]) / 3.0f);
            if (uv2.Count > 0)
                uv2.Add((uv2[i1] + uv2[i2] + uv2[i3]) / 3.0f);
            if (uv3.Count > 0)
                uv3.Add((uv3[i1] + uv3[i2] + uv3[i3]) / 3.0f);
            if (uv4.Count > 0)
                uv4.Add((uv4[i1] + uv4[i2] + uv4[i3]) / 3.0f);
            return newIndex;
        }


        /// <summary>
        /// Devides each triangles into 9. A quad(2 tris) will be splitted into 3x3 quads( 18 tris )
        /// </summary>
        /// <param name="mesh"></param>
        private static void Subdivide9Submesh(Mesh mesh)
        {
            newVectices = new Dictionary<uint, int>();

            InitArrays(mesh);

            int[] triangles = mesh.triangles;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                int i1 = triangles[i + 0];
                int i2 = triangles[i + 1];
                int i3 = triangles[i + 2];

                int a1 = GetNewVertex9(i1, i2, i1);
                int a2 = GetNewVertex9(i2, i1, i2);
                int b1 = GetNewVertex9(i2, i3, i2);
                int b2 = GetNewVertex9(i3, i2, i3);
                int c1 = GetNewVertex9(i3, i1, i3);
                int c2 = GetNewVertex9(i1, i3, i1);

                int d = GetNewVertex9(i1, i2, i3);

                indices.Add(i1); indices.Add(a1); indices.Add(c2);
                indices.Add(i2); indices.Add(b1); indices.Add(a2);
                indices.Add(i3); indices.Add(c1); indices.Add(b2);
                indices.Add(d); indices.Add(a1); indices.Add(a2);
                indices.Add(d); indices.Add(b1); indices.Add(b2);
                indices.Add(d); indices.Add(c1); indices.Add(c2);
                indices.Add(d); indices.Add(c2); indices.Add(a1);
                indices.Add(d); indices.Add(a2); indices.Add(b1);
                indices.Add(d); indices.Add(b2); indices.Add(c1);
            }

            mesh.vertices = vertices.ToArray();
            if (normals.Count > 0)
                mesh.normals = normals.ToArray();
            if (tangents.Count > 0)
                mesh.tangents = tangents.ToArray();
            if (colors.Count > 0)
                mesh.colors = colors.ToArray();
            if (uv.Count > 0)
                mesh.uv = uv.ToArray();
            if (uv2.Count > 0)
                mesh.uv2 = uv2.ToArray();
            if (uv3.Count > 0)
                mesh.uv3 = uv3.ToArray();
            if (uv4.Count > 0)
                mesh.uv4 = uv4.ToArray();

            mesh.triangles = indices.ToArray();

            CleanUp();
        }
        #endregion Subdivide9 (3x3)


        #region Subdivide
        /// <summary>
        /// This functions subdivides the mesh based on the level parameter
        /// Note that only the 4 and 9 subdivides are supported so only those divides
        /// are possible. [2,3,4,6,8,9,12,16,18,24,27,32,36,48,64, ...]
        /// The function tried to approximate the desired level 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="level">Should be a number made up of (2^x * 3^y)
        /// [2,3,4,6,8,9,12,16,18,24,27,32,36,48,64, ...]
        /// </param>
        private static void SubdivideSubmesh(Mesh mesh, int level)
        {
            if (level < 2)
                return;
            while (level > 1)
            {
                // remove prime factor 3
                while (level % 3 == 0)
                {
                    Subdivide9Submesh(mesh);
                    level /= 3;
                }
                // remove prime factor 2
                while (level % 2 == 0)
                {
                    Subdivide4Submesh(mesh);
                    level /= 2;
                }
                // try to approximate. All other primes are increased by one
                // so they can be processed
                if (level > 3)
                    level++;
            }
        }
        #endregion Subdivide

        private static Mesh Subdivide(Mesh originalMesh, System.Action<Mesh> subdivideAction)
        {
            if (originalMesh.subMeshCount == 1)
            {
                Mesh subdividedMesh = DuplicateMesh(originalMesh);
                subdivideAction(subdividedMesh);
                return subdividedMesh;
            }

            CombineInstance[] combineInstances = new CombineInstance[originalMesh.subMeshCount];
            for (int i = 0; i < originalMesh.subMeshCount; ++i)
            {
                CombineInstance inst = new CombineInstance();
                Mesh submesh = ExtractSubmesh(originalMesh, i);
                subdivideAction(submesh);
                inst.mesh = submesh;
                inst.transform = Matrix4x4.identity;
                inst.subMeshIndex = 0;
                combineInstances[i] = inst;
            }

            Mesh result = new Mesh();
            result.CombineMeshes(combineInstances, false);
            return result;
        }

        public static Mesh Subdivide4(Mesh originalMesh)
        {
            return Subdivide(originalMesh, m => Subdivide4Submesh(m));
        }

        public static Mesh Subdivide9(Mesh originalMesh)
        {
            return Subdivide(originalMesh, m => Subdivide9Submesh(m));
        }

        public static Mesh Subdivide(Mesh originalMesh, int level)
        {
            return Subdivide(originalMesh, m => SubdivideSubmesh(m, level));
        }

        public static Mesh DuplicateMesh(Mesh mesh)
        {
            return Instantiate(mesh);
        }

        public static Mesh ExtractSubmesh(Mesh mesh, int submeshIndex)
        {
            MeshTopology topology = mesh.GetTopology(submeshIndex);
            if (topology != MeshTopology.Triangles)
            {
                Debug.LogWarningFormat("Extract Submesh method could handle triangle topology only. Current topology is {0}. Mesh name {1} submeshIndex {2}", topology, mesh, submeshIndex);
                return mesh;
            }

            int[] triangles = mesh.GetTriangles(submeshIndex);
            int[] newTriangles = new int[triangles.Length];

            Dictionary<int, int> remapping = new Dictionary<int, int>();
            int newIndex = 0;
            for (int i = 0; i < triangles.Length; ++i)
            {
                int index = triangles[i];
                if (!remapping.ContainsKey(index))
                {
                    newTriangles[i] = newIndex;

                    remapping.Add(index, newIndex);
                    newIndex++;
                }
                else
                {
                    newTriangles[i] = remapping[index];
                }
            }

            Vector3[] vertices = mesh.vertices;
            Vector3[] newVertices = new Vector3[newIndex];
            foreach (KeyValuePair<int, int> kvp in remapping)
            {
                newVertices[kvp.Value] = vertices[kvp.Key];
            }

            Mesh result = new Mesh();
            result.vertices = newVertices;

            Color[] colors = mesh.colors;
            if (colors.Length == vertices.Length)
            {
                Color[] newColors = new Color[newIndex];
                foreach (KeyValuePair<int, int> kvp in remapping)
                {
                    newColors[kvp.Value] = colors[kvp.Key];
                }
                result.colors = newColors;
            }
            else
            {
                if (colors.Length != 0)
                {
                    Debug.LogWarning("colors.Length != vertices.Length");
                }
            }

            Color32[] colors32 = mesh.colors32;
            if (colors32.Length == vertices.Length)
            {
                Color32[] newColors32 = new Color32[newIndex];
                foreach (KeyValuePair<int, int> kvp in remapping)
                {
                    newColors32[kvp.Value] = colors32[kvp.Key];
                }
                result.colors32 = newColors32;
            }
            else
            {
                if (colors32.Length != 0)
                {
                    Debug.LogWarning("colors32.Length != vertices.Length");
                }
            }

            BoneWeight[] boneWeights = mesh.boneWeights;
            if (boneWeights.Length == vertices.Length)
            {
                BoneWeight[] newBoneWeights = new BoneWeight[newIndex];
                foreach (KeyValuePair<int, int> kvp in remapping)
                {
                    newBoneWeights[kvp.Value] = boneWeights[kvp.Key];
                }
                result.boneWeights = newBoneWeights;
            }
            else
            {
                if (boneWeights.Length != 0)
                {
                    Debug.LogWarning("boneWeights.Length != vertices.Length");
                }
            }

            Vector3[] normals = mesh.normals;
            if (normals.Length == vertices.Length)
            {
                Vector3[] newNormals = new Vector3[newIndex];
                foreach (KeyValuePair<int, int> kvp in remapping)
                {
                    newNormals[kvp.Value] = normals[kvp.Key];
                }
                result.normals = newNormals;
            }
            else
            {
                if (normals.Length != 0)
                {
                    Debug.LogWarning("normals.Length != vertices.Length");
                }
            }

            Vector4[] tangents = mesh.tangents;
            if (tangents.Length == vertices.Length)
            {
                Vector4[] newTangents = new Vector4[newIndex];
                foreach (KeyValuePair<int, int> kvp in remapping)
                {
                    newTangents[kvp.Value] = tangents[kvp.Key];
                }
                result.tangents = newTangents;
            }
            else
            {
                if (tangents.Length != 0)
                {
                    Debug.LogWarning("tangents.Length != vertices.Length");
                }
            }

            Vector2[] uv = mesh.uv;
            if (uv.Length == vertices.Length)
            {
                Vector2[] newUv = new Vector2[newIndex];
                foreach (KeyValuePair<int, int> kvp in remapping)
                {
                    newUv[kvp.Value] = uv[kvp.Key];
                }
                result.uv = newUv;
            }
            else
            {
                if (uv.Length != 0)
                {
                    Debug.LogWarning("uv.Length != vertices.Length");
                }
            }

            Vector2[] uv2 = mesh.uv2;
            if (uv2.Length == vertices.Length)
            {
                Vector2[] newUv2 = new Vector2[newIndex];
                foreach (KeyValuePair<int, int> kvp in remapping)
                {
                    newUv2[kvp.Value] = uv2[kvp.Key];
                }
                result.uv2 = newUv2;
            }
            else
            {
                if (uv2.Length != 0)
                {
                    Debug.LogWarning("uv2.Length != vertices.Length");
                }
            }

            Vector2[] uv3 = mesh.uv3;
            if (uv3.Length == vertices.Length)
            {
                Vector2[] newUv3 = new Vector2[newIndex];
                foreach (KeyValuePair<int, int> kvp in remapping)
                {
                    newUv3[kvp.Value] = uv3[kvp.Key];
                }
                result.uv3 = newUv3;
            }
            else
            {
                if (uv3.Length != 0)
                {
                    Debug.LogWarning("uv3.Length != vertices.Length");
                }
            }

            Vector2[] uv4 = mesh.uv4;
            if (uv4.Length == vertices.Length)
            {
                Vector2[] newUv4 = new Vector2[newIndex];
                foreach (KeyValuePair<int, int> kvp in remapping)
                {
                    newUv4[kvp.Value] = uv4[kvp.Key];
                }
                result.uv4 = newUv4;
            }
            else
            {
                if (uv4.Length != 0)
                {
                    Debug.LogWarning("uv4.Length != vertices.Length");
                }
            }

            result.triangles = newTriangles;
            return result;
        }
    }
}
