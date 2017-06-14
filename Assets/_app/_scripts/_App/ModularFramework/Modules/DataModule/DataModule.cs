
using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Modules System with strategy pattern implementation.
/// </summary>
namespace EA4S.Core
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