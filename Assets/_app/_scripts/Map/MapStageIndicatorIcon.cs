using UnityEngine;
using UnityEngine.UI;

namespace Antura.Map
{
    public class MapStageIndicatorIcon : MonoBehaviour
    {
        public StageMapsManager stageMapsManager;
        public int assignedStage = 0;

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

        public void OnClick()
        {
            stageMapsManager.MoveToStageMap(assignedStage);
        }
    }
}