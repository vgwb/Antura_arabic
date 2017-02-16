////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Deploy
// @author Stanislav Osipov (Stan's Assets) 
// @support support@stansassets.com
//
////////////////////////////////////////////////////////////////////////////////


using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif


namespace SA.IOSDeploy {

	#if UNITY_EDITOR
	[InitializeOnLoad]
	#endif
	public class ISD_Settings : ScriptableObject{

		public const string VERSION_NUMBER = "2.5/17";


		//Editor Window
		public bool IsfwSettingOpen;
		public bool IsLibSettingOpen;
		public bool IslinkerSettingOpne;
		public bool IscompilerSettingsOpen;
		public bool IsPlistSettingsOpen;
		public bool IsLanguageSettingOpen = true;
		public bool IsDefFrameworksOpen = false;
		public bool IsDefLibrariesOpen = false;
		public bool IsBuildSettingsOpen;
		public int ToolbarIndex = 0;


		//BuildOptions
		public bool enableBitCode = false;
		public bool enableTestability = false;
		public bool generateProfilingCode = false;


		public List<Framework> Frameworks =  new List<Framework>();
		public List<BaseFramework> BaseFrameworks =  new List<BaseFramework>();
		public List<Lib> Libraries =  new List<Lib>(); 
		public List<Flag> Flags = new List<Flag> ();
		public List<Variable>  PlistVariables =  new List<Variable>();
		public List<VariableId> VariableDictionary = new List<VariableId>();


		public List<string> langFolders = new List<string>();

		
		private const string ISDAssetName = "ISD_Settings";
		private const string ISDAssetExtension = ".asset";

		private static ISD_Settings instance;
		 


		public static ISD_Settings Instance
		{
			get
			{
				if(instance == null)
				{
					instance = Resources.Load(ISDAssetName) as ISD_Settings;
					if(instance == null)
					{
						instance = CreateInstance<ISD_Settings>();
						#if UNITY_EDITOR

						SA.Common.Util.Files.CreateFolder(SA.Common.Config.SETTINGS_PATH);
						string fullPath = Path.Combine(Path.Combine("Assets", SA.Common.Config.SETTINGS_PATH), ISDAssetName + ISDAssetExtension );
						
						AssetDatabase.CreateAsset(instance, fullPath);
						#endif

					}
				}
			
				return instance;
			}
		}


		public void AddNewVariable(Variable var){
			foreach (Variable v in PlistVariables.ToList()) {
				if (v.Name.Equals (var.Name)) {
					PlistVariables.Remove (v);
				}
			}
			PlistVariables.Add(var);
		}

		public void AddLinkerFlag(string s){
			Flag newFlag = new Flag ();
			newFlag.Name = s;
			newFlag.Type = FlagType.LinkerFlag;
			foreach (Flag f in Flags) {
				if (f.Type.Equals (FlagType.LinkerFlag) && f.Name.Equals (s)) {
					break;
				}
			}
			Flags.Add (newFlag);
		}

		public void AddOrReplaceNewVariable(Variable var){
			foreach (Variable v in PlistVariables) {
				if (v.Name.Equals (var.Name)) {
					PlistVariables.Remove (v);
				}
			}
			PlistVariables.Add(var);
		}

		public void AddVariableToDictionary(string uniqueIdKey,Variable var){
			VariableId newVar = new VariableId ();
			newVar.uniqueIdKey = uniqueIdKey;
			newVar.VariableValue = var;
			VariableDictionary.Add(newVar);
		}

		public void RemoveVariable(Variable v, IList ListWithThisVariable){
			if (ISD_Settings.Instance.PlistVariables.Contains (v)) {
				ISD_Settings.Instance.PlistVariables.Remove (v);
			} else {
				foreach(VariableId vid in VariableDictionary){
					if (vid.VariableValue.Equals (v)) {
						VariableDictionary.Remove (vid);
						string id = vid.uniqueIdKey;
						if(ListWithThisVariable.Contains(id))
							ListWithThisVariable.Remove (vid.uniqueIdKey);
						break;
					}
				}
			}
		}

		public Variable getVariableByKey(string uniqueIdKey){
			foreach (VariableId vid in VariableDictionary) {
				if (vid.uniqueIdKey.Equals (uniqueIdKey)) {
					return vid.VariableValue;
				}
			}
			return null;
		}

		public Variable GetVariableByName(string name) {
			foreach(Variable var in ISD_Settings.Instance.PlistVariables) {
				if(var.Name.Equals(name)) {
					return var;
				}
			}

			return null;
		}


		public string getKeyFromVariable(Variable var){
			foreach (VariableId vid in VariableDictionary) {
				if (vid.VariableValue.Equals (var)) {
					return vid.uniqueIdKey;
				}
			}
			return null;
		}

		public bool ContainsFreamworkWithName(string name) {
			foreach(Framework f in ISD_Settings.Instance.Frameworks) {
				if(f.Name.Equals(name)) {
					return true;
				}
			}
			
			return false;
		}

		public bool ContainsPlistVarWithName(string name) {
			foreach(Variable var in ISD_Settings.Instance.PlistVariables) {
				if(var.Name.Equals(name)) {
					return true;
				}
			}
			
			return false;
		}
			
		public void AddPlistVariable(Variable newVariable) {
			if (ISD_Settings.Instance.PlistVariables.Count > 0) {
				foreach(Variable var in ISD_Settings.Instance.PlistVariables) {
					if (var.Name.Equals(newVariable.Name)) {
						if (var.Type.Equals (newVariable.Type)) { //add parrams
							switch (var.Type) {
							case PlistValueTypes.Dictionary:
								foreach (string newChildId in newVariable.ChildrensIds) {
									var.AddChild (ISD_Settings.Instance.getVariableByKey (newChildId));
								}
								break;
							case PlistValueTypes.Array:
								foreach (string newChildId in newVariable.ChildrensIds) {
									var.AddChild (ISD_Settings.Instance.getVariableByKey (newChildId));
								}
								break;
							case PlistValueTypes.Boolean:
								var.BooleanValue = newVariable.BooleanValue;
								break;
							case PlistValueTypes.Float:
								var.FloatValue = newVariable.FloatValue;
								break;
							case PlistValueTypes.Integer:
								var.IntegerValue = newVariable.IntegerValue;
								break;
							case PlistValueTypes.String:
								var.StringValue = newVariable.StringValue;
								break;
							}
						} else {							//replace new with old
							RemoveVariable (var, ISD_Settings.Instance.PlistVariables);
							ISD_Settings.Instance.PlistVariables.Add (newVariable);
						}
					} else { //No manes match
						ISD_Settings.Instance.PlistVariables.Add (newVariable);
					}
				}
			} else { //First variable
				ISD_Settings.Instance.PlistVariables.Add (newVariable);
			}
		}



		public bool ContainsLibWithName(string name) {
			foreach(Lib l in ISD_Settings.Instance.Libraries) {
				if(l.Name.Equals(name)) {
					return true;
				}
			}
			
			return false;
		}
							
	}
}
