using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
namespace Battlehub.MeshDeformer2
{
    public class MeshDeformerAssetPrcessor : AssetPostprocessor
    {

        private void OnPostprocessModel(GameObject g)
        {
            if (g == null)
            {
                return;
            }

            MeshFilter[] filters = g.GetComponentsInChildren<MeshFilter>();
            if (filters == null)
            {
                return;
            }

            HashSet<string> hs = new HashSet<string>();
            foreach (MeshFilter filter in filters)
            {
                if (filter == null)
                {
                    continue;
                }

                Mesh mesh = filter.sharedMesh;
                if (mesh == null)
                {
                    continue;
                }

                if (!hs.Contains(mesh.name))
                {
                    hs.Add(mesh.name);
                }
            }

            MeshDeformer[] deformers = GameObject.FindObjectsOfType<MeshDeformer>();
            foreach (MeshDeformer deformer in deformers)
            {
                if (deformer.Original != null)
                {
                    if (hs.Contains(deformer.Original.name))
                    {
                        deformer.Internal_HasChanged = true;
                        Debug.Log("Rebuild deformer");
                    }
                }

                if (deformer.ColliderOriginal != null)
                {
                    if (hs.Contains(deformer.ColliderOriginal.name))
                    {
                        deformer.Internal_HasChanged = true;
                        Debug.Log("Rebuild deformer");
                    }
                }
            }

        }

    }
}

