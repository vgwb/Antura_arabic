using UnityEngine;
using System.Collections;

namespace EA4S.HideAndSeek
{
    public class HideAndSeekTreeController : MonoBehaviour
    {
		public delegate void TouchAction(int i);
		public static event TouchAction onTreeTouched;
        
	    void OnMouseDown()
		{
			if (onTreeTouched != null)
            {
				onTreeTouched (id);
			}
		}
		
		public int id;
    }

}
