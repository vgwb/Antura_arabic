
using UnityEngine;
using System.Collections;

namespace EA4S.Core
{

    /// <summary>
    /// Iterface for any module. 
    /// Force the strategy pattern implementation.
    /// </summary>
    public interface IModule<T>{
        IModuleSettings Settings { get; set; }
        T ConcreteModuleImplementation { get; set; }
        T SetupModule(T _concreteModule, IModuleSettings _settings = null);
    }

    public interface IModuleSettings { }

}