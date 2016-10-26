using UnityEngine;
using System.Linq;
using Battlehub.MeshDeformer2;
namespace Battlehub.Wire2
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Scaffold))]
    public class Wire : MeshDeformer
    {
        [SerializeField]
        [HideInInspector]
        private int m_sectorsCount = 4;

        [SerializeField]
        [HideInInspector]
        private float m_wireRadius = 0.075f;

        public override int Approximation
        {
            get { return base.Approximation; }
            set
            {
                Original = new WireGen().Generate(m_sectorsCount / 2 * 2, value, m_wireRadius, 0.5f);
                base.Approximation = value;
            }
        }

        public int SectorsCount
        {
            get { return m_sectorsCount; }
            set
            {
                m_sectorsCount = value;
                if(m_sectorsCount < 4)
                {
                    m_sectorsCount = 4;
                }
           
                Original = new WireGen().Generate(m_sectorsCount / 2 * 2, Approximation, m_wireRadius, 0.5f);
                WrapAndDeformAll();
            }
        }

        public float WireRadius
        {
            get { return m_wireRadius; }
            set
            {
                m_wireRadius = value;
                Original = new WireGen().Generate(m_sectorsCount / 2 * 2, Approximation, m_wireRadius, 0.5f);
                WrapAndDeformAll();
            }
        }

        protected override void AwakeOverride()
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            if(meshFilter == null)
            {
                meshFilter = gameObject.AddComponent<MeshFilter>();
            }

            if(meshFilter.sharedMesh == null)
            {
               meshFilter.sharedMesh = new WireGen().Generate(m_sectorsCount / 2 * 2, Approximation, m_wireRadius, 0.5f);
            }
        }

        protected override void ResetOverride()
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            if (meshFilter == null)
            {
                meshFilter = gameObject.AddComponent<MeshFilter>();
            }

            if (meshFilter.sharedMesh == null)
            {
                meshFilter.sharedMesh = new WireGen().Generate(m_sectorsCount / 2 * 2, Approximation, m_wireRadius, 0.5f);
            }
            base.ResetOverride();
          
        }
    }

}
