using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;



namespace SA.IOSDeploy {
	public class ISD_FrameworkHandler : MonoBehaviour {



		private static List<BaseFramework> _DefaultFrameworks = null;
	//	public static List<BaseFramework> BaseFrameworks =  new List<BaseFramework>();

		public static List<BaseFramework> AvailableFrameworks{
			get{
				List<BaseFramework> resultList = new List<BaseFramework> ();
				List<string> strings = new List<string>( Enum.GetNames (typeof(IOSBaseFrameworks)));
				foreach (BaseFramework frmwrk in ISD_Settings.Instance.BaseFrameworks) {
					if (strings.Contains(frmwrk.Type.ToString())) {
						strings.Remove (frmwrk.Type.ToString());
					}
				}
				foreach (BaseFramework frmwrk in DefaultFrameworks) {
					if (strings.Contains(frmwrk.Type.ToString())) {
						strings.Remove (frmwrk.Type.ToString());
					}
				}
				foreach (IOSBaseFrameworks v in Enum.GetValues(typeof(IOSBaseFrameworks))) {
					if(strings.Contains(v.ToString())){
						resultList.Add(new BaseFramework((IOSBaseFrameworks)v) );
					}
				}
				return resultList;
			}
		}

		public static List<string> GetImportedFrameworks(){
			List<string> FoundedFrameworks = new List<string> ();
			DirectoryInfo DirectoryPath = new DirectoryInfo (Application.dataPath);

			string[] dirrExtensions = new[] { ".framework"};
			FileInfo[] allFiles = DirectoryPath.GetFiles ("*", SearchOption.AllDirectories);
			DirectoryInfo[] dirrFiles = DirectoryPath.GetDirectories ("*", SearchOption.AllDirectories);

			allFiles = allFiles.Where(f => dirrExtensions.Contains(f.Extension.ToLower())).ToArray();
			dirrFiles = dirrFiles.Where (f => dirrExtensions.Contains (f.Extension.ToLower ())).ToArray ();

			foreach (FileInfo file in allFiles) {
				string NewFrameworkName = file.Name;
				bool found = false;
				foreach (Framework framework in ISD_Settings.Instance.Frameworks) {
					if (framework.Name.Equals (NewFrameworkName)) {
						found = true;
					}
				}
				if (!found) {
					FoundedFrameworks.Add (NewFrameworkName);
				}
			}
			foreach (DirectoryInfo file in dirrFiles) {
				string NewFrameworkName = file.Name;
				bool found = false;
				foreach (Framework framework in ISD_Settings.Instance.Frameworks) {
					if (framework.Name.Equals (NewFrameworkName)) {
						found = true;
					}
				}
				if (!found) {
					FoundedFrameworks.Add (NewFrameworkName);
				}
			}
			return FoundedFrameworks;
		}


		public static List<string> GetImportedLibraries(){
			List<string> FoundedFrameworks = new List<string> ();
			DirectoryInfo DirectoryPath = new DirectoryInfo (Application.dataPath);

			string[] fileExtensions = new[] {".a", ".dylib"};
		//	string[] dirrExtensions = new[] { ".framework", ".bundle"};
			FileInfo[] allFiles = DirectoryPath.GetFiles ("*", SearchOption.AllDirectories);
		//	DirectoryInfo[] dirrFiles = DirectoryPath.GetDirectories ("*", SearchOption.AllDirectories);

			allFiles = allFiles.Where(f => fileExtensions.Contains(f.Extension.ToLower())).ToArray();
		//	dirrFiles = dirrFiles.Where (f => dirrExtensions.Contains (f.Extension.ToLower ())).ToArray ();

			//	allFiles = allFiles.Intersect (dirrFiles);
			foreach (FileInfo file in allFiles) {
				string NewFrameworkName = file.Name;
				bool found = false;
				foreach (Framework framework in ISD_Settings.Instance.Frameworks) {
					if (framework.Name.Equals (NewFrameworkName)) {
						found = true;
					}
				}
				if (!found) {
					FoundedFrameworks.Add (NewFrameworkName);
				}
			}
//			foreach (DirectoryInfo file in dirrFiles) {
//				string NewFrameworkName = file.Name;
//				bool found = false;
//				foreach (Framework framework in ISD_Settings.Instance.Frameworks) {
//					if (framework.Name.Equals (NewFrameworkName)) {
//						found = true;
//					}
//				}
//				if (!found) {
//					FoundedFrameworks.Add (NewFrameworkName);
//				}
//			}
			return FoundedFrameworks;
		}

		public static List<BaseFramework> DefaultFrameworks{
			get{
				if(_DefaultFrameworks == null){
					_DefaultFrameworks = new List<BaseFramework>();
					_DefaultFrameworks.Add (new BaseFramework (IOSBaseFrameworks.CoreText));
					_DefaultFrameworks.Add (new BaseFramework (IOSBaseFrameworks.AudioToolbox));
					_DefaultFrameworks.Add (new BaseFramework (IOSBaseFrameworks.AVFoundation));
					_DefaultFrameworks.Add (new BaseFramework (IOSBaseFrameworks.CFNetwork));
					_DefaultFrameworks.Add (new BaseFramework (IOSBaseFrameworks.CoreGraphics));
					_DefaultFrameworks.Add (new BaseFramework (IOSBaseFrameworks.CoreLocation));
					_DefaultFrameworks.Add (new BaseFramework (IOSBaseFrameworks.CoreMedia));
					_DefaultFrameworks.Add (new BaseFramework (IOSBaseFrameworks.CoreMotion));
					_DefaultFrameworks.Add (new BaseFramework (IOSBaseFrameworks.CoreVideo));
					_DefaultFrameworks.Add (new BaseFramework (IOSBaseFrameworks.Foundation));
					_DefaultFrameworks.Add (new BaseFramework (IOSBaseFrameworks.iAd));
					_DefaultFrameworks.Add (new BaseFramework (IOSBaseFrameworks.MediaPlayer));
					_DefaultFrameworks.Add (new BaseFramework (IOSBaseFrameworks.OpenAL));
					_DefaultFrameworks.Add (new BaseFramework (IOSBaseFrameworks.OpenGLES));
					_DefaultFrameworks.Add (new BaseFramework (IOSBaseFrameworks.QuartzCore));
					_DefaultFrameworks.Add (new BaseFramework (IOSBaseFrameworks.SystemConfiguration));
					_DefaultFrameworks.Add (new BaseFramework (IOSBaseFrameworks.UIKit));
					_DefaultFrameworks.Add (new BaseFramework (IOSBaseFrameworks.GameKit));
				}
				return _DefaultFrameworks;
			}
		}

		public static string[] BaseFrameworksArray(){
			List<string> array = new List<string> (AvailableFrameworks.Capacity);
			foreach (BaseFramework framework in AvailableFrameworks) {
				array.Add (framework.Type.ToString ());
			}
			return array.ToArray ();
		} 

	}
}
