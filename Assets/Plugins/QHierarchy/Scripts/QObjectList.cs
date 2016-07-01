using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace qtools.qhierarchy
{
	[ExecuteInEditMode]
	public class QObjectList: MonoBehaviour
	{
		public List<GameObject> lockedObjects 			 = new List<GameObject>();
		public List<GameObject> editModeVisibileObjects  = new List<GameObject>();
		public List<GameObject> editModeInvisibleObjects = new List<GameObject>();
		public List<GameObject> wireframeHiddenObjects   = new List<GameObject>();

		public void Awake()
		{
			checkIntegrity();

			foreach (GameObject gameObject in editModeVisibileObjects)               
				gameObject.SetActive(!Application.isPlaying);                
			
			foreach (GameObject gameObject in editModeInvisibleObjects)                
				gameObject.SetActive(Application.isPlaying);
		}

		public void OnDestroy()
		{
			if (!Application.isPlaying)
			{
				checkIntegrity();

				foreach (GameObject gameObject in editModeVisibileObjects)               
					gameObject.SetActive(false);                
				
				foreach (GameObject gameObject in editModeInvisibleObjects)                
					gameObject.SetActive(true);

				foreach (GameObject gameObject in lockedObjects)   			
					gameObject.hideFlags &= ~HideFlags.NotEditable;
			}
		}

		public void merge(QObjectList anotherInstance)
		{ 
			lockedObjects.AddRange(anotherInstance.lockedObjects);
			editModeVisibileObjects.AddRange(anotherInstance.editModeVisibileObjects);
			editModeInvisibleObjects.AddRange(anotherInstance.editModeInvisibleObjects);
			wireframeHiddenObjects.AddRange(anotherInstance.wireframeHiddenObjects);
        } 

		public void checkIntegrity()
		{
			lockedObjects.RemoveAll(item => item == null);
			editModeVisibileObjects.RemoveAll(item => item == null);
			editModeInvisibleObjects.RemoveAll(item => item == null);
			wireframeHiddenObjects.RemoveAll(item => item == null);
		}              
	}
}