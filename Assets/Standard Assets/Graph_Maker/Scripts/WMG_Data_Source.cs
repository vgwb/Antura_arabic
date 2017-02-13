using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// This defines a data source which can be used with WMG_Series, WMG_Pie_Graph, or WMG_Ring_Graph.
/// </summary>
public class WMG_Data_Source : MonoBehaviour {

	/* // DELETE THIS LINE FOR USE WITH PLAYMAKER
	public void addPlaymakerVar (PlayMakerFSM dataProvider, string varName) {
		this.dataProviders.Add(dataProvider.FsmVariables.GetVariable(varName));
	}
	
	public void removePlaymakerVar (PlayMakerFSM dataProvider, string varName) {
		this.dataProviders.Remove(dataProvider.FsmVariables.GetVariable(varName));
	}
	*/ // DELETE THIS LINE FOR USE WITH PLAYMAKER

	public enum WMG_DataSourceTypes {
		Single_Object_Multiple_Variables,
		Multiple_Objects_Single_Variable,
		Single_Object_Single_Variable
	}

	public enum WMG_VariableTypes {
		Not_Specified, // Will try all the possibilities below (so specify this to improve performance)
		Field, // e.g. public float test;
		Property, // e.g. public float Test {get; private set};
		Property_Field, // e.g. localPosition.y from a Transform dataProvider
		Field_Field // e.g. test.y from public Vector3 test;
	}

	/// <summary>
	/// Determines the type of this data source in terms of multi vs single objects and multi vs single variables.
	/// - Single_Object_Multiple_Variables: Expected data source refers to a single script object and multiple variables (Use #dataProvider, and #variableNames)
	/// - Multiple_Objects_Single_Variable: Expected data source refers to multiple instanced script objects and a single variable (Use #dataProviders, and #variableName)
	/// - Single_Object_Single_Variable: Expected data source refers to a single script object and a single variable (Use #dataProvider, and #variableName)
	/// </summary>
	public WMG_DataSourceTypes dataSourceType;

	/// <summary>
	/// When using #dataSourceType = Multiple_Objects_Single_Variable, this is the reference to the list of instanced script objects from which data will be pulled. 
	/// </summary>
	public List<object> dataProviders = new List<object>();
	/// <summary>
	/// When using #dataSourceType = Single_Object_Multiple_Variables or Single_Object_Single_Variable, this is the reference to the instanced script object from which data will be pulled.
	/// </summary>
	public object dataProvider;

	/// <summary>
	/// Optionally set the variable type corresponding with #variableNames to slightly improve performance.
	/// </summary>
	public List<WMG_VariableTypes> variableTypes = new List<WMG_VariableTypes>();
	/// <summary>
	/// Optionally set the variable type corresponding with #variableName to slightly improve performance.
	/// </summary>
	public WMG_VariableTypes variableType;

	/// <summary>
	/// When using #dataSourceType = Single_Object_Multiple_Variables, this is the name of the variables from which data will be pulled.
	/// Variable can correspond to the field of a property, for example Transform.localPosition.x (Transform is the data provider, and localPosition.x is the variableName).
	/// Use the #setVariableNames, #addVariableNameToList, or #removeVariableNameFromList functions to change this.
	/// </summary>
	public List<string> variableNames;
	/// <summary>
	/// When using #dataSourceType = Multiple_Objects_Single_Variable or Single_Object_Single_Variable, this is the name of the variable from which data will be pulled.
	/// Variable can correspond to the field of a property, for example Transform.localPosition.x (Transform is the data provider, and localPosition.x is the variableName).
	/// Use the #setVariableName function to change this.
	/// </summary>
	public string variableName;

	// first part of the strings split on "."
	List<string> varNames1 = new List<string>();
	string varName1;

	// second part of the strings split on "."
	List<string> varNames2 = new List<string>();
	string varName2;

	void Start() {
		if (variableNames == null)
			variableNames = new List<string> ();
		parseStrings();
	}

