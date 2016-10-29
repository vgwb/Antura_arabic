using ModularFramework.Modules;

namespace EA4S
{

    /// <summary>
    /// Game Setting Extension class.
    /// </summary>
    [System.Serializable]
    public class AppSettings : ModularFramework.Modules.GlobalOptions
    {
        public bool DoLogPlayerBehaviour;
        public bool HighQualityGfx;

        public int LastActivePlayerId;

        public bool UseTestDatabase;
    }
}