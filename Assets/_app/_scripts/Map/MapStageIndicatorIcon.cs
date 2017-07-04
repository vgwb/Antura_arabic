using UnityEngine;
using UnityEngine.UI;

namespace EA4S.Map
{
    public class MapStageIndicatorIcon : MonoBehaviour
    {
        public Color SelectedColor = Color.white;
        public Image ColorizedImage;

        bool initialized;
        Color defColor;

        public void Select(bool doSelect)
        {
            if (!initialized) {
                initialized = true;
                defColor = ColorizedImage.color;
            }

            ColorizedImage.color = doSelect ? SelectedColor : defColor;
        }
    }
}