	void parseStrings() { // variable name(s) can be of the format property.field, parse these.
		varNames1.Clear();
		varNames2.Clear();
		for (int i = 0; i < variableNames.Count; i++) {
			string vName1 = "";
			string vName2 = "";
			parseString(variableNames[i], ref vName1, ref vName2);
			varNames1.Add(vName1);
			varNames2.Add(vName2);
		}
		parseString(variableName, ref varName1, ref varName2);
	}

	void parseString(string inputString, ref string stringPart1, ref string stringPart2) {
		string[] variableNames = inputString.Split('.');
		stringPart1 = "";
		stringPart2 = "";
		if (variableNames.Length < 2) return;
		stringPart1 = variableNames[0];
		stringPart2 = variableNames[1];
	}

	/// <summary>
	/// Set entire list of variable names to #variableNames.
	/// </summary>
	/// <param name="variableNames">Variable names.</param>
	public void setVariableNames (List<string> variableNames) {
		this.variableNames.Clear();
		foreach (string obj in variableNames) {
			this.variableNames.Add(obj);
		}
		parseStrings();
	}

	/// <summary>
	/// Sets #variableName.
	/// </summary>
	/// <param name="variableName">Variable name.</param>
	public void setVariableName (string variableName) {
		this.variableName = variableName;
		parseString(variableName, ref varName1, ref varName2);
	}

	/// <summary>
	/// Adds a variable name to #variableNames.
	/// </summary>
	/// <param name="variableName">Variable name.</param>
	public void addVariableNameToList (string variableName) {
		this.variableNames.Add(variableName);
		parseStrings();
	}

	/// <summary>
	/// Removes the specified variable name from #variableNames.
	/// </summary>
	/// <param name="variableName">Variable name.</param>
	public void removeVariableNameFromList (string variableName) {
		this.variableNames.Remove(variableName);
		parseStrings();
	}

