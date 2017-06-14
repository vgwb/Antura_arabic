using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace EA4S.Debugging
{
	public class DebugPanel : MonoBehaviour
	{
		public static DebugPanel I;

		[Header("References")]
		public GameObject Panel;
		public GameObject Container;
		public GameObject PrefabRow;
		public GameObject PrefabButton;

		private int clickCounter;

		void Awake()
		{
			if (I != null) {
				Destroy(gameObject);
			} else {
				I = this;
				DontDestroyOnLoad(gameObject);
			}

			if (Panel.activeSelf) {
				Panel.SetActive(false);
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