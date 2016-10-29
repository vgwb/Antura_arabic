using UnityEngine;
using System.Collections;

namespace Battlehub.WiresLegacy
{
	[ExecuteInEditMode]
	public class Knot : MonoBehaviour 
	{
	
		public Vector3 Position
		{
			get { return gameObject.transform.localPosition; }
		}
		
		[HideInInspector]
		public Vector3 P1;
		
		[HideInInspector]
		public Vector3 P2;
		
		[HideInInspector]
		public int Rings;
		
		public float Radius = 0.12f;

		public float LOD = 1.0f;

		private void Start()
		{
			if(Application.isEditor && !Application.isPlaying)
			{
				if(gameObject.GetComponent<KnotEditor>() == null)
				{
					gameObject.AddComponent<KnotEditor>();
				}
			}
			else
			{
				KnotEditor editor = gameObject.GetComponent<KnotEditor>();
				if(editor != null)
				{
					DestroyImmediate(editor);
				}

			}
		}
	}
}
