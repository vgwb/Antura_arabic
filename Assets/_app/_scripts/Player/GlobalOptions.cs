namespace EA4S.Core
{
    /// <summary>
    /// Serializable options not related to a specific player profile.
    /// </summary>
    public class GlobalOptions : ModularFramework.Modules.GlobalOptions
    {
        public int LastActivePlayerId;
    }
}