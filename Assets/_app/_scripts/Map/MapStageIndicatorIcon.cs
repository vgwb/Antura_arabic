

using UnityEngine;
using UnityEngine.UI;

namespace _app._scripts.Map
{
    public class MapStageIndicatorIcon : MonoBehaviour
    {
        #region Serialized

        public Color SelectedColor = Color.white;
        public Image ColorizedImage;

        #endregion

        bool initialized;
        Color defColor;

        #region Public Methods

        public void Select(bool doSelect)
        {
            if (!initialized)
            {
                initialized = true;
                defColor = ColorizedImage.color;
            }

            ColorizedImage.color = doSelect ? SelectedColor : defColor;
        }

        #endregion
    }
}