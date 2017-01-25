namespace EA4S
{
    public enum MenuButtonType
    {
        Unset,
        PauseToggle,
        Continue,
        Back,
        MusicToggle,
        FxToggle,
        Restart,
        Credits
    }

    /// <summary>
    /// A button used in a menu.
    /// </summary>
    public class MenuButton : UIButton
    {
        public MenuButtonType Type;
    }
}
