using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Battlehub.MeshTools
{
    public class MeshTransform
    {
        public Mesh Mesh;
        public Matrix4x4 Transform;

        public MeshTransform(Mesh mesh, Matrix4x4 transform)
        {
            Mesh = mesh;
            Transform = transform;
        }
    }

    public class CombineResult
    {
        public GameObject GameObject;
        public Mesh Mesh;

        public CombineResult(GameObject gameObject, Mesh mesh)
        {
            GameObject = gameObject;
            Mesh = mesh;
        }
    }

    public static partial class MeshUtils 
    {
      
        /// <summary>
        /// Combine meshes of gameobjects
        /// </summary>
        /// <param name="gameObjects">game objects array</param>
        /// <param name="target">Combine method will assign combined mesh to this game object. If not specified then first object from game objects array will be used as a target</param>
        /// <param name="centerPivot">move pivot to BoundingBox.center</param>
        public static CombineResult Combine(GameObject[] gameObjects, GameObject target = null)
        {
            if (gameObjects == null)
            {
                throw new System.ArgumentNullException("gameObjects");
            }

            if (gameObjects.Length == 0)
            {
                return null;
            }

            if (target == null)
            {
                target = gameObjects[0];
            }

            //save parents and unparent selected objects
            Transform[] selectionParents = new Transform[gameObjects.Length];
            for (int i = 0; i < gameObjects.Length; ++i)
            {
                GameObject obj = gameObjects[i];
                selectionParents[i] = obj.transform.parent;
                obj.transform.SetParent(null, true);
            }


            //duplicate target and remove it's children
            GameObject targetClone = Object.Instantiate(target);
            DestroyChildren(targetClone);

            //destroy it's colliders
            Collider[] targetCloneColliders = targetClone.GetComponents<Collider>();
            foreach (Collider collider in targetCloneColliders)
            {
                if (Application.isPlaying)
                {
                    GameObject.Destroy(collider);
                }
                else
                {
                    GameObject.DestroyImmediate(collider);
                }

            }

            Matrix4x4 targetRotationMatrix = Matrix4x4.TRS(Vector3.zero, target.transform.rotation, target.transform.localScale);
            Matrix4x4 targetToLocal = targetClone.transform.worldToLocalMatrix;


            //find all MeshFilters and SkinnedMeshRenderers
            List<MeshFilter> allMeshFilters = new List<MeshFilter>();
            List<SkinnedMeshRenderer> allSkinned = new List<SkinnedMeshRenderer>();

            foreach (GameObject obj in gameObjects)
            {
                MeshFilter[] filters = obj.GetComponents<MeshFilter>();
                allMeshFilters.AddRange(filters);

                SkinnedMeshRenderer[] skinned = obj.GetComponents<SkinnedMeshRenderer>();
                allSkinned.AddRange(skinned);
            }




#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(target, "SetActive(false)");
#endif
            //deactivate original object                  
            target.SetActive(false);

            //combine colliders
            List<CombineInstance> colliderCombine = new List<CombineInstance>();

            foreach (GameObject obj in gameObjects)
            {
                List<Mesh> meshes = GetColliderMeshes(obj);
                for (int i = 0; i < meshes.Count; i++)
                {
                    Mesh mesh = meshes[i];
                    CombineInstance colliderCombineInstance = new CombineInstance();
                    colliderCombineInstance.mesh = mesh;
                    //convert to active selected object's local coordinate system
                    colliderCombineInstance.transform = targetToLocal * obj.transform.localToWorldMatrix;
                    colliderCombine.Add(colliderCombineInstance);
                }
            }

            //copy original object name
            string name = target.name;
            targetClone.name = name;

            if(colliderCombine.Count != 0)
            {
                Mesh colliderMesh = new Mesh();
                colliderMesh.CombineMeshes(colliderCombine.ToArray());

                CombineInstance[] removeColliderRotation = new CombineInstance[1];
                removeColliderRotation[0].mesh = colliderMesh;
                removeColliderRotation[0].transform = targetRotationMatrix;

                Mesh finalColliderMesh = new Mesh();
                finalColliderMesh.name = name + "Collider";
                finalColliderMesh.CombineMeshes(removeColliderRotation);

                MeshCollider targetCollider = targetClone.AddComponent<MeshCollider>();
                Rigidbody rigidBody = targetClone.GetComponent<Rigidbody>();
                if (rigidBody != null)
                {
                    targetCollider.sharedMesh = finalColliderMesh;
                    if (!rigidBody.isKinematic)
                    {
                        targetCollider.convex = true;
                    }
                }
                else
                {
                    targetCollider.sharedMesh = finalColliderMesh;
                }
            }
            

            CombineInstance[] meshCombine;
            Material[] materials;
            bool merge = BuildCombineInstance(targetToLocal, allMeshFilters, allSkinned, out meshCombine, out materials);
            Mesh intermediateMesh = new Mesh();
            intermediateMesh.name = name;
            intermediateMesh.CombineMeshes(meshCombine, merge);

            //then remove rotation
            Mesh finalMesh = RemoveRotation(intermediateMesh, targetRotationMatrix, merge);
            finalMesh.name = name;

            targetClone.transform.rotation = Quaternion.identity;
            targetClone.transform.localScale = Vector3.one;



#if UNITY_EDITOR
            UnityEditor.Selection.activeGameObject = targetClone;
#endif

            for (int i = 0; i < gameObjects.Length; ++i)
            {
                GameObject obj = gameObjects[i];
                obj.transform.SetParent(selectionParents[i], true);

#if UNITY_EDITOR
                UnityEditor.Undo.RecordObject(obj, "SetActive(false)");
#endif
                obj.SetActive(false);
            }

            //restore parents
            if (target.transform.parent != null && target.transform.parent.gameObject.activeInHierarchy)
            {
                targetClone.transform.SetParent(target.transform.parent);
            }
#if UNITY_EDITOR
            UnityEditor.Undo.RegisterCreatedObjectUndo(targetClone, "SelectedObjCopy");
#endif
            SkinnedMeshRenderer skinnedMeshRenderer = targetClone.GetComponent<SkinnedMeshRenderer>();
            if(skinnedMeshRenderer != null)
            {
                if (Application.isPlaying)
                {
                    GameObject.Destroy(skinnedMeshRenderer);
                }
                else
                {
                    GameObject.DestroyImmediate(skinnedMeshRenderer);
                }
            }

            MeshFilter meshFiter = targetClone.GetComponent<MeshFilter>();
            if (meshFiter == null)
            {
                meshFiter = targetClone.AddComponent<MeshFilter>();
            }
            meshFiter.sharedMesh = finalMesh;

            MeshRenderer meshRenderer = targetClone.GetComponent<MeshRenderer>();
            if (meshRenderer == null)
            {
                meshRenderer = targetClone.AddComponent<MeshRenderer>();
            }
            meshRenderer.sharedMaterials = materials;


            return new CombineResult(targetClone, finalMesh);
        }

        private static Mesh RemoveRotation(Mesh mesh, Matrix4x4 targetRotationMatrix, bool merge)
        {
            Mesh[] meshes = MeshUtils.Separate(mesh);
            CombineInstance[] combineInstances = new CombineInstance[meshes.Length];
            for (int i = 0; i < meshes.Length; ++i)
            {
                CombineInstance removeRotation = new CombineInstance();
                removeRotation.mesh = meshes[i];
                removeRotation.subMeshIndex = 0;
                removeRotation.transform = targetRotationMatrix;
                combineInstances[i] = removeRotation;
            }

            //got final mesh without rotation
            mesh = new Mesh();
            mesh.CombineMeshes(combineInstances, merge);
            mesh.RecalculateBounds();
            return mesh;
        }

        private static bool BuildCombineInstance(Matrix4x4 targetToLocal, List<MeshFilter> allMeshFilters, List<SkinnedMeshRenderer> allSkinned,
            out CombineInstance[] combineInstances,
            out Material[] materials)
        {
            bool merge = true;
            Dictionary<Material, List<MeshTransform>> meshGroups = new Dictionary<Material, List<MeshTransform>>();
            for (int i = 0; i < allMeshFilters.Count; ++i)
            {
                MeshFilter meshFilter = allMeshFilters[i];
                PopulateMeshGroups(meshGroups, meshFilter.gameObject, meshFilter.sharedMesh);
            }
            for (int i = 0; i < allSkinned.Count; ++i)
            {
                PopulateMeshGroups(meshGroups, allSkinned[i].gameObject, allSkinned[i].sharedMesh);
            }

            const int VERTEX_LIMIT = 65534;
            List<Material> resultMaterials = new List<Material>(meshGroups.Count);
            List<CombineInstance> resultCombineInstances = new List<CombineInstance>(allMeshFilters.Count + allSkinned.Count);

            foreach (KeyValuePair<Material, List<MeshTransform>> kvp in meshGroups)
            {
                List<MeshTransform> group = kvp.Value;
                List<List<CombineInstance>> listOfGroupInstances = new List<List<CombineInstance>>();
                List<CombineInstance> groupCombineInstances = new List<CombineInstance>();
                int groupIndex = 0;
                int vertexCount = 0;
                while (groupIndex < group.Count)
                {
                    MeshTransform meshTransform = group[groupIndex];
                    if (meshTransform.Mesh.subMeshCount > 1)
                    {
                        merge = false;
                    }
                    vertexCount += meshTransform.Mesh.vertexCount;
                    if (vertexCount > VERTEX_LIMIT && listOfGroupInstances.Count > 0)
                    {
                        listOfGroupInstances.Add(groupCombineInstances);
                        groupCombineInstances = new List<CombineInstance>();
                    }

                    CombineInstance groupCombineInstance = new CombineInstance();
                    groupCombineInstance.mesh = meshTransform.Mesh;
                    groupCombineInstance.transform = targetToLocal * meshTransform.Transform;
                    groupCombineInstances.Add(groupCombineInstance);

                    groupIndex++;
                }

                listOfGroupInstances.Add(groupCombineInstances);

                for (int i = 0; i < listOfGroupInstances.Count; ++i)
                {
                    List<CombineInstance> groupInstances = listOfGroupInstances[i];

                    Mesh mesh = new Mesh();
                    mesh.CombineMeshes(groupInstances.ToArray(), true);

                    resultMaterials.Add(kvp.Key);
                    resultCombineInstances.Add(new CombineInstance { mesh = mesh, transform = Matrix4x4.identity });
                }
            }

            combineInstances = resultCombineInstances.ToArray();
            materials = resultMaterials.ToArray();
            return merge && materials.Length <= 1;
        }

        private static void PopulateMeshGroups(Dictionary<Material, List<MeshTransform>> meshGroups, GameObject go, Mesh mesh)
        {
            Mesh[] meshes = MeshUtils.Separate(mesh);
            Material[] rendererMaterials;
            Renderer renderer = go.GetComponent<Renderer>();
            if (renderer == null)
            {
                rendererMaterials = new Material[meshes.Length];
            }
            else
            {
                rendererMaterials = renderer.sharedMaterials;
                System.Array.Resize(ref rendererMaterials, meshes.Length);
            }

            for (int m = 0; m < rendererMaterials.Length; ++m)
            {
                Material material = rendererMaterials[m];
                if (!meshGroups.ContainsKey(material))
                {
                    meshGroups.Add(material, new List<MeshTransform>());
                }

                List<MeshTransform> group = meshGroups[material];
                group.Add(new MeshTransform(meshes[m], go.transform.localToWorldMatrix));
            }
        }

        private static void DestroyChildren(GameObject gameObject)
        {
            if (Application.isPlaying)
            {
                foreach (Transform child in gameObject.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
            else
            {
                GameObject[] children = new GameObject[gameObject.transform.childCount];
                int index = 0;
                foreach (Transform child in gameObject.transform)
                {
                    children[index] = child.gameObject;
                    index++;
                }
                for (int i = 0; i < children.Length; ++i)
                {
                    GameObject.DestroyImmediate(children[i]);
                }
            }

        }

        private static List<Mesh> GetColliderMeshes(GameObject obj)
        {
            List<Mesh> meshes = new List<Mesh>();
            Collider[] allColliders = obj.GetComponents<Collider>();

            if (allColliders.Length == 0)
            {
                MeshFilter f = obj.GetComponent<MeshFilter>();
                if (f != null)
                {
                    //UNCOMMENT TO INCLUDE OBJECT MESH TO COLLIDER
                    //meshes.AddRange(Separate(f.sharedMesh));
                }
            }
            else
            {
                for (int i = 0; i < allColliders.Length; ++i)
                {
                    MeshCollider meshCollider = allColliders[i] as MeshCollider;
                    if (meshCollider != null)
                    {
                        meshes.AddRange(MeshUtils.Separate(meshCollider.sharedMesh));
                    }
                    else
                    {
                        //Use object's mesh if there is no meshcollider
                        MeshFilter f = obj.GetComponent<MeshFilter>();
                        if (f != null)
                        {
                            meshes.AddRange(MeshUtils.Separate(f.sharedMesh));
                        }

                    }
                }
            }

            return meshes;
        }

        /*
      private static bool CombineBones(GameObject target, List<SkinnedMeshRenderer> skinnedRenderers)
      {
          bool hasBones = false;
          for(int i = 0; i < skinnedRenderers.Count; ++i)
          {
              SkinnedMeshRenderer renderer = skinnedRenderers[i];
              if (renderer.gameObject == target)
              {
                  continue;
              }
              if (renderer.bones.Length > 0)
              {
                  hasBones = true;
                  break;
              }
          }

          if(!hasBones)
          {
              return false;
          }

          SkinnedMeshRenderer targetRenderer = target.GetComponent<SkinnedMeshRenderer>();
          if(targetRenderer == null)
          {
              throw new System.ArgumentException("SkinnedMeshRenderer component is required", "target");
          }

          Transform rootBone = targetRenderer.rootBone;

          if(rootBone == null)
          {
              for (int i = 0; i < skinnedRenderers.Count; ++i)
              {
                  SkinnedMeshRenderer renderer = skinnedRenderers[i];
                  if(renderer.gameObject == target)
                  {
                      continue;
                  }
                  if(renderer.rootBone != null)
                  {
                      rootBone = renderer.rootBone;
                      break;
                  }
              }

              if(rootBone == null)
              {
                  Debug.LogWarning("Unable to find root bone");
                  return false;
              }

              if (rootBone.transform == targetRenderer.transform)
              {
                  Debug.LogWarning("rootBone.transform == targetRenderer.transform");
                  return false;
              }
          }

          Transform rootBoneClone = ((GameObject)Object.Instantiate(rootBone.gameObject, rootBone.transform.position, rootBone.transform.rotation)).transform;
          targetRenderer.rootBone = rootBoneClone;
          rootBoneClone.SetParent(targetRenderer.transform, true);

          List<Transform> boneClones = new List<Transform>();
          for (int i = 0; i < skinnedRenderers.Count; ++i)
          {
              SkinnedMeshRenderer renderer = skinnedRenderers[i];
              if (renderer.gameObject == target)
              {
                  continue;
              }
              if (renderer.rootBone == null)
              {
                  Debug.LogWarning(renderer.gameObject.name + " root bone is null.");
                  Transform[] bones = renderer.bones;
                  for (int b = 0; b < bones.Length; ++b)
                  {
                      Transform bone = bones[b];
                      Transform boneClone = ((GameObject)Object.Instantiate(bone.gameObject, bone.position, bone.rotation)).transform;
                      boneClone.SetParent(rootBoneClone, true);
                      boneClones.Add(boneClone);
                  }
              }
              else
              {
                  Transform[] bones = renderer.bones;
                  for (int b = 0; b < bones.Length; ++b)
                  {
                      Transform bone = bones[b];
                      if (bone == rootBone)
                      {
                          boneClones.Add(rootBone);
                      }
                      else 
                      {
                          Transform boneClone = ((GameObject)Object.Instantiate(bone.gameObject, bone.position, bone.rotation)).transform;
                          if (bone == renderer.rootBone)
                          {
                              boneClone.SetParent(rootBoneClone, true);
                          }
                          boneClones.Add(rootBoneClone);
                      }
                  }
              }
          }

          targetRenderer.bones = boneClones.ToArray();
          return true;
      }
      */
    }

}
