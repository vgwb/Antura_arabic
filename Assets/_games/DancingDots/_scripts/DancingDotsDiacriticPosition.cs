using UnityEngine;
using System.Collections;


namespace EA4S.DancingDots
{
	public class DancingDotsDiacriticPosition : MonoBehaviour {

		public MeshRenderer letterMesh;
		public MeshRenderer dotmesh;
		public Diacritic diacritic;
		public float offSetX = 0.0f;
		public float offSetY = 0.0f;

		private MeshRenderer diacriticMesh;

		void Awake () {
			diacriticMesh = GetComponent<MeshRenderer>();
		}

		// Use this for initialization
		void Start () {
		}

		public void CheckPosition()
		{
			if (letterMesh && diacriticMesh)
			{
				float newY = Mathf.Clamp(diacriticMesh.bounds.extents.y, 0.5f, 5f) + offSetY;

				if (diacritic == Diacritic.Kasrah)
				{
					float letterBottom = letterMesh.bounds.center.y - letterMesh.bounds.extents.y;
					float dotBottom = dotmesh.bounds.center.y - dotmesh.bounds.extents.y;
					newY = -newY;
					newY += letterBottom < dotBottom ? letterBottom : dotBottom;
				}
				else
				{
					float letterTop = letterMesh.bounds.center.y + letterMesh.bounds.extents.y;
					float dotTop = dotmesh.bounds.center.y + dotmesh.bounds.extents.y;
					newY += letterTop > dotTop ? letterTop : dotTop;
				}	

				transform.position =  new Vector3(transform.position.x, newY, transform.position.z);
			}
		}
			
	}
}
