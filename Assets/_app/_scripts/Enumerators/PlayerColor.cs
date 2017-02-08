using UnityEngine;

namespace EA4S
{
    /// <summary>
    /// Color used in player's profile/avatar
    /// </summary>
    public enum PlayerColor
    {
        None = 0,
        Red = 1,
        Orange = 2,
        Yellow = 3,
        Green = 4,
        Blue = 5,
        Purple = 6,
        Fucsia = 7,
        Pink = 8
    }

    public static class PlayerColorConverter
    {
        public static Color ToColor(PlayerColor value)
        {
            switch (value)
            {
                case PlayerColor.Red: return new Color(0.9254903f, 0.1803922f, 0.09411766f, 1f);
                case PlayerColor.Orange: return new Color(0.9176471f, 0.4980392f, 0.1215686f, 1f);
                case PlayerColor.Yellow: return new Color(1f, 0.8509805f, 0.2313726f, 1f);
                case PlayerColor.Green: return new Color(0.2039216f, 0.9019608f, 0.07058824f, 1f);
                case PlayerColor.Blue: return new Color(0.1176471f, 0.4313726f, 0.9450981f, 1f);
                case PlayerColor.Purple: return new Color(0.5803922f, 0.2509804f, 0.9254903f, 1f);
                case PlayerColor.Fucsia: return new Color(0.9333334f, 0.2235294f, 0.882353f, 1f);
                case PlayerColor.Pink: return new Color(1f, 0.4980392f, 0.654902f, 1f);
                default: return Color.white;
            }
        }
    }
}