	/// <summary>
	/// Set entire list of data providers to #dataProviders.
	/// </summary>
	/// <param name="dataProviderList">Data provider list.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public void setDataProviders<T> (List<T> dataProviderList) {
		this.dataProviders.Clear();
		foreach (T obj in dataProviderList) {
			this.dataProviders.Add(obj);
		}
	}

	/// <summary>
	/// Sets #dataProvider.
	/// </summary>
	/// <param name="dataProvider">Data provider.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public void setDataProvider<T> (T dataProvider) {
		this.dataProvider = dataProvider as object;
	}

	/// <summary>
	/// Adds a data provider to #dataProviders.
	/// </summary>
	/// <param name="dataProvider">Data provider.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public void addDataProviderToList<T> (T dataProvider) {
		this.dataProviders.Add(dataProvider);
	}

	/// <summary>
	/// Removes the specified data provider from #dataProviders.
	/// </summary>
	/// <returns><c>true</c>, if data provider from list was removed, <c>false</c> otherwise.</returns>
	/// <param name="dataProvider">Data provider.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public bool removeDataProviderFromList<T> (T dataProvider) {
		return this.dataProviders.Remove(dataProvider);
	}

	/// <summary>
	/// Pulls a list of data such as a list of floats, from this data source.
	/// </summary>
	/// <returns>The data.</returns>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public List<T> getData<T>() {
		if (dataSourceType == WMG_DataSourceTypes.Multiple_Objects_Single_Variable) {
			List<T> returnVals = new List<T>();
			foreach (object dp in dataProviders) {
				returnVals.Add(getDatum<T>(dp, variableName, variableType, varName1, varName2));
			}
			return returnVals;
		}
		else if (dataSourceType == WMG_DataSourceTypes.Single_Object_Multiple_Variables) {
			List<T> returnVals = new List<T>();
			for (int i = 0; i < variableNames.Count; i++) {
				string varName = variableNames[i];
				WMG_VariableTypes varType = WMG_VariableTypes.Not_Specified; 
				if (i < variableTypes.Count) varType = variableTypes[i];
				if (i >= varNames1.Count) parseStrings(); // strings added to list through the Unity editor at run-time, or set / add variableName API not used 
				returnVals.Add(getDatum<T>(dataProvider, varName, varType, varNames1[i], varNames2[i]));
			}
			return returnVals;
		}
		else if (dataSourceType == WMG_DataSourceTypes.Single_Object_Single_Variable) {
			try { // try field, e.g. public List<Vector2> test;
				return (List<T>) WMG_Reflection.GetField(dataProvider.GetType(), variableName).GetValue(dataProvider);
			}
			catch (Exception e) {
				Debug.Log("Field: " + variableName + " not found. " + e.Message);
				return new List<T>();
			}
		}
		return new List<T>();
	}

	/// <summary>
	/// Pulls a single value such as a float, from this data source.
	/// </summary>
	/// <returns>The datum.</returns>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public T getDatum<T>() {
		if (dataSourceType == WMG_DataSourceTypes.Single_Object_Single_Variable) {
			return getDatum<T>(dataProvider, variableName, variableType, varName1, varName2);
		}
		else {
			Debug.Log("getDatum() is not supported for dataSourceType not equal to Single_Object_Single_Variable.");
			return default(T);
		}
	}

	T getDatum<T>(object dp, string variableName, WMG_VariableTypes varType, string vName1, string vName2) {
		if (varType == WMG_VariableTypes.Field) {
			try { // try field, e.g. public float test;
				return (T) WMG_Reflection.GetField(dp.GetType(), variableName).GetValue(dp);
			}
			catch (Exception e) {
				Debug.Log("Field: " + variableName + " not found. " + e.Message);
				return default(T);
			}
		}
		else if (varType == WMG_VariableTypes.Property) {
			try { // try property, e.g. public float Test {get; private set};
				return (T) WMG_Reflection.GetProperty(dp.GetType(), variableName).GetValue(dp, null);
			}
			catch (Exception e) {
				Debug.Log("Property: " + variableName + " not found. " + e.Message);
				return default(T);
			}
		}
		else if (varType == WMG_VariableTypes.Property_Field) {
			try { // try property.field, e.g. localPosition.y from a Transform dataProvider
				object dpProperty = WMG_Reflection.GetProperty(dp.GetType(), vName1).GetValue(dp, null);
				return (T) WMG_Reflection.GetField(dpProperty.GetType(), vName2).GetValue(dpProperty);
			}
			catch (Exception e) {
				Debug.Log("property.field: " + variableName + " not found. " + e.Message);
				return default(T);
			}
		}
		else if (varType == WMG_VariableTypes.Field_Field) {
			try { // try field.field, e.g. test.y from public Vector3 test;
				object dpField = WMG_Reflection.GetField(dp.GetType(), vName1).GetValue(dp);
				return (T) WMG_Reflection.GetField(dpField.GetType(), vName2).GetValue(dpField);
			}
			catch (Exception e) {
				Debug.Log("field.field: " + variableName + " not found. " + e.Message);
				return default(T);
			}
		}
		else if (varType == WMG_VariableTypes.Not_Specified) {
			try { // try field, e.g. public float test;
				return (T) WMG_Reflection.GetField(dp.GetType(), variableName).GetValue(dp);
			}
			catch {
				try { // try property, e.g. public float Test {get; private set};
					return (T) WMG_Reflection.GetProperty(dp.GetType(), variableName).GetValue(dp, null);
				}
				catch { 
					try { // try property.field, e.g. localPosition.y from a Transform dataProvider
						object dpProperty = WMG_Reflection.GetProperty(dp.GetType(), vName1).GetValue(dp, null);
						return (T) WMG_Reflection.GetField(dpProperty.GetType(), vName2).GetValue(dpProperty);
					}
					catch { // try field.field, e.g. test.y from public Vector3 test;
						try {
							object dpField = WMG_Reflection.GetField(dp.GetType(), varName1).GetValue(dp);
							return (T) WMG_Reflection.GetField(dpField.GetType(), vName2).GetValue(dpField);
						}
						catch (Exception e) {
							Debug.Log("field, property, property.field, or field.field: " + variableName + " not found. " + e.Message);
							return default(T);
						}
					}
				}
			}
		}
		return default(T);
	}

}
