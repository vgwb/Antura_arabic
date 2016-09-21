/* --------------------------------------------------------------
*   Indie Contruction : Modular Framework for Unity
*   Copyright(c) 2016 Indie Construction / Paolo Bragonzi
*   All rights reserved. 
*   For any information refer to http://www.indieconstruction.com
*   
*   This library is free software; you can redistribute it and/or
*   modify it under the terms of the GNU Lesser General Public
*   License as published by the Free Software Foundation; either
*   version 3.0 of the License, or(at your option) any later version.
*   
*   This library is distributed in the hope that it will be useful,
*   but WITHOUT ANY WARRANTY; without even the implied warranty of
*   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
*   Lesser General Public License for more details.
*   
*   You should have received a copy of the GNU Lesser General Public
*   License along with this library.
* -------------------------------------------------------------- */
using UnityEngine;
using System;
using System.Collections;
using ModularFramework.Core;

/// <summary>
/// Modules System with strategy pattern implementation.
/// </summary>
namespace ModularFramework.Modules
{
	/// <summary>
	/// This is the Context implementation of this Module type with all functionalities provided from the Strategy interface.
	/// </summary>
	public class DataModule : IDataModule
	{

		#region IModule implementation
		/// <summary>
		/// Concrete Module Implementation.
		/// </summary>
		public IDataModule ConcreteModuleImplementation { get; set; }
		public IModuleSettings Settings { get; set; }

		/// <summary>
		/// Module Setup.
		/// </summary>
		/// <param name="_concreteModule">Concrete module implementation to set as active module behaviour.</param>
		/// <returns></returns>
		public IDataModule SetupModule(IDataModule _concreteModule, IModuleSettings _settings = null)
		{
			ConcreteModuleImplementation = _concreteModule.SetupModule(_concreteModule, _settings);
			if (ConcreteModuleImplementation == null)
				OnSetupError();
			return ConcreteModuleImplementation;
		}

		/// <summary>
		/// Called if an error occurred during the setup.
		/// </summary>
		void OnSetupError()
		{
			Debug.LogErrorFormat("Module {0} setup return an error.", this.GetType());
		}
		#endregion

		public ISerializableData LoadData(string _resourceName)
		{
			return ConcreteModuleImplementation.LoadData(_resourceName);
		}

		public void SaveData(ISerializableData _dataToStore, string _filename)
		{
			ConcreteModuleImplementation.SaveData(_dataToStore, _filename);
		}

	}

	/// <summary>
	/// Strategy interface. 
	/// Provide All the functionalities required for any Concrete implementation of the module.
	/// </summary>
	public interface IDataModule : IModule<IDataModule>
	{
		// Add here functionalities for the module
		ISerializableData LoadData(string _resourceName);
		void SaveData(ISerializableData _dataToStore, string _filename);
	}

	/// <summary>
	/// Interface for all serializable data.
	/// </summary>
	public interface ISerializableData { }
}