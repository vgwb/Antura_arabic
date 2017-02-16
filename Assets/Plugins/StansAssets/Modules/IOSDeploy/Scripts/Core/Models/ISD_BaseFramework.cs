
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace SA.IOSDeploy {

	[System.Serializable]
	public class BaseFramework  {


		//Editor Use Only
		public bool IsOpen = true;

		public IOSBaseFrameworks Type;
		public bool IsOptional;

		public string Name{
			get{
				return Type.ToString () + ".framework";
			}
		}
		public BaseFramework(IOSBaseFrameworks type){
			this.Type = type;
		}
		public BaseFramework(){

		}

		private int _Index;
		public int Index{
			get{
				return _Index;
			}
			set{
				_Index = value;
				Type = ISD_FrameworkHandler.AvailableFrameworks[value].Type;
			}

		}



		public int[] BaseIndexes(){
			string[] mainArray = ISD_FrameworkHandler.BaseFrameworksArray();
			List<int> indexes = new List<int>(mainArray.Length);
			for (int i = 0; i < mainArray.Length; i++) {
				indexes.Add (i);
			}
			return indexes.ToArray ();
		}


		public string TypeString{
			get{
				return Type.ToString ();
			}
			set{
				foreach (BaseFramework bf in ISD_Settings.Instance.BaseFrameworks) {
					if(bf.TypeString.Equals(value)){
						Type = bf.Type;
					}
				}
			}
		}

	}


}