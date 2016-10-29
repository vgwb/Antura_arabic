using UnityEngine;
using System.Collections;

namespace Battlehub.WiresLegacy
{
	[ExecuteInEditMode]
	public class KnotEditor : MonoBehaviour 
	{
		[HideInInspector]
		public WireEditor WireEditor;
		private Knot m_knot;

		private Vector3 m_previosPosition;
		private float m_previosRadius;
		private Vector3 m_previosP1;
		private Vector3 m_previosP2;
		private int m_previosRings;
		private float m_previosLOD;
	
	
		// Use this for initialization
		private void Start () 
		{
			m_knot = GetComponent<Knot>();
			if(m_knot == null)
			{
				Debug.LogError("KnotEditor requires Knot script");
			}

			m_previosPosition = m_knot.Position;
			m_previosRadius = m_knot.Radius;
			m_previosP1 = m_knot.P1;
			m_previosP2 = m_knot.P2;
			m_previosRings = m_knot.Rings;
			m_previosLOD = m_knot.LOD;

		}

	
		// Update is called once per frame
		private void Update () 
		{
			if(m_previosPosition != m_knot.Position)
			{
				m_previosPosition = m_knot.Position;
				if(WireEditor != null)
				{
					WireEditor.NotifyKnotChanged();
				}


			}
			else if(m_previosRings != m_knot.Rings)
			{
				m_previosRings = m_knot.Rings;
				if(WireEditor != null)
				{
					WireEditor.NotifyKnotChanged();
				}
			}
			else if(m_previosRadius != m_knot.Radius)
			{
				m_previosRadius = m_knot.Radius;
				if(WireEditor != null)
				{
					WireEditor.NotifyKnotChanged();
				}
			}
			else if(m_previosP1 != m_knot.P1)
			{
				m_previosP1 = m_knot.P1;
				if(WireEditor != null)
				{
					WireEditor.NotifyKnotChanged();
				}
			}
			else if(m_previosP2 != m_knot.P2)
			{
				m_previosP2 = m_knot.P2;
				if(WireEditor != null)
				{
					WireEditor.NotifyKnotChanged();
				}
			}
			else if(m_previosLOD != m_knot.LOD)
			{
				m_previosLOD = m_knot.LOD;
				if(WireEditor != null)
				{
					WireEditor.NotifyKnotChanged();
				}
			}


		}
	}

}
