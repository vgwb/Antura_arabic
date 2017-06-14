using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EA4S.Core
{
    /// <summary>
    /// This is the Context implementation of this Module type with all functionalities provided from the Strategy interface.
    /// </summary>
    public class PlayerProfileModule : IPlayerProfileModule {

        /// <summary>
        /// Actual active player profile.
        /// </summary>
        public IPlayerProfile ActivePlayer {
            get { return ConcreteModuleImplementation.ActivePlayer; }
            set { ConcreteModuleImplementation.ActivePlayer = value; }
        }
        /// <summary>
        /// List of Available players profiles.
        /// </summary>
        public GlobalOptions Options {
            get { return ConcreteModuleImplementation.Options; }
            set { ConcreteModuleImplementation.Options = value; }
        }

        public bool MultipleProfileSupported = false;

        #region IModule implementation
        /// <summary>
        /// Concrete Module Implementation.
        /// </summary>
        public IPlayerProfileModule ConcreteModuleImplementation { get; set; }
        public IModuleSettings Settings { get; set; }
        
        /// <summary>
        /// Module Setup.
        /// </summary>
        /// <param name="_concreteModule">Concrete module implementation to set as active module behaviour.</param>
        /// <returns></returns>
        public IPlayerProfileModule SetupModule(IPlayerProfileModule _concreteModule, IModuleSettings _settings = null) {
            ConcreteModuleImplementation = _concreteModule.SetupModule(_concreteModule, _settings);
            if (ConcreteModuleImplementation == null)
                OnSetupError();
            return ConcreteModuleImplementation;
        }

        /// <summary>
        /// Called if an error occurred during the setup.
        /// </summary>
        void OnSetupError() {
            Debug.LogErrorFormat("Module {0} setup return an error.", this.GetType());
        }

        #endregion

        /// <summary>
        /// Create new player with informations provided.
        /// </summary>
        /// <param name="_newPlayer"></param>
        /// <param name="_extProfile"></param>
        /// <returns>Player created or null if player with user id already exist.</returns>
        public IPlayerProfile CreateNewPlayer(IPlayerProfile _newPlayer, IPlayerExtendedProfile _extProfile = null) {
            return ConcreteModuleImplementation.CreateNewPlayer(_newPlayer, _extProfile);
        }

        /// <summary>
        /// Delete player with corresponding Id.
        /// </summary>
        /// <param name="_playerId"></param>
        public void DeletePlayer(string _playerId) {
            ConcreteModuleImplementation.DeletePlayer(_playerId);
        }

        /// <summary>
        /// Update player profile with data in param.
        /// </summary>
        /// <param name="_newPlayer"></param>
        /// <param name="_extProfile"></param>
        /// <returns></returns>
        public IPlayerProfile UpdatePlayer(IPlayerProfile _newPlayer, IPlayerExtendedProfile _extProfile = null) {
            return ConcreteModuleImplementation.UpdatePlayer(_newPlayer, _extProfile);
        }

        /// <summary>
        /// Set Active player as Active Player.
        /// </summary>
        /// <param name="_playerId"></param>
        public void SetActivePlayer<T>(string _playerId) where T : IPlayerProfile {
            ConcreteModuleImplementation.SetActivePlayer<T>(_playerId);
        }

        /// <summary>
        /// Save player settins.
        /// </summary>
        /// <param name="_newPlayer"></param>
        /// <param name="_extProfile"></param>
        public void SavePlayerSettings(IPlayerProfile _newPlayer, IPlayerExtendedProfile _extProfile = null) {
            ConcreteModuleImplementation.SavePlayerSettings(_newPlayer, _extProfile);
        }

        /// <summary>
        /// Load player settings from previous saved.
        /// </summary>
        /// <param name="_playerId"></param>
        public IPlayerProfile LoadPlayerSettings<T>(string _playerId) where T : IPlayerProfile {
            return ConcreteModuleImplementation.LoadPlayerSettings<T>(_playerId);
        }

        /// <summary>
        /// Load all global options included list of players profiles.
        /// </summary>
        /// <returns></returns>
        //public IGlobalOptions LoadGlobalOptions<T>() where T : IGlobalOptions {
        //    return ConcreteModuleImplementation.LoadGlobalOptions<T>();
        //}

        public GlobalOptions LoadGlobalOptions<T>(T _defaultOptions) where T : GlobalOptions {
            return ConcreteModuleImplementation.LoadGlobalOptions<T>(_defaultOptions);
        }

        /// <summary>
        /// Store alla player profiles.
        /// </summary>
        public void SaveAllOptions() {
            ConcreteModuleImplementation.SaveAllOptions();
        }

        /// <summary>
        /// WARNING! Delete all stored profiles and set actual profile to null.
        /// </summary>
        public void DeleteAllPlayerProfiles() {
            SaveAllOptions();
            ActivePlayer = null;
        }
    }

    #region Interfaces

    /// <summary>
    /// Strategy interface. 
    /// Provide All the functionalities required for any Concrete implementation of the module.
    /// </summary>
    public interface IPlayerProfileModule : IModule<IPlayerProfileModule> {
        IPlayerProfile ActivePlayer { get; set; }
        GlobalOptions Options { get; set; }
        // Player creation
        IPlayerProfile CreateNewPlayer(IPlayerProfile _newPlayer, IPlayerExtendedProfile _extProfile = null);
        void DeletePlayer(string _playerId);
        // Mod Player 
        IPlayerProfile UpdatePlayer(IPlayerProfile _newPlayer, IPlayerExtendedProfile _extProfile = null);
        // Change player
        void SetActivePlayer<T>(string _playerId) where T : IPlayerProfile;
        // Save and load
        void SavePlayerSettings(IPlayerProfile _newPlayer, IPlayerExtendedProfile _extProfile = null);
        IPlayerProfile LoadPlayerSettings<T>(string _playerId) where T : IPlayerProfile;
        // Save and load Players
        //IGlobalOptions LoadGlobalOptions<T>() where T : IGlobalOptions;
        GlobalOptions LoadGlobalOptions<T>(T _defaultOptions) where T : GlobalOptions;
        void SaveAllOptions();
    }

    ///// <summary>
    ///// Data struct to store organized data info for players profiles into module.
    ///// </summary>
    //[Serializable]
    //public class PlayersData {
    //    public string Dummy;
    //    public List<string> AvailablePlayers;
    //}

    /// <summary>
    /// Interface for optional Extended player profile.
    /// </summary>
    public interface IPlayerExtendedProfile {
        string PlayerRef { get; set; }
    }

    public interface IPlayerProfile {
        string Key { get; set; }
    }

    #endregion
}