using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace EA4S.Debugging
{
	public class DebugPanel : MonoBehaviour
	{
		public static DebugPanel I;

		public GameObject Panel;

		private int clickCounter;

		void Awake()
		{
			if (I != null) {
				Destroy(gameObject);
			} else {
				I = this;
				DontDestroyOnLoad(gameObject);
			}
		}

		public void OnClickOpen()
		{
			clickCounter++;
			if (clickCounter >= 3) {
				open();
			}
		}

		public void OnClickClose()
		{
			close();
		}

		private void open()
		{
			Panel.SetActive(true);
		}

		private void close()
		{
			clickCounter = 0;
			Panel.SetActive(false);
		}
	}
}