namespace EA4S.Core
{
    /// <summary>
    /// Strategy interface. 
    /// Provide All the functionalities required for any Concrete implementation of the module.
    /// </summary>
    public interface IPlayerProfileModule : IModule<IPlayerProfileModule>
    {
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
}