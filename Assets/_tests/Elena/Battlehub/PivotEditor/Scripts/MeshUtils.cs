using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Battlehub.MeshTools
{
    public static partial class MeshUtils
    {
        public static Vector3 CenterOfMass(GameObject gameObject)
        {
            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            Vector3 centerOfMass = CenterOfMass(meshFilter.sharedMesh);
            return gameObject.transform.TransformPoint(centerOfMass);
        }

        public static Vector3 CenterOfMass(Mesh mesh)
        {
            Vector3[] vertices = mesh.vertices;
            int[] tris = mesh.triangles;
            float totalVol = 0;
            float currentVol;
            float xCenter = 0, yCenter = 0, zCenter = 0;

            for (int i = 0; i < tris.Length; i += 3)
            {
                Vector3 v1 = vertices[tris[i]];
                Vector3 v2 = vertices[tris[i + 1]];
                Vector3 v3 = vertices[tris[i + 2]];

                totalVol += currentVol = (v1.x * v2.y * v3.z - v1.x * v3.y * v2.z - v2.x * v1.y * v3.z + v2.x * v3.y * v1.z + v3.x * v1.y * v2.z - v3.x * v2.y * v1.z) / 6;
                xCenter += ((v1.x + v2.x + v3.x) / 4) * currentVol;
                yCenter += ((v1.y + v2.y + v3.y) / 4) * currentVol;
                zCenter += ((v1.z + v2.z + v3.z) / 4) * currentVol;
            }

            if (totalVol == 0)
            {
                return Vector3.zero;
            }

            return new Vector3(xCenter / totalVol, yCenter / totalVol, zCenter / totalVol);
        }

        public static Vector3 BoundsCenter(GameObject gameObject)
        {
            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            Vector3 boundsCenter = BoundsCenter(meshFilter.sharedMesh);
            return gameObject.transform.TransformPoint(boundsCenter);
        }

        public static Vector3 BoundsCenter(Mesh mesh)
        {
            mesh.RecalculateBounds();
            return mesh.bounds.center;
        }

        

#if UNITY_EDITOR
        public static void SaveMesh(GameObject[] selectedObjects, string root)
        {
            for (int i = 0; i < selectedObjects.Length; ++i)
            {
                GameObject selected = selectedObjects[i];

                MeshFilter meshFilter = selected.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    SaveMesh(meshFilter.sharedMesh, root, selected.name);
                }

                SkinnedMeshRenderer smr = selected.GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    SaveMesh(smr.sharedMesh, root, selected.name + " skinned");
                }
            }
        }

        public static void SaveMesh(Mesh mesh, string root, string name)
        {
            if (!System.IO.Directory.Exists(Application.dataPath + "/" + root + "/SavedMeshes"))
            {
                AssetDatabase.CreateFolder("Assets/" + root.Remove(root.Length - 1), "SavedMeshes");
            }


            string path = "Assets/" + root + "SavedMeshes/" + name + ".prefab";
            if (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(mesh)))
            {
                AssetDatabase.CreateAsset(mesh, AssetDatabase.GenerateUniqueAssetPath(path));
                Debug.Log("Mesh saved: " + path);
            }

            AssetDatabase.SaveAssets();
        }
#endif

        /// <summary>
        /// Set mesh pivot
        /// </summary>
        /// <param name="mesh">mesh</param>
        /// <param name="position">pivot local position</param>
        /// <returns></returns>
        public static Mesh SetPivot(Mesh mesh, Vector3 position)
        {
            Vector3 center = mesh.bounds.center;
            Vector3 offset = position - center;
            return EditPivot(mesh, offset);
        }

        /// <summary>
        /// Set mesh pivot and alter object transform
        /// </summary>
        /// <param name="transform">transform</param>
        /// <param name="mesh">mesh</param>
        /// <param name="position">pivot local position</param>
        /// <returns></returns>
        public static Mesh SetPivot(Transform transform, Mesh mesh, Vector3 position)
        {
            Vector3 center = mesh.bounds.center;
            Vector3 offset = position - center;
            transform.position -= offset;
            return EditPivot(mesh, offset);
        }


        /// <summary>
        /// Edit mesh pivot
        /// </summary>
        /// <param name="mesh">mesh</param>
        /// <param name="offset">pivot local offset</param>
        /// <returns></returns>
        public static Mesh EditPivot(Mesh mesh, Vector3 offset)
        {
            Mesh[] meshes = Separate(mesh);
            CombineInstance[] combine = new CombineInstance[meshes.Length];
            for (int i = 0; i < meshes.Length; ++i)
            {
                Mesh separatedMesh = meshes[i];
                combine[i].mesh = separatedMesh;
                combine[i].subMeshIndex = 0;
                combine[i].transform = Matrix4x4.TRS(offset, Quaternion.identity, Vector3.one);
            }

            Mesh result = new Mesh();
            result.name = mesh.name;
            result.CombineMeshes(combine, mesh.subMeshCount <= 1);
            return result;
        }

        /// <summary>
        /// Edit Pivot
        /// </summary>
        /// <param name="transform">object transform</param>
        /// <param name="worldOffset">pivat offset in world coordinates</param>
        /// <param name="collider">optionally object MeshCollider</param>
        public static Mesh EditPivot(Transform transform, Vector3 worldOffset, Collider[] colliders)
        {
            Vector3 offset = transform.InverseTransformDirection(worldOffset);
            MeshFilter filter = transform.gameObject.GetComponent<MeshFilter>();
            filter.sharedMesh = EditPivot(filter.sharedMesh, offset);

            for (int i = 0; i < colliders.Length; ++i)
            {
                Collider collider = colliders[i];

                if (collider != null)
                {
                    if (collider is MeshCollider)
                    {
                        MeshCollider meshCollider = (MeshCollider)collider;
                        meshCollider.sharedMesh = EditPivot(meshCollider.sharedMesh, offset);
                    }
                    else if (collider is BoxCollider)
                    {
                        BoxCollider boxCollider = (BoxCollider)collider;
                        boxCollider.center += offset;
                    }
                    else if (collider is SphereCollider)
                    {
                        SphereCollider sphereCollider = (SphereCollider)collider;
                        sphereCollider.center += offset;
                    }
                    else if (collider is CapsuleCollider)
                    {
                        CapsuleCollider capsuleCollider = (CapsuleCollider)collider;
                        capsuleCollider.center += offset;
                    }

                }
            }

            return filter.sharedMesh;
        }

        public static Mesh[] Separate(Mesh mesh)
        {
            if (mesh.subMeshCount < 2)
            {
                return new[] { mesh };
            }

            Mesh[] result = new Mesh[mesh.subMeshCount];
            for (int i = 0; i < mesh.subMeshCount; ++i)
            {
                result[i] = ExtractSubmesh(mesh, i);
            }

            return result;
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
