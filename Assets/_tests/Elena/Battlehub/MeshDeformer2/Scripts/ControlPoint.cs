using UnityEngine;

using Battlehub.SplineEditor;
namespace Battlehub.MeshDeformer2
{
    [ExecuteInEditMode]
    public class ControlPoint : SplineControlPoint
    {

#if UNITY_EDITOR && MD_GIZMOS
        private void OnDrawGizmosSelected()
        {
            MeshDeformer deformer = m_spline as MeshDeformer;
            if(deformer == null)
            {
                return;
            }

            if(Selection.activeGameObject != gameObject)
            {
                return;
            }

            if (!deformer.DrawGizmos)
            {
                return;
            }
            // Display the explosion radius when selected
            Gizmos.color = Color.white;
            
            ScaffoldWrapper scaffold = deformer.Scaffolds.Where(s => s.CurveIndices.Contains((Index - 1) / 3)).FirstOrDefault();
            if(scaffold != null && !scaffold.IsEmptySpace)
            {
                if(scaffold.Obj != null)
                {
                    MeshFilter filter = scaffold.Obj.GetComponent<MeshFilter>();
                    if(filter != null)
                    {
                        Mesh mesh = filter.sharedMesh;
                        if(mesh != null)
                        {
                            Gizmos.matrix = deformer.transform.localToWorldMatrix;
                            Gizmos.DrawWireMesh(mesh);
                        }
                    }
                }
            }  
        }
#endif
    }

}
