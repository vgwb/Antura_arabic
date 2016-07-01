//	Grab Yer Assets
//	Copyright Frederic Bell, 2014

//#if (UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
//#define UNITY_3_0_OR_NEWER
//#endif

//#if (UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7)
//#define UNITY_4_0_OR_NEWER
//#endif

#if UNITY_5
#define UNITY_5_0_OR_NEWER
#endif

#if (UNITY_5_0_OR_NEWER && !UNITY_5_0)
#define UNITY_5_1_OR_NEWER
#endif

#if (UNITY_5_1_OR_NEWER && !UNITY_5_1)
#define UNITY_5_2_OR_NEWER
#endif

#if (UNITY_5_2_OR_NEWER && !UNITY_5_2)
#define UNITY_5_3_OR_NEWER
#endif

// Unity 5.3.4 and newer, auto assigns: UNITY_x_y_OR_NEWER

using UnityEngine;
using UnityEngine.Serialization;
using UnityEditor;
using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GYAInternal.Json;
using UnityEditorInternal;
using System.Reflection;
// Symlink
using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Runtime.InteropServices;
// OS X SymLink
// /Applications/Unity/Unity.app/Contents/Frameworks/Mono/lib/mono/2.0/Mono.Posix.dll
//using Mono.Unix;
//using System.Security.AccessControl;

namespace XeirGYA
{
	// Project Class
	[System.Serializable]
	public partial class GrabYerAssets : EditorWindow
	{
		public static GrabYerAssets Instance;
		//public static GrabYerAssets window;
		private Event evt = null;

		// GYA related strings
		const string gyaName = "Grab Yer Assets";
		const string gyaAbbr = "GYA";
		const string gyaVersion = "2.16d13a";

		// New path info
		//public string gyaRootPath = GYAExt.PathGYADataFiles;
		const string pathOldAssetsFolderName = "Asset Store-Old";		// Old Assets folder
		const string jsonFilePackagesName = gyaAbbr + " Assets.json";	// GYA Packages Data file
		const string jsonFileUserName = gyaAbbr + " Settings.json";		// GYA User Data file

		// Path related
		private string pathOldAssetsFolder = pathOldAssetsFolderName; // Old Assets folder
		private string jsonFilePackages = jsonFilePackagesName;	// GYA Packages Data file
		private string jsonFileUser = jsonFileUserName;	// GYA User Data file

		// Dropdown text for categories
		const string unity5VersionText = "Unity 5+";
		const string unity5FolderText = "Asset Store-5.x";
		const string unityOlderVersionText = "Unity -4";
		const string unityOlderFolderText = "Asset Store";
		const string packageText = "Asset Store";
		const string exportedText = "Exported";
		const string ungroupedText = "Ungrouped";
		const string damagedText = "Damaged";

		// Detect if script is recompiled, check is in OnInspectorUpdate
		private class GYARecompiled { }
		private GYARecompiled gyaRecompiled;

		// Debug/Diag handling
		private bool guiOverride = false;	// Used to only show the top of the GUI, managed by GUIOverride()
		private string fldOverrideEntry = String.Empty; // Used for text entry during GUIOverride()
		private string fldOverridePassed = String.Empty; // Used for text entry during GUIOverride()

		// Stopwatch
		System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();

		// SymLink vars
		//private bool isSymLinkPathAS = GYAIO.IsSymLink(GYAExt.PathUnityAssetStore);
		//private bool isSymLinkPathAS5 = GYAIO.IsSymLink(GYAExt.PathUnityAssetStore5);
		//private string symlinkTargetAS = GYAIO.GetSymLinkTarget(GYAExt.PathUnityAssetStore);
		//private string symlinkTargetAS5 = GYAIO.GetSymLinkTarget(GYAExt.PathUnityAssetStore5);
		private bool isSymLinkPathAS = false;
		private bool isSymLinkPathAS5 = false;
		private string symlinkTargetAS = "";
		private string symlinkTargetAS5 = "";

		// JSON handling
		internal RootPackages pkgData = new RootPackages();	// Assets: Store, Standard, Old, User, Project
		internal RootUser userData = new RootUser();	// User Data: Settings & Groups, etc.
		private Packages pkgDetails = new Packages(); // Package data for the currently hilighted asset in the scroll view
		private List<Packages> packageShow = null; //pkgData.Store;

		private string infoToShow = String.Empty;
		private Dictionary<int, List<Packages>> grpData = new Dictionary<int, List<Packages>>(); // Group data for group popups (Rename, delete, etc)
		// Search & Display related
		private bool infoToDisplayHasChanged = true; // If info to display has changed, refresh the view
		private bool ddActive = false; // Is a drop-down active?
		[SerializeField]
		private string fldSearch = String.Empty;	// Search field
		[SerializeField]
		private string ddCategory = "";
		[SerializeField]
		private int ddPublisher = 0;
		private string headerLast = String.Empty; // Last Assets Category
		// Package List and Counts
		[SerializeField]
		private Dictionary<int, string> popupView = new Dictionary<int, string>();	// Asset & Group section popup data
		private int cntMarkedToImport = 0;
		private string pkgInfoText = String.Empty;
		private int countPackageErrors = 0; // Package error count

		// GUI placement
		[SerializeField]
		private string selectionText = String.Empty;
		private int wTop = 0; // Top position for item placement in window
		private int controlHeight;
		// Scrollview area
		[SerializeField]
		private Vector2 svPosition = new Vector2();
		private Rect svFrame, svList, svToggle, svButton;
		private int svHeight; // Scrollview height - reserved height
		private float svLineHeight; // Scrollview item vertical spacing
		// Scrollview Style
		private GUIStyle svStyleDefault;
		private GUIStyle svStyleStore;
		//private GUIStyle svStyleStoreAlt;
		private GUIStyle svStyleStandard;
		private GUIStyle svStyleOld;
		private GUIStyle svStyleOldToMove;
		private GUIStyle svStyleUser;
		private GUIStyle svStyleProject;
		private GUIStyle svStyleSeperator;
		private GUIStyle svStyleIcon;
		private GUIStyle svStyleIconLeft;
		private GUIStyle tbStyle;
		private GUIStyle tbStyleDD;	// 2nd Toolbar style
		private GUIStyle foStyleInfo;	// Info panel style - not used right now
		// Textures
		private Texture2D iconPrev;
		private Texture2D iconNext;
		private Texture2D iconFavorite;
		private Texture2D iconDamaged;
		private Texture2D iconMenu;
		private Texture2D iconRefresh;
		private Texture2D iconReset;
		private Texture2D iconResetX;
		private Texture2D iconCategory;
		private Texture2D iconCategoryX;
		private Texture2D iconPublisher;
		private Texture2D iconPublisherX;
		private Texture2D iconStore;
		private Texture2D iconUser;
		private Texture2D iconStandard;
		private Texture2D iconOld;
		private Texture2D iconProject;
		private Texture2D iconStoreAlt;
		private Texture2D iconUserAlt;
		private Texture2D iconStandardAlt;
		private Texture2D iconOldAlt;
		//private Texture2D iconBlank;

		private Texture2D texTransparent;
		private Texture2D texDivider;
		private Texture2D texSelector;

		// Multi-Import Type
		public enum MultiImportType
		{
			// Default version preference
			Default,	// Any				<Best option auto choosen>
			UnityAsync,	// 4.2x - 5.2x		AssetDatabase.ImportPackage
			GYASync,	// 5.3x - 5.4.0b12	GYAImport.ImportPackage -> AssetDatabase.ImportPackage
			UnitySync	// 5.4.0b13 +		AssetDatabase.ImportPackageImmediately
		};

		// Stop GUI Reasons
		private static OverrideReason overrideReason = OverrideReason.None;
		private enum OverrideReason
		{
			None,
			Error,
			ErrorStep2,
			GroupCreate,
			GroupRename,
			UserFolder,
			MoveOldAssetFolder
		};

		// Test for Unity Pro Skin change
		UnityGUISkin GUISkinChangedCurrent;
		UnityGUISkin GUISkinChangedLast;
		private enum UnityGUISkin
		{
			Pro,
			NonPro
		};

		// Show Assets Toggle
		[SerializeField]
		internal svCollection showActive;
		[SerializeField]
		internal int showGroup = 0; // Group to show if showActive = Group
		internal enum svCollection
		{
			All,
			Store,
			User,
			Standard,
			Old,
			Group,	// User created groups
			Project	// Local Project Assets, shown at the top of the All collection
		}

		// Sort Toggle
		[SerializeField]
		private svSortBy sortActive;
		private enum svSortBy
		{
			Title,
			Size,
			Publisher,
			PackageID,
			VersionID,
			Category,
			CategorySub,
			UploadID,
			DateFile,		// File Date
			DatePublish,	// Publish Date
			DatePackage		// Package Build Date
		}

		// Search Toggle - Embedded fields inside the unitypackage files unless otherwise noted
		[SerializeField]
		private svSearchBy searchActive;
		private enum svSearchBy
		{
			Title,
			Category,
			Publisher,
			Description
			//PublishNotes,
			//UserNotes
		}

		// JSON data from unitypackage files
		internal class Link
		{
			public string type { get; set; }
			[JsonConverter(typeof(StringToIntConverter))]
			public int id { get; set; }
		}

		internal class Category
		{
			public string label { get; set; }
			[JsonConverter(typeof(StringToIntConverter))]
			public int id { get; set; }
		}

		internal class Publisher
		{
			public string label { get; set; }
			[JsonConverter(typeof(StringToIntConverter))]
			public int id { get; set; }
		}

		//[System.Serializable]
		internal class Packages
		{
			public Link link { get; set; }
			public string unity_version { get; set; }
			public string pubdate { get; set; }
			public string version { get; set; }
			public int upload_id { get; set; }
			[JsonConverter(typeof(StringToIntConverter))]
			public int version_id { get; set; }
			public string description { get; set; }
			public string publishnotes { get; set; }
			public Category category { get; set; }
			[JsonConverter(typeof(StringToIntConverter))]
			public int id { get; set; }
			public string title { get; set; }
			public Publisher publisher { get; set; }
			public string filePath { get; set; }
			public double fileSize { get; set; }
			public DateTimeOffset fileDataCreated { get; set; }
			public DateTime fileDateCreated { get; set; }
			//public DateTime fileDateModified { get; set; }
			//public DateTime fileDateLastOpen { get; set; }
			public bool isExported { get; set; } // True = User Created Pkg, False = Asset Store Pkg
			public bool isDamaged { get; set; }
			//[JsonConverter(typeof(StringEnumConverter))]
			public svCollection collection { get; set; } // Use Store, Standard, Old, User when creating json
			// Generated later
			[JsonIgnore]
			public bool isOldToMove { get; set; } // Is old file that can be moved/consolidated
			[JsonIgnore]
			public bool isMarked { get; set; } // Is asset marked for import or multiple
			[JsonIgnore]
			public bool isInAGroup { get; set; } // Is asset in a group
			//public bool isFileMissing { get; set; } // Is file missing
			public bool isInASFolder { get; set; } // Is it in the AS5 folder
			public bool isInAS5Folder { get; set; } // Is it in the AS5 folder
			public string isInNativeASFolder { get; set; } // Is it in the AS5 folder

			public Packages () {
				isOldToMove = false;
				isMarked = false;
				isInAGroup = false;
				//isFileMissing = false;
				//isInASFolder = false;
				//isInAS5Folder = false;
			}
		}

		internal class RootPackages
		{
			//internal class RootPackages : GYASingleton<RootPackages> {
			public string version { get; set; }				// GYA Version that made the Package file
			//public string scannedFolders { get; set; }	// Unity AS Folder(s) last scanned
			public List<Packages> Assets { get; set; }
			[JsonIgnore]
			public int countAll { get; set; }
			[JsonIgnore]
			public int countStore { get; set; }
			[JsonIgnore]
			public int countUser { get; set; }
			[JsonIgnore]
			public int countStandard { get; set; }
			[JsonIgnore]
			public int countOld { get; set; }
			[JsonIgnore]
			public int countOldToMove { get; set; }
			[JsonIgnore]
			public int countProject { get; set; }
			[JsonIgnore]
			public double filesizeAll { get; set; }
			[JsonIgnore]
			public double filesizeStore { get; set; }
			[JsonIgnore]
			public double filesizeUser { get; set; }
			[JsonIgnore]
			public double filesizeStandard { get; set; }
			[JsonIgnore]
			public double filesizeOld { get; set; }
		}

		// User File Data
		internal class Settings
		{
			//public bool refreshOnStartup { get; set; }
			public bool scanAllAssetStoreFolders { get; set; }
			public bool showAllAssetStoreFolders { get; set; }
			public bool isPersist { get; set; }
			public string pathUserAssets { get; set; }
			public List<string> pathUserAssetsList { get; set; }
			public bool enableHeaders { get; set; }
			public bool enableColors { get; set; }
			public bool showSVInfo { get; set; }
			public bool reportPackageErrors { get; set; }
			public bool nestedDropDowns { get; set; }
			public bool nestedVersions { get; set; }
			public bool enableCollectionTypeIcons { get; set; }
			public bool enableAltIconOtherVersions { get; set; }
			//public bool enableAltIconSwap { get; set; }

			public bool autoPreventASOverwrite { get; set; }
			public bool autoConsolidate { get; set; }
			public bool autoDeleteConsolidated { get; set; }
			public bool enableOfflineMode { get; set; }
			public bool openURLInUnity { get; set; }
			public bool showProgressBarDuringRefresh { get; set; }
			public bool showProgressBarDuringFileAction { get; set; }
			//public bool autoBackup { get; set; }
			public MultiImportType multiImportOverride { get; set; }

			public Settings ()
			{
				//pathGYAFolder = GYAExt.PathGYADataFiles;
				//refreshOnStartup = false;
				scanAllAssetStoreFolders = true;
				showAllAssetStoreFolders = true;
				isPersist = false;
				pathUserAssets = String.Empty;
				pathUserAssetsList = new List<string>();
				enableHeaders = true;
				enableColors = true;
				showSVInfo = true;
				reportPackageErrors = false;
				nestedDropDowns = true;
				nestedVersions = false;
				enableCollectionTypeIcons = true;
				enableAltIconOtherVersions = true;
				//enableAltIconSwap = true;
				autoPreventASOverwrite = false;
				autoConsolidate = false;
				autoDeleteConsolidated = false;
				enableOfflineMode = false;
				openURLInUnity = false;
				showProgressBarDuringRefresh = true;
				showProgressBarDuringFileAction = true;
				//autoBackup = false;
				multiImportOverride = MultiImportType.Default;
			}
		}

		internal class Info
		{
			public int id { get; set; }
			public string notes { get; set; }			// Asset Notes

			public Info () {
				id = 0;
				notes = String.Empty;
			}
		}

		internal class Asset
		{
			// Asset Store Packages
			public string title { get; set; }			// Package title
			public bool isExported { get; set; }		// Is asset store package?
			public int id { get; set; }					// If isassetstorepkg, assets id
			public bool useLatestVersion { get; set; }	// If isassetstorepkg, use latest version of asset?
			public int version_id { get; set; }			// If useLatestVersion, use this version
			// Used for exported (Non-Asset Store) packages, Get the title as the filename in the path
			public string filePath { get; set; }		// If !isassetstorepkg, use filepath
			// Generated later
			[JsonIgnore]
			public bool isFileMissing { get; set; } 	// Is file missing

			public Asset ()
			{
				isExported = false;
				id = 0;
				useLatestVersion = true;
				version_id = 0;
				filePath = String.Empty;
				isFileMissing = false;
			}
		}

		internal class Group
		{
			public string name { get; set; }			// Group name
			public string notes { get; set; }			// Group notes
			public List<Asset> Assets { get; set; }

			public Group ()
			{
				name = String.Empty;
				notes = String.Empty;
				Assets = new List<Asset>();
			}

			public Group (string pName, string pNotes, List<Asset> pAssets)
			{
				name = pName;
				notes = pNotes;
				Assets = new List<Asset>(pAssets);
			}
		}

		internal class RootUser
		{
			public string version { get; set; }		// GYA Version that made the User file
			public Settings Settings { get; set; }
			public List<Info> Info { get; set; }
			public List<Group> Group { get; set; }

			public RootUser ()
			{
				version = gyaVersion;
				Settings = new Settings();
				Info = new List<Info>();
				Group = new List<Group>();
			}
		}

		// Json deserialization override - StringToIntConverter
		internal class StringToIntConverter : JsonConverter
		{
			public override bool CanConvert(Type objectType)
			{
				return objectType == typeof(int);
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
			{
				if (reader.TokenType == JsonToken.Null)
				{
					return 0;
				}
				if (reader.TokenType == JsonToken.Integer)
				{
					return reader.Value;
				}
				if (reader.TokenType == JsonToken.String) {
					if (string.IsNullOrEmpty((string)reader.Value))
					{
						return 0;
					}
					int num;
					if (int.TryParse((string)reader.Value, out num))
					{
						return num;
					}
					throw new JsonReaderException(string.Format("Expected integer, got {0}", reader.Value));
				}
				throw new JsonReaderException(string.Format("Unexcepted token {0}", reader.TokenType));
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			{
				writer.WriteValue(value);
			}
		}

		// Add to the Window menu
		[MenuItem ("Window/GrabYerAssets")]
		private static void Init ()
		{
			GrabYerAssets window;
			string title = "Grab Yer Assets";

			var width = 350;
			var height = (int)(Screen.currentResolution.height*0.6);
			//var center = EditorWindow.focusedWindow.position.center;
			var center = new Vector2(Screen.currentResolution.width/2, Screen.currentResolution.height/2);
			Rect position = new Rect((int)(center.x - 0.5f * width), (int)(center.y - height * 0.5f), width, height);
			// Dock next to Inspector
			//var editorAsm = typeof(UnityEditor.EditorWindow).Assembly;
			//var inspWndType = editorAsm.GetType("UnityEditor.InspectorWindow");
			//window = GetWindow(wTitle, true, inspWndType);
			////window = EditorWindow.GetWindow<WindowA>(typeof(WindowB));

			// If window exist, focus; else center on screen
			if (FindEditorWindow<GrabYerAssets>())
			{
				window = GetWindow<GrabYerAssets>(title, typeof(GrabYerAssets));;
				window.Focus();
			}
			else
			{
				window = (GrabYerAssets)EditorWindow.GetWindowWithRect(typeof(GrabYerAssets), new Rect(0, 0, width, height));
				window.position = position;
			}

			window.minSize = new Vector2(300, 400);

			#if UNITY_5_1_OR_NEWER
			window.titleContent.text = title;
			#else
			window.title = title;
			#endif
		}

		public static bool FindEditorWindow<WindowType>() where WindowType : EditorWindow
		{
			//get {
			WindowType[] windows = Resources.FindObjectsOfTypeAll<WindowType>();
			if(windows != null && windows.Length > 0)
			{
					// do something...
				return true;
			}
			return false;
			//}
		}

		public void OnGUI ()
		{
			// Repaint only when needed
			wantsMouseMove = true;
			evt = Event.current;

			try
			{
				if ( mouseOverWindow.GetType() == (typeof(GrabYerAssets)) && evt.isMouse )
				{
					this.Repaint();
					FocusWindowIfItsOpen(typeof(GrabYerAssets));
				}
			} catch { }

			UpdateWindow ();

			switch( evt.type )
			{
				case EventType.MouseDown:
				case EventType.MouseUp:
				case EventType.MouseDrag:
					evt.Use();
					break;
			}

			//EditorGUIUtility.ExitGUI(); // Breaks - Mouse y offset is wrong
			if (evt == null) { EditorGUIUtility.ExitGUI(); }

		}

		//public void Update () {

		//}

		public void OnInspectorUpdate ()
		{

			// Recompile Check
			if (this.gyaRecompiled == null)
			{
				//Debug.Log (gyaAbbr + " - Recompiled!\n");
				this.gyaRecompiled = new GYARecompiled();
				// Perform after recompile
				infoToDisplayHasChanged = true;
				CheckIfGUISkinHasChanged(true); // Force reload
			}
			else
			{
				// Check if GUI Skin changed
				CheckIfGUISkinHasChanged();
			}
		}

		public void OnFocus ()
		{
			ddActive = false;
			//infoToDisplayHasChanged = true;
		}

		public void OnLostFocus ()
		{
			//infoToDisplayHasChanged = false;
		}

		// Not interecepted on Unity shutdown
		public void OnDestroy ()
		{
			//Debug.Log("OnDestroy");
		}

		// Not interecepted on Unity shutdown
		public void OnDisable ()
		{
			//Debug.Log("OnDisable");
		}

		private int GetAssetCountFromFolder (string folder)
		{
			DirectoryInfo directory = new DirectoryInfo (folder);

			if (directory.Exists)
			{
				FileInfo[] files = directory.GetFiles ("*.unity?ackage", SearchOption.AllDirectories).Where( fi => (fi.Attributes & FileAttributes.Hidden) == 0 ).ToArray();
				return files.Count();
			}
			return 0;
		}

		// Move both prior Old Assets contents to the new one
		private int MoveFolderContentsOldAssets (bool overwrite = false)
		{
			int count = 0;
			count += MoveFolderContents(Path.Combine(GYAExt.PathUnityAssetStore, "-" + gyaName + " (Old)"), pathOldAssetsFolder, overwrite);
			count += MoveFolderContents(Path.Combine(GYAExt.PathGYADataFilesOLD, "-" + gyaName + " (Old)"), pathOldAssetsFolder, overwrite);
			return count;
		}

		// Move Contents from one folder to another accounting for symlinks, Create if required
		private int MoveFolderContents (string moveFrom, string moveTo, bool overwrite = false)
		{
			//Debug.Log(gyaAbbr + " - Moving: " + moveFrom + "\n\tTo: " + moveTo + "\n");

			int count = 0;
			int countTotal = 0;
			string result = "";
			//string fileFrom = "";
			string fileTo = "";

			if (moveFrom.Length != 0 && moveFrom != null &&
				moveTo.Length != 0 && moveTo != null)
			{
				if (!Directory.Exists(moveFrom))
				{
					// Folder missing, exit
					//Debug.Log(gyaAbbr + " - Source folder does NOT exist: " + moveFrom + "\n");
				}
				else
				{
						if (!Directory.Exists(moveTo))
							CreateFolder(moveTo);

						DirectoryInfo directory = new DirectoryInfo (moveFrom);

					if (directory.Exists)
					{
							FileInfo[] files = directory.GetFiles ("*.unity?ackage", SearchOption.AllDirectories).Where( fi => (fi.Attributes & FileAttributes.Hidden) == 0 ).ToArray();

							int filenameStartIndex = (directory.FullName.Length + 1);
							using (
								var progressBar = new ProgressBar(string.Format("{0} Copying {1}", gyaAbbr, directory.FullName), files.Length, 80, (stepNumber) => files[stepNumber].FullName.Substring(filenameStartIndex), userData.Settings.showProgressBarDuringFileAction)
							)

								for (int i = 0; i < files.Length; ++i)
								{
									fileTo = Path.Combine(moveTo, files[i].Name);
									progressBar.Update(i);
									try
									{
										File.SetAttributes(files[i].FullName, FileAttributes.Normal);
										countTotal += 1;
										if (File.Exists(fileTo) && !overwrite)
										{
											//result += "EXISTS:\t" + files[i].FullName + "\n\tTo:\t" + fileTo + "\n";
											result += "EXISTS:\t" + files[i].FullName + "\n\tTo: " + fileTo + "\n";
										}
										else
										{
											if (GYAIO.IsSymLink(moveFrom) || GYAIO.IsSymLink(moveTo))
											{
												File.Copy(files[i].FullName, fileTo, overwrite);
												result += "Copied:\t" + files[i].FullName + "\n\tTo: " + fileTo + "\n";
											}
											else
											{
												if (overwrite)
												{
													File.Delete(fileTo);
													File.Move(files[i].FullName, fileTo);
												}
												else
												{
													File.Move(files[i].FullName, fileTo);
												}
												result += "Moved:\t" + files[i].FullName + "\n\tTo: " + fileTo + "\n";
											}
											// If successful, delete the old file
											if (File.Exists(fileTo))
											{
												count += 1;
												//Debug.Log(gyaAbbr + " - Copied GYA User File To:" + moveTo + "\n");
												//result += "Success Moving:\t" + files[i].FullName + "\n\tTo:\t" + fileTo + "\n";
												//File.SetAttributes(files[i].FullName, FileAttributes.Normal);
												File.Delete(files[i].FullName);
												//Debug.Log(gyaAbbr + " - Deleted (OLD) GYA User File: " + moveFrom + "\n");
											}
										}
									//} catch (IOException ex) {
									}
									catch (IOException ex)
									{
										Debug.LogWarning(gyaAbbr + " - Error Moving: " + files[i].FullName + "\n\tTo: " + fileTo + "\n" + ex.Message);
										//Debug.LogWarning(gyaAbbr + " - Error Moving: " + files[i].FullName + "\n\t\t" + ex.Message);
										//result += "ERROR:\t" + files[i].FullName + "\n\tTo:\t" + fileTo + "\n";
										result += "ERROR:\t" + files[i].FullName + "\n\tTo: " + fileTo + "\n";
									}
								}
							if (countTotal > 0)
							{
								result = gyaAbbr + " - Copied " + count.ToString() + " of " + countTotal.ToString() + "\n" + result;
								Debug.Log(result);
								Debug.Log(gyaAbbr + " - Once you have verified that your Old Assets have been moved without error,\n\tyou can use 'Menu->Maintenance->Clean up Outdated GYA Support Files' to cleanup the outdated files/folders.");
								RefreshPackages();
							}
						}
					}
				}
			return count;
		}

		// GYA Related PreProcess Handling
		private void GYAPreProcessor ()
		{
			//CreateFolder(GYAExt.PathUnityAssetStore5); // ../Unity/Asset Store-5.x

			// If Old Assets folder & User file exist in correct place then end preprocessing
			if (Directory.Exists(pathOldAssetsFolder) && File.Exists(jsonFileUser))
			{
				//Debug.Log(gyaAbbr + " - Support files found in default location.\n");
				return;
			}

			CreateFolder(GYAExt.PathGYADataFiles); // ../Unity/GYA
			if (!Directory.Exists(GYAExt.PathGYADataFiles))
			{
				GUIOverride(OverrideReason.Error);
			}

			// ----

			// GYA Data file/folder strings 2.15c15 ad earlier
			string oldpathOldAssetsFolderName = "-" + gyaName + " (Old)"; // Old Assets folder
			//string oldjsonFilePackagesName = gyaName + ".json";	// GYA Packages Data file
			string oldjsonFileUserName = gyaName + " User.json";	// GYA User Data file

			// Move GYA User file to its own folder
			string moveTo = Path.GetFullPath(Path.Combine(GYAExt.PathGYADataFiles, jsonFileUserName));
			string moveFrom = "";
			// Original support file path
			string moveFrom1 = Path.GetFullPath(Path.Combine(GYAExt.PathUnityAssetStore, oldjsonFileUserName));
			// Recent change, that only lasted for a version, decided to give GYA a folder of its own
			string moveFrom2 = Path.GetFullPath(Path.Combine(GYAExt.PathGYADataFilesOLD, oldjsonFileUserName));

			// ---

			if (Directory.Exists(moveFrom1) || Directory.Exists(moveFrom2))
			{
				Debug.Log(gyaAbbr + " - Support files have been moved in this version of GYA.\n\tNew Location: " + GYAExt.PathGYADataFiles);
			}

			// ---

			// Check if need to move User File
			if (!File.Exists(moveTo))
			{
				// Check for most recent first
				if (File.Exists(moveFrom2))
				{
					// File exists in 1st old loc
					//Debug.Log(gyaAbbr + " - moveFrom2:\t" + moveFrom2 + "\n\tmoveTo:\t\t" + moveTo);
					moveFrom = moveFrom2;
				}
				else if (File.Exists(moveFrom1))
				{
					// File exists in 2nd old loc
					//Debug.Log(gyaAbbr + " - moveFrom1:\t" + moveFrom1 + "\n\tmoveTo:\t\t" + moveTo);
					moveFrom = moveFrom1;
				}
				else
				{
					// '../Unitty/GYA' Doesn't exist, so create '../Unity/GYA/Old Assets'
					//CreateFolder(pathOldAssetsFolder);
				}
			}
			// Move User/Settings file
			if (!File.Exists(moveTo) && File.Exists(moveFrom))
			{
				try
				{
					//Debug.Log(gyaAbbr + " -\tMoving:\t" + moveFrom + "\n\tTo:\t" + moveTo);
					File.SetAttributes(moveFrom, FileAttributes.Normal);
					File.Copy(moveFrom, moveTo);

					if (File.Exists(moveTo))
					{
						File.Delete(moveFrom);
					}
				}
				catch (IOException ex)
				{
					Debug.LogError(gyaAbbr + " - Error Moving: " + moveFrom + " To: " + moveTo + "\nPlease manually copy the file to the new location to retain any saved groups/settings/etc.\n\n" + ex.Message);
				}
			}

			// ---

			//string doesFileExist = Path.GetFullPath(GYAExt.PathUnityAssetStore);
			////doesFileExist = @"/Volumes/OSX/Users/xeir/Library/Unity/Asset Store2";
			//Debug.Log( (Directory.Exists(doesFileExist) ? "TRUE" : "FALSE") + " - " + doesFileExist);
			//Debug.Log( (GYAIO.IsSymLink(doesFileExist) ? "TRUE" : "FALSE") + " - " + doesFileExist);
			//doesFileExist = Path.GetFullPath(jsonFileUser);
			//Debug.Log( (File.Exists(doesFileExist) ? "TRUE" : "FALSE") + " - " + doesFileExist);
			//Debug.Log( (GYAIO.IsSymLink(doesFileExist) ? "TRUE" : "FALSE") + " - " + doesFileExist);
			//Debug.Log( (isSymLinkPathAS ? "TRUE" : "FALSE") + " - " + symlinkTargetAS);
			//Debug.Log( (isSymLinkPathAS5 ? "TRUE" : "FALSE") + " - " + symlinkTargetAS5);

			//Debug.LogWarning(gyaAbbr + " - Please manually move your GYA Settings file to the new location to retain any saved groups/settings/etc.");


			// --------------------
			// Relocate Old Assets if required

			//moveTo = Path.GetFullPath(Path.Combine(GYAExt.PathGYADataFiles, pathOldAssetsFolderName));
			moveFrom1 = Path.GetFullPath(Path.Combine(GYAExt.PathUnityAssetStore, oldpathOldAssetsFolderName));
			moveFrom2 = Path.GetFullPath(Path.Combine(GYAExt.PathGYADataFilesOLD, oldpathOldAssetsFolderName));

			// Move Old Assets Folder if required
			if (!Directory.Exists(pathOldAssetsFolder))
			{
				// Check for most recent first
				if (Directory.Exists(moveFrom2))
				{
					// Directory exists in 1st old loc
					//Debug.Log(gyaAbbr + " - moveFrom2:\t" + moveFrom2 + "\n\tmoveTo:\t\t" + moveTo);
					moveFrom = moveFrom2;
				}
				else if (Directory.Exists(moveFrom1))
				{
					// Directory exists in 2nd old loc
					//Debug.Log(gyaAbbr + " - moveFrom1:\t" + moveFrom1 + "\n\tmoveTo:\t\t" + moveTo);
					moveFrom = moveFrom1;
				}
				else
				{
					// '../Unitty/GYA' Doesn't exist, so create '../Unity/GYA/Old Assets'
					//CreateFolder(pathOldAssetsFolder);
				}
			}

			// Move GYA OLD directory from the AS folder to the Unity folder
			if (!Directory.Exists(pathOldAssetsFolder) && Directory.Exists(moveFrom))
			{
				//Debug.Log(gyaAbbr + " -\tMoving GYA Old Assets folder:\t" + moveFrom + "\n\tTo:\t" + moveTo);

				// Check if AS folder
				if (!isSymLinkPathAS)
				{
					// Not symlink, safe to move OA folder
					try
					{
						//Directory.SetAccessControl(moveFrom, FileAttributes.Normal);
						DirectoryInfo folderMoveFrom = new DirectoryInfo(moveFrom);
						folderMoveFrom.Attributes = folderMoveFrom.Attributes & ~FileAttributes.ReadOnly;

						Directory.Move(moveFrom, pathOldAssetsFolder);
						Debug.Log(gyaAbbr + " - Moved Old Assets folder: " + moveFrom + "\n\tTo: " + pathOldAssetsFolder);

					}
					catch (IOException ex)
					{
						Debug.LogError(gyaAbbr + " - IOException Moving: " + moveFrom + " to " + pathOldAssetsFolder + "\n" + ex.Message);
						Debug.LogWarning(gyaAbbr + " -\tOld Assets folder contents were NOT moved!  Please move them manually.\n");
						Debug.LogWarning(gyaAbbr + " -\tYou can use 'Menu->Maintenance->Merge Old Assets from Old GYA Versions'.\n\tThis will Copy then Delete the outdated folder to prevent issues with SymLinks.\n");
						//GUIOverride(OverrideReason.MoveOldAssetFolder);
					}
					catch (UnauthorizedAccessException ex)
					{
						Debug.LogError(gyaAbbr + " - UnauthorizedAccessException Moving: " + moveFrom + " to " + pathOldAssetsFolder + "\n" + ex.Message);
						Debug.LogWarning(gyaAbbr + " -\tOld Assets folder contents were NOT moved!  Please move them manually.\n");
						Debug.LogWarning(gyaAbbr + " -\tYou can use 'Menu->Maintenance->Merge Old Assets from Old GYA Versions'.\n\tThis will Copy then Delete the outdated folder contents to prevent issues with SymLinks.\n");
						//GUIOverride(OverrideReason.MoveOldAssetFolder);
					}
				}
				else
				{
					// is symlink and Old Asset is not where it belongs, notify user
					//Debug.LogWarning(gyaAbbr + " -\tSymLink Detected:\t'" + GYAExt.PathUnityAssetStore + "'\n\tSymLink Redirects:\t'" + symlinkTargetAS + "'");
					Debug.LogWarning(gyaAbbr + " -\tSymLink Detected for:\t'" + GYAExt.PathUnityAssetStore + "'\n");
					//Debug.LogWarning(gyaAbbr + " -\tPlease check the GYA window for details.\n");
					Debug.LogWarning(gyaAbbr + " -\tOld Assets folder contents were NOT moved!  Please move them manually.\n" + "\tSetup any SymLink for the Old Assets folder prior to performing the next step.\n");
					Debug.LogWarning(gyaAbbr + " -\tYou can use 'Menu->Maintenance->Merge Old Assets from Old GYA Versions'.\n\tThis will Copy then Delete the outdated folder to prevent issues with SymLinks.\n");

					//GUIOverride(OverrideReason.MoveOldAssetFolder);
					//return;
				}

			}

			// Check for and create if not already exists
			CreateFolder(pathOldAssetsFolder);


			// --------------------

			//// Is AS folder a symlink and Old Asset is not where it belongs, notify user and pause GYA
			//if (isSymLinkPathAS && !Directory.Exists(pathOldAssetsFolder)) {
			//	Debug.LogWarning(gyaAbbr + " -\tSymLink Detected:\t'" + GYAExt.PathUnityAssetStore + "'\n\tSymLink Redirects to:\t'" + symlinkTargetAS + "'");
			//	Debug.LogWarning(gyaAbbr + " -\tPlease check the GYA window for details.\n");

			//	GUIOverride(OverrideReason.MoveOldAssetFolder);
			//	return;
			//}

		}

		public void GYAStatus (object pObj)
		{
			GYAStatus();
		}

		public void GYAStatus ()
		{
			// Display Main variables
			Debug.Log(gyaAbbr + " " + gyaVersion + " - Status Info\n" +
				//"\npackageShow[i].unity_version.Substring(0,3):\t" + packageShow[2].unity_version.Substring(0,3) +
				"\nApplication.unityVersion:\t\t" + Application.unityVersion +

				"\n\nGYAExt.PathUnityAppFolder:\t" + GYAExt.PathUnityAppFolder +
				"\nGYAExt.PathUnityStandardAssets:\t" + GYAExt.PathUnityStandardAssets +
				"\nGYAExt.PathGYADataFiles:\t\t" + GYAExt.PathGYADataFiles +
				"\nGYAExt.PathUnityAssetStore:\t" + GYAExt.PathUnityAssetStore +
				"\nGYAExt.PathUnityAssetStore5:\t" + GYAExt.PathUnityAssetStore5 +
				"\nGYAExt.PathUnityProject:\t\t" + GYAExt.PathUnityProject +
				"\nGYAExt.PathUnityProjectAssets:\t" + GYAExt.PathUnityProjectAssets +

				"\n\npathOldAssetsFolderName:\t" + pathOldAssetsFolderName +
				"\njsonFilePackagesName:\t\t" + jsonFilePackagesName +
				"\njsonFileUserName:\t\t" + jsonFileUserName +

				"\n\npathOldAssets:\t\t\t" + pathOldAssetsFolder +
				"\njsonFilePackages:\t\t\t" + jsonFilePackages +
				"\njsonFileUser:\t\t\t" + jsonFileUser +

				//"\n\nlogin.file:\t\t\t" + GYAStore.asVar.login.file +
				//"\ndownloads.file:\t\t\t" + GYAStore.asVar.downloads.file +
				//"\nwishlist.file:\t\t\t" + GYAStore.asVar.wishlist.file +

				"\n\nisSymLinkPathAS:\t\t\t" + isSymLinkPathAS.ToString() +
				"\nsymlinkTargetAS:\t\t\t" + symlinkTargetAS +
				"\nisSymLinkPathAS5:\t\t" + isSymLinkPathAS5.ToString() +
				"\nsymlinkTargetAS5:\t\t" + symlinkTargetAS5 + "\n");

			//Debug.Log("IsSymLink:\t\t\t" + GYAIO.IsSymLink(pathOldAssetsFolder) +
			//	"\nSymLinkTarget:\t\t\t" + GYAIO.GetSymLinkTarget(pathOldAssetsFolder) + "\n");

			//Debug.Log("\nDatePublish: " + packageShow[0].fileDataCreated);
			//Debug.Log("\nDatePublish: " + packageShow[0].fileDateCreated);
			//Debug.Log("\nDatePublish: " + packageShow[0].pubdate);
			//Debug.Log("\nDatePublish: " + PubDateToDateTime(packageShow[1].pubdate) );

		}

		public DateTime PubDateToDateTime (string pubDate)
		{
			//string newDate = "";
			//Convert.ToDateTime(pubDate).ToString();
			DateTime time;
			if ( DateTime.TryParse(pubDate, out time) )
			{
				//DateTime time = DateTime.Parse(pubDate);
				return time;
			}
			else
			{
				//time = DateTime.Parse("0000-00-00");
				return time;
			}
			//newDate = time.ToString();
			//string mTimeInfo = packageShow[i].fileDataCreated.ToString("dd MMM yyyy");
			//return time;
		}

		//public void Awake () {
		//Debug.Log(gyaAbbr + " - GYA LOADED: " + PathOfThisScript);
		//}

		public void OnEnable ()
		{
			Instance = this;

			//var gyaStore = GYAStore.Instance.asVar;

			pathOldAssetsFolder = Path.GetFullPath(Path.Combine(GYAExt.PathGYADataFiles, pathOldAssetsFolderName));
			jsonFilePackages = Path.GetFullPath(Path.Combine(GYAExt.PathGYADataFiles, jsonFilePackagesName));
			jsonFileUser = Path.GetFullPath(Path.Combine(GYAExt.PathGYADataFiles, jsonFileUserName));

			OnEnableStep2();
		}

		public void OnEnableStep2 ()
		{
			GUIOverride(OverrideReason.None); // Reset Error state when refreshing from an override

			isSymLinkPathAS = GYAIO.IsSymLink(GYAExt.PathUnityAssetStore);
			isSymLinkPathAS5 = GYAIO.IsSymLink(GYAExt.PathUnityAssetStore5);
			symlinkTargetAS = GYAIO.GetSymLinkTarget(GYAExt.PathUnityAssetStore);
			symlinkTargetAS5 = GYAIO.GetSymLinkTarget(GYAExt.PathUnityAssetStore5);

			// Make sure folders exists
			//CreateFolder(pathOldAssetsFolder);

			LoadTextures ();
			SetStyles ();
			GYAPreProcessor();

			//GUIOverride(OverrideReason.Error); // Testing only

			if (guiOverride)
			{
				return;
			}

			// If data files exists load it, else create it
			if (Directory.Exists (GYAExt.PathGYADataFiles))
			{
				LoadUser ();
				//if (File.Exists (jsonFilePackages) && !userData.Settings.refreshOnStartup) {
				if (File.Exists (jsonFilePackages))
				{
					LoadJSON ();
				}
				else
				{
					//Debug.Log(gyaAbbr + " - Creating Data File: " + jsonFilePackages + "\n");
					RefreshPackages ();
				}
			}
			else
			{
				GUIOverride(OverrideReason.Error);
				Debug.LogWarning(gyaAbbr + " - Error Folder: " + GYAExt.PathGYADataFiles + " does not exist.\n");

			}

			if (!guiOverride)
			{
				//  scan for project unitypackges
				ScanProject();
				// Force rescan of Standard Packages/Sample Assets folder to handle changing between Unity 5 and older versions as GYA uses the same data BUT Unity has differing fodersbetween versions
				ScanStandard();

				GroupUpdatePkgData();
				CheckIfGUISkinHasChanged(true);
				BuildPrevNextList();
				SVShowCollection(showActive);
			}
		}

		// Override GUI for displaying other info in the main frame
		private void GUIOverride (OverrideReason pReason)
		{
			// Set reason for GUIOverrideHandler
			overrideReason = pReason;
			if (overrideReason == OverrideReason.None)
			{
				guiOverride = false;
			}
			else
			{
				guiOverride = true;
			}
		}

		// Stop overriding the GUI main frame and reset text field
		private void GUIOverrideReset ()
		{
			// End Override
			GUIUtility.keyboardControl = 0; // Cause nullRef error in Unity 3 if in OnEnable
			fldOverrideEntry = String.Empty;
			GUIOverride(OverrideReason.None);
		}

		private void GUIOverrideHandler()
		{

			// Reason: MoveOldAssetFolder
			if (overrideReason == OverrideReason.MoveOldAssetFolder)
			{
				string errorMsg = "\nUnable to move GYA data files/folder.\n";
				errorMsg = errorMsg + "Check the log for any relevant messages.\n\n";
				errorMsg = errorMsg + "If you have created a SymLink/Junction for your Asset Store folder, please manually move the following files/folder:\n\n";
				errorMsg = errorMsg + "OLD ASSETS FOLDER:\nThe contents of the '-Grab Yer Assets (Old)' folder should be moved to the '../Unity/Grab Yer Assets/Asset Store-Old' folder.\n";
				//errorMsg = errorMsg + "Old: '../Unity/Asset Store/-Grab Yer Assets (Old)'\n";
				//errorMsg = errorMsg + "Or: '../Unity/-Grab Yer Assets (Old)'\n";
				//errorMsg = errorMsg + "To: '../Unity/GYA/Old Assets'\n\n";
				errorMsg = errorMsg + "\nUSER FILE:\nShould alredy be moved, but just in case.\n";
				errorMsg = errorMsg + "\nMove/Rename the User File:\n";
				errorMsg = errorMsg + "From\t: '../Unity/Asset Store/Grab Yer Assets User.json'\n";
				errorMsg = errorMsg + "-OR-\t: '../Unity/Grab Yer Assets User.json'\n\n";
				errorMsg = errorMsg + "To\t: '../Unity/Grab Yer Assets/GYA Settings.json'\n\n";
				//errorMsg = errorMsg + "File:   '" + jsonFilePackagesName + "'\n\n";
				//errorMsg = errorMsg + "\nFrom Old location:\t'../Unity/Asset Store/' or '../Unity/'\n";
				//errorMsg = errorMsg + "To New location:\t'../Unity/'\n";
				//errorMsg = errorMsg + "A future version will handle this better!\n";
				errorMsg = errorMsg + "\nOnce done, please verify that any created groups are showing up as expected.\n";
				errorMsg = errorMsg + "\nIf the error persists, please include any error messages when contacting: support@unity.xeir.com";
				errorMsg = errorMsg + "";

				EditorGUI.LabelField( new Rect(4,  wTop, position.width-8, controlHeight), "Error encountered:");
				EditorGUI.TextArea( new Rect(4,  wTop + controlHeight, position.width-8, position.height-(wTop + controlHeight + 4)), errorMsg, EditorStyles.wordWrappedLabel);
			}

			// Reason: Error
			if (overrideReason == OverrideReason.Error || overrideReason == OverrideReason.ErrorStep2)
			{
				string errorMsg = "\nUnable to display asset data due to an error.\n\n";
				errorMsg = errorMsg + "Please check the console log for any relevant messages.\n\n";
				errorMsg = errorMsg + "Click on \"Refresh\" to manaully rebuild the \"Grab Yer Assets.json\" file.\n\n";
				errorMsg = errorMsg + "If the error persists, please include any error messages when contacting: support@unity.xeir.com";
				errorMsg = errorMsg + "";

				EditorGUI.LabelField( new Rect(4,  wTop, position.width-8, controlHeight), "Error encountered:");
				EditorGUI.TextArea( new Rect(4,  wTop + controlHeight, position.width-8, position.height-(wTop + controlHeight + 4)), errorMsg, EditorStyles.wordWrappedLabel);
			}

			// Reason: GroupCreate
			if (overrideReason == OverrideReason.GroupCreate)
			{
				int grpNext = userData.Group.Count();
				wTop = wTop + controlHeight;

				// Cancel
				if( GUI.Button(new Rect(position.width-54,wTop,50,18),"Cancel") )
				{
					// End Override
					GUIOverrideReset();
				}
				// OK
				//GUI.Label( new Rect(0,100,200,100), guiLabel );
				if( GUI.Button(new Rect(4,wTop,50,18),"OK") )
				{
					// Create default group with name if no name is passed
					if (fldOverrideEntry == String.Empty)
					{
						fldOverrideEntry = "New Group " + grpNext.ToString();
					}

					GroupCreate(fldOverrideEntry);
					BuildPrevNextList();
					// End Override
					GUIOverrideReset();
				}

				wTop = wTop + (controlHeight * 2);
				EditorGUI.LabelField( new Rect(4,  wTop, position.width-8, controlHeight), "Enter New Group Name:");
				fldOverrideEntry = EditorGUI.TextField( new Rect(4,  wTop + controlHeight, position.width-8, controlHeight), fldOverrideEntry);
				wTop = wTop + (controlHeight * 3);
				EditorGUI.LabelField( new Rect(4,  wTop, position.width-8, controlHeight), "Follwing converisions will apply:");
				wTop = wTop + (controlHeight);
				EditorGUI.LabelField( new Rect(4,  wTop, position.width-8, controlHeight), "'&' = '+' and '/' = '\\'");
			}

			// Reason: GroupRename
			if (overrideReason == OverrideReason.GroupRename)
			{
				int passedGrp = Convert.ToInt32(fldOverridePassed);
				wTop = wTop + controlHeight;

				// Cancel
				if( GUI.Button(new Rect(position.width-54,wTop,50,18),"Cancel") )
				{
				// End Override
					GUIOverrideReset();
				}
				// OK
				if( GUI.Button(new Rect(4,wTop,50,18),"OK") )
				{
					// Create default group with name if no name is passed
					if (fldOverrideEntry == String.Empty)
					{
						fldOverrideEntry = "New Group " +  passedGrp.ToString();
					}

					GroupRename(passedGrp, EscapeMenuItem(fldOverrideEntry) );
					// End Override
					GUIOverrideReset();
				}

				wTop = wTop + (controlHeight * 2);
				EditorGUI.LabelField( new Rect(4,  wTop, position.width-8, controlHeight), "Rename Group:");
				fldOverrideEntry = EditorGUI.TextField( new Rect(4,  wTop + controlHeight, position.width-8, controlHeight), fldOverrideEntry);
				wTop = wTop + (controlHeight * 3);
				EditorGUI.LabelField( new Rect(4,  wTop, position.width-8, controlHeight), "Follwing converisions will apply:");
				wTop = wTop + (controlHeight);
				EditorGUI.LabelField( new Rect(4,  wTop, position.width-8, controlHeight), "'&' = '+' and '/' = '\\'");
			}

			// Reason: UserFolder
			if (overrideReason == OverrideReason.UserFolder)
			{
				wTop += controlHeight;

				// Cancel
				//if( GUI.Button(new Rect(position.width-54,wTop,50,18),"Cancel") ) {
				//	// End Override
				//	GUIOverrideReset();
				//}
				// OK
				//GUI.Label( new Rect(0,100,200,100), guiLabel );
				if( GUI.Button(new Rect(4,wTop,50,18),"Done") )
				{
					// Remove blank entry
					userData.Settings.pathUserAssetsList.RemoveAll(x => x == "");
					userData.Settings.pathUserAssetsList = userData.Settings.pathUserAssetsList.Distinct().ToList();

					SaveUser();
					RefreshPackages();
					BuildPrevNextList();
					// End Override
					GUIOverrideReset();
				}
				wTop += controlHeight*2;

				//EditorGUI.LabelField( new Rect(60, wTop, position.width-8, controlHeight), "'Copy To User Folder' goes to Main.");
				//wTop += controlHeight*2;

				EditorGUI.LabelField( new Rect(4,  wTop, position.width-8, controlHeight), "Main User Folder: (For 'Copy To User')");
				wTop += controlHeight;

				// Main User Path
				if( GUI.Button(new Rect(4,wTop,35,18),"Set") )
				{
					string pathMain = EditorUtility.SaveFolderPanel(gyaAbbr + " Select User Assets Folder:", userData.Settings.pathUserAssets, "");
					if (pathMain != "")
					{
						userData.Settings.pathUserAssets = pathMain;
					}
				}
				if (userData.Settings.pathUserAssets != "")
				{
					if( GUI.Button(new Rect(40,wTop,20,18),"-") )
					{
						userData.Settings.pathUserAssets = "";
					}
				}
				EditorGUI.LabelField(new Rect(60, wTop, position.width-65, controlHeight*2), userData.Settings.pathUserAssets);

				// Make sure userData.Settings.pathUserAssetsList.Count != 0
				//if (userData.Settings.pathUserAssetsList.Count == 0) {
				//	userData.Settings.pathUserAssetsList.Add("");
				//}

				// Add entry if last record is not already added
				if (userData.Settings.pathUserAssetsList.Count == 0 || userData.Settings.pathUserAssetsList[userData.Settings.pathUserAssetsList.Count-1] != "")
				{
					userData.Settings.pathUserAssetsList.Add("");
				}

				wTop += controlHeight*2;
				EditorGUI.LabelField( new Rect(4,  wTop, position.width-8, controlHeight), "Alternate User Folders:");
				// Alternate User Paths
				for (int i = 0; i < userData.Settings.pathUserAssetsList.Count; i++)
				{
					wTop += controlHeight;
					if( GUI.Button(new Rect(4,wTop,35,18),"Set") )
					{
						string pathTemp = EditorUtility.SaveFolderPanel(gyaAbbr + " Select User Assets Folder:", userData.Settings.pathUserAssetsList[i], "");
						if (pathTemp != "")
						{
							userData.Settings.pathUserAssetsList[i] = pathTemp;
						}

					}
					// Show - button if not last/empty entry
					if (userData.Settings.pathUserAssetsList[i] != "")
					{
						if( GUI.Button(new Rect(40,wTop,20,18),"-") )
						{
							//userData.Settings.pathUserAssetsList = "";
							userData.Settings.pathUserAssetsList.RemoveAt(i);
						}
					}
					EditorGUI.LabelField(new Rect(60, wTop, position.width-65, controlHeight*2), userData.Settings.pathUserAssetsList[i]);
				}
				//Debug.Log(userData.Settings.pathUserAssetsList.Count);

				wTop += controlHeight*2;
				EditorGUI.LabelField( new Rect(4,  wTop, position.width-8, controlHeight*3), "Notes:\nSub-folders will be scanned recursively.\nNetworked folders may affect scan time.");

			}
		}

		private string EscapeMenuItem (string pString)
		{
			pString = pString.Replace('/', '\\');
			//pString = pString.Replace("/", " \u2044 ");
			pString = pString.Replace('&', '+');
			pString = pString.Replace('%', ' ');
			//pString = pString.Replace("%", " \u2052 ");
			pString = pString.Replace('#', ' ');
			return pString;
		}

		// UpdateWindow called from OnGui
		private void UpdateWindow ()
		{
			// ToolBar Style
			tbStyle = new GUIStyle(EditorStyles.toolbar);
			tbStyle.alignment = TextAnchor.MiddleCenter;
			tbStyle.fontSize = 9;

			wTop = 0; // Vertical positioning
			svHeight = 0; // Reset vertical placement
			controlHeight = 18;
			int infoHeight = 18; // info 1 line

			// If foldInfo is true, adjust height of Info pane
			if (userData.Settings.showSVInfo)
			{
				infoHeight = infoHeight * 5; // info 4 lines
			}
			svHeight = (int)position.height - (controlHeight + infoHeight); // Scrollview height

			// Toolbar
			TBDrawLine1();
			wTop = controlHeight; // -1 to adjust for topbar

			// Check for GUI Override/Error State and prevent the rest of the GUI from displaying if so
			if (guiOverride)
			{

				GUIOverrideHandler();

			}
			else
			{
				// No override/error detected, continue processing
				// If CountToImport fails, it means something was not handled/caught early on, Just a precaution when making changes
				try
				{
					cntMarkedToImport = CountToImport();
				}
				catch (Exception ex)
				{
					Debug.LogError(gyaAbbr + " - UpdateWindow Error: Check previous error messages.\n" + ex.Message);
					GUIOverride(OverrideReason.ErrorStep2);
					//cntMarkedToImport = 0;
					return;
				}

				// BEGIN 2nd Toolbar
				TBDrawLine2();

				// Draw the list
				SVDraw ();

				// FOLDOUT - Asset Info
				wTop = (int)position.height - infoHeight;
				GUI.BeginGroup( new Rect(0, wTop, position.width, infoHeight), GUI.skin.box);

				bool _showSVInfo = userData.Settings.showSVInfo;
				//userData.Settings.showSVInfo = GUI.Toggle( new Rect(0, 0, position.width, controlHeight), userData.Settings.showSVInfo, "Click to " + (userData.Settings.showSVInfo ? "HIDE" : "SHOW") + " Package Info", tbStyle);
				Rect infoFoldOut =  new Rect(0, 0, position.width, controlHeight);

				// FoldOut click
				if ( (evt.type == EventType.MouseUp) && infoFoldOut.Contains(evt.mousePosition) )
				{
					//ddActive = false;
					// If right click, prevent toggling
					if (evt.button == 1)
					{
						userData.Settings.showSVInfo = !userData.Settings.showSVInfo;
						//GUI.Toggle( infoFoldOut, userData.Settings.showSVInfo, "Click to " + (userData.Settings.showSVInfo ? "HIDE" : "SHOW") + " Package Info", tbStyle);
					}
				}
				userData.Settings.showSVInfo = GUI.Toggle( infoFoldOut, userData.Settings.showSVInfo, "Click to " + (userData.Settings.showSVInfo ? "HIDE" : "SHOW") + " Package Info", tbStyle);

				if (_showSVInfo != userData.Settings.showSVInfo)
				{
					SaveUser();
				}

				if (userData.Settings.showSVInfo)
				{
					// This is so the info window shows blank when nothing has been initially selected
					// Show if Asset Store Package, if not then show minimal info
					GUI.Label( new Rect(4, controlHeight, position.width-8, infoHeight), infoToShow); // tbInfo
				}
				GUI.EndGroup();
			}
			//evt.Use();
			//ddActive = false; // Result = constant popup
		}

		// 1st Toolbar
		private void TBDrawLine1 ()
		{
			float xPos = 0;
			int xOffset = 6;
			int bWidth = 26; // Button Width
			int bSearch = 16; // Unity Search Icon
			//int bLabel = 10; // Unity Search Icon
			//int ddTextToShow = 9; // # of chars to show in DD

			// Calculate control layout

			GUI.BeginGroup( new Rect(0, wTop, position.width, controlHeight), EditorStyles.toolbar);
			GUI.EndGroup();
			xPos += xOffset;

			// Menu Button
			float menuPos = xPos;
			Rect menuButton = new Rect(xPos, wTop, bWidth, controlHeight);
			xPos += bWidth;

			// Category dropdown
			float catPos = xPos;
			Rect catButton = new Rect(xPos, wTop, bWidth, controlHeight);
			xPos += bWidth;

			// Publisher dropdown
			float pubPos = xPos;
			Rect pubButton = new Rect(xPos, wTop, bWidth, controlHeight);
			xPos += bWidth;

			// Search Button/Icon, If left-click show search popup
			xPos += xOffset;
			float searchPos = xPos;
			Rect searchButton = new Rect(xPos, wTop+2, bSearch, controlHeight);
			// Search field
			Rect searchField = new Rect(xPos, wTop+2, position.width-((xOffset+bSearch+bWidth)+(bWidth)+xPos), controlHeight);
			// Search Icon at the end of the search field, blank or cancel
			xPos = position.width-(xOffset+bSearch+bWidth*2);
			float searchEndPos = xPos;
			Rect searchEndButton = new Rect(searchEndPos, wTop+2, bSearch, controlHeight);

			// Refresh Button
			xPos += bSearch;
			float refreshPos = xPos;
			Rect refreshButton = new Rect(refreshPos, wTop, bWidth*2, controlHeight);

			// Process Events

			// Menu Button
			if ( (evt.type == EventType.MouseUp) && menuButton.Contains(evt.mousePosition) )
			{
				if (evt.button == 0 && !ddActive)
				{
					ddActive = true;
					if ( evt.alt )
					{
						GenericMenu infoMenu = new GenericMenu();
						// Experimental Menu
						infoMenu.AddDisabledItem(new GUIContent("EXPERIMENTAL TESTING:"));
						infoMenu.AddSeparator("");
						infoMenu.AddDisabledItem(new GUIContent("Old Asset Handling:"));
						//infoMenu.AddItem(new GUIContent("Auto Consolidate"), (userData.Settings.autoConsolidate), TBPopUpCallback, "AutoConsolidate");
						infoMenu.AddItem(new GUIContent("Auto Delete Consolidated"), (userData.Settings.autoDeleteConsolidated), TBPopUpCallback, "AutoDeleteConsolidated");
						//infoMenu.AddSeparator("");
						infoMenu.AddItem(new GUIContent("Auto Protect Asset Store Files"), (userData.Settings.autoPreventASOverwrite), TBPopUpCallback, "AutoPreventASOverwrite");
						infoMenu.AddSeparator("");
						infoMenu.AddDisabledItem(new GUIContent("ANYTHING IN THIS MENU IS FOR TESTING ONLY!"));
						infoMenu.AddDisabledItem(new GUIContent("USE THESE OPTIONS AT YOUR OWN RISK!"));

						infoMenu.DropDown(new Rect(menuPos, 0, 0, 18));
						//EditorGUIUtility.ExitGUI();
						//evt.Use();

					}
					else
					{
						// Standard Menu
						GenericMenu infoMenu = new GenericMenu();

						//infoMenu.AddDisabledItem(new GUIContent("Native AS Folder: " + GYAExt.FolderUnityAssetStoreActive));
						//infoMenu.AddSeparator("");

						// Open Folder/URL Options
						infoMenu.AddDisabledItem(new GUIContent("Open Folder:"));

						// Unity 5 Asset Store
						if (Directory.Exists(GYAExt.PathUnityAssetStore5))
						{
							infoMenu.AddItem(new GUIContent("Asset Store (Unity 5)" + (GYAExt.FolderUnityAssetStoreActive == GYAExt.FolderUnityAssetStore5 ? " *" : "") ), false, TBPopUpCallback, "AssetStoreFolder5");
						}
						else
						{
							infoMenu.AddDisabledItem(new GUIContent("Asset Store (Unity 5)"));
						}

						// Unity 4 Asset Store
						if (Directory.Exists(GYAExt.PathUnityAssetStore))
						{
							infoMenu.AddItem(new GUIContent("Asset Store (Unity 4)" + (GYAExt.FolderUnityAssetStoreActive == GYAExt.FolderUnityAssetStore ? " *" : "") ), false, TBPopUpCallback, "AssetStoreFolder");
						}
						else
						{
							infoMenu.AddDisabledItem(new GUIContent("Asset Store (Unity 4)"));
						}

						// Folder User
						if (pkgData.countUser != 0 || Directory.Exists(userData.Settings.pathUserAssets))
						{
							infoMenu.AddItem(new GUIContent("User Assets"), false, TBPopUpCallback, "UserAssetsFolder");
						}
						else
						{
							infoMenu.AddDisabledItem(new GUIContent("User Assets"));
						}
						if (pkgData.countStandard != 0 || Directory.Exists(GYAExt.PathUnityStandardAssets))
						{
							infoMenu.AddItem(new GUIContent("" + GYAExt.FolderUnityStandardAssets), false, TBPopUpCallback, "StandardAssetsFolder");
						}
						else
						{
							infoMenu.AddDisabledItem(new GUIContent("" + GYAExt.FolderUnityStandardAssets));
						}
						if (Directory.Exists(pathOldAssetsFolder))
						{
							infoMenu.AddItem(new GUIContent("Old Assets"), false, TBPopUpCallback, "OldAssetsFolder");
						}
						else
						{
							infoMenu.AddDisabledItem(new GUIContent("Old Assets"));
						}
						infoMenu.AddSeparator("");
						infoMenu.AddDisabledItem(new GUIContent("Open URL:"));

						// If NOT in error state
						if (overrideReason == OverrideReason.None)
						{
							infoMenu.AddItem(new GUIContent("Unity Asset Store"), false, TBPopUpCallback, "AssetStoreURL");
							infoMenu.AddItem(new GUIContent("Fast Asset Store"), false, TBPopUpCallback, "AssetStoreURL2");
							infoMenu.AddSeparator("");
							infoMenu.AddDisabledItem(new GUIContent("Store      : " + GYAExt.BytesToKB( pkgData.filesizeStore) ));
							infoMenu.AddDisabledItem(new GUIContent("User       : " + GYAExt.BytesToKB( pkgData.filesizeUser) ));
							infoMenu.AddDisabledItem(new GUIContent("Standard: " + GYAExt.BytesToKB( pkgData.filesizeStandard) ));
							infoMenu.AddDisabledItem(new GUIContent("Old        : " + GYAExt.BytesToKB( pkgData.filesizeOld) ));

							// Settings
							infoMenu.AddSeparator("");

							infoMenu.AddDisabledItem(new GUIContent("Settings/Application Options:"));
							//infoMenu.AddItem(new GUIContent("Settings/Refresh On Startup"), (userData.Settings.refreshOnStartup), TBPopUpCallback, "RefreshOnStartup");
							infoMenu.AddItem(new GUIContent("Settings/Persist Mode"), (userData.Settings.isPersist), TBPopUpCallback, "Persist");
							infoMenu.AddItem(new GUIContent("Settings/Report Asset Errors"), (userData.Settings.reportPackageErrors), TBPopUpCallback, "ToggleReportPackageErrors");

							infoMenu.AddItem(new GUIContent("Settings/"), false, TBPopUpCallback, "");
							infoMenu.AddDisabledItem(new GUIContent("Settings/Progress Bar Options:"));
							//openURLInUnity
							infoMenu.AddItem(new GUIContent("Settings/Show during Refresh"), (userData.Settings.showProgressBarDuringRefresh), TBPopUpCallback, "showProgressBarDuringRefresh");
							infoMenu.AddItem(new GUIContent("Settings/Show during File Actions"), (userData.Settings.showProgressBarDuringFileAction), TBPopUpCallback, "showProgressBarDuringFileAction");

							infoMenu.AddItem(new GUIContent("Settings/"), false, TBPopUpCallback, "");
							infoMenu.AddDisabledItem(new GUIContent("Settings/Asset Store Options:"));
							//openURLInUnity
							infoMenu.AddItem(new GUIContent("Settings/Open URLs In Unity"), (userData.Settings.openURLInUnity), TBPopUpCallback, "openURLInUnity");

							infoMenu.AddItem(new GUIContent("Settings/"), false, TBPopUpCallback, "");
							infoMenu.AddItem(new GUIContent("Settings/Scan All Asset Store Folders"), (userData.Settings.scanAllAssetStoreFolders), TBPopUpCallback, "ScanAllAssetStoreFolders");
							infoMenu.AddItem(new GUIContent("Settings/Show All Asset Store Folders"), (userData.Settings.showAllAssetStoreFolders), TBPopUpCallback, "ShowAllAssetStoreFolders");


							//if ( GYAExt.UnityVersionIsEqualOrNewerThan("5.4.0b13") )
							//{
								infoMenu.AddItem(new GUIContent("Settings/"), false, TBPopUpCallback, "");
								infoMenu.AddDisabledItem(new GUIContent("Settings/Multi-Import Override:"));

								// Default Import routine
							infoMenu.AddItem(new GUIContent("Settings/Default                 (Auto select)"), (userData.Settings.multiImportOverride == MultiImportType.Default), TBPopUpCallback, "DefaultImport");
								// New Unity Sync Import routine
								infoMenu.AddItem(new GUIContent("Settings/Unity Sync  5.4+"), (userData.Settings.multiImportOverride == MultiImportType.UnitySync), TBPopUpCallback, "UnitySync");
								//	infoMenu.AddDisabledItem(new GUIContent("Settings/Unity Sync  (5.4+)"));
								// GYA's Sync Import routine - Any Unity version can use
								infoMenu.AddItem(new GUIContent("Settings/GYA   Sync  5.0+"), (userData.Settings.multiImportOverride == MultiImportType.GYASync), TBPopUpCallback, "GYASync");
								// Ye olde standard/default Import routine - BROKEN in 5.3x - 5.4.0b12
							infoMenu.AddItem(new GUIContent("Settings/Unity Async 5.0+ (Except 5.3)"), (userData.Settings.multiImportOverride == MultiImportType.UnityAsync), TBPopUpCallback, "UnityAsync");
								//infoMenu.AddDisabledItem(new GUIContent("Settings/Unity Async (5.0+ except 5.3)"));
							//}


							infoMenu.AddItem(new GUIContent("Settings/"), false, TBPopUpCallback, "");
							infoMenu.AddDisabledItem(new GUIContent("Settings/Display Options:"));
							infoMenu.AddItem(new GUIContent("Settings/Nested Cat\\Pub Drop-downs"), (userData.Settings.nestedDropDowns), TBPopUpCallback, "ToggleNestedDropDowns");
							//infoMenu.AddItem(new GUIContent("Options/Nested Versions in List"), (userData.Settings.nestedVersions), TBPopUpCallback, "ToggleNestedVersions");
							infoMenu.AddItem(new GUIContent("Settings/"), false, TBPopUpCallback, "");
							infoMenu.AddItem(new GUIContent("Settings/Headers"), userData.Settings.enableHeaders, TBPopUpCallback, "Headers");
							infoMenu.AddItem(new GUIContent("Settings/Colors"), userData.Settings.enableColors, TBPopUpCallback, "Colors");
							infoMenu.AddItem(new GUIContent("Settings/Icons"), (userData.Settings.enableCollectionTypeIcons), TBPopUpCallback, "EnableCollectionTypeIcons");
							//infoMenu.AddItem(new GUIContent("Settings/Alt Icon Old Versions"), (userData.Settings.enableAltIconOldVersions), TBPopUpCallback, "enableAltIconOldVersions");
							infoMenu.AddItem(new GUIContent("Settings/"), false, TBPopUpCallback, "");
							infoMenu.AddDisabledItem(new GUIContent("Settings/Other Unity Versions:"));
							infoMenu.AddItem(new GUIContent("Settings/Show Alt Icons"), (userData.Settings.enableAltIconOtherVersions), TBPopUpCallback, "enableAltIconOtherVersions");
							//infoMenu.AddItem(new GUIContent("Settings/Auto Swap Alt Icon per Unity Version"), (userData.Settings.enableAltIconSwap), TBPopUpCallback, "enableAltIconSwap");

							infoMenu.AddItem(new GUIContent("Settings/"), false, TBPopUpCallback, "");
							infoMenu.AddDisabledItem(new GUIContent("Settings/Old Asset Handling:"));
							infoMenu.AddItem(new GUIContent("Settings/Auto Consolidate"), (userData.Settings.autoConsolidate), TBPopUpCallback, "AutoConsolidate");
							//infoMenu.AddItem(new GUIContent("Options/Auto Delete Consolidated"), (userData.Settings.autoDeleteConsolidated), TBPopUpCallback, "AutoDeleteConsolidated");
							//infoMenu.AddItem(new GUIContent("Options/"), false, TBPopUpCallback, "");
							//infoMenu.AddItem(new GUIContent("Options/Auto Protect Asset Store Files"), (userData.Settings.autoPreventASOverwrite), TBPopUpCallback, "AutoPreventASOverwrite");

							//infoMenu.AddItem(new GUIContent("Enable Auto Backup"), (userData.Settings.autoBackup), TBPopUpCallback, "AutoBackup");
							infoMenu.AddSeparator("");

							// Group Options
							infoMenu.AddItem(new GUIContent("Groups/Create"), false, TBPopUpCallback, "GroupCreate");
							// Group Rename
							if (userData.Group.Count > 1)
							{
								for (int i = 1; i < userData.Group.Count; ++i)
								{
									infoMenu.AddItem (new GUIContent ("Groups/Rename/" + i.ToString() + " - " + userData.Group[i].name), false, TBPopUpCallback, "GroupRename:" + i );
								}
							}
							else
							{
								infoMenu.AddDisabledItem(new GUIContent("Groups/Rename"));
							}
							infoMenu.AddItem(new GUIContent("Groups/"), false, TBPopUpCallback, "");
							// Group Delete
							if (userData.Group.Count > 1)
							{
								for (int i = 1; i < userData.Group.Count; ++i)
								{
									infoMenu.AddItem (new GUIContent ("Groups/Delete/" + i.ToString() + " - " + userData.Group[i].name), false, TBPopUpCallback, "GroupDelete:" + i );
								}
							}
							else
							{
								infoMenu.AddDisabledItem(new GUIContent("Groups/Delete"));
							}

							// Old Asset Options, If there are no Old Assets, don't show
							if ( pkgData.countOldToMove > 0 )
							{
								infoMenu.AddItem(new GUIContent("Old/Consolidate ( " + pkgData.countOldToMove + " ) Assets"), false, TBPopUpCallback, "MoveOldAssets");
							}
							else
							{
								infoMenu.AddDisabledItem(new GUIContent("Old/Consolidate ( " + pkgData.countOldToMove + " ) Assets"));
							}

							if (pkgData.countOld > 0)
							{
								infoMenu.AddItem(new GUIContent("Old/"), false, TBPopUpCallback, "");
								int fCount = CountAssetsInFolder(pathOldAssetsFolder);
								if (fCount > 0)
								{
									infoMenu.AddItem(new GUIContent("Old/Delete ( " + fCount + " ) Assets"), false, TBPopUpCallback, "OldAssetsDeleteAll");
								}
								else
								{
									infoMenu.AddDisabledItem(new GUIContent("Old/Delete ( " + fCount + " ) Assets"));
								}
							}
							else
							{
								infoMenu.AddDisabledItem(new GUIContent("Old"));
							}

							// User Asset Options
							infoMenu.AddSeparator("");
							infoMenu.AddItem(new GUIContent("Assign User Folders"), false, TBPopUpCallback, "UserFolder");
							//infoMenu.AddItem(new GUIContent("User File/Backup"), false, TBPopUpCallback, "BackupUserFile");
							//infoMenu.AddItem(new GUIContent("User File/Restore"), false, TBPopUpCallback, "RestoreUserFile");

							infoMenu.AddSeparator("");

							//infoMenu.AddItem(new GUIContent("Maintenance/"), false, TBPopUpCallback, "");
							infoMenu.AddDisabledItem(new GUIContent("Maintenance/General:"));
							infoMenu.AddItem(new GUIContent("Maintenance/User File/Backup"), false, TBPopUpCallback, "BackupUserFile");
							infoMenu.AddItem(new GUIContent("Maintenance/User File/Restore"), false, TBPopUpCallback, "RestoreUserFile");

							// Export CSV
							infoMenu.AddItem(new GUIContent("Maintenance/Export Asset Data as CSV"), false, TBPopUpCallback, "ExportAsCSV");

							infoMenu.AddItem(new GUIContent("Maintenance/"), false, TBPopUpCallback, "");
							infoMenu.AddDisabledItem(new GUIContent("Maintenance/Asset Store Specific:"));
							// DeleteEmptySubFolders
							//infoMenu.AddItem(new GUIContent("Maintenance/"), false, TBPopUpCallback, "");
							infoMenu.AddItem(new GUIContent("Maintenance/Delete Empty Asset Store Sub Folders"), false, TBPopUpCallback, "DeleteEmptySubFolders");

							// Both AS folders exists?
							if (Directory.Exists(GYAExt.PathUnityAssetStore) && Directory.Exists(GYAExt.PathUnityAssetStore5))
							{
								infoMenu.AddItem(new GUIContent("Maintenance/Move Asset Store Assets/To Unity 5 " + GYAExt.FolderUnityAssetStore5), false, TBPopUpCallback, "ASMoveASToAS5");
								infoMenu.AddItem(new GUIContent("Maintenance/Move Asset Store Assets/To Unity 4 " + GYAExt.FolderUnityAssetStore), false, TBPopUpCallback, "ASMoveAS5ToAS");
							}
							else
							{
								infoMenu.AddDisabledItem(new GUIContent("Maintenance/Move Asset Store Assets/To Unity 5 " + GYAExt.FolderUnityAssetStore5));
								infoMenu.AddDisabledItem(new GUIContent("Maintenance/Move Asset Store Assets/To Unity 4 " + GYAExt.FolderUnityAssetStore));
							}

							infoMenu.AddItem(new GUIContent("Maintenance/"), false, TBPopUpCallback, "");
							infoMenu.AddDisabledItem(new GUIContent("Maintenance/Offline Mode:"));
							infoMenu.AddItem(new GUIContent("Maintenance/Enable Offline Mode"), (userData.Settings.enableOfflineMode), TBPopUpCallback, "OfflineMode");
							infoMenu.AddItem(new GUIContent("Maintenance/Load Alternate GYA Data File"), false, TBPopUpCallback, "LoadAltDataFile");

							infoMenu.AddItem(new GUIContent("Maintenance/"), false, TBPopUpCallback, "");
							// Asset Store 3-4-5 File/Folder Options
							//Debug.Log("GYAExt.PathUnityAssetStore5 : " + GYAExt.PathUnityAssetStore5);

							//int countTmp = 0;
							//countTmp += GetAssetCountFromFolder(Path.Combine(GYAExt.PathUnityAssetStore, "-" + gyaName + " (Old)"));
							//countTmp += GetAssetCountFromFolder(Path.Combine(GYAExt.PathGYADataFilesOLD, "-" + gyaName + " (Old)"));
							//if (countTmp > 0) {
							//infoMenu.AddItem(new GUIContent("Maintenance/Merge Old Assets from Old GYA Versions (" + countTmp + ")"), false, TBPopUpCallback, "MoveFolderContentsOldAssets");
							infoMenu.AddDisabledItem(new GUIContent("Maintenance/GYA Specific:"));
							infoMenu.AddItem(new GUIContent("Maintenance/Merge Old Assets from Old GYA Versions"), false, TBPopUpCallback, "MoveFolderContentsOldAssets");
							//Clean up Outdated GYA Support Files
							infoMenu.AddItem(new GUIContent("Maintenance/Clean up Outdated GYA Support Files"), false, TBPopUpCallback, "CleanOutdatedGYASupportFiles");
							infoMenu.AddItem(new GUIContent("Maintenance/"), false, TBPopUpCallback, "");
							//infoMenu.AddSeparator("Maintenance/");
							//}

						}
						else
						{
							// In error state
							infoMenu.AddItem(new GUIContent("WARNING - GYA has detected an error!"), false, TBPopUpCallback, "");
						}
						// Version
						infoMenu.AddSeparator("");
						//infoMenu.AddDisabledItem(new GUIContent(gyaAbbr + " v" + gyaVersion));
						infoMenu.AddItem(new GUIContent(gyaAbbr + " v" + gyaVersion), false, GYAStatus, "");
						//infoMenu.AddItem(new GUIContent("Test: ProgressBar"), false, ProgressBarTest, "");

						// Offset menu from right of editor window
						infoMenu.DropDown(new Rect(menuPos, 0, 0, 18));
						//EditorGUIUtility.ExitGUI();
						//evt.Use();
					}
				//} else if (evt.button == 1 && !ddActive) {

				}
				else
				{
					ddActive = false; // Required by Win
				}
			}

			// Category dropdown
			if ( (evt.type == EventType.MouseUp) && catButton.Contains(evt.mousePosition) )
			{
				if (overrideReason == OverrideReason.None)
				{
					if (evt.button == 0 && !ddActive)
					{
						ddActive = true;
						GenericMenu infoMenu = new GenericMenu();
						// Option Menu
						infoMenu.AddItem(new GUIContent("ALL"), (ddCategory == ""), TBCatCallback, "ALL");
						infoMenu.AddSeparator("");
						infoMenu.AddDisabledItem(new GUIContent("Package Version:"));

						// Unity 5 Version Packages
						int u5verPackageCount = pkgData.Assets.FindAll(x => x.unity_version.StartsWith("5.")).Count;
						int u5verFolderCount = pkgData.Assets.FindAll(x => x.isInAS5Folder == true).Count;
						int u4verPackageCount = pkgData.Assets.FindAll(x => !x.unity_version.StartsWith("5.")).Count;
						//int u4verFolderCount = pkgData.Assets.FindAll(x => x.collection == svCollection.Store && !x.filePath.Contains(GYAExt.FolderUnityAssetStore5)).Count;
						int u4verFolderCount = pkgData.Assets.FindAll(x => x.isInASFolder == true).Count;

						//infoMenu.AddDisabledItem(new GUIContent("Unity 5:"));
						//pkgData.Assets[0].unity_version.StartsWith("5.")
						//if (u5verPackageCount > 0) {
						infoMenu.AddItem (new GUIContent (unity5VersionText + " .. (" + u5verPackageCount + ")"), (ddCategory == unity5VersionText), TBCatCallback, unity5VersionText );
						//} else {
						//	infoMenu.AddDisabledItem(new GUIContent(unity5VersionText));
						//}

						// Unity 3-4 Version Packages
						//infoMenu.AddDisabledItem(new GUIContent("Unity -4:"));
						//pkgData.Assets[0].unity_version.StartsWith("5.")
						//if (u4verPackageCount > 0) {
						//if (pkgData.Assets.FindAll(x => x.collection == svCollection.Store && !x.unity_version.StartsWith("5.")).Count > 0) {
						infoMenu.AddItem (new GUIContent (unityOlderVersionText + " .. (" + u4verPackageCount + ")"), (ddCategory == unityOlderVersionText), TBCatCallback, unityOlderVersionText );
						//} else {
							//infoMenu.AddDisabledItem(new GUIContent(unityOlderVersionText));
						//}
						infoMenu.AddSeparator("");
						infoMenu.AddDisabledItem(new GUIContent("Contained In Folder:"));

						// Unity 5 Folder Packages
						//if (u5verFolderCount > 0) {
						infoMenu.AddItem (new GUIContent (unity5FolderText + " .. (" + u5verFolderCount + ")"), (ddCategory == unity5FolderText), TBCatCallback, unity5FolderText );
						//} else {
							//infoMenu.AddDisabledItem(new GUIContent(unity5FolderText));
						//}

						// Unity 3-4 Folder Packages
						//if (pkgData.Assets.FindAll(x => !x.filePath.Contains("Asset Store-5.x")).Count > 0) {
						//if (u4verFolderCount > 0) {
						infoMenu.AddItem (new GUIContent (unityOlderFolderText + " .. (" + u4verFolderCount + ")"), (ddCategory == unityOlderFolderText), TBCatCallback, unityOlderFolderText );
						//} else {
							//infoMenu.AddDisabledItem(new GUIContent(unityOlderFolderText));
						//}
						infoMenu.AddSeparator("");
						infoMenu.AddDisabledItem(new GUIContent("Package Type:"));
						//infoMenu.AddItem(new GUIContent("ALL"), (ddCategory == ""), TBCatCallback, "ALL");
						//infoMenu.AddSeparator("");

						// Add category for Exported packages
						//if (pkgData.Assets.FindAll(x => x.isExported).Count > 0) {
						//	infoMenu.AddItem (new GUIContent (exportedText), (ddCategory == exportedText), TBCatCallback, exportedText );
						//} else {
						//	infoMenu.AddDisabledItem(new GUIContent(exportedText));
						//}
						// Add category for Unity packages
						if (pkgData.Assets.FindAll(x => !x.isExported).Count > 0)
						{
							infoMenu.AddItem (new GUIContent (packageText), (ddCategory == packageText), TBCatCallback, packageText );
						}
						else
						{
							infoMenu.AddDisabledItem(new GUIContent(packageText));
						}
						// Add category for Exported packages
						if (pkgData.Assets.FindAll(x => x.isExported).Count > 0)
						{
							infoMenu.AddItem (new GUIContent (exportedText), (ddCategory == exportedText), TBCatCallback, exportedText );
						}
						else
						{
							infoMenu.AddDisabledItem(new GUIContent(exportedText));
						}
						// Ungrouped
						infoMenu.AddItem (new GUIContent (ungroupedText), (ddCategory == ungroupedText), TBCatCallback, ungroupedText );
						// Damaged
						if (pkgData.Assets.FindAll(x => x.isDamaged).Count > 0)
						{
							infoMenu.AddItem (new GUIContent (damagedText), (ddCategory == damagedText), TBCatCallback, damagedText );
						}
						else
						{
							infoMenu.AddDisabledItem(new GUIContent(damagedText));
						}
						infoMenu.AddSeparator("");
						infoMenu.AddDisabledItem(new GUIContent("Categories:"));

						List<string> tempCat = new List<string>();
						if (!userData.Settings.nestedDropDowns)
						{

							// Categories List
							tempCat = CategoryListBuild();
							if (tempCat.Count > 0)
							{
								for (int i = 0; i < tempCat.Count; ++i)
								{
									//if (tempCat[i] != "Asset Store Tools") {
									infoMenu.AddItem (new GUIContent (tempCat[i]), (ddCategory == tempCat[i]), TBCatCallback, tempCat[i] );
									//}
								}
							}
							else
							{
								infoMenu.AddDisabledItem(new GUIContent("No Categories"));
							}
							//infoMenu.AddDisabledItem(new GUIContent("Sub-Categories:"));
						}

						// Sub-Category dropdown
						List<string> tempCatFull = new List<string>();
						List<string> tempCatRoot = new List<string>();
						IEnumerable<string> tempCatFinal = null;
						// Sub-Categories List
						if (!userData.Settings.nestedDropDowns)
						{
							// Standard List
							tempCat = SubCategoryListBuild();
							if (tempCat.Count > 0)
							{
								infoMenu.AddSeparator("");
								infoMenu.AddDisabledItem(new GUIContent("Sub-Categories:"));
							}
						}
						else
						{
							// Nested List
							tempCatFull = SubCategoryListBuildNested();
							tempCatRoot = SubCategoryListBuildNested(true); // Nested -1 last element
							tempCatFinal = tempCatFull.Except(tempCatRoot);

							foreach(var item in tempCatFinal)
							{
								tempCat.Add(item);
							}
						}

						if (tempCat.Count > 0)
						{
							for (int i = 0; i < tempCat.Count; ++i)
							{
								infoMenu.AddItem (new GUIContent (tempCat[i]), (ddCategory == tempCat[i]), TBCatCallback, tempCat[i].Replace('/', '\\') );
							}
						}
						else
						{
							infoMenu.AddDisabledItem(new GUIContent("No Sub-Categories"));
						}

						if (tempCatRoot.Count > 0)
						{
							for (int i = 0; i < tempCatRoot.Count; ++i)
							{
								infoMenu.AddItem(new GUIContent(tempCatRoot[i] + "/"), false, TBPopUpCallback, "");
								infoMenu.AddItem (new GUIContent (tempCatRoot[i] + "/All"), (ddCategory == tempCatRoot[i]), TBCatCallback, tempCatRoot[i].Replace('/', '\\') );
							}
						}
						infoMenu.DropDown(new Rect(catPos, 0, 0, 18));
						//EditorGUIUtility.ExitGUI();
						//evt.Use();
					//} else if (evt.button == 1 && !ddActive) {

					}
					else
					{
						ddActive = false; // Required by Win
					}
				}
			}

			// Publisher dropdown
			if ( (evt.type == EventType.MouseUp) && pubButton.Contains(evt.mousePosition) )
			{
				if (overrideReason == OverrideReason.None)
				{
					if (evt.button == 0 && !ddActive)
					{
						ddActive = true;
						GenericMenu infoMenu = new GenericMenu();
						// Option Menu
						infoMenu.AddItem(new GUIContent("ALL"), (ddPublisher == 0), TBCatCallback, "ALL");
						infoMenu.AddSeparator("");
						infoMenu.AddDisabledItem(new GUIContent("Publisher:"));

						// Publisher List
						var tempPub = PublisherListBuild();
						if (tempPub.Count > 0)
						{
							foreach (KeyValuePair<string, int> lineItem in tempPub)
							{
								if (!userData.Settings.nestedDropDowns)
								{
									// Show list
									infoMenu.AddItem (new GUIContent (lineItem.Key), (ddPublisher == lineItem.Value), TBPubCallback, lineItem.Value );
								}
								else
								{
									// Show list as sub-menus
									infoMenu.AddItem (new GUIContent (lineItem.Key.Substring(0,1).ToUpper()+"/"+lineItem.Key), (ddPublisher == lineItem.Value), TBPubCallback, lineItem.Value );
								}
							}
						}
						else
						{
							infoMenu.AddDisabledItem(new GUIContent("No Publishers"));
						}
						infoMenu.DropDown(new Rect(pubPos, 0, 0, 18));
						//EditorGUIUtility.ExitGUI();
						//evt.Use();
					//} else if (evt.button == 1 && !ddActive) {

					}
					else
					{
						ddActive = false; // Required by Win
					}
				}
			}

			// Search Button/Icon, If left-click show search popup
			if ( (evt.type == EventType.MouseUp) && searchButton.Contains(evt.mousePosition) )
			{
				if (overrideReason == OverrideReason.None)
				{
					if (evt.button == 0 && !ddActive)
					{
						ddActive = true;
						// Activate Popup menu
						GenericMenu searchMenu = new GenericMenu();
						searchMenu.AddDisabledItem(new GUIContent("Search:"));
						searchMenu.AddItem(new GUIContent("in Title"), IsSearchActive(svSearchBy.Title), PackagesSearchBy, svSearchBy.Title);
						searchMenu.AddItem(new GUIContent("in Category"), IsSearchActive(svSearchBy.Category), PackagesSearchBy, svSearchBy.Category);
						searchMenu.AddItem(new GUIContent("in Publisher"), IsSearchActive(svSearchBy.Publisher), PackagesSearchBy, svSearchBy.Publisher);
						searchMenu.AddSeparator("");
						searchMenu.AddDisabledItem(new GUIContent("Sort:"));
						searchMenu.AddItem(new GUIContent("by Title"), IsSortActive(svSortBy.Title), PackagesSortBy, svSortBy.Title);
						searchMenu.AddItem(new GUIContent("by Category"), IsSortActive(svSortBy.Category), PackagesSortBy, svSortBy.Category);
						searchMenu.AddItem(new GUIContent("by Subcategory"), IsSortActive(svSortBy.CategorySub), PackagesSortBy, svSortBy.CategorySub);
						searchMenu.AddItem(new GUIContent("by Publisher"), IsSortActive(svSortBy.Publisher), PackagesSortBy, svSortBy.Publisher);
						searchMenu.AddItem(new GUIContent("by Size"), IsSortActive(svSortBy.Size), PackagesSortBy, svSortBy.Size);
						searchMenu.AddItem(new GUIContent("by Date (File)"), IsSortActive(svSortBy.DateFile), PackagesSortBy, svSortBy.DateFile);
						searchMenu.AddItem(new GUIContent("by Date (Publish)"), IsSortActive(svSortBy.DatePublish), PackagesSortBy, svSortBy.DatePublish);
						searchMenu.AddItem(new GUIContent("by Date (Package Build)"), IsSortActive(svSortBy.DatePackage), PackagesSortBy, svSortBy.DatePackage);
						searchMenu.AddItem(new GUIContent("by Package ID"), IsSortActive(svSortBy.PackageID), PackagesSortBy, svSortBy.PackageID);
						searchMenu.AddItem(new GUIContent("by Version ID"), IsSortActive(svSortBy.VersionID), PackagesSortBy, svSortBy.VersionID);
						searchMenu.AddItem(new GUIContent("by Upload ID"), IsSortActive(svSortBy.UploadID), PackagesSortBy, svSortBy.UploadID);
						searchMenu.DropDown(new Rect(searchPos, 0, 0, 18));
						//EditorGUIUtility.ExitGUI();
						//evt.Use();
					//} else if (evt.button == 1 && !ddActive) {

					}
					else
					{
						ddActive = false; // Required by Win
					}
				}
			}

			// Refresh Button
			if ( (evt.type == EventType.MouseUp) && refreshButton.Contains(evt.mousePosition) )
			{
				//ddActive = false;
				if (evt.button == 0)
				{
					if (overrideReason != OverrideReason.MoveOldAssetFolder && overrideReason != OverrideReason.ErrorStep2)
					{
						// Reload Groups during refresh
						//userData = null;
						LoadUser ();
						RefreshPackages (true);
						GroupUpdatePkgData();
						BuildPrevNextList();

						if (userData.Settings.isPersist)
						{
							if (PersistEnable())
							{
								//RefreshPackages();	// Complete refresh
								ScanStandard();			// Scan just standard assets
								BuildPrevNextList();
								SVShowCollection(showActive);
							}
						}
						ddCategory = "";
						ddPublisher = 0;
					}
					else
					{
						// If in error state step2 restart gya from onenablestep2
						OnEnableStep2();
						//Path.Combine(PathOfThisScript,"GrabYerAssets.cs")
						//AssetDatabase.ImportAsset(PathOfThisScript, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive);
						//AssetDatabase.ImportAsset(PathOfThisScript, ImportAssetOptions.ForceUpdate);
					}

				}
				else if (evt.button == 1)
				{
					// right click

				}
			}

			// Draw controls as defined by the layout, processed after events or they will not trigger

			// Menu Menu (Gear) button
			GUI.Button( menuButton, iconMenu, EditorStyles.toolbarButton);
			// Category button
			GUI.Button( catButton, (ddCategory == "" ? iconCategory : iconCategoryX), EditorStyles.toolbarButton);
			// Publisher button
			GUI.Button( pubButton, (ddPublisher == 0 ? iconPublisher : iconPublisherX), EditorStyles.toolbarButton);
			// Search field
			//fldSearch = EditorGUI.TextField ( new Rect(searchPos, wTop+2, position.width-((xOffset+bSearch+bWidth)+(bWidth)+searchPos), controlHeight), fldSearch, GUI.skin.FindStyle("ToolbarSeachTextField"));
			if ( evt.type == EventType.MouseUp && searchField.Contains(evt.mousePosition) )
			{
				//ddActive = false;
			}
			fldSearch = EditorGUI.TextField ( searchField, fldSearch, GUI.skin.FindStyle("ToolbarSeachTextFieldPopup"));
			// Search magnifying button with down arrow, overlays default search icon
			//GUI.Button( searchButton, "", GUI.skin.FindStyle("ToolbarSeachTextFieldPopup"));
			if (fldSearch.Length > 0)
			{
				if (GUI.Button( searchEndButton, "", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
				{
					SearchClear();
				}
			}
			else
			{
				GUI.Button( searchEndButton, "", GUI.skin.FindStyle("ToolbarSeachCancelButtonEmpty"));
			}
			// Refresh button
			//GUI.Button( refreshButton, iconRefresh, EditorStyles.toolbarButton);
			// Refresh button and tooltip (Displays in the search bar)
			GUI.Button( refreshButton, new GUIContent (iconRefresh, "Refresh"), EditorStyles.toolbarButton);
			//DisplayToolTip (GUI.tooltip);
		}


		private void DisplayToolTip (string toolTip)
		{

			if (!String.IsNullOrEmpty(toolTip))
			{
				//GUI.Box(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, 200, 100), toolTip);
				//GUI.Label (new Rect (Screen.width-130,1,60,20), GUI.tooltip);
				//GUI.Box (new Rect (Screen.width-130,1,60,20), GUI.tooltip);
				//GUI.Box(new Rect (evt.mousePosition.x-60,evt.mousePosition.y-20,60,20), GUI.tooltip);
				// Works needs background
				//GUI.Box(new Rect (evt.mousePosition.x-66,evt.mousePosition.y+18,60,20), toolTip);
				GUI.Button(new Rect (evt.mousePosition.x-66,evt.mousePosition.y+18,60,20), toolTip, EditorStyles.toolbarButton);

			}
		}


		// 2nd Toolbar
		private void TBDrawLine2 ()
		{
			float xPos = 0;
			int xOffset = 6;
			int bWidth = 26; // Button Width

			// 2nd Toolbar Style
			tbStyleDD = new GUIStyle(EditorStyles.toolbarDropDown);
			tbStyleDD.alignment = TextAnchor.MiddleCenter;
			tbStyleDD.fontSize = 10;

			// Calculate control layout
			GUI.BeginGroup( new Rect(xPos, wTop, position.width, controlHeight), EditorStyles.toolbar);
			GUI.EndGroup();
			xPos += xOffset;

			// Menu Button
			float markedPos = xPos;
			Rect markedButton = new Rect(markedPos, wTop, bWidth*2, controlHeight);
			xPos += bWidth*2;

			// Reset button
			float resetPos = xPos;
			Rect resetButton = new Rect(resetPos, wTop, bWidth, controlHeight);
			xPos += bWidth;

			// Collection selection
			float collectionPos = xPos;
			Rect collectionButton = new Rect(collectionPos, wTop, position.width-(xPos*2-bWidth), controlHeight);

			// Prev
			float prevPos = position.width-(xOffset+bWidth*2); // Calculate from the right
			Rect prevButton = new Rect(prevPos, wTop, bWidth, controlHeight);
			xPos = prevPos + bWidth;

			// Next
			float nextPos = xPos; // Calculate from the right
			Rect nextButton = new Rect(nextPos, wTop, bWidth, controlHeight);

			// Process Events

			// Marked Assets Popup
			if ( (evt.type == EventType.MouseUp) && markedButton.Contains(evt.mousePosition) )
			{
				if (evt.button == 0 && !ddActive)
				{
					ddActive = true;
					TBPopUpMarked(markedPos);
				//} else if (evt.button == 1 && !ddActive) {

				}
				else
				{
					ddActive = false; // Required by Win
				}
			}

			// Reset button
			// If any of these settings are not at default value, highlight the icon
			bool resetActive = false;
			if (fldSearch != "" || searchActive != svSearchBy.Title || sortActive != svSortBy.Title || ddCategory != "" || ddPublisher != 0)
			{
				resetActive = true;
			}

			if ( (evt.type == EventType.MouseUp) && resetButton.Contains(evt.mousePosition) )
			{
				//ddActive = false;
				if (evt.button == 0)
				{
					//SVShowCollection(svCollection.All);
					SearchClear();
					PackagesSearchBy(svSearchBy.Title);
					PackagesSortBy(svSortBy.Title);
					ddCategory = "";
					ddPublisher = 0;
				}
				else if (evt.button == 1)
				{

				}
			}

			// Collection selection
			if ( (evt.type == EventType.MouseUp) && collectionButton.Contains(evt.mousePosition) )
			{
				if (evt.button == 0 && !ddActive)
				{
					ddActive = true;
					TBPopUpAssets(collectionPos);
				//} else if (evt.button == 1 && !ddActive) {

				}
				else
				{
					ddActive = false; // Required by Win
				}
			}

			// Prev
			if ( (evt.type == EventType.MouseUp) && prevButton.Contains(evt.mousePosition) )
			{
				//ddActive = false;
				if (evt.button == 0)
				{
					PrevSelection ();
				//} else if (evt.button == 1) {

				}
			}

			// Next
			if ( (evt.type == EventType.MouseUp) && nextButton.Contains(evt.mousePosition) )
			{
				//ddActive = false;
				if (evt.button == 0)
				{
					NextSelection ();
				//} else if (evt.button == 1) {

				}
			}

			// Draw controls as defined by the layout, processed after events or they will not trigger

			GUI.Button( markedButton, cntMarkedToImport.ToString(), tbStyleDD);
			GUI.Button( resetButton, (!resetActive ? iconReset : iconResetX), EditorStyles.toolbarButton);
			GUI.Button( collectionButton, selectionText, tbStyleDD);
			GUI.Button( prevButton, iconPrev, EditorStyles.toolbarButton);
			GUI.Button( nextButton, iconNext, EditorStyles.toolbarButton);
			//GUI.Button( nextButton, iconFavorite, EditorStyles.toolbarButton);
		}

		// Builds complete sorted category.label list
		private List<String> PublisherListBuildLIST ()
		{
			List<Packages> packagesTemp = new List<Packages> (pkgData.Assets);

			packagesTemp.RemoveAll(x => x.publisher.label == "");
			packagesTemp.Sort((x, y) => string.Compare(x.publisher.label, y.publisher.label));
			List<string> pkgList = packagesTemp.Select(p => p.publisher.label).Distinct().ToList();
			packagesTemp = null;

			// Find assets that match the category
			//string tmpCat = "";
			//List<Packages> resultsCat = packagesList.FindAll( delegate(Packages pk) { return (pk.category.label == tmpCat && pk.isassetstorepkg); } );

			// Categories
			for (int i = 0; i < pkgList.Count; ++i)
			{
				pkgList[i] = pkgList[i].Replace('/', '\\').Replace('&', '+');
				//Debug.Log(mainCategories[i]);
			}

			return pkgList;
		}

		// Builds complete sorted publisher.label/id dict
		private Dictionary<string,int> PublisherListBuild ()
		{
			List<Packages> packagesTemp = new List<Packages> (pkgData.Assets);
			packagesTemp.RemoveAll(x => x.publisher.label == "");
			packagesTemp.Sort((x, y) => string.Compare(x.publisher.label, y.publisher.label));


			IEnumerable<Packages> resultsDupes = from package in packagesTemp
				group package by package.publisher.label into grouped
				from package in grouped.Skip(0)
				select package;
			packagesTemp = null;

			List<Packages> resultsOld = resultsDupes.ToList();

			Dictionary<string, int> pkgList = new Dictionary<string, int>();
			// Build list minus dupes
			foreach(var result in resultsOld)
			{
				if (!pkgList.ContainsKey(result.publisher.label))
				{
					pkgList.Add(result.publisher.label, result.publisher.id);
				}
			}

			return pkgList;
		}

		// Builds complete sorted category.label list
		private List<string> CategoryListBuild ()
		{
			List<Packages> packagesTemp = new List<Packages> (pkgData.Assets);

			// Remove Exported entries as they do not have a category
			packagesTemp.RemoveAll(x => x.category.label == "");
			//packagesTemp.RemoveAll(x => x.category.label == "Asset Store Tools");
			packagesTemp.Sort((x, y) => string.Compare(x.category.label, y.category.label));
			List<string> pkgCategories = packagesTemp.Select(p => (p.category.label).Split('/')[0]).Distinct().ToList();
			packagesTemp = null;

			// Categories
			for (int i = 0; i < pkgCategories.Count; ++i)
			{
				pkgCategories[i] = pkgCategories[i].Replace('/', '\\').Replace('&', '+');
			}

			return pkgCategories;
		}

		// Builds complete sorted sub-category.label list
		private List<String> SubCategoryListBuild ()
		{
			List<Packages> packagesTemp = new List<Packages> (pkgData.Assets);

			// Remove Exported entries as they do not have a category
			packagesTemp.RemoveAll(x => x.category.label == "");
			// Remove any entries that are not a sub-category
			packagesTemp.RemoveAll(x => !x.category.label.Contains("/"));
			packagesTemp.Sort((x, y) => string.Compare(x.category.label, y.category.label));
			List<string> pkgCategories = packagesTemp.Select(p => p.category.label).Distinct().ToList();
			packagesTemp = null;

			// Sub-Categories
			for (int i = 0; i < pkgCategories.Count; ++i)
			{
				pkgCategories[i] = pkgCategories[i].Replace('/', '\\').Replace('&', '+');
			}

			return pkgCategories;
		}

		// Builds complete sorted sub-category.label list for Nested Sub-Menu's
		private List<String> SubCategoryListBuildNested (bool getRoot = false)
		{
			List<Packages> packagesTemp = new List<Packages> (pkgData.Assets);

			// Remove Exported entries as they do not have a category
			packagesTemp.RemoveAll(x => x.category.label == "");
			packagesTemp.Sort((x, y) => string.Compare(x.category.label, y.category.label));
			List<string> pkgCategories = packagesTemp.Select(p => p.category.label).Distinct().ToList();
			packagesTemp = null;

			// Sub-Categories
			for (int i = 0; i < pkgCategories.Count; ++i)
			{
				pkgCategories[i] = pkgCategories[i].Replace('&', '+');
			}
			//pkgCategories = pkgCategories.Distinct().ToList();
			//List<string> newList = CategoryListBuild();
			List<string> newList = new List<string>();
			//newList.Clear();
			//newList.AddRange(pkgCategories);
			//int newListLength = newList.Count();

			for (int i = 0; i < pkgCategories.Count; ++i)
			{
				int itemElements = pkgCategories[i].Split('/').Count();
				//Debug.Log("Elements: " + itemElements.ToString() + " - " + pkgCategories[i]);
				//eList += itemElements.ToString() + " - " + tempCat[i] + "\n";
				//Debug.Log( "i: " + i.ToString() );

				if (getRoot)
				{
				// Return category minus lastindex
					if ( itemElements > 1)
					{
						newList.Add( pkgCategories[i].Substring(0, pkgCategories[i].LastIndexOf('/')) );
					}
					else
					{
					//newList.Add( pkgCategories[i] );
					}
				}
				else
				{
					// Return full category
					newList.Add( pkgCategories[i] );
				}
			}
			newList = newList.Distinct().ToList();
			newList.Sort();
			pkgCategories = null;

			return newList;
			//return pkgCategories;
		}

		private void PrevSelection()
		{
			//		(int)showActive = 0 = All, 1 = Store, 2 = Standard, 3 = User, 4 = Old, 5 = Groups
			//		popupView.ElementAt(currentSelection).Key
			//		0-4 = Selections
			//		5+  = Groups, starting with Favorites

			int itemActive = (int)showActive;

			if (itemActive == 0)
			{	// Selection All Packages
				itemActive = 5;		// Selection Groups
				showGroup = userData.Group.Count-1;	// Group Last
			}
			else
			{
				if (itemActive == 5 && showGroup > 0)
				{	// If Groups and current group != 0
					showGroup -= 1;	// Group Prev
				}
				else
				{
					itemActive -= 1;	// Selection Prev
				}
			}

			switch (itemActive)
			{
			case 0:	// Selections
				if (pkgData.countAll > 0)
				{
					SVShowCollection(svCollection.All);
					break;
				}
				goto case 5;
			case 1:
				if (pkgData.countStore > 0)
				{
					SVShowCollection(svCollection.Store);
					break;
				}
				goto case 0;
			case 2:
				if (pkgData.countUser > 0)
				{
					SVShowCollection(svCollection.User);
					break;
				}
				goto case 1;
			case 3:
				if (pkgData.countStandard > 0)
				{
					SVShowCollection(svCollection.Standard);
					break;
				}
				goto case 2;
			case 4:
				if (pkgData.countOld > 0)
				{
					SVShowCollection(svCollection.Old);
					break;
				}
				goto case 3;
			case 5:	// Groups
				if (userData.Group.Count > 0)
				{
					SVShowCollection(svCollection.Group);
					break;
				}
				goto case 4;
			default:
				SVShowCollection(svCollection.All);
				break;
			}
		}

		private void NextSelection()
		{
			//		(int)showActive = 0 = All, 1 = Store, 2 = Standard, 3 = User, 4 = Old, 5 = Groups
			//		popupView.ElementAt(currentSelection).Key
			//		0-4 = Selections
			//		5+  = Groups, starting with Favorites

			int itemActive = (int)showActive;

			// Next
			if (itemActive != 5)
			{	// Selection Groups
				itemActive += 1;	// Selection Next
				showGroup = 0;	// Group First - Favorites
			}
			else
			{
				if (itemActive == 5 && showGroup < userData.Group.Count-1)
				{	// If Groups and current group != last group
					showGroup += 1;	// Next Group
				}
				else
				{
					itemActive = 0;	// Selection Asset Store
				}
			}

			switch (itemActive)
			{
			case 0:	// Selections
				if (pkgData.countAll > 0)
				{
					SVShowCollection(svCollection.All);
					break;
				}
				goto case 1;
			case 1:
				if (pkgData.countStore > 0)
				{
					SVShowCollection(svCollection.Store);
					break;
				}
				goto case 2;
			case 2:
				if (pkgData.countUser > 0)
				{
					SVShowCollection(svCollection.User);
					break;
				}
				goto case 3;
			case 3:
				if (pkgData.countStandard > 0)
				{
					SVShowCollection(svCollection.Standard);
					break;
				}
				goto case 4;
			case 4:
				if (pkgData.countOld > 0)
				{
					SVShowCollection(svCollection.Old);
					break;
				}
				goto case 5;
			case 5:	// Groups
				if (userData.Group.Count > 0)
				{
					SVShowCollection(svCollection.Group);
					break;
				}
				goto case 0;
			default:
				SVShowCollection(svCollection.All);
				break;
			}
		}

		// Builds same list as shown in Asset sections/groups popup
		internal void BuildPrevNextList ()
		{
			//if (!guiOverride) {	// Secondary check
			// Get current counts
			TallyAssets ();

			int iterateCount = 0;

			// Catch invalid data
			if (popupView != null)
			{
				popupView.Clear();
			}
			else
			{
				GUIOverride(OverrideReason.ErrorStep2);
				return;
			}

			popupView.Add (iterateCount, svCollection.All + " ( " + pkgData.countAll + " )");
			iterateCount += 1;
			popupView.Add (iterateCount, svCollection.Store + " ( " + pkgData.countStore + " )");
			iterateCount += 1;
			popupView.Add (iterateCount, svCollection.User + " ( " + pkgData.countUser + " )");
			iterateCount += 1;
			//popupView.Add (iterateCount, svCollection.Standard + " ( " + pkgData.countStandard + " )");
			popupView.Add (iterateCount, GYAExt.FolderUnityStandardAssets + " ( " + pkgData.countStandard + " )");
			iterateCount += 1;

			if (pkgData.countOldToMove > 0)
			{
				popupView.Add (iterateCount, svCollection.Old + " ( " + pkgData.countOld + " - " + pkgData.countOldToMove + " )" );
			}
			else
			{
				popupView.Add (iterateCount, svCollection.Old + " ( " + pkgData.countOld + " )");
			}
			iterateCount += 1;

			for (int i = 0; i < userData.Group.Count; ++i)
			{
				string grpLine = i.ToString() + " - " + userData.Group[i].name + " ( " + userData.Group[i].Assets.Count() + " )";
				popupView.Add (iterateCount, grpLine);
				iterateCount += 1;
			}
				PackagesSortBy(sortActive);
			//}
		}

		// Popup window routine for marked packages
		private void TBPopUpMarked (float xPos)
		{
			GenericMenu markedMenu = new GenericMenu();
			markedMenu.AddDisabledItem (new GUIContent ("Multiple Asset Options:"));
			//markedMenu.AddSeparator("");
			if (cntMarkedToImport > 0)
			{
				markedMenu.AddItem (new GUIContent ("Import Selected"), false, TBPopUpCallback, "PackageImportMultiple" );
				//markedMenu.AddSeparator("");
				if (showActive == svCollection.Group)
				{
					markedMenu.AddItem (new GUIContent ("Import Entire Group"), false, TBPopUpCallback, "PackageImportGroup" );
				}
				else
				{
					markedMenu.AddDisabledItem (new GUIContent("Import Entire Group"));
				}

				markedMenu.AddSeparator("");

				markedMenu.AddItem (new GUIContent ("Open URL of Selected Packages"), false, BrowseSelectedPackages, "BrowseSelectedPackages" );

				markedMenu.AddSeparator("");
				for (int i = 0; i < userData.Group.Count; ++i)
				{
					markedMenu.AddItem (new GUIContent ("Add to Group/" + i.ToString() + " - " + userData.Group[i].name), false, GroupAddToMultiple, i );
				}
				markedMenu.AddSeparator("");
				if (showActive == svCollection.Group)
				{
				// Remove asset from group
				//(showGroup == i && showActive == svCollection.Group)
					markedMenu.AddItem (new GUIContent ("Remove from Current Group"), false, GroupRemoveAssetMultiple, showGroup );
				}
				else
				{
					markedMenu.AddDisabledItem(new GUIContent("Remove from Current Group"));
				}
				markedMenu.AddSeparator("");
				// Copy To ...
				markedMenu.AddItem(new GUIContent ("File Options/Copy To User Folder"), false, TBPopUpCallback, "CopyToUserMulti");
				markedMenu.AddItem(new GUIContent ("File Options/Copy To ..."), false, TBPopUpCallback, "CopyToSelectableMulti");
				markedMenu.AddItem(new GUIContent ("File Options/"), false, TBPopUpCallback, "");
				markedMenu.AddItem (new GUIContent ("File Options/Rename with Version"), false, TBPopUpCallback, "RenameWithVersionSelected" );
				markedMenu.AddItem(new GUIContent ("File Options/"), false, TBPopUpCallback, "");
				markedMenu.AddItem (new GUIContent ("File Options/Delete ALL Selected"), false, TBPopUpCallback, "DeleteAssetMultiple" );
				markedMenu.AddItem(new GUIContent("Export Group Data as CSV"), false, TBPopUpCallback, "ExportAsCSVGroup");

				markedMenu.AddSeparator("");
				markedMenu.AddItem (new GUIContent ("Select All"), false, TBPopUpCallback, "SelectVisible" );
				markedMenu.AddItem (new GUIContent ("Invert Selections"), false, TBPopUpCallback, "InvertSelections" );
				markedMenu.AddItem (new GUIContent ("Clear"), false, TBPopUpCallback, "ClearMarked" );
			}
			else
			{
				markedMenu.AddDisabledItem (new GUIContent ("Import Selected"));
				//markedMenu.AddSeparator("");
				if (showActive == svCollection.Group)
				{
					markedMenu.AddItem (new GUIContent ("Import Entire Group"), false, TBPopUpCallback, "PackageImportGroup" );
					//markedMenu.AddSeparator("");
				}
				else
				{
					markedMenu.AddDisabledItem (new GUIContent("Import Entire Group"));
				}

				markedMenu.AddSeparator("");
				markedMenu.AddDisabledItem (new GUIContent ("Open URL of Selected Packages"));
				markedMenu.AddSeparator("");
				markedMenu.AddDisabledItem (new GUIContent ("Add to Group"));
				markedMenu.AddSeparator("");
				markedMenu.AddDisabledItem (new GUIContent ("Remove from Current Group"));
				markedMenu.AddSeparator("");
				//markedMenu.AddDisabledItem (new GUIContent ("File Options/Copy To User Folder"));
				//markedMenu.AddDisabledItem (new GUIContent ("File Options/Copy To ..."));
				markedMenu.AddDisabledItem (new GUIContent ("File Options/"));
				if (showActive == svCollection.Group)
				{
					markedMenu.AddItem(new GUIContent("Export Group Data as CSV"), false, TBPopUpCallback, "ExportAsCSVGroup");
				}
				else
				{
					markedMenu.AddDisabledItem(new GUIContent("Export Group Data as CSV"));
				}
				markedMenu.AddSeparator("");
				markedMenu.AddItem (new GUIContent ("Select All"), false, TBPopUpCallback, "SelectVisible" );
				markedMenu.AddDisabledItem (new GUIContent ("Invert Selections"));
				markedMenu.AddDisabledItem (new GUIContent ("Clear"));
			}
			//markedMenu.AddSeparator("");
			//markedMenu.AddItem (new GUIContent ("Select All"), false, TBPopUpCallback, "SelectVisible" );
			// Offset menu from right of editor window
			//markedMenu.DropDown(new Rect(6, 18, 0, 18));
			markedMenu.DropDown(new Rect(xPos, 18, 0, 18));
			//EditorGUIUtility.ExitGUI();
			//evt.Use();
		}

		// Popup window routine to Select which collection to show
		private void TBPopUpAssets (float xPos)
		{
		//Rect ddRect = new Rect(xPos, 18, 0, 18);
			//if (evt.type == EventType.MouseDown && ddRect.Contains(evt.mousePosition) ) {
			////	ddActive = false;
			//	//evt.Use();
			//}

			// Show Asset sections
			GenericMenu assetMenu = new GenericMenu ();

			if ( pkgData.countOldToMove > 0 )
			{
				//assetMenu.AddDisabledItem(new GUIContent("Old Assets Detected:"));
				assetMenu.AddItem(new GUIContent("Consolidate ( " + pkgData.countOldToMove + " ) Asset(s)"), false, TBPopUpCallback, "MoveOldAssets");
				assetMenu.AddSeparator("");
			}
			else
			{
				//assetMenu.AddDisabledItem(new GUIContent("Old/Consolidate ( " + pkgData.countOldToMove + " ) Assets"));
			}

			//assetMenu.AddDisabledItem(new GUIContent("Native AS Folder: " + (userData.Settings.scanAllAssetStoreFolders ? "BOTH" : GYAExt.FolderUnityAssetStoreActive) ));
			//assetMenu.AddDisabledItem(new GUIContent("Native AS Folder: " + GYAExt.FolderUnityAssetStoreActive));
			//assetMenu.AddSeparator("");
			assetMenu.AddDisabledItem(new GUIContent("Collections:"));
			//assetMenu.AddSeparator("");
			assetMenu.AddItem (new GUIContent (popupView.ElementAt(0).Value), (showActive == svCollection.All), TBPopUpCallback, "AllAssets" );
			assetMenu.AddItem (new GUIContent (popupView.ElementAt(1).Value), (showActive == svCollection.Store), TBPopUpCallback, "StoreAssets" );
			assetMenu.AddItem (new GUIContent (popupView.ElementAt(2).Value), (showActive == svCollection.User), TBPopUpCallback, "UserAssets" );
			assetMenu.AddItem (new GUIContent (popupView.ElementAt(3).Value), (showActive == svCollection.Standard), TBPopUpCallback, "StandardAssets" );
			assetMenu.AddItem (new GUIContent (popupView.ElementAt(4).Value), (showActive == svCollection.Old), TBPopUpCallback, "OldAssets" );
			assetMenu.AddSeparator("");

			assetMenu.AddDisabledItem(new GUIContent("Groups:"));
			for (int i = 0; i < userData.Group.Count; ++i)
			{
				string grpLine = popupView.ElementAt( 5 + i ).Value;
				assetMenu.AddItem (new GUIContent (grpLine), (showGroup == i && showActive == svCollection.Group), TBPopUpShowGroup, i );
			}
			//assetMenu.DropDown(new Rect(84, 18, 0, 18));
			assetMenu.DropDown(new Rect(xPos, 18, 0, 18));
			//assetMenu.DropDown(ddRect);
			//evt.Use();
		}

		// Group Popup Add To Menu Callback
		private void TBPopUpShowGroup (object obj)
		{
			showGroup = (int)obj;
			SVShowCollection(svCollection.Group);
			ddActive = false;
		}

		private void TBCatCallback (object obj)
		{
			//Debug.Log("TBCatCallback: " + obj.ToString());
			if ( obj.ToString() == "ALL" )
			{
				ddCategory = "";
				ddPublisher = 0;
			}
			else
			{
				//string exportedText = "- Exported Packages -";
				//if (obj.ToString() == exportedText) {
				//	ddCategory = obj.ToString();
				//} else {
				//Debug.Log(obj.ToString());
				ddCategory = obj.ToString();
				ddPublisher = 0;
				//}
			}
			ddActive = false;
		}

		private void TBPubCallback (object obj)
		{
			if ( obj.ToString() == "ALL" )
			{
				ddCategory = "";
				ddPublisher = 0;
			}
			else
			{
				//Debug.Log(obj.ToString());
				ddCategory = "";
				//ddPublisher = System.Int32.Parse(obj.ToString());
				ddPublisher = Convert.ToInt32(obj.ToString());
			}
			ddActive = false;
		}

		// Callback check from object
		private void TBPopUpCallback (object obj)
		{
			string passedVar = String.Empty;

			// If : next obj is a passed value
			if ( obj.ToString().Contains(":") )
			{
				passedVar = obj.ToString ().Split(':')[1];
			}

			if ( obj.ToString().Contains("BackupUserFile") )
			{
				//BackupUserData();
				if (EditorUtility.DisplayDialog(gyaAbbr + " - Backup User File",
					"Are you sure you want to BACKUP the User File?\n\n" +
					"This will save your current Settings & Groups.\n\n"
					, "Cancel", "BACKUP")) {
						// Cancel as default - Do nothing
				}
				else
				{
					BackupUserData();
				}
			}

			if ( obj.ToString().Contains("RestoreUserFile") )
			{
				//RestoreUserData();
				if (EditorUtility.DisplayDialog(gyaAbbr + " - Restore User File",
					"Are you sure you want to RESTORE the User File?\n\n" +
					"This will restore saved Settings & Groups.\n\n"
					, "Cancel", "RESTORE"))
				{
						// Cancel as default - Do nothing
				}
				else
				{
					RestoreUserData();
					RefreshPackages();
				}
			}

			// refreshOnStartup
			//if ( obj.ToString().Contains("RefreshOnStartup") ) {
			//	userData.Settings.refreshOnStartup = !userData.Settings.refreshOnStartup;
			//	Debug.Log(gyaAbbr + " - Force Refresh On Startup: " + (userData.Settings.refreshOnStartup ? "ENABLED" : "DISABLED") + "\nWhen ENABLED, this will Force a Refresh when starting up.");
			//	SaveUser();
			//	//RefreshPackages();
			//}

			//showProgressBarDuringRefresh
			if ( obj.ToString().Contains("showProgressBarDuringRefresh") )
			{
				userData.Settings.showProgressBarDuringRefresh = !userData.Settings.showProgressBarDuringRefresh;
				Debug.Log(gyaAbbr + " - Show Progress Bar During Refresh: " + (userData.Settings.showProgressBarDuringRefresh ? "ENABLED\n" : "DISABLED\n") );
				SaveUser();
			}

			//showProgressBarDuringFileAction
			if ( obj.ToString().Contains("showProgressBarDuringFileAction") )
			{
				userData.Settings.showProgressBarDuringFileAction = !userData.Settings.showProgressBarDuringFileAction;
				Debug.Log(gyaAbbr + " - Show Progress Bar During File Actions (Copy/Move): " + (userData.Settings.showProgressBarDuringFileAction ? "ENABLED\n" : "DISABLED\n") );
				SaveUser();
			}

			//openURLInUnity
			if ( obj.ToString().Contains("openURLInUnity") )
			{
				userData.Settings.openURLInUnity = !userData.Settings.openURLInUnity;
				Debug.Log(gyaAbbr + " - Open URLs In Unity: " + (userData.Settings.openURLInUnity ? "ENABLED\n" : "DISABLED\n") );
				SaveUser();
			}

			// Settings Menu - Scan All Asset Store Folders
			if ( obj.ToString().Contains("ScanAllAssetStoreFolders") )
			{
				userData.Settings.scanAllAssetStoreFolders = !userData.Settings.scanAllAssetStoreFolders;
				Debug.Log(gyaAbbr + " - Scan All Asset Store Folders: " + (userData.Settings.scanAllAssetStoreFolders ? "ENABLED - Refresh will now SCAN BOTH folders: '" + GYAExt.FolderUnityAssetStore + "' & '" + GYAExt.FolderUnityAssetStore5 + "'\nClick REFRESH to update your list!" : "DISABLED - Refresh will now SCAN ONLY the NATIVE folder: '" + GYAExt.FolderUnityAssetStoreActive + "'\nClick REFRESH to update your list!") );
				//"'\nNOTICE - The NATIVE folder is determined by which version of Unity is running.") );
				SaveUser();
				//RefreshPackages();
			}

			//showAllAssetStoreFolders
			if ( obj.ToString().Contains("ShowAllAssetStoreFolders") )
			{
				userData.Settings.showAllAssetStoreFolders = !userData.Settings.showAllAssetStoreFolders;
				Debug.Log(gyaAbbr + " - Show All Asset Store Folders: " + (userData.Settings.showAllAssetStoreFolders ? "ENABLED - Refresh will now SHOW BOTH folders: '" + GYAExt.FolderUnityAssetStore + "' & '" + GYAExt.FolderUnityAssetStore5 + "'\n" : "DISABLED - Refresh will now SHOW ONLY the NATIVE folder: '" + GYAExt.FolderUnityAssetStoreActive + "'\n") );
				//"'\nNOTICE - The NATIVE folder is determined by which version of Unity is running.") );
				SaveUser();
				//RefreshPackages();
			}

			// Default Import
			if ( obj.ToString().Contains("DefaultImport") )
			{
				userData.Settings.multiImportOverride = MultiImportType.Default;
				Debug.Log(gyaAbbr + " - Use Import Method: " + userData.Settings.multiImportOverride.ToString() + "\t\t(Auto-select the best option available)\n");
				SaveUser();
			}

			// UnitySync
			if ( obj.ToString().Contains("UnitySync") )
			{
				userData.Settings.multiImportOverride = MultiImportType.UnitySync;
				Debug.Log(gyaAbbr + " - Use Import Method: " + userData.Settings.multiImportOverride.ToString() + "\t(Unity 5.4+)\n");
				SaveUser();
			}

			// GYASync
			if ( obj.ToString().Contains("GYASync") )
			{
				userData.Settings.multiImportOverride = MultiImportType.GYASync;
				Debug.Log(gyaAbbr + " - Use Import Method: " + userData.Settings.multiImportOverride.ToString() + "\t\t(Unity 5.0+) <Only option for Unity 5.3.x>\n");
				SaveUser();
			}

			// UnityAsync
			if ( obj.ToString().Contains("UnityAsync") )
			{
				userData.Settings.multiImportOverride = MultiImportType.UnityAsync;
				Debug.Log(gyaAbbr + " - Use Import Method: " + userData.Settings.multiImportOverride.ToString() + "\t(Unity 5.0+) <Excepting Unity 5.3.x>\n");
				SaveUser();
			}

			// Persist in Standard Assets
			if (obj.ToString() == "Persist")
			{
				if (userData.Settings.isPersist)
				{
					// If enabled, verify disabling
					if (EditorUtility.DisplayDialog(gyaAbbr + " - Disable Persist Mode",
						"Are you sure you want to DISABLE Persist mode?\n\n" +
						gyaName + " will no longer be maintained in the Standard Assets folder.\n"
						, "Cancel", "DISABLE"))
					{
							// Cancel - do nothing
					}
					else
					{
						// Disable Persist mode
						PersistDisable();
						Debug.Log(gyaAbbr + " - Persist Mode has been Disabled.\n");
						SaveUser();
						//RefreshPackages();
						ScanStandard();
					}
				}
				else
				{
					// If disabled, verify enabling
					if (EditorUtility.DisplayDialog(gyaAbbr + " - Enable Persist Mode",
						gyaName + " will be copied to and maintained in the " + GYAExt.FolderUnityStandardAssets + " folder.\n\n" + "When a new version is downloaded from the Asset Store, simply click \"Refresh\" and " + gyaAbbr +
						" will automatically update the version in " + GYAExt.FolderUnityStandardAssets + "."
						+ "\n\nRequires that Unity's " + GYAExt.FolderUnityStandardAssets + " has already been installed."
						, "Cancel", "ENABLE"))
					{
						// Cancel - do nothing
					}
					else
					{
						// Check if SA needs to be created
						//CreateFolder(GYAExt.PathUnityStandardAssets);
						// Enable Persist mode
						Debug.Log(gyaAbbr + " - Persist Mode has been Enabled.\n");
						userData.Settings.isPersist = true;
						PersistEnable();
						SaveUser();
						//RefreshPackages();
						ScanStandard();
					}
				}
			}

			// Option Menu - Enable Auto Consolidate
			if ( obj.ToString().Contains("AutoPreventASOverwrite") )
			{
				if (userData.Settings.autoPreventASOverwrite)
				{
					// If enabled, verify disabling
					if (EditorUtility.DisplayDialog(gyaAbbr + " - Disable Version Protection",
						"Are you sure you want to DISABLE Version Protection for Asset Store files?\n\n" + "As noted when enabling, disabling will NOT revert the renamed files.\n"
						, "Cancel", "DISABLE"))
					{
							// Cancel - do nothing
					}
					else
					{
						userData.Settings.autoPreventASOverwrite = false;
						Debug.Log(gyaAbbr + " - Protect Current Versions from Overwrite: " + (userData.Settings.autoPreventASOverwrite ? "ENABLED" : "DISABLED") + " .. New package downloads will no longer be protected\n");
						SaveUser();
					}
				}
				else
				{
					// If disabled, verify enabling
					if (EditorUtility.DisplayDialog(gyaAbbr + " - Enable Version Protection",
						"WARNING: This will RENAME EVERY asset file in the Asset Store folder!\n\n" +
						"Once enabled, even disabling will NOT revert the renamed files.\n\n" +
						"Current Asset Versions will be maintained when updating.\n"
						, "Cancel", "ENABLE"))
					{
						// Cancel - do nothing
					}
					else
					{
						userData.Settings.autoPreventASOverwrite = true;
						Debug.Log(gyaAbbr + " - Protect Current Versions from Overwrite: " + (userData.Settings.autoPreventASOverwrite ? "ENABLED" : "DISABLED") + " .. Current packages in the Asset Store folder and future downloads will be protected.\nRefreshing Package List");
						RefreshPackages();
						SaveUser();
					}
				}
			}

			// Option Menu - Enable Auto Consolidate
			if ( obj.ToString().Contains("AutoConsolidate") )
			{
				if (userData.Settings.autoConsolidate)
				{
					// If enabled, verify disabling
					if (EditorUtility.DisplayDialog(gyaAbbr + " - Disable Auto Consolidate",
						"Are you sure you want to DISABLE Auto Consolidate?\n\n" + "\n"
						, "Cancel", "DISABLE"))
					{
							// Cancel - do nothing
					}
					else
					{
						userData.Settings.autoConsolidate = false;
						Debug.Log(gyaAbbr + " - Auto Consolidate: " + (userData.Settings.autoConsolidate ? "ENABLED" : "DISABLED") + "\n");
						SaveUser();
					}
				}
				else
				{
					// If disabled, verify enabling
					if (EditorUtility.DisplayDialog(gyaAbbr + " - Enable Auto Consolidate",
						"Are you sure you want to ENABLE Auto Consolidate?\n\n" +
						"Old files in the Asset Store folder will be automacially MOVED to the Old Assets folder whenever the list is refreshed."
						, "Cancel", "ENABLE"))
					{
							// Cancel - do nothing
					}
					else
					{
						userData.Settings.autoConsolidate = true;
						Debug.Log(gyaAbbr + " - Auto Consolidate: " + (userData.Settings.autoConsolidate ? "ENABLED\nOld assets will be consolidated every time the list is refresehed." : "DISABLED\n"));
						SaveUser();
					}
				}
			}

			// Option Menu - Enable Auto Delete of Consolidated Files
			if ( obj.ToString().Contains("AutoDeleteConsolidated") )
			{
				if (userData.Settings.autoDeleteConsolidated)
				{
					// If enabled, verify disabling
					if (EditorUtility.DisplayDialog(gyaAbbr + " - Disable Auto Delete Consolidated",
						"Are you sure you want to DISABLE Auto Deletion of Consolidated Assets?\n\n" + "\n"
						, "Cancel", "DISABLE"))
					{
							// Cancel - do nothing
					}
					else
					{
						userData.Settings.autoDeleteConsolidated = false;
						SaveUser();
					}
				}
				else
				{
					// If disabled, verify enabling
					if (EditorUtility.DisplayDialog(gyaAbbr + " - Enable Auto Delete Consolidated",
						"Are you sure you want to ENABLE Auto Deletion of Consolidated Assets?\n\n" +
						"Old files in the Asset Store folder will be automacially DELETED from the Old Assets folder."
						, "Cancel", "ENABLE"))
					{
							// Cancel - do nothing
					}
					else
					{
						userData.Settings.autoDeleteConsolidated = true;
						SaveUser();
					}
				}
			}

			// Option Menu - Toggle Nested Cat/Pub Drop-downs
			if ( obj.ToString().Contains("ToggleNestedDropDowns") )
			{
				userData.Settings.nestedDropDowns = !userData.Settings.nestedDropDowns;
				Debug.Log(gyaAbbr + " - Nested Drop-Downs: " + (userData.Settings.nestedDropDowns ? "ENABLED" : "DISABLED") + " for Category/Publisher lists.\n");
				SaveUser();
			}

			// Option Menu - Toggle Nested Versions in the list
			if ( obj.ToString().Contains("ToggleNestedVersions") )
			{
				userData.Settings.nestedVersions = !userData.Settings.nestedVersions;
				Debug.Log(gyaAbbr + " - Nested Versions: " + (userData.Settings.nestedVersions ? "ENABLED" : "DISABLED") + " in the list view.\n");
				SaveUser();
			}

			// Option Menu - Enable Reporting on Package Errors
			if ( obj.ToString().Contains("ToggleReportPackageErrors") )
			{

				if (!userData.Settings.reportPackageErrors)
				{
					userData.Settings.reportPackageErrors = !userData.Settings.reportPackageErrors;
					Debug.Log(gyaAbbr + " - Report Package Errors: " + (userData.Settings.reportPackageErrors ? "ENABLED" : "DISABLED") + 				"\nClick the REFRESH icon to test unitypackages for errors.  Errors will be listed in the console.");
				}
				else
				{
					userData.Settings.reportPackageErrors = !userData.Settings.reportPackageErrors;
					Debug.Log(gyaAbbr + " - Report Package Errors: " + (userData.Settings.reportPackageErrors ? "ENABLED" : "DISABLED") + "\n");
				}
				SaveUser();
			}

			// Toggle Display of Collection Type Icons in the List
			if ( obj.ToString().Contains("EnableCollectionTypeIcons") )
			{
				userData.Settings.enableCollectionTypeIcons = !userData.Settings.enableCollectionTypeIcons;
				SaveUser ();
			}

			//// enableAltIconOldVersions
			//if ( obj.ToString().Contains("enableAltIconOldVersions") ) {
			//	userData.Settings.enableAltIconOldVersions = !userData.Settings.enableAltIconOldVersions;
			//	Debug.Log(gyaAbbr + " - Alt Icons for Older Packages: " + (userData.Settings.enableAltIconOldVersions ? "ENABLED - Alt Icons will be shown for assets both OLDER and NEWER then the current Unity version." : "DISABLED - Alt Icons will be shown for assets that are NEWER then the current Unity version.") + "\n");
			//	SaveUser ();
			//}

			// enableAltIconOtherVersions
			if ( obj.ToString().Contains("enableAltIconOtherVersions") )
			{
				userData.Settings.enableAltIconOtherVersions = !userData.Settings.enableAltIconOtherVersions;
				Debug.Log(gyaAbbr + " - Alt Icons for Other Unity Versions: " + (userData.Settings.enableAltIconOtherVersions ? "ENABLED - Alt Icons will be shown for Other Unity Version Packages." : "DISABLED - Alt Icons will NOT be shown for Other Unity Version Packages.") + "\n");
				SaveUser ();
			}

			//// enableAltIconSwap
			//if ( obj.ToString().Contains("enableAltIconSwap") ) {
			//	userData.Settings.enableAltIconSwap = !userData.Settings.enableAltIconSwap;
			//	//Debug.Log(gyaAbbr + " - Swap Alt/Default Icons: " + (userData.Settings.enableAltIconSwap ? "ENABLED - Unity 5 = Alt Icons & Unity 4 = Default Icons." : "DISABLED - Unity 5 = Default Icons & Unity 4 = Alt Icons.") + "\n");
			//	Debug.Log(gyaAbbr + " - Swap Alt/Default Icons: " + (userData.Settings.enableAltIconSwap ? "ENABLED - Unity 5 = Alt Icons & Unity 4 = Default Icons." : "DISABLED - Unity 5 = Default Icons & Unity 4 = Alt Icons.") + "\n");
			//	LoadTextures();
			//	SaveUser ();
			//}

			// Toggle Headers
			if ( obj.ToString().Contains("Headers") )
			{
				userData.Settings.enableHeaders = !userData.Settings.enableHeaders;
				SaveUser ();
			}

			// Toggle Colors
			if ( obj.ToString().Contains("Colors") )
			{
				userData.Settings.enableColors = !userData.Settings.enableColors;
				CheckIfGUISkinHasChanged(true); // Force reload
				SaveUser ();
			}

			// Move asset to old assets folder - Used prior to updating a package from the Asset Store
			if (obj.ToString() == "MoveToOld")
			{
				if (EditorUtility.DisplayDialog(gyaAbbr + " - Move Selected Asset",
					"Are you sure you want to MOVE this asset?\n\n" +
					"This is handy to temporarily backup the asset just prior to downloading a new version.\n\n" +
					pkgDetails.title + "\n\nTo: " + pathOldAssetsFolder
					, "Cancel", "MOVE"))
				{
						// Cancel as default - Do nothing
				}
				else
				{
					var fileData = MoveAssetToPath(pkgDetails);
					if (fileData.Key > 0)
					{
						Debug.Log(gyaAbbr + " - ( " + fileData.Key.ToString() + " ) package(s) moved to the Old Assets Folder.\n" + fileData.Value);
					}
					RefreshPackages();
					//ScanOld();
				}
			}

			// Copy assets to user folder
			if (obj.ToString() == "CopyToUserMulti")
			{
				if (EditorUtility.DisplayDialog(gyaAbbr + " - Copy Multiple Assets To User folder",
					"Copying is NOT performed in a seperate thread at this time.\n\n" +
					"Unity may seem to pause during the copy.\n\n" +
					"This is here purely for convenience.\n\n" +
					"Copy To: " + userData.Settings.pathUserAssets
					, "Cancel", "COPY"))
				{
						// Cancel as default - Do nothing
				}
				else
				{
					CopyToSelected(userData.Settings.pathUserAssets);
					RefreshPackages();
					//ScanUser();
				}
			}

			// Copy assets to selectable folder
			if (obj.ToString() == "CopyToSelectableMulti")
			{
				string path = EditorUtility.SaveFolderPanel(gyaAbbr + " - Copy to Selected Folder", "", "");

				if (path.Length != 0)
				{
					CopyToSelected(path);
					//RefreshPackages();
				}
			}

			// Copy asset to selectable folder
			if (obj.ToString() == "CopyToSelectable")
			{
				string path = EditorUtility.SaveFolderPanel(gyaAbbr + " - Copy to Selected Folder", "", "");
				if (path.Length != 0)
				{
					var fileData = MoveAssetToPath(pkgDetails, path, true);
					if (fileData.Key > 0)
					{
						Debug.Log(gyaAbbr + " - ( " + fileData.Key.ToString() + " ) package(s) copied:\n" + fileData.Value + "\n");
					}
					//RefreshPackages();
				}
			}

			// Copy asset to User folder
			if (obj.ToString() == "CopyToUser")
			{
				var fileData = MoveAssetToPath(pkgDetails, userData.Settings.pathUserAssets, true);
				if (fileData.Key > 0)
				{
					Debug.Log(gyaAbbr + " - ( " + fileData.Key.ToString() + " ) package(s) copied to the User Assets Folder.\n" + fileData.Value);
				}
				RefreshPackages();
				//ScanUsers();
			}

			// Move asset to old assets folder - Used prior to updating a package from the Asset Store
			if (obj.ToString() == "DeleteAsset")
			{
				if (EditorUtility.DisplayDialog(gyaAbbr + " - Delete Selected Asset",
					"Are you sure you want to DELETE this asset?\n\n" + pkgDetails.title + "\n\n" + pkgDetails.filePath
					, "Cancel", "DELETE"))
				{
						// Cancel as default - Do nothing
				}
				else
				{
						// Yes, delete file
						var fileData = DeleteAsset (pkgDetails);
					if (fileData.Key > 0)
					{
						Debug.Log(gyaAbbr + " - ( " + fileData.Key.ToString() + " ) package(s) deleted.\n" + fileData.Value);
					}
					//RefreshPackages();
				}
			}

			// Delete assets multiple
			if (obj.ToString() == "DeleteAssetMultiple")
			{
				if (EditorUtility.DisplayDialog(gyaAbbr + " - Delete ALL Selected Assets",
					"Are you sure you want to DELETE ALL selected assets?\n\n" +
					cntMarkedToImport + " file(s) selected for deletion.\n\n" +
					"Deleted file(s) are NOT moved to the Trash!\n"
					, "Cancel", "DELETE ALL SELECTED"))
				{
					// Cancel as default - Do nothing
				}
				else
				{
					// Yes, delete file
					DeleteAssetMultiple ();
					//if (fileData.Key > 0) {
					//	Debug.Log(gyaAbbr + " - ( " + fileData.Key.ToString() + " ) package(s) deleted.\n" + fileData.Value);
					//}
					//RefreshPackages();
				}
			}

			// consolidate Old Assets
			if (obj.ToString() == "MoveOldAssets")
			{
				// Confirm Move old assets
				if (EditorUtility.DisplayDialog(gyaAbbr + " - Consolidate (MOVE) Old Assets",
					"This is SAFE to do at ANY time.\n\n" + "Files will be moved to '" + pathOldAssetsFolderName + "'\n\n" + "Only the Asset Store folder is processed.\nThe User folder is NOT processed.\n\n" + "NOTE: Consider this a temp folder for Old assets."
					, "Cancel", "CONSOLIDATE"))
				{
					// Cancel as default - Do nothing
				} else {
					// Yes, Move files
					OldAssetsMove();
				}
			}

			// Move asset to old assets folder - Used prior to updating a package from the Asset Store
			if (obj.ToString() == "OldAssetsDeleteAll")
			{
				if (EditorUtility.DisplayDialog(gyaAbbr + " - Delete ALL Consolidated Assets",
					"WARNING!!  This will delete ALL files currently located within: " + pathOldAssetsFolder
					+ "\n\nPlease backup any Old versions you wish to keep to your User folder before proceeding."
					+ "\n\nConfirm: DELETE ALL Consolidated Assets?"
					, "Cancel", "DELETE ALL"))
				{
					// Cancel as default - Do nothing
				} else {
					// Yes, delete all
					OldAssetsDeleteAll();
				}
			}

			// Settings - Select User Asset Folder
			if (obj.ToString() == "UserAssetsSelect")
			{
				string path = EditorUtility.SaveFolderPanel(gyaAbbr + " Select User Assets Folder:", "", "");
				if (path.Length != 0)
				{
					userData.Settings.pathUserAssets = path;
					SaveUser();
					RefreshPackages();
					//if (pkgData.countUser > 0) {
					//	SVShowCollection(svCollection.User);
					//}
				}
			}
			// Clear User Asset Folder
			if (obj.ToString() == "UserAssetsClear")
			{
				userData.Settings.pathUserAssets = String.Empty;
				SaveUser();
				SVShowCollection(svCollection.All); // Force back to Store folder
				RefreshPackages();
			}

			// Show asset selection
			if (obj.ToString() == "AllAssets")
			{
				SVShowCollection(svCollection.All);
				showGroup = 0;
			}
			if (obj.ToString() == "StoreAssets")
			{
				SVShowCollection(svCollection.Store);
				showGroup = 0;
			}
			if (obj.ToString() == "UserAssets")
			{
				SVShowCollection(svCollection.User);
				showGroup = 0;
			}
			if (obj.ToString() == "StandardAssets")
			{
				SVShowCollection(svCollection.Standard);
				showGroup = 0;
			}
			if (obj.ToString() == "OldAssets")
			{
				SVShowCollection(svCollection.Old);
				showGroup = 0;
			}
			if (obj.ToString() == "PackageImportGroup")
			{
				// Verify Import Multiple
				if (EditorUtility.DisplayDialog(gyaAbbr + " - Import Entire Group",
					//"Method: " + userData.Settings.multiImportOverride.ToString() +"\n\n
					"Are you sure you want to import this group?\n\nThis may take awhile depending on the number/size of assets in the group."
					, "Cancel", "Import Group"))
				{
						// Cancel as default - Do nothing
				}
				else
				{
					// Yes, import
					ImportMultiple(true); // true = Import entire group
				}
			}
			if (obj.ToString() == "PackageImportMultiple")
			{
				// Verify Import Multiple
				if (EditorUtility.DisplayDialog(gyaAbbr + " - Import Selected Assets",
					//"Method: " + userData.Settings.multiImportOverride.ToString() +"\n\n
					"Are you sure you want to import selected assets?\n\nThis may take awhile depending on the number/size of assets selected."
					, "Cancel", "Import Selected"))
				{
						// Cancel as default - Do nothing
				}
				else
				{
					// Yes, import
					ImportMultiple();
				}
			}

			// RenameWithVersionSelected
			if (obj.ToString() == "RenameWithVersionSelected")
			{
				// Verify Import Multiple
				if (EditorUtility.DisplayDialog(gyaAbbr + " - Rename Selected Assets",
					"This will append the package/unity version to the filename(s).\n\nNote: Only applies to actual Asset Store packages."
					, "Cancel", "Rename Selected"))
				{
						// Cancel as default - Do nothing
				}
				else
				{
						// Yes, rename
						RenameWithVersionSelected();
					}
			}

			// InvertSelections
			if (obj.ToString() == "InvertSelections")
			{
				MarkedForImportInvert();
			}

			// ClearMarked
			if (obj.ToString() == "ClearMarked")
			{
				MarkedForImportClear();
			}

			// SelectVisible
			if (obj.ToString() == "SelectVisible")
			{
				SelectVisible();
			}

			// CopyPathToClipboard
			if (obj.ToString() == "CopyPathToClipboard")
			{
				TextEditor te = new TextEditor();
				#if UNITY_5_3_OR_NEWER
				te.text = pkgDetails.filePath;
				#else
				te.content = new GUIContent(pkgDetails.filePath);
				#endif
				te.SelectAll();
				te.Copy();
			}

			// Popup Menu
			if (obj.ToString() == "PopupLoad")
			{
				ImportSingle (pkgDetails.filePath);
			}
			if (obj.ToString() == "PopupLoadInteractive")
			{
				ImportSingle (pkgDetails.filePath, true);
			}
			if (obj.ToString() == "AssetFolder")
			{
				GYAExt.ShellOpenFolder (pkgDetails.filePath, true);
			}

			if (obj.ToString() == "AssetURL")
			{
				GYAExt.OpenAssetURL(pkgDetails.link.id, userData.Settings.openURLInUnity);
			}

			if (obj.ToString() == "AssetURL2")
			{
				GYAExt.OpenAssetURL(pkgDetails.link.id, "http://uas.pxl.so/");
			}

			//if (obj.ToString() == "AssetURL") {
			//	//EditorApplication.ExecuteMenuItem( "Window/Asset Store" );
			//	//string openURLSite = "com.unity3d.kharma:content/";
			//	string openURLSite = "https://www.assetstore.unity3d.com/#/";
			//	string openURL = pkgDetails.link.type + "/" + pkgDetails.link.id.ToString();

			//	if (GYAExt.IsOSMac) {
			//		if (userData.Settings.openURLInUnity) {
			//			// Open in Unity's Asset Store Window
			//			//UnityEditorInternal.AssetStore.Open (string.Format ("content/{0}?assetID={1}", activeAsset.packageID, activeAsset.id));
			//			//UnityEditorInternal.AssetStore.Open (string.Format ("content/{0}", pkgDetails.link.id.ToString()));
			//			UnityEditorInternal.AssetStore.Open (string.Format (openURL));
			//		} else {
			//			openURL = openURLSite + openURL;
			//			System.Diagnostics.Process.Start ("open", openURL);
			//		}
			//	}
			//	if (GYAExt.IsOSWin) {
			//		if (userData.Settings.openURLInUnity) {
			//			// Open in Unity's Asset Store Window
			//			//UnityEditorInternal.AssetStore.Open (string.Format ("content/{0}", pkgDetails.link.id.ToString()));
			//			UnityEditorInternal.AssetStore.Open (string.Format (openURL));
			//		} else {
			//			openURL = openURLSite + openURL;
			//			System.Diagnostics.Process.Start (@openURL);
			//		}
			//	}
			//}

			//if (obj.ToString() == "AssetURL2") {
			//	string openURLSite = "http://uas.pxl.so/";
			//	string openURL = pkgDetails.link.type + "/" + pkgDetails.link.id.ToString();
			//	openURL = openURLSite + openURL;

			//	if (GYAExt.IsOSMac) {
			//		System.Diagnostics.Process.Start ("open", openURL);
			//	}
			//	if (GYAExt.IsOSWin) {
			//		System.Diagnostics.Process.Start (@openURL);
			//	}
			//}

			if (obj.ToString() == "AssetStoreFolder")
			{
				GYAExt.ShellOpenFolder(GYAExt.PathUnityAssetStore);
			}
			if (obj.ToString() == "AssetStoreFolder5")
			{
				GYAExt.ShellOpenFolder(GYAExt.PathUnityAssetStore5);
			}
			if (obj.ToString() == "StandardAssetsFolder")
			{
				GYAExt.ShellOpenFolder(GYAExt.PathUnityStandardAssets);
			}
			if (obj.ToString() == "OldAssetsFolder")
			{
				GYAExt.ShellOpenFolder(pathOldAssetsFolder);
			}
			if (obj.ToString() == "UserAssetsFolder")
			{
				GYAExt.ShellOpenFolder(userData.Settings.pathUserAssets);
			}
			if (obj.ToString() == "AssetStoreURL")
			{
				// Open the Asset Store in the default browser
				string openURL = "https://www.assetstore.unity3d.com/";
				if (GYAExt.IsOSMac)
				{
					if (userData.Settings.openURLInUnity)
					{
						// Open in Unity's Asset Store Window
						UnityEditorInternal.AssetStore.Open (null); // Unity browser
					}
					else
					{
						System.Diagnostics.Process.Start("open", openURL); // Default browser
					}
				}
				if (GYAExt.IsOSWin)
				{
					if (userData.Settings.openURLInUnity)
					{
						// Open in Unity's Asset Store Window
						UnityEditorInternal.AssetStore.Open (null); // Unity browser
					}
					else
					{
						System.Diagnostics.Process.Start(@openURL);
					}
				}
			}
			if (obj.ToString() == "AssetStoreURL2")
			{
				// Open the Asset Store in the default browser
				string openURL = "http://assets.pxl.so/";
				if (GYAExt.IsOSMac)
				{
					System.Diagnostics.Process.Start("open", openURL); // Default browser
					//UnityEditorInternal.AssetStore.Open (); // Unity browser
				}
				if (GYAExt.IsOSWin)
				{
					System.Diagnostics.Process.Start(@openURL);
				}
			}

			// Toggle Offline mode
			if ( obj.ToString() == "OfflineMode" )
			{
				userData.Settings.enableOfflineMode = !userData.Settings.enableOfflineMode;
				SaveUser ();
			}

			// Testing: Load data bypassing version checking
			if (obj.ToString() == "LoadAltDataFile")
			{

				string path = EditorUtility.OpenFilePanel(gyaAbbr + " Select User Assets Folder:", "", "json");
				if (path.Length != 0)
				{
					// Auto Enable Offline Mode since we are loading a data file
					userData.Settings.enableOfflineMode = true;
					SaveUser ();

					//if (EditorUtility.DisplayDialog(gyaAbbr + " - Load Alternate Data File",
					//	"Load an alternate GYA data file for Offline use.\n" +
					//	"Copy data file to AS Folder, then press LOAD\n\n" +
					//	"NOTE: Refreshing will overwrite this file and return things to normal!"
					//	, "Cancel", "LOAD")) {
					// Cancel as default - Do nothing
					//} else {
					//Debug.Log(gyaAbbr + " - Bypassing version check when loading json data file.\n");

					// Copy data file to AS folder
					//Debug.Log(path + "\n" + jsonFileUser);

					File.SetAttributes(jsonFilePackages, FileAttributes.Normal);
					File.Copy(@path, @jsonFilePackages, true);
					//LoadJSON(true);
					LoadJSON();
					ScanProject();
					BuildPrevNextList();
				}
			}

			// ASMoveASToAS5 - AS to AS 5 Folder
			if ( obj.ToString() == "ASMoveASToAS5" )
			{
				// Is scanAllAssetStoreFolders ENABLED, this is required
				if (userData.Settings.scanAllAssetStoreFolders)
				{
					if (EditorUtility.DisplayDialog(gyaAbbr + " - Move Assets TO " + GYAExt.FolderUnityAssetStore5,
						"This will MOVE your (Old) Asset Store files to the (Unity 5) Asset Store folder.\n\n" +
						"From: " + GYAExt.PathUnityAssetStore + "\n" +
						"To: " + GYAExt.PathUnityAssetStore5 + "\n\n" +
						"This will ONLY move files in the AS folder.\n\n"
						, "Cancel", "MOVE Assets"))
					{
						// Cancel as default - Do nothing
					}
					else
					{
						//MoveASFiles(GYAExt.FolderUnityAssetStore, GYAExt.FolderUnityAssetStore5);
						MoveASFiles(GYAExt.FolderUnityAssetStore5);
					}
				}
				else
				{
					Debug.Log(gyaAbbr + " - 'Settings->Scan All Asset Store Folders' needs to be ENABLED to perform Move\n");
				}
			}

			// ASMoveASToAS - AS 5 to AS Folder
			if ( obj.ToString() == "ASMoveAS5ToAS" )
			{
				// Is scanAllAssetStoreFolders ENABLED, this is required
				if (userData.Settings.scanAllAssetStoreFolders)
				{
					if (EditorUtility.DisplayDialog(gyaAbbr + " - Move Assets TO " + GYAExt.FolderUnityAssetStore,
						"This will MOVE your (Unity 5) Asset Store files to the (Old) Asset Store folder.\n\n" +
						"From: " + GYAExt.PathUnityAssetStore5 + "\n" +
						"To: " + GYAExt.PathUnityAssetStore + "\n\n" +
						"This will ONLY move files in the (Unity 5) AS folder.\n\n"
						, "Cancel", "MOVE Assets"))
					{
						// Cancel as default - Do nothing
					}
					else
					{
						//MoveASFiles(GYAExt.FolderUnityAssetStore, GYAExt.FolderUnityAssetStore5);
						MoveASFiles(GYAExt.FolderUnityAssetStore);
					}
				}
				else
				{
					Debug.Log(gyaAbbr + " - 'Settings->Scan All Asset Store Folders' needs to be ENABLED to perform Move\n");
				}
			}

			// MoveFolderContentsOldAssets
			if ( obj.ToString() == "MoveFolderContentsOldAssets" )
			{
				if (EditorUtility.DisplayDialog(gyaAbbr + " - Merge Old Assets from Old GYA Versions",
					"This will locate & merge any Old Assets from older versions of GYA.\n\n" +
					"Old Assets found will be moved to:\n" + pathOldAssetsFolder
					, "Cancel", "MERGE"))
				{
					// Cancel as default - Do nothing
				}
				else
				{
					MoveFolderContentsOldAssetsMain();
				}
			}

			//CleanOutdatedGYASupportFiles
			if ( obj.ToString() == "CleanOutdatedGYASupportFiles" )
			{
				if (EditorUtility.DisplayDialog(gyaAbbr + " - Clean Outdated GYA Support Files",
					"This will delete GYA specific files left behind from older versions of GYA.\n\n" +
					"It is recommended to run Merge prior to Cleaning to make sure that your Old Assets have been moved.\n\n" +
					"Please verify that any User created Groups/Old Assets are accessible prior to Cleaning."
					, "Cancel", "CLEAN"))
				{
					// Cancel as default - Do nothing
				}
				else
				{
					CleanOutdatedGYASupportFiles();
				}
			}

			// DeleteEmptySubFolders
			if ( obj.ToString() == "DeleteEmptySubFolders" )
			{
				DeleteEmptySubFolders(GYAExt.PathUnityAssetStore);
				DeleteEmptySubFolders(GYAExt.PathUnityAssetStore5);
				Debug.Log(gyaAbbr + " - Deleted Empty Sub Folders & .DS_Store files from the Asset Store folder(s).\n");
			}

			// ExportAsCSV
			if (obj.ToString() == "ExportAsCSV")
			{
				string fileName = "GYA Assets Export";
				string path = EditorUtility.SaveFilePanel(gyaAbbr + " - Export CSV file as: " + fileName + ".csv", "", fileName + ".csv", "csv");
				if (path.Length != 0)
				{
					SaveAsCSV(pkgData.Assets, path);
				}
			}

			// ExportAsCSVGroup
			if (obj.ToString() == "ExportAsCSVGroup")
			{
				string fileName = "GYA Assets Export (Group)";
				string path = EditorUtility.SaveFilePanel(gyaAbbr + " - Export CSV file as: " + fileName + ".csv", "", fileName + ".csv", "csv");
				if (path.Length != 0)
				{
					SaveAsCSVGroup(path);
				}
			}

			// Assign User Folders
			if (obj.ToString() == "UserFolder")
			{
				GUIOverride(OverrideReason.UserFolder);
			}

			// Create New Group
			if (obj.ToString() == "GroupCreate")
			{
				GUIOverride(OverrideReason.GroupCreate);
			}

			// Group - Add asset to a group
			if ( obj.ToString().StartsWith("GroupDelete") )
			{
				if (EditorUtility.DisplayDialog(gyaAbbr + " - Delete Selected Group",
					"Are you sure you want to DELETE this group?\n\n" +
					"No Assets will be deleted, only the virtual group.\n\n" +
					userData.Group[Convert.ToInt32 (passedVar)].name
					, "Cancel", "DELETE"))
				{
					// Cancel as default - Do nothing
				}
				else
				{
					GroupDelete(Convert.ToInt32 (passedVar));
				}
			}

			// GroupRename
			if ( obj.ToString().StartsWith("GroupRename") )
			{
				fldOverridePassed = passedVar;
				fldOverrideEntry = userData.Group[Convert.ToInt32(fldOverridePassed)].name;
				GUIOverride(OverrideReason.GroupRename);
			}

			// AssetsNotInAGroup
			//if ( obj.ToString() == "AssetsNotInAGroup" ) {
			//	AssetsNotInAGroup();
			//}

			ddActive = false;
		}

		// Yes I could do this in a loop, but I didn't
		private void CleanOutdatedGYASupportFiles()
		{
			// Delete ALL outdated files
			string resultText = gyaAbbr + " -\tScanning for and Deleting Outdated GYA Files/Folders.\n";

			string oldpathOldAssetsFolderName = "-" + gyaName + " (Old)"; // Old Assets folder
			string oldjsonFilePackagesName = gyaName + ".json";	// GYA Packages Data file
			string oldjsonFileUserName = gyaName + " User.json";	// GYA User Data file
			string oldjsonFileUserBak = gyaName + " User.bak";	// GYA User Data file

			// User File
			string path1 = Path.GetFullPath(Path.Combine(GYAExt.PathUnityAssetStore, oldjsonFileUserName));
			string path2 = Path.GetFullPath(Path.Combine(GYAExt.PathGYADataFilesOLD, oldjsonFileUserName));
			// User File BAK
			string path3 = Path.GetFullPath(Path.Combine(GYAExt.PathUnityAssetStore, oldjsonFileUserBak));
			string path4 = Path.GetFullPath(Path.Combine(GYAExt.PathGYADataFilesOLD, oldjsonFileUserBak));
			// Assets File
			string path5 = Path.GetFullPath(Path.Combine(GYAExt.PathUnityAssetStore, oldjsonFilePackagesName));
			string path6 = Path.GetFullPath(Path.Combine(GYAExt.PathGYADataFilesOLD, oldjsonFilePackagesName));
			// Old Assets Folder
			string path7 = Path.GetFullPath(Path.Combine(GYAExt.PathUnityAssetStore, oldpathOldAssetsFolderName));
			string path8 = Path.GetFullPath(Path.Combine(GYAExt.PathGYADataFilesOLD, oldpathOldAssetsFolderName));

			try
			{
				// User file
				if (File.Exists(path1))
				{
					resultText = resultText + "\tDeleting: " + path1 + "\n";
					File.Delete(path1);
				}
				if (File.Exists(path2))
				{
					resultText = resultText + "\tDeleting: " + path2 + "\n";
					File.Delete(path2);
				}
				// User file BAK
				if (File.Exists(path3))
				{
					resultText = resultText + "\tDeleting: " + path3 + "\n";
					File.Delete(path3);
				}
				if (File.Exists(path4))
				{
					resultText = resultText + "\tDeleting: " + path4 + "\n";
					File.Delete(path4);
				}
				// Assets file
				if (File.Exists(path5))
				{
					resultText = resultText + "\tDeleting: " + path5 + "\n";
					File.Delete(path5);
				}
				if (File.Exists(path6))
				{
					resultText = resultText + "\tDeleting: " + path6 + "\n";
					File.Delete(path6);
				}
				// Old Assets folder
				if (Directory.Exists(path7))
				{
					resultText = resultText + "\tDeleting: " + path7 + "\n";
					Directory.Delete(path7, true);
				}
				if (Directory.Exists(path8))
				{
					resultText = resultText + "\tDeleting: " + path8 + "\n";
					Directory.Delete(path8, true);
				}
			}
				catch (Exception ex)
			{
				Debug.LogError(gyaAbbr + " - Error Removing file/folder: \n" + ex.Message);
			}
			resultText = resultText + "\tScanning complete.\n";
			Debug.Log(resultText);
		}

		private void MoveFolderContentsOldAssetsMain()
		{
			int count = 0;
			string pathTmp = Path.Combine(GYAExt.PathUnityAssetStore, "-" + gyaName + " (Old)");
			string pathTmp2 = Path.Combine(GYAExt.PathGYADataFilesOLD, "-" + gyaName + " (Old)");
			if (Directory.Exists(pathTmp) || Directory.Exists(pathTmp2))
			{
				count = MoveFolderContentsOldAssets(false);
				Debug.Log(gyaAbbr + " - (" + count.ToString() + ") Old Asset(s) reclaimed and moved to the '"+ pathOldAssetsFolderName + "' folder.\n");

				int countTmp = 0;

				// running even thou folder not exist!
				if (Directory.Exists(pathTmp))
				{
					countTmp += GetAssetCountFromFolder(pathTmp);
					if (countTmp == 0)
					{
						DeleteEmptySubFolders(pathTmp);
						try
						{
							if (Directory.GetFileSystemEntries(pathTmp).Length == 0)
							{
								Directory.Delete(pathTmp, false);
								Debug.Log(gyaAbbr + " - Deleted: " + pathTmp);
							}
						}
							catch (IOException ex)
						{
							Debug.LogWarning(gyaAbbr + " - Error Cleaning: \n" + pathTmp + "\n" + ex.Message);
						}
					}
				}

				//PathGYADataFilesOLD
				countTmp = 0;
				if (Directory.Exists(pathTmp2))
				{
					countTmp += GetAssetCountFromFolder(pathTmp2);
					if (countTmp == 0) {
						DeleteEmptySubFolders(pathTmp2);
						try
						{
							if (Directory.GetFileSystemEntries(pathTmp2).Length == 0)
							{
								Directory.Delete(pathTmp2, false);
								Debug.Log(gyaAbbr + " - Deleted: " + pathTmp2);
							}
						}
							catch (IOException ex)
						{
							Debug.LogWarning(gyaAbbr + " - Error Cleaning: \n" + pathTmp2 + "\n" + ex.Message);
						}
					}
				}
			}
			else
			{
				Debug.Log(gyaAbbr + " - No prior Old Asset Store folders found.\n");
			}
		}

		// Move/Merge assets to the target Asset Store folder
		//private void MoveASFiles(string fromFolder, string toFolder) {
		private void MoveASFiles(string toFolder)
		{
			//Debug.Log(gyaAbbr + " - Moving Asset Store Files To: " + toFolder + "\n");

			CreateFolder(toFolder, true);

			bool processPath;
			int filesMoved = 0;
			string filesInfo = string.Empty;
			string pathNew = "";

			// old "/"
			//string pathDivider = Path.DirectorySeparatorChar.ToString();
			string pathDivider = "/";
			string pathUAS = GYAExt.PathFixedForOS(GYAExt.PathUnityAssetStore + pathDivider);
			string pathUAS5 = GYAExt.PathFixedForOS(GYAExt.PathUnityAssetStore5 + pathDivider);
			string pathAsset = "";

			// Progressbar
			//int filenameStartIndex = (toFolder.Length + 1);
			//using (var progressBar = new ProgressBar(
			//	string.Format("{0} Moving Assets to Folder: {1}", gyaAbbr, toFolder),
			//	pkgData.Assets.Count(),
			//	60,
			//	(stepNumber) => pkgData.Assets[stepNumber].title.Substring(filenameStartIndex)))

			// Check all old assets
			for (int i = 0; i < pkgData.Assets.Count; ++i)
			{

				processPath = false;

				// pathNew can also be derived from: (removing: "&" "/" in the tags)
				// GYAExt.PathUnityAssetStore + "/" + publisher.label + "/" + category.label + "/" + label
				pathAsset = GYAExt.PathFixedForOS(pkgData.Assets[i].filePath);

				if (pathAsset.Contains(pathUAS) || pathAsset.Contains(pathUAS5))
				{

					// Progressbar update
					//progressBar.Update(i);

					// Move to Unity 5 AS
					if (toFolder == GYAExt.FolderUnityAssetStore5 && !pathAsset.Contains(pathUAS5))
					{
						processPath = true;
						pathNew = pathAsset.Replace( pathUAS, pathUAS5 );
					}

					// Move to Unity AS ignoring Unity 5 specific files
					if (toFolder == GYAExt.FolderUnityAssetStore && !pathAsset.Contains(pathUAS) && !pkgData.Assets[i].unity_version.StartsWith("5."))
					{
						processPath = true;
						pathNew = pathAsset.Replace( pathUAS5, pathUAS );
					}
					//Debug.Log(pkgData.Assets[i].filePath + "\n" + pathNew);

					if (processPath)
					{
						// Is asset store file?
						if ( pkgData.Assets[i].collection == svCollection.Store )
						{
							//Debug.Log(pkgData.Assets[i].filePath + "\n" + pathNew);

							// If file does NOT exist at target location then move it
							if (!File.Exists(pathNew))
							{
								// Move asset
								var fileData = MoveAssetToPath(pkgData.Assets[i], pathNew, false, true, false, false);
								filesMoved = filesMoved + fileData.Key;
								filesInfo = filesInfo + fileData.Value;
							}
						}
					}
					else
					{
						//if ( pkgData.Assets[i].collection == svCollection.Store ) {
						//	Debug.Log(gyaAbbr + " - File not moved: " + toFolder + "\n" + pathNew);
						//}
					}
				}
			}

			if (filesMoved > 0)
			{
				Debug.Log(gyaAbbr + " - ( " + filesMoved.ToString() + " ) package(s) moved to: " + toFolder + "\n" + filesInfo);
			}

			// Clean up folders
			DeleteEmptySubFolders(GYAExt.PathUnityAssetStore);
			DeleteEmptySubFolders(GYAExt.PathUnityAssetStore5);

			// Make sure list is up-to-date
			RefreshPackages();
			//RefreshPackagesASOnly();
		}

		// Update selected assets to Show
		private int UpdateInfoToDisplay()
		{
			int searchCount = 0;

			//List<Packages> packageShow = null; //pkgData.Store;

			// Show selected assets
			if (showActive == svCollection.All)
			{
				packageShow = pkgData.Assets;
			}
			if (showActive == svCollection.Store)
			{
				packageShow = pkgData.Assets.FindAll( x => x.collection == svCollection.Store );
			}
			if (showActive == svCollection.Standard)
			{
				packageShow = pkgData.Assets.FindAll( x => x.collection == svCollection.Standard );
			}
			if (showActive == svCollection.User)
			{
				packageShow = pkgData.Assets.FindAll( x => x.collection == svCollection.User );
			}
			if (showActive == svCollection.Old)
			{
				packageShow = pkgData.Assets.FindAll( x => x.collection == svCollection.Old );
			}
			if (showActive == svCollection.Group)
			{
				packageShow = grpData[showGroup];
			}

			// Show only needed AS Folder Info
			if (!userData.Settings.showAllAssetStoreFolders)
			{
				if (ddCategory.In(unity5VersionText, unity5FolderText, unityOlderVersionText, unityOlderFolderText) )
				{
					// Do nothing
				}
				else
				{
					// Remove non-native AS files
					//packageShow.RemoveAll( x => x.isInNativeASFolder == GYAExt.FolderUnityAssetStoreInActive);
					packageShow = packageShow.FindAll( x => !(x.isInNativeASFolder == GYAExt.FolderUnityAssetStoreInActive) );
				}
			}

			// Dropdown Category
			if (ddCategory != "")
			{
				if (ddCategory == unity5VersionText)
				{
					packageShow = packageShow.FindAll( x => (x.unity_version.StartsWith("5.")) );
				}
				else if (ddCategory == unity5FolderText)
				{
					//packageShow = packageShow.FindAll( x => (x.filePath.Contains(GYAExt.FolderUnityAssetStore5)) );
					packageShow = packageShow.FindAll( x => x.isInAS5Folder );
				}
				else if (ddCategory == unityOlderVersionText)
				{
					packageShow = packageShow.FindAll( x => !x.unity_version.StartsWith("5.") );
				}
				else if (ddCategory == unityOlderFolderText)
				{
					//packageShow = packageShow.FindAll( x => x.collection == svCollection.Store && GYAExt.PathFixedForOS(x.filePath).Contains(GYAExt.PathFixedForOS(GYAExt.PathUnityAssetStore + "/")) );
					packageShow = packageShow.FindAll( x => x.isInASFolder );
				}
				else if (ddCategory == packageText)
				{
					packageShow = packageShow.FindAll( x => !x.isExported );
				}
				else if (ddCategory == exportedText)
				{
					packageShow = packageShow.FindAll( x => x.isExported );
				}
				else if (ddCategory == ungroupedText)
				{
					//packageShow = packageShow.FindAll( x => !(x.isInAGroup) && (x.collection == svCollection.Store) );
					packageShow = packageShow.FindAll( x => !(x.isInAGroup) );
				}
				else if (ddCategory == damagedText)
				{
					packageShow = packageShow.FindAll( x => (x.isDamaged) );
				}
				else
				{
					packageShow = packageShow.FindAll( x => (x.category.label).Replace('/', '\\').Replace('&', '+').StartsWith(ddCategory) );
				}
			}

			// Dropdown Category
			//if (ddCategory != "") {
			//	packageShow = packageShow.FindAll( x => (x.category.label).Replace('/', '\\').Replace('&', '+').StartsWith(ddCategory) ).ForEach(x => x.isMarked = true);
			//}
			// Dropdown Publisher
			if (ddPublisher != 0)
			{
				//packageShow = packageShow.FindAll( x => (x.publisher.label).Replace('/', '\\').Replace('&', '+').StartsWith(ddPublisher) ).ForEach(x => x.isMarked = true);
				//packageShow = packageShow.FindAll( x => (x.publisher.id).StartsWith(ddPublisher) );
				packageShow = packageShow.FindAll( x => x.publisher.id == ddPublisher );
			}
			// Figure out how many items are returned if searching, all if not
			//Debug.Log("SVDraw - " + packageShow.Count);
			if (searchActive == svSearchBy.Title)
			{
				searchCount = packageShow.FindAll( x => x.title.Contains(fldSearch, StringComparison.OrdinalIgnoreCase) ).Count;
			}
			if (searchActive == svSearchBy.Category)
			{
				searchCount = packageShow.FindAll( x => x.category.label.Contains(fldSearch, StringComparison.OrdinalIgnoreCase) ).Count;
			}
			if (searchActive == svSearchBy.Publisher)
			{
				searchCount = packageShow.FindAll( x => x.publisher.label.Contains(fldSearch, StringComparison.OrdinalIgnoreCase) ).Count;
			}
			return searchCount;
		}

		// SelectVisible - Selects/Marks all visible assets in the list
		private void SelectVisible()
		{
			// Handle differences for searchActive
			if (searchActive == svSearchBy.Title)
			{
				packageShow.FindAll( x =>  x.title.Contains(fldSearch, StringComparison.OrdinalIgnoreCase)  ).ForEach(x =>  x.isMarked = true);
			}
			if (searchActive == svSearchBy.Category)
			{
				packageShow.FindAll( x =>  x.category.label.Contains(fldSearch, StringComparison.OrdinalIgnoreCase)  ).ForEach(x =>  x.isMarked = true);
			}
			if (searchActive == svSearchBy.Publisher)
			{
				packageShow.FindAll( x =>  x.publisher.label.Contains(fldSearch, StringComparison.OrdinalIgnoreCase)  ).ForEach(x =>  x.isMarked = true);
			}
		}

		// Invert isMarked
		private void MarkedForImportInvert()
		{
			packageShow.ForEach(x => x.isMarked = !x.isMarked);
		}

		// Clear isMarked
		private void MarkedForImportClear()
		{
			//pkgData.Assets.ForEach(x => x.isMarked = false);
			packageShow.ForEach(x => x.isMarked = false);
		}

		// svCollection.Group
		private bool IsActiveGroup (int grpID)
		{
			//Debug.Log("IsActiveGroup - Group: " + showActive.ToString() + " - " + showGroup.ToString());
			return (showActive == svCollection.Group && showGroup == grpID ? true : false);
			//return false;
		}

		// Create grpData Package List, for display, from userData info
		// pSort true = sort by sortActive value
		// pkgData.Assets[0].isInAGroup;
		// GroupAssetNameFromID
		private void GroupUpdatePkgData (bool pSort = true)
		{
			List<Packages> combinedGroup = new List<Packages>();
			List<Packages> pkgResults;

			// Catch invalid data
			if (grpData != null)
			{
				grpData.Clear();
			}
			else
			{
				GUIOverride(OverrideReason.ErrorStep2);
				return;
			}

			for (int i = 0; i < userData.Group.Count; ++i)
			{
				if (!grpData.ContainsKey(i))
				{
					grpData.Add(i, new List<Packages>());
				}
				for (int j = 0; j < userData.Group[i].Assets.Count; ++j)
				{
					Asset curAsset = userData.Group[i].Assets[j];

					// Mark isInAGroup
					//pkgData.Assets[curAsset.id].isInAGroup;
					//var results = pkgData.Assets.FindAll( x => x.id == curAsset.id );
					//pkgData.Assets.ForEach(x =>  x.isMarked = false);
					//pkgData.Assets.ForEach(x => (x.id == curAsset.id) = true);

					//pkgData.Assets.FindAll( x => x.id == curAsset.id ).ForEach(x => x.isInAGroup = true);
					//pkgData.Assets.Find( x => x.id == curAsset.id ).ForEach(x => x.isInAGroup = true);

					// TODO: grpData, Handle main file missing when an asset exists in a group

					if (!curAsset.isExported)
					{
						// IS Asset Store ID, use asset ID

						// Mark isInAGroup
						pkgData.Assets.FindAll(x => x.id == curAsset.id ).ForEach(x => x.isInAGroup = true);

						if (curAsset.useLatestVersion)
						{
							// Use latest version

							pkgResults = pkgData.Assets.FindAll( x => x.id == curAsset.id );

							// Find if asset file exists
							if (pkgResults.Count() == 0)
							{
								userData.Group[i].Assets[j].isFileMissing = true;
								//Debug.Log(curAsset.title);
							}
							else
							{
								userData.Group[i].Assets[j].isFileMissing = false;
							}

							combinedGroup.Clear();
							combinedGroup.AddRange(pkgResults);
							//combinedGroup.Sort((x,y) => -x.version_id.CompareTo(y.version_id));
							combinedGroup = combinedGroup.OrderByDescending(x => x.collection == svCollection.Store).ThenBy(x => x.version_id).ToList();

							if (combinedGroup.Count() > 0)
							{
								grpData[i].Add(combinedGroup[0]);
							}
						}
						else
						{
							// Use selected version

						}
					}
					else
					{
						// No asset Id, use filepath
						// Mark isInAGroup
						pkgData.Assets.FindAll(x => x.filePath == curAsset.filePath ).ForEach(x => x.isInAGroup = true);

						pkgResults = pkgData.Assets.FindAll( x => x.filePath == curAsset.filePath );

						// Find if asset file exists
						if (pkgResults.Count() == 0)
						{
							userData.Group[i].Assets[j].isFileMissing = true;
							//Debug.Log(curAsset.title);
						}
						else
						{
							userData.Group[i].Assets[j].isFileMissing = false;
						}

						combinedGroup.Clear();
						combinedGroup.AddRange(pkgResults);
						combinedGroup.Sort((x,y) => -x.title.CompareTo(y.title));
						if (combinedGroup.Count() > 0)
						{
							grpData[i].Add(combinedGroup[0]);
						}
					}
				}

				if (pSort)
				{
					PackagesSortBy(sortActive);
				}
			}
		}

		// NOT USED
		// Get the name of the asset from it's ID and/or version ID, return filename if
		private string GroupAssetNameFromID(int pGrpID, int pGrpLine)
		{
			List<Packages> pkgResults;
			Asset grpAsset = userData.Group[pGrpID].Assets[pGrpLine];
			int assetID = grpAsset.id;
			string assetName = String.Empty;

			// Get package name
			if (grpAsset.isExported)
			{
				// NOT an asset store package, get name from filepath
				assetName = Path.GetFileNameWithoutExtension(grpAsset.filePath);
			}
			else
			{
				// IS an asset store package
				if (grpAsset.useLatestVersion)
				{
					// Find latest version of ID
					pkgResults = pkgData.Assets.FindAll( x => x.collection == svCollection.User && x.id == assetID );
					pkgResults.Sort((x,y) => -x.version_id.CompareTo(y.version_id));
				}
				else
				{
					// Don't find latest version, use specific version_ID
					pkgResults = pkgData.Assets.FindAll( x => x.collection == svCollection.User && x.id == assetID && x.version_id == grpAsset.version_id );
				}

				if ( pkgResults.Count() > 0 )
				{
					assetName = pkgResults[0].title;
				}
			}
			return assetName;
		}

		// NOT USED
		// Assets not in a group, result = list in log
		private void AssetsNotInAGroup ()
		{
			int rCount = 0;
			string rString = "";
			for (int i = 0; i < pkgData.Assets.Count; ++i)
			{
				if (!pkgData.Assets[i].isInAGroup)
				{
					rCount += 1;
					rString += pkgData.Assets[i].title + " " + pkgData.Assets[i].version + "\n";
				}
			}
			if (rString.Length > 0)
			{
				rString = "Assets that are NOT in a Group: ( " + rCount + " )\n" + rString;
				Debug.Log(rString);
			}
		}

		// Check if Group already contains a given asset
		private bool GroupContainsAsset (int grpID, Packages pItem)
		{
			int results = 0;

			if (pItem == null)
			{
				GUIOverride(OverrideReason.ErrorStep2);
				return false;
			}

			if (pItem.isExported)
			{
				// No asset Id, use filepath
				results = userData.Group[grpID].Assets.FindAll( x => x.filePath == pItem.filePath ).Count();
			}
			else
			{
				// IS Asset Store ID, use asset ID
				results = userData.Group[grpID].Assets.FindAll( x => x.id == pItem.id ).Count();
			}
			// If found return true
			if (results > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private bool GroupContainsAsset (int grpID, Asset aItem)
		{
			int results = 0;

			if (aItem.isExported)
			{
				// No asset Id, use filepath
				results = userData.Group[grpID].Assets.FindAll( x => x.filePath == aItem.filePath ).Count();
			}
			else
			{
				// IS Asset Store ID, use asset ID
				results = userData.Group[grpID].Assets.FindAll( x => x.id == aItem.id ).Count();
			}
			// If found return true
			if (results > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		// Create New Group
		private void GroupCreate (string grpName)
		{
			List<Group> grpTemp = new List<Group>()
			{
				new Group { name = grpName, Assets = new List<Asset> { } }	// Create Group without Asset
			};
			userData.Group.Add(grpTemp[0]);

			GroupUpdatePkgData(false);
			SaveUser ();
		}

		// Group Popup Remove from
		private void GroupRemoveAsset (object obj)
		{
			GroupRemoveAsset();
		}

		//private void GroupRemoveAsset (int pkgID, Packages pkgTemp = null) {
		private void GroupRemoveAsset ()
		{
			Asset itemToRemove = new Asset();

			if (pkgDetails.isExported)
			{
				itemToRemove = userData.Group[showGroup].Assets.SingleOrDefault(x => x.title == pkgDetails.title);
			}
			else
			{
				itemToRemove = userData.Group[showGroup].Assets.SingleOrDefault(x => x.id == pkgDetails.id);
			}

			if (itemToRemove != null)
			{
				userData.Group[showGroup].Assets.Remove(itemToRemove);
			}

			GroupUpdatePkgData();
			BuildPrevNextList();
			//PackagesSortBy(sortActive);
			SaveUser ();
		}

		//GroupRemoveAssetMultiple
		private void GroupRemoveAssetMultiple (object obj)
		{
			GroupRemoveAssetMultiple();
		}

		private void GroupRemoveAssetMultiple ()
		{
			Asset itemToRemove = new Asset();
			//foreach (Packages package in pkgData.Assets) {

			//foreach (RootUser package in userData.Group[i]) {
			foreach (Packages package in packageShow)
			{
				for (int i = 0; i < userData.Group[showGroup].Assets.Count; ++i)
				{
					itemToRemove = null;
					if (package.isMarked)
					{
						// Exported package
						if (userData.Group[showGroup].Assets[i].isExported && userData.Group[showGroup].Assets[i].title == package.title)
						{
							itemToRemove = userData.Group[showGroup].Assets.SingleOrDefault(x => x.title == pkgDetails.title);
						}
						// AS package
						if (userData.Group[showGroup].Assets[i].id == package.id)
						{
							itemToRemove = userData.Group[showGroup].Assets.SingleOrDefault(x => x.id == pkgDetails.id);
						}

						if (itemToRemove != null)
						{
							//Debug.Log("Remove: " + userData.Group[showGroup].Assets[i].title + "\npackageShow: " + package.title);
							userData.Group[showGroup].Assets.Remove(itemToRemove);
						}
					}
				}
			}

			// Add to in-memory grpData List
			GroupUpdatePkgData();
			BuildPrevNextList();
			//PackagesSortBy(sortActive);
			SaveUser ();

			MarkedForImportClear();
		}

		// Group Popup Add to
		private void GroupAddTo (object obj)
		{
			GroupAddTo((int)obj, false, null);
		}

		private void GroupAddTo (int grpID, bool batch = false, Packages pkgTemp = null)
		{
			if (pkgTemp == null)
				pkgTemp = pkgDetails;
			bool tmpUseLatest = true;
			int tmpVersionID = 0;
			string tmpTitle = String.Empty;

			// Adds a complete group
			if (tmpUseLatest)
			{
				// Use latest version, version_id doesn't matter in this case
				tmpVersionID = 0;
			}
			else
			{
				// Use specific version
				tmpVersionID = pkgTemp.version_id;
			}

			if (pkgTemp.isExported)
			{
				tmpTitle = Path.GetFileNameWithoutExtension(pkgTemp.filePath);
			}
			else
			{
				tmpTitle = pkgTemp.title;
			}

			List<Asset> grpAssets = new List<Asset>
			{
				new Asset
				{
					title = tmpTitle,
					isExported = pkgTemp.isExported,
					id = pkgTemp.id,
					useLatestVersion = tmpUseLatest,
					version_id = tmpVersionID,
					filePath = pkgTemp.filePath
				}
			};

			// If asset already in group then bypass
			if ( GroupContainsAsset(grpID, grpAssets[0]) )
			{
				Debug.Log (gyaAbbr + " - Asset already added - Group: " + userData.Group[grpID].name + " - Title: " + pkgTemp.title + "\n");
				return;
			}

			// If asset already in group then bypass
			if ( pkgTemp.collection == svCollection.Project )
			{
				Debug.Log (gyaAbbr + " - Local Project files cannot be added to a group: " + userData.Group[grpID].name + " - Title: " + pkgTemp.title + "\n");
				return;
			}

			// Add to group
			userData.Group[grpID].Assets.Add(grpAssets[0]);

			// Don't process if running in batch mode (multiple selections)
			if (!batch)
			{
				// Add to in-memory grpData List
				GroupUpdatePkgData();
				BuildPrevNextList();
				//PackagesSortBy(sortActive);
				SaveUser ();
			}
		}

		// Group Popup Add to
		private void GroupAddToMultiple (object obj)
		{
			GroupAddToMultiple((int)obj);
		}

		//private void GroupAddToMultiple (int grpID, Packages pkgTemp = null) {
		private void GroupAddToMultiple (int grpID)
		{
			//Debug.Log (gyaAbbr + " - Adding ( " + cntMarkedToImport + " ) assets to Group: " + userData.Group[grpID].name + "\n");
			foreach (Packages package in pkgData.Assets)
			{
				if (package.isMarked) {
					GroupAddTo(grpID, true, package);
				}
			}

			// Add to in-memory grpData List
			GroupUpdatePkgData();
			BuildPrevNextList();
			//PackagesSortBy(sortActive);
			SaveUser ();

			MarkedForImportClear();
		}

		// Rename Group
		private void GroupRename (int grpID, string grpName)
		{
			Debug.Log ("GroupRename : " + grpID.ToString () + " - " + grpName + "\n");
			userData.Group[grpID].name = grpName;

			GroupUpdatePkgData();
			BuildPrevNextList();
			SaveUser ();
		}

		// Delete Group
		private void GroupDelete (object obj)
		{
			GroupDelete((int)obj);
		}

		private void GroupDelete (int grpID)
		{
			// Do not allow deleting index 0 (Favorites)
			if (grpID != 0)
			{
				userData.Group.RemoveAt(grpID);

				// Reset scrollview
				if (showGroup > userData.Group.Count-1)
				{
					showGroup = userData.Group.Count-1;
				}
				SVShowCollection(svCollection.Group);

				GroupUpdatePkgData();
				BuildPrevNextList();
				SaveUser ();
			}
		}

		// Set which assets to show along with any pre-processing
		internal void SVShowCollection (svCollection svType, string showCount = "")
		{

			if (showCount != "")
			{
				showCount = "" + packageShow.Count + " / ";
			}

			// Show asset selection
			if (svType == svCollection.All)
			{
				selectionText = svCollection.All + " ( " + showCount + pkgData.countAll + " ) ";
				showActive = svCollection.All;
			}
			if (svType == svCollection.Store)
			{
				selectionText = svCollection.Store + " ( " + showCount + pkgData.countStore + " ) ";
				showActive = svCollection.Store;
			}
			if (svType == svCollection.User)
			{
				selectionText = svCollection.User + " ( " + showCount + pkgData.countUser + " )";
				showActive = svCollection.User;
			}
			if (svType == svCollection.Standard)
			{
				//selectionText = svCollection.Standard + " ( " + pkgData.countStandard + " )";
				selectionText = GYAExt.FolderUnityStandardAssets + " ( " + showCount + pkgData.countStandard + " )";
				showActive = svCollection.Standard;
			}
			if (svType == svCollection.Old)
			{
				//selectionText = svCollection.Old + " ( " + showCount + pkgData.countOld;
				//if (pkgData.countOldToMove > 0) {
				//	selectionText += " - " + pkgData.countOldToMove + " )";
				//} else {
				//	selectionText += " )";
				//}

				selectionText = svCollection.Old + " ( " + showCount + pkgData.countOld + " )";
				if (pkgData.countOldToMove > 0)
				{
					selectionText += " ( " + pkgData.countOldToMove + " )";
					//} else {
					//	selectionText += " )";
				}

				showActive = svCollection.Old;
			}
			if (svType == svCollection.Group)
			{
				// Make sure that showGroup is never more then the count-1
				if (showGroup > userData.Group.Count-1)
				{
					showGroup = userData.Group.Count-1;
				}

				selectionText = showGroup.ToString() + " - " + userData.Group[showGroup].name;
				showActive = svCollection.Group;
			}
		}

		// Process the passed collection: Store, Standard, User
		private void SVDraw ()
		{
			svLineHeight = 16;
			if (!userData.Settings.enableCollectionTypeIcons)
			{
				svToggle = new Rect(0, 0, 18, svLineHeight);
				svButton = new Rect(18, 0, position.width-18, svLineHeight);
			}
			else
			{
				svToggle = new Rect(0, 0, 36, svLineHeight);
				svButton = new Rect(36, 0, position.width-36, svLineHeight);
			}

			svHeight = svHeight - 18;

			int searchCount = 0;
			int optionalCount = 0;			// Count optional lines to draw in the list: Categories, etc
			string pkgText = String.Empty;	// Asset field text for comparing to Search field
			headerLast = String.Empty;
			string headerText = String.Empty;
			string[] mainCategory;
			bool forceHeaders = false;	// Force headers to true if not sortign by title
			//List<Packages> packageShow = null;

			// ----------------------------------------------------------------------------------
			// Optimize the logic section

			// Does list need to be refreshed?
			//userData.Settings.scanAllAssetStoreFolders; // bool
			//userData.Settings.showAllAssetStoreFolders; // bool
			//userData.Settings.enableHeaders // bool
			//fldSearch // string
			//ddCategory == exportedText; // string
			//ddPublisher == Publisher.id; // int
			//searchActive == svSearchBy.Title;
			//showActive == svCollection.Store;
			//sortActive == svSortBy.Title;
			//RefreshPackages..

			//if (GUI.changed) { Debug.Log("Text field has changed."); }

			if (infoToDisplayHasChanged)
			{
				searchCount = UpdateInfoToDisplay();
				//SVShowCollection(showActive, searchCount.ToString()); // Testing showCount
				//SelectVisible();
				//infoToDisplayHasChanged = false;
			}

			// Force Headers if required
			if (sortActive != svSortBy.Title)
				forceHeaders = true;

			// TODO: Reduce the pre-calc to a tighter function
			// Check if headers are turned off
			if (userData.Settings.enableHeaders || forceHeaders)
			{
				// Pre-calc extra list height to account for showing headers
				for (int i = 0; i < packageShow.Count; ++i)
				{

					if (searchActive == svSearchBy.Title)
					{
						if (packageShow[i].title != null)
							pkgText = packageShow[i].title;
					}
					if (searchActive == svSearchBy.Category)
					{
						if (packageShow[i].category.label != null)
							pkgText = packageShow[i].category.label;
					}
					if (searchActive == svSearchBy.Publisher)
					{
						if (packageShow[i].publisher.label != null)
							pkgText = packageShow[i].publisher.label;
					}

					// Reduce to first char of string
					if (sortActive == svSortBy.Title)
					{
						if (packageShow[i].collection == svCollection.Project)
						{
							headerText = "- Local Project -";
						}
						else
						{
							if (packageShow[i].isDamaged && packageShow[i].title == "unknown") {
								headerText = "- Unknown Asset Title -";
							}
							else
							{
								if (packageShow[i].title != null)
								{
									headerText = RemoveLeading( packageShow[i].title )[0].ToString().ToUpper();
								}
							}
						}
					}
					if (sortActive == svSortBy.Category)
					{
						mainCategory = packageShow[i].category.label.ToUpper().Split('/');
						headerText = mainCategory[0];
					}
					if (sortActive == svSortBy.CategorySub)
					{
						headerText = packageShow[i].category.label.ToUpper ();
					}
					if (sortActive == svSortBy.Publisher)
					{
						headerText = packageShow[i].publisher.label.ToUpper ();
					}
					if (sortActive == svSortBy.Size)
					{
						headerText = GYAExt.GetByteRangeHeader(packageShow[i].fileSize);
					}
					if (sortActive == svSortBy.PackageID)
					{
						headerText = "Most Recent by Package ID";
					}
					if (sortActive == svSortBy.VersionID)
					{
						headerText = "Most Recent by Version ID";
					}
					if (sortActive == svSortBy.UploadID)
					{
						headerText = "Most Recent by Upload ID";
					}
					// Count active categories, compare search element to search field
					if ( pkgText.Contains (fldSearch, StringComparison.OrdinalIgnoreCase) && (headerLast != headerText) )
					{
						headerLast = headerText;
						optionalCount += 1;

						//Debug.Log("optionalCount: " + optionalCount + "\n");
					}
				}

				// Optional counts for Exported packages for cat/pub
				if (sortActive == svSortBy.Category || sortActive == svSortBy.CategorySub || sortActive == svSortBy.Publisher)
				{
					if (packageShow.FindAll( x => x.category.label == "" ).Count > 0)
						optionalCount += 1;

					//if (packageShow.FindAll( x => x.publisher.label.Contains(fldSearch, StringComparison.OrdinalIgnoreCase) ).Count > 0) {
					//	optionalCount += 1;
					//}
				}

			}

			// Optimize End
			// ----------------------------------------------------------------------------------

			headerLast = String.Empty;
			headerText = String.Empty;

			// Hotkey event scrolling to Letter pressed?
			//if (evt.type == Input.anyKeyDown) {
			//}

			// The viewable frame of the scrollview
			svFrame = new Rect(0, (wTop + controlHeight), position.width, svHeight);
			// The scrollable area inside the frame (List of assets)
			svList = new Rect(0, 0, position.width-15, (svLineHeight * (searchCount + optionalCount)));

			svPosition = GUI.BeginScrollView(svFrame, svPosition, svList);
			for (int i = 0; i < packageShow.Count; ++i)
			{
				// Search handling
				if (searchActive == svSearchBy.Title)
				{
					if (packageShow[i].title != null)
						pkgText = packageShow[i].title;
				}
				if (searchActive == svSearchBy.Category)
				{
					if (packageShow[i].category.label != null)
						pkgText = packageShow[i].category.label;
				}
				if (searchActive == svSearchBy.Publisher)
				{
					if (packageShow[i].publisher.label != null)
						pkgText = packageShow[i].publisher.label;
				}

				if (pkgText.Contains (fldSearch, StringComparison.OrdinalIgnoreCase))
				{
					// Title header text
					if (sortActive == svSortBy.Title)
					{
						if (packageShow[i].collection == svCollection.Project)
						{
							headerText = "- Local Project -";
						}
						else
						{
							if (packageShow[i].isDamaged && packageShow[i].title.StartsWith("unknown")) {
								headerText = "- Unknown Asset Title -";
							}
							else
							{
								//headerText = packageShow[i].title.Trim()[0].ToString();
								headerText = RemoveLeading( packageShow[i].title )[0].ToString();
							}
						}
					}
					// Category header text
					if (sortActive == svSortBy.Category)
					{
						mainCategory = packageShow[i].category.label.Split('/');
						headerText = mainCategory[0];
					}
					// Category Sub header text
					if (sortActive == svSortBy.CategorySub)
						headerText = packageShow[i].category.label;
					// Publisher header text
					if (sortActive == svSortBy.Publisher)
						headerText = packageShow[i].publisher.label;
					if (sortActive == svSortBy.Size)
						headerText = GYAExt.GetByteRangeHeader(packageShow[i].fileSize);
					if (sortActive == svSortBy.PackageID)
						headerText = "Most Recent by Package ID";
					if (sortActive == svSortBy.VersionID)
						headerText = "Most Recent by Version ID";
					if (sortActive == svSortBy.UploadID)
						headerText = "Most Recent by Upload ID";

					//// Draw ddCat/ddPub header if needed
					//if (i == 0 && (ddCategory != "" || ddPublisher != "")) {
					//	if (ddCategory != "") {
					//		SVDrawHeader(svButton, ddCategory);
					//	}
					//	if (ddPublisher != "") {
					//		SVDrawHeader(svButton, ddPublisher);
					//	}
					//}

					//Draw header
					if (userData.Settings.enableHeaders || forceHeaders)
						SVDrawHeader(svButton, headerText);

					// Don't draw the control if the rect is not visible
					if ( svButton.yMin >= (svPosition.y - controlHeight) && svButton.yMax <= (svPosition.y + controlHeight) + svFrame.height )
					{
						// If hovering, get current asset info & draw line item
						//SVDrawLine(svButton, packageShow, i);
						SVDrawLine(svButton, i);
					}
					// Set up rectangles for the next line
					svButton.y += svLineHeight;
					svToggle.y += svLineHeight;
				}
			}
			GUI.EndScrollView();
		}

		// Determine if need to show header
		private void SVDrawHeader (Rect button, string headerCurrent)
		{
			// Force header If 1st char is blank/space
			//if (sortActive == svSortBy.Title) {
			//	if ( headerCurrent == " " ) {
			//		headerCurrent = "- Blank -";
			//	}
			//}
			// Force header if not an asset store package
			if (sortActive == svSortBy.Category || sortActive == svSortBy.CategorySub || sortActive == svSortBy.Publisher)
			{
				if ( headerCurrent == String.Empty )
					headerCurrent = "Exported Packages";
			}

			headerCurrent = headerCurrent.ToUpper();
			headerLast = headerLast.ToUpper();

			// Has header changed since last asset?
			if (headerLast != headerCurrent)
			{
				headerLast = headerCurrent;

				// Button - Header title
				GUI.Button (svToggle, "", svStyleSeperator);
				GUI.Button (svButton, headerCurrent, svStyleSeperator);

				// set up rectangles for the next line
				svButton.y += svLineHeight;
				svToggle.y += svLineHeight;
			}
		}

		// If hovering, get current assets info
		//private void SVDrawLine (Rect button, List<Packages> packageList, int i) {
		private void SVDrawLine (Rect button, int i)
		{
			////ddActive = false;

			// Selection line Hightlight - the toggle & icon
			GUI.BeginGroup( new Rect(0, svButton.y, position.width, svLineHeight), svStyleDefault);
			// Don't highlight the toggle & icon
			//GUI.BeginGroup( new Rect(0, svButton.y, position.width, svLineHeight));
			GUI.EndGroup();

			// TOGGLE - Asset Marked
			packageShow[i].isMarked = GUI.Toggle (svToggle, packageShow[i].isMarked, "");
			//packageList[i].isMarked = GUI.Toggle (svToggle, packageList[i].isMarked, "", svStyleDefault);

			//bool isAlt = false;
			string tmpUnityVer = packageShow[i].unity_version;
			if (packageShow[i].unity_version.Length == 0)
				tmpUnityVer = "0.0.0";

			// Pin Collection Type icon to left of list if enabled
			if (userData.Settings.enableCollectionTypeIcons)
			{
				//svStyleIcon.contentOffset = new Vector2(0, 1);
				Rect svIconLeft = new Rect(18, svButton.y+1, 18, svLineHeight);

				// Show Alt Unity Icons if Applicable
				if (!userData.Settings.enableAltIconOtherVersions)
				{
					// If not enable: Default - Forced
					if ( packageShow[i].collection == svCollection.Store)
						GUI.Button (svIconLeft, iconStore, svStyleIconLeft);
					if ( packageShow[i].collection == svCollection.User)
						GUI.Button (svIconLeft, iconUser, svStyleIconLeft);
					if ( packageShow[i].collection == svCollection.Standard)
						GUI.Button (svIconLeft, iconStandard, svStyleIconLeft);
					if ( packageShow[i].collection == svCollection.Old)
						GUI.Button (svIconLeft, iconOld, svStyleIconLeft);
				}
				else if ( int.Parse(tmpUnityVer.Substring(0,1)) == GYAExt.GetUnityVersionMajor )
				{
						// Default Icons - unity_version <= Application.unityVersion
						if ( packageShow[i].collection == svCollection.Store)
							GUI.Button (svIconLeft, iconStore, svStyleIconLeft);
						if ( packageShow[i].collection == svCollection.User)
							GUI.Button (svIconLeft, iconUser, svStyleIconLeft);
						if ( packageShow[i].collection == svCollection.Standard)
							GUI.Button (svIconLeft, iconStandard, svStyleIconLeft);
						if ( packageShow[i].collection == svCollection.Old)
							GUI.Button (svIconLeft, iconOld, svStyleIconLeft);
				}
				else if ( int.Parse(tmpUnityVer.Substring(0,1)) < 5 && GYAExt.GetUnityVersionMajor < 5 )
				{
					// Default Icons if unity_version & App.Unity < 5
					if ( packageShow[i].collection == svCollection.Store)
						GUI.Button (svIconLeft, iconStore, svStyleIconLeft);
					if ( packageShow[i].collection == svCollection.User)
						GUI.Button (svIconLeft, iconUser, svStyleIconLeft);
					if ( packageShow[i].collection == svCollection.Standard)
						GUI.Button (svIconLeft, iconStandard, svStyleIconLeft);
					if ( packageShow[i].collection == svCollection.Old)
						GUI.Button (svIconLeft, iconOld, svStyleIconLeft);
				}
				else
				{
					// Alt Icon
					if ( packageShow[i].collection == svCollection.Store)
						GUI.Button (svIconLeft, iconStoreAlt, svStyleIconLeft);
					if ( packageShow[i].collection == svCollection.User)
						GUI.Button (svIconLeft, iconUserAlt, svStyleIconLeft);
					if ( packageShow[i].collection == svCollection.Standard)
						GUI.Button (svIconLeft, iconStandardAlt, svStyleIconLeft);
					if ( packageShow[i].collection == svCollection.Old)
						GUI.Button (svIconLeft, iconOldAlt, svStyleIconLeft);
					//isAlt = true;
				}

				if ( packageShow[i].collection == svCollection.Project)
					GUI.Button (svIconLeft, iconProject, svStyleIconLeft);
			}

			// Event: MouseMove
			if ( button.Contains(evt.mousePosition) )
			{
				//if (evt.type != EventType.MouseUp || evt.type != EventType.MouseDown) {
				////	ddActive = false;
				//}
				pkgDetails = packageShow[i];

				// Asset info for foldout
				if (userData.Settings.showSVInfo)
				{
				infoToShow = GetTitleAssetVersionAppended(packageShow[i]);

					// This is so the info window shows blank when nothing has been initially selected
					// Show if Asset Store Package, if not then show minimal info
					string mTimeInfo = packageShow[i].fileDataCreated.ToString("dd MMM yyyy");
					if (!packageShow[i].isExported)
					{
						infoToShow += "\nCategory: " + packageShow[i].category.label;
						infoToShow += "\nPublisher: " + packageShow[i].publisher.label;
						//infoToShow += "\n" + packageShow[i].pubdate + "  -  " + GYAExt.BytesToKB(packageShow[i].fileSize);
						infoToShow += "\n" + packageShow[i].pubdate;
						if (mTimeInfo != packageShow[i].pubdate)
							infoToShow += " (" + mTimeInfo + ")";

						infoToShow += "  -  " + GYAExt.BytesToKB(packageShow[i].fileSize);
						infoToShow += " - ID: " + packageShow[i].id.ToString() + " - vID: " + packageShow[i].version_id.ToString();
						if (packageShow[i].upload_id > 0)
							infoToShow += " - uID: " + packageShow[i].upload_id.ToString();
					}
					else
					{
						if (infoToShow != String.Empty && packageShow[i].fileSize != 0)
						{
							infoToShow += "\nCategory: n/a";
							infoToShow += "\nPublisher: n/a";
							infoToShow += "\nNA - (" + mTimeInfo + ")  -  " + GYAExt.BytesToKB(packageShow[i].fileSize);
							//infoToShow += "\n" + GYAExt.BytesToKB(packageList[i].fileSize);
						}
					}
					////infoToShow += "\nCollection: " + packageList[i].collection.ToString().ToUpper();
					//string collName = packageList[i].collection.ToString().ToUpper();
					//// Handle Unity 4/5 difference
					//if (collName == "STANDARD") {
					//	collName = GYAExt.FolderUnityStandardAssets.ToUpper();
					//}
					//infoToShow += "\nCollection: " + collName;
					//if (packageList[i].isOldToMove) {
					//	infoToShow += " - CAN BE CONSOLIDATED";
					//}

					if (IsSortActive(svSortBy.DateFile))
					{
						infoToShow += "\n" + packageShow[i].fileDateCreated;
					}
					else if (IsSortActive(svSortBy.DatePublish))
					{
						infoToShow += "\n" + packageShow[i].pubdate;
					}
					else if (IsSortActive(svSortBy.DatePackage))
					{
						infoToShow += "\n" + mTimeInfo;
					}
					else
					{
						infoToShow += "\n" + packageShow[i].filePath;
					}

				}
			}

			// Event: MouseDown
			// Possible issue with updaing the line
			// MouseUp changes line correctly on right-click
			// MouseDown prevents left-click not responding/locking

			// ScrollView Click handing

			if ( evt.type == EventType.MouseUp && button.Contains(evt.mousePosition))
			{
			//if ( Input.GetMouseButtonUp && button.Contains(evt.mousePosition) ) {
				//Debug.Log("SVevt SVPopUpRC: " + evt + "\n" + evt.button.ToString());

				// Left Click
				if (evt.button == 0) {
					if (ddActive)
					{
						ddActive = false;
					}
					else
					{
						packageShow[i].isMarked = !packageShow[i].isMarked;
					}
				}

				// Right Click
				if (evt.button == 1)
				{
					if (ddActive)
					{
						ddActive = false;
					}
					else
					{
						// Activate Popup menu
						ddActive = true;
						SVPopUpRightClick();
					}
				}
			}

			//} else {
			// List Item - Asset Title
			string pkgTitle = packageShow[i].title;
			if (!packageShow[i].isExported)
			{

				if (pkgTitle.EndsWith( packageShow[i].version ))
				{
				// Do not add version
				}
				else
				{
					pkgTitle += " v" + packageShow[i].version.TrimStart ( 'v', 'V');
				}
			}

			// Highlight titles
			switch (packageShow[i].collection)
			{
			case svCollection.Store:
				if (pkgData.countStore > 0)
				{
					//GUI.Button (svButton, pkgTitle, svStyle);
					//if (isAlt) {
					//	GUI.Button (svButton, pkgTitle, svStyleStoreAlt);
					//} else {
						GUI.Button (svButton, pkgTitle, svStyleStore);
					//}
				}
				break;
			case svCollection.User:
				if (pkgData.countUser > 0)
					GUI.Button (svButton, pkgTitle, svStyleUser);
				break;
			case svCollection.Standard:
				if (pkgData.countStandard > 0)
					GUI.Button (svButton, pkgTitle, svStyleStandard);
				break;
				//goto case svCollection.All;
			case svCollection.Old:
				if (pkgData.countOld > 0 || pkgData.countOldToMove > 0)
				{
					if (packageShow[i].isOldToMove)
					{
						GUI.Button (svButton, pkgTitle, svStyleOldToMove);
					}
					else
					{
						GUI.Button (svButton, pkgTitle, svStyleOld);
					}
				}
				break;
			case svCollection.Project:
				if (pkgData.countProject > 0)
					GUI.Button (svButton, pkgTitle, svStyleProject);
				break;
			default:
				GUI.Button (svButton, pkgTitle, svStyleStandard);
				break;
			}

			// Icon X position
			int lineX = 0;
			int isPinned = 0; // // # of pinned items on right side +1 for scrollbar if required

			// Shift icon position to account for scrollbar
			if ( svList.height > svFrame.height )
			{
				lineX = -16;
				isPinned += 1;
			}

			// Pin favorite icon to right of list if required
			if ( GroupContainsAsset(0, packageShow[i]) )
			{
				lineX = -16 * isPinned;
				isPinned += 1;

				svStyleIcon.contentOffset = new Vector2(lineX, 1);
				GUI.Button (svButton, iconFavorite, svStyleIcon);
			}

			// Pin Damaged Icon to right of list, left of fav icon if exist
			if (packageShow[i].isDamaged)
			{

			//if ( isFavPinned ) {
				lineX = -16 * isPinned;
				isPinned += 1;
				//}
				svStyleIcon.contentOffset = new Vector2(lineX, 1);

				//Rect svIconRight = new Rect(lineX, svButton.y+1, 18, svLineHeight);
				GUI.Button (svButton, iconDamaged, svStyleIcon);
				//GUI.Button (svIconRight, iconStore, svStyleIconRight);
			}

			// Pin Unity 5 Icon to right of list, left of fav icon if exist
			/*
			if (packageShow[i].filePath.Contains(GYAExt.FolderUnityAssetStore5)) {

			//if ( isFavPinned ) {
				lineX = -16 * isPinned;
				isPinned += 1;
			//}
				svStyleIcon.contentOffset = new Vector2(lineX, 1);

			//Rect svIconRight = new Rect(lineX, svButton.y+1, 18, svLineHeight);
			//GUI.Button (svIconRight, iconStore, svStyleIconRight);
				GUI.Button (svButton, iconUnity5, svStyleIcon);
			}
			*/

			//}
		}

		// Popup window routine for Left button
		private void SVPopUpRightClick ()
		{
			// Now create the menu, add items and show it
			GenericMenu popupMenu = new GenericMenu ();

			popupMenu.AddDisabledItem (new GUIContent ("Highlighted Asset Options:"));
			//popupMenu.AddSeparator("");
			popupMenu.AddItem (new GUIContent ("Import"), false, TBPopUpCallback, "PopupLoad" );
			popupMenu.AddItem (new GUIContent ("Import Interactively"), false, TBPopUpCallback, "PopupLoadInteractive" );
			popupMenu.AddSeparator ("");
			// Clicking will copy asset path to clipboard
			popupMenu.AddItem (new GUIContent ("Copy Path To Clipboard"), false, TBPopUpCallback, "CopyPathToClipboard" );
			popupMenu.AddSeparator ("");
			popupMenu.AddItem (new GUIContent ("Open Folder"), false, TBPopUpCallback, "AssetFolder" );
			popupMenu.AddSeparator ("");
			// Is Asset Store package? Then can go to URL
			if (!pkgDetails.isExported)
			{
				popupMenu.AddItem (new GUIContent ("Open URL: Unity Asset Store"), false, TBPopUpCallback, "AssetURL" );
				popupMenu.AddItem (new GUIContent ("Open URL: Fast Asset Store"), false, TBPopUpCallback, "AssetURL2" );
			}
			else
			{
				popupMenu.AddDisabledItem (new GUIContent ("Open URL"));
			}
			popupMenu.AddSeparator("");
			// Group Add To Menu - Don't show for Old/Project as assets in there are only temporary
			if (pkgDetails.collection != svCollection.Old && pkgDetails.collection != svCollection.Project)
			{
				for (int i = 0; i < userData.Group.Count; ++i)
				{
					popupMenu.AddItem (new GUIContent ("Add to Group/" + i.ToString() + " - " + userData.Group[i].name), false, GroupAddTo, i );
				}
			}
			else
			{
				popupMenu.AddDisabledItem (new GUIContent ("Add to Group"));
			}
			if (showActive == svCollection.Group)
			{
				// Remove asset from group
				popupMenu.AddItem (new GUIContent ("Remove from Group"), false, GroupRemoveAsset, null );
			}
			else
			{
				popupMenu.AddDisabledItem (new GUIContent ("Remove from Group"));
			}
			popupMenu.AddSeparator("");

			if (showActive != svCollection.Group)
			{
				// If not in User Assets folder
				if ( !pkgDetails.filePath.Contains(userData.Settings.pathUserAssets, StringComparison.OrdinalIgnoreCase) )
				{
					popupMenu.AddItem (new GUIContent ("File Options/Copy to User Folder"), false, TBPopUpCallback, "CopyToUser" );
				}
				else
				{
					popupMenu.AddDisabledItem (new GUIContent ("File Options/Copy to User Folder"));
				}
				popupMenu.AddItem (new GUIContent ("File Options/Copy to ..."), false, TBPopUpCallback, "CopyToSelectable" );
				if ( !pkgDetails.filePath.Replace('/', '\\').StartsWith(pathOldAssetsFolder.Replace('/', '\\'), StringComparison.OrdinalIgnoreCase) && !(showActive == svCollection.Standard) && (!pkgDetails.isExported))
				{
					popupMenu.AddItem (new GUIContent ("File Options/Move to Old Assets"), false, TBPopUpCallback, "MoveToOld" );
				}
				else
				{
					popupMenu.AddDisabledItem (new GUIContent ("File Options/Move to Old Assets"));
				}
				popupMenu.AddItem(new GUIContent("File Options/"), false, TBPopUpCallback, "");
				//popupMenu.AddItem (new GUIContent ("File Options/Rename Asset"), false, TBPopUpCallback, "RenameAsset" );
				if (!pkgDetails.isExported)
				{
					popupMenu.AddItem (new GUIContent ("File Options/Rename with Version"), false, RenameWithVersion, pkgDetails );
				}
				else
				{
					popupMenu.AddDisabledItem (new GUIContent ("File Options/Rename with Version"));
				}
				popupMenu.AddItem(new GUIContent("File Options/"), false, TBPopUpCallback, "");
				popupMenu.AddItem (new GUIContent ("File Options/Delete Asset"), false, TBPopUpCallback, "DeleteAsset" );
			}

			popupMenu.AddSeparator ("");
			// category.label: Change '/' and '&' as they affect the popup
			string pkgTitle = EscapeMenuItem(pkgDetails.title);
			string pkgCategoryLabel = EscapeMenuItem(pkgDetails.category.label);

			popupMenu.AddDisabledItem (new GUIContent ("Title: " + pkgTitle));
			if(!pkgDetails.isExported)
				popupMenu.AddDisabledItem (new GUIContent ("Version: " + pkgDetails.version));

			popupMenu.AddDisabledItem (new GUIContent ("Size: " + GYAExt.BytesToKB(pkgDetails.fileSize)));
			if (!userData.Settings.showSVInfo)
			{
				if(!pkgDetails.isExported)
				{
					popupMenu.AddDisabledItem (new GUIContent ("Category: " + pkgCategoryLabel));
					popupMenu.AddDisabledItem (new GUIContent ("Publisher: " + pkgDetails.publisher.label));
					popupMenu.AddDisabledItem (new GUIContent ("Date: " + pkgDetails.pubdate));
				}
			}

			// Dump package info to console
			//popupMenu.AddItem (new GUIContent ("Dump Package Info to Console"), false, SVPopUpCallback, pkgDetails.version_id );

			// Show if there is more then 1 version of the asset
			if (CountDuplicatesOfID(pkgDetails.id) > 1 && !pkgDetails.isExported)
			{
				popupMenu.AddSeparator ("");
				// Show other versions at bottom of the popup
				popupMenu.AddDisabledItem (new GUIContent ("All Versions:"));
				// Show newest version(s), should only be 1
				foreach (Packages package in pkgData.Assets)
				{
					// Check for '/' in the asset title
					pkgTitle = EscapeMenuItem(package.title);
					if (package.id == pkgDetails.id && package.collection == svCollection.Store)
					{
						// Pass version_id to know which package to go to
						popupMenu.AddItem (new GUIContent (package.version + " - " + pkgTitle), (package.version_id == pkgDetails.version_id), SVPopUpCallback, package.version_id );
						// Clicking will copy asset path to clipboard "CopyPathToClipboard"
						//popupMenu.AddItem (new GUIContent (package.version + " - " + pkgTitle), (package.version_id == pkgDetails.version_id), TBPopUpCallback, "CopyPathToClipboard" );
					}
				}

				// Sort Packages, work on a copy
				List<Packages> packagesTemp = new List<Packages> (pkgData.Assets);
				packagesTemp.Sort((x,y) => -x.version_id.CompareTo(y.version_id));

				// Show oldest versions
				foreach (Packages package in packagesTemp)
				{
					// Check for '/' in the asset title
					pkgTitle = EscapeMenuItem(package.title);
					if (package.id == pkgDetails.id)
					{
						// Pass version_id to know which package to go to
						//popupMenu.AddItem (new GUIContent (package.version + " - " + pkgTitle), (package.version_id == pkgDetails.version_id), SVPopUpCallback, package.version_id );
						popupMenu.AddItem (new GUIContent (package.version + " - " + pkgTitle), (package.version_id == pkgDetails.version_id), SVPopUpCallback, package.filePath );
						//popupMenu.AddItem (new GUIContent (package.version + " - " + pkgTitle), (package.version_id == pkgDetails.version_id), TBPopUpCallback, "CopyPathToClipboard" );
					}
				}
			}
			// Adjust for Icons
			float ddLoc = 18;
			if (userData.Settings.enableCollectionTypeIcons)
				ddLoc += 18;

			// Fix a popup y axis position difference between the 2 platforms in the scrollview
			if (GYAExt.IsOSMac)
			{
				//popupMenu.DropDown(new Rect(ddLoc, svButton.yMax-8, 0, 0));
				popupMenu.DropDown(new Rect(ddLoc, svButton.y+8, 0, 0));
			}
			else
			{
				//popupMenu.DropDown(new Rect(ddLoc, svButton.yMax, 0, 0));
				popupMenu.DropDown(new Rect(ddLoc+4, svButton.y+16, 0, 0));
			}
			//evt.Use();
			//ddActive = false;
		}

		// TODO: Dump package info to Console
		private void SVPopUpCallback (object pObj)
		{
			//string objString = (string)pObj;
			TextEditor te = new TextEditor();
			//te.content = new GUIContent(pkgDetails.filePath);
			#if UNITY_5_3_OR_NEWER
			te.text = pObj.ToString();
			#else
			te.content = new GUIContent(pObj.ToString());
			#endif
			te.SelectAll();
			te.Copy();
		}

		// Check if search is active
		bool IsSearchActive (svSearchBy searchVal)
		{
			if (searchVal == searchActive)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		// Check if sort is active
		bool IsSortActive (svSortBy sortVal)
		{
			if (sortVal == sortActive)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private string RemoveLeading(string s)
		{
			if (!String.IsNullOrEmpty(s))
				s = s.Trim();

			if (String.IsNullOrEmpty(s))
				return s;

		    //if (s.StartsWith("the ", StringComparison.CurrentCultureIgnoreCase)) {
			//	return s.Substring(4).TrimStart();
			//}

			if (!char.IsLetterOrDigit(s[0]))
				return s.Trim().Substring(1).TrimStart();

			return s;
		}

		// Sort packages by ...
		private void PackagesSearchBy (object searchBy)
		{
			PackagesSearchBy((svSearchBy)searchBy);
		}

		private void PackagesSearchBy (svSearchBy searchBy)
		{
			ddActive = false;
			searchActive = (svSearchBy)searchBy;
		}

		// Sort packages by ...
		private void PackagesSortBy (object sortBy)
		{
			PackagesSortBy((svSortBy)sortBy);
		}

		private void PackagesSortBy (svSortBy sortBy)
		{
			//Debug.Log (gyaAbbr + " -PackagesSortBy-\n");
			ddActive = false;
			// Sort by Title
			if (sortBy == svSortBy.Title)
			{
				sortActive = svSortBy.Title;
				//pkgData.Assets = pkgData.Assets.OrderByDescending(x => x.collection == svCollection.Project).ThenBy(x => RemoveLeading(x.title)).ThenByDescending(x => x.version_id).ThenBy(x => x.collection).ToList ();
				pkgData.Assets = pkgData.Assets.OrderByDescending(x => x.isDamaged && x.title.StartsWith("unknown")).ThenByDescending(x => x.collection == svCollection.Project).ThenBy(x => RemoveLeading(x.title)).ThenByDescending(x => x.version_id).ThenBy(x => x.collection).ToList ();

				// Sort Groups
				for (int i = 0; i < grpData.Count; ++i)
				{
					grpData[i] = grpData[i].OrderBy(x => RemoveLeading(x.title)).ThenByDescending(x => x.version_id).ToList ();
				}
			}
			// Sort by Main Category and Title
			if (sortBy == svSortBy.Category)
			{
				sortActive = svSortBy.Category;;
				pkgData.Assets = pkgData.Assets.OrderBy(x => x.category.label.Split('/')[0]).ThenBy(x => x.title).ToList();
				// Sort Groups
				for (int i = 0; i < grpData.Count; ++i)
				{
					grpData[i] = grpData[i].OrderBy(x => x.category.label.Split('/')[0]).ThenBy(x => x.title).ToList();
				}
			}
			// Sort by Sub Categories and Title
			if (sortBy == svSortBy.CategorySub)
			{
				sortActive = svSortBy.CategorySub;
				pkgData.Assets = pkgData.Assets.OrderBy(x => x.category.label).ThenBy(x => x.title).ToList ();
				// Sort Groups
				for (int i = 0; i < grpData.Count; ++i)
				{
					grpData[i] = grpData[i].OrderBy(x => x.category.label).ThenBy(x => x.title).ToList ();
				}
			}
			// Sort by Publisher and Title
			if (sortBy == svSortBy.Publisher)
			{
				sortActive = svSortBy.Publisher;
				pkgData.Assets = pkgData.Assets.OrderBy(x => x.publisher.label).ThenBy(x => x.title).ToList ();
				// Sort Groups
				for (int i = 0; i < grpData.Count; ++i)
				{
					grpData[i] = grpData[i].OrderBy(x => x.publisher.label).ThenBy(x => x.title).ToList ();
				}
			}
			// Sort by Size
			if (sortBy == svSortBy.Size)
			{
				sortActive = svSortBy.Size;
				pkgData.Assets.Sort((x,y) => -x.fileSize.CompareTo(y.fileSize));
				// Sort Groups
				for (int i = 0; i < grpData.Count; ++i)
				{
					grpData[i].Sort((x,y) => -x.fileSize.CompareTo(y.fileSize));
				}
			}
			// Sort by Date File
			if (sortBy == svSortBy.DateFile)
			{
				sortActive = svSortBy.DateFile;
				pkgData.Assets.Sort((x,y) => -x.fileDateCreated.CompareTo(y.fileDateCreated));
				// Sort Groups
				for (int i = 0; i < grpData.Count; ++i)
				{
					grpData[i].Sort((x,y) => -x.fileDateCreated.CompareTo(y.fileDateCreated));
				}
			}
			// Sort by Date Publish
			if (sortBy == svSortBy.DatePublish)
			{
				sortActive = svSortBy.DatePublish;
				//pkgData.Assets.Sort((x,y) => -x.pubdate.CompareTo(y.pubdate));
				pkgData.Assets.Sort((x,y) => -PubDateToDateTime(x.pubdate).CompareTo( PubDateToDateTime(y.pubdate) ));
				// Sort Groups
				for (int i = 0; i < grpData.Count; ++i)
				{
					//grpData[i].Sort((x,y) => -x.pubdate.CompareTo(y.pubdate));
					grpData[i].Sort((x,y) => -PubDateToDateTime(x.pubdate).CompareTo( PubDateToDateTime(y.pubdate) ));
				}
			}
			// Sort by Date Package Build
			if (sortBy == svSortBy.DatePackage)
			{
				sortActive = svSortBy.DatePackage;
				pkgData.Assets.Sort((x,y) => -x.fileDataCreated.CompareTo(y.fileDataCreated));
				// Sort Groups
				for (int i = 0; i < grpData.Count; ++i)
				{
					grpData[i].Sort((x,y) => -x.fileDataCreated.CompareTo(y.fileDataCreated));
				}
			}
			// Sort by Package ID
			if (sortBy == svSortBy.PackageID)
			{
				sortActive = svSortBy.PackageID;
				pkgData.Assets = pkgData.Assets.OrderByDescending(x => x.id).ThenByDescending(x => x.version_id).ToList ();
				// Sort Groups
				for (int i = 0; i < grpData.Count; ++i)
				{
					grpData[i] = grpData[i].OrderByDescending(x => x.id).ThenByDescending(x => x.version_id).ToList ();
				}
			}
			// Sort by Version ID
			if (sortBy == svSortBy.VersionID)
			{
				sortActive = svSortBy.VersionID;
				pkgData.Assets.Sort((x,y) => -x.version_id.CompareTo(y.version_id));
				// Sort Groups
				for (int i = 0; i < grpData.Count; ++i)
				{
					grpData[i].Sort((x,y) => -x.version_id.CompareTo(y.version_id));
				}
			}
			// Sort by Upload ID
			if (sortBy == svSortBy.UploadID)
			{
				sortActive = svSortBy.UploadID;
				pkgData.Assets = pkgData.Assets.OrderByDescending(x => x.upload_id).ThenBy(x => RemoveLeading(x.title)).ToList ();

				// Sort Groups
				for (int i = 0; i < grpData.Count; ++i)
				{
					grpData[i] = grpData[i].OrderByDescending(x => x.upload_id).ThenBy(x => RemoveLeading(x.title)).ToList ();
				}
			}
		}

		// Create folder if not exist for old versions
		private void CreateFolder (string folder, bool silent = false)
		{
			// Create Folder if it is missing
			try
			{
				if (!Directory.Exists(folder))
				{
					Directory.CreateDirectory (folder);
					if (!silent) {
						Debug.Log(gyaAbbr + " - Created Folder:\t'" + folder + "'\n");
					}
				}
			}
			catch (IOException ex)
			{
				Debug.LogError(gyaAbbr + " - Error Creating Folder:\t'" + folder + "'\n" + ex.Message);
			}
		}

		bool OldAssetNeedsToMove (List<Packages> packages, int oldLine)
		{
			bool needsToMove = !(Path.GetFullPath( Path.GetDirectoryName(packages[oldLine].filePath) ) == pathOldAssetsFolder);
			return needsToMove;
		}

		private void OldAssetsDeleteAll (bool needsRefresh = true)
		{
			string filesInfo = String.Empty;
			int filesDeleted = 0;
			List<Packages> packageDelete = pkgData.Assets;
			string fileToDelete = String.Empty;

			// Check all old assets
			for (int i = 0; i < packageDelete.Count; ++i)
			{
				fileToDelete = Path.GetFullPath( packageDelete[i].filePath );
				// Is asset in the old asset folder
				if ( fileToDelete.Replace('/', '\\').Contains(pathOldAssetsFolder.Replace('/', '\\'), StringComparison.OrdinalIgnoreCase) )
				{
					// Delete asset
					var fileData = DeleteAsset (packageDelete[i]);
					filesDeleted = filesDeleted + fileData.Key;
					filesInfo = filesInfo + fileData.Value.Split('\n')[1] + "\n";
				}
			}
			if (filesDeleted > 0)
			{
				Debug.Log(gyaAbbr + " - ( " + filesDeleted.ToString() + " ) package(s) deleted from the Old Assets folder.\n" + filesInfo);
			}
			// Make sure list is up-to-date
			if (needsRefresh)
			{
				//RefreshPackages();
				ScanOld();
			}
		}

		// Delete the selected assets
		private void DeleteAssetMultiple (bool needsRefresh = true)
		{
			string filesInfo = String.Empty;
			int filesDeleted = 0;
			List<Packages> packageDelete = pkgData.Assets.FindAll( x => x.isMarked );

			// Check all assets
			for (int i = 0; i < packageDelete.Count; ++i)
			{
				// Is asset marked
				if ( packageDelete[i].isMarked )
				{
					//Debug.Log(packageDelete[i].isMarked + " - " + packageDelete[i].title);

					// Delete asset
					var fileData = DeleteAsset (packageDelete[i]);
					filesDeleted = filesDeleted + fileData.Key;
					filesInfo = filesInfo + fileData.Value.Split('\n')[1] + "\n";
				}
			}
			if (filesDeleted > 0)
				Debug.Log(gyaAbbr + " - ( " + filesDeleted.ToString() + " ) package(s) deleted.\n" + filesInfo);

			// Make sure list is up-to-date
			if (needsRefresh)
				RefreshPackages();
		}

		// Move assets to the passed folder - defaults to old assets
		private KeyValuePair<int,string> DeleteAsset (Packages packageMove)
		{
			string moveInfo = String.Empty;
			int filesDeleted = 0;
			string fileToDelete = Path.GetFullPath( packageMove.filePath );

			// Does file already exist at destination?
			if (File.Exists(fileToDelete))
			{
				// Yes it is
				moveInfo = moveInfo + "Deleted: " + packageMove.title + "\n" + "Path: " + fileToDelete + "\n";

				try
				{
					// Delete the file
					File.Delete(@fileToDelete);
					pkgData.Assets.Remove(packageMove);

					// Verification
					if (File.Exists(fileToDelete))
					{
						Debug.LogWarning(gyaAbbr + " - Error: File not Deleted: \n" + moveInfo);
					}
					else
					{
						filesDeleted += 1;
					}
				}
				catch (IOException ex)
				{
					Debug.LogWarning(gyaAbbr + " - Error: File Delete Failed: \n" + moveInfo + "\n" + ex.Message);
				}
			}
			else
			{
				Debug.Log(gyaAbbr + " - Unable to delete - File doesn't exist: " + fileToDelete + "\n");
			}
			return new KeyValuePair<int,string>( filesDeleted, moveInfo );
		}

	// Create full version suffix to append: v<Asset Version (<Unity Version>)
	//private string GetAssetVersionToAppend (Packages packageName, bool includeUnknown = false)
	private string GetAssetVersionToAppend (Packages packageName)
	{
		// Asset version - Default
		string appendString = "";
		string verString = packageName.version;
		string uniString = packageName.unity_version;

		if (!packageName.isExported)
		{

			// Unity version string
			if (uniString == null || uniString.Length == 0)
			{
				// Unity version tag missing or blank
				uniString = "";
			}

			// Version string
			if (verString.Length > 0)
			{
				appendString = " v" + verString;
			}

			if (uniString.Length > 0)
				appendString = appendString + " (" + uniString + ")";
		}

		//Debug.Log("GetAssetVersionToAppend: " + appendString);
		return appendString;
	}

	private string GetTitleAssetVersionRemoved (Packages packageName)
	{
		// Make asset title the filename
		//Debug.Log(GYAIO.RemoveInvalidCharsFromFileName(packageMove.title));
		//filename = GYAIO.RemoveInvalidCharsFromFileName(packageMove.title);
		string filename = GYAIO.ReturnValidFile(packageName.title);
		// Create full version suffix to append
		string verString = GetAssetVersionToAppend(packageName);

		// Remove version
		if (verString.Length > 0)
			filename = filename.Replace(verString, "");

		//Debug.Log("GetTitleAssetVersionRemoved: " + filename);
		return filename;
	}

	private string GetTitleAssetVersionAppended (Packages packageName, bool includeExtension = false)
	{
		// Make asset title the filename
		//Debug.Log(GYAIO.RemoveInvalidCharsFromFileName(packageMove.title));
		//filename = GYAIO.RemoveInvalidCharsFromFileName(packageMove.title);
		string filename = GYAIO.ReturnValidFile(packageName.title);
		// Create full version suffix to append
		string verString = GetAssetVersionToAppend(packageName);

		//Debug.Log("GetTitleAssetVersionRemoved: " + GetTitleAssetVersionRemoved(packageName));

		// Get filename and add version to it
		//filename = Path.GetFileNameWithoutExtension(packageMove.filePath);
		//if (filename.ToLower() == "unknown" && !string.IsNullOrEmpty(packageMove.title) ) {

		//}

		//// In case the version is longer then the filename
		//	string verString2 = String.Empty;
		//	if (verString.Length < filename.Length) {
		//		verString2 = filename.Substring(filename.Length - verString.Length);
		//	}

		// Todo: change this to generate from the title, version_id, unity_version

		//// Check if version is already appended
		//	if (verString == verString2) {
		//	// Version is already appended, do not re-append
		//		verString = "";
		//	} else {
		//	// Version has not been appended by GYA, append it
		//		verString = " " + verString;
		//	//verString = " " + verString + (packageMove.unity_version.Length > 0 ? " (" + packageMove.unity_version + ")" : "");
		//	}

		// Add version and extension
		if (!filename.Contains(verString))
			filename = filename + verString;
		if (includeExtension)
			filename = filename + Path.GetExtension(packageName.filePath);

		// Check for invalid chars - Filename
		//Debug.Log(GYAIO.RemoveInvalidCharsFromFileName (filename, false));
		//Debug.Log(GYAIO.ReturnValidPath(filename));
		// Check for invalid chars - Path
		//pathMoveAsset = GYAIO.ReturnValidPath(pathMoveAsset);
		//pathMoveAsset = Path.GetFullPath( Path.Combine(pathMoveAsset, filename) );

		//Debug.Log("GetTitleAssetVersionAppended: " + filename);
		return filename;
	}

	// Move assets to folder - defaults to old assets - copyOverride forces a copy instead of move
	private KeyValuePair<int,string> MoveAssetToPath (Packages packageMove, string pathMoveAsset = null, bool copyOverride = false, bool quietMode = false, bool appendVer = true, bool dryRun = false)
	{
		string moveInfo = String.Empty;
		int filesMoved = 0;

		//string filename = Path.GetFileNameWithoutExtension(packageMove.filePath);
		string filename = Path.GetFileName( packageMove.filePath );
		string pathMoveFrom = GYAExt.PathFixedForOS( Path.GetFullPath( packageMove.filePath ) );

		// Path to move to
		if (pathMoveAsset == null)
			pathMoveAsset = pathOldAssetsFolder;

		// Begin rename
		if (appendVer)
		{
			//
			filename = GetTitleAssetVersionAppended(packageMove, true);

			// Check for invalid chars - Filename
			//Debug.Log(GYAIO.RemoveInvalidCharsFromFileName (filename, false));
			//Debug.Log(GYAIO.ReturnValidPath(filename));
			// Check for invalid chars - Path
			pathMoveAsset = GYAIO.ReturnValidPath(pathMoveAsset);
			pathMoveAsset = Path.GetFullPath( Path.Combine(pathMoveAsset, filename) );

		}
		// End rename

		// Does file already exist at destination?
		if (File.Exists(pathMoveAsset))
		{
			// Yes it is, do nothing
			if (!quietMode)
				Debug.Log(gyaAbbr + " - File already exists: " + pathMoveAsset + "\nFile NOT copied !!");
		}
		else
		{
			// Create folder if required
			if (!Directory.Exists( Path.GetDirectoryName(pathMoveAsset) ))
			{
				//Debug.Log(gyaAbbr + " - Creating Asset Folder: " + Path.GetDirectoryName(pathMoveAsset) + "\n");
				Directory.CreateDirectory (Path.GetDirectoryName(pathMoveAsset));
			}

			// No, it's not
			string copyMoveTxt = "Move";
			if (copyOverride)
				copyMoveTxt = "Copy";

			moveInfo = moveInfo + copyMoveTxt + ": " + pathMoveFrom + "\n" +
				"To: " + pathMoveAsset + "\n";

			try
			{
				// If not dryRun, perform action
				if (!dryRun)
				{
					// Move the file
					try
					{
						if (File.Exists (pathMoveFrom))
							File.SetAttributes(pathMoveFrom, FileAttributes.Normal);

						if (copyOverride)
						{
							// Copy file
							File.Copy(pathMoveFrom, pathMoveAsset);
						}
						else
						{
							// Move file
							if (GYAIO.IsSymLink(pathMoveFrom) || GYAIO.IsSymLink(pathMoveAsset))
							{
							//if (GYAIO.IsSymLink(pathMoveFrom)) {
								// Must Copy/Delete because of symlink
								File.Copy(pathMoveFrom, pathMoveAsset);
								if (File.Exists(pathMoveAsset))
									File.Delete(pathMoveFrom);
							}
							else
							{
								// Can Move file
								File.Move(pathMoveFrom, pathMoveAsset);
							}
						}
					}
					catch (IOException ex)
					{
						Debug.LogWarning(gyaAbbr + " - Error " + (copyOverride ? "Copying: " : "Moving: ") + pathMoveFrom + " to " + pathMoveAsset + " \n" + ex.Message);
					}

					// Verification
					if (File.Exists(pathMoveAsset))
					{
						filesMoved += 1;
					}
					else
					{
						Debug.LogWarning(gyaAbbr + " - Error: File " + copyMoveTxt + " - Unable to locate file at the target path: \n" + moveInfo);
					}
				}
				else
				{
					filesMoved += 1;
				}
			}
			catch (IOException ex)
			{
				Debug.LogWarning(gyaAbbr + " - Error: File " + copyMoveTxt + " Failed: \n" + moveInfo + "\n" + ex.Message);
			}
		}
		return new KeyValuePair<int,string>( filesMoved, moveInfo );
	}

		// Delete empty folders recursively & handle .DS_Store files
		private static void DeleteEmptySubFolders(string startLocation)
		{
			if (Directory.Exists(startLocation))
			{
				foreach (var directory in Directory.GetDirectories(startLocation))
				{
					//Debug.Log( Directory.GetFileSystemEntries(directory,".DS_Store").Length + " - " + Directory.GetFileSystemEntries(directory).Length + " - " + directory);

					try
					{
						// If exists ds_store, delete it if it's the only file in folder
						if (Directory.GetFileSystemEntries(directory,".DS_Store").Length == 1)
						{
						//File.SetAttributes(Path.Combine(directory, ".DS_Store"), FileAttributes.Normal);
							File.Delete(Path.Combine(directory, ".DS_Store"));
						}

						DeleteEmptySubFolders(directory);
						if (Directory.GetFileSystemEntries(directory).Length == 0)
							Directory.Delete(directory, false);
					}
					catch (IOException ex)
					{
						Debug.LogWarning(gyaAbbr + " - Error: DeleteEmptySubFolders Failed: \n" + directory + "\n" + ex.Message);
					}
				}
			}
		}

		// Move marked assets to the prescribed folder
		private void CopyToSelected (string path)
		{
			int filesMoved = 0;
			string filesInfo = String.Empty;

	        // Check all old assets
	        //        for (int i = 0; i < pkgData.Old.Count; ++i) {
			//if (userData.Settings.showProgressBarDuringFileAction) {
			int stepNumber = 0;
			Packages packageToCopy = null;
			using (
				var progressBar = new ProgressBar(string.Format("{0} Copying Selected Package(s)", gyaAbbr), CountToImport(), 0, _ => Path.GetFileName(packageToCopy.filePath), userData.Settings.showProgressBarDuringFileAction)
			)
			//}
				for (int i = 0; i < pkgData.Assets.Count; ++i)
				{
        			// Is asset not in the old asset folder
					if ( pkgData.Assets[i].isMarked )
					{
					//if (userData.Settings.showProgressBarDuringFileAction) {
						packageToCopy = pkgData.Assets[i];
						//progressBar.Update(stepNumber++, userData.Settings.showProgressBarDuringFileAction);
						progressBar.Update(stepNumber++);
					//}
            	   // Move asset
						var fileData = MoveAssetToPath(pkgData.Assets[i], path, true);
						filesMoved = filesMoved + fileData.Key;
						filesInfo = filesInfo + fileData.Value;
					}
				}
			if (filesMoved > 0)
				Debug.Log(gyaAbbr + " - ( " + filesMoved.ToString() + " ) package(s) copied to: " + path + "\n" + filesInfo);

			// Make sure list is up-to-date
			//RefreshPackages();
		}

		// NOTUSED - Move marked assets to the prescribed folder
		private void CopyToSelected2 (string path)
		{
			int filesMoved = 0;
			string filesInfo = String.Empty;

			// Check all old assets
			//		for (int i = 0; i < pkgData.Old.Count; ++i) {
			for (int i = 0; i < pkgData.Assets.Count; ++i)
			{
				// Is asset not in the old asset folder
				if ( pkgData.Assets[i].isMarked )
				{
					// Move asset
					var fileData = MoveAssetToPath(pkgData.Assets[i], path, true);
					filesMoved = filesMoved + fileData.Key;
					filesInfo = filesInfo + fileData.Value;
				}
			}
			if (filesMoved > 0)
				Debug.Log(gyaAbbr + " - ( " + filesMoved.ToString() + " ) package(s) copied to: " + path + "\n" + filesInfo);

			// Make sure list is up-to-date
			RefreshPackages();
		}

	// Rename asset to include version
	private void RenameWithVersion (object package)
	{
		Packages pObject = (Packages)package;
		//Debug.Log(gyaAbbr + " -\n" + pObject.filePath);
		RenameWithVersion(pObject, true);

	}

	// Rename asset to include version
	private void RenameWithVersion (Packages package, bool showResults = true)
	{
		int filesMoved = 0;
		string filesInfo = String.Empty;

		if ( !package.isExported )
		{
			// Move asset
			var fileData = MoveAssetToPath(package, Path.GetDirectoryName(package.filePath), false, false, true);
			filesMoved = filesMoved + fileData.Key;
			filesInfo = filesInfo + fileData.Value;

			// Make sure list is up-to-date
			//RefreshPackages();
		}

		if (showResults && filesMoved > 0)
		{
			//Debug.Log(gyaAbbr + " - ( " + filesMoved.ToString() + " ) package(s) renamed.\n" + filesInfo);
			Debug.Log(gyaAbbr + " - Package renamed:\n" + filesInfo);
			//Debug.Log(gyaAbbr + " - ( " + filesMoved.ToString() + " ) package renamed.\n");
		}
	}

	// Rename selected to include version
	private void RenameWithVersionSelected ()
	{
		int filesMoved = 0;
		string filesInfo = String.Empty;

		// Check all old assets
		//		for (int i = 0; i < pkgData.Old.Count; ++i) {
		for (int i = 0; i < pkgData.Assets.Count; ++i)
		{
			// Is asset not in the old asset folder
			if ( pkgData.Assets[i].isMarked )
			{
				// Move asset
				var fileData = MoveAssetToPath(pkgData.Assets[i], Path.GetDirectoryName(pkgData.Assets[i].filePath), false, true, true);
				filesMoved = filesMoved + fileData.Key;
				filesInfo = filesInfo + fileData.Value;
			}
		}
		if (filesMoved > 0)
		{
			Debug.Log(gyaAbbr + " - ( " + filesMoved.ToString() + " ) package(s) renamed.\n" + filesInfo);
			// Make sure list is up-to-date
			RefreshPackages();
		}
	}

	// Rename AS assets to include version
	private void RenameWithVersionCollection ()
	{
		int filesMoved = 0;
		string filesInfo = String.Empty;

		// Check all old assets
		//		for (int i = 0; i < pkgData.Old.Count; ++i) {
		for (int i = 0; i < pkgData.Assets.Count; ++i)
		{
			// Is asset not in the old asset folder
			if ( pkgData.Assets[i].collection == svCollection.Store )
			{
				// Move asset
				var fileData = MoveAssetToPath(pkgData.Assets[i], Path.GetDirectoryName(pkgData.Assets[i].filePath), false, true);
				filesMoved = filesMoved + fileData.Key;
				filesInfo = filesInfo + fileData.Value;
			}
		}
		if (filesMoved > 0)
		{
			//Debug.Log(gyaAbbr + " - ( " + filesMoved.ToString() + " ) package(s) protected.\n" + filesInfo);
			Debug.Log(gyaAbbr + " - ( " + filesMoved.ToString() + " ) package(s) protected.\n");
		}
		// Make sure list is up-to-date
		//RefreshPackages();
	}

		// Move old assets to the prescribed folder
		private void OldAssetsMove (bool needsRefresh = true)
		{
			int filesMoved = 0;
			string filesInfo = String.Empty;

			// Check all old assets
			//		for (int i = 0; i < pkgData.Old.Count; ++i) {
			for (int i = 0; i < pkgData.Assets.Count; ++i)
			{
				// Is asset not in the old asset folder
				if ( pkgData.Assets[i].isOldToMove )
				{
					// Move asset
					var fileData = MoveAssetToPath(pkgData.Assets[i]);
					filesMoved = filesMoved + fileData.Key;
					filesInfo = filesInfo + fileData.Value;
				}
			}
			if (filesMoved > 0)
			{
				//pkgData.countOld += filesMoved;
				Debug.Log(gyaAbbr + " - ( " + filesMoved.ToString() + " ) package(s) moved to the Old Assets Folder.\n" + filesInfo);
			}
			// Make sure list is up-to-date
			if (needsRefresh)
			{
				RefreshPackages();
				//ScanStore();
				ScanOld();
			}
		}

		// Figure out the Old Assets of pkgData.Store including what is in the Old Folder
		private void OldAssetsListBuild ()
		{
			//Debug.Log (gyaAbbr + " -OldAssetsListBuild-\n");
			// Store results
			//List<Packages> packagesTemp = new List<Packages> (packagesList);
			List<Packages> packagesTemp = new List<Packages> (pkgData.Assets);

			try
			{
				packagesTemp.RemoveAll(x => x.isExported);
				packagesTemp.RemoveAll(x => x.collection == svCollection.Standard);
				packagesTemp.RemoveAll(x => x.collection == svCollection.User);
				packagesTemp.RemoveAll(x => x.collection == svCollection.Project);

				if (packagesTemp.Count() > 0)
				{

					// Sort by id then version_id, descending
					packagesTemp.Sort((x, y) =>
					{
						int compare = -x.id.CompareTo(y.id);
						if (compare != 0)
							return compare;

						compare = -x.version_id.CompareTo(y.version_id);
						if (compare != 0)
							return compare;

						return x.id.CompareTo(y.id);
					});

					int tmpPkgID = packagesTemp[0].id;
					int tmpPkgVID = packagesTemp[0].version_id;
					bool isPrimaryPkg = true;
					foreach (Packages package in packagesTemp)
					{
						//Debug.Log(package.id + "-" + tmpPkgID + " === " + package.version_id + "-" + tmpPkgVID);
						if (package.id == tmpPkgID)
						{
							isPrimaryPkg = false;
						}
						else
						{
							isPrimaryPkg = true;
						}

						// Testing only
						//if (package.id == 15398) {
						//	Debug.Log(package.id + "=" + tmpPkgID + " && " + package.version_id + "<" + tmpPkgVID + " isPrimary = " + isPrimaryPkg + " - " + package.filePath);
						//}

						// Check for primary package
						if (package.id == tmpPkgID && package.version_id < tmpPkgVID && package.collection == svCollection.Store && isPrimaryPkg == false)
						{
							package.collection = svCollection.Old;
							package.isOldToMove = true;
						}
						tmpPkgID = package.id;
						// Only update if this is the primary package for a given PkgID
						if (isPrimaryPkg)
							tmpPkgVID = package.version_id;
					}
				}
			}
			catch (Exception ex)
			{
				GUIOverride(OverrideReason.Error);
				Debug.LogError(gyaAbbr + " - Processing JSON Failed: " + ex.Message);
			}
			packagesTemp = null;
		}

		// Tally file size of dupes
		string CalcFolderSize (List<Packages> packageCalc)
		{
			double fileSizeTotal = 0;
			foreach (Packages package in packageCalc)
			{
				fileSizeTotal = fileSizeTotal + package.fileSize;
			}
			return GYAExt.BytesToKB(fileSizeTotal);
		}

		// Populate counts and size data
		void TallyAssets()
		{
			pkgData.countAll = pkgData.Assets.Count;
			pkgData.countStore = pkgData.Assets.FindAll( x => x.collection == svCollection.Store ).Count;
			pkgData.countUser = pkgData.Assets.FindAll( x => x.collection == svCollection.User ).Count;
			pkgData.countStandard = pkgData.Assets.FindAll( x => x.collection == svCollection.Standard ).Count;
			pkgData.countOld = pkgData.Assets.FindAll( x => x.collection == svCollection.Old ).Count;
			pkgData.countOldToMove = pkgData.Assets.FindAll( x => x.isOldToMove ).Count;
			pkgData.countProject = pkgData.Assets.FindAll( x => x.collection == svCollection.Project ).Count;

			pkgData.filesizeAll = pkgData.Assets.Sum (item => item.fileSize);
			pkgData.filesizeStore = pkgData.Assets.FindAll( x => x.collection == svCollection.Store ).Sum (item => item.fileSize);
			pkgData.filesizeUser = pkgData.Assets.FindAll( x => x.collection == svCollection.User ).Sum (item => item.fileSize);
			pkgData.filesizeStandard = pkgData.Assets.FindAll( x => x.collection == svCollection.Standard ).Sum (item => item.fileSize);
			pkgData.filesizeOld = pkgData.Assets.FindAll( x => x.collection == svCollection.Old ).Sum (item => item.fileSize);
		}

		// Count instances of package ID, ignore "Non Asset Store packages"
		int CountDuplicatesOfID (int id)
		{
			List<Packages> pkgResults = pkgData.Assets.FindAll( x => x.id == id && !x.isExported );
			return (pkgResults.Count);
		}

		// Count instances of package ID, ignore "Non Asset Store packages"
		int CountToImport ()
		{
			List<Packages> pkgResults = pkgData.Assets.FindAll( x => x.isMarked );
			return (pkgResults.Count);
		}

		// Import single asset
		private void ImportSingle (string pFilePath, bool pInteractive = false)
		{
			if (File.Exists (pFilePath))
			{
				Debug.Log (gyaAbbr + " - Import: " + pFilePath + "\n");

				// The Grab Yer Assets UI is being reset to defaults when this is run
				AssetDatabase.ImportPackage(pFilePath, pInteractive);
				AssetDatabase.Refresh();
			}
			else
			{
				Debug.Log (gyaAbbr + " - Import failed - Asset not found: " + pFilePath + "\n");
			}
		}

		// Loop thru and import packages marked for import
		private void ImportMultipleOLD (bool importEntireGroup = false)
		{
			// Import for Unity 4.2 and up
			int countToImport = 0;
			string listToImport = String.Empty;
			if (importEntireGroup)
			{
				countToImport = grpData[showGroup].Count();
				listToImport = gyaAbbr + " - Import ( " + countToImport.ToString() + " ) packages";
				foreach (Packages package in grpData[showGroup])
				{
					listToImport = listToImport + "\nImport: " + package.title + " ( " + package.version + " )";
					AssetDatabase.ImportPackage (package.filePath, false);
				}
			}
			else
			{
				countToImport = CountToImport();
				listToImport = gyaAbbr + " - Import ( " + countToImport.ToString() + " ) packages";
				foreach (Packages package in pkgData.Assets)
				{
					if (package.isMarked)
					{
						listToImport = listToImport + "\nImport: " + package.title + " ( " + package.version + " )";
						AssetDatabase.ImportPackage (package.filePath, false);
					}
				}
			}
			AssetDatabase.Refresh();
			Debug.Log (listToImport);
		}

		// Loop thru and import packages marked for import
		public void ImportMultiple (bool importEntireGroup = false)
		{
			// Import for Unity 4.2 and up
			List<string> importQueue = new List<string>();

			int countToImport = 0;
			string listToImport = String.Empty;

			if (importEntireGroup)
			{
				countToImport = grpData[showGroup].Count();
				//listToImport = gyaAbbr + " - Importing ( " + countToImport.ToString() + " ) packages";
				foreach (Packages package in grpData[showGroup])
				{
					listToImport += "\nImport: " + package.title + " ( " + package.version + " )";
					//#if UNITY_5_3_OR_NEWER
					importQueue.Add(package.filePath);
					//#else
					//AssetDatabase.ImportPackage (package.filePath, false);
					//#endif
				}
			}
			else
			{
				countToImport = CountToImport();
				//listToImport = gyaAbbr + " - Importing ( " + countToImport.ToString() + " ) packages";
				foreach (Packages package in pkgData.Assets)
				{
					if (package.isMarked)
					{
						listToImport += "\nImport: " + package.title + " ( " + package.version + " )";
						//#if UNITY_5_3_OR_NEWER
						importQueue.Add(package.filePath);
						//#else
						//AssetDatabase.ImportPackage (package.filePath, false);
						//#endif
					}
				}
			}

			// Override the import option if required
			MultiImportType _internalMultiImportOverride;

			// Handle Unity 5.4.0b13 and newer
			if ( GYAExt.UnityVersionIsEqualOrNewerThan("5.4.0b13") )
			{
				if ( userData.Settings.multiImportOverride == MultiImportType.Default )
					_internalMultiImportOverride = MultiImportType.UnitySync;
				else
					_internalMultiImportOverride = userData.Settings.multiImportOverride;
			}
			else
			{
				// Handle Unity 5.3.x - 5.4.0b12, force GYASync
				if ( GYAExt.UnityVersionIsEqualOrNewerThan("5.3.0", 3) )
				{
					_internalMultiImportOverride = MultiImportType.GYASync;
				}
				// Handle Unity 5.2.x and older
				else
				{
					// If pref is UnitySync, force UnityAsync for Unity 5.0.x - 5.2.x
					if ( userData.Settings.multiImportOverride == MultiImportType.Default || userData.Settings.multiImportOverride == MultiImportType.UnitySync)
						_internalMultiImportOverride = MultiImportType.UnityAsync;
					else
						_internalMultiImportOverride = userData.Settings.multiImportOverride;
				}
			}

			listToImport = gyaAbbr + " - Import ( " + countToImport.ToString() + " ) packages - (Method Selected: " + userData.Settings.multiImportOverride.ToString() + " / Method Used: " + _internalMultiImportOverride.ToString() + ")\n"
				+ listToImport +  "\n";

			// Unity Sync Import, Unity 5.4+ Only
			if (_internalMultiImportOverride == MultiImportType.UnitySync)
			{
				//#if UNITY_5_4_OR_NEWER
				foreach (string pFilePath in importQueue)
				{
					//AssetDatabase.ImportPackageImmediately(pFilePath);	// Complete each import before next
					GYAImport.ImportPackageImmediately(pFilePath);	// via Reflection
				}
				//#endif
				Debug.Log (listToImport);
			}

			// GYA Sync Import, Any Unity version, ONLY one that works for 5.3x - 5.4.0b12
			if (_internalMultiImportOverride == MultiImportType.GYASync)
			{
				GYACoroutine.start(GYAImport.ImportPackageQueue(importQueue, listToImport));
			}

			// Unity Async Import (Default), Any Unity version EXCEPT 5.3x - 5.4.0b12
			if (_internalMultiImportOverride == MultiImportType.UnityAsync)
			{
				foreach (string pFilePath in importQueue)
				{
					AssetDatabase.ImportPackage(pFilePath, false);			// Extract all, then start importing
				}
				Debug.Log (listToImport);
			}

		}

		// Verify JSON objects exist
		bool JSONObjectsAreNotNULL
		{
			get
			{
				if (pkgData.Assets == null)
				{
					Debug.Log(gyaAbbr + " - JSON Object Changed or Missing.  Refreshing the data file.\n");
					GUIOverride(OverrideReason.Error);
					return false;
				}

				GUIOverride(OverrideReason.None);
				return true;
			}
		}

		// Backup the "Grab Yer Assets User.json" file
		private void BackupUserData ()
		{
			//string backupExt = DateTime.Now.ToString ("MMddHHmmss");
			string backupExt = "bak";
			string jsonFileUserBackup = Path.ChangeExtension(jsonFileUser, backupExt);

			Debug.Log (gyaAbbr + " - Backing up:\t" + jsonFileUser + "\n\tTo:\t" + jsonFileUserBackup);

			// If data files exists load it, else create it
			if (Directory.Exists (GYAExt.PathGYADataFiles))
			{
				if (File.Exists (jsonFileUser))
				{
					File.SetAttributes(jsonFileUser, FileAttributes.Normal);
					if (File.Exists (jsonFileUserBackup))
						File.SetAttributes(jsonFileUserBackup, FileAttributes.Normal);

					File.Copy(jsonFileUser, jsonFileUserBackup, true);
				}
				else
				{
					Debug.LogWarning (gyaAbbr + " - User file not found: " + jsonFileUser + "\n");
				}
			}
		}

		// Restore the "Grab Yer Assets User.json" file
		private void RestoreUserData ()
		{
			//string backupExt = DateTime.Now.ToString ("MMddHHmmss");
			string backupExt = "bak";
			string jsonFileUserBackup = Path.ChangeExtension(jsonFileUser, backupExt);

			Debug.Log (gyaAbbr + " - Restoring: " + jsonFileUser + "\nFrom: " + jsonFileUserBackup);

			// If data files exists load it, else create it
			if (Directory.Exists (GYAExt.PathGYADataFiles))
			{
				if (File.Exists (jsonFileUserBackup))
				{
					File.SetAttributes(jsonFileUserBackup, FileAttributes.Normal);
					if (File.Exists (jsonFileUser))
						File.SetAttributes(jsonFileUser, FileAttributes.Normal);

					File.Copy(jsonFileUserBackup, jsonFileUser, true);
				}
				else
				{
					Debug.LogWarning (gyaAbbr + " - User backup file not found: " + jsonFileUserBackup + "\n");
				}
			}
		}

		// Scan Store assets folder for unitypackages
		internal void ScanStore ()
		{

			string jsonText = "";

			// Unity 3 & 4 Asset Store
			if ( (userData.Settings.scanAllAssetStoreFolders) || (!userData.Settings.scanAllAssetStoreFolders && (GYAExt.PathUnityAssetStoreActive == GYAExt.PathUnityAssetStore)) )
			{
				//Debug.Log("Pre-Scanning AS Folder");
				if (Directory.Exists(GYAExt.PathUnityAssetStore))
				{
					//jsonText += GetPackageInfoFromFolder (svCollection.Store, GYAExt.PathUnityAssetStore, false);
					jsonText = GetPackageInfoFromFolder (svCollection.Store, GYAExt.PathUnityAssetStore);
					if (jsonText.Length > 0)
						pkgInfoText += jsonText + ",";
				}
			}

			// Unity 5 Asset Store
			if ( (userData.Settings.scanAllAssetStoreFolders) || (!userData.Settings.scanAllAssetStoreFolders && (GYAExt.PathUnityAssetStoreActive == GYAExt.PathUnityAssetStore5)) )
			{
				//Debug.Log("Pre-Scanning AS5 Folder");
				if (Directory.Exists(GYAExt.PathUnityAssetStore5))
				{
					//jsonText += GetPackageInfoFromFolder (svCollection.Store, GYAExt.PathUnityAssetStore5, false);
					jsonText = GetPackageInfoFromFolder (svCollection.Store, GYAExt.PathUnityAssetStore5);
					if (jsonText.Length > 0)
						pkgInfoText += jsonText + ",";
				}
			}

			if (jsonText.Length > 0)
				jsonText = pkgInfoText.Substring(0,jsonText.Length-1);

			jsonText = "{\"version\":\"" + gyaVersion + "\",\"Assets\":[" + jsonText + "]}";

			OldAssetsListBuild ();
			BuildPrevNextList();
		}

		// Scan Store assets folder for unitypackages
		internal void ScanPersist ()
		{
			if (Directory.Exists(GYAExt.PathUnityAssetStoreActive))
			{
				//pkgData.Assets.RemoveAll(x => x.collection == svCollection.Store);

				string jsonText = GetPackageInfoFromFolder (svCollection.Store, GYAExt.PathUnityAssetStoreActive, "gyaUpdate");
				//Debug.Log (gyaAbbr + " - jsonText: " + jsonText + "\n");
				if (jsonText.Length != 0)
				{
					jsonText = "{\"Assets\":[" + jsonText + "]}";
					//Debug.Log("ScanProject (" + GYAExt.PathUnityProjectAssets + ")\n" + jsonText);

					RootPackages scanData = new RootPackages();
					scanData = JsonConvert.DeserializeObject<RootPackages>(jsonText);
					//IEnumerable<Packages> scanIE = from package in scanData.Assets select package;
					//pkgData.Assets.AddRange( scanIE );

					//OldAssetsListBuild ();
					Debug.Log(scanData.Assets.Count);
				}
			}
		}

		// Scan standard assets folder for unitypackages
		internal void ScanStandard ()
		{
			if (Directory.Exists(GYAExt.PathUnityStandardAssets))
			{
				pkgData.Assets.RemoveAll(x => x.collection == svCollection.Standard);

				string jsonText = GetPackageInfoFromFolder (svCollection.Standard, GYAExt.PathUnityStandardAssets);
				//Debug.Log (gyaAbbr + " - jsonText: " + jsonText + "\n");
				if (jsonText.Length != 0)
				{
					jsonText = "{\"Assets\":[" + jsonText + "]}";
					//Debug.Log("ScanStandard (" + GYAExt.PathUnityStandardAssets + ")\n" + jsonText);

					RootPackages scanData = new RootPackages();
					//List<Packages> projectData = new List<Packages>();
					scanData = JsonConvert.DeserializeObject<RootPackages>(jsonText);
					IEnumerable<Packages> scanIE = from package in scanData.Assets select package;
					//List<Packages> resultsOld = resultsDupes.ToList();
					pkgData.Assets.AddRange( scanIE );
					//Debug.Log( pkgData.Assets.FindAll( x => x.collection == svCollection.Project ).Count + "\n");

					OldAssetsListBuild ();
					PackagesSortBy(sortActive);
				}
			}
		}

		// Scan project folder for unitypackages
		internal void ScanProject ()
		{
			if (Directory.Exists(GYAExt.PathUnityProjectAssets))
			{
				pkgData.Assets.RemoveAll(x => x.collection == svCollection.Project);

				string jsonText = GetPackageInfoFromFolder (svCollection.Project, GYAExt.PathUnityProjectAssets);
				//Debug.Log (gyaAbbr + " - jsonText: " + jsonText + "\n");
				if (jsonText.Length != 0)
				{
					jsonText = "{\"Assets\":[" + jsonText + "]}";
					//Debug.Log("ScanProject (" + GYAExt.PathUnityProjectAssets + ")\n" + jsonText);

					RootPackages scanData = new RootPackages();
					//List<Packages> projectData = new List<Packages>();
					scanData = JsonConvert.DeserializeObject<RootPackages>(jsonText);
					IEnumerable<Packages> projectIE = from package in scanData.Assets select package;
					//List<Packages> resultsOld = resultsDupes.ToList();
					pkgData.Assets.AddRange( projectIE );
					//Debug.Log( pkgData.Assets.FindAll( x => x.collection == svCollection.Project ).Count + "\n");

					PackagesSortBy(sortActive);
				}
			}
		}

		// Scan old folder for unitypackages
		internal void ScanOld ()
		{
			if (Directory.Exists(GYAExt.PathUnityAssetStore))
			{
				pkgData.Assets.RemoveAll(x => x.collection == svCollection.Old);

				string jsonText = GetPackageInfoFromFolder (svCollection.Old, pathOldAssetsFolder);
				//Debug.Log (gyaAbbr + " - jsonText: " + jsonText + "\n");
				if (jsonText.Length != 0)
				{
					jsonText = "{\"Assets\":[" + jsonText + "]}";
					//Debug.Log("ScanOld (" + pathOldAssetsFolder + ")\n" + jsonText);

					RootPackages projectData = new RootPackages();
					//List<Packages> projectData = new List<Packages>();
					projectData = JsonConvert.DeserializeObject<RootPackages>(jsonText);
					IEnumerable<Packages> projectIE = from package in projectData.Assets select package;
					//List<Packages> resultsOld = resultsDupes.ToList();
					pkgData.Assets.AddRange( projectIE );
					//Debug.Log( pkgData.Assets.FindAll( x => x.collection == svCollection.Project ).Count + "\n");

					OldAssetsListBuild ();
					PackagesSortBy(sortActive);
				}
			}
		}

		private void RefreshPackages (bool showRefresh = false)
		{

			StopwatchStart();

			if (userData.Settings.enableOfflineMode)
			{
				// Offline mode enabled, do not refresh package list
				Debug.Log(gyaAbbr + " - Offline Mode is currently ENABLED.  Exisitng data file used.\n");
				LoadJSON ();
				BuildPrevNextList();
				return;
			}

			//string tmpText = "";
			countPackageErrors = 0;
			//ScanStore();				// TESTING
			PreProcessRefresh();		// Enable for Auto Consolidation, Prevent AS Overwrite
			//RefreshPackagesASOnly();	// OLD - Enable for Auto Consolidation, Prevent AS Overwrite
			pkgInfoText = "";

			// Process Asset Store Folder
			if ( (userData.Settings.scanAllAssetStoreFolders) || (!userData.Settings.scanAllAssetStoreFolders && (GYAExt.PathUnityAssetStoreActive == GYAExt.PathUnityAssetStore)) )
			{
				pkgInfoText += RefreshProcessCollection(svCollection.Store, GYAExt.PathUnityAssetStore);
			}
			// Process Asset Store Folder (Unity 5)
			if ( (userData.Settings.scanAllAssetStoreFolders) || (!userData.Settings.scanAllAssetStoreFolders && (GYAExt.PathUnityAssetStoreActive == GYAExt.PathUnityAssetStore5)) )
			{
				pkgInfoText += RefreshProcessCollection(svCollection.Store, GYAExt.PathUnityAssetStore5);
			}
			// Process Standard Assets
			pkgInfoText += RefreshProcessCollection(svCollection.Standard, GYAExt.PathUnityStandardAssets);
			// Process Old Folder
			pkgInfoText += RefreshProcessCollection(svCollection.Old, pathOldAssetsFolder);
			// Process User Folders
			pkgInfoText += RefreshProcessCollection(svCollection.User, userData.Settings.pathUserAssets);
			// Process pathUserAssetsList
			for (int i = 0; i < userData.Settings.pathUserAssetsList.Count; i++)
				pkgInfoText += RefreshProcessCollection(svCollection.User, userData.Settings.pathUserAssetsList[i]);

			// Stop processing Refresh if error detected in gathering package info
			if (guiOverride)
				return;

			// Remove trailing comma
			if (pkgInfoText.Length > 0)
				pkgInfoText = pkgInfoText.Substring(0,pkgInfoText.Length-1);

			// Complete the JSON data
			pkgInfoText = "{\"version\":\"" + gyaVersion + "\",\"Assets\":[" + pkgInfoText + "]}";
			// What was the last scanned AS folder(s): "Asset Store", "Asset Store-5.x" or both
			//string scannedFolders = "";
			//if (userData.Settings.scanAllAssetStoreFolders) {
			//	//scannedFolders = GYAExt.FolderUnityAssetStore + "," + GYAExt.FolderUnityAssetStore5;
			//	scannedFolders = "ALL";
			//} else {
			//	scannedFolders = GYAExt.FolderUnityAssetStoreActive;
			//}
			//pkgInfoText = "{\"version\":\"" + gyaVersion + "\",\"scannedFolders\":\"" + scannedFolders + "\",\"Assets\":[" + pkgInfoText + "]}";
			//Debug.Log("RP5: " + pkgInfoText);

			SaveJSON (pkgInfoText);
			LoadJSON ();

			// Set isInASFolder/isInAS5Folder
			//pkgData.Assets.FindAll(x => x.filePath == curAsset.filePath ).ForEach(x => x.isInAS5Folder = true);


			// Testing scan for project unitypackges
			ScanProject();
			//ScanStandard();

			BuildPrevNextList();

			// Refresh topbar count
			//PackagesSortBy(sortActive);
			SVShowCollection(showActive);

			// Stopwatch End
			string swEnd = StopwatchElapsed(false);

			// Don't show this if Persist returns false, otherwise it's shown twice
			if (showRefresh)
			{
				//PersistEnable();
				string debugString = gyaAbbr + " - Packages Refreshed: All (" + pkgData.countAll + "), Store (" + pkgData.countStore + "), User (" + pkgData.countUser + "), " + GYAExt.FolderUnityStandardAssets + " (" + pkgData.countStandard + "), Old Versions (" + pkgData.countOld + "), Old To Consolidate (" + pkgData.countOldToMove + "), Project (" + pkgData.countProject + ")\n";

				debugString += "Scanned in: " + swEnd;

				if (countPackageErrors > 0)
				{
					debugString += " - Package Errors Detected: ( " + countPackageErrors + " )";
					//if (!userData.Settings.reportPackageErrors) {
					//	debugString += " .. Extended reporting can be enabled in the menu. ";
					//} else {
					//	debugString += " .. Extended reporting can be disabled in the menu. ";
					//}
				}

				Debug.Log(debugString);
			}
		}

		private string RefreshProcessCollection (svCollection pCollection, string pPath)
		{
			string tmpText = "";
			if (Directory.Exists(pPath))
			{
				tmpText = GetPackageInfoFromFolder (pCollection, pPath);
				if (guiOverride)
					return tmpText;
				if (tmpText.Length > 0)
					tmpText += ",";
			}
			return tmpText;
		}

		private void RefreshPackagesASOnly ()
		{

			countPackageErrors = 0;
			pkgInfoText = "";

			// Process Asset Store Folder
			if ( (userData.Settings.scanAllAssetStoreFolders) || (!userData.Settings.scanAllAssetStoreFolders && (GYAExt.PathUnityAssetStoreActive == GYAExt.PathUnityAssetStore)) )
			{
				pkgInfoText += RefreshProcessCollection(svCollection.Store, GYAExt.PathUnityAssetStore);
			}
			// Process Asset Store Folder (Unity 5)
			if ( (userData.Settings.scanAllAssetStoreFolders) || (!userData.Settings.scanAllAssetStoreFolders && (GYAExt.PathUnityAssetStoreActive == GYAExt.PathUnityAssetStore5)) )
			{
				pkgInfoText += RefreshProcessCollection(svCollection.Store, GYAExt.PathUnityAssetStore5);
			}

			// Stop processing Refresh if error detected in gathering package info
			if (guiOverride)
				return;

			// Remove trailing comma
			if (pkgInfoText.Length > 0)
				pkgInfoText = pkgInfoText.Substring(0,pkgInfoText.Length-1);

			pkgInfoText = "{\"version\":\"" + gyaVersion + "\",\"Assets\":[" + pkgInfoText + "]}";

			//SaveJSON (pkgInfoText);
			//LoadJSON ();

			// Replaced above 2 lines with:
			pkgData = JsonConvert.DeserializeObject<RootPackages>(pkgInfoText);

			// Testing scan for project unitypackges
			//ScanProject();

			OldAssetsListBuild ();
			BuildPrevNextList();

			// Process Auto Consolidation Move/Delete if required
			//AutoConsolidate();

			// Refresh topbar count
			//PackagesSortBy(sortActive);
			//SVShowCollection(showActive);
			countPackageErrors = 0;
		}

		// Called from: RefreshPackages
		// Process Auto Consolidation Move/Delete if required
		private void PreProcessRefresh ()
		{

			// DISABLED - Rename AS assets
			if (userData.Settings.autoPreventASOverwrite && pkgData.countStore > 0)
			{
				RenameWithVersionCollection();
				//RefreshPackages();
				RefreshPackagesASOnly();
			}

			// Auto Consolidate
			if (userData.Settings.autoConsolidate && pkgData.countOldToMove > 0)
			{
				OldAssetsMove(false);
				//RefreshPackages();
				RefreshPackagesASOnly();
			}

			// DISABLED - Auto Delete Consolidated
			//Debug.Log(pkgData.countOld - pkgData.countOldToMove);
			if (userData.Settings.autoDeleteConsolidated && (pkgData.countOld - pkgData.countOldToMove) > 0)
			{
				OldAssetsDeleteAll(false);
				//RefreshPackages();
			}
		}

		// Load the user data file: Grab Yer Assets Info.json
		// Settings, Groups, Favorites, Comments, etc
		private void LoadUser ()
		{

			// Change User JSON file if required
			//UpdateSchema (jsonFileUser, "\"showHeaders\":", "\"enableHeaders\":");
			//UpdateSchema (jsonFileUser, "\"isAssetStorePkg\":", "\"isExported\":");

			if (File.Exists (jsonFileUser))
			{
				try
				{
					File.SetAttributes(jsonFileUser, FileAttributes.Normal);
					string jsonText = File.ReadAllText (jsonFileUser);

					userData = JsonConvert.DeserializeObject<RootUser>(jsonText);

					// Make any last minute changes to User data prior to display
					//Debug.Log ("userData" + userData.Group[0].Assets[0].isExported + "\n");

					// Extra check to fix isExported value getting inverted if v1
					// is ever run again after having already upgraded to v2
					for (int i = 0; i < userData.Group.Count; ++i)
					{
						for (int j = 0; j < userData.Group[i].Assets.Count; ++j)
						{
						//foreach (RootUser package in userData.Group[i]) {
							// If isExported is true BUT it has a valid id, then fix isExported
							if (userData.Group[i].Assets[j].isExported && userData.Group[i].Assets[j].id > 0)
							{
								userData.Group[i].Assets[j].isExported = false;
							}
							// If isExported is false BUT it does NOT have a valid id, then fix isExported
							if (!userData.Group[i].Assets[j].isExported && userData.Group[i].Assets[j].id == 0)
							{
								userData.Group[i].Assets[j].isExported = true;
							}
							//Debug.Log ("userData: " + userData.Group[i].name + " - " + userData.Group[i].Assets[j].isExported + " - " + userData.Group[i].Assets[j].id + " - " + 						userData.Group[i].Assets[j].title + "\n");
						}
					}

				}
				catch (Exception ex)
				{
					Debug.LogWarning(gyaAbbr + " - LoadUser: " + ex.Message);
					GUIOverride(OverrideReason.Error);
				}
			}
			else
			{
				// If file doesn't exist, create default Favorites group
				Debug.Log(gyaAbbr + " - Creating User File: " + jsonFileUser + "\n");

				// If there are no groups, add Favorites as the 1st and default group
				if (userData.Group.Count() == 0)
				{
					// Create new group with name only
					GroupCreate("Favorites");
				}
			}
		}

		// Create or over-write the json file for user info
		private void SaveUser (bool append = false)
		{
			// Make any last minute changes to User data prior to saving
			userData.version = gyaVersion;

			// Let the saving commence
			TextWriter writer = null;
			RootUser objectToWrite = userData;

			try
			{
				if (File.Exists (jsonFileUser))
					File.SetAttributes(jsonFileUser, FileAttributes.Normal);

				var contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite, Formatting.Indented);
				//var contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite, Formatting.Indented, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
				writer = new StreamWriter(jsonFileUser, append);
				writer.Write(contentsToWriteToFile);
			}
			catch (Exception ex)
			{
				Debug.LogWarning(gyaAbbr + " - SaveUser Error: " + ex.Message);
				GUIOverride(OverrideReason.Error);
			}
			finally
			{
				if (writer != null)
					writer.Close();
			}
		}

		// Return the true asset name of an asset IF the title is blank, often the filename is 'unknown'
		private string GetAssetNameFromOldIDIfExist(int assetID, int assetVersionID = 0)
		{
			List<Packages> pkgResults;
			string assetName = "unknown"; //String.Empty;

			pkgResults = pkgData.Assets.FindAll( x => x.id == assetID );
			pkgResults.Sort((x,y) => -x.version_id.CompareTo(y.version_id));

			if ( pkgResults.Count() > 0 )
			{
				foreach(Packages package in pkgResults)
				{
					//Debug.Log("GetName Title 1: " + package.title);
					if (!package.title.StartsWith("unknown"))
					{
						assetName = package.title;
						//Debug.Log("GetName Title 2: " + assetName);
						break;
					}
					else if (package.version_id == assetVersionID)
					{
						assetName = Path.GetFileNameWithoutExtension(package.filePath);
					}
				}
			}
			else
			{
				assetName = pkgData.Assets[0].title;
				//Debug.Log("GetName Title 3: " + assetName);
			}

			//if (assetVersionID != 0) {
			//	assetName = pkgData.Assets.FindAll( x => x.id == assetID && x.version_id == assetVersionID )
			//}

			//Debug.Log(assetName);
			return assetName;
		}

		// Post-proc json
		private void LoadJSONPostProcess()
		{
			string newTitle = "";

			pkgData.Assets.Sort((x,y) => -x.version_id.CompareTo(y.version_id));

			for (int i = 0; i < pkgData.Assets.Count; ++i)
			{
				// Check for '/' in the asset title
				//if (pkgData.Assets[i].title.Contains("/")) {
				//	//Debug.Log(pkgData.Assets[i].title);
				//	pkgData.Assets[i].title = pkgData.Assets[i].title.Replace('/','-');
				//}
				// Check for damaged asset
				if (pkgData.Assets[i].isDamaged)
				{
					if (pkgData.Assets[i].title.StartsWith("unknown"))
					{
						newTitle = GetAssetNameFromOldIDIfExist(pkgData.Assets[i].id, pkgData.Assets[i].version_id);
						pkgData.Assets[i].title = newTitle;
						//Debug.Log("NewTitle: " + newTitle + " - " + pkgData.Assets[i].version);
					}
				}
			}
		}

		// Load the package data from file: Grab Yer Assets.json
		//private void LoadJSON (bool bypassVerCheck = false) {
		private void LoadJSON ()
		{
			if (File.Exists (jsonFilePackages))
				File.SetAttributes(jsonFilePackages, FileAttributes.Normal);

			string jsonText = File.ReadAllText (jsonFilePackages);
			// Catch malformed json
			try
			{
				pkgData = JsonConvert.DeserializeObject<RootPackages>(jsonText);
				// verify JSON
				if (JSONObjectsAreNotNULL)
				{
					// File version check to make sure that old data isn't used in case of layout changes
					//string fileVersion = pkgData.version;
					int verCompare = pkgData.version.CompareTo(gyaVersion);
					//if (fileVersion != gyaVersion && !bypassVerCheck) {
					if (userData.Settings.enableOfflineMode)
					{
						// Offline mode enabled, do not refresh package list
						Debug.Log(gyaAbbr + " - Offline Mode is currently ENABLED.  Using exisitng data file.\n");
					}
					else if (verCompare < 0)
					{
						// If pkgData.version doesn't match the current assetVersion, refresh package data
						Debug.Log(gyaAbbr + " - " + Path.GetFileName(jsonFilePackages) + " is out of date - Refreshing data file.\nVersions: (JSON = " + pkgData.version + ") ( GYA = " + gyaVersion + ")");
						RefreshPackages();
						return;
					}

					LoadJSONPostProcess();

					// Calculate the old assets
					OldAssetsListBuild ();
					//}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(gyaAbbr + " - LoadJSON: " + ex.Message);
				GUIOverride(OverrideReason.Error);
			}
		}

		// Create or over-write the json file for package info
		private void SaveJSON (string jsonToWrite)
		{
			bool append = false;
			TextWriter writer = null;

			var parsedJSON = JsonConvert.DeserializeObject(jsonToWrite);
			try
			{
				if (File.Exists (jsonFilePackages))
					File.SetAttributes(jsonFilePackages, FileAttributes.Normal);

				var contentsToWriteToFile = JsonConvert.SerializeObject(parsedJSON, Formatting.Indented);
				//var contentsToWriteToFile = JsonConvert.SerializeObject(parsedJSON, Formatting.Indented, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
				writer = new StreamWriter(jsonFilePackages, append);
				writer.Write(contentsToWriteToFile);
			}
			catch (Exception ex)
			{
				Debug.LogWarning(gyaAbbr + " - SaveJSON Error: " + ex.Message);
				GUIOverride(OverrideReason.Error);
			}
			finally
			{
				if (writer != null)
					writer.Close();
			}
		}

		// Disabled in 2.15j28c
		private void UpdateSchema (string jsonFile, string oldName, string newName)
		{
			//Debug.Log (gyaAbbr + " -UpdateSchema-\n");
			if (File.Exists (jsonFile))
			{
				string jsonText = File.ReadAllText (jsonFile);

				// update class structure if not already updated
				if ( jsonText.Contains(oldName, StringComparison.OrdinalIgnoreCase) )
				{
					BackupUserData();
					Debug.Log (gyaAbbr + " - Updating Schema: " + jsonFileUser + "\n");

					jsonText = jsonText.Replace(oldName + " true", newName + " false");
					jsonText = jsonText.Replace(oldName + " false", newName + " true");

					// Deserialize Json
					try
					{
						userData = JsonConvert.DeserializeObject<RootUser>(jsonText);
					}
					catch (Exception ex)
					{
						Debug.LogWarning(gyaAbbr + " - UpdateSchema: " + ex.Message);
						GUIOverride(OverrideReason.Error);
					}

					//Write updated file
					SaveUser ();
				}
			}
		}

		// Not Used
		private void CheckUserSettings()
		{
			// Do any version specific handling prior to updating the version in the Users file
			userData.version = gyaVersion;
			userData.Settings = new Settings
			{
				scanAllAssetStoreFolders = true,
				showAllAssetStoreFolders = true,
				isPersist = false,
				pathUserAssets = String.Empty,
				pathUserAssetsList = new List<string>(),
				enableHeaders = true,
				enableColors = true,
				showSVInfo = true,
				reportPackageErrors = false,
				nestedDropDowns = true,
				nestedVersions = false,
				enableCollectionTypeIcons = true,
				autoPreventASOverwrite = false,
				autoConsolidate = false,
				autoDeleteConsolidated = false,
				enableOfflineMode = false,
				openURLInUnity = true,
				showProgressBarDuringRefresh = true,
				showProgressBarDuringFileAction = true,
				multiImportOverride = MultiImportType.Default

			};
		}

		// Get info from unitypackage
		//private string GetPackageInfoFromFile (FileInfo fileData, svCollection collection, bool processErrors = true) {
		private string GetPackageInfoFromFile (FileInfo fileData, svCollection collection)
		{
			//Debug.Log (gyaAbbr + " -GetPackageInfoFromFile- " + collection + " - " + fileData.Name + "\n");
			bool processErrors = true;
			string infoJSON = "";
			// Path & File Name
			string fileFullName = fileData.FullName;
			fileFullName = fileFullName.Replace('\\', '/');
			string fileAssetName = Path.GetFileNameWithoutExtension(fileFullName);
			bool hasValidHeader = false;
			bool isDamaged = false;
			DateTimeOffset mTimeStamp = default(DateTimeOffset);

			// If file exists, process it
			if (File.Exists (fileFullName))
			{

				/*
				* Header info for unitypackage files
				* 1st 4 bytes: 			1F8B08xx, 1F8B0804 = Asset Store, 1F8B0800 = Exported
				* - 1-3 bytes: uID		ID, Fixed value 1F8B = GZip
				* - 3rd byte : uCM		Compression Method, usually 08 (deflate)
				* - 4th byte : uFlag	04 = Extra Field Data (Asset Store), 00 = None (Exported)
				* 2nd 4 bytes: uMTime	Modification Time
				* 3rd 4 bytes: 			0x03xxxx = Marker + Data Length
				* - 1st byte : uXFlag	Extra Flag set by CM
				* - 2nd byte : uOS		OS = Usually 03
				*                       -------	End of common bytes between Asset Store / Exported
				* - 3-4 bytes: lenData	Asset Store: xxxx = Data Length 4 bytes longer then Info Length
				*                       Exported Package: ECBD
				* 4th 4 bytes: 			4124xxxx = Marker + JSON string length
				* - 1-2 bytes: uSubID	Subfield ID, Fixed 4124
				* - 3-4 bytes: lenJSON	xxxx = JSON string length
				* End bytes  : uEnd		ECBD = Found at end of JSON Info / Start of Asset marker

				Updated info: (GZip Header)
				01:	1F	ID1 Fixed value
				02:	8B	ID2 Fixed value
				03:	08	CM Compression method, 08 = deflate
				04: xx	FLG Flag, 04 = FEXTRA

				5-8: xxxxxxxx	MTIME Modification Time (UNIX) (IGNORE)

				09:	xx	XFL Extra Flags set by CM (IGNORE)
				10:	xx	OS (IGNORE)
				11-12:	xxxx	If FLG=04: XLEN, Extra field length
				If FLG=00: (IGNORE)
				If FLG=08: FLG.NAME, 00 terminated (IGNORE)

				13-14:	4124	Subfield ID
				15-16:	xxxx	Subfield length
				*/

				// Make sure the file is at least long enough to test for data
				if (fileData.Length >= 32)
				{
					mTimeStamp = new DateTimeOffset(fileData.CreationTimeUtc);
					try
					{
						// Finds the length of the Info contained in the package and populates a string
						using (FileStream fs = new FileStream(fileFullName, FileMode.Open, FileAccess.Read))
						{
							using (BinaryReader br = new BinaryReader(fs, Encoding.UTF8))
							{
								byte[] headerBytes;
								headerBytes = br.ReadBytes(16);
								string headerString = BitConverter.ToString(headerBytes, 0, 16);
								headerString = headerString.Replace("-", ""); // Remove "-" dividers
								string uID = headerString.Substring(0,4);
								//string uCM = headerString.Substring(4,2);
								string uFlag = headerString.Substring(6,2);
								//string uMTime = headerString.Substring(8,8);
								//string uXFlag = headerString.Substring(16,2);
								//string uOS = headerString.Substring(18,2);
								string hexData = headerString.Substring(22,2) + headerString.Substring(20,2);	// Length of Data
								int lenData = int.Parse(hexData, System.Globalization.NumberStyles.HexNumber);	// Length of Data (Converted)
								string uSubID = headerString.Substring(24,4);
								string hexJSON = headerString.Substring(30,2) + headerString.Substring(28,2);	// Length of JSON String
								int lenJSON = int.Parse(hexJSON, System.Globalization.NumberStyles.HexNumber);	// Length of JSON String (Converted)

								// Change collection to Old if asset is in pathOldAssetsFolder
								if ( fileFullName.Replace('/', '\\').Contains(pathOldAssetsFolder.Replace('/', '\\'), StringComparison.OrdinalIgnoreCase) )
								{
									collection = svCollection.Old;
								}

								// Check if .unitypackage is valid to process
								if (uID == "1F8B" )
								{ // Valid GZip header
									mTimeStamp = DateTimeOffset.Parse("1970-01-01Z").AddSeconds(headerBytes[4] + (headerBytes[5] << 8) + (headerBytes[6] << 16) + (headerBytes[7] << 24));
									if ( uFlag == "04" && uSubID == "4124" )
									{
									// Process Asset Store package
										if ( lenData == (lenJSON + 4) )
										{ // Validate json size
											infoJSON = new string (br.ReadChars (lenJSON)); // <- Info String

											// Begin Validation Check - infoJSON
											try
											{
												#pragma warning disable 0168
												// Convert raw text to json
												var checkIfValid = JsonConvert.DeserializeObject<Packages>(infoJSON);
												#pragma warning restore 0168

												// Remove last char '}' for adding following elements
												infoJSON = infoJSON.Remove(infoJSON.Length - 1);

												// Fix missing/damaged field(s), ie- missing title

												// Blank title
												if ( infoJSON.Contains("\"title\":\"\"", StringComparison.OrdinalIgnoreCase) )
												{
													isDamaged = true;
													string infoJSONRaw = infoJSON;
													// Add missing title
													string newTitle = fileAssetName;

													//newTitle = GetAssetNameFromOldIDIfExist(tmpJSON.id);
													//Debug.Log("NewTitle: " + newTitle + " - " + tmpJSON.version);

													infoJSON = infoJSON.Replace(
														",\"title\":\"\"",
														",\"title\":\"" + newTitle + "\""
													);
													if (processErrors)
														countPackageErrors += 1;

													if (processErrors && userData.Settings.reportPackageErrors)
													{
														Debug.LogWarning (gyaAbbr + " - Blank package title: " + fileFullName + "\nHeader: (" + headerString + ")" + "\nJSON: (" + infoJSONRaw + ")");
													}
													//hasValidHeader = false;
												}
												// Missing title
												if ( !infoJSON.Contains("\"title\":\"", StringComparison.OrdinalIgnoreCase) )
												{
													isDamaged = true;
													// Add missing title
													infoJSON += ",\"title\":\"" + fileAssetName + "\"";
													if (processErrors)
														countPackageErrors += 1;

													if (processErrors && userData.Settings.reportPackageErrors)
													{
														//Debug.LogWarning (gyaAbbr + " - Fixing missing title: " + fileAssetName + fixedAbbr + "\nHeader: (" + headerString + ")" + "\nJSON: (" + infoJSON + ")");
														Debug.LogWarning (gyaAbbr + " - Missing package title: " + fileFullName + "\nHeader: (" + headerString + ")" + "\nJSON: (" + infoJSON + ")");
													}
													//hasValidHeader = false;
												}
												// Missing unity_version
												if ( !infoJSON.Contains("\"unity_version\":\"", StringComparison.OrdinalIgnoreCase) )
												{
													// Add missing title
													infoJSON += ",\"unity_version\":\"\"";
												}
												// Missing unity_version
												if ( !infoJSON.Contains("\"upload_id\":\"", StringComparison.OrdinalIgnoreCase) )
												{
													// Add missing title
													//infoJSON += ",\"upload_id\":\"\"";
													infoJSON += ",\"upload_id\":\"0\"";
												}

												infoJSON += ",\"filePath\":\"" + fileFullName + "\"";
												infoJSON += ",\"fileSize\":\"" + fileData.Length + "\"";
												infoJSON += ",\"fileDataCreated\":\"" + mTimeStamp + "\"";
												infoJSON += ",\"fileDateCreated\":\"" + fileData.CreationTime + "\"";
												//infoJSON += ",\"fileDateModified\":\"" + fileData.LastWriteTime + "\"";
												//infoJSON += ",\"fileDateLastOpen\":\"" + fileData.LastAccessTime + "\"";
												infoJSON += ",\"isExported\":\"" + false + "\"";
												infoJSON += ",\"isDamaged\":\"" + isDamaged + "\"";
												//infoJSON += ",\"collection\":\"" + collection + "\"}";
												infoJSON += ",\"collection\":\"" + collection + "\"";
												//checkIfValid = null;
												hasValidHeader = true; // Header is valid!
											}
											catch (Exception)
											{
												isDamaged = true;
												if (processErrors)
													countPackageErrors += 1;

												if (processErrors && userData.Settings.reportPackageErrors)
													Debug.LogWarning(gyaAbbr + " - Damaged UnityPackage (JSON Corrupt): " + fileFullName + "\nHeader: (" + headerString + ")" + "\nJSON: (" + infoJSON + ")");
											}
											// End Validation check
										}
										else
										{
											isDamaged = true;
											if (processErrors)
												countPackageErrors += 1;

											if (processErrors && userData.Settings.reportPackageErrors)
												Debug.LogWarning(gyaAbbr + " - Damaged UnityPackage (JSON Length Mismatch): " + fileFullName + "\nHeader: (" + headerString + ")");
										}
									}
								}
								else
								{
									isDamaged = true;
									// Handle invalid files
									if (processErrors)
										countPackageErrors += 1;

									if (processErrors && userData.Settings.reportPackageErrors)
										Debug.LogWarning(gyaAbbr + " - Damaged UnityPackage (Missing GZip Header):" + fileFullName + "\nHeader: (" + headerString + ")");
									//return "";	// File invalid, process failed
								} // End extracting Asset Store JSON data from file

							}
						}
					}
					catch (Exception ex)
					{
						isDamaged = true;
						if (processErrors)
							countPackageErrors += 1;

						Debug.LogWarning(gyaAbbr + " - Damaged UnityPackage (Exception): " + fileFullName + "\nHeader: (" + infoJSON + ")\n" + ex);
					}
				}
				else
				{
					isDamaged = true;
					if (processErrors)
						countPackageErrors += 1;

					if (processErrors && userData.Settings.reportPackageErrors)
						Debug.LogWarning(gyaAbbr + " - Damaged UnityPackage (File Length Too Short): " + fileFullName + "\nFile Size in Bytes: " + fileData.Length.ToString());
				}
			}
			else
			{
				isDamaged = true;
				if (processErrors)
					countPackageErrors += 1;

				Debug.LogWarning(gyaAbbr + " - Missing UnityPackage: " + fileFullName + "\n");
			}

			// Set isInASFolder/isInAS5Folder bools
			bool isInASFolder = false;
			bool isInAS5Folder = false;
			string isInNativeASFolder = "";
			if (GYAExt.PathFixedForOS(fileFullName).Contains(GYAExt.PathFixedForOS(GYAExt.PathUnityAssetStore + "/"), StringComparison.OrdinalIgnoreCase))
			{
				isInASFolder = true;
				isInNativeASFolder = GYAExt.FolderUnityAssetStore;
			}
			if (GYAExt.PathFixedForOS(fileFullName).Contains(GYAExt.PathFixedForOS(GYAExt.PathUnityAssetStore5 + "/"), StringComparison.OrdinalIgnoreCase))
			{
				isInAS5Folder = true;
				isInNativeASFolder = GYAExt.FolderUnityAssetStore5;
			}

			if (hasValidHeader)
			{
				infoJSON += ",\"isInASFolder\":\"" + isInASFolder + "\"";
				infoJSON += ",\"isInAS5Folder\":\"" + isInAS5Folder + "\"";
				infoJSON += ",\"isInNativeASFolder\":\"" + isInNativeASFolder + "\"}";

				return infoJSON;
			}
			else
			{
				// Process Exported file - Build required info
				string infoFillerString = "";
				infoJSON = "{\"link\":{\"type\":\"" + infoFillerString + "\",\"id\":\"" + infoFillerString + "\"}," + "\"unity_version\":\"" + infoFillerString + "\",\"pubdate\":\"" + infoFillerString + "\",\"version\":\"" + infoFillerString + "\",\"upload_id\":\"0\",\"version_id\":\"" + infoFillerString + "\"," + "\"category\":{\"label\":\"" + infoFillerString + "\",\"id\":\"" + infoFillerString + "\"},\"id\":\"" + infoFillerString + "\"," + "\"title\":\"" + fileAssetName + "\",\"publisher\":{\"label\":\"" + infoFillerString + "\",\"id\":\"" + infoFillerString + "\"}";

				infoJSON += ",\"filePath\":\"" + fileFullName + "\"";
				infoJSON += ",\"fileSize\":\"" + fileData.Length + "\"";
				infoJSON += ",\"fileDataCreated\":\"" + mTimeStamp + "\"";
				infoJSON += ",\"fileDateCreated\":\"" + fileData.CreationTime + "\"";
				infoJSON += ",\"isExported\":\"" + true + "\"";
				infoJSON += ",\"isDamaged\":\"" + isDamaged + "\"";
			//infoJSON += ",\"collection\":\"" + collection + "\"}";
				infoJSON += ",\"collection\":\"" + collection + "\"";
				infoJSON += ",\"isInASFolder\":\"" + isInASFolder + "\"";
				infoJSON += ",\"isInAS5Folder\":\"" + isInAS5Folder + "\"";
				infoJSON += ",\"isInNativeASFolder\":\"" + isInNativeASFolder + "\"}";

				return infoJSON;
			}
		}

		// Loop through Assets Folder to get info from all assets
		//private string GetPackageInfoFromFolder (svCollection collection, string assetPath, bool processErrors = true, string filePath = "") {
		private string GetPackageInfoFromFolder (svCollection collection, string assetPath, string filePath = "")
		{

			// scanGYAFiles
			//string filePattern = "*.unity?ackage";
			if (filePath == "gyaUpdate" && assetPath == GYAExt.PathUnityAssetStoreActive)
			{
				assetPath = Path.Combine(assetPath, "Xeir");
			//} else {
			//	filePath = assetPath;
			}

			string pkgText = "";
			// Make sure path is not empty or null
			if (assetPath.Length != 0 && assetPath != null)
			{
			// Find all *.unitypackage files
				DirectoryInfo directory = new DirectoryInfo (assetPath);

				// Verify folder exists
				if (directory.Exists)
				{
					// Does exist, so process files
					try
					{
						FileInfo[] unityPackageFiles = directory.GetFiles ("*.unity?ackage", SearchOption.AllDirectories).Where( fi => (fi.Attributes & FileAttributes.Hidden) == 0 ).ToArray();

					// Progressbar
					int filenameStartIndex = (directory.FullName.Length + 1);
					using (
						var progressBar = new ProgressBar(
						string.Format("{0} Scanning {1}", gyaAbbr, directory.FullName),
						unityPackageFiles.Length,
						80,
						(stepNumber) => unityPackageFiles[stepNumber].FullName.Substring(filenameStartIndex),
						(collection == svCollection.Standard ? false : userData.Settings.showProgressBarDuringRefresh))
					)
					// Always hide progressbar for Standard Assets

					// Process unitypackage files
					//Debug.Log("Package count: " + unityPackageFiles.Length + " - " + assetPath + "\n");
						for (int i = 0; i < unityPackageFiles.Length; ++i)
						{
							// Progressbar update
							progressBar.Update(i);

							//pkgText += GetPackageInfoFromFile (unityPackageFiles[i], collection, processErrors);
							pkgText += GetPackageInfoFromFile (unityPackageFiles[i], collection);
							// Add seperator for next file to process
							if (pkgText.Length > 0 && i < unityPackageFiles.Length - 1) {
								pkgText += ",";
							}
						}
						//Debug.Log(gyaAbbr + "- GetPackageInfoFromFolder: ( " + unityPackageFiles.Length + " ) " + collection.ToString() + "\n");
					}
					catch (Exception ex)
					{
						Debug.LogError(gyaAbbr + " - GetPackageInfoFromFolder - Path is Invalid: " + assetPath + "\nMake sure the path is currently accessible and try again.\n" + ex.Message);
						GUIOverride(OverrideReason.ErrorStep2);
					}
				}
				else
				{
					// Folder path does not exist
					Debug.Log(gyaAbbr + " - Folder not found: " + assetPath + "\n");
				}
			}
			else
			{
				// Un-assigned folder path (i.e.- User Folder)
				Debug.Log(gyaAbbr + "- NULL Folder" + "\n");
			}

			if (pkgText.Length == 0)
				pkgText = "";

			return pkgText;
		}

		public void ProgressBarTest (object pObj)
		{
			StopwatchStart();

			int timeToShow = 1;
			int stepCount = packageShow.Count;

			int fauxSecond = 511875; //524160;
			int fauxDelay = (timeToShow * fauxSecond) / stepCount;

			//int filenameStartIndex = (directory.FullName.Length + 1);
			using ( var progressBar = new ProgressBar (
				string.Format("{0} Scanning {1}", gyaAbbr, "Title"),
				packageShow.Count,
				60,
				(stepNumber) => packageShow[stepNumber].filePath,
				true, true
				)
			)
				//(stepNumber) => packageShow[stepNumber].FullName.Substring(filenameStartIndex)

			for (int i = 0; i < stepCount; ++i)
			{
				for (int k = 0; k < stepCount*fauxDelay; ++k)
				{

				}
				progressBar.Update(i);
				//if (!progressBar.Update(i))
				//	return;
			}
			StopwatchElapsed();
		}

		// ProgressBar
		public partial class ProgressBar : IDisposable
		{
			string title;
			float stepCount;
			Func<int, string> infoFormatter;
			long refreshIntervalTicks;
			long nextUpdateTicks;
			bool showProgressBar;
			bool canBeCancelled;

			public ProgressBar (
				string title,
				int stepCount,
				int refreshIntervalMilliseconds,
				Func<int, string> infoFormatter,
				bool showProgressBar = true,
				bool canBeCancelled = false)
			{
				if (string.IsNullOrEmpty(title))
					title = "Progress";

				if (title.Length > 58)
				{
					title = string.Format(
						"{0}...{1}",
						title.Substring(0, 33),
						title.Substring(title.Length - 22)
					);
				}

				this.title = title;
				this.stepCount = stepCount;
				this.refreshIntervalTicks = (TimeSpan.TicksPerMillisecond * refreshIntervalMilliseconds);
				if (infoFormatter != null)
				{
					this.infoFormatter = infoFormatter;
				}
				else
				{
					this.infoFormatter = (stepNumber) =>
						string.Format("Step {0}", (stepNumber + 1));
				}
				this.showProgressBar = showProgressBar;
				this.canBeCancelled = canBeCancelled;
			}

			public void Update (int stepNumber)
			{
				if (showProgressBar)
				{
					long currentTicks = DateTime.UtcNow.Ticks;
					if (nextUpdateTicks <= currentTicks)
					{
						nextUpdateTicks = (currentTicks + refreshIntervalTicks);
						if (canBeCancelled)
						{
							EditorUtility.DisplayCancelableProgressBar(
								title,
								infoFormatter(stepNumber),
								(stepNumber / stepCount)
							);
						}
						else
						{
							EditorUtility.DisplayProgressBar(
								title,
								infoFormatter(stepNumber),
								(stepNumber / stepCount)
							);
							//return false;
						}
					}
				}
				//return false;
			}

			public void Dispose()
			{
				EditorUtility.ClearProgressBar();
			}
		}

		// Count files in a given folder
		private int CountAssetsInFolder (string folder, bool includeSubFolders = true)
		{
			int fileCount = 0;
			if (Directory.Exists(folder))
			{
				if (includeSubFolders)
				{
					// Include sub-folders
					fileCount = Directory.GetFiles(folder, "*.unity?ackage", SearchOption.AllDirectories).Length;
				}
				else
				{
					// Do NOT include sub-folders
					fileCount = Directory.GetFiles(folder, "*.unity?ackage", SearchOption.TopDirectoryOnly).Length;
				}
			}
			return fileCount;
		}

		// Clear search
		private void SearchClear ()
		{
			GUIUtility.keyboardControl = 0;
			fldSearch = String.Empty;
			//ddCategory = ""; // Reset Cat dd
			//ddPublisher = ""; // Reset Pub dd
		}

		// Remove GYA from Standard Assets
		private void PersistDisable()
		{
			List<Packages> resultsStandard = pkgData.Assets.FindAll( x => x.id == 15398 && x.collection == svCollection.Standard );
			int countInStandard = resultsStandard.Count;

			userData.Settings.isPersist = false;

			// If found, delete instance of GYA from Standard Assets
			if (countInStandard > 0)
			{
				string pathDeleteAsset = resultsStandard[0].filePath;
				try
				{
					// Delete if exists
					File.Delete(pathDeleteAsset);
				}
				catch (Exception ex)
				{
					Debug.LogWarning(gyaAbbr + " - Persist Disable Error: " + pathDeleteAsset + "\n" + ex.Message);
				}
			}
		}

		// Persist in Standard Assets (Copy/Update GYA version in Standard Assets)
		// If enabled, this will be called from RefreshPackages so that it is always up-to-date
		private bool PersistEnable ()
		{
			//Debug.Log (gyaAbbr + " - PersistEnabled\n");
			//userData.Settings.isPersist = true;

			// Add check to include the unity_version
			//int unityVersionCurrent = GYAExt.GetUnityVersionMajor();

			// Check if GYA is already in the Standard Assets folder
			List<Packages> resultsStore = pkgData.Assets.FindAll( x => x.id == 15398 && x.collection == svCollection.Store );
			List<Packages> resultsStandard = pkgData.Assets.FindAll( x => x.id == 15398 && x.collection == svCollection.Standard );

			int verIDInStore = 0;
			int verIDInStandard = 0;

			// Check count in Standard Assets
			if (resultsStore.Count > 0)
			{
				if (resultsStore[0].unity_version.Length > 0)
				{
					if ( int.Parse(resultsStore[0].unity_version.Substring(0,1)) <= GYAExt.GetUnityVersionMajor )
					{
						verIDInStore = resultsStore[0].version_id;
					}
				}
			}

			// Check count in Standard Assets
			if (resultsStandard.Count > 0)
			{
				//for (int i = 0; i < verIDInStandard.Count; i++) {
				if (resultsStandard[0].unity_version.Length > 0)
				{
					//|| int.Parse(resultsStore[i].unity_version.Substring(0,1)) <= GYAExt.GetUnityVersionMajor()
					if ( int.Parse(resultsStandard[0].unity_version.Substring(0,1)) <= GYAExt.GetUnityVersionMajor )
					{
						verIDInStandard = resultsStandard[0].version_id;
					}
				}
				//}
			}

			//if (resultsStandard.Count > 0) {
			//	// Get the version ID in Standard Assets
			//	verIDInStandard = resultsStandard[0].version_id;
			//}


			// Perform update if required
			if( verIDInStore > verIDInStandard)
			{
				// Copy GYA to Standard Asset folder
				string pathCopyAsset = GYAExt.PathUnityStandardAssets;
				string fileName = Path.GetFileName(resultsStore[0].filePath);
				//string fileName = "- " + Path.GetFileName(resultsStore[0].filePath);
				//pathCopyAsset = Path.GetFullPath( Path.Combine(pathCopyAsset, fileName) );

				try
				{
					// Copy the file
					//if (Directory.Exists(Path.GetDirectoryName(pathCopyAsset))) {
					if (Directory.Exists(pathCopyAsset))
					{
						// NOTWORKING - Check SA permissions
						//DirectoryInfo folder = new DirectoryInfo(GYAExt.PathUnityStandardAssets);
						//folder.Attributes = folder.Attributes & ~FileAttributes.ReadOnly;

						////--
						//DirectoryInfo sourceDirectory = new DirectoryInfo(pathCopyAsset);
						//DirectorySecurity security = sourceDirectory.GetAccessControl();

						//security.SetAccessRuleProtection(true, true);
						//destinationDirectory.SetAccessControl(security);

						//var filesToCopy = sourceDirectory.GetFiles();

						////foreach (FileInfo file in filesToCopy)
						////{
						//	String path = Path.Combine(destinationFolder, file.Name);
						//	FileSecurity fileSecurity = file.GetAccessControl();

						//	fileSecurity.SetAccessRuleProtection(true, true);

						//	file.CopyTo(path, false);

						//	FileInfo copiedFile = new FileInfo(path);

						//	copiedFile.SetAccessControl(fileSecurity);
						////}
						//// --

						pathCopyAsset = Path.GetFullPath( Path.Combine(pathCopyAsset, fileName) );

						if (File.Exists (resultsStore[0].filePath))
							File.SetAttributes(resultsStore[0].filePath, FileAttributes.Normal);

						if (File.Exists (pathCopyAsset))
							File.SetAttributes(pathCopyAsset, FileAttributes.Normal);

						File.Copy (GYAExt.PathFixedForOS(resultsStore[0].filePath), pathCopyAsset, true);
						//GYAIO.FileCopy(GYAExt.PathFixedForOS(resultsStore[0].filePath), pathCopyAsset);
					}
					else
					{
						Debug.LogWarning(gyaAbbr + " - Persist - Folder not found: " + Path.GetDirectoryName(pathCopyAsset) + "\nPlease install Unity's " + GYAExt.FolderUnityStandardAssets + ".");
						return false;
					}
					// Verification
					if (File.Exists(pathCopyAsset))
					{
						//Debug.Log(gyaAbbr + " - Persist: Latest version copied to the " + GYAExt.FolderUnityStandardAssets + " folder.  Refreshing packages ...\n");
						Debug.Log(gyaAbbr + " - Persist: Latest version (" + resultsStore[0].version + ") copied to the " + GYAExt.FolderUnityStandardAssets + " folder.\n");
					}
					else
					{
						Debug.LogWarning(gyaAbbr + " - Persist - File Copy Failed: " + pathCopyAsset + "\nMake sure you have permissions to write to the " + GYAExt.PathUnityStandardAssets + " folder.");
						return false;
					}
				}
				catch (Exception ex)
				{
					Debug.LogWarning(gyaAbbr + " - Persist - Exception Copying: " + pathCopyAsset +
						"\nPlease make sure you have Write Access to the '" + GYAExt.FolderUnityStandardAssets + "' folder.\n" + ex.Message);
					return false; // Try failed
				}
				return true; // Persist is running, package refresh should not be shown
			}
			return false; // Persist did not copy/update, package refresh should be shown
		}

		// Return the relative path of this script
		private string PathOfThisScript
		{
			get
			{
				MonoScript script = MonoScript.FromScriptableObject( this );
				string sPath = AssetDatabase.GetAssetPath( script );
				sPath = Path.GetDirectoryName ( sPath );
				// Enable these lines to return the full path
				//string dPath = Path.GetDirectoryName (Application.dataPath);
				//sPath = Path.Combine( dPath, sPath );
				return sPath;
			}
		}

		// Load textures
		private void LoadTextures()
		{
			//Debug.Log (gyaAbbr + " - Textures loaded!\n");
			iconPrev = GetIconPrev ();
			iconNext = GetIconNext ();
			iconFavorite = GetIconFavorite ();
			iconDamaged = GetIconDamaged ();
			iconMenu = GetIconMenu ();
			iconRefresh = GetIconRefresh ();
			iconReset = GetIconReset ();
			iconResetX = GetIconResetX ();
			iconCategory = GetIconCategory ();
			iconCategoryX = GetIconCategoryX ();
			iconPublisher = GetIconPublisher ();
			iconPublisherX = GetIconPublisherX ();

			//if (userData.Settings.enableAltIconSwap) {
			//	// Swap Icons
			//	iconStore = GetIconStoreAlt ();
			//	iconUser = GetIconUserAlt ();
			//	iconStandard = GetIconStandardAlt ();
			//	iconOld = GetIconOldAlt ();
			//	iconStoreAlt = GetIconStore ();
			//	iconUserAlt = GetIconUser ();
			//	iconStandardAlt = GetIconStandard ();
			//	iconOldAlt = GetIconOld ();
			//} else {
				// Default Icons
				iconStore = GetIconStore ();
				iconUser = GetIconUser ();
				iconStandard = GetIconStandard ();
				iconOld = GetIconOld ();
				iconStoreAlt = GetIconStoreAlt ();
				iconUserAlt = GetIconUserAlt ();
				iconStandardAlt = GetIconStandardAlt ();
				iconOldAlt = GetIconOldAlt ();
			//}

			//iconUser = GetIconUser ();
			//iconStandard = GetIconStandard ();
			//iconOld = GetIconOld ();
			iconProject = GetIconProject ();

			//iconBlank = GetIconBlank ();

			texTransparent = GetTexTransparent ();
			texDivider = GetTexDivider ();
			texSelector = GetTexSelector ();
		}

		private void CheckIfGUISkinHasChanged (bool forceReload = false)
		{
			// Check if GUI Skin has been changed
			if (GUISkinHasChanged || forceReload)
			{
				LoadTextures ();
				//if (texSelector == null)
				//	LoadTextures ();
				//svStyleDefault.normal.background = texTransparent;
				svStyleDefault.active.background = texSelector;
				svStyleDefault.hover.background = texSelector;
				//svStyleDefault.hover.background = GUI.skin.textField.hover.background;
				//svStyleDefault.focused.background = GUI.skin.textField.hover.background;
				//svStyleStore.normal.background = texTransparent;
				svStyleStore.hover.background = texSelector;
				svStyleStore.active.background = texSelector;
				//svStyleStoreAlt.hover.background = texSelector;
				//svStyleStoreAlt.active.background = texSelector;
				svStyleUser.hover.background = texSelector;
				svStyleUser.active.background = texSelector;
				svStyleStandard.hover.background = texSelector;
				svStyleStandard.active.background = texSelector;
				svStyleOld.hover.background = texSelector;
				svStyleOld.active.background = texSelector;
				svStyleOldToMove.hover.background = texSelector;
				svStyleOldToMove.active.background = texSelector;
				svStyleProject.hover.background = texSelector;
				svStyleProject.active.background = texSelector;
				svStyleSeperator.normal.background = texDivider;

				// Tango Colors
				//Color32 cButter1 = new Color32(252, 233, 79, 255);
				//Color32 cButter2 = new Color32(237, 212, 0, 255);
				//Color32 cButter3 = new Color32(196, 160, 0, 255);
				Color32 cChameleon1 = new Color32(138, 226, 52, 255);
				//Color32 cChameleon2 = new Color32(115, 210, 22, 255);
				//Color32 cChameleon3 = new Color32(78, 154, 6, 255);
				//Color32 cOrange1 = new Color32(252, 175, 62, 255);
				//Color32 cOrange2 = new Color32(245, 121, 0, 255);
				Color32 cOrange3 = new Color32(206, 92, 0, 255);
				Color32 cSkyBlue1 = new Color32(114, 159, 207, 255);
				//Color32 cSkyBlue2 = new Color32(52, 101, 164, 255);
				//Color32 cSkyBlue3 = new Color32(32, 74, 135, 255);
				Color32 cPlum1 = new Color32(173, 127, 168, 255);
				//Color32 cPlum2 = new Color32(117, 80, 123, 255);
				//Color32 cPlum3 = new Color32(92, 53, 102, 255);
				//Color32 cChocolate1 = new Color32(233, 185, 110, 255);
				Color32 cChocolate2 = new Color32(193, 125, 17, 255);
				//Color32 cChocolate3 = new Color32(143, 89, 2, 255);
				Color32 cScarletRed1 = new Color32(239, 41, 41, 255);
				//Color32 cScarletRed2 = new Color32(204, 0, 0, 255);
				Color32 cScarletRed3 = new Color32(164, 0, 0, 255);
				Color32 cAluminium1 = new Color32(238, 238, 236, 255);
				Color32 cAluminium2 = new Color32(211, 215, 207, 255);
				Color32 cAluminium3 = new Color32(186, 189, 182, 255);
				//Color32 cAluminium4 = new Color32(136, 138, 133, 255);
				//Color32 cAluminium5 = new Color32(85, 87, 83, 255);
				//Color32 cAluminium6 = new Color32(46, 52, 54, 255);

				// Misc colors
				//Color32 cWhite = new Color32(249, 250, 252, 255);
				Color32 cDarkGreen = new Color32(0, 100, 0, 255);
				Color32 cDarkBlue = new Color32(0, 0, 139, 255);
				Color32 cDarkMagenta = new Color32(139, 0, 139, 255);
				//Color32 cDarkRed2 = new Color32(139, 0, 0, 255);
				Color32 cGold = new Color32(220, 220, 28, 255);

				if (GYAExt.IsProSkin)
				{
					// Pro
					if (userData.Settings.enableColors)
					{
						//svStyleDefault.normal.textColor = cChameleon1;
						//svStyleDefault.hover.textColor = cAluminium2;
						//svStyleDefault.active.textColor = cAluminium2;
						svStyleStore.normal.textColor = cChameleon1;
						svStyleStore.hover.textColor = cAluminium2;
						svStyleStore.active.textColor = cAluminium2;
						//svStyleStoreAlt.normal.textColor = cChameleon2;
						//svStyleStoreAlt.hover.textColor = cAluminium2;
						//svStyleStoreAlt.active.textColor = cAluminium2;
						svStyleUser.normal.textColor = cSkyBlue1;
						svStyleUser.hover.textColor = cAluminium2;
						svStyleUser.active.textColor = cAluminium2;
						svStyleStandard.normal.textColor = cPlum1;
						svStyleStandard.hover.textColor = cAluminium2;
						svStyleStandard.active.textColor = cAluminium2;
						svStyleOld.normal.textColor = cChocolate2;
						svStyleOld.hover.textColor = cAluminium2;
						svStyleOld.active.textColor = cAluminium2;
						svStyleOldToMove.normal.textColor = cScarletRed1;
						svStyleOldToMove.hover.textColor = cAluminium2;
						svStyleOldToMove.active.textColor = cAluminium2;
						svStyleProject.normal.textColor = cAluminium3;
						svStyleProject.hover.textColor = cAluminium2;
						svStyleProject.active.textColor = cAluminium2;
						svStyleSeperator.normal.textColor = cGold;
					}
					else
					{
						svStyleStore.normal.textColor = cAluminium3;
						svStyleStore.hover.textColor = cAluminium2;
						svStyleStore.active.textColor = cAluminium2;
						//svStyleStoreAlt.normal.textColor = cAluminium3;
						//svStyleStoreAlt.hover.textColor = cAluminium2;
						//svStyleStoreAlt.active.textColor = cAluminium2;
						svStyleUser.normal.textColor = cAluminium3;
						svStyleUser.hover.textColor = cAluminium2;
						svStyleUser.active.textColor = cAluminium2;
						svStyleStandard.normal.textColor = cAluminium3;
						svStyleStandard.hover.textColor = cAluminium2;
						svStyleStandard.active.textColor = cAluminium2;
						svStyleOld.normal.textColor = cAluminium3;
						svStyleOld.hover.textColor = cAluminium2;
						svStyleOld.active.textColor = cAluminium2;
						svStyleOldToMove.normal.textColor = cAluminium3;
						svStyleOldToMove.hover.textColor = cAluminium2;
						svStyleOldToMove.active.textColor = cAluminium2;
						svStyleProject.normal.textColor = cAluminium3;
						svStyleProject.hover.textColor = cAluminium2;
						svStyleProject.active.textColor = cAluminium2;
						svStyleSeperator.normal.textColor = cGold;
					}
				}
				else
				{
					// Free
					if (userData.Settings.enableColors)
					{
						svStyleStore.normal.textColor = cDarkGreen;
						svStyleStore.hover.textColor = cAluminium1;
						svStyleStore.active.textColor = cAluminium1;
						//svStyleStoreAlt.normal.textColor = cDarkGreen;
						//svStyleStoreAlt.hover.textColor = cAluminium1;
						//svStyleStoreAlt.active.textColor = cAluminium1;
						svStyleUser.normal.textColor = cDarkBlue;
						svStyleUser.hover.textColor = cAluminium1;
						svStyleUser.active.textColor = cAluminium1;
						svStyleStandard.normal.textColor = cDarkMagenta;
						svStyleStandard.hover.textColor = cAluminium1;
						svStyleStandard.active.textColor = cAluminium1;
						svStyleOld.normal.textColor = cOrange3;
						svStyleOld.hover.textColor = cAluminium1;
						svStyleOld.active.textColor = cAluminium1;
						svStyleOldToMove.normal.textColor = cScarletRed3;
						svStyleOldToMove.hover.textColor = cAluminium1;
						svStyleOldToMove.active.textColor = cAluminium1;
						svStyleProject.normal.textColor = Color.black;
						svStyleProject.hover.textColor = cAluminium1;
						svStyleProject.active.textColor = cAluminium1;
						svStyleSeperator.normal.textColor = cAluminium1;
					}
					else
					{
						svStyleStore.normal.textColor = Color.black;
						svStyleStore.hover.textColor = cAluminium1;
						svStyleStore.active.textColor = cAluminium1;
						//svStyleStoreAlt.normal.textColor = Color.black;
						//svStyleStoreAlt.hover.textColor = cAluminium1;
						//svStyleStoreAlt.active.textColor = cAluminium1;
						svStyleUser.normal.textColor = Color.black;
						svStyleUser.hover.textColor = cAluminium1;
						svStyleUser.active.textColor = cAluminium1;
						svStyleStandard.normal.textColor = Color.black;
						svStyleStandard.hover.textColor = cAluminium1;
						svStyleStandard.active.textColor = cAluminium1;
						svStyleOld.normal.textColor = Color.black;
						svStyleOld.hover.textColor = cAluminium1;
						svStyleOld.active.textColor = cAluminium1;
						svStyleOldToMove.normal.textColor = Color.black;
						svStyleOldToMove.hover.textColor = cAluminium1;
						svStyleOldToMove.active.textColor = cAluminium1;
						svStyleProject.normal.textColor = Color.black;
						svStyleProject.hover.textColor = cAluminium1;
						svStyleProject.active.textColor = cAluminium1;
						svStyleSeperator.normal.textColor = cAluminium1;
					}
				}
			}
		}

		// Check if Unity Skin has changed
		private bool GUISkinHasChanged
		{
			get
			{
				// Current state of the Pro skin
				if (GYAExt.IsProSkin)
				{
					GUISkinChangedCurrent = UnityGUISkin.Pro;
				}
				else
				{
					GUISkinChangedCurrent = UnityGUISkin.NonPro;
				}
				// Has the state changed
				if (GUISkinChangedLast == GUISkinChangedCurrent)
				{
					// No it has not
					return false;
				}
				else
				{
					// Yes it has
					GUISkinChangedLast = GUISkinChangedCurrent;
					return true;
				}
			}
		}

		// Create Texture2D from base64 string
		// return BuildTexture(texString, 16, 16);
		internal static Texture2D BuildTexture (string base64Text, int x, int y)
		{
			Texture2D texture = new Texture2D(x, y);
			texture.hideFlags = HideFlags.HideAndDontSave;
			texture.LoadImage( Convert.FromBase64String(base64Text) );
			return texture;
		}

		private static Texture2D ShowColor(Color32 color)
		{
			return ShowColor ((Color)color);
		}

		private static Texture2D ShowColor(Color color)
		{
			Texture2D texture = new Texture2D(1, 1);
			texture.SetPixel(0, 0, color);
			texture.Apply();
			texture.hideFlags = HideFlags.HideAndDontSave;
			return texture;
		}

		// SV Header bar color
		private static Texture2D GetTexDivider()
		{
			if (GYAExt.IsProSkin)
			{
				return ShowColor(new Color32(70, 70, 70, 255));
			}
			else
			{
				return ShowColor(new Color32(154, 154, 154, 255));
			}
		}

		// SV Selection bar color
		private static Texture2D GetTexSelector()
		{
			if (GYAExt.IsProSkin)
			{
				return ShowColor(new Color32(62, 95, 150, 255));
			}
			else
			{
				return ShowColor(new Color32(62, 125, 231, 255));
			}
		}

		// Transparent Texture
		private static Texture2D GetTexTransparent ()
		{
			string texString = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABAQMAAAAl21bKAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAADUExURQAAAKd6PdoAAAABdFJOUwBA5thmAAAACklEQVQI12NgAAAAAgAB4iG8MwAAAABJRU5ErkJggg==";
			return BuildTexture(texString, 1, 1);
		}

		private static Texture2D GetIconFavorite ()
		{
			//string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQBAMAAADt3eJSAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAYUExURQAAAP3XHWVlY9e6Ln13WJmLTOvJJK2bQqN+ohMAAAAIdFJOUwD+Mc9Zi9691ssrzgAAAFxJREFUCNdjYEACrAFQBrMBlOEoAmUYCoMpJVdBwRAlBgb2REEgECtgYDIEMYQVGBiUQQwjoBqQEEiAgaFQUFAcohsoAqKZEoUNxUBSTGJBqokgBksRA4O6A8IJADIdCPg+hJMMAAAAAElFTkSuQmCC";
			string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAALEwAACxMBAJqcGAAABCRpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IlhNUCBDb3JlIDUuNC4wIj4KICAgPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4KICAgICAgPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIKICAgICAgICAgICAgeG1sbnM6dGlmZj0iaHR0cDovL25zLmFkb2JlLmNvbS90aWZmLzEuMC8iCiAgICAgICAgICAgIHhtbG5zOmV4aWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20vZXhpZi8xLjAvIgogICAgICAgICAgICB4bWxuczpkYz0iaHR0cDovL3B1cmwub3JnL2RjL2VsZW1lbnRzLzEuMS8iCiAgICAgICAgICAgIHhtbG5zOnhtcD0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wLyI+CiAgICAgICAgIDx0aWZmOlJlc29sdXRpb25Vbml0PjI8L3RpZmY6UmVzb2x1dGlvblVuaXQ+CiAgICAgICAgIDx0aWZmOkNvbXByZXNzaW9uPjE8L3RpZmY6Q29tcHJlc3Npb24+CiAgICAgICAgIDx0aWZmOlhSZXNvbHV0aW9uPjcyPC90aWZmOlhSZXNvbHV0aW9uPgogICAgICAgICA8dGlmZjpPcmllbnRhdGlvbj4xPC90aWZmOk9yaWVudGF0aW9uPgogICAgICAgICA8dGlmZjpZUmVzb2x1dGlvbj43MjwvdGlmZjpZUmVzb2x1dGlvbj4KICAgICAgICAgPGV4aWY6UGl4ZWxYRGltZW5zaW9uPjE2PC9leGlmOlBpeGVsWERpbWVuc2lvbj4KICAgICAgICAgPGV4aWY6Q29sb3JTcGFjZT4xPC9leGlmOkNvbG9yU3BhY2U+CiAgICAgICAgIDxleGlmOlBpeGVsWURpbWVuc2lvbj4xNjwvZXhpZjpQaXhlbFlEaW1lbnNpb24+CiAgICAgICAgIDxkYzpzdWJqZWN0PgogICAgICAgICAgICA8cmRmOkJhZy8+CiAgICAgICAgIDwvZGM6c3ViamVjdD4KICAgICAgICAgPHhtcDpNb2RpZnlEYXRlPjIwMTUtMDYtMTBUMjE6MDY6NTQ8L3htcDpNb2RpZnlEYXRlPgogICAgICAgICA8eG1wOkNyZWF0b3JUb29sPlBpeGVsbWF0b3IgMy4zLjI8L3htcDpDcmVhdG9yVG9vbD4KICAgICAgPC9yZGY6RGVzY3JpcHRpb24+CiAgIDwvcmRmOlJERj4KPC94OnhtcG1ldGE+ChwneFUAAAFFSURBVDgRY2CgJZjR7d0NwvjsYMEn6aD/2AWfPF65uorIyL/XZf+BMIiNVzE2yRu79M7DDACxsakBiTHCJJKTkw1BbGnRbxpSwp+MUn0uFcPkQPTsLXq9z97ynXv6musGiD937lywoWAD1s522mun90hRiO+nAkiSEHj3if3BoUty94NT9zkzgRRvO6FY8vYj10dCGmHyILUgPSA+M4g4f/78i+9sPqc0ZN9ZCPP/kACJ4QK3Hwte7F5pmgTzAtgAkGJiDEHXDNIH9gKIAQIgU68+FHkH4WGSIDmYzTBZFANAgtryb4Rgkug0NjkUA0BRKcz/jR+mEeRkEIbxQXKw6IaJYSRlUFSComntIfW1p29ILAMpNNV4ERVsdzMYWzSjGABKRBuOqO4HRRGyX+cCw/h0cvIyL4v7PSA1QDNxpkyYy4imAfzPkqa2yUriAAAAAElFTkSuQmCC";
			return BuildTexture(texString, 16, 16);
		}

		private static Texture2D GetIconDamaged ()
		{
			//string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQBAMAAADt3eJSAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAYUExURQAAADAoKB40NCUxMfIAANwEBLEDAPOXl3ZvmQoAAAAEdFJOUwD9OXqw25OBAAAAVElEQVQI12NgQABFQUEhEM0kFhqaqAASSHFxcQMKMYm6uJe4AIUYU0AMNwEGxhAYIyUExggNLw2FMIAAWQRoHpABMlExNTQ0TAhsRVoa2Aq4pVAAAEASFqUvE0KOAAAAAElFTkSuQmCC";
			string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAALEwAACxMBAJqcGAAABCRpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IlhNUCBDb3JlIDUuNC4wIj4KICAgPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4KICAgICAgPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIKICAgICAgICAgICAgeG1sbnM6dGlmZj0iaHR0cDovL25zLmFkb2JlLmNvbS90aWZmLzEuMC8iCiAgICAgICAgICAgIHhtbG5zOmV4aWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20vZXhpZi8xLjAvIgogICAgICAgICAgICB4bWxuczpkYz0iaHR0cDovL3B1cmwub3JnL2RjL2VsZW1lbnRzLzEuMS8iCiAgICAgICAgICAgIHhtbG5zOnhtcD0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wLyI+CiAgICAgICAgIDx0aWZmOlJlc29sdXRpb25Vbml0PjI8L3RpZmY6UmVzb2x1dGlvblVuaXQ+CiAgICAgICAgIDx0aWZmOkNvbXByZXNzaW9uPjE8L3RpZmY6Q29tcHJlc3Npb24+CiAgICAgICAgIDx0aWZmOlhSZXNvbHV0aW9uPjcyPC90aWZmOlhSZXNvbHV0aW9uPgogICAgICAgICA8dGlmZjpPcmllbnRhdGlvbj4xPC90aWZmOk9yaWVudGF0aW9uPgogICAgICAgICA8dGlmZjpZUmVzb2x1dGlvbj43MjwvdGlmZjpZUmVzb2x1dGlvbj4KICAgICAgICAgPGV4aWY6UGl4ZWxYRGltZW5zaW9uPjE2PC9leGlmOlBpeGVsWERpbWVuc2lvbj4KICAgICAgICAgPGV4aWY6Q29sb3JTcGFjZT4xPC9leGlmOkNvbG9yU3BhY2U+CiAgICAgICAgIDxleGlmOlBpeGVsWURpbWVuc2lvbj4xNjwvZXhpZjpQaXhlbFlEaW1lbnNpb24+CiAgICAgICAgIDxkYzpzdWJqZWN0PgogICAgICAgICAgICA8cmRmOkJhZy8+CiAgICAgICAgIDwvZGM6c3ViamVjdD4KICAgICAgICAgPHhtcDpNb2RpZnlEYXRlPjIwMTUtMDYtMTFUMDA6MDY6NTk8L3htcDpNb2RpZnlEYXRlPgogICAgICAgICA8eG1wOkNyZWF0b3JUb29sPlBpeGVsbWF0b3IgMy4zLjI8L3htcDpDcmVhdG9yVG9vbD4KICAgICAgPC9yZGY6RGVzY3JpcHRpb24+CiAgIDwvcmRmOlJERj4KPC94OnhtcG1ldGE+Cgao4N8AAADzSURBVDgRY2CgEDCi65c3M7MU/PTpCLo4iP+ej8/m4alTx7HJgcVAmg00NP5uZGb4f4eFBQWDxEByIDVYDUDW/ImB4T82jNMQmGaQrcgaP0+f/h+EkcXQDWECOQfk58bbN5jE/vzB6jpkQce/DAwgtbBwAhsAUqDNyIKsjmg23ICr/yG2v2IhzSCsqmGGiEPdAeODuOjexGoAzP0vc3NhTJw03AvYVIhPnswAwvgA2ABQCqtX1fi3nxmfUogcSA1ILUgPimpYWsCWCmGpEj0NgAxAyQsgQ2BpAsV0KAdmM3J+QDEApA5mCDYDCGYmbJoIiQEAl0CNWA7JYkkAAAAASUVORK5CYII=";
			return BuildTexture(texString, 16, 16);
		}

		private static Texture2D GetIconMenu ()
		{
			//string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQBAMAAADt3eJSAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAYUExURQAAAIaGhoWFhX9/f4SEhIWFhYWFhYWFhVweI34AAAAIdFJOUwD+4xhSpsmIBkwnpAAAAGBJREFUCNdjYEACYQzGEIaiQ6IBkHJyFxQUAvELBYEMkAgbkCGoFMDAYCgYwC4oGMDAJCgIVC4CloEwgFIFQKkCBqCckKKgIlBxIFg7UL8b0EAxiBWCzooGYEYqzFIwAAD+owqndEVovAAAAABJRU5ErkJggg==";
			string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAALEwAACxMBAJqcGAAABCRpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IlhNUCBDb3JlIDUuNC4wIj4KICAgPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4KICAgICAgPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIKICAgICAgICAgICAgeG1sbnM6dGlmZj0iaHR0cDovL25zLmFkb2JlLmNvbS90aWZmLzEuMC8iCiAgICAgICAgICAgIHhtbG5zOmV4aWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20vZXhpZi8xLjAvIgogICAgICAgICAgICB4bWxuczpkYz0iaHR0cDovL3B1cmwub3JnL2RjL2VsZW1lbnRzLzEuMS8iCiAgICAgICAgICAgIHhtbG5zOnhtcD0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wLyI+CiAgICAgICAgIDx0aWZmOlJlc29sdXRpb25Vbml0PjI8L3RpZmY6UmVzb2x1dGlvblVuaXQ+CiAgICAgICAgIDx0aWZmOkNvbXByZXNzaW9uPjE8L3RpZmY6Q29tcHJlc3Npb24+CiAgICAgICAgIDx0aWZmOlhSZXNvbHV0aW9uPjcyPC90aWZmOlhSZXNvbHV0aW9uPgogICAgICAgICA8dGlmZjpPcmllbnRhdGlvbj4xPC90aWZmOk9yaWVudGF0aW9uPgogICAgICAgICA8dGlmZjpZUmVzb2x1dGlvbj43MjwvdGlmZjpZUmVzb2x1dGlvbj4KICAgICAgICAgPGV4aWY6UGl4ZWxYRGltZW5zaW9uPjE2PC9leGlmOlBpeGVsWERpbWVuc2lvbj4KICAgICAgICAgPGV4aWY6Q29sb3JTcGFjZT4xPC9leGlmOkNvbG9yU3BhY2U+CiAgICAgICAgIDxleGlmOlBpeGVsWURpbWVuc2lvbj4xNjwvZXhpZjpQaXhlbFlEaW1lbnNpb24+CiAgICAgICAgIDxkYzpzdWJqZWN0PgogICAgICAgICAgICA8cmRmOkJhZy8+CiAgICAgICAgIDwvZGM6c3ViamVjdD4KICAgICAgICAgPHhtcDpNb2RpZnlEYXRlPjIwMTUtMDYtMTBUMjM6MDY6OTU8L3htcDpNb2RpZnlEYXRlPgogICAgICAgICA8eG1wOkNyZWF0b3JUb29sPlBpeGVsbWF0b3IgMy4zLjI8L3htcDpDcmVhdG9yVG9vbD4KICAgICAgPC9yZGY6RGVzY3JpcHRpb24+CiAgIDwvcmRmOlJERj4KPC94OnhtcG1ldGE+CuODdQIAAAFhSURBVDgRtZLBSsNAEIabYHoqNLmJxZs3r/YsIr6E+BAiFKEJJIGmIAXpQ4g+hKh41qu33kTpLTm0lxYS5wudZan1IOLC8s/Mzj/778w2Gv+1BoPBE1vr93q9bbb6iq4a6+g4zp7ruodpmh5z1m6374Ig+Fwv4igxSZJTz/OulsvlpeCBFDjXM8Wqqj7CMNxVHzQKVqROs9m82URW0o8KSODNyNbkTYgK4oKTKIqOjAIq22RJGPf7fZeNrcVEXYdtfIwsy97tIDGIoC47Z7FYnEnPbjmrk5Cjib/FugBvyfN8xybLjdfqY9sKabSemzGSbMvEF2V1w2wyMXz6IiO9MAVWt3ybPYV0QS6KojsajaY6TtMo+UCvZVk+C55QXUk2SnwCmXHzK/mdRoGdiD0cDkuQgrPZ7M33/Rek06tWq7UvH++eaWyRtGmhhngcxw+gSO6CKBCYypPH8/n8kdif1heGjKig3CVfNAAAAABJRU5ErkJggg==";
			return BuildTexture(texString, 16, 16);
		}

		private static Texture2D GetIconRefresh ()
		{
			//string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQBAMAAADt3eJSAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAYUExURQAAAISEhISEhIWFhYODg4WFhX19fYaGhrlItTAAAAAHdFJOUwB9XOZDtRVqnEXmAAAAX0lEQVQI12NgQAaMgoIQBnN5CYgLYhgAGeIBQEYhAwNbeRkDs3h5AgNTuQADE2O5AgMIg3lwBkt5AAMDa7kDA4N5EQODejFQu3t5oCjEwHIgEABZ4FxebgKxylEF2QUAVA8Q6HDrYnQAAAAASUVORK5CYII=";
			string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAALEwAACxMBAJqcGAAABCRpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IlhNUCBDb3JlIDUuNC4wIj4KICAgPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4KICAgICAgPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIKICAgICAgICAgICAgeG1sbnM6dGlmZj0iaHR0cDovL25zLmFkb2JlLmNvbS90aWZmLzEuMC8iCiAgICAgICAgICAgIHhtbG5zOmV4aWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20vZXhpZi8xLjAvIgogICAgICAgICAgICB4bWxuczpkYz0iaHR0cDovL3B1cmwub3JnL2RjL2VsZW1lbnRzLzEuMS8iCiAgICAgICAgICAgIHhtbG5zOnhtcD0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wLyI+CiAgICAgICAgIDx0aWZmOlJlc29sdXRpb25Vbml0PjI8L3RpZmY6UmVzb2x1dGlvblVuaXQ+CiAgICAgICAgIDx0aWZmOkNvbXByZXNzaW9uPjE8L3RpZmY6Q29tcHJlc3Npb24+CiAgICAgICAgIDx0aWZmOlhSZXNvbHV0aW9uPjcyPC90aWZmOlhSZXNvbHV0aW9uPgogICAgICAgICA8dGlmZjpPcmllbnRhdGlvbj4xPC90aWZmOk9yaWVudGF0aW9uPgogICAgICAgICA8dGlmZjpZUmVzb2x1dGlvbj43MjwvdGlmZjpZUmVzb2x1dGlvbj4KICAgICAgICAgPGV4aWY6UGl4ZWxYRGltZW5zaW9uPjE2PC9leGlmOlBpeGVsWERpbWVuc2lvbj4KICAgICAgICAgPGV4aWY6Q29sb3JTcGFjZT4xPC9leGlmOkNvbG9yU3BhY2U+CiAgICAgICAgIDxleGlmOlBpeGVsWURpbWVuc2lvbj4xNjwvZXhpZjpQaXhlbFlEaW1lbnNpb24+CiAgICAgICAgIDxkYzpzdWJqZWN0PgogICAgICAgICAgICA8cmRmOkJhZy8+CiAgICAgICAgIDwvZGM6c3ViamVjdD4KICAgICAgICAgPHhtcDpNb2RpZnlEYXRlPjIwMTUtMDYtMTBUMjM6MDY6NTI8L3htcDpNb2RpZnlEYXRlPgogICAgICAgICA8eG1wOkNyZWF0b3JUb29sPlBpeGVsbWF0b3IgMy4zLjI8L3htcDpDcmVhdG9yVG9vbD4KICAgICAgPC9yZGY6RGVzY3JpcHRpb24+CiAgIDwvcmRmOlJERj4KPC94OnhtcG1ldGE+CtOyabsAAAEvSURBVDgRrVIxTsNAEIwtu3NLQbr8AtGChJQfREoewAeQLItYBlt+SPIFOmij/CIdLniCLTkzh8faHEGKBFdkZ2d3Zi/nnUz+eIJL9FVVPft9WZa9kov8wi/5YxAE16q1bXsvHAownptk68R93zd5nn+IHw0oxpSXsizfVDwXeRM7yBmkaXpFMQVhGM6Z+2JMXotjr3qcQZIkDyqysa7rL+VDfOKjWRNp9BdmRnAw2EGItwNna04jA19zce4Muq7bSYFrLoT9aGvSjIuEl/3kC1OExpW5tvNBfYn6Zqg3qE+Jx0WC4yqO43eSbMTnXCDumcPwBnhOzMPeb4ReAUbtguV8DLM1v4j4EwOSRVHcRVG0wcRxdclD2HCy3ULyPwxI8gxGt4AHCE/W1zX8188R1hd6f8X3O1cAAAAASUVORK5CYII=";
			return BuildTexture(texString, 16, 16);
		}

		private static Texture2D GetIconReset ()
		{
			//string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQBAMAAADt3eJSAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAYUExURXV1dYSEhISEhIODg4KCgoSEhH9/f4WFhVQmwD0AAAAHdFJOUwHhhVs6rBUNkunCAAAAcElEQVQI12NgYGBwNmEAA8XyciEQnViobCQuBmSIK4A4DAxMRamhDuFAHqNACEMYa6IAg7tCAIMxA1MJg7lBAIMKA3MxkOFqJAJiuCuwmbgrAKUYxQKAOoGKmcqLGBhUw4GGiQswMKQWIlsBtxTqDACEgRN2t8azqgAAAABJRU5ErkJggg==";
			string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAALEwAACxMBAJqcGAAABCRpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IlhNUCBDb3JlIDUuNC4wIj4KICAgPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4KICAgICAgPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIKICAgICAgICAgICAgeG1sbnM6dGlmZj0iaHR0cDovL25zLmFkb2JlLmNvbS90aWZmLzEuMC8iCiAgICAgICAgICAgIHhtbG5zOmV4aWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20vZXhpZi8xLjAvIgogICAgICAgICAgICB4bWxuczpkYz0iaHR0cDovL3B1cmwub3JnL2RjL2VsZW1lbnRzLzEuMS8iCiAgICAgICAgICAgIHhtbG5zOnhtcD0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wLyI+CiAgICAgICAgIDx0aWZmOlJlc29sdXRpb25Vbml0PjI8L3RpZmY6UmVzb2x1dGlvblVuaXQ+CiAgICAgICAgIDx0aWZmOkNvbXByZXNzaW9uPjE8L3RpZmY6Q29tcHJlc3Npb24+CiAgICAgICAgIDx0aWZmOlhSZXNvbHV0aW9uPjcyPC90aWZmOlhSZXNvbHV0aW9uPgogICAgICAgICA8dGlmZjpPcmllbnRhdGlvbj4xPC90aWZmOk9yaWVudGF0aW9uPgogICAgICAgICA8dGlmZjpZUmVzb2x1dGlvbj43MjwvdGlmZjpZUmVzb2x1dGlvbj4KICAgICAgICAgPGV4aWY6UGl4ZWxYRGltZW5zaW9uPjE2PC9leGlmOlBpeGVsWERpbWVuc2lvbj4KICAgICAgICAgPGV4aWY6Q29sb3JTcGFjZT4xPC9leGlmOkNvbG9yU3BhY2U+CiAgICAgICAgIDxleGlmOlBpeGVsWURpbWVuc2lvbj4xNjwvZXhpZjpQaXhlbFlEaW1lbnNpb24+CiAgICAgICAgIDxkYzpzdWJqZWN0PgogICAgICAgICAgICA8cmRmOkJhZy8+CiAgICAgICAgIDwvZGM6c3ViamVjdD4KICAgICAgICAgPHhtcDpNb2RpZnlEYXRlPjIwMTUtMDYtMTBUMjE6MDY6NTQ8L3htcDpNb2RpZnlEYXRlPgogICAgICAgICA8eG1wOkNyZWF0b3JUb29sPlBpeGVsbWF0b3IgMy4zLjI8L3htcDpDcmVhdG9yVG9vbD4KICAgICAgPC9yZGY6RGVzY3JpcHRpb24+CiAgIDwvcmRmOlJERj4KPC94OnhtcG1ldGE+ChwneFUAAAGUSURBVDgRjZOxTgJBFEV3N1a7lNYEKhNqG37AHyAUJv6ArR1mqXYjHaHzB+yMoeITjI21LcbS2EIJ3vOct8xKwyTDzLx3731v7ixpkiSp5tGo63qoYD8k1mVZvh2BFIDcEhCx3u/3N1mWdWPCbrf7StP0SUJlHG8EJpPJeZ7n7xAdLOBHAA9clNx2u72czWY/5M4CIHGygA/T6bRVBYwKLBzDqlCPuHVA22rvHvJms1kURfFIMow59xfkWZgRHdAlWK6TAaI9EgQCGdKYqfQd12Mv3Avtg4UDN5PyEEUMIsAQOHZ8KdGrEB9zd7BwjKtEn6SGG/Z3OvwOtF0fjrZzbL8xMQLMua/OS01zXxVXUb61xQNXp5K1LyNvA2ol8rXmq0TrEGMxrNa1vUJVVZ9EMQgTg3mEbECWSPNK4RkTPXfPXkFJM6XT6XxrP/pXja5Knk1qFxSITTcBADwN5fw5rXT0w/fBkQIxpjGR9v1L40p0Jby77WZ2IYN1bfPAD6y0z0dCm3EcIqJ0G8ePBDwpnZP+zr8HHeRuLU/6AAAAAABJRU5ErkJggg==";
			return BuildTexture(texString, 16, 16);
		}

		private static Texture2D GetIconResetX ()
		{
			//string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQBAMAAADt3eJSAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAYUExURbljaYpoaqheYoh4eeUxPNU8RJRuccFGTQ2hhQAAAAAIdFJOUwHwIpfzhlLynCRFRgAAAHdJREFUCNc1jrEKg0AQRJ85LraufoESsD0VrEO4kPYK7YUDa5H8P9kVss2+YZidBeg6rhlEGtutzFlqBQkmwDVu2gZVRTUyuqKiT4Edf9JvgZm7Qc71Bem2t0Gt8kyaLA98fMO0JPg+wX/08iOua3xZ2RLN5f/GDzoAENIGH+AwAAAAAElFTkSuQmCC";
			string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAALEwAACxMBAJqcGAAABCRpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IlhNUCBDb3JlIDUuNC4wIj4KICAgPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4KICAgICAgPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIKICAgICAgICAgICAgeG1sbnM6dGlmZj0iaHR0cDovL25zLmFkb2JlLmNvbS90aWZmLzEuMC8iCiAgICAgICAgICAgIHhtbG5zOmV4aWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20vZXhpZi8xLjAvIgogICAgICAgICAgICB4bWxuczpkYz0iaHR0cDovL3B1cmwub3JnL2RjL2VsZW1lbnRzLzEuMS8iCiAgICAgICAgICAgIHhtbG5zOnhtcD0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wLyI+CiAgICAgICAgIDx0aWZmOlJlc29sdXRpb25Vbml0PjI8L3RpZmY6UmVzb2x1dGlvblVuaXQ+CiAgICAgICAgIDx0aWZmOkNvbXByZXNzaW9uPjE8L3RpZmY6Q29tcHJlc3Npb24+CiAgICAgICAgIDx0aWZmOlhSZXNvbHV0aW9uPjcyPC90aWZmOlhSZXNvbHV0aW9uPgogICAgICAgICA8dGlmZjpPcmllbnRhdGlvbj4xPC90aWZmOk9yaWVudGF0aW9uPgogICAgICAgICA8dGlmZjpZUmVzb2x1dGlvbj43MjwvdGlmZjpZUmVzb2x1dGlvbj4KICAgICAgICAgPGV4aWY6UGl4ZWxYRGltZW5zaW9uPjE2PC9leGlmOlBpeGVsWERpbWVuc2lvbj4KICAgICAgICAgPGV4aWY6Q29sb3JTcGFjZT4xPC9leGlmOkNvbG9yU3BhY2U+CiAgICAgICAgIDxleGlmOlBpeGVsWURpbWVuc2lvbj4xNjwvZXhpZjpQaXhlbFlEaW1lbnNpb24+CiAgICAgICAgIDxkYzpzdWJqZWN0PgogICAgICAgICAgICA8cmRmOkJhZy8+CiAgICAgICAgIDwvZGM6c3ViamVjdD4KICAgICAgICAgPHhtcDpNb2RpZnlEYXRlPjIwMTUtMDYtMTBUMjE6MDY6MjQ8L3htcDpNb2RpZnlEYXRlPgogICAgICAgICA8eG1wOkNyZWF0b3JUb29sPlBpeGVsbWF0b3IgMy4zLjI8L3htcDpDcmVhdG9yVG9vbD4KICAgICAgPC9yZGY6RGVzY3JpcHRpb24+CiAgIDwvcmRmOlJERj4KPC94OnhtcG1ldGE+Ck1QMw8AAAHISURBVDgRbVNLSwJRFJ6ZJDTNFDLCBCuKoDZTBEIvWmSb6AfUUlqU1LKQ2rQpSjeBYjt31dJFtEmISNtFj4VtDIW0kB40laZIYPMNnuEyeeFyHvf7vnPumTt8leN4rs469Hi62PR8JJJhY/J5rYDftx7mJWmOAKytWixHazvbXjanCqDiU6P+mg4BNhV/YogLxiY3K9pRKQ9RRzoiELleFRkTlQsECFOzVnAVgVrbHJERkzC68Ab3orm29lW5C0pzwCjXwQz8i14JG/6ub33/wOPphq+NcabF63B3VzortJbKIZKn+yFGB5LRLMpuhgY4k0yF3wz6ZXAFIp2LA1fksxYDZGP4LFYdIoFQEfeDpelbKuUAnWut2sHkbXIYhxiY4yWvECCCT4aps4MlLPDKoC6mZz8R5Bw2UUxlF/rjsQ3EtEDGO6CvRPiJ0+OWhk1ZpGh3Ntulb1dn/n1JX/l1ZXv7TLbH9BkJjCbiJ5dTbidiK68fAVYQ+BAw6kuEas/rh/Jf2G8SZiKz9n7cvWUplFYebNYqquNMHSLalwXukHweHPuSTIYgfFogylsJgaW82gElqArFrIWodj7/BIiAR0I+LPu42Pwf5vXo/NNnpL8AAAAASUVORK5CYII=";
			return BuildTexture(texString, 16, 16);
		}

		private static Texture2D GetIconCategory ()
		{
			//string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAGUExURQAAAIWFhZm/ewUAAAABdFJOUwBA5thmAAAAJUlEQVQI12NgAAPmHwwMPAz8TxgYTBjspzDYX4GhC2B0AITAAADfXApX7mKBhQAAAABJRU5ErkJggg==";
			string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAALEwAACxMBAJqcGAAABCRpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IlhNUCBDb3JlIDUuNC4wIj4KICAgPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4KICAgICAgPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIKICAgICAgICAgICAgeG1sbnM6dGlmZj0iaHR0cDovL25zLmFkb2JlLmNvbS90aWZmLzEuMC8iCiAgICAgICAgICAgIHhtbG5zOmV4aWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20vZXhpZi8xLjAvIgogICAgICAgICAgICB4bWxuczpkYz0iaHR0cDovL3B1cmwub3JnL2RjL2VsZW1lbnRzLzEuMS8iCiAgICAgICAgICAgIHhtbG5zOnhtcD0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wLyI+CiAgICAgICAgIDx0aWZmOlJlc29sdXRpb25Vbml0PjI8L3RpZmY6UmVzb2x1dGlvblVuaXQ+CiAgICAgICAgIDx0aWZmOkNvbXByZXNzaW9uPjE8L3RpZmY6Q29tcHJlc3Npb24+CiAgICAgICAgIDx0aWZmOlhSZXNvbHV0aW9uPjcyPC90aWZmOlhSZXNvbHV0aW9uPgogICAgICAgICA8dGlmZjpPcmllbnRhdGlvbj4xPC90aWZmOk9yaWVudGF0aW9uPgogICAgICAgICA8dGlmZjpZUmVzb2x1dGlvbj43MjwvdGlmZjpZUmVzb2x1dGlvbj4KICAgICAgICAgPGV4aWY6UGl4ZWxYRGltZW5zaW9uPjE2PC9leGlmOlBpeGVsWERpbWVuc2lvbj4KICAgICAgICAgPGV4aWY6Q29sb3JTcGFjZT4xPC9leGlmOkNvbG9yU3BhY2U+CiAgICAgICAgIDxleGlmOlBpeGVsWURpbWVuc2lvbj4xNjwvZXhpZjpQaXhlbFlEaW1lbnNpb24+CiAgICAgICAgIDxkYzpzdWJqZWN0PgogICAgICAgICAgICA8cmRmOkJhZy8+CiAgICAgICAgIDwvZGM6c3ViamVjdD4KICAgICAgICAgPHhtcDpNb2RpZnlEYXRlPjIwMTUtMDYtMTBUMjM6MDY6NDM8L3htcDpNb2RpZnlEYXRlPgogICAgICAgICA8eG1wOkNyZWF0b3JUb29sPlBpeGVsbWF0b3IgMy4zLjI8L3htcDpDcmVhdG9yVG9vbD4KICAgICAgPC9yZGY6RGVzY3JpcHRpb24+CiAgIDwvcmRmOlJERj4KPC94OnhtcG1ldGE+CrMdZrUAAABpSURBVDgRxZHdCoAwCIVX9NZe+9yF0DcOpREEORjHufMj2xjda6kGcPe9ujOzUldpZj+M1fzmpJdTdRYkB4f6yinPmqwhGwpt0gMjLXbGWSE9YSaE/8oAcoY9BvoD/06gybzH5wkw6sMDgb4mTex+YFkAAAAASUVORK5CYII=";
			return BuildTexture(texString, 16, 16);
		}

		private static Texture2D GetIconCategoryX ()
		{
			//string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQBAMAAADt3eJSAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAYUExURQAAALZMUuEzPpBhZKhUWcFFTdM7RMpBSX4uJy4AAAABdFJOUwBA5thmAAAAQklEQVQI12NgwAaMjY0N4GyYADNUwIUFRLi4OLCAGIJAwMiIzAgFAlZWIKMcCNhBOtKAgA3EUFJSU2KAMJSUsNoMAHijCaSDKz/cAAAAAElFTkSuQmCC";
			string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAALEwAACxMBAJqcGAAABCRpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IlhNUCBDb3JlIDUuNC4wIj4KICAgPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4KICAgICAgPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIKICAgICAgICAgICAgeG1sbnM6dGlmZj0iaHR0cDovL25zLmFkb2JlLmNvbS90aWZmLzEuMC8iCiAgICAgICAgICAgIHhtbG5zOmV4aWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20vZXhpZi8xLjAvIgogICAgICAgICAgICB4bWxuczpkYz0iaHR0cDovL3B1cmwub3JnL2RjL2VsZW1lbnRzLzEuMS8iCiAgICAgICAgICAgIHhtbG5zOnhtcD0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wLyI+CiAgICAgICAgIDx0aWZmOlJlc29sdXRpb25Vbml0PjI8L3RpZmY6UmVzb2x1dGlvblVuaXQ+CiAgICAgICAgIDx0aWZmOkNvbXByZXNzaW9uPjE8L3RpZmY6Q29tcHJlc3Npb24+CiAgICAgICAgIDx0aWZmOlhSZXNvbHV0aW9uPjcyPC90aWZmOlhSZXNvbHV0aW9uPgogICAgICAgICA8dGlmZjpPcmllbnRhdGlvbj4xPC90aWZmOk9yaWVudGF0aW9uPgogICAgICAgICA8dGlmZjpZUmVzb2x1dGlvbj43MjwvdGlmZjpZUmVzb2x1dGlvbj4KICAgICAgICAgPGV4aWY6UGl4ZWxYRGltZW5zaW9uPjE2PC9leGlmOlBpeGVsWERpbWVuc2lvbj4KICAgICAgICAgPGV4aWY6Q29sb3JTcGFjZT4xPC9leGlmOkNvbG9yU3BhY2U+CiAgICAgICAgIDxleGlmOlBpeGVsWURpbWVuc2lvbj4xNjwvZXhpZjpQaXhlbFlEaW1lbnNpb24+CiAgICAgICAgIDxkYzpzdWJqZWN0PgogICAgICAgICAgICA8cmRmOkJhZy8+CiAgICAgICAgIDwvZGM6c3ViamVjdD4KICAgICAgICAgPHhtcDpNb2RpZnlEYXRlPjIwMTUtMDYtMTBUMjM6MDY6MTM8L3htcDpNb2RpZnlEYXRlPgogICAgICAgICA8eG1wOkNyZWF0b3JUb29sPlBpeGVsbWF0b3IgMy4zLjI8L3htcDpDcmVhdG9yVG9vbD4KICAgICAgPC9yZGY6RGVzY3JpcHRpb24+CiAgIDwvcmRmOlJERj4KPC94OnhtcG1ldGE+Ctn+x9MAAACzSURBVDgRY2AYaMCIywETElP+45IrmD8Hpz5ceuDiIIORDccwCVkSrgvKgNkMUgNjo6vByV8REvkfhEEKYDSIzQIiQABZECKCICPWLGcEYWxqmGDK+H78ZMCFt/kEgW0GyYMAjAax4QaAOOQAuBe4f/4mqB+mBkaDNMAN4Pzzh6ABMDX2uzfDYw9hwC/CLuDEogZuAN+PXxgu+MTBBgwwhLju0T1wm2GK8QYismaYhsFHAwBRtkxjHh0stwAAAABJRU5ErkJggg==";
			return BuildTexture(texString, 16, 16);
		}

		private static Texture2D GetIconBars ()
		{
			string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAGUExURQAAAIWFhZm/ewUAAAABdFJOUwBA5thmAAAAE0lEQVQI12NggAH5PyCEn83AAACcyQajSOB9uwAAAABJRU5ErkJggg==";
			return BuildTexture(texString, 16, 16);
		}

		private static Texture2D GetIconPublisher ()
		{
			//string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQBAMAAADt3eJSAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAYUExURQAAAIWFhYSEhISEhIWFhYWFhYSEhJCQkBvTjdsAAAAHdFJOUwD+IHPQTKIrKkW6AAAAYUlEQVQI12NgQAJsjg5gmilUSQzMUCkXUjQACbgLFiqDGMyFguIJMEaSApDBChIBKzYUFFNVABtT6KAiDFITUuhqEA5UnQ5UU8JaAJQQFBQUd3BgYBcEgVIFBkYwQxThBADIGg5QTbvBDwAAAABJRU5ErkJggg==";
			string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAALEwAACxMBAJqcGAAABCRpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IlhNUCBDb3JlIDUuNC4wIj4KICAgPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4KICAgICAgPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIKICAgICAgICAgICAgeG1sbnM6dGlmZj0iaHR0cDovL25zLmFkb2JlLmNvbS90aWZmLzEuMC8iCiAgICAgICAgICAgIHhtbG5zOmV4aWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20vZXhpZi8xLjAvIgogICAgICAgICAgICB4bWxuczpkYz0iaHR0cDovL3B1cmwub3JnL2RjL2VsZW1lbnRzLzEuMS8iCiAgICAgICAgICAgIHhtbG5zOnhtcD0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wLyI+CiAgICAgICAgIDx0aWZmOlJlc29sdXRpb25Vbml0PjI8L3RpZmY6UmVzb2x1dGlvblVuaXQ+CiAgICAgICAgIDx0aWZmOkNvbXByZXNzaW9uPjE8L3RpZmY6Q29tcHJlc3Npb24+CiAgICAgICAgIDx0aWZmOlhSZXNvbHV0aW9uPjcyPC90aWZmOlhSZXNvbHV0aW9uPgogICAgICAgICA8dGlmZjpPcmllbnRhdGlvbj4xPC90aWZmOk9yaWVudGF0aW9uPgogICAgICAgICA8dGlmZjpZUmVzb2x1dGlvbj43MjwvdGlmZjpZUmVzb2x1dGlvbj4KICAgICAgICAgPGV4aWY6UGl4ZWxYRGltZW5zaW9uPjE2PC9leGlmOlBpeGVsWERpbWVuc2lvbj4KICAgICAgICAgPGV4aWY6Q29sb3JTcGFjZT4xPC9leGlmOkNvbG9yU3BhY2U+CiAgICAgICAgIDxleGlmOlBpeGVsWURpbWVuc2lvbj4xNjwvZXhpZjpQaXhlbFlEaW1lbnNpb24+CiAgICAgICAgIDxkYzpzdWJqZWN0PgogICAgICAgICAgICA8cmRmOkJhZy8+CiAgICAgICAgIDwvZGM6c3ViamVjdD4KICAgICAgICAgPHhtcDpNb2RpZnlEYXRlPjIwMTUtMDYtMTBUMjM6MDY6OTM8L3htcDpNb2RpZnlEYXRlPgogICAgICAgICA8eG1wOkNyZWF0b3JUb29sPlBpeGVsbWF0b3IgMy4zLjI8L3htcDpDcmVhdG9yVG9vbD4KICAgICAgPC9yZGY6RGVzY3JpcHRpb24+CiAgIDwvcmRmOlJERj4KPC94OnhtcG1ldGE+CjetbyMAAAFrSURBVDgRpVI9S8RAEM0mkiJJZ2Mp1nL6B9KltBCsLNS/EJAr8oGF5KMQ/4OIrSBondZKULGxEQsb+6QKSW7eXkY2uUMOXcjO7Mx7b2dnomn/XOI3fpIk15SfCCEmXde9xnG8O8YvFcjzfJMI2wAT+Q22bdsPit2QyDHOvBYEQG6a5s5xnB2AqqrqdF3fgg8RMtMoii5xxtLnZr6rZBDx2bYteiJAUxUPfyBAwAPcDCIDWYSqOldv5vwaO6tYVEi4WxU7EKCGvavJsR8Ewec4ttDENE1PCXShAtF9wzDOMBlMRRUa9AAkvBMENA9n8jF/OTrqUYKG9pcgrRly7zckPM9bJ9KhaZobdV0j802xL4q5JHRC/hOJX7muWxZF8fjzBPx1NIEjMNQp4MzVlGX5gr8xy7I9y7Lufd8X8glMBnFMZkHEMWLCPodh+AAx5OQUcPMyIgDqYhFUQM3cR05WsAqZhRirToJzf7IzMhbInKijdR4AAAAASUVORK5CYII=";
			return BuildTexture(texString, 16, 16);
		}

		private static Texture2D GetIconPublisherX ()
		{
			//string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQBAMAAADt3eJSAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAYUExURQAAALNLUJ1TWZdbX848ReEyPbZLUZZdYNnI0vIAAAAFdFJOUwBgILPGfO8trAAAAF1JREFUCNdjYEACzOYGYJpJUKkYzFAuL1IXAAmYl5crghiM5eXlBmBGWlqakQKUAdEllpasqAA2Js1BJREkJZLmKCAGVO0aGhoawhjAwBIKAg4ODKxgRqACjIFwAgC//xOw3Ob9FwAAAABJRU5ErkJggg==";
			string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAALEwAACxMBAJqcGAAABCRpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IlhNUCBDb3JlIDUuNC4wIj4KICAgPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4KICAgICAgPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIKICAgICAgICAgICAgeG1sbnM6dGlmZj0iaHR0cDovL25zLmFkb2JlLmNvbS90aWZmLzEuMC8iCiAgICAgICAgICAgIHhtbG5zOmV4aWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20vZXhpZi8xLjAvIgogICAgICAgICAgICB4bWxuczpkYz0iaHR0cDovL3B1cmwub3JnL2RjL2VsZW1lbnRzLzEuMS8iCiAgICAgICAgICAgIHhtbG5zOnhtcD0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wLyI+CiAgICAgICAgIDx0aWZmOlJlc29sdXRpb25Vbml0PjI8L3RpZmY6UmVzb2x1dGlvblVuaXQ+CiAgICAgICAgIDx0aWZmOkNvbXByZXNzaW9uPjE8L3RpZmY6Q29tcHJlc3Npb24+CiAgICAgICAgIDx0aWZmOlhSZXNvbHV0aW9uPjcyPC90aWZmOlhSZXNvbHV0aW9uPgogICAgICAgICA8dGlmZjpPcmllbnRhdGlvbj4xPC90aWZmOk9yaWVudGF0aW9uPgogICAgICAgICA8dGlmZjpZUmVzb2x1dGlvbj43MjwvdGlmZjpZUmVzb2x1dGlvbj4KICAgICAgICAgPGV4aWY6UGl4ZWxYRGltZW5zaW9uPjE2PC9leGlmOlBpeGVsWERpbWVuc2lvbj4KICAgICAgICAgPGV4aWY6Q29sb3JTcGFjZT4xPC9leGlmOkNvbG9yU3BhY2U+CiAgICAgICAgIDxleGlmOlBpeGVsWURpbWVuc2lvbj4xNjwvZXhpZjpQaXhlbFlEaW1lbnNpb24+CiAgICAgICAgIDxkYzpzdWJqZWN0PgogICAgICAgICAgICA8cmRmOkJhZy8+CiAgICAgICAgIDwvZGM6c3ViamVjdD4KICAgICAgICAgPHhtcDpNb2RpZnlEYXRlPjIwMTUtMDYtMTBUMjM6MDY6ODM8L3htcDpNb2RpZnlEYXRlPgogICAgICAgICA8eG1wOkNyZWF0b3JUb29sPlBpeGVsbWF0b3IgMy4zLjI8L3htcDpDcmVhdG9yVG9vbD4KICAgICAgPC9yZGY6RGVzY3JpcHRpb24+CiAgIDwvcmRmOlJERj4KPC94OnhtcG1ldGE+CipnGj0AAAFNSURBVDgRY2CgEDDi0z89On4zSP4/E6MP47//WzKXLvRFV4/VgPkBEQrCv344gBS/ZeM4AKK/83Lcx2YIhgEgzT+42SeDbAVpBAHOzz8UQTTIEJm3HxL9tm1YAOKDABOEgpDYNINkQBpBNEgziEYGKAaAnI1sM7JCkKuQbYbJscAYMFrh3UcYE4V+KMDHAHLhWwaGA8gSGAYgS6KzEzeseIAuhhGIm7wCElgYGecjKwTavoXj689ckBdBsYJsEEoYgDSB/AnSADNA4tPX47D4l/n8LU3898/7IEtg8igGgCRA2PzRc2GYAhANEgPZbnR4l9Wf//8TQS6EGQL3wjlbt2PCX79bImtEZ7/l5jwOMgSkWf/F2/ny5w4zgl1AjGaQYSALQGpB3gQZBhIDxwIhm0EKYQCkFuSCi2wcUSAxlDCAKSKGRo4JYtTjVAMAfUOMR9GNuIwAAAAASUVORK5CYII=";
			return BuildTexture(texString, 16, 16);
		}

		private static Texture2D GetIconPrev ()
		{
			//string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAGUExURX9/f3R0dFgOCFQAAAACdFJOUwLw+RhdpgAAACNJREFUCNdjYGBgYGxgYD7AwP6Agf8Dg/wPBvs/IDZWxMAAACU8DLUcFXzMAAAAAElFTkSuQmCC";
			string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAALEwAACxMBAJqcGAAABCRpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IlhNUCBDb3JlIDUuNC4wIj4KICAgPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4KICAgICAgPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIKICAgICAgICAgICAgeG1sbnM6dGlmZj0iaHR0cDovL25zLmFkb2JlLmNvbS90aWZmLzEuMC8iCiAgICAgICAgICAgIHhtbG5zOmV4aWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20vZXhpZi8xLjAvIgogICAgICAgICAgICB4bWxuczpkYz0iaHR0cDovL3B1cmwub3JnL2RjL2VsZW1lbnRzLzEuMS8iCiAgICAgICAgICAgIHhtbG5zOnhtcD0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wLyI+CiAgICAgICAgIDx0aWZmOlJlc29sdXRpb25Vbml0PjI8L3RpZmY6UmVzb2x1dGlvblVuaXQ+CiAgICAgICAgIDx0aWZmOkNvbXByZXNzaW9uPjE8L3RpZmY6Q29tcHJlc3Npb24+CiAgICAgICAgIDx0aWZmOlhSZXNvbHV0aW9uPjcyPC90aWZmOlhSZXNvbHV0aW9uPgogICAgICAgICA8dGlmZjpPcmllbnRhdGlvbj4xPC90aWZmOk9yaWVudGF0aW9uPgogICAgICAgICA8dGlmZjpZUmVzb2x1dGlvbj43MjwvdGlmZjpZUmVzb2x1dGlvbj4KICAgICAgICAgPGV4aWY6UGl4ZWxYRGltZW5zaW9uPjE2PC9leGlmOlBpeGVsWERpbWVuc2lvbj4KICAgICAgICAgPGV4aWY6Q29sb3JTcGFjZT4xPC9leGlmOkNvbG9yU3BhY2U+CiAgICAgICAgIDxleGlmOlBpeGVsWURpbWVuc2lvbj4xNjwvZXhpZjpQaXhlbFlEaW1lbnNpb24+CiAgICAgICAgIDxkYzpzdWJqZWN0PgogICAgICAgICAgICA8cmRmOkJhZy8+CiAgICAgICAgIDwvZGM6c3ViamVjdD4KICAgICAgICAgPHhtcDpNb2RpZnlEYXRlPjIwMTUtMDYtMTBUMjE6MDY6NTM8L3htcDpNb2RpZnlEYXRlPgogICAgICAgICA8eG1wOkNyZWF0b3JUb29sPlBpeGVsbWF0b3IgMy4zLjI8L3htcDpDcmVhdG9yVG9vbD4KICAgICAgPC9yZGY6RGVzY3JpcHRpb24+CiAgIDwvcmRmOlJERj4KPC94OnhtcG1ldGE+CrVsGGQAAAB0SURBVDgRY2xoaGBioADg1fzly5d3IIzPfJwGIGtEZqMbhtUAbBqwiYEMwzAAl0KQYmxyKAZgUwDSiAzQ1cANQJdA1oTORlbLApPk4eERgrGRFcDEQDSyGpg43AUwAVLpUQOwJCS6ByI8HSDbjC2+keWR2QBBUjOKATyNgwAAAABJRU5ErkJggg==";
			return BuildTexture(texString, 16, 16);
		}

		private static Texture2D GetIconNext ()
		{
			//string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAQMAAAAlPW0iAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAGUExURX9/f3R0dFgOCFQAAAACdFJOUwLw+RhdpgAAACFJREFUCNdjYGBgYH+AHdn/YZD/wcD/AcRmPsDA2ABUCwAxrgy1ogTUHgAAAABJRU5ErkJggg==";
			string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAALEwAACxMBAJqcGAAABCRpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IlhNUCBDb3JlIDUuNC4wIj4KICAgPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4KICAgICAgPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIKICAgICAgICAgICAgeG1sbnM6dGlmZj0iaHR0cDovL25zLmFkb2JlLmNvbS90aWZmLzEuMC8iCiAgICAgICAgICAgIHhtbG5zOmV4aWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20vZXhpZi8xLjAvIgogICAgICAgICAgICB4bWxuczpkYz0iaHR0cDovL3B1cmwub3JnL2RjL2VsZW1lbnRzLzEuMS8iCiAgICAgICAgICAgIHhtbG5zOnhtcD0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wLyI+CiAgICAgICAgIDx0aWZmOlJlc29sdXRpb25Vbml0PjI8L3RpZmY6UmVzb2x1dGlvblVuaXQ+CiAgICAgICAgIDx0aWZmOkNvbXByZXNzaW9uPjE8L3RpZmY6Q29tcHJlc3Npb24+CiAgICAgICAgIDx0aWZmOlhSZXNvbHV0aW9uPjcyPC90aWZmOlhSZXNvbHV0aW9uPgogICAgICAgICA8dGlmZjpPcmllbnRhdGlvbj4xPC90aWZmOk9yaWVudGF0aW9uPgogICAgICAgICA8dGlmZjpZUmVzb2x1dGlvbj43MjwvdGlmZjpZUmVzb2x1dGlvbj4KICAgICAgICAgPGV4aWY6UGl4ZWxYRGltZW5zaW9uPjE2PC9leGlmOlBpeGVsWERpbWVuc2lvbj4KICAgICAgICAgPGV4aWY6Q29sb3JTcGFjZT4xPC9leGlmOkNvbG9yU3BhY2U+CiAgICAgICAgIDxleGlmOlBpeGVsWURpbWVuc2lvbj4xNjwvZXhpZjpQaXhlbFlEaW1lbnNpb24+CiAgICAgICAgIDxkYzpzdWJqZWN0PgogICAgICAgICAgICA8cmRmOkJhZy8+CiAgICAgICAgIDwvZGM6c3ViamVjdD4KICAgICAgICAgPHhtcDpNb2RpZnlEYXRlPjIwMTUtMDYtMTBUMjE6MDY6ODU8L3htcDpNb2RpZnlEYXRlPgogICAgICAgICA8eG1wOkNyZWF0b3JUb29sPlBpeGVsbWF0b3IgMy4zLjI8L3htcDpDcmVhdG9yVG9vbD4KICAgICAgPC9yZGY6RGVzY3JpcHRpb24+CiAgIDwvcmRmOlJERj4KPC94OnhtcG1ldGE+CuXyC9MAAABvSURBVDgRY2xoaGBioACwYNP75cuXd9jEeXh4hNDFKbIdZNioAYMhDODpAFfcI8c7shpYmoBHI0wAWQMuNrJauAEgxcgSxGgGqUExACSAzxBschgG4DIEm2aQWqwGoBuCSzNIHTwWQBx0gE8jTC0AJ2YPxN1VFewAAAAASUVORK5CYII=";
			return BuildTexture(texString, 16, 16);
		}

		// Collection Icons

		private static Texture2D GetIconProject ()
		{
			//string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQBAMAAADt3eJSAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAYUExURQAAAHh4eKuqq6ioqLGxsba2tqalpq6trmKXDq4AAAABdFJOUwBA5thmAAAAPUlEQVQI12NggANGQUFBATAjNDQwEMYwBgkxurg4CkIZLi5gRjkQgBlKQABmGAMBKiMNCFBF4NoFYZahAQD1mg0ZZX/t4AAAAABJRU5ErkJggg==";
			string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAALEwAACxMBAJqcGAAABCRpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IlhNUCBDb3JlIDUuNC4wIj4KICAgPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4KICAgICAgPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIKICAgICAgICAgICAgeG1sbnM6dGlmZj0iaHR0cDovL25zLmFkb2JlLmNvbS90aWZmLzEuMC8iCiAgICAgICAgICAgIHhtbG5zOmV4aWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20vZXhpZi8xLjAvIgogICAgICAgICAgICB4bWxuczpkYz0iaHR0cDovL3B1cmwub3JnL2RjL2VsZW1lbnRzLzEuMS8iCiAgICAgICAgICAgIHhtbG5zOnhtcD0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wLyI+CiAgICAgICAgIDx0aWZmOlJlc29sdXRpb25Vbml0PjI8L3RpZmY6UmVzb2x1dGlvblVuaXQ+CiAgICAgICAgIDx0aWZmOkNvbXByZXNzaW9uPjE8L3RpZmY6Q29tcHJlc3Npb24+CiAgICAgICAgIDx0aWZmOlhSZXNvbHV0aW9uPjcyPC90aWZmOlhSZXNvbHV0aW9uPgogICAgICAgICA8dGlmZjpPcmllbnRhdGlvbj4xPC90aWZmOk9yaWVudGF0aW9uPgogICAgICAgICA8dGlmZjpZUmVzb2x1dGlvbj43MjwvdGlmZjpZUmVzb2x1dGlvbj4KICAgICAgICAgPGV4aWY6UGl4ZWxYRGltZW5zaW9uPjE2PC9leGlmOlBpeGVsWERpbWVuc2lvbj4KICAgICAgICAgPGV4aWY6Q29sb3JTcGFjZT4xPC9leGlmOkNvbG9yU3BhY2U+CiAgICAgICAgIDxleGlmOlBpeGVsWURpbWVuc2lvbj4xNjwvZXhpZjpQaXhlbFlEaW1lbnNpb24+CiAgICAgICAgIDxkYzpzdWJqZWN0PgogICAgICAgICAgICA8cmRmOkJhZy8+CiAgICAgICAgIDwvZGM6c3ViamVjdD4KICAgICAgICAgPHhtcDpNb2RpZnlEYXRlPjIwMTUtMDYtMTBUMjM6MDY6NTI8L3htcDpNb2RpZnlEYXRlPgogICAgICAgICA8eG1wOkNyZWF0b3JUb29sPlBpeGVsbWF0b3IgMy4zLjI8L3htcDpDcmVhdG9yVG9vbD4KICAgICAgPC9yZGY6RGVzY3JpcHRpb24+CiAgIDwvcmRmOlJERj4KPC94OnhtcG1ldGE+CtOyabsAAACXSURBVDgRzZExDsMgDEXdpAu3YWXhXByBc7GwchZWCokiVVHib6uJspTJPD4PWxA9XK/j/RDCctzzOsZ4ym/nbx7y3nNEKSX68oVLJkgroNZK1lriXUIHrTVRUUoROQiklHNuxznnvd4KEPRPPwV+bVDQnwrGPcHlX9BGgRFGH1pW5CCY5ntNgcAYI76kQRDwf9Yu/g9fASkXJwMe3lfNAAAAAElFTkSuQmCC";
			return BuildTexture(texString, 16, 16);
		}

		private static Texture2D GetIconStandard ()
		{
			//string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQBAMAAADt3eJSAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAASUExURQAAAJdKn61gtbtuw6RXrK5htn+q0hkAAAABdFJOUwBA5thmAAAAOUlEQVQI12NgwAeYBB1YBBWADEZBBSZBAQiDFchgFjQUFFIEMhgFwQCJAZICMxggYigMJhBDAY+NAESmBHfDB/8IAAAAAElFTkSuQmCC";
			string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAAlwSFlzAAALEwAACxMBAJqcGAAABCRpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IlhNUCBDb3JlIDUuNC4wIj4KICAgPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4KICAgICAgPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIKICAgICAgICAgICAgeG1sbnM6dGlmZj0iaHR0cDovL25zLmFkb2JlLmNvbS90aWZmLzEuMC8iCiAgICAgICAgICAgIHhtbG5zOmV4aWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20vZXhpZi8xLjAvIgogICAgICAgICAgICB4bWxuczpkYz0iaHR0cDovL3B1cmwub3JnL2RjL2VsZW1lbnRzLzEuMS8iCiAgICAgICAgICAgIHhtbG5zOnhtcD0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wLyI+CiAgICAgICAgIDx0aWZmOlJlc29sdXRpb25Vbml0PjI8L3RpZmY6UmVzb2x1dGlvblVuaXQ+CiAgICAgICAgIDx0aWZmOkNvbXByZXNzaW9uPjE8L3RpZmY6Q29tcHJlc3Npb24+CiAgICAgICAgIDx0aWZmOlhSZXNvbHV0aW9uPjcyPC90aWZmOlhSZXNvbHV0aW9uPgogICAgICAgICA8dGlmZjpPcmllbnRhdGlvbj4xPC90aWZmOk9yaWVudGF0aW9uPgogICAgICAgICA8dGlmZjpZUmVzb2x1dGlvbj43MjwvdGlmZjpZUmVzb2x1dGlvbj4KICAgICAgICAgPGV4aWY6UGl4ZWxYRGltZW5zaW9uPjE2PC9leGlmOlBpeGVsWERpbWVuc2lvbj4KICAgICAgICAgPGV4aWY6Q29sb3JTcGFjZT4xPC9leGlmOkNvbG9yU3BhY2U+CiAgICAgICAgIDxleGlmOlBpeGVsWURpbWVuc2lvbj4xNjwvZXhpZjpQaXhlbFlEaW1lbnNpb24+CiAgICAgICAgIDxkYzpzdWJqZWN0PgogICAgICAgICAgICA8cmRmOkJhZy8+CiAgICAgICAgIDwvZGM6c3ViamVjdD4KICAgICAgICAgPHhtcDpNb2RpZnlEYXRlPjIwMTUtMDYtMTBUMjA6MDY6ODk8L3htcDpNb2RpZnlEYXRlPgogICAgICAgICA8eG1wOkNyZWF0b3JUb29sPlBpeGVsbWF0b3IgMy4zLjI8L3htcDpDcmVhdG9yVG9vbD4KICAgICAgPC9yZGY6RGVzY3JpcHRpb24+CiAgIDwvcmRmOlJERj4KPC94OnhtcG1ldGE+Cna6PxcAAAC2SURBVDgRY2AY8oARmw+muM/5/+rVK7AULy8vQ+mhXLC6brvJ/z9//gwWFxMTY8jZmcLIhM0AmGaQHCcnJ1wJMhumBqsBcB1oDEZGTAezgNQsCl31/86dO2DlKioqDDA2SABkE8hLIPbLly9BFAoAG4CsAZkNUwlzLoyPTJPkBWSNMDbYAJCzYQCZDRPDR2OGClB1nWEb2M/4NILkms5XYY9GQhqR5bGGASiREALEqCFkxiCRBwDX8jExUuX4cgAAAABJRU5ErkJggg==";
			return BuildTexture(texString, 16, 16);
		}

		private static Texture2D GetIconUser ()
		{
			//string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAgMAAABinRfyAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAMUExURQAAAFpaxZmZ28DA6Rz/hPwAAAABdFJOUwBA5thmAAAAMklEQVQI12NgAAPOBCChGgEkREMwCbAEWAkIME8NO8DAGRqawKAaGhrBIBoaGgIhQAAAKeIJf6MR6/kAAAAASUVORK5CYII=";
			string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAAlwSFlzAAALEwAACxMBAJqcGAAABCRpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IlhNUCBDb3JlIDUuNC4wIj4KICAgPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4KICAgICAgPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIKICAgICAgICAgICAgeG1sbnM6dGlmZj0iaHR0cDovL25zLmFkb2JlLmNvbS90aWZmLzEuMC8iCiAgICAgICAgICAgIHhtbG5zOmV4aWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20vZXhpZi8xLjAvIgogICAgICAgICAgICB4bWxuczpkYz0iaHR0cDovL3B1cmwub3JnL2RjL2VsZW1lbnRzLzEuMS8iCiAgICAgICAgICAgIHhtbG5zOnhtcD0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wLyI+CiAgICAgICAgIDx0aWZmOlJlc29sdXRpb25Vbml0PjI8L3RpZmY6UmVzb2x1dGlvblVuaXQ+CiAgICAgICAgIDx0aWZmOkNvbXByZXNzaW9uPjE8L3RpZmY6Q29tcHJlc3Npb24+CiAgICAgICAgIDx0aWZmOlhSZXNvbHV0aW9uPjcyPC90aWZmOlhSZXNvbHV0aW9uPgogICAgICAgICA8dGlmZjpPcmllbnRhdGlvbj4xPC90aWZmOk9yaWVudGF0aW9uPgogICAgICAgICA8dGlmZjpZUmVzb2x1dGlvbj43MjwvdGlmZjpZUmVzb2x1dGlvbj4KICAgICAgICAgPGV4aWY6UGl4ZWxYRGltZW5zaW9uPjE2PC9leGlmOlBpeGVsWERpbWVuc2lvbj4KICAgICAgICAgPGV4aWY6Q29sb3JTcGFjZT4xPC9leGlmOkNvbG9yU3BhY2U+CiAgICAgICAgIDxleGlmOlBpeGVsWURpbWVuc2lvbj4xNjwvZXhpZjpQaXhlbFlEaW1lbnNpb24+CiAgICAgICAgIDxkYzpzdWJqZWN0PgogICAgICAgICAgICA8cmRmOkJhZy8+CiAgICAgICAgIDwvZGM6c3ViamVjdD4KICAgICAgICAgPHhtcDpNb2RpZnlEYXRlPjIwMTUtMDYtMTBUMjA6MDY6MDE8L3htcDpNb2RpZnlEYXRlPgogICAgICAgICA8eG1wOkNyZWF0b3JUb29sPlBpeGVsbWF0b3IgMy4zLjI8L3htcDpDcmVhdG9yVG9vbD4KICAgICAgPC9yZGY6RGVzY3JpcHRpb24+CiAgIDwvcmRmOlJERj4KPC94OnhtcG1ldGE+CsUgS+UAAACUSURBVDgRY2CgEDDi0j9z5u3/Bw++Akvb24sxpKerYlWLVRBZM8wCXIYwwRQg0zCbCYmB5LEagKyREHuQGgAKMHSATQykBmssgCSQYwJXDIDUUQxQXHDgwMv/N29+YsAWjSCbQC5RV+djcHAQh+uDM5CdTMhZyF4CG0CKZpjhMEPA6QCXk2GKsdEwPRQnJGyGkyQGAK3xN+3h6WLSAAAAAElFTkSuQmCC";
			return BuildTexture(texString, 16, 16);
		}

		private static Texture2D GetIconStore ()
		{
			//string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQBAMAAADt3eJSAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAVUExURQAAAFWPVV+fX0yATIGzgW+pb5C8kIjhYxQAAAABdFJOUwBA5thmAAAAS0lEQVQI12NgYGBwUmEAAyclJRUoDWYxKYGBAgMrRCSBgdkUpCZRgIHZ2NRJJVEQxDA2TRSEMAwFiWUwBIMYYiDLgg0hNANDIIgGADMSDmkPhb0oAAAAAElFTkSuQmCC";
			string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAAlwSFlzAAALEwAACxMBAJqcGAAABCRpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IlhNUCBDb3JlIDUuNC4wIj4KICAgPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4KICAgICAgPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIKICAgICAgICAgICAgeG1sbnM6dGlmZj0iaHR0cDovL25zLmFkb2JlLmNvbS90aWZmLzEuMC8iCiAgICAgICAgICAgIHhtbG5zOmV4aWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20vZXhpZi8xLjAvIgogICAgICAgICAgICB4bWxuczpkYz0iaHR0cDovL3B1cmwub3JnL2RjL2VsZW1lbnRzLzEuMS8iCiAgICAgICAgICAgIHhtbG5zOnhtcD0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wLyI+CiAgICAgICAgIDx0aWZmOlJlc29sdXRpb25Vbml0PjI8L3RpZmY6UmVzb2x1dGlvblVuaXQ+CiAgICAgICAgIDx0aWZmOkNvbXByZXNzaW9uPjE8L3RpZmY6Q29tcHJlc3Npb24+CiAgICAgICAgIDx0aWZmOlhSZXNvbHV0aW9uPjcyPC90aWZmOlhSZXNvbHV0aW9uPgogICAgICAgICA8dGlmZjpPcmllbnRhdGlvbj4xPC90aWZmOk9yaWVudGF0aW9uPgogICAgICAgICA8dGlmZjpZUmVzb2x1dGlvbj43MjwvdGlmZjpZUmVzb2x1dGlvbj4KICAgICAgICAgPGV4aWY6UGl4ZWxYRGltZW5zaW9uPjE2PC9leGlmOlBpeGVsWERpbWVuc2lvbj4KICAgICAgICAgPGV4aWY6Q29sb3JTcGFjZT4xPC9leGlmOkNvbG9yU3BhY2U+CiAgICAgICAgIDxleGlmOlBpeGVsWURpbWVuc2lvbj4xNjwvZXhpZjpQaXhlbFlEaW1lbnNpb24+CiAgICAgICAgIDxkYzpzdWJqZWN0PgogICAgICAgICAgICA8cmRmOkJhZy8+CiAgICAgICAgIDwvZGM6c3ViamVjdD4KICAgICAgICAgPHhtcDpNb2RpZnlEYXRlPjIwMTUtMDYtMTBUMjA6MDY6MDk8L3htcDpNb2RpZnlEYXRlPgogICAgICAgICA8eG1wOkNyZWF0b3JUb29sPlBpeGVsbWF0b3IgMy4zLjI8L3htcDpDcmVhdG9yVG9vbD4KICAgICAgPC9yZGY6RGVzY3JpcHRpb24+CiAgIDwvcmRmOlJERj4KPC94OnhtcG1ldGE+Cpjpl+cAAADZSURBVDgRY2TAARo3N/6/9+YeWFZJRImh3reeEZtSDEFkjegasBkENwCfRnwGgQ2Inx//H10RMfyFiQsZmUAKBbgEGEDOIxaA1BrKGoKVs4DIu9fvMoCgsqYygxCXEAMs8NANBGnk5+RnOHr5KFwKbACMh8sgbBphelAMgAkiG6QorIhiI0wNjMZqAEzy2bNnDCCMD4ADEZ8CQnLDxYAtDVsYQWmAWGCta82wunA1OBXD8wJMc/7K/P+gaAQBTn5OmDCYBmkscClA0YPCQVYNMggWhdg0wtQCAFYnTucqsnaIAAAAAElFTkSuQmCC";
			return BuildTexture(texString, 16, 16);
		}

		private static Texture2D GetIconOld ()
		{
			//string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQBAMAAADt3eJSAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAPUExURQAAAJEfH6klJrwoKJ6dn6zOSNcAAAABdFJOUwBA5thmAAAAOUlEQVQI12NgQABmQUEDKA1hGYIYwkCGIBhAZUByjEJKQKAoAGS4AAEqAyoFVwzXjjAQbgXcUigAAH3pCOdEJ/4uAAAAAElFTkSuQmCC";
			string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAAlwSFlzAAALEwAACxMBAJqcGAAABCRpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IlhNUCBDb3JlIDUuNC4wIj4KICAgPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4KICAgICAgPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIKICAgICAgICAgICAgeG1sbnM6dGlmZj0iaHR0cDovL25zLmFkb2JlLmNvbS90aWZmLzEuMC8iCiAgICAgICAgICAgIHhtbG5zOmV4aWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20vZXhpZi8xLjAvIgogICAgICAgICAgICB4bWxuczpkYz0iaHR0cDovL3B1cmwub3JnL2RjL2VsZW1lbnRzLzEuMS8iCiAgICAgICAgICAgIHhtbG5zOnhtcD0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wLyI+CiAgICAgICAgIDx0aWZmOlJlc29sdXRpb25Vbml0PjI8L3RpZmY6UmVzb2x1dGlvblVuaXQ+CiAgICAgICAgIDx0aWZmOkNvbXByZXNzaW9uPjE8L3RpZmY6Q29tcHJlc3Npb24+CiAgICAgICAgIDx0aWZmOlhSZXNvbHV0aW9uPjcyPC90aWZmOlhSZXNvbHV0aW9uPgogICAgICAgICA8dGlmZjpPcmllbnRhdGlvbj4xPC90aWZmOk9yaWVudGF0aW9uPgogICAgICAgICA8dGlmZjpZUmVzb2x1dGlvbj43MjwvdGlmZjpZUmVzb2x1dGlvbj4KICAgICAgICAgPGV4aWY6UGl4ZWxYRGltZW5zaW9uPjE2PC9leGlmOlBpeGVsWERpbWVuc2lvbj4KICAgICAgICAgPGV4aWY6Q29sb3JTcGFjZT4xPC9leGlmOkNvbG9yU3BhY2U+CiAgICAgICAgIDxleGlmOlBpeGVsWURpbWVuc2lvbj4xNjwvZXhpZjpQaXhlbFlEaW1lbnNpb24+CiAgICAgICAgIDxkYzpzdWJqZWN0PgogICAgICAgICAgICA8cmRmOkJhZy8+CiAgICAgICAgIDwvZGM6c3ViamVjdD4KICAgICAgICAgPHhtcDpNb2RpZnlEYXRlPjIwMTUtMDYtMTBUMjA6MDY6NzA8L3htcDpNb2RpZnlEYXRlPgogICAgICAgICA8eG1wOkNyZWF0b3JUb29sPlBpeGVsbWF0b3IgMy4zLjI8L3htcDpDcmVhdG9yVG9vbD4KICAgICAgPC9yZGY6RGVzY3JpcHRpb24+CiAgIDwvcmRmOlJERj4KPC94OnhtcG1ldGE+Cukyeq8AAACvSURBVDgRrZLRDcMgDERJFanKJpGyRlZhqqySNSKxCcpX2qM6ZINViFo+Enz4nm2Ecz+uwfLv83wdMaqjZZrcGkKVrwTLqCjvoARlQI+ZMAl5UCxbpm79ZW4CoLqV+E2jZ0QSiWitZyGfngSQpue2ybDan94rLd+BUm8EVQdlhRYrATC7nKtlwjnvK41gvbAWhJ58ByS2jLI69vklIuh5jSjE6hUAApYFKo2fzD98X2PARGldSSTFAAAAAElFTkSuQmCC";
			return BuildTexture(texString, 16, 16);
		}

		private static Texture2D GetIconStandardAlt ()
		{
			string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAZ0lEQVQ4y2NgGJ6gzqC1oc6w7T8I1xq0PoCJg9gwcZAa3AZAFUANOIBkwAFkObwGIBuE7jJkNRhORlEEEwfRSGxMA3AowmY4uisJGkDIm3i9QLQBJEkSawA+bxCORmQv4cJEhNMQAQD7JZC/Q0ALQwAAAABJRU5ErkJggg==";
			return BuildTexture(texString, 16, 16);
		}

		private static Texture2D GetIconUserAlt ()
		{
			string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAgMAAABinRfyAAAADFBMVEUAAABaWsNZWcNaWsVbjQuMAAAAA3RSTlMAgMBakZ4lAAAAN0lEQVQIW2NgAAP2C0BCtgRIWOgACRMZGAHmgiXASsAg/yeIB+TLMjCUMJgwMMgw2P///wcsBwBXVwtp7rDqZgAAAABJRU5ErkJggg==";
			//string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQBAMAAADt3eJSAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAPUExURQAAAFlZw7i4uFhYw1paxd2lGLwAAAAEdFJOUwCAf8BhZdqTAAAAQUlEQVQI12NgQAIijhCa0VFEAMxgMVJ2gDCEFNEZMCm4Yrh2OBB2cTGESCgpgSQZgTSQJQA0BcQAmsTiAgYOcC0A+goJGmvUYNgAAAAASUVORK5CYII=";
			return BuildTexture(texString, 16, 16);
		}

		private static Texture2D GetIconStoreAlt ()
		{
			string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAm0lEQVR42uWRwQnDMBAEU6Ce0lfuwG7hUkBsNxDnGdJFSkklVhgwCK9xTv8sHDp0u8MJXc6U79m6R1coep27wbzkNz1FLyA/qPOfoHANK4PWLfGmOa0VYKEMr6G+19kSb5pS2QE44y2agjSIh7sjYJOCNIgcQJUGmwFq+hfAVuYB+mdv3McxfnYDwgrCqEFKf+YUhLk5qMLcEvwCZkHFNptBydUAAAAASUVORK5CYII=";
			//string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQBAMAAADt3eJSAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAkUExURbe3twAAAFycXFOMU0t/S16dXmynbFSOVFWPVUyATF+fX2+pb0u0UTMAAAAIdFJOUxkAwMDA78D7GYgLjwAAAGFJREFUCNdjEBQU1FoEJBiAdJDqIhBDK4iBAchikFzAAARcjQySuxQYGJhWAxnMuxSYVhsAGQzMu1YbMIAYDNwGDBAGBwMhxhQQI6OQQdBzCgdDRjPIUs8ZQBrEELQA0oIAUhoWGLzK/7QAAAAASUVORK5CYII=";
			//string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQBAMAAADt3eJSAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAwUExURQAAAF+fX0yATO/v71WPVYGzgW+pb5C8kISEhDY2Nt7e3mZmZkxMTB0dHZeXlz8/P/b/8owAAAABdFJOUwBA5thmAAAAX0lEQVQI12NgYGAIFGUAg0BBQVEoDWYxCoKBAAMbRKSAgUkNpKbEgYFJSS1Q2NjYAMhQUiueewPMUHGebIzO6DkMZRjvgzE6DBgYkoCM3aeADIYkFedVxiAGQ4o7kAAAH5wWVRimgscAAAAASUVORK5CYII=";
			return BuildTexture(texString, 16, 16);
		}

		private static Texture2D GetIconOldAlt ()
		{
			string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAW0lEQVQ4y2NgoAXol5NrmCgv/x8Zg8RI0oisAZc4Vs34bMKrhlhnwgwhLIgHYFhGdCDhshDGOX36dMOZM2f+48MgNch6qGcARV6gOBApjkaqJCSKkzJVMhM5AAB5eawN4m7GaQAAAABJRU5ErkJggg==";
			//string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQBAMAAADt3eJSAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAASUExURQAAALi4uI8dHcvLy5EfH8zMzGyTr3sAAAAEdFJOUwB/gIA6BVKcAAAARElEQVQI12NgQAAmFxcFCK0oBGapKAoKCjkBGU6CQKAClHEEMUQUGFgUQQwhBwYW4VAgMERmwKTgiuHaEQbCrYBbCgUAf2UMfVo4vkYAAAAASUVORK5CYII=";
			//string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQAgMAAABinRfyAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAMUExURQAAAJEfH7woKMPDw1Rmoc8AAAABdFJOUwBA5thmAAAAO0lEQVQI12NgAAPVCAYGptDQBgbO0NAEBtbQ0AAG1dDQCAaxVaumMIj9/w8jQFywBFgJWDFYG9gAEAAA884TEQXAJh8AAAAASUVORK5CYII=";
			return BuildTexture(texString, 16, 16);
		}

		private static Texture2D GetIconBlank ()
		{
			string texString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAW0lEQVQ4y2NgoAXol5NrmCgv/x8Zg8RI0oisAZc4Vs34bMKrhlhnwgwhLIgHYFhGdCDhshDGOX36dMOZM2f+48MgNch6qGcARV6gOBApjkaqJCSKkzJVMhM5AAB5eawN4m7GaQAAAABJRU5ErkJggg==";
			return BuildTexture(texString, 16, 16);
		}

		// -- Styles

		private void SetStyles ()
		{
			//Debug.Log (gyaAbbr + " - Set styles!\n");
			svStyleIcon = new GUIStyle();
			svStyleIcon.alignment = TextAnchor.MiddleRight;
			svStyleIcon.fixedHeight = 14;
			svStyleIcon.normal.background = texTransparent;
			svStyleIcon.hover.background = texTransparent;
			svStyleIcon.active.background = texTransparent;

			svStyleIconLeft = new GUIStyle();
			svStyleIconLeft.alignment = TextAnchor.MiddleLeft;
			svStyleIconLeft.fixedHeight = 14;
			svStyleIconLeft.normal.background = texTransparent;
			svStyleIconLeft.hover.background = texTransparent;
			svStyleIconLeft.active.background = texTransparent;

			// Asset style based on default
			svStyleDefault = new GUIStyle();
			svStyleDefault.alignment = TextAnchor.MiddleLeft;
			svStyleDefault.normal.background = texTransparent;
			//svStyleDefault.focused.background = texSelector;
			//svStyleDefault.onFocused.background = texSelector;
			//svStyleDefault.active.background = texSelector;
			//svStyleDefault.onActive.background = texSelector;
			//svStyleDefault.hover.background = texSelector;
			//svStyleDefault.onHover.background = texSelector;

			svStyleStore = new GUIStyle();
			svStyleStore.alignment = TextAnchor.MiddleLeft;
			svStyleStore.normal.background = texTransparent;

			//svStyleStoreAlt = new GUIStyle();
			//svStyleStoreAlt.alignment = TextAnchor.MiddleLeft;
			//svStyleStoreAlt.normal.background = texTransparent;

			svStyleStandard = new GUIStyle();
			svStyleStandard.alignment = TextAnchor.MiddleLeft;
			svStyleStandard.normal.background = texTransparent;

			svStyleOld = new GUIStyle();
			svStyleOld.alignment = TextAnchor.MiddleLeft;
			svStyleOld.normal.background = texTransparent;

			svStyleOldToMove = new GUIStyle();
			svStyleOldToMove.alignment = TextAnchor.MiddleLeft;
			svStyleOldToMove.normal.background = texTransparent;

			svStyleUser = new GUIStyle();
			svStyleUser.alignment = TextAnchor.MiddleLeft;
			svStyleUser.normal.background = texTransparent;

			svStyleProject = new GUIStyle();
			svStyleProject.alignment = TextAnchor.MiddleLeft;
			svStyleProject.normal.background = texTransparent;

			// Category style
			svStyleSeperator = new GUIStyle();
			svStyleSeperator.alignment = TextAnchor.MiddleLeft;
			svStyleSeperator.fontSize = 9;
			svStyleSeperator.fontStyle = FontStyle.Bold;

			foStyleInfo = new GUIStyle();
			foStyleInfo.alignment = TextAnchor.MiddleLeft;
			foStyleInfo.fontSize = 9;
		}

		private string GetLine(string text, int lineNo)
		{
			string[] lines = text.Replace("\r","").Split('\n');
			return lines.Length >= lineNo ? lines[lineNo-1] : "";
		}

		//byte[] data = FromHex("47-61-74-65-77-61-79-53-65-72-76-65-72");
	    //string s = Encoding.ASCII.GetString(data);
		public static byte[] FromHex(string hex)
		{
			hex = hex.Replace("-", "");
			byte[] raw = new byte[hex.Length / 2];
			for (int i = 0; i < raw.Length; i++)
			{
				raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
			}
			return raw;
		}

		// Convert 0 to EMPTY field for CSV export
		private static string CSVZeroToEmpty(int pInt)
		{
			if (pInt == 0)
			{
				return "";
			}
			else
			{
				return pInt.ToString();
			}
		}

		private static string WrapCSVCell(object obj)
		{
			bool mustQuote = true;
			//bool mustQuote = (str.Contains(",") || str.Contains("\"") || str.Contains("\r") || str.Contains("\n"));

			string str = obj.ToString();
			if (str == null)
				str = "";

			if (mustQuote)
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("\"");

				foreach (char nextChar in str)
				{
					sb.Append(nextChar);
					if (nextChar == '"')
						sb.Append("\"");
				}

				sb.Append("\"");
				return sb.ToString();
			}
			return str;
		}

		// Save Embedded Asset Info as CSV
		private void SaveAsCSV(List<Packages> pkgList, string path)
		{
			var result = new StringBuilder();
			var csvFile = new List<string>();
			TextWriter writer = null;

			csvFile.Add("\"title\",\"link\",\"id\",\"pubdate\",\"version\",\"version_id\",\"unity_version\",\"category_label\",\"category_id\",\"publisher_label\",\"publisher_id\",\"filePath\",\"fileSize\",\"isExported\",\"collection\"");

			foreach (Packages item in pkgList)
			{
				string assetURL = "";
				// Ignore exported (non-Asset Store packages)
				if (!item.isExported)
				{
					// Works for Google Sheets
					// Numbers/Excel may encode # to %23 when clicking/sending the link to the browser
					assetURL = "https://www.assetstore.unity3d.com/#/" + item.link.type + "/" + item.link.id.ToString();
					assetURL = "=HYPERLINK(\"" + assetURL + "\", \"link\")";

				}

				csvFile.Add(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}",
					WrapCSVCell(item.title),
					WrapCSVCell(assetURL),
					//WrapCSVCell(Path.GetFileNameWithoutExtension(item.filePath)),
					WrapCSVCell(CSVZeroToEmpty(item.id)),
					WrapCSVCell(item.pubdate),
					WrapCSVCell(item.version),
					WrapCSVCell(CSVZeroToEmpty(item.version_id)),
					WrapCSVCell(item.unity_version),
					WrapCSVCell(item.category.label),
					WrapCSVCell(CSVZeroToEmpty(item.category.id)),
					WrapCSVCell(item.publisher.label),
					WrapCSVCell(CSVZeroToEmpty(item.publisher.id)),
					WrapCSVCell(item.filePath),
					WrapCSVCell(item.fileSize),
					WrapCSVCell(item.isExported),
					WrapCSVCell(item.collection)
					//WrapCSVCell(item.isDamaged)
					//WrapCSVCell(item.description),
					//WrapCSVCell(item.publishnotes)
				));

			}

			foreach (string line in csvFile)
			{
				result.AppendLine(line);
			}

			try
			{
				writer = new StreamWriter(path);
				writer.Write(result);
			}
			catch (Exception ex)
			{
				Debug.LogWarning(gyaAbbr + " - Exporting Error: \n" + ex.Message);
			}
			finally
			{
				if (writer != null)
					writer.Close();

				Debug.Log(gyaAbbr + " - Exported Asset List as: " + path + "\n");
			}
		}

		// SaveAsCSVGroup - Save Embedded Asset Info as CSV
		private void SaveAsCSVGroup(string path)
		{
			var result = new StringBuilder();
			var csvFile = new List<string>();
			TextWriter writer = null;

			csvFile.Add("\"title\",\"link\",\"version\",\"category_label\"");

			foreach (Packages item in grpData[showGroup])
			{
				string assetURL = "";

				// Ignore exported (non-Asset Store packages)
				if (!item.isExported)
				{
					// Works for Google Sheets, Libre Office
					// Numbers/Excel may encode # to %23 when clicking/sending the link to the browser

					// Asset Link
					assetURL = "https://www.assetstore.unity3d.com/#/" + item.link.type + "/" + item.link.id.ToString();
					// Quotes in title double up
					assetURL = "=HYPERLINK(\"" + assetURL + "\", \"link\")";
				}

				csvFile.Add(string.Format("{0},{1},{2},{3}",
					WrapCSVCell(item.title),
					WrapCSVCell(assetURL),
					WrapCSVCell(item.version),
					WrapCSVCell(item.category.label)
				));

			}

			foreach (string line in csvFile)
			{
				result.AppendLine(line);
			}

			try
			{
				writer = new StreamWriter(path);
				writer.Write(result);
			}
				catch (Exception ex)
				{
					Debug.LogWarning(gyaAbbr + " - Exporting Error: \n" + ex.Message);
				}
				finally
			{
				if (writer != null)
				{
					writer.Close();
				}
				Debug.Log(gyaAbbr + " - Exported Group List as: " + path + "\n");
			}
		}

		//// Version for GYAStore - embed asset icon url for google sheets
		//private void SaveAsCSVGroupGYAStore(string path)
		//{
		//	var result = new StringBuilder();
		//	var csvFile = new List<string>();
		//	TextWriter writer = null;

		//	//csvFile.Add("\"title\",\"link\",\"version\",\"category_label\"");
		//	csvFile.Add("\"icon\",\"title\",\"link\",\"version\",\"category_label\"");

		//	foreach (Packages item in grpData[showGroup])
		//	{
		//		string assetURL = "";
		//		string assetIcon = "";

		//		// Asset Icon
		//		// Ignore exported (non-Asset Store packages)
		//		if (!item.isExported)
		//		{
		//			// Retrieve icon url
		//			if ( GYAStore.GetDownloadRowForAssetID(item.id) )
		//			{
		//				//assetIcon = "=HYPERLINK( \"\" ; IMAGE( \"http://d2ujflorbtfzji.cloudfront.net/key-image/ef2e3c94-1bac-4437-97fa-c0588c7fcbae.png\" ) )";

		//				assetIcon = "=IMAGE(\"http:";
		//				assetIcon += GYAStore.asData.downloads.currentRow.icon;
		//				assetIcon += "\")";
		//			}

		//			// Works for Google Sheets, Libre Office
		//			// Numbers/Excel may encode # to %23 when clicking/sending the link to the browser

		//			// Asset Link
		//			assetURL = "https://www.assetstore.unity3d.com/#/" + item.link.type + "/" + item.link.id.ToString();
		//			// Quotes in title double up
		//			assetURL = "=HYPERLINK(\"" + assetURL + "\", \"link\")";
		//		}

		//		csvFile.Add(string.Format("{0},{1},{2},{3},{4}",
		//			WrapCSVCell(assetIcon),
		//			WrapCSVCell(item.title),
		//			WrapCSVCell(assetURL),
		//			WrapCSVCell(item.version),
		//			WrapCSVCell(item.category.label)
		//		));

		//	}

		//	foreach (string line in csvFile)
		//	{
		//		result.AppendLine(line);
		//	}

		//	try
		//	{
		//		writer = new StreamWriter(path);
		//		writer.Write(result);
		//	}
		//	catch (Exception ex)
		//	{
		//		Debug.LogWarning(gyaAbbr + " - Exporting Error: \n" + ex.Message);
		//	}
		//	finally
		//	{
		//		if (writer != null)
		//		{
		//			writer.Close();
		//		}
		//		Debug.Log(gyaAbbr + " - Exported Group List as: " + path + "\n");
		//	}
		//}

		public void StopwatchStart ()
		{
			stopWatch.Start();
		}

		public void StopwatchStop ()
		{
			stopWatch.Stop();
		}

		public string StopwatchElapsed (bool consoleOutput = true, bool cumulative = false)
		{
			stopWatch.Stop();
			TimeSpan ts = stopWatch.Elapsed;
			//string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00} ({4:00}ms)", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10, ts.TotalMilliseconds);
			string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00} ({4:00}ms)", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10, ts.TotalMilliseconds);
			elapsedTime = elapsedTime.Replace("00:","");
			if (consoleOutput)
				Debug.Log(gyaAbbr + " - Elapsed Time: " + elapsedTime + "\n");

			if (!cumulative)
				stopWatch.Reset();

			return elapsedTime;
		}


		protected static void BrowseSelectedPackages(object pObj)
		{
			string dialogTitle = "Browse Asset Store";
			var gya = GrabYerAssets.Instance;
			var assets = gya.pkgData.Assets;
			int assetCounter = 0;
			const int batchCount = 10;  // How many assets to open in every batch.

			foreach (var asset in assets)
			{
				if (!asset.isMarked)
					continue;

				if ((assetCounter > 0)
					&& ((assetCounter % batchCount) == 0))
				{
					if (!EditorUtility.DisplayDialog(dialogTitle, string.Format("{0} URLs have already been opened.\nDo you want more?", assetCounter), "Continue", "Cancel"))
						break;
				}

				assetCounter++;
				if (asset.link.id > 0) {
					string openURL = string.Format("https://www.assetstore.unity3d.com/#/{0}/{1}", asset.link.type, asset.link.id);
					Application.OpenURL(openURL);
				}
			}

			if (assetCounter <= 0)
				EditorUtility.DisplayDialog(dialogTitle, "There is nothing to browse!\nFirst select one or more packages.", "OK");
		}

	}

	// GYA Extensions
	public static class GYAExt
	{

		// Open Asset Store page for specific asset
		public static void OpenAssetURL(int assetID, string urlOverride = null)
		{
			OpenAssetURL(assetID, false, urlOverride);
		}
		// Open Asset Store page for specific asset
		public static void OpenAssetURL(int assetID, bool openURLInUnity = false, string urlOverride = null)
		{
			//EditorApplication.ExecuteMenuItem( "Window/Asset Store" );
			string openURLSite = "https://www.assetstore.unity3d.com/#/";

			if (urlOverride != null)
				openURLSite = urlOverride;

			//string openURL = pkgDetails.link.type + "/" + pkgDetails.link.id.ToString();
			string openURL = "content/" + assetID.ToString();

			if (openURLInUnity)
			{

				openURL = "com.unity3d.kharma:" + openURL;

				// Open in Unity's Asset Store Window
				//UnityEditorInternal.AssetStore.Open (string.Format ("content/{0}?assetID={1}", activeAsset.packageID, activeAsset.id));
				//UnityEditorInternal.AssetStore.Open (string.Format ("content/{0}", pkgDetails.link.id.ToString()));
				UnityEditorInternal.AssetStore.Open (string.Format (openURL));

			}
			else
			{
				// Open in browser
				openURL = openURLSite + openURL;

				if (GYAExt.IsOSMac)
				{
					System.Diagnostics.Process.Start ("open", openURL);
				}
				else
				{ // if (GYAExt.IsOSWin) {
					System.Diagnostics.Process.Start (@openURL);
				}
			}
		}

		// Return path modified for platform
		public static string PathFixedForOS (string source)
		{
			char directorySeparatorDefault = '/';	// OS X, iOS, *nix, Android, etc
			char directorySeparatorWindows = '\\';	// Windows
			if (GYAExt.IsOSWin)
			{
				source = source.Replace(directorySeparatorDefault, directorySeparatorWindows);;
			}
			else
			{
				source = source.Replace(directorySeparatorWindows, directorySeparatorDefault);;
			}
			return source;
		}

		// Use: bool contains = string2.Contains("string1", StringComparison.OrdinalIgnoreCase);
		public static bool Contains(this string source, string toCheck, StringComparison comp)
		{
			if (source == null || toCheck == null)
			{
				//Debug.Log("GYA .Contains NULL Comparison: (" + toCheck + ")\n" + source);
				return false;
			}
			else
			{
				return source.IndexOf(toCheck, comp) >= 0;
			}
		}

		public static bool In<T>(this T source, params T[] list)
		{
			if(null==source)
				throw new ArgumentNullException("source");

			return list.Contains(source);
		}

		// Prev for enum
		public static T Prev<T>(this T src) where T : struct
		{
			if (!typeof(T).IsEnum)
				throw new ArgumentException(String.Format("Argumnent {0} is not an Enum", typeof(T).FullName));

			T[] Arr = (T[])Enum.GetValues(src.GetType());
			//Debug.Log ();
			int cnt = Array.IndexOf<T>(Arr, src) - 1;
			return (cnt == -1) ? Arr[Arr.Length-1] : Arr[cnt];
		}

		// Next for enum
		public static T Next<T>(this T src) where T : struct
		{
			if (!typeof(T).IsEnum)
				throw new ArgumentException(String.Format("Argumnent {0} is not an Enum", typeof(T).FullName));

			T[] Arr = (T[])Enum.GetValues(src.GetType());
			int cnt = Array.IndexOf<T>(Arr, src) + 1;
			return (cnt == Arr.Length) ? Arr[0] : Arr[cnt];
		}

		// Return the count of an enumerable
		public static int CountEnum (IEnumerable enumerable)
		{
			return (from object item in enumerable
				select item).Count();
		}

		// For Casting to Enum
		public static T ParseEnum<T>( string value )
		{
			return (T) Enum.Parse( typeof( T ), value, true );
		}

		// Open in window passed folder name, optionally strip the filename
		public static void ShellOpenFolder (string @folder, bool stripName = false)
		{
			if (stripName)
				folder = Path.GetDirectoryName (folder);

			if (Directory.Exists(folder))
			{
				folder = "\"" + folder + "\"";

				if (IsOSMac)
					System.Diagnostics.Process.Start ("open", folder);

				if (IsOSWin)
					System.Diagnostics.Process.Start (@folder);
			}
		}

		// Running Pro version?
		public static bool IsPro
		{
			get { return UnityEditorInternal.InternalEditorUtility.HasPro(); }
		}
		// Using Pro skin?
		public static bool IsProSkin
		{
			get { return EditorGUIUtility.isProSkin; }
		}
		// Is current OS Mac
		public static bool IsOSMac
		{
			get { return SystemInfo.operatingSystem.IndexOf("Mac OS") != -1; }
		}
		// Is current OS Windows
		public static bool IsOSWin
		{
			get { return SystemInfo.operatingSystem.IndexOf("Windows") != -1; }
		}

		// Is mouse over gui component & asset window
		public static bool IsMouseOver (Rect item)
		{
			return Event.current.type == EventType.Repaint && item.Contains(Event.current.mousePosition);
		}

		// Return the Folder: Unity App
		public static string PathUnityAppFolder
		{
			get { return Path.GetDirectoryName (EditorApplication.applicationPath); }
		}

		// Return the Folder: Unity Project Assets
		public static string PathUnityProjectAssets
		{
			get { return Path.GetFullPath( Path.Combine(PathUnityProject, "Assets") ); }
		}

		// Return the Folder: Unity Project
		public static string PathUnityProject
		{
			get { return Path.GetDirectoryName (Application.dataPath); }
		}

		// Return the Unity Folder: Standard Assets with Path
		public static string PathUnityStandardAssets
		{
			get { return Path.GetFullPath( Path.Combine (Path.GetDirectoryName (EditorApplication.applicationPath), FolderUnityStandardAssets) ); }
		}

		// Return the Unity Folder: Standard Assets without Path
		public static string FolderUnityStandardAssets
		{
			get
			{
				// Pre Unity 5 Asset Folder
				string standardAssetsPath = "Standard Packages";
			#if UNITY_5
				// Unity 5 Standard Assets folder
				standardAssetsPath = "Standard Assets";
			#endif
				return standardAssetsPath;
			}
		}

		// Return the Unity 3/4 Folder: Asset Store
		public static string PathGYADataFilesOLD
		{
			get
			{
				// System specific asset path
				if (GYAExt.IsOSMac)
					return Path.GetFullPath( Path.Combine (System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal), "Library/Unity") );

				if (GYAExt.IsOSWin)
					return Path.GetFullPath( Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData), "Unity") );

				return null;
			}
		}

		public static string FileInGYADataFiles (string pFile)
		{
			return Path.GetFullPath(Path.Combine(GYAExt.PathGYADataFiles, pFile));
		}

		public static string PathGYADataFiles
		{
			get
			{
				const string dataPath = "Grab Yer Assets";
				// System specific asset path
				if (GYAExt.IsOSMac)
					return Path.GetFullPath( Path.Combine (System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal), "Library/Unity/" + dataPath) );

				if (GYAExt.IsOSWin)
					return Path.GetFullPath( Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData), "Unity/" + dataPath) );

				return null;
			}
		}

		// Return the Unity Folder: Asset Store
		public static string FolderUnityAssetStore
		{
			get
			{
				return "Asset Store";
			}
		}

		// Return the Unity 5 Folder: Asset Store
		public static string FolderUnityAssetStore5
		{
			get
			{
				return "Asset Store-5.x";
			}
		}


		// Substring extractions


		// Get string Between first/last of other strings
		public static string Between(this string value, string a, string b)
		{
			int posA = value.IndexOf(a);
			int posB = value.LastIndexOf(b);
			if (posA == -1)
			{
				return "";
			}
			if (posB == -1)
			{
				return "";
			}
			int adjustedPosA = posA + a.Length;
			if (adjustedPosA >= posB)
			{
				return "";
			}
			return value.Substring(adjustedPosA, posB - adjustedPosA);
		}

		// Get first string Before another string
		public static string Before(this string value, string a)
		{
			int posA = value.IndexOf(a);
			if (posA == -1)
			{
				return "";
			}
			return value.Substring(0, posA);
		}
		// Get last string After another string
		public static string After(this string value, string a)
		{
			int posA = value.LastIndexOf(a);
			if (posA == -1)
			{
				return "";
			}
			int adjustedPosA = posA + a.Length;
			if (adjustedPosA >= value.Length)
			{
				return "";
			}
			return value.Substring(adjustedPosA);
		}


		// Return Digits or Alpha only


		public static string DigitsOnly(string pString)
		{
			System.Text.RegularExpressions.Regex text = new System.Text.RegularExpressions.Regex(@"[^\d]");
			return text.Replace(pString, "");
		}

		public static string AlphaOnly(string pString)
		{
			System.Text.RegularExpressions.Regex text = new System.Text.RegularExpressions.Regex(@"[^a-zA-Z]");
			return text.Replace(pString, "");
		}


		// BEGIN - Unity Version Methods


		// Return: true if == pVersion, false if != pVersion
		// pVersion format: "5.4.0b13", vDepth handling: "1.2.3444"
		public static bool UnityVersionIsEqualTo(String pVersion, int vDepth = 4)
		{
			//return Application.unityVersion.Equals(pVersion);
			int _bool = UnityVersionCompareTo(pVersion, vDepth);

			if (_bool == 0)
				return true;

			return false;
		}

		// Return: true if >= pVersion, false if < pVersion
		// pVersion format: "5.4.0b13", vDepth handling: "1.2.3444"
		public static bool UnityVersionIsEqualOrNewerThan(String pVersion, int vDepth = 4)
		{
			int _bool = UnityVersionCompareTo(pVersion, vDepth);

			if (_bool == -1)
				return false;

			return true;
		}

		// Return: true if <= pVersion, false if > pVersion
		// pVersion format: "5.4.0b13", vDepth handling: "1.2.3444"
		public static bool UnityVersionIsEqualOrOlderThan(String pVersion, int vDepth = 4)
		{
			int _bool = UnityVersionCompareTo(pVersion, vDepth);

			if (_bool == 1)
				return false;

			return true;
		}

		// Return: -1 if < pVersion, 0 if == pVersion, -1 if > pVersion
		// pVersion format: "5.4.0b13", vDepth handling: "1.2.3444"
		public static int UnityVersionCompareTo(String pVersion, int vDepth = 4)
		{
			if(pVersion == null) // If v2Ver is null, then Unity version is greater
			{
				return 1;
			}

			string v1Ver = UnityEngine.Application.unityVersion;						// 5.4.0b13
			Version v1 = UnityEditorInternal.InternalEditorUtility.GetUnityVersion();	// 5.4.0
			int v1BuildType = GetUnityVersionTypeWeight(v1Ver);					// b (weighted = 2)
			int v1BuildRev = GetUnityVersionRevision(v1Ver);			// 13

			Version v2 = new Version (GetUnityVersionBasic(pVersion));
			int v2BuildType = GetUnityVersionTypeWeight(pVersion);
			int v2BuildRev = GetUnityVersionRevision(pVersion);

			if(v1 == null)	// If v1 is null, then Unity version exception
			{
				throw new ArgumentNullException("version");
			}
			if(v2 == null)	// If v2 is null, then Unity version is greater
			{
				return 1;
			}

			if(v1.Major != v2.Major && vDepth >= 1)	// 5.4.0b13 = 5
				if(v1.Major > v2.Major)
					return 1;
				else
					return -1;

			if(v1.Minor != v2.Minor && vDepth >= 2)	// 5.4.0b13 = 4
				if(v1.Minor > v2.Minor)
					return 1;
				else
					return -1;

			if(v1.Build != v2.Build && vDepth >= 3)	// 5.4.0b13 = 0
				if(v1.Build > v2.Build)
					return 1;
				else
					return -1;

			if(v1BuildType != v2BuildType && vDepth >= 4)	// 5.4.0b13 = b as digit 2
				if(v1BuildType > v2BuildType)
					return 1;
				else
					return -1;

			if(v1BuildRev != v2BuildRev && vDepth >= 4)	// 5.4.0b13 = 13
				if(v1BuildRev > v2BuildRev)
					return 1;
				else
					return -1;

			return 0; // Unity version is equal
		}

		// Return version segments

		// 5.4.0b13 -> 5.4.0
		public static string GetUnityVersionBasic(string pString)
		{
			//if no versionType detected, return pString
			if ( (pString.Before(GetUnityVersionType(pString)) == String.Empty) )
				return pString;

			return pString.Before(GetUnityVersionType(pString));
		}

		// 5.4.0b13 -> b .. Build Type : a == alpha, b == beta, rc == release candidate, f == final, p == patch
		public static string GetUnityVersionType(string pString)
		{
			System.Text.RegularExpressions.Regex text = new System.Text.RegularExpressions.Regex(@"[^a-zA-Z]");
			return text.Replace(pString, "");

		}
		// 5.4.0b13 -> 2 .. Build Type : 1 == alpha, 2 == beta, 3 == release candidate, 4 == final, 5 == patch
		public static int GetUnityVersionTypeWeight(string pString)
		{
			string _text = GetUnityVersionType(pString);

			if (_text == "a")
				return 1;
			if (_text == "b")
				return 2;
			if (_text == "rc")
				return 3;
			if (_text == "f")
				return 4;
			if (_text == "p")
				return 5;

			return 0; // 0 = <Not Detected>
		}
		// 5.4.0b13 -> 13
		public static int GetUnityVersionRevision(string pString)
		{
			return int.Parse(pString.After(GetUnityVersionType(pString)));
		}

		// Unity Version Major/Minor/Builds


		// Return the Unity version full, "5.4.0b13"
		public static string GetUnityVersionWithTypeRevision
		{
			get {
				return UnityEngine.Application.unityVersion;
			}
		}

		// Return the Unity version digits, "5.4.0"
		public static string GetUnityVersionDigits
		{
			get {
				return UnityEditorInternal.InternalEditorUtility.GetUnityVersionDigits();
			}
		}

		// Return the Unity Major version, "5.4.0" Returns 5
		public static int GetUnityVersionMajor
		{
			get {
				Version version = new Version (UnityEditorInternal.InternalEditorUtility.GetUnityVersionDigits ());
				//Debug.Log( version.MajorRevision );	// -1
				return version.Major;
			}
		}

		// Return the Unity Minor version, "5.4.0" Returns 4
		public static int GetUnityVersionMinor
		{
			get {
				Version version = new Version (UnityEditorInternal.InternalEditorUtility.GetUnityVersionDigits ());
				//Debug.Log( version.MinorRevision );	// -1
				return version.Minor;
			}
		}

		// Return the Unity Build version, "5.4.0" Returns 0
		public static int GetUnityVersionBuild
		{
			get {
				Version version = new Version (UnityEditorInternal.InternalEditorUtility.GetUnityVersionDigits ());
				//Debug.Log( version.Revision );		// -1
				return version.Build;
			}
		}

		// Get Unity version date int, such as 1459313001
		public static int GetUnityVersionDate
		{
			get {
				return UnityEditorInternal.InternalEditorUtility.GetUnityVersionDate();
			}
		}

		// Return true/false, Is Unity version a beta build?
		public static bool IsUnityBeta
		{
			get {
				return UnityEditorInternal.InternalEditorUtility.IsUnityBeta();
			}
		}


		// END - Unity Version Methods


		// Find the Inactive AS Folder
		public static string FolderUnityAssetStoreInActive
		{
			get
			{
				if (GYAExt.FolderUnityAssetStoreActive == GYAExt.FolderUnityAssetStore5)
				{
					return GYAExt.FolderUnityAssetStore;
				}
				else
				{
					return GYAExt.FolderUnityAssetStore5;
				}
			}
		}

		// Return the correct Unity Folder: Asset Store for the actively running version
		public static string FolderUnityAssetStoreActive
		{
			get
			{
				return FolderUnityAssetStore5;
			}
		}

		// Return the correct Path of the Unity Folder: Asset Store for the actively running version
		public static string PathUnityAssetStoreActive
		{
			get
			{
				return PathUnityAssetStore5;
			}
		}

		// Return the Unity Folder: Asset Store Parent Folder
		public static string PathUnityAssetStoreParent
		{
			get {
				// System specific asset path
				if (GYAExt.IsOSMac)
					return Path.GetFullPath( Path.Combine (System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal), "Library/Unity") );

				if (GYAExt.IsOSWin)
					return Path.GetFullPath( Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData), "Unity") );

				return null;
			}
		}

		// Return the Unity Folder: Asset Store
		public static string PathUnityAssetStore
		{
			get {
				// System specific asset path
				if (GYAExt.IsOSMac)
					return Path.GetFullPath( Path.Combine (System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal), "Library/Unity/Asset Store") );

				if (GYAExt.IsOSWin)
					return Path.GetFullPath( Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData), "Unity/Asset Store") );

				return null;
			}
		}

		// Return the Unity 5 Folder: Asset Store "Asset Store-5.x"
		public static string PathUnityAssetStore5
		{
			get
			{
				// System specific asset path
				if (GYAExt.IsOSMac)
					return Path.GetFullPath( Path.Combine (System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal), "Library/Unity/Asset Store-5.x") );

				if (GYAExt.IsOSWin)
					return Path.GetFullPath( Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData), "Unity/Asset Store-5.x") );

				return null;
			}
		}

		// Convert bytes to KB/MB/GB
		public static string BytesToKB (double fileSizeBytes)
		{
			// Get filesize of asset
			string[] sizes = { "KB", "MB", "GB" };
			int order = 0;
			fileSizeBytes = fileSizeBytes/1024;

			while (fileSizeBytes >= 1024 && order + 1 < sizes.Length)
			{
				order++;
				fileSizeBytes = fileSizeBytes/1024;
			}
			// Format fileSize string
			return String.Format("{0:0.##} {1}", fileSizeBytes, sizes[order]);
		}

		// Return the byte range for the size header
		public static string GetByteRangeHeader(double pkgSize)
		{
			string headerText = String.Empty;
			int kb = 1024;
			pkgSize = pkgSize/1024;

			if (pkgSize > kb*1000)
				headerText = "1 GB+";
			if (pkgSize > kb*500 && pkgSize < kb*1000)
				headerText = "500 MB - < 1 GB";
			if (pkgSize > kb*250 && pkgSize < kb*500)
				headerText = "250 MB - < 500 MB";
			if (pkgSize > kb*100 && pkgSize < kb*250)
				headerText = "100 MB - < 250 MB";
			if (pkgSize > kb*50 && pkgSize < kb*100)
				headerText = "50 MB - < 100 MB";
			if (pkgSize > kb*10 && pkgSize < kb*50)
				headerText = "10 MB - < 50 MB";
			if (pkgSize > kb*1 && pkgSize < kb*10)
				headerText = "1 MB - < 10 MB";
			if (pkgSize < kb*1)
				headerText = "< 1 MB";

			return headerText;
		}

		public static Color WithAlpha(this Color color, float alpha)
		{
			return new Color(color.r, color.g, color.b, alpha);
		}
	}

	// Asset Post Processing
	public class GYAPostprocessor : AssetPostprocessor
	{
		static bool packageDetected = false;

		public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			//Check for imported unitypackages AND ignore changes to ProjectSettings.asset
			if (importedAssets.Count() > 0)
			{
				for (int i = 0; i < importedAssets.Length; i++)
				{
					// Check if a unitypackage was extracted into the local project during import
					if (Path.GetExtension(importedAssets[i]).ToLower() == ".unitypackage")
					{
						packageDetected = true;
					}
				}
			}

			// Check for deleted unitypackages
			if (deletedAssets.Count() > 0)
			{
				for (int i = 0; i < deletedAssets.Length; i++)
				{
					// Check if a unitypackage was deleted from the local project
					if (Path.GetExtension(deletedAssets[i]).ToLower() == ".unitypackage")
					{
						packageDetected = true;
					}
				}
			}

			// Check for moved unitypackages
			if (movedAssets.Count() > 0)
			{
				for (int i = 0; i < movedAssets.Length; i++)
				{
					// Check if a unitypackage was moved in the local project
					if (Path.GetExtension(movedAssets[i]).ToLower() == ".unitypackage")
					{
						packageDetected = true;
					}
				}
			}

			// If a unitypackage was imported/deleted/moved, rescan the local project
			if ( packageDetected )
			{
				packageDetected = false;
				GrabYerAssets.Instance.ScanProject();
				GrabYerAssets.Instance.BuildPrevNextList();
				GrabYerAssets.Instance.SVShowCollection(GrabYerAssets.Instance.showActive);
			}
		}
	}

	// NOT USED
	//[ExecuteInEditMode]
	public class GYASingleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static T _instance;
		private static object _lock = new object();
		public static bool isShuttingDown = false;

		public static T Instance
		{
			get
			{
				if (isShuttingDown)
					return null;

				lock(_lock)
				{
					if (_instance == null)
					{
						_instance = (T) FindObjectOfType(typeof(T));

						if ( FindObjectsOfType(typeof(T)).Length > 1 )
							return _instance;

						if (_instance == null)
						{
							GameObject singleton = new GameObject();
							singleton.hideFlags = HideFlags.HideAndDontSave;
							singleton.name = "GYASingleton";
							_instance = singleton.AddComponent<T>();
							DontDestroyOnLoad(singleton);
						}
					}
					return _instance;
				}
			}
		}

		public void Awake ()
		{
			//Debug.Log ("Awake");
		}

		public void OnDestroy()
		{
			isShuttingDown = true;
			//Debug.Log("OnDestroy");
		}

		public void OnApplicationQuit()
		{
			isShuttingDown = true;
			//Debug.Log("OnApplicationQuit");
		}

	}

	public static class GYAIO
	{
		const string gyaAbbr = "GYA";

		private const int FILE_SHARE_READ = 1;
		private const int FILE_SHARE_WRITE = 2;

		private const int CREATION_DISPOSITION_OPEN_EXISTING = 3;
		private const int FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;

		[DllImport("kernel32.dll", EntryPoint = "GetFinalPathNameByHandleW", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int GetFinalPathNameByHandle(IntPtr handle, [In, Out] StringBuilder path, int bufLen, int flags);

		[DllImport("kernel32.dll", EntryPoint = "CreateFileW", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern SafeFileHandle CreateFile(string lpFileName, int dwDesiredAccess, int dwShareMode,
			IntPtr SecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

		// Return target path is a symlink
		// OR .. return the unmodified file path if not a symlink or not Win OS
		public static string GetSymLinkTarget(string symlink)
		{
			if (Directory.Exists(symlink))
			{
				DirectoryInfo folderPath = new DirectoryInfo(symlink);
				return GetSymLinkTarget(folderPath);
			}
			else
			{
				return "";	// Doesn't exist
			}
		}


		/*
		string file = @"C:\Temp\SymlinkUnitTest\Original.txt";
        string actual = new FileInfo(file).GetSymbolicLinkTarget();
        Assert.IsTrue(actual.EndsWith(@"SymlinkUnitTest\Original.txt"));
        string dir = @"C:\Temp\SymlinkUnitTest";
        string actual = new DirectoryInfo(dir).GetSymbolicLinkTarget();
        Assert.IsTrue(actual.EndsWith(@"SymlinkUnitTest"));
		*/
		public static string GetSymLinkTarget(System.IO.DirectoryInfo symlink)
		{
			// If Windows
			if (GYAExt.IsOSWin)
			{
				SafeFileHandle directoryHandle = CreateFile(symlink.FullName, 0, 2, System.IntPtr.Zero, CREATION_DISPOSITION_OPEN_EXISTING, FILE_FLAG_BACKUP_SEMANTICS, System.IntPtr.Zero);
				if(directoryHandle.IsInvalid)
				{
					Debug.LogError(gyaAbbr + " - Path Is Invalid: " + symlink.ToString() + "\nMake sure the path is currently accessible and try again.\n");
					throw new Win32Exception(Marshal.GetLastWin32Error());
				}

				StringBuilder path = new StringBuilder(512);
				int size = GetFinalPathNameByHandle(directoryHandle.DangerousGetHandle(), path, path.Capacity, 0);
				if (size<0)
					throw new Win32Exception(Marshal.GetLastWin32Error());

				// The remarks section of GetFinalPathNameByHandle mentions
				// the return being prefixed with "\\?\"
				// More information about "\\?\" here ->
				// http://msdn.microsoft.com/en-us/library/aa365247(v=VS.85).aspx

				if (path[0] == '\\' && path[1] == '\\' && path[2] == '?' && path[3] == '\\')
				{
					return path.ToString().Substring(4);
				}
				else
				{
					return path.ToString();
				}
			}
			else
			{
				// If NOT Windows return path string
				return symlink.FullName;
			}
		}

		// TRUE if a symlink/reparse point
		public static bool IsSymLink(string path)
		{
			bool pathBool = false; // Default is NOT a symlink
			try
			{
				if (File.Exists(path) || Directory.Exists(path))
				{
					if ((File.GetAttributes(path) & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
						pathBool = true;
				}
			}
			catch (UnauthorizedAccessException ex)
			{
				Debug.LogWarning(gyaAbbr + " - IsSymLink UnauthorizedAccessException: \n" + ex.Message);
			}
			catch (Exception ex)
			{
				Debug.LogWarning(gyaAbbr + " - IsSymLink Exception: \n" + ex.Message);
			}
			return pathBool;
		}

		// Remove File Attributes
		//		FileAttributes attributes = File.GetAttributes(path);
		//		if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
		//		{
		//			attributes = GYAIO.RemoveFileAttribute(attributes, FileAttributes.ReadOnly);
		//			File.SetAttributes(path, attributes);
		//		}
		public static FileAttributes RemoveFileAttribute(FileAttributes attributes, FileAttributes attributesToRemove)
		{
			return attributes & ~attributesToRemove;
		}

		//internal static void UnityDirectoryRemoveReadonlyAttribute (string target_dir)
		//{
		//	string[] files = Directory.GetFiles (target_dir);
		//	string[] directories = Directory.GetDirectories (target_dir);
		//	string[] array = files;
		//	for (int i = 0; i < array.Length; i++)
		//	{
		//		string text = array [i];
		//		File.SetAttributes (text, 128);
		//	}
		//	string[] array2 = directories;
		//	for (int j = 0; j < array2.Length; j++)
		//	{
		//		string target_dir2 = array2 [j];
		//		GYAIO.UnityDirectoryRemoveReadonlyAttribute (target_dir2);
		//	}
		//}

		// -- GYAIO FileUtil

		// Return valid string, remove invalid chars from filename or path
		public static string ReturnValidPath (string pText)
		{
			string invalidChars = new string(GYAIO.GetInvalidPathChars());
			var validChars = pText.Where(x => !invalidChars.Contains(x)).ToArray();
			return new string(validChars);
		}

		public static string ReturnValidFile (string pText)
		{
			string invalidChars = new string(GetInvalidFileNameChars());
			var validChars = pText.Where(x => !invalidChars.Contains(x)).ToArray();
			return new string(validChars);
		}

		public static bool IsValidFileName (string filename)
		{
			string text = GYAIO.RemoveInvalidCharsFromFileName (filename, false);
			return !(text != filename) && !string.IsNullOrEmpty (text);
		}

		public static string RemoveInvalidCharsFromFileName (string filename, bool logIfInvalidChars = false)
		{
			if (string.IsNullOrEmpty (filename))
				return filename;

			filename = filename.Trim ();
			if (string.IsNullOrEmpty (filename))
				return filename;

			string text = new string (GYAIO.GetInvalidFileNameChars ());
			string text2 = string.Empty;
			bool flag = false;
			string text3 = filename;
			for (int i = 0; i < text3.Length; i++)
			{
				//char c = text3.get_Chars (i);
				char c = text3[i];
				if (text.IndexOf (c) == -1)
				{
					text2 += c;
				}
				else
				{
					flag = true;
				}
			}
			if (flag && logIfInvalidChars)
			{
				string displayStringOfInvalidCharsOfFileName = GYAIO.GetDisplayStringOfInvalidCharsOfFileName (filename);
				if (displayStringOfInvalidCharsOfFileName.Length > 0)
				{
					Debug.LogWarning ("A filename cannot contain the following character(s): " + displayStringOfInvalidCharsOfFileName + "\n");
					//UnityEngine.Debug.LogWarningFormat ("A filename cannot contain the following character{0}:  {1}", new object[]
					//{
					//	(displayStringOfInvalidCharsOfFileName.Length <= 1) ? string.Empty : "s",
					//	displayStringOfInvalidCharsOfFileName
					//});
				}
			}
			return text2;
		}

		public static string GetDisplayStringOfInvalidCharsOfFileName (string filename)
		{
			if (string.IsNullOrEmpty (filename))
				return string.Empty;

			string text = new string (GYAIO.GetInvalidFileNameChars ());
			string text2 = string.Empty;
			for (int i = 0; i < filename.Length; i++)
			{
				//char c = filename.get_Chars (i);
				char c = filename[i];
				if (text.IndexOf (c) >= 0 && text2.IndexOf (c) == -1)
				{
					if (text2.Length > 0)
						text2 += " ";

					text2 += c;
				}
			}
			return text2;
		}

		public static char[] GetInvalidFileNameChars ()
		{
			//if (Environment.IsRunningOnWindows)
			if (GYAExt.IsOSWin)
			{
				return new char[]
				{
					'\0',
					'\u0001',
					'\u0002',
					'\u0003',
					'\u0004',
					'\u0005',
					'\u0006',
					'\a',
					'\b',
					'\t',
					'\n',
					'\v',
					'\f',
					'\r',
					'\u000e',
					'\u000f',
					'\u0010',
					'\u0011',
					'\u0012',
					'\u0013',
					'\u0014',
					'\u0015',
					'\u0016',
					'\u0017',
					'\u0018',
					'\u0019',
					'\u001a',
					'\u001b',
					'\u001c',
					'\u001d',
					'\u001e',
					'\u001f',
					'"',
					'<',
					'>',
					'|',
					':',
					'*',
					'?',
					'\\',
					'/'
				};
			}
			return new char[]
			{
				'\0',
				'/',
				'|',
				':',
				'*'
			};
		}

		public static char[] GetInvalidPathChars ()
		{
			//if (Environment.IsRunningOnWindows)
			if (GYAExt.IsOSWin)
			{
				return new char[]
				{
					'"',
					'<',
					'>',
					'|',
					'\0',
					'\u0001',
					'\u0002',
					'\u0003',
					'\u0004',
					'\u0005',
					'\u0006',
					'\a',
					'\b',
					'\t',
					'\n',
					'\v',
					'\f',
					'\r',
					'\u000e',
					'\u000f',
					'\u0010',
					'\u0011',
					'\u0012',
					'\u0013',
					'\u0014',
					'\u0015',
					'\u0016',
					'\u0017',
					'\u0018',
					'\u0019',
					'\u001a',
					'\u001b',
					'\u001c',
					'\u001d',
					'\u001e',
					'\u001f'
				};
			}
			return new char[1];
		}

		// --

		//internal static bool AppendTextAfter (string path, string find, string append)
		//{
		//	bool result = false;
		//	path = GYAIO.NiceWinPath (path);
		//	List<string> list = new List<string> (File.ReadAllLines (path));
		//	for (int i = 0; i < list.get_Count (); i++)
		//	{
		//		if (list.get_Item (i).Contains (find))
		//		{
		//			list.Insert (i + 1, append);
		//			result = true;
		//			break;
		//		}
		//	}
		//	File.WriteAllLines (path, list.ToArray ());
		//	return result;
		//}
		internal static void CopyFileIfExists (string src, string dst, bool overwrite)
		{
			if (File.Exists (src))
				GYAIO.UnityFileCopy (src, dst, overwrite);
		}

		internal static void CreateOrCleanDirectory (string dir)
		{
			if (Directory.Exists (dir))
				Directory.Delete (dir, true);

			Directory.CreateDirectory (dir);
		}

		internal static List<string> GetAllFilesRecursive (string path)
		{
			List<string> files = new List<string> ();
			GYAIO.WalkFilesystemRecursively (path, delegate (string p)
			{
				files.Add (p);
			}, (string p) => true);
			return files;
		}

		internal static void MoveFileIfExists (string src, string dst)
		{
			if (File.Exists (src))
			{
				FileUtil.DeleteFileOrDirectory (dst);
				FileUtil.MoveFileOrDirectory (src, dst);
				//File.SetLastWriteTime (dst, DateTime.get_Now ());
			}
		}

		internal static string NiceWinPath (string unityPath)
		{
			return (Application.platform != RuntimePlatform.WindowsEditor) ? unityPath : unityPath.Replace ("/", "\\");
		}

		internal static string RemovePathPrefix (string fullPath, string prefix)
		{
			string[] array = fullPath.Split (new char[]
			{
				Path.DirectorySeparatorChar
			});
			string[] array2 = prefix.Split (new char[]
			{
				Path.DirectorySeparatorChar
			});

			int num = 0;
			if (array [0] == string.Empty)
				num = 1;

			while (num < array.Length && num < array2.Length && array [num] == array2 [num])
			{
				num++;
			}
			if (num == array.Length)
				return string.Empty;

			char directorySeparatorChar = Path.DirectorySeparatorChar;
			return string.Join (directorySeparatorChar.ToString (), array, num, array.Length - num);
		}

		internal static void ReplaceText (string path, params string[] input)
		{
			path = GYAIO.NiceWinPath (path);
			string[] array = File.ReadAllLines (path);
			for (int i = 0; i < input.Length; i += 2)
			{
				for (int j = 0; j < array.Length; j++)
				{
					array [j] = array [j].Replace (input [i], input [i + 1]);
				}
			}
			File.WriteAllLines (path, array);
		}

		//internal static bool ReplaceTextRegex (string path, params string[] input)
		//{
		//	bool result = false;
		//	path = GYAIO.NiceWinPath (path);
		//	string[] array = File.ReadAllLines (path);
		//	for (int i = 0; i < input.Length; i += 2)
		//	{
		//		for (int j = 0; j < array.Length; j++)
		//		{
		//			string text = array [j];
		//			array [j] = Regex.Replace (text, input [i], input [i + 1]);
		//			if (text != array [j])
		//			{
		//				result = true;
		//			}
		//		}
		//	}
		//	File.WriteAllLines (path, array);
		//	return result;
		//}

		internal static void UnityFileCopy (string from, string to)
		{
			GYAIO.UnityFileCopy (from, to, false);
		}

		internal static void UnityFileCopy (string from, string to, bool overwrite)
		{
			File.Copy (GYAIO.NiceWinPath (from), GYAIO.NiceWinPath (to), overwrite);
		}

		internal static void WalkFilesystemRecursively (string path, Action<string> fileCallback, Func<string, bool> directoryCallback)
		{
			string[] files = Directory.GetFiles (path);
			for (int i = 0; i < files.Length; i++)
			{
				string text = files [i];
				fileCallback.Invoke (text);
			}
			string[] directories = Directory.GetDirectories (path);
			for (int j = 0; j < directories.Length; j++)
			{
				string text2 = directories [j];
				if (directoryCallback (text2))
					GYAIO.WalkFilesystemRecursively (text2, fileCallback, directoryCallback);
			}
		}
	}

	// Coroutine used for: Multi-import 5.3+

	public class GYACoroutine
	{
		public static GYACoroutine start(IEnumerator _routine)
		{
			GYACoroutine coroutine = new GYACoroutine(_routine);
			coroutine.start();
			return coroutine;
		}

		readonly IEnumerator routine;
		GYACoroutine(IEnumerator _routine)
		{
			routine = _routine;
		}

		public void start()
		{
			EditorApplication.update += update;
		}

		public void stop()
		{
			EditorApplication.update -= update;
		}

		public void update()
		{
			if (!routine.MoveNext())
			{
				stop();
			}
		}
	}

	//Use: GYACoroutine.start(GYAImport.ImportPackageQueue(importQueue));
	//[ExecuteInEditMode]
	public class GYAImport : MonoBehaviour
	{
		// Should work in place of older import method, but just in case, keep it separate
		public static IEnumerator ImportPackageQueue (List<string> importQueue, string pMsg)
		{
			EditorApplication.LockReloadAssemblies();

			foreach (string pFilePath in importQueue)
			{
				//EditorApplication.LockReloadAssemblies();
				AssetDatabase.ImportPackage(pFilePath, false);
				yield return null;
				//EditorApplication.UnlockReloadAssemblies();
			}

			Debug.Log (pMsg);
			EditorApplication.UnlockReloadAssemblies();
			AssetDatabase.Refresh();
		}

		// AssetDatabase.ImportPackageImmediately via Reflection
		public static void ImportPackageImmediately (string pFilePath)
		{
			BindingFlags _flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
			// Import
			object[] paramsImport = new [] { pFilePath };
		
			MethodInfo miImport =
				typeof(EditorWindow).Assembly
				.GetType("UnityEditor.AssetDatabase").GetMethod("ImportPackageImmediately",
				_flags, Type.DefaultBinder,
				new[] { typeof(string) },
				null);
		
			miImport.Invoke( null, paramsImport );
		}
	}
}
// EOF
