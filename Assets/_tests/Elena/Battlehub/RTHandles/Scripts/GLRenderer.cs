using UnityEngine;
using System.Collections.Generic;
using System;

namespace Battlehub.RTHandles
{
    public interface IGL
    {
        void Draw();
    }

    [ExecuteInEditMode]
    public class GLRenderer : MonoBehaviour
    {
        private static GLRenderer m_instance;

        public static GLRenderer Instance
        {
            get { return m_instance; }
        }

        private List<IGL> m_renderObjects;
        public void Add(IGL gl)
        {
            if (m_renderObjects.Contains(gl))
            {
                return;
            }

            m_renderObjects.Add(gl);
        }

        public void Remove(IGL line)
        {
            m_renderObjects.Remove(line);
        }

        private void Awake()
        {
            if(m_instance != null)
            {
                Debug.LogWarning("Another instance of GLLinesRenderer aleready exist");
            }
            m_instance = this;

            m_renderObjects = new List<IGL>();
          
        }

        public void Draw()
        {
            if(m_renderObjects == null)
            {
                return;
            }
          
            for (int i = 0; i < m_renderObjects.Count; ++i)
            {
                IGL line = m_renderObjects[i];
                line.Draw();
            }
        }


#if UNITY_EDITOR
        private void Update()
        {
            if (m_instance == null)
            {
                m_instance = this;
                m_renderObjects = new List<IGL>();
            }
        }
#endif
    }
}
