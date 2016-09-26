// Written by Davide Barbieri <davide.barbieri AT ghostshark.it>
namespace EA4S
{
    public static class IGameContextExtensions
    {
        public static void ToggleMusic(this IAudioManager manager)
        {
            manager.MusicEnabled = !manager.MusicEnabled;
        }
    }
}