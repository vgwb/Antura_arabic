using UnityEngine;
using System.Collections;

namespace Battlehub.WiresLegacy
{
	[ExecuteInEditMode]
	public class WireEditor : WireRuntime {

		private float m_previousLOD;
		private bool m_knotChanged;
		public void NotifyKnotChanged()
		{
			m_knotChanged = true;
		}

		private KnotEditor[] m_knotEditors;
		private float m_previosUpdateTime;

		// Update is called once per frame
		private void Update () 
		{
			if(m_previousLOD != m_wire.LOD)
			{
				m_previousLOD = m_wire.LOD;
				m_knotChanged = true;
			}

		}

		public override void CreateWire ()
		{
			base.CreateWire ();
			m_previosUpdateTime = Time.realtimeSinceStartup;
			m_previousLOD = m_wire.LOD;
			m_knotEditors = GetComponentsInChildren<KnotEditor>();
			foreach(KnotEditor editor in m_knotEditors)
			{
				editor.WireEditor = this;
			}
		}

	
		private void OnEnable()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.update += OnEditorUpdate;
#endif
		}
		
		private void OnDisable()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.update -= OnEditorUpdate;
#endif
		}

		
		private void OnEditorUpdate()
		{
			if(Time.realtimeSinceStartup - m_previosUpdateTime > 1.0f)
			{
				m_previosUpdateTime = Time.realtimeSinceStartup;
				if(m_knotChanged)
				{
					m_knotChanged = false;
					CreateWire();
				}
			}
			
		}
	}

}
