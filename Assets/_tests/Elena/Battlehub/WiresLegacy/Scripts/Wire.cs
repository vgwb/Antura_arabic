using UnityEngine;
using System.Collections;

namespace Battlehub.WiresLegacy
{
	[ExecuteInEditMode]
	public class Wire : MonoBehaviour
	{
		public Material WireMaterial;
		public int Slices = 5;
		public float LOD = 1.0f;
			
		private WireRuntime m_wire;

		public void CreateWire()
		{
			if(m_wire == null)
			{
				return;
			}

			m_wire.CreateWire();
		}

	

		// Use this for initialization
		private void Start () 
		{
			if(Application.isEditor && !Application.isPlaying)
			{
			
				WireRuntime runtime = gameObject.GetComponent<WireRuntime>();
				if(runtime != null)
				{
					DestroyImmediate(runtime);
				}

				m_wire = gameObject.GetComponent<WireEditor>();
				if(m_wire == null)
				{
					m_wire = gameObject.AddComponent<WireEditor>();
				}

			
			}
			else
			{
			
				WireEditor editor = gameObject.GetComponent<WireEditor>();
				if(editor != null)
				{
					DestroyImmediate(editor);
				}

				m_wire = gameObject.GetComponent<WireRuntime>();
				if(m_wire == null)
				{
					m_wire = gameObject.AddComponent<WireRuntime>();
				}


			}
		}
	}

}
