using UnityEngine;
using System.Collections;

namespace EA4S.DancingDots
{
	public class DancingDotsLivingLetterClickCollider : MonoBehaviour
	{
		public DancingDotsLivingLetter Controller;

		void OnMouseDown()
		{
			Controller.Clicked();
		}
	}
}