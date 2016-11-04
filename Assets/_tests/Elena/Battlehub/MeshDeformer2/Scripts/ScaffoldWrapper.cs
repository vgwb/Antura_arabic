using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using Battlehub.SplineEditor;

namespace Battlehub.MeshDeformer2
{
    [Serializable]
    public class ScaffoldWrapper 
    {
        [SerializeField]
        private bool m_isRigid;

        [SerializeField]
        private int[] m_curveIndices;

        [SerializeField]    
        private Scaffold m_scaffold;

        [SerializeField]
        private int m_instanceId;

        [SerializeField]
        private bool m_isEmptySpace;

        public Scaffold Scaffold
        {
            get { return m_scaffold; }
        }

        public bool IsRigid
        {
            get { return m_isRigid; }
            set { m_isRigid = value; }
        }

        public int[] CurveIndices
        {
            get { return m_curveIndices; }
            set { m_curveIndices = value; }
        }

        public int SliceCount
        {
            get { return m_scaffold == null ? 0 : m_scaffold.SliceCount; }
        }

        public int ObjInstanceId
        {
            get { return m_instanceId; }
        }

        public Scaffold Obj
        {
            get { return m_scaffold; }
            set { m_scaffold = value;  }
        }

        public bool IsEmptySpace
        {
            get { return m_isEmptySpace; }
        }

        public ScaffoldWrapper()
        {

        }

        public ScaffoldWrapper(Scaffold scaffold, bool isRigid)
        {
            m_isRigid = isRigid;
            if (scaffold == null)
            {
                m_isEmptySpace = true;
            }
            else
            {
                m_scaffold = scaffold;
                m_instanceId = scaffold.GetInstanceID();
            }
        }

        public void Wrap(Mesh mesh, Mesh colliderMesh, Axis axis, int[] curveIndices, int sliceCount)
        {
            m_curveIndices = curveIndices;

            if (m_scaffold != null)
            {
                m_scaffold.Wrap(mesh, colliderMesh, axis, curveIndices, sliceCount);
            }
        }

        public void Shift(int delta)
        {
            for (int i = 0; i < m_curveIndices.Length; ++i)
            {
                m_curveIndices[i] += delta;
            }

            if (m_scaffold != null)
            {
                m_scaffold.Shift(delta);
            }
        }

        public void RecalculateNormals()
        {
            if(m_scaffold != null)
            {
                m_scaffold.RecalculateNormals();
            }
        }

        public void Deform(MeshDeformer deformer, Mesh original, Mesh colliderOriginal)
        {
            if(m_scaffold != null)
            {
                m_scaffold.Deform(deformer, original, colliderOriginal, m_isRigid);
            }
        }

        public void SlerpContacts(MeshDeformer deformer, Mesh original, Mesh colliderOriginal, ScaffoldWrapper prev, ScaffoldWrapper next)
        {
            
            if(m_isRigid)
            {
                return;
            }

            if(m_scaffold != null)
            {
                Scaffold prevObj = null;
                if (prev != null)
                {
                    int contactPointIndex = prev.CurveIndices.Max() * 3 + 3;
                    ControlPointMode mode = deformer.GetControlPointMode(contactPointIndex);
                    if(mode != ControlPointMode.Free)
                    {
                        prevObj = prev.Obj;
                    }
                }
                Scaffold nextObj = null;
                if (next != null)
                {
                    int contactPointIndex = next.CurveIndices.Min() * 3;
                    ControlPointMode mode = deformer.GetControlPointMode(contactPointIndex);
                    if(mode != ControlPointMode.Free)
                    {
                        nextObj = next.Obj;
                    }
                }
                m_scaffold.SlerpContacts(deformer, original, colliderOriginal, prevObj, nextObj, m_isRigid);
            }
        }
    }
